using Assets.PaperGameforge.Terminal.Services;
using Assets.PaperGameforge.Terminal.Services.Interfaces;
using Assets.PaperGameforge.Terminal.Services.Responses;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(FileManager))]
    public class Interpreter : MonoBehaviour
    {
        #region FIELDS
        private List<ServiceResponse> responses = new();
        private FileManager fileManager;
        [SerializeField] private List<InterpreterService> interpreterServices;
        [SerializeField] private List<DecoratorService> decoratorServices;
        [SerializeField] private ErrorHandlerService errorHandlerService;
        #endregion

        #region PROPERTIES
        public FileManager _FileManager => fileManager ??= GetComponent<FileManager>();
        public List<ServiceResponse> Responses { get => responses ??= new(); set => responses = value; }
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
                        directoryService.SetUpValues(_FileManager);
                        break;
                    case MethodExecuterService methodExecuterService:
                        methodExecuterService.SetUpValues(GetServices());
                        break;
                }
            }
        }
        public List<ServiceResponse> Interpret(string userInput)
        {
            Responses.Clear();
            List<ServiceError> errorResponses = new();

            // Run interpretation services
            for (int i = 0; i < interpreterServices.Count; i++)
            {
                InterpreterService service = interpreterServices[i];
                var serviceResponses = service.Execute(userInput);

                if (serviceResponses == null)
                {
                    continue;
                }

                if (serviceResponses.All(item => item is not ServiceError))
                {
                    Responses.AddRange(serviceResponses);
                    break; // Detener la iteración si un servicio interpretó el input
                }
                else
                {
                    errorResponses.AddRange(serviceResponses);
                }
            }

            // Analyzes responses in search of possible errors
            List<ServiceResponse> priorErrors = new();
            if (Responses.Count <= 0 && errorResponses.Count > 0)
            {
                priorErrors = PrioritizeError(errorResponses);
                Responses.Clear();
                Responses = errorHandlerService.Execute(priorErrors);
            }

            // Run decoration services
            for (int i = 0; i < decoratorServices.Count && Responses.Count > 0; i++)
            {
                DecoratorService service = decoratorServices[i];
                var decoResponse = service.ParseResponse(Responses);
                Responses = decoResponse; // Now responses are decorated
            }

            return Responses;
        }
        private List<ServiceResponse> PrioritizeError(List<ServiceError> errorResponses)
        {
            List<ServiceResponse> priorErrors = new();
            int priority = int.MaxValue;

            foreach (var response in errorResponses)
            {
                if (response.Priority < priority)
                {
                    priorErrors.Clear();
                    priorErrors.Add(response);
                    priority = response.Priority;
                }
                else if (response.Priority == priority)
                {
                    priorErrors.Add(response);
                }
            }

            return priorErrors;
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