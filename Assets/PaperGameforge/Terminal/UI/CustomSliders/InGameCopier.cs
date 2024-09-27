using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.CustomSliders
{
    public class InGameCopier : MonoBehaviour
    {
        #region FIELDS
        private float percentageProgress = 0f;
        private float currentSize = 0f;
        private float totalSize = 0f;
        private float currentSpeed = 0f;
        private DateTime spendedTime = DateTime.MinValue;
        #endregion

        #region PROPERTIES
        public float PercentageProgress
        {
            get => percentageProgress;
            private set
            {
                percentageProgress = Mathf.Clamp01(value / 1000000);
                // Here you can update UI elements, for example:
                //Debug.Log($"Progress: {percentageProgress * 100}%");
            }
        }
        public float CurrentSize
        {
            get => currentSize;
            private set
            {
                currentSize = value;
                currentSize /= (1024f * 1024f); // MB
                //Debug.Log($"Current Size: {currentSize / (1024f * 1024f):F2} MB");
            }
        }
        public float TotalSize
        {
            get => totalSize;
            private set
            {
                if (value > 0f)
                {
                    totalSize = value;
                    totalSize /= (1024f * 1024f); // MB
                    //Debug.Log($"Total Size: {totalSize / (1024f * 1024f):F2} MB");
                }
            }
        }
        public float CurrentSpeed { get => currentSpeed; set => currentSpeed = value; }
        public DateTime SpendedTime { get => spendedTime; set => spendedTime = value; }
        #endregion

        #region CONSTRUCTOR
        public InGameCopier(float percentageProgress, float currentSize, float totalSize, float currentSpeed, DateTime spendedTime)
        {
            this.percentageProgress = percentageProgress;
            this.currentSize = currentSize;
            this.totalSize = totalSize;
            this.currentSpeed = currentSpeed;
            this.spendedTime = spendedTime;
        }
        #endregion

        #region METHODS
        public (bool sourceExists, bool destExists) Copy(string source, string destination, bool hard = false)
        {
            if (File.Exists(source))
            {
                var copyResult = ValidateAndPrepareFile(source, destination, hard);
                if (copyResult == (true, true))
                {
                    CopyFile(source, destination);
                }
                return copyResult;
            }
            else if (Directory.Exists(source))
            {
                var copyResult = ValidateAndPrepareDirectories(source, destination, hard);
                if (copyResult == (true, true))
                {
                    StartCoroutine(CopyDirectoryAsync(source, destination));
                }
                return copyResult;
            }
            return (false, false);
        }
        private void CopyFile(string sourceFile, string destination)
        {
            string targetFilePath = Path.Combine(destination, Path.GetFileName(sourceFile));
            StartCoroutine(CopyFileAsync(sourceFile, targetFilePath));
        }
        private IEnumerator CopyFileAsync(string sourceFile, string targetFilePath)
        {
            Debug.Log("File copy started...");
            long fileLength = new FileInfo(sourceFile).Length;
            long totalBytesCopied = 0;
            TotalSize = fileLength; // Set the total size for the file

            int bufferSize = 1024 * 1024;
            using (FileStream sourceStream = new(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream targetStream = new(targetFilePath, FileMode.Create, FileAccess.Write))
            {
                byte[] buffer = new byte[bufferSize];
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    targetStream.Write(buffer, 0, bytesRead);
                    totalBytesCopied += bytesRead;
                    CurrentSize = totalBytesCopied; // Update current size
                    PercentageProgress = totalBytesCopied / TotalSize; // Update progress

                    yield return null;
                }
            }
            Debug.Log("File copied successfully.");
        }
        private IEnumerator CopyDirectoryAsync(string sourceDirectory, string destination)
        {
            Debug.Log("Directory copy started...");

            TotalSize = GetDirectorySize(sourceDirectory); // Set total size for directory
            long totalBytesCopied = 0;

            Task copyTask = Task.Run(() => CopyDirectory(sourceDirectory, destination, ref totalBytesCopied));

            while (!copyTask.IsCompleted)
            {
                CurrentSize = totalBytesCopied; // Update current size during copy
                PercentageProgress = totalBytesCopied / TotalSize; // Update progress
                yield return null;
            }

            if (copyTask.IsFaulted)
            {
                Debug.LogError("Error during directory copy: " + copyTask.Exception);
            }
            else
            {
                Debug.Log("Directory copied successfully.");
            }
        }
        private void CopyDirectory(string sourceDirectory, string destination, ref long totalBytesCopied)
        {
            string newDestinationDir = Path.Combine(destination, Path.GetFileName(sourceDirectory));
            Directory.CreateDirectory(newDestinationDir);

            foreach (var file in Directory.GetFiles(sourceDirectory))
            {
                string targetFilePath = Path.Combine(newDestinationDir, Path.GetFileName(file));
                CopyFileWithProgress(file, targetFilePath, ref totalBytesCopied);
            }

            foreach (var subDir in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectory(subDir, newDestinationDir, ref totalBytesCopied);
            }
        }
        private void CopyFileWithProgress(string sourceFile, string destinationFile, ref long totalBytesCopied)
        {
            int bufferSize = 1024 * 1024;
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            using (FileStream sourceStream = new(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream targetStream = new(destinationFile, FileMode.Create, FileAccess.Write))
            {
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    targetStream.Write(buffer, 0, bytesRead);
                    totalBytesCopied += bytesRead;
                }
            }
        }
        private long GetDirectorySize(string directory)
        {
            long size = 0;
            foreach (var file in Directory.GetFiles(directory, "*", SearchOption.AllDirectories))
            {
                size += new FileInfo(file).Length;
            }
            return size;
        }
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
        #endregion
    }
}