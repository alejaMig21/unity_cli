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
        #region FIELDS
        [SerializeField]
        private List<SColorKey> colors = new()
        {
            new("black", ColorManager.ParseColor("#021b21")),
            new("gray", ColorManager.ParseColor("#555d71")),
            new("red", ColorManager.ParseColor("#ff5879")),
            new("yellow", ColorManager.ParseColor("#f2f1b9")),
            new("blue", ColorManager.ParseColor("#9ed9d8")),
            new("purple", ColorManager.ParseColor("#d936ff")),
            new("orange", ColorManager.ParseColor("#ef5847")),
            new("nfcp_intense_yellow", ColorManager.ParseColor("#ffca00")) // nfcp stands for "not from color palette"
        };
        #endregion

        #region CONSTANTS
        // Constants for color keys
        private const string ERROR_CMD = "[ERROR]";
        private const string INFO_CMD = "[INFO]";
        private const string ERROR_COLOR = "red";
        private const string ERROR_BODY_COLOR = "yellow";
        private const string INFO_COLOR = "blue";
        private const string INFO_BODY_COLOR = "yellow";
        private const string SPECIAL_COLOR = "purple";
        private const string SIMPLE_RESPONSE_COLOR = "nfcp_intense_yellow";
        #endregion

        #region PROPERTIES
        public List<SColorKey> Colors { get => colors; set => colors = value; }
        #endregion

        #region METHODS
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

            // Possible special responses
            if (args.Length == 2)
            {
                string header = args[0];
                string body = args[1];

                switch (header)
                {
                    // Error case
                    case ERROR_CMD:
                        args[0] = ColorManager.ColorString(header, GetColor(ERROR_COLOR));
                        args[1] = ColorManager.ColorString(body, GetColor(ERROR_BODY_COLOR));
                        response.Text = string.Join(' ', args);
                        break;
                    // Information case
                    case INFO_CMD:
                        args[0] = ColorManager.ColorString(header, GetColor(INFO_COLOR));
                        args[1] = ColorManager.ColorString(body, GetColor(INFO_BODY_COLOR));
                        response.Text = string.Join(' ', args);
                        break;
                    // Directory or simple response with 'TWO_DOTS_SEPARATOR' character
                    default:
                        response.Text = string.Join(':', args);
                        response.Text = ColorManager.ColorString(response.Text, GetColor(SIMPLE_RESPONSE_COLOR));
                        break;
                }

                return new() { response };
            }

            // Plain text
            if (response != null)
            {
                response.Text = ColorManager.ColorString(response.Text, GetColor(SIMPLE_RESPONSE_COLOR));
                return new() { response };
            }

            return null;
        }
        protected string GetColor(string colorKey)
        {
            SColorKey colorStruct = colors.Find(item => item.Key == colorKey);

            return "#" + ColorManager.ParseHex(colorStruct.Color);
        }
        public string DecorateSpecialResponse(string response)
        {
            return ColorManager.ColorString(response, SPECIAL_COLOR);
        }
        #endregion
    }
}