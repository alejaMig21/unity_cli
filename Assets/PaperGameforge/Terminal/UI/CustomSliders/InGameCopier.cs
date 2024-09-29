using System;
using System.Collections;
using System.IO;
using System.Linq;
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
        private DateTime remainingTime = DateTime.MinValue;
        private static readonly byte[] buffer = new byte[1024 * 1024]; // Shared 1 MB buffer to avoid reallocation
        #endregion

        #region PROPERTIES
        public float PercentageProgress
        {
            get => percentageProgress;
            private set => percentageProgress = Mathf.Clamp01(value);
        }
        public string CurrentSizeReadable { get; private set; }
        public string TotalSizeReadable { get; private set; }
        public float CurrentSize
        {
            get => currentSize;
            private set
            {
                currentSize = value;
                CurrentSizeReadable = FormatSizeReadable(currentSize);
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
                    TotalSizeReadable = FormatSizeReadable(totalSize);
                }
            }
        }
        public float CurrentSpeed
        {
            get => currentSpeed;
            set => currentSpeed = value;
        }
        public DateTime RemainingTime
        {
            get => remainingTime;
            set => remainingTime = value;
        }
        #endregion

        #region CONSTRUCTORS
        public InGameCopier(float percentageProgress, float currentSize, float totalSize, float currentSpeed, DateTime spendedTime)
        {
            this.percentageProgress = percentageProgress;
            this.currentSize = currentSize;
            this.totalSize = totalSize;
            this.currentSpeed = currentSpeed;
            this.remainingTime = spendedTime;
        }
        #endregion

        #region METHODS
        public (bool sourceExists, bool destExists) Copy(string source, string destination, bool overwrite = false)
        {
            if (File.Exists(source))
            {
                return CopyFile(source, destination, overwrite);
            }
            else if (Directory.Exists(source))
            {
                return CopyDirectory(source, destination, overwrite);
            }

            return (false, false);
        }
        private string FormatSizeReadable(float size)
        {
            var readableSize = Utils.ByteConverter.GetReadableSize((long)size);
            return $"{readableSize.size:F2} {readableSize.unit}";
        }
        private (bool, bool) CopyFile(string source, string destination, bool overwrite)
        {
            string destinationFilePath = Path.Combine(destination, Path.GetFileName(source));
            var validationResult = ValidateAndPreparePaths(source, destination, overwrite);

            if (validationResult.sourceExists && validationResult.destExists)
            {
                StartCoroutine(CopyFileAsync(source, destinationFilePath));
            }

            return validationResult;
        }
        private (bool, bool) CopyDirectory(string sourceDirectory, string destination, bool overwrite)
        {
            var validationResult = ValidateAndPreparePaths(sourceDirectory, destination, overwrite);

            if (validationResult.sourceExists && validationResult.destExists)
            {
                StartCoroutine(CopyDirectoryAsync(sourceDirectory, destination));
            }

            return validationResult;
        }
        private (bool sourceExists, bool destExists) ValidateAndPreparePaths(string source, string destination, bool createDestination)
        {
            bool sourceExists = Directory.Exists(source) || File.Exists(source);
            bool destExists = Directory.Exists(destination);

            if (!sourceExists) return (false, destExists);

            if (!destExists && createDestination)
            {
                Directory.CreateDirectory(destination);
                destExists = true;
            }

            return (sourceExists, destExists);
        }
        private IEnumerator CopyFileAsync(string sourceFilePath, string destinationFilePath)
        {
            Debug.Log($"Starting file copy: {sourceFilePath}");

            long fileLength = new FileInfo(sourceFilePath).Length;
            TotalSize = fileLength;
            long totalBytesCopied = 0;

            using (FileStream sourceStream = new(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream destinationStream = new(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                    totalBytesCopied += bytesRead;

                    CurrentSize = totalBytesCopied;
                    PercentageProgress = totalBytesCopied / TotalSize;

                    yield return null; // Wait for the next frame to continue
                }
            }

            Debug.Log("File copy completed.");
        }
        private IEnumerator CopyDirectoryAsync(string sourceDirectory, string destinationDirectory)
        {
            Debug.Log($"Starting directory copy: {sourceDirectory}");

            long totalDirectorySize = GetDirectorySize(sourceDirectory);
            TotalSize = totalDirectorySize;
            long totalBytesCopied = 0;

            Task directoryCopyTask = Task.Run(() =>
            {
                CopyDirectoryWithProgress(sourceDirectory, destinationDirectory, ref totalBytesCopied);
            });

            while (!directoryCopyTask.IsCompleted)
            {
                CurrentSize = totalBytesCopied;
                PercentageProgress = totalBytesCopied / TotalSize;
                yield return null;
            }

            if (directoryCopyTask.IsFaulted)
            {
                Debug.LogError($"Error during directory copy: {directoryCopyTask.Exception}");
            }
            else
            {
                Debug.Log("Directory copy completed.");
            }
        }
        private void CopyDirectoryWithProgress(string sourceDirectory, string destinationDirectory, ref long totalBytesCopied)
        {
            string newDestinationDir = Path.Combine(destinationDirectory, Path.GetFileName(sourceDirectory));
            Directory.CreateDirectory(newDestinationDir);

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string targetFilePath = Path.Combine(newDestinationDir, Path.GetFileName(file));
                CopyFileWithProgress(file, targetFilePath, ref totalBytesCopied);
            }

            foreach (string subDirectory in Directory.GetDirectories(sourceDirectory))
            {
                CopyDirectoryWithProgress(subDirectory, newDestinationDir, ref totalBytesCopied);
            }
        }
        private void CopyFileWithProgress(string sourceFile, string destinationFile, ref long totalBytesCopied)
        {
            using (FileStream sourceStream = new(sourceFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (FileStream destinationStream = new(destinationFile, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                int bytesRead;
                while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    destinationStream.Write(buffer, 0, bytesRead);
                    totalBytesCopied += bytesRead;

                    CurrentSize = totalBytesCopied;
                    PercentageProgress = totalBytesCopied / TotalSize;
                }
            }
        }
        private long GetDirectorySize(string directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                            .Sum(file => new FileInfo(file).Length);
        }
        #endregion
    }
}