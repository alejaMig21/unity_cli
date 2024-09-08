using Assets.PaperGameforge.Terminal.Managers;
using Assets.PaperGameforge.Terminal.Services.Responses;
using Assets.PaperGameforge.Terminal.Services.Structs;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.Services
{
    [CreateAssetMenu(fileName = "TextFormatterService", menuName = "TerminalServices/DecoratorServices/TextFormatterService")]
    public class TextFormatterService : DecoratorService
    {
        [SerializeField]
        private List<SColorKey> colors = new()
        {
            new("black", ColorManager.ParseColor("#021b21")),
            new("gray", ColorManager.ParseColor("#555d71")),
            new("red", ColorManager.ParseColor("#ff5879")),
            new("yellow", ColorManager.ParseColor("#f2f1b9")),
            new("blue", ColorManager.ParseColor("#9ed9d8")),
            new("purple", ColorManager.ParseColor("#d936ff")),
            new("orange", ColorManager.ParseColor("#ef5847"))
        };

        private const string ERROR_CMD = "[ERROR]";
        private const string INFO_CMD = "[INFO]";
        private const string ERROR_COLOR = "red";
        private const string ERROR_BODY_COLOR = "yellow";
        private const string INFO_COLOR = "blue";
        private const string INFO_BODY_COLOR = "yellow";
        private const string SPECIAL_COLOR = "purple";

        public List<SColorKey> Colors { get => colors; set => colors = value; }

        public override List<ServiceResponse> ParseResponse(List<ServiceResponse> responses)
        {
            List<ServiceResponse> finalList = new();

            foreach (var item in responses)
            {
                if (item.BackgroundProcess)
                {
                    continue;
                }

                List<ServiceResponse> decoratedResponses = ProcessResponse(item);

                if (decoratedResponses != null)
                {
                    // Añadir elementos decorados en lugar de la respuesta original
                    finalList.AddRange(decoratedResponses);
                }
                else
                {
                    // Añadir la respuesta original si no puede ser decorada
                    finalList.Add(item);
                }
            }

            return finalList.Count > 0 ? finalList : null;
        }
        public override List<ServiceResponse> ProcessResponse(ServiceResponse response, string userInput = "")
        {
            string[] args = response.Text.Split(TWO_DOTS_SEPARATOR);

            if (args.Length == 2)
            {
                string header = args[0];
                string body = args[1];

                switch (header)
                {
                    case ERROR_CMD:
                        args[0] = ColorString(header, GetColor(ERROR_COLOR));
                        args[1] = ColorString(body, GetColor(ERROR_BODY_COLOR));
                        break;
                    case INFO_CMD:
                        args[0] = ColorString(header, GetColor(INFO_COLOR));
                        args[1] = ColorString(body, GetColor(INFO_BODY_COLOR));
                        break;
                }

                response.Text = string.Join(' ', args);

                return new() { response };
            }

            return null;
        }
        private string GetColor(string colorKey)
        {
            SColorKey colorStruct = colors.Find(item => item.Key == colorKey);

            return "#" + ColorManager.ParseHex(colorStruct.Color);
        }
        public string ColorString(string s, string color)
        {
            string leftTag = "<color=" + color + ">";
            string rightTag = "</color>";

            return leftTag + s + rightTag;
        }
        public string DecorateSpecialResponse(string response)
        {
            return ColorString(response, SPECIAL_COLOR);
        }
    }
}