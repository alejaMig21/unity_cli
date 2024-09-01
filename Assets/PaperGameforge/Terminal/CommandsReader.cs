using Assets.PaperGameforge.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    public static class CommandsReader
    {
        private const string FILE_NAME = "terminal_commands.csv";
        private const string NOT_FOUND_COMMAND = "ERROR NOT_FOUND";
        private static string[,] commands = null;

        public static string[,] Commands
        {
            get
            {
                if (commands == null)
                {
                    var csvFilePath = Path.Combine(Application.streamingAssetsPath, FILE_NAME);
                    commands = CsvFileReader.ReadCsvByColumns(csvFilePath);
                }
                return commands;
            }
        }

        /// <summary>
        /// Método para cargar y organizar los comandos a partir del CSV
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Tuple with the responses and a bool with a possible error triggered on the responses search</returns>
        public static (bool error, List<string> responses) GetResponses(string command)
        {
            string[] args = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            List<string> responses = new();

            int rows = Commands.GetLength(0);
            int cols = Commands.GetLength(1);
            int currentRow;
            int lastKeyColIndex;

            if (!TryFindKeyColumnIndex(args, out currentRow, out lastKeyColIndex, out int lastKeyIndex))
            {
                responses.AddRange(GetResponses(NOT_FOUND_COMMAND).responses);
                return (true, responses);
            }

            responses.Add(Commands[currentRow, cols - 1]);

            // Buscar respuestas consecutivas en la misma columna de la última clave encontrada
            responses.AddRange(GetConsecutiveResponses(currentRow + 1, lastKeyColIndex, rows, cols));

            return (false, responses);
        }
        private static bool TryFindKeyColumnIndex(string[] args, out int currentRow, out int lastKeyColIndex, out int lastKeyIndex)
        {
            int rows = Commands.GetLength(0);
            int cols = Commands.GetLength(1);

            currentRow = 0;
            lastKeyColIndex = 0;
            lastKeyIndex = -1;

            for (int argIndex = 0; argIndex < args.Length; argIndex++)
            {
                string key = args[argIndex];
                int colIndex = argIndex;

                if (colIndex >= cols - 1)
                {
                    lastKeyIndex = cols - 1;
                    return false;
                }

                while (currentRow < rows && (string.IsNullOrEmpty(Commands[currentRow, colIndex]) || Commands[currentRow, colIndex] != key))
                {
                    currentRow++;
                }

                if (currentRow >= rows || Commands[currentRow, colIndex] != key)
                {
                    lastKeyIndex = cols - 1;
                    return false;
                }

                lastKeyColIndex = colIndex;
                int nextRow = currentRow + 1;

                while (nextRow < rows && !string.IsNullOrEmpty(Commands[nextRow, colIndex]) && Commands[nextRow, colIndex] == key)
                {
                    nextRow++;
                }

                currentRow = nextRow - 1;
            }

            return true;
        }
        private static List<string> GetConsecutiveResponses(int startRow, int keyColIndex, int rows, int cols)
        {
            List<string> responses = new();
            int nextResponseRow = startRow;
            bool flag = false;

            while (nextResponseRow < rows && string.IsNullOrEmpty(Commands[nextResponseRow, keyColIndex]) && !flag)
            {
                for (int i = 0; i < cols - 1; i++)
                {
                    if (!string.IsNullOrEmpty(Commands[nextResponseRow, i]))
                    {
                        flag = true;
                        break;
                    }
                }

                if (flag)
                {
                    break;
                }

                responses.Add(Commands[nextResponseRow, cols - 1]);
                nextResponseRow++;
            }

            return responses;
        }
    }
}