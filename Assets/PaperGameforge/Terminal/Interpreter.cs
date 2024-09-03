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
        private List<string> decoratedResponses = new();
        private FileManager fileManager;
        [SerializeField] private List<InterpreterService> interpreterServices;
        [SerializeField] private List<DecoratorService> decoratorServices;
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
            SetUpValues(interpreterServices);
            SetUpValues(decoratorServices);
        }
        private void SetUpValues<T>(List<T> list) where T : ITerminalService
        {
            foreach (var item in list)
            {
                switch (item)
                {
                    case DirectoryService directoryService:
                        directoryService.SetUpValues(_FileManager, this);
                        break;
                    case MethodExecuterService methodExecuterService:
                        methodExecuterService.SetUpValues(GetServices());
                        break;
                }
            }
        }
        public List<string> Interpret(string userInput)
        {
            responses.Clear();
            decoratedResponses.Clear();

            foreach (var service in interpreterServices)
            {
                (bool error, List<string> serviceResponses) = service.Execute(userInput);

                if (!error)
                {
                    responses.AddRange(serviceResponses);

                    break; // Detener la iteración si un servicio interpretó el input
                }
            }

            // Decorate responses
            for (int i = 0; i < decoratorServices.Count; i++)
            {
                DecoratorService service = decoratorServices[i];
                (bool error, List<string> decoResponse) = service.ParseResponse(responses);

                if (error)
                {
                    continue;
                }
                decoratedResponses.AddRange(decoResponse);
            }

            return decoratedResponses;
        }
        public List<ITerminalService> GetServices()
        {
            List<ITerminalService> services = new();

            services.AddRange(interpreterServices);
            services.AddRange(decoratorServices);

            return services;
        }
        #endregion
    }
}