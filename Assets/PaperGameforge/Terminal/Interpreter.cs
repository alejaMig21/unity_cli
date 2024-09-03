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
        [SerializeField] private List<InterpreterService> interpreterServices;
        [SerializeField] private List<DecoratorService> decoratorServices;
        #endregion

        #region PROPERTIES
        public FileManager _FileManager => fileManager ??= GetComponent<FileManager>();
        public List<string> Responses { get => responses ??= new(); set => responses = value; }
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
            Responses.Clear();

            // Ejecuta los servicios de interpretación
            foreach (var service in interpreterServices)
            {
                (bool error, List<string> serviceResponses) = service.Execute(userInput);

                if (!error)
                {
                    Responses.AddRange(serviceResponses);
                    break; // Detener la iteración si un servicio interpretó el input
                }
            }

            // Itera sobre cada decorador para decorar las respuestas
            for (int i = 0; i < decoratorServices.Count && Responses.Count > 0; i++)
            {
                DecoratorService service = decoratorServices[i];
                (bool error, List<string> decoResponse) = service.ParseResponse(Responses);

                if (!error)
                {
                    Responses = decoResponse; // Actualizar la lista de respuestas
                }
            }

            return Responses; // Devuelve la lista final de respuestas decoradas
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