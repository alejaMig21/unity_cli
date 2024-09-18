using Gpm.Ui;
using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.InfiniteScroller
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TerminalItem : InfiniteScrollItem
    {
        private TextMeshProUGUI text;

        public TextMeshProUGUI Text
        {
            get
            {
                if (text == null)
                {
                    text = GetComponent<TextMeshProUGUI>();
                }
                return text;
            }
        }

        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            TerminalData td = scrollData as TerminalData;

            Text.text = td.DataText;
        }
    }
}