using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "DirectoryService", menuName = "TerminalServices/InterpreterServices/DirectoryService")]
    public class DirectoryService : InterpreterService
    {
        private const string CHANGE_DIR_COMMAND = "cd";
        private const string ERROR_DIR_COMMAND = "ERROR DIR_NOT_FOUND";
        private const string PREVIOUS_DIR_COMMAND = "..";
        private const char WHITE_SAPACE_SEPARATOR = ' ';
        private FileManager fileManager;
        private Interpreter interpreter;

        public void SetUpValues(FileManager fileManager, Interpreter interpreter)
        {
            this.fileManager = fileManager;
            this.interpreter = interpreter;
        }

        public override (bool, List<string>) Execute(string userInput)
        {
            return TryProcessDirectoryRequest(userInput, interpreter, fileManager);
        }
        public (bool, List<string>) TryProcessDirectoryRequest(
            string userInput,
            Interpreter interpreter,
            FileManager fileManager
            )
        {
            string[] dirArgs = userInput.Split(WHITE_SAPACE_SEPARATOR);

            if (dirArgs[0] == CHANGE_DIR_COMMAND)
            {
                // Join all arguments after the command into a single string for the directory
                string dir = string.Join(WHITE_SAPACE_SEPARATOR, dirArgs.Skip(1));

                (bool exists, string newPath) = ChangeDirectory(dir, fileManager);

                if (!exists)
                {
                    interpreter.Interpret(ERROR_DIR_COMMAND);
                }

                return (true, new List<string> { newPath }); // Returns the found path
            }

            return (false, new List<string> { fileManager.Path }); // Returns the current path
        }
        private (bool exists, string newPath) ChangeDirectory(string folderName, FileManager fileManager)
        {
            if (folderName != PREVIOUS_DIR_COMMAND)
            {
                return fileManager.MoveToDirectory(folderName);
            }
            return fileManager.MoveToPreviousDirectory();
        }
        public List<string> LoadDirectories()
        {
            List<string> responses = new();
            responses.AddRange(fileManager.LoadDirData());

            return responses;
        }
        public List<string> LoadFolders()
        {
            List<string> responses = new();
            responses.AddRange(fileManager.LoadFolders());

            return responses;
        }
    }
}