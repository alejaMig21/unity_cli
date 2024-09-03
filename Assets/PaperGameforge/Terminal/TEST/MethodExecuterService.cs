using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "MethodExecuterService", menuName = "TerminalServices/InterpreterServices/MethodExecuterService")]
    public class MethodExecuterService : InterpreterService
    {
        public override (bool, List<string>) Execute(string userInput)
        {
            throw new System.NotImplementedException();
        }
        public List<string> ExecuteMethodCommand(string method, List<ITerminalService> services)
        {
            List<string> responses = new();

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
                    if (result is List<string> stringList)
                    {
                        responses.AddRange(stringList);
                    }

                    break;
                }
            }

            return responses;
        }
    }
}