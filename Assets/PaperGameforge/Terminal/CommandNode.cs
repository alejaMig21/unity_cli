using System.Collections.Generic;

namespace Assets.PaperGameforge.Terminal
{
    public class CommandNode
    {
        public string Response { get; set; }
        public Dictionary<string, List<string>> AvailableArgs { get; set; }

        public CommandNode(string response = null, Dictionary<string, List<string>> availableArgs = null)
        {
            Response = response;
            AvailableArgs = availableArgs;
        }
    }
}
