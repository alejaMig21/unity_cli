using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI
{
    [RequireComponent(typeof(RectTransform), typeof(TextMeshProUGUI))]
    public class TextAdjuster : MonoBehaviour
    {
        #region FIELDS
        private RectTransform rt;
        private TextMeshProUGUI text;
        #endregion

        #region PROPERTIES
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
        #endregion

        #region METHODS
        private void Awake()
        {
            SubscribeAdjustment();
        }
        private void Start()
        {
            AdjustSize();
        }
        private void OnDestroy()
        {
            UnsubscribeAdjustment();
        }
        private void ON_TEXT_CHANGED(Object obj)
        {
            AdjustSize();
        }
        public void AdjustSize()
        {
            Rt.sizeDelta = Text.GetRenderedValues(true);
        }
        public void SubscribeAdjustment()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
        }
        public void UnsubscribeAdjustment()
        {
            TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
        }
        #endregion
    }
}