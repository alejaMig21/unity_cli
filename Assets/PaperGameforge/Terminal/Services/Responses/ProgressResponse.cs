using Assets.PaperGameforge.Terminal.UI.CustomSliders;
using Assets.PaperGameforge.Terminal.UI.InfiniteScroller;
using Gpm.Ui;
using System;
using System.Collections;

namespace Assets.PaperGameforge.Terminal.Services.Responses
{
    public class ProgressResponse : ServiceResponse
    {
        private TextBar textBar;
        private InGameCopier copier;

        public event Action OnProgressStarted;
        public event Action OnProgressFinished;

        public TextBar TextBar { get => textBar; set => textBar = value; }

        public ProgressResponse(TextBar textBar, InGameCopier copier, bool backgroundProcess = true) :
            base(textBar.GenerateText(), backgroundProcess)
        {
            this.textBar = new(
                textBar.CompletedChar,
                textBar.UncompletedChar,
                textBar.TextLength,
                textBar.CompletedColor,
                textBar.UncompletedColor,
                textBar.MinValue,
                textBar.MaxValue,
                0f
                );
            this.copier = copier;
        }
        public IEnumerator SimulateProgress(InfiniteScroll scroller)
        {
            OnProgressStarted?.Invoke();

            while (textBar.PercentValue < 1f)
            {
                textBar.PercentValue = copier.PercentageProgress;

                int lastIndex = scroller.GetDataCount() - 1;
                var lastData = scroller.GetData(lastIndex) as TerminalData;
                if (lastData != null)
                {
                    lastData.DataText = textBar.GenerateText() + " " + copier.CurrentSize + "/" + copier.TotalSize + " MB" + " SPEED " + " TIME";
                }
                scroller.UpdateData(lastData);

                yield return null; // Wait for the next frame
            }

            OnProgressFinished?.Invoke();
        }
    }
}