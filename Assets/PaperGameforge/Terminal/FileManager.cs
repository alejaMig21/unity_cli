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
        #endregion
    }
}