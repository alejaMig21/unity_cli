using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "InformationService", menuName = "TerminalServices/ResponseServices/InformationService")]
    public class InformationService : TextFormatterService
    {
        private const string INFO_CONST = "INFO";

        public override List<ServiceResponse> ProcessResponse(ServiceResponse response, string userInput = null)
        {
            string[] args = response.Text.Split(TWO_DOTS_SEPARATOR);

            if (args.Length > 1)
            {
                string commandType = args[0];
                string commandParam = args[1];

                if (commandType.Equals(INFO_CONST))
                {
                    return null; ///// FIX
                }
            }

            return new() { response };
        }
    }
}