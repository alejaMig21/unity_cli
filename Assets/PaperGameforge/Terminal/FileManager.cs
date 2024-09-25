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
        /// <summary>
        /// Gets all directories in the given path.
        /// </summary>
        /// <param name="pathInfo">The full path of the directory to check.</param>
        /// <returns>
        /// A list of strings containing the full path of the directories found.
        /// </returns>
        private List<string> GetDirectories(string[] pathInfo)
        {
            // Initialize an empty list to store the found directories
            List<string> foundDirectories = new();

            // Iterate over the directories in the given path
            foreach (string dir in pathInfo)
            {
                // Get the information of the current directory
                DirectoryInfo info = new(dir);

                // Get the full path of the directory
                string directoryPath = WHITE_SPACE + info.FullName;

                // Replace the backslashes with double backslashes
                directoryPath = directoryPath.Replace(@"\", @"\\");

                // Add the full path of the directory to the list
                foundDirectories.Add(directoryPath);
            }

            // Return the list of directories
            return foundDirectories;
        }
        /// <summary>
        /// Checks whether the specified directory exists.
        /// </summary>
        /// <param name="fullDirName">The full path of the directory to check.</param>
        /// <returns>
        /// A boolean indicating whether the directory exists.
        /// </returns>
        private bool CheckDirExistence(string fullDirName)
        {
            // Get the full path of the directory
            string newPath = fullDirName;

            // Check if the directory exists
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
        /// <summary>
        /// Moves the current directory to its parent directory.
        /// </summary>
        /// <returns>
        /// A tuple with two values. The first value is a boolean indicating whether the parent directory exists and
        /// the second value is the new path of the current directory.
        /// </returns>
        public (bool exists, string newPath) MoveToPreviousDirectory()
        {
            // Get the parent directory of the current directory
            DirectoryInfo info = Directory.GetParent(path);

            // If the parent directory does not exist, return false and the current path
            if (info == null)
            {
                return (false, path);
            }

            // Check if the parent directory exists
            bool exists = CheckDirExistence(info.FullName);

            // If the parent directory exists, set the new path to the parent directory
            if (exists)
            {
                Path = info.FullName;
            }

            // Return the existence of the parent directory and the new path
            return (exists, path);
        }
        /// <summary>
        /// Copies a file or directory. If the source is a directory, it will be copied recursively.
        /// </summary>
        /// <param name="source">The source file or directory.</param>
        /// <param name="destination">The destination directory.</param>
        /// <param name="hard">True if the destination directory should be created if it does not exist.</param>
        /// <returns>
        /// A tuple with two booleans. The first boolean indicates whether the source file or directory exists and the second boolean
        /// indicates whether the destination directory exists.
        /// </returns>
        public (bool sourceExists, bool destExists) Copy(string source, string destination, bool hard = false)
        {
            // Check if the source is a file
            if (File.Exists(source))
            {
                // Validate the source file and destination directory
                var copyResult = ValidateAndPrepareFile(source, destination, hard);

                // If both the source and destination exist, copy the file
                if (copyResult == (true, true))
                {
                    CopyFile(source, destination);
                }

                return copyResult;
            }
            // Check if the source is a directory
            else if (Directory.Exists(source))
            {
                // Validate the source directory and destination directory
                var copyResult = ValidateAndPrepareDirectories(source, destination, hard);

                // If both the source and destination exist, call the asynchronous method to copy the directory
                if (copyResult == (true, true))
                {
                    // Llamar al método asíncrono para copiar directorios
                    StartCoroutine(CopyDirectoryAsync(source, destination));
                }

                return copyResult;
            }

            // If the source is neither a file nor a directory, return false for both
            return (false, false);
        }
        /// <summary>
        /// Validates that the source file exists and the destination directory exists and is either created if it does not or overwritten if it does.
        /// </summary>
        /// <param name="sourceDirectory">The source file.</param>
        /// <param name="destination">The destination directory.</param>
        /// <param name="hard">True if the destination directory should be created if it does not exist.</param>
        /// <returns>A tuple with a boolean indicating whether the source file exists and another indicating whether the destination directory exists.</returns>
        private (bool sourceExists, bool destExists) ValidateAndPrepareFile(string sourceFile, string destination, bool hard)
        {
            // Check if the source file exists
            if (!File.Exists(sourceFile))
            {
                return (false, true); // Source file does not exist
            }

            // Check if the destination directory exists and create it if it does not
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

            return (true, true);
        }
        /// <summary>
        /// Copies a file.
        /// </summary>
        /// <param name="sourceFile">The source file.</param>
        /// <param name="destination">The destination directory.</param>
        private void CopyFile(string sourceFile, string destination)
        {
            // Create the target file path by combining the destination directory with the source file name
            string targetFilePath = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceFile));
            // Start the copy operation asynchronously
            StartCoroutine(CopyFileAsync(sourceFile, targetFilePath));
        }

        /// <summary>
        /// Copies a file asynchronously.
        /// </summary>
        /// <param name="file">The source file.</param>
        /// <param name="targetFilePath">The target file path.</param>
        /// <returns>An IEnumerator representing the asynchronous copy operation.</returns>
        private IEnumerator CopyFileAsync(string file, string targetFilePath)
        {
            // Log the start of the file copy operation
            Debug.Log("Copy started...");

            // Run the copy operation in a separate thread
            Task copyTask = Task.Run(() => File.Copy(file, targetFilePath, overwrite: true));

            // Wait for the copy operation to complete without blocking the main thread
            yield return new WaitUntil(() => copyTask.IsCompleted);

            // If there was an error during the copy operation, log it
            if (copyTask.IsFaulted)
            {
                Debug.LogError("Error during copy: " + copyTask.Exception);
            }
            else
            {
                // Log the successful completion of the file copy operation
                Debug.Log("File copied successfully.");
            }
        }
        /// <summary>
        /// Copies a directory and all its contents asynchronously.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destination">The destination directory.</param>
        /// <returns>An IEnumerator representing the asynchronous copy operation.</returns>
        private IEnumerator CopyDirectoryAsync(string sourceDirectory, string destination)
        {
            // Log the start of the directory copy operation
            Debug.Log("Directory copy started...");

            // Run the copy operation in a separate thread
            Task copyTask = Task.Run(() => CopyDirectory(sourceDirectory, destination));

            // Wait for the copy operation to complete without blocking the main thread
            yield return new WaitUntil(() => copyTask.IsCompleted);

            // If there was an error during the copy operation, log it
            if (copyTask.IsFaulted)
            {
                Debug.LogError("Error during directory copy: " + copyTask.Exception);
            }
            else
            {
                // Log the successful completion of the directory copy operation
                Debug.Log("Directory copied successfully.");
            }
        }
        /// <summary>
        /// Validates that the source directory exists and the destination directory exists and is either created if it does not or overwritten if it does.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destination">The destination directory.</param>
        /// <param name="hard">True if the destination directory should be created if it does not exist.</param>
        /// <returns>A tuple with a boolean indicating whether the source directory exists and another indicating whether the destination directory exists.</returns>
        private (bool sourceExists, bool destExists) ValidateAndPrepareDirectories(string sourceDirectory, string destination, bool hard)
        {
            // Check if the source directory exists
            if (!Directory.Exists(sourceDirectory))
            {
                return (false, true); // Source directory does not exist
            }

            // Check if the destination directory exists and create it if it does not
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

            return (true, true);
        }
        /// <summary>
        /// Copies all files and subdirectories from the source directory to the destination directory.
        /// </summary>
        /// <param name="sourceDirectory">The source directory.</param>
        /// <param name="destination">The destination directory.</param>
        private void CopyDirectory(string sourceDirectory, string destination)
        {
            // Create the destination directory if it does not exist
            string newDestinationDir = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceDirectory));
            Directory.CreateDirectory(newDestinationDir);

            // Copy all files from the source directory to the destination directory
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string targetFilePath = System.IO.Path.Combine(newDestinationDir, System.IO.Path.GetFileName(file));
                File.Copy(file, targetFilePath, overwrite: true);
            }

            // Recursively copy all subdirectories from the source directory to the destination directory
            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectory(subDir, newDestinationDir);  // Recursion for subdirectories
            }
        }
        #endregion
    }
}