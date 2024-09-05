using Assets.PaperGameforge.Terminal.TEST;
using System.Collections.Generic;
using System.Linq;
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
                        directoryService.SetUpValues(_FileManager, this);
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
            List<ServiceResponse> errorResponses = new();

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

                errorResponses.AddRange(serviceResponses);
            }

            // Analyzes responses in search of possible errors
            List<ServiceResponse> priorErrors = new();

            if (Responses.Count <= 0 && errorResponses.Count > 0)
            {
                int priority = int.MaxValue;

                foreach (var response in errorResponses)
                {
                    if (response is ServiceError foundError)
                    {
                        if (foundError.Priority < priority)
                        {
                            priorErrors.Clear();
                            priorErrors.Add(foundError);
                            priority = foundError.Priority;
                        }
                        else if (foundError.Priority == priority)
                        {
                            priorErrors.Add(foundError);
                        }
                    }
                }

                Responses.Clear();
                foreach (var item in priorErrors)
                {
                    List<ServiceResponse> errorMessages = errorHandlerService.Execute(item.Text);
                    Responses.AddRange(errorMessages);
                }
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