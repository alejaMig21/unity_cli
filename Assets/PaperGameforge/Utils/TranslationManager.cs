using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Utils
{
    public static class TranslationManager
    {
        private static Dictionary<string, Dictionary<string, string>> translations = new();
        private const string FILE_NAME = "translations.csv"; // Nombre de archivo por defecto (se puede cambiar)

        // Propiedad para exponer las traducciones
        public static Dictionary<string, Dictionary<string, string>> Translations
        {
            get
            {
                // Cargar solo si no está cargado
                if (translations.Count <= 0)
                {
                    LoadTranslations();
                }
                return translations;
            }
        }
        /// <summary>
        /// Método privado para cargar las traducciones usando CsvFileReader una sola vez.
        /// </summary>
        private static void LoadTranslations()
        {
            var csvFilePath = Path.Combine(Application.streamingAssetsPath, FILE_NAME);
            var rows = CsvFileReader.ReadCsvByRows(csvFilePath);

            foreach (var row in rows)
            {
                var key = row["Key"];
                if (!translations.ContainsKey(key))
                {
                    translations[key] = new Dictionary<string, string>();
                }

                foreach (var column in row.Keys)
                {
                    if (column != "Key")
                    {
                        translations[key][column] = row[column];
                    }
                }
            }
        }
        /// <summary>
        /// Método para obtener una traducción específica usando el diccionario cargado.
        /// </summary>
        public static string GetTranslation(string key, string language)
        {
            if (Translations.ContainsKey(key) && Translations[key].ContainsKey(language))
            {
                return Translations[key][language];
            }

            Debug.LogWarning($"Translation not found for key '{key}' and language '{language}'.");
            return null;
        }
    }
}