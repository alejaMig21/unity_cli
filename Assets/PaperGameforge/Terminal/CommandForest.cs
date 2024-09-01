using System.Collections.Generic;

namespace Assets.PaperGameforge.Terminal
{
    public class CommandForest
    {
        public List<CommandNode> Forest { get; set; }

        public CommandForest(List<CommandNode> forest)
        {
            Forest = forest;
        }
    }
}
