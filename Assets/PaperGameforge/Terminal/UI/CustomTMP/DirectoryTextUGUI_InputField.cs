using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.CustomTMP
{
    public class DirectoryTextUGUI_InputField : TMP_InputField
    {
        private RectTransform rt;

        public RectTransform Rt
        {
            get
            {
                if (rt == null)
                {
                    rt = GetComponent<RectTransform>();
                }
                return rt;
            }
        }

        protected override void Start()
        {
            base.Start();
            this.interactable = false;
            this.onValueChanged.AddListener(
                delegate
                {
                    AdjustSize();
                }
                );
        }

        private void AdjustSize()
        {
            Rt.sizeDelta = this.textComponent.GetRenderedValues(true);
        }
    }
}