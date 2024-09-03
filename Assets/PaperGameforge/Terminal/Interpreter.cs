using Assets.PaperGameforge.Terminal.TEST;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(FileManager))]
    public class Interpreter : MonoBehaviour
    {
        #region FIELDS
        private List<string> responses = new();
        private FileManager fileManager;
        [SerializeField] private CommandService commandService;
        [SerializeField] private DirectoryService directoryService;
        [SerializeField] private TextFormatterService textFormatterService;
        [SerializeField] private AsciiLoaderService asciiLoaderService;
        [SerializeField] private CleanerService cleanerService;
        [SerializeField] private MethodExecuterService methodExecuterService;
        [SerializeField] private InformationService informationService;

        [SerializeField] private List<InterpreterService> interpreterServices;
        [SerializeField] private List<ResponseService> responseServices;
        #endregion

        #region CONSTANTS
        private const string METHOD_CONST = "METHOD";
        private const string INFO_CONST = "INFO";
        private const char TWO_DOTS_SEPARATOR = ':';
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
        private void Awake()
        {
            directoryService.SetUpValues(_FileManager, this);
            asciiLoaderService.SetUpValues(textFormatterService);
        }
        public List<string> Interpret(string userInput)
        {
            responses.Clear();

            (bool cmd_error, List<string> commandResponses) = commandService.Execute(userInput);

            if (cmd_error)
            {
                (bool dir_error, List<string> dirResponse) = directoryService.TryProcessDirectoryRequest(userInput, this, _FileManager);

                if (!dir_error)
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
            string[] args = response.Split(TWO_DOTS_SEPARATOR);

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                switch (commandType)
                {
                    case METHOD_CONST:
                        responses.AddRange(methodExecuterService.ExecuteMethodCommand(commandParam, GetServices()));
                        break;
                    case INFO_CONST:
                        responses.AddRange(informationService.ListEntry(commandType, commandParam));
                        break;
                }
            }
            else
            {
                responses.Add(response);
            }
        }
        public List<ITerminalService> GetServices()
        {
            List<ITerminalService> services = new() {
                commandService,
                directoryService,
                textFormatterService,
                asciiLoaderService,
                cleanerService
            };

            return services;
        }
        #endregion
    }
}