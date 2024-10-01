namespace Assets.PaperGameforge.Utils.GenericTree
{
    public class Tree<T>
    {
        public TreeNode<T> Root { get; set; }

        public Tree(T rootValue)
        {
            Root = new TreeNode<T>(rootValue);
        }
    }
}