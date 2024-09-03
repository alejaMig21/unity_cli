using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "TextFormatterService", menuName = "TerminalServices/ResponseServices/TextFormatterService")]
    public class TextFormatterService : DecoratorService
    {
        private Dictionary<string, string> colors = new()
        {
            {"black",   "#021b21"},
            {"gray",    "#555d71"},
            {"red",     "#ff5879"},
            {"yellow",  "#f2f1b9"},
            {"blue",    "#9ed9d8"},
            {"purple",  "#d936ff"},
            {"orange",  "#ef5847"}
        };
        public Dictionary<string, string> Colors { get => colors; set => colors = value; }

        public string ColorString(string s, string color)
        {
            string leftTag = "<color=" + color + ">";
            string rightTag = "</color>";

            return leftTag + s + rightTag;
        }
        public List<string> ListEntry(string header, string headerColor, List<string> textsToDecorate, string bodyColor)
        {
            List<string> decoratedTexts = new();

            foreach (string s in textsToDecorate)
            {
                decoratedTexts.Add(ColorString(header, headerColor) + ": " + ColorString(s, bodyColor));
            }

            return decoratedTexts;
        }
    }
}