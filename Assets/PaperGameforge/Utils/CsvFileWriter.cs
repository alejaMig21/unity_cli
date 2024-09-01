using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.PaperGameforge.Utils
{
    public static class CsvFileWriter
    {
        public static void UpdateCsv(string fileName, string key, string column, string newValue)
        {
            var csvFilePath = Path.Combine(Application.streamingAssetsPath, fileName);

            try
            {
                var records = new List<Dictionary<string, string>>();

                using (var reader = new StreamReader(csvFilePath))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var headers = csv.HeaderRecord.ToList();

                    while (csv.Read())
                    {
                        var record = new Dictionary<string, string>();
                        foreach (var header in headers)
                        {
                            record[header] = csv.GetField(header);
                        }
                        records.Add(record);
                    }
                }

                var updatedRecords = new List<Dictionary<string, string>>();
                foreach (var record in records)
                {
                    if (record["Key"] == key)
                    {
                        record[column] = newValue;
                    }
                    updatedRecords.Add(record);
                }

                using (var writer = new StreamWriter(csvFilePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteField("Key");
                    foreach (var header in records[0].Keys.Where(k => k != "Key"))
                    {
                        csv.WriteField(header);
                    }
                    csv.NextRecord();

                    foreach (var record in updatedRecords)
                    {
                        csv.WriteField(record["Key"]);
                        foreach (var header in record.Keys.Where(k => k != "Key"))
                        {
                            csv.WriteField(record[header]);
                        }
                        csv.NextRecord();
                    }
                }

                Debug.Log("CSV updated successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating CSV: {ex.Message}");
            }
        }
    }
}