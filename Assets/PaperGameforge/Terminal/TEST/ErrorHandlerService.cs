using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "ErrorHandlerService", menuName = "TerminalServices/InterpreterServices/ErrorHandlerService")]
    public class ErrorHandlerService : CommandService
    {
        private readonly ErrorKey baseError = new("ERROR");

        public override (bool, List<ServiceResponse>) Execute(string userInput = null)
        {
            if (userInput == null)
            {
                List<ServiceResponse> errorResponses = GenerateServiceResponses(
                    CommandsReader.GetResponses(baseError.Cmd).responses,
                    false
                    );

                return (true, errorResponses); // Interpretación fallida
            }

            var (cmd_error, commandResponses) = CommandsReader.GetResponses(userInput);

            if (cmd_error)
            {
                List<ServiceResponse> errorResponses = GenerateServiceResponses(
                    CommandsReader.GetResponses(baseError.Cmd).responses,
                    false
                    );

                return (true, errorResponses); // Interpretación fallida
            }

            List<ServiceResponse> finalResponses = GenerateServiceResponses(commandResponses, false);

            return (false, finalResponses); // Interpretación exitosa
        }
    }
}