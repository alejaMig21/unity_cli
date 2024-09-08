using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CommandService", menuName = "TerminalServices/InterpreterServices/CommandService")]
    public class CommandService : InterpreterService
    {
        private readonly ErrorKey notFound = new("ERROR NOT_FOUND");
        private const int ERROR_PRIORITY = 10;

        public override List<ServiceResponse> Execute(string userInput = null)
        {
            if (userInput == null)
            {
                return new() { new(notFound.Cmd, false) }; // Interpretación fallida
            }

            var (cmd_error, commandResponses) = CommandsReader.GetResponses(userInput);

            if (cmd_error)
            {
                return new() { new ServiceError(notFound.Cmd, false, ERROR_PRIORITY) }; // Interpretación fallida
            }

            List<ServiceResponse> finalResponses = GenerateServiceResponses(commandResponses, false);

            return finalResponses; // Interpretación exitosa
        }
        public override List<ServiceResponse> Execute(List<string> userInput)
        {
            List<ServiceResponse> results = new();

            foreach (string cmd in userInput)
            {
                results.AddRange(Execute(cmd));
            }

            return results;
        }
        protected static List<ServiceResponse> GenerateServiceResponses(List<string> commandResponses, bool visibility)
        {
            List<ServiceResponse> finalResponses = new();
            foreach (var response in commandResponses)
            {
                finalResponses.Add(new(response, visibility));
            }

            return finalResponses;
        }
    }
}