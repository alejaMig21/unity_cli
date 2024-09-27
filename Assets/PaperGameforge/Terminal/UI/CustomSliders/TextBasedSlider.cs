using TMPro;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.CustomSliders
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextBasedSlider : MonoBehaviour
    {
        #region FIELDS
        [Header("Text Settings")]
        private TextMeshProUGUI sliderText;
        [SerializeField] private TextBar bar;
        #endregion

        #region PROPERTIES
        public TextMeshProUGUI SliderText => sliderText ??= GetComponent<TextMeshProUGUI>();
        public TextBar Bar { get => bar; set => bar = value; }
        #endregion

        #region METHODS
        private void Start()
        {
            bar.SetValue(bar.PercentValue);
        }
#if UNITY_EDITOR
        private void LateUpdate()
        {
            bar.SetValue(bar.PercentValue);
        }
#endif
        public void UpdateText()
        {
            sliderText.text = bar.GenerateText();
        }
        #endregion
    }
}