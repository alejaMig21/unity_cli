using Assets.PaperGameforge.Utils.GenericTree;

namespace Assets.PaperGameforge.Terminal.Commands
{
    public class CommandTree : Tree<string>
    {
        public CommandTree(string rootValue) : base(rootValue)
        {
        }
    }
}
