using Assets.PaperGameforge.Terminal.UI.CustomTMP;
using System.Collections.Generic;
using System.IO;
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
        /// <summary>
        /// Copies the content of a directory or file to the destination.
        /// </summary>
        /// <param name="source">The source directory or file to copy.</param>
        /// <param name="destination">The destination where the content will be copied.</param>
        /// <param name="hard">If true, creates the destination directory if it does not exist.</param>
        /// <returns>A tuple indicating whether the source and destination exist.</returns>
        public (bool sourceExists, bool destExists) Copy(string source, string destination, bool hard = false)
        {
            if (File.Exists(source))
            {
                return CopyFile(source, destination, hard);
            }
            else if (Directory.Exists(source))
            {
                return CopyDirectory(source, destination, hard);
            }

            return (false, false);
        }
        /// <summary>
        /// Copies a file to the destination.
        /// </summary>
        /// <param name="sourceFile">The source file to copy.</param>
        /// <param name="destination">The destination where the file will be copied.</param>
        /// <param name="hard">If true, creates the destination directory if it does not exist.</param>
        /// <returns>A tuple indicating whether the source file and destination exist.</returns>
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

            // Copy the file to the destination
            string targetFilePath = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceFile));
            File.Copy(sourceFile, targetFilePath, overwrite: true);

            return (true, true);
        }
        /// <summary>
        /// Copies a directory and its contents to the destination.
        /// </summary>
        /// <param name="sourceDirectory">The source directory to copy.</param>
        /// <param name="destination">The destination where the directory will be copied.</param>
        /// <param name="hard">If true, creates the destination directory if it does not exist.</param>
        /// <returns>A tuple indicating whether the source and destination directories exist.</returns>
        private (bool sourceExists, bool destExists) CopyDirectory(string sourceDirectory, string destination, bool hard = false)
        {
            if (!Directory.Exists(sourceDirectory))
            {
                return (false, true); // Source directory does not exist
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

            // Create the new directory inside the destination
            string newDestinationDir = System.IO.Path.Combine(destination, System.IO.Path.GetFileName(sourceDirectory));
            Directory.CreateDirectory(newDestinationDir);

            // Copy files in the source directory
            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string targetFilePath = System.IO.Path.Combine(newDestinationDir, System.IO.Path.GetFileName(file));
                File.Copy(file, targetFilePath, overwrite: true);
            }

            // Copy subdirectories recursively
            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectory(subDir, newDestinationDir, hard);
            }

            return (true, true);
        }
        #endregion
    }
}