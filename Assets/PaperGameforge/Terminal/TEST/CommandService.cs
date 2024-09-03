using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CommandService", menuName = "TerminalServices/InterpreterServices/CommandService")]
    public class CommandService : InterpreterService
    {
        public override (bool, List<string>) Execute(string userInput)
        {
            var (cmd_error, commandResponses) = CommandsReader.GetResponses(userInput);

            if (commandResponses == null)
            {
                commandResponses = new();
            }

            return (cmd_error, commandResponses); // Interpretación exitosa
        }
    }
}