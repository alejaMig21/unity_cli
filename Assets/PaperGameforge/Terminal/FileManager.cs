using Assets.PaperGameforge.Terminal.UI.CustomTMP;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    public class FileManager : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private string path = "C:";
        private List<string> directories = new();
        [SerializeField] private DirectoryTextUGUI dirText;
        #endregion

        #region CONSTANTS
        private const string WHITE_SPACE = "   ";
        #endregion

        #region PROPERTIES
        public string Path
        {
            get => path;
            private set
            {
                path = value;
                dirText.text = value + "\\";
                dirText.Adjust();
            }
        }
        public List<string> Directories
        {
            get
            {
                if (directories == null || directories.Count <= 0)
                {
                    LoadDirData();
                }
                return directories;
            }
            private set => directories = value;
        }
        public DirectoryTextUGUI DirText { get => dirText; set => dirText = value; }
        #endregion

        #region METHODS
        private void Awake()
        {
            Path = Application.streamingAssetsPath;
        }
        public List<string> LoadDirData()
        {
            directories.Clear();

            directories.AddRange(LoadFolders());

            directories.AddRange(LoadFiles());

            return directories;
        }
        public List<string> LoadFolders()
        {
            return GetDirectories(Directory.GetDirectories(Path));
        }
        public List<string> LoadFiles()
        {
            return GetDirectories(Directory.GetFiles(Path));
        }
        private List<string> GetDirectories(string[] pathInfo)
        {
            List<string> foundDirectories = new();

            foreach (string dir in pathInfo)
            {
                DirectoryInfo info = new(dir);
                string directoryPath = WHITE_SPACE + info.FullName.Replace(@"\", @"\\");
                foundDirectories.Add(directoryPath);
            }

            return foundDirectories;
        }
        private bool CheckDirExistence(string fullDirName)
        {
            string newPath = fullDirName;
            return Directory.Exists(newPath);
        }
        public (bool exists, string newPath) MoveToDirectory(string folderName)
        {
            bool exists = CheckDirExistence(path + "\\" + folderName);

            if (exists)
            {
                Path += "\\" + folderName;
            }

            return (exists, path);
        }
        public (bool exists, string newPath) MoveToPreviousDirectory()
        {
            DirectoryInfo info = Directory.GetParent(path);

            if (info == null)
            {
                return (false, path);
            }

            bool exists = CheckDirExistence(info.FullName);

            if (exists)
            {
                Path = info.FullName;
            }
            return (exists, path);
        }
        public (bool sourceExists, bool destExists) Copy(string source, string destination, bool hard = false)
        {
            if (File.Exists(source))
            {
                return CopyFile(source, destination, hard);
            }
            else if (Directory.Exists(source))
            {
                // Llamar al método asíncrono para copiar directorios
                StartCoroutine(CopyDirectoryAsync(source, destination, hard));
                return (true, true);
            }

            return (false, false);
        }

        private (bool sourceExists, bool destExists) CopyFile(string sourceFile, string destination, bool hard = false)
        {
            if (!File.Exists(sourceFile))
            {
                return (false, true); // Source file does not exist
            }

            if (!Directory.Exists(destination))
            {
                if (hard)
                {
                    Directory.CreateDirectory(destination);
                }
                else
                {
                    return (true, false); // Destination directory does not exist
                }
            }

            string targetFilePath = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceFile));
            StartCoroutine(CopyFileAsync(sourceFile, targetFilePath));

            return (true, true);
        }
        private IEnumerator CopyFileAsync(string file, string targetFilePath)
        {
            Debug.Log("Copy started...");

            // Ejecutar la operación de copia en un hilo separado
            Task copyTask = Task.Run(() => File.Copy(file, targetFilePath, overwrite: true));

            // Esperar a que la tarea termine sin bloquear el hilo principal
            yield return new WaitUntil(() => copyTask.IsCompleted);

            if (copyTask.IsFaulted)
            {
                Debug.LogError("Error during copy: " + copyTask.Exception);
            }
            else
            {
                Debug.Log("File copied successfully.");
            }
        }
        private IEnumerator CopyDirectoryAsync(string sourceDirectory, string destination, bool hard = false)
        {
            Debug.Log("Directory copy started...");

            Task copyTask = Task.Run(() => CopyDirectory(sourceDirectory, destination, hard));

            // Esperar a que la tarea de copia termine sin bloquear el hilo principal
            yield return new WaitUntil(() => copyTask.IsCompleted);

            if (copyTask.IsFaulted)
            {
                Debug.LogError("Error during directory copy: " + copyTask.Exception);
            }
            else
            {
                Debug.Log("Directory copied successfully.");
            }
        }
        private void CopyDirectory(string sourceDirectory, string destination, bool hard)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                return; // Source directory does not exist
            }

            if (!Directory.Exists(destination))
            {
                if (hard)
                {
                    Directory.CreateDirectory(destination);
                }
                else
                {
                    return; // Destination directory does not exist
                }
            }

            string newDestinationDir = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceDirectory));
            Directory.CreateDirectory(newDestinationDir);

            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string targetFilePath = System.IO.Path.Combine(newDestinationDir, System.IO.Path.GetFileName(file));
                File.Copy(file, targetFilePath, overwrite: true);  // Copiar archivos de forma sincrónica dentro del hilo separado
            }

            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectory(subDir, newDestinationDir, hard);  // Recursión para subdirectorios
            }
        }
        #endregion
    }
}