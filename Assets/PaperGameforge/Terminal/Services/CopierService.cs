using Assets.PaperGameforge.Terminal.Services.Responses;
using Assets.PaperGameforge.Terminal.UI.CustomSliders;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    [CreateAssetMenu(fileName = "CopierService", menuName = "TerminalServices/InterpreterServices/CopierService")]
    public class CopierService : InterpreterService
    {
        private readonly ErrorKey errorSourceCmd = new("ERROR SOURCE_NOT_FOUND");
        private readonly ErrorKey errorDestCmd = new("ERROR DEST_NOT_FOUND");
        private readonly ErrorKey errorConflictFilesCmd = new("ERROR CONFLICT_DEST_FILES");
        private FileManager fileManager;
        [SerializeField] private TextBar textBar;

        private const string MOVE_DIR_COMMAND = "move";
        private const string HARD_COMMAND = "--hard";
        private const char WHITE_SAPACE_SEPARATOR = ' ';
        private const int ERROR_PRIORITY = 9;

        public void SetUpValues(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }
        public override List<ServiceResponse> Execute(string userInput)
        {
            string[] dirArgs = userInput.Split(WHITE_SAPACE_SEPARATOR);

            if (dirArgs.Length >= 3)
            {
                if (dirArgs[0] == MOVE_DIR_COMMAND)
                {
                    // Check for hard executions
                    bool hard = dirArgs[dirArgs.Length - 1] == HARD_COMMAND;

                    (bool sourceExists, bool destExists) = fileManager.Copier.Copy(dirArgs[1], dirArgs[2], hard);

                    if (!sourceExists)
                    {
                        return new() { new ServiceError(errorSourceCmd.Cmd, false, ERROR_PRIORITY) }; // Launch error
                    }
                    if (!destExists)
                    {
                        return new() { new ServiceError(errorDestCmd.Cmd, false, ERROR_PRIORITY) }; // Launch error
                    }

                    var operation = dirArgs[1] + " -> " + dirArgs[2];

                    return new() {
                        new(operation, false),
                        new ProgressResponse(textBar, fileManager.Copier, false)
                    }; // Else do not launch error
                }
            }

            return null; // Launch error
        }
        public override List<ServiceResponse> Execute(List<string> userInput)
        {
            List<ServiceResponse> results = new();

            foreach (string dir in userInput)
            {
                results.AddRange(Execute(dir));
            }

            return results;
        }
    }
}