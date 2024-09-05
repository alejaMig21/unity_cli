using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "MethodExecuterService", menuName = "TerminalServices/InterpreterServices/MethodExecuterService")]
    public class MethodExecuterService : DecoratorService
    {
        private const string I_METHOD_CONST = "<IMETHOD>";
        private List<ITerminalService> services = new();

        public void SetUpValues(List<ITerminalService> services)
        {
            this.services = services;
        }

        public override (bool, List<ServiceResponse>) ProcessResponse(ServiceResponse response, string userInput = null)
        {
            string[] args = response.Text.Split(TWO_DOTS_SEPARATOR);

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                if (commandType.Equals(I_METHOD_CONST) || commandType.Equals(D_METHOD_CONST))
                {
                    return ExecuteMethodCommand(commandParam, services);
                }
            }

            return (true, null);
        }
        public (bool, List<ServiceResponse>) ExecuteMethodCommand(string method, List<ITerminalService> services)
        {
            List<ServiceResponse> responses = new();

            // Iterar sobre cada instancia en la lista
            foreach (var service in services)
            {
                // Obtener el tipo de la instancia actual
                var serviceType = service.GetType();

                // Intentar obtener el método a través de reflexión
                var methodInfo = serviceType.GetMethod(method);

                if (methodInfo != null) // Verificar si el método existe
                {
                    // Invocar el método en la instancia actual
                    var result = methodInfo.Invoke(service, new object[0]);

                    // Verificar si el resultado es una lista de strings
                    if (result is List<ServiceResponse> stringList)
                    {
                        responses.AddRange(stringList);
                    }

                    return (false, responses);
                }
            }

            return (true, responses);
        }
    }
}