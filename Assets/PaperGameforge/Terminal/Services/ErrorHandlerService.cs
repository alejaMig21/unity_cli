using Assets.PaperGameforge.Terminal.Services.Responses;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    [CreateAssetMenu(fileName = "ErrorHandlerService", menuName = "TerminalServices/InterpreterServices/ErrorHandlerService")]
    public class ErrorHandlerService : CommandService
    {
        private readonly ErrorKey baseError = new("ERROR");

        public override List<ServiceResponse> Execute(string userInput = null)
        {
            if (userInput == null)
            {
                List<ServiceResponse> errorResponses = GenerateServiceResponses(
                    CommandsReader.GetResponses(baseError.Cmd).responses,
                    false
                    );

                return errorResponses; // Interpretación fallida
            }

            var (cmd_error, commandResponses) = CommandsReader.GetResponses(userInput);

            if (cmd_error)
            {
                List<ServiceResponse> errorResponses = GenerateServiceResponses(
                    CommandsReader.GetResponses(baseError.Cmd).responses,
                    false
                    );

                return errorResponses; // Interpretación fallida
            }

            List<ServiceResponse> finalResponses = GenerateServiceResponses(commandResponses, false);

            return finalResponses; // Interpretación exitosa
        }
        public List<ServiceResponse> Execute<T>(List<T> errors) where T : ServiceResponse
        {
            List<ServiceResponse> responses = new();

            foreach (T error in errors)
            {
                responses.AddRange(this.Execute(error.Text));
            }

            return responses;
        }
    }
}