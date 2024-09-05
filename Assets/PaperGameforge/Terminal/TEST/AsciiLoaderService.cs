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

        public List<ServiceResponse> LoadTitle()
        {
            return LoadAscii(ASCII_TITLE_ASSET, SPACING);
        }
        private List<ServiceResponse> LoadAscii(string fileName, int spacing)
        {
            List<ServiceResponse> responses = new();

            StreamReader file = new(Path.Combine(UnityEngine.Application.streamingAssetsPath, fileName));

            for (int i = 0; i < spacing; i++)
            {
                responses.Add(new(string.Empty, false));
            }

            while (!file.EndOfStream)
            {
                responses.Add(new(file.ReadLine(), false));
            }

            for (int i = 0; i < spacing; i++)
            {
                responses.Add(new(string.Empty, false));
            }

            file.Close();

            return responses;
        }
    }
}