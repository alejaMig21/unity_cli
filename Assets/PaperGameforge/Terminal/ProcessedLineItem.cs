using Assets.PaperGameforge.Terminal.UI.InfiniteScroller;
using Gpm.Ui;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    public class ProcessedLineItem : TerminalItem
    {
        #region FIELDS
        [SerializeField] private ProcessedLineData data;
        #endregion

        #region PROPERTIES
        public ProcessedLineData Data { get => data; set => data = value; }
        #endregion

        #region CONSTRUCTOR
        public ProcessedLineItem(string directory, string userInput)
        {
            this.data = new(directory, userInput);
            CreateTextInfo();
        }
        #endregion

        #region METHODS
        //private void OnEnable()
        //{
        //    CreateTextInfo();
        //}
        public void CreateTextInfo(string directory = null, string userInput = null)
        {
            if (directory != null)
            {
                this.data.Directory = directory;
            }
            if (userInput != null)
            {
                this.data.UserInput = userInput;
            }

            Text.text = $"{this.data.Directory} {this.data.UserInput}";
        }
        public override void UpdateData(InfiniteScrollData scrollData)
        {
            base.UpdateData(scrollData);

            switch (scrollData)
            {
                case ProcessedLineData pld:
                    CreateTextInfo(pld.Directory, pld.UserInput);
                    break;
                case TerminalData td:
                    Text.text = td.DataText;
                    break;
            }
        }
        #endregion
    }
}