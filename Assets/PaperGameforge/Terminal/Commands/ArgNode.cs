using Assets.PaperGameforge.Utils.GenericTree;

namespace Assets.PaperGameforge.Terminal.Commands
{
    public class ArgNode : TreeNode<string>
    {
        public ArgNode(string value) : base(value)
        {
        }
    }
}
