using Assets.PaperGameforge.Terminal.UI.InfiniteScroller;
using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.HierarchicalVisitor
{
    public class TerminalVisitor : HierarchicalVisitor<ProcessedLineData>
    {
        #region FIELDS
        [SerializeField] private Terminal terminal;
        [SerializeField] private TMP_InputField userInput;
        #endregion

        #region PROPERTIES
        public override int CurrentIndex
        {
            get => currentIndex;
            set
            {
                currentIndex = value;

                if (currentIndex >= 0 && currentIndex < Elements.Count)
                {
                    SelectedElement = Elements[currentIndex];
                }
            }
        }
        public override ProcessedLineData SelectedElement
        {
            get => selectedElement;
            set
            {
                selectedElement = value;

                if (userInput != null)
                {
                    userInput.text = selectedElement.UserInput;
                }
            }
        }
        #endregion

        #region METHODS
        private void Awake()
        {
            if (terminal != null)
            {
                Elements = terminal.PLines;

                terminal.OnLineProcessed += ResetCurrentIndex;
            }
        }
        private void OnDestroy()
        {
            if (terminal != null)
            {
                terminal.OnLineProcessed -= ResetCurrentIndex;
            }
        }
        private void ResetCurrentIndex()
        {
            CurrentIndex = IterationLimit();
        }
        #endregion
    }
}