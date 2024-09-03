using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "CommandService", menuName = "TerminalServices/InterpreterServices/CommandService")]
    public class CommandService : InterpreterService
    {
        public override (bool, List<string>) Execute(string userInput)
        {
            return CommandsReader.GetResponses(userInput);
        }
    }
}