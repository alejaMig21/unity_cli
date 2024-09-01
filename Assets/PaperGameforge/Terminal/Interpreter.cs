using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(FileManager))]
    public class Interpreter : MonoBehaviour
    {
        #region FIELDS
        private List<string> responses = new();
        private Dictionary<string, string> colors = new()
        {
            {"black",   "#021b21"},
            {"gray",    "#555d71"},
            {"red",     "#ff5879"},
            {"yellow",  "#f2f1b9"},
            {"blue",    "#9ed9d8"},
            {"purple",  "#d936ff"},
            {"orange",  "#ef5847"}
        };
        private FileManager fileManager;
        #endregion

        #region CONSTANTS
        private const string CHANGE_DIR_COMMAND = "cd";
        private const string PREVIOUS_DIR_COMMAND = "..";
        private const string ASCII_CONST = "ASCII";
        private const string METHOD_CONST = "METHOD";
        private const string INFO_CONST = "INFO";
        private const string CLEAR_CONST = "CLEAR";
        private const string ASCII_TITLE_ASSET = "ascii.txt";
        private const string ERROR_DIR_COMMAND = "ERROR DIR_NOT_FOUND";
        #endregion

        #region EVENTS
        public event Action OnClearStart;
        #endregion

        #region PROPERTIES
        public FileManager _FileManager
        {
            get
            {
                if (fileManager == null)
                {
                    fileManager = GetComponent<FileManager>();
                }
                return fileManager;
            }
        }
        #endregion

        #region METHODS
        public List<string> Interpret(string userInput)
        {
            responses.Clear();

            (bool errorDuringProcessing, List<string> commandResponses) = CommandsReader.GetResponses(userInput);

            if (errorDuringProcessing)
            {
                (bool dirProcessed, string dirResponse) = TryProcessDirectoryRequest(userInput);

                if (!dirProcessed)
                {
                    ProcessMultipleResponses(commandResponses);
                }
            }
            else
            {
                ProcessMultipleResponses(commandResponses);
            }

            return responses;
        }
        private void ProcessMultipleResponses(List<string> commandResponses)
        {
            foreach (var item in commandResponses)
            {
                ProcessResponse(item);
            }
        }
        private void ProcessResponse(string response, string userInput = null)
        {
            string[] args = response.Split(':');

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                switch (commandType)
                {
                    case METHOD_CONST:
                        ExecuteMethodCommand(commandParam);
                        break;
                    case INFO_CONST:
                        ListEntry(commandType, commandParam);
                        break;
                }
            }
            else
            {
                responses.Add(response);
            }
        }
        private (bool error, string response) TryProcessDirectoryRequest(string userInput)
        {
            string[] dirArgs = userInput.Split(' ');

            if (dirArgs.Length == 2 && dirArgs[0] == CHANGE_DIR_COMMAND)
            {
                string dir = dirArgs[1];

                (bool exists, string newPath) = ChangeDirectory(dir);

                if (!exists)
                {
                    Interpret(ERROR_DIR_COMMAND);
                }

                return (true, newPath); // Returns the found path
            }

            return (false, _FileManager.Path); // Returns the current path
        }
        private void ExecuteMethodCommand(string method)
        {
            switch (method)
            {
                case ASCII_CONST:
                    //LoadTitle();
                    GetType().GetMethod("LoadTitle").Invoke(this, new object[0]);
                    break;
                case CLEAR_CONST:
                    OnClearStart?.Invoke();
                    break;
            }
        }
        public string ColorString(string s, string color)
        {
            string leftTag = "<color=" + color + ">";
            string rightTag = "</color>";

            return leftTag + s + rightTag;
        }
        private void ListEntry(string a, string b)
        {
            responses.Add(ColorString(a, colors["orange"]) + ": " + ColorString(b, colors["yellow"]));
        }
        public void LoadTitle()
        {
            LoadAscii(ASCII_TITLE_ASSET, "red", 2);
        }
        private void LoadAscii(string path, string color, int spacing)
        {
            StreamReader file = new(Path.Combine(Application.streamingAssetsPath, path));

            for (int i = 0; i < spacing; i++)
            {
                responses.Add("");
            }

            while (!file.EndOfStream)
            {
                responses.Add(ColorString(file.ReadLine(), colors[color]));
            }

            for (int i = 0; i < spacing; i++)
            {
                responses.Add("");
            }

            file.Close();
        }
        private (bool exists, string newPath) ChangeDirectory(string folderName)
        {
            if (folderName != PREVIOUS_DIR_COMMAND)
            {
                return _FileManager.MoveToDirectory(folderName);
            }
            return _FileManager.MoveToPreviousDirectory();
        }
        #endregion
    }
}