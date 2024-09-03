using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "AsciiLoaderService", menuName = "TerminalServices/InterpreterServices/AsciiLoaderService")]
    public class AsciiLoaderService : InterpreterService
    {
        private const string ASCII_TITLE_ASSET = "ascii.txt";
        private TextFormatterService textFormatterService;

        public void SetUpValues(TextFormatterService textFormatterService)
        {
            this.textFormatterService = textFormatterService;
        }

        public override (bool, List<string>) Execute(string userInput)
        {
            throw new System.NotImplementedException();
        }
        public List<string> LoadTitle()
        {
            return LoadAscii(ASCII_TITLE_ASSET, "red", 2);
        }
        private List<string> LoadAscii(string path, string color, int spacing)
        {
            List<string> responses = new();

            StreamReader file = new(Path.Combine(Application.streamingAssetsPath, path));

            for (int i = 0; i < spacing; i++)
            {
                responses.Add("");
            }

            while (!file.EndOfStream)
            {
                responses.Add(textFormatterService.ColorString(file.ReadLine(), textFormatterService.Colors[color]));
            }

            for (int i = 0; i < spacing; i++)
            {
                responses.Add("");
            }

            file.Close();

            return responses;
        }
    }
}