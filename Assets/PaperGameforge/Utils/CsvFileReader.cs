using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Utils
{
    public static class CsvFileReader
    {
        /// <summary>
        /// Reads the entire CSV and returns a list of rows as dictionaries (header -> percentValue)
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<Dictionary<string, string>> ReadCsvByRows(string fileName)
        {
            var rows = new List<Dictionary<string, string>>();

            try
            {
                var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord;

                    while (csv.Read())
                    {
                        var row = new Dictionary<string, string>();
                        foreach (var header in headers)
                        {
                            row[header] = csv.GetField<string>(header);
                        }
                        rows.Add(row);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading CSV file: {ex.Message}");
            }

            return rows;
        }
        public static string[,] ReadCsvByColumns(string fileName)
        {
            try
            {
                var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    int numCols = csv.HeaderRecord.Length;

                    // Leer los registros del CSV
                    var records = new List<string[]>();
                    while (csv.Read())
                    {
                        var row = new string[numCols];
                        for (int i = 0; i < numCols; i++)
                        {
                            row[i] = csv.GetField<string>(i);
                        }
                        records.Add(row);
                    }

                    int numRows = records.Count;
                    string[,] result = new string[numRows, numCols];

                    // Agregar las filas a la matriz resultante
                    for (int i = 0; i < numRows; i++)
                    {
                        for (int j = 0; j < numCols; j++)
                        {
                            result[i, j] = records[i][j];
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading CSV file: {ex.Message}");
                return new string[0, 0]; // Devuelve un arreglo vacío en caso de error
            }
        }
        /// <summary>
        /// Reads a specific cell from the CSV file given a key and column name
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="key"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetCell(string fileName, string key, string column)
        {
            try
            {
                var filePath = Path.Combine(Application.streamingAssetsPath, fileName);
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        if (csv.GetField("Key") == key)
                        {
                            return csv.GetField(column);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading CSV: {ex.Message}");
            }

            return null;
        }
    }
}