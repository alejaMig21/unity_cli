using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Managers
{
    public static class ColorManager
    {
        public static Color ParseColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            else
            {
                Debug.LogError($"Formato de color hexadecimal inválido: {hex}");
                return Color.white; // Devuelve blanco como color predeterminado en caso de error
            }
        }
        public static string ParseHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGBA(color);
        }
        public static string ColorString(string s, string color)
        {
            string leftTag = "<color=" + color + ">";
            string rightTag = "</color>";

            return leftTag + s + rightTag;
        }
    }
}
