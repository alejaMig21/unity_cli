using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.CustomTMP
{
    [RequireComponent(typeof(TextAdjuster))]
    public class DirectoryTextUGUI : TextMeshProUGUI
    {
        private TextAdjuster adjuster;

        public TextAdjuster Adjuster
        {
            get
            {
                if (adjuster == null)
                {
                    adjuster = GetComponent<TextAdjuster>();
                }
                return adjuster;
            }
        }

        public void Adjust()
        {
            Adjuster.AdjustSize();
        }
        public void StartAdjustment()
        {
            Adjuster.SubscribeAdjustment();
        }
        public void StopAdjustment()
        {
            Adjuster.UnsubscribeAdjustment();
        }
    }
}