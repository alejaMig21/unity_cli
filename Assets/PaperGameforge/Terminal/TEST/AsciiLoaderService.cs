using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.TEST
{
    [CreateAssetMenu(fileName = "AsciiLoaderService", menuName = "TerminalServices/InterpreterServices/AsciiLoaderService")]
    public class AsciiLoaderService : DecoratorService
    {
        private const string ASCII_TITLE_ASSET = "ascii.txt";
        private const int SPACING = 1;

        public List<string> LoadTitle()
        {
            return LoadAscii(ASCII_TITLE_ASSET, SPACING);
        }
        private List<string> LoadAscii(string fileName, int spacing)
        {
            List<string> responses = new();

            StreamReader file = new(Path.Combine(UnityEngine.Application.streamingAssetsPath, fileName));

            for (int i = 0; i < spacing; i++)
            {
                responses.Add("");
            }

            while (!file.EndOfStream)
            {
                responses.Add(file.ReadLine());
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