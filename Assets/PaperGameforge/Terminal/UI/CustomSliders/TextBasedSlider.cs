using Assets.PaperGameforge.Terminal.Managers;
using Assets.PaperGameforge.Terminal.Services.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.PaperGameforge.Terminal.UI.CustomSliders
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextBasedSlider : MonoBehaviour
    {
        #region FIELDS
        [Header("Text Settings")]
        private TextMeshProUGUI sliderText;
        [SerializeField] private char completedChar = '█';
        [SerializeField] private char uncompletedChar = '░';
        [SerializeField, Min(1)] private int textLength = 10;

        [Header("Color Settings")]
        [SerializeField] private SColorKey completedColor = new("green", ColorManager.ParseColor("#67ff76"));
        [SerializeField] private SColorKey uncompletedColor = new("red", ColorManager.ParseColor("#e2424a"));

        [Header("Slider Values")]
        [SerializeField, Min(0)] private float minValue = 0f;
        [SerializeField, Min(1)] private float maxValue = 1f;
        [SerializeField, Range(0, 1)] private float percentValue = 0.5f;
        #endregion

        #region EVENTS
        [Header("Events")]
        [SerializeField] private UnityEvent<float> onValueChanged;
        #endregion

        #region PROPERTIES
        public TextMeshProUGUI SliderText => sliderText ??= GetComponent<TextMeshProUGUI>();
        #endregion

        #region METHODS
        private void Start()
        {
            SetValue(percentValue);
        }
#if UNITY_EDITOR
        private void LateUpdate()
        {
            SetValue(percentValue);
        }
#endif
        public void SetValue(float newValue)
        {
            // Ensures the value is always between 0 and 1
            percentValue = Mathf.Clamp01(newValue);

            UpdateSliderText();
            onValueChanged?.Invoke(percentValue * (maxValue - minValue) + minValue); // Returns value in minValue - maxValue scale
        }
        private void UpdateSliderText()
        {
            // Calculate percentage of completed characters based on current value
            int completedCount = Mathf.RoundToInt(percentValue * textLength); // `percentValue` is between 0 and 1

            // Draw slider info based on defined characters
            string completedPart = new(completedChar, completedCount);
            string uncompletedPart = new(uncompletedChar, textLength - completedCount);

            // Enriching slider text info
            string hex = "#" + ColorManager.ParseHex(completedColor.Color);
            string enrichedCompletedText = ColorManager.ColorString(completedPart, hex);
            hex = "#" + ColorManager.ParseHex(uncompletedColor.Color);
            string enrichedUncompletedText = ColorManager.ColorString(uncompletedPart, hex);

            // Update slider text
            SliderText.text = enrichedCompletedText + enrichedUncompletedText;
        }
        #endregion
    }
}