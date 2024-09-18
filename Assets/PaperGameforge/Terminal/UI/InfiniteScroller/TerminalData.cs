using Gpm.Ui;

namespace Assets.PaperGameforge.Terminal.UI.InfiniteScroller
{
    public class TerminalData : InfiniteScrollData
    {
        private string dataText = string.Empty;

        public string DataText { get => dataText; set => dataText = value; }

        public TerminalData(string text)
        {
            this.dataText = text;
        }
    }
}
