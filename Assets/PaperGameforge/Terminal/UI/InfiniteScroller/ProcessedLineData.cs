using System;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.InfiniteScroller
{
    [Serializable]
    public class ProcessedLineData : TerminalData
    {
        [SerializeField] private string directory = string.Empty;
        [SerializeField] private string userInput = string.Empty;

        public string Directory { get => directory; set => directory = value; }
        public string UserInput { get => userInput; set => userInput = value; }

        public ProcessedLineData(string directory, string userInput) : base(directory + userInput)
        {
            this.directory = directory;
            this.userInput = userInput;
        }
    }
}
