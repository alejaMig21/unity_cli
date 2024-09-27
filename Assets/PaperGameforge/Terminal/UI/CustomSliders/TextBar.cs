using Assets.PaperGameforge.Terminal.Managers;
using Assets.PaperGameforge.Terminal.Services.Structs;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.PaperGameforge.Terminal.UI.CustomSliders
{
    [Serializable]
    public class TextBar
    {
        #region FIELDS
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

        #region PROPERTIES
        public float PercentValue
        {
            get => percentValue;
            set
            {
                percentValue = Mathf.Clamp01(value);
            }
        }
        public char CompletedChar { get => completedChar; set => completedChar = value; }
        public char UncompletedChar { get => uncompletedChar; set => uncompletedChar = value; }
        public int TextLength { get => textLength; set => textLength = value; }
        public SColorKey CompletedColor { get => completedColor; set => completedColor = value; }
        public SColorKey UncompletedColor { get => uncompletedColor; set => uncompletedColor = value; }
        public float MinValue { get => minValue; set => minValue = value; }
        public float MaxValue { get => maxValue; set => maxValue = value; }
        #endregion

        #region EVENTS
        [Header("Events")]
        [SerializeField] private UnityEvent<float> onValueChanged;
        #endregion

        #region CONSTRUCTOR
        public TextBar(char completedChar, char uncompletedChar, int textLength, SColorKey completedColor, SColorKey uncompletedColor, float minValue, float maxValue, float percentValue)
        {
            this.completedChar = completedChar;
            this.uncompletedChar = uncompletedChar;
            this.textLength = textLength;
            this.completedColor = completedColor;
            this.uncompletedColor = uncompletedColor;
            this.minValue = minValue;
            this.maxValue = maxValue;
            this.percentValue = percentValue;
        }
        #endregion

        #region METHODS        
        public void SetValue(float newValue)
        {
            // Ensures the value is always between 0 and 1
            percentValue = Mathf.Clamp01(newValue);

            //UpdateSliderText();
            onValueChanged?.Invoke(percentValue * (maxValue - minValue) + minValue); // Returns value in minValue - maxValue scale
        }
        public string GenerateText()
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

            return enrichedCompletedText + enrichedUncompletedText;
        }
        #endregion
    }
}