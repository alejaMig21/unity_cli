using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "DirectoryService", menuName = "TerminalServices/InterpreterServices/DirectoryService")]
    public class DirectoryService : InterpreterService
    {

        private readonly ErrorKey errorDirCmd = new("ERROR DIR_NOT_FOUND");
        private FileManager fileManager;
        private Interpreter interpreter;

        private const string CHANGE_DIR_COMMAND = "cd";
        private const string PREVIOUS_DIR_COMMAND = "..";
        private const char WHITE_SAPACE_SEPARATOR = ' ';
        private const int ERROR_PRIORITY = 9;

        public void SetUpValues(FileManager fileManager, Interpreter interpreter)
        {
            this.fileManager = fileManager;
            this.interpreter = interpreter;
        }
        public override List<ServiceResponse> Execute(string userInput)
        {
            string[] dirArgs = userInput.Split(WHITE_SAPACE_SEPARATOR);

            if (dirArgs[0] == CHANGE_DIR_COMMAND)
            {
                // Join all arguments after the command into a single string for the directory
                string dir = string.Join(WHITE_SAPACE_SEPARATOR, dirArgs.Skip(1));

                (bool exists, string _) = ChangeDirectory(dir, fileManager);

                if (!exists) // If it does not exists
                {
                    return new() { new ServiceError(errorDirCmd.Cmd, false, ERROR_PRIORITY) }; // Launch error
                }

                return new() { new(string.Empty, true) }; // Else do not launch error
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
        private (bool exists, string newPath) ChangeDirectory(string folderName, FileManager fileManager)
        {
            if (folderName != PREVIOUS_DIR_COMMAND)
            {
                return fileManager.MoveToDirectory(folderName);
            }
            return fileManager.MoveToPreviousDirectory();
        }
        public List<ServiceResponse> LoadDirectories()
        {
            List<ServiceResponse> responses = new();

            foreach (var item in fileManager.LoadDirData())
            {
                responses.Add(new(item, false));
            }

            return responses;
        }
        public List<ServiceResponse> LoadFolders()
        {
            List<ServiceResponse> responses = new();

            foreach (var item in fileManager.LoadFolders())
            {
                responses.Add(new(item, false));
            }

            return responses;
        }
    }
}