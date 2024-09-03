using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "InformationService", menuName = "TerminalServices/ResponseServices/InformationService")]
    public class InformationService : TextFormatterService
    {
        private const string INFO_CONST = "INFO";

        public override (bool, List<string>) ProcessResponse(string response, string userInput = null)
        {
            string[] args = response.Split(TWO_DOTS_SEPARATOR);

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                if (commandType.Equals(INFO_CONST))
                {
                    return (true, null); ///// FIX
                }
            }

            return (true, new() { response });
        }
    }
}