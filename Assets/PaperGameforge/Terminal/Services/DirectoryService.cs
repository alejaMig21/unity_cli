﻿using Assets.PaperGameforge.Terminal.Services.Responses;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    [CreateAssetMenu(fileName = "DirectoryService", menuName = "TerminalServices/InterpreterServices/DirectoryService")]
    public class DirectoryService : InterpreterService
    {
        private readonly ErrorKey errorDirCmd = new("ERROR DIR_NOT_FOUND");
        private readonly ErrorKey errorIncompleteCmd = new("cd");
        private FileManager fileManager;

        private const string CHANGE_DIR_COMMAND = "cd";
        private const string PREVIOUS_DIR_COMMAND = "..";
        private const char WHITE_SAPACE_SEPARATOR = ' ';
        private const int ERROR_PRIORITY = 8;

        public void SetUpValues(FileManager fileManager)
        {
            this.fileManager = fileManager;
        }
        public override List<ServiceResponse> Execute(string userInput)
        {
            string[] dirArgs = userInput.Split(WHITE_SAPACE_SEPARATOR);

            if (dirArgs.Length == 2)
            {
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
            }
            else if (dirArgs.Length == 1 && dirArgs[0] == CHANGE_DIR_COMMAND)
            {
                return new() { new ServiceError(errorIncompleteCmd.Cmd, false, ERROR_PRIORITY) }; // Launch error
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