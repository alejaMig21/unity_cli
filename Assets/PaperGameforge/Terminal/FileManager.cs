using Assets.PaperGameforge.Terminal.UI.CustomSliders;
using Assets.PaperGameforge.Terminal.UI.CustomTMP;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal
{
    [RequireComponent(typeof(InGameCopier))]
    public class FileManager : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private string path = "C:";
        private List<string> directories = new();
        [SerializeField] private DirectoryTextUGUI dirText;
        private InGameCopier copier;
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
        public InGameCopier Copier => copier ??= GetComponent<InGameCopier>();
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
        #endregion
    }
}