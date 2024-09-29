using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.PaperGameforge.Terminal.UI.CustomSliders
{
    /// <summary>
    /// The InGameCopier class is responsible for handling the copying of files and directories within a Unity game. 
    /// It provides functionality to copy files or directories from a source path to a destination path, 
    /// track the progress of the operation, and ensure that the copying is done asynchronously to avoid performance 
    /// issues during gameplay.
    /// </summary>
    public class InGameCopier : MonoBehaviour
    {
        #region FIELDS
        /// <summary>
        /// Tracks the progress of the copying operation as a percentage (0 to 1).
        /// </summary>
        private float percentageProgress = 0f;
        /// <summary>
        /// Stores the current size of the copied data.
        /// </summary>
        private float currentSize = 0f;
        /// <summary>
        /// Stores the total size of the data to be copied.
        /// </summary>
        private float totalSize = 0f;
        /// <summary>
        /// Tracks the current copying speed in bytes per second.
        /// </summary>
        private (double size, string unit) currentSpeed;
        /// <summary>
        /// A shared 1MB buffer to minimize memory allocation during file copy operations.
        /// </summary>
        private static readonly byte[] buffer = new byte[1024 * 1024];
        /// <summary>
        /// Tracks the number of bytes copied in the last second to calculate speed.
        /// </summary>
        private long lastBytesCopied = 0;
        #endregion

        #region PROPERTIES
        /// <summary>
        /// Provides access to the progress of the copy operation as a percentage.
        /// </summary>
        public float PercentageProgress
        {
            get => percentageProgress;
            private set => percentageProgress = Mathf.Clamp01(value);
        }
        /// <summary>
        /// Readable version of the current size.
        /// </summary>
        public string CurrentSizeReadable { get; private set; }
        /// <summary>
        /// Readable version of the total size.
        /// </summary>
        public string TotalSizeReadable { get; private set; }
        /// <summary>
        /// The current amount of data copied, which updates the readable size.
        /// </summary>
        public float CurrentSize
        {
            get => currentSize;
            private set
            {
                currentSize = value;
                CurrentSizeReadable = FormatSizeReadable(currentSize);
            }
        }
        /// <summary>
        /// The total size of the data to be copied, which updates the readable size.
        /// </summary>
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
        /// <summary>
        /// Provides the current speed of the copy operation in a human-readable format (e.g., KB/s, MB/s).
        /// </summary>
        public string CurrentSpeed
        {
            get => $"{currentSpeed.size:F2}{currentSpeed.unit}";
        }
        #endregion

        #region CONSTRUCTORS
        public InGameCopier(float percentageProgress, float currentSize, float totalSize, float currentSpeed)
        {
            this.percentageProgress = percentageProgress;
            this.currentSize = currentSize;
            this.totalSize = totalSize;
            this.currentSpeed = (0, "Bytes");
        }
        #endregion

        #region METHODS
        /// <summary>
        /// Copies a file or directory from a source path to a destination path, optionally overwriting the destination if it exists.
        /// </summary>
        /// <param name="source">The path to the source file or directory to be copied.</param>
        /// <param name="destination">The path to the destination where the source will be copied.</param>
        /// <param name="overwrite">A boolean value indicating whether to overwrite the destination if it exists. Default is false.</param>
        /// <returns>A tuple containing two boolean values: 
        /// <c>sourceExists</c> indicating whether the source path exists, and 
        /// <c>destExists</c> indicating whether the destination path exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination path is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
        /// <summary>
        /// Converts a file size in bytes to a human-readable string format.
        /// </summary>
        /// <param name="size">The size in bytes to be converted.</param>
        /// <returns>A string representing the size in a human-readable format with appropriate units (e.g., KB, MB, GB).</returns>
        private string FormatSizeReadable(float size)
        {
            var readableSize = Utils.ByteConverter.GetReadableSize((long)size);
            return $"{readableSize.size:F2}{readableSize.unit}";
        }
        /// <summary>
        /// Copies a file from a source path to a destination path, optionally overwriting the destination file.
        /// </summary>
        /// <param name="source">The path to the source file to be copied.</param>
        /// <param name="destination">The path to the destination directory where the source file will be copied.</param>
        /// <param name="overwrite">A boolean value indicating whether to overwrite the destination file if it exists.</param>
        /// <returns>A tuple containing two boolean values: 
        /// <c>sourceExists</c> indicating whether the source file exists, and 
        /// <c>destExists</c> indicating whether the destination directory exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination path is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
        /// <summary>
        /// Copies a directory from a source path to a destination path, optionally overwriting the destination directory.
        /// </summary>
        /// <param name="sourceDirectory">The path to the source directory to be copied.</param>
        /// <param name="destination">The path to the destination directory where the source directory will be copied.</param>
        /// <param name="overwrite">A boolean value indicating whether to overwrite the destination directory if it exists.</param>
        /// <returns>A tuple containing two boolean values: 
        /// <c>sourceExists</c> indicating whether the source directory exists, and 
        /// <c>destExists</c> indicating whether the destination directory exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination path is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        private (bool, bool) CopyDirectory(string sourceDirectory, string destination, bool overwrite)
        {
            var validationResult = ValidateAndPreparePaths(sourceDirectory, destination, overwrite);

            if (validationResult.sourceExists && validationResult.destExists)
            {
                StartCoroutine(CopyDirectoryAsync(sourceDirectory, destination));
            }

            return validationResult;
        }
        /// <summary>
        /// Validates the existence of the source and destination paths and optionally creates the destination directory if it does not exist.
        /// </summary>
        /// <param name="source">The path to the source file or directory.</param>
        /// <param name="destination">The path to the destination directory.</param>
        /// <param name="createDestination">A boolean value indicating whether to create the destination directory if it does not exist.</param>
        /// <returns>A tuple containing two boolean values: 
        /// <c>sourceExists</c> indicating whether the source path exists, and 
        /// <c>destExists</c> indicating whether the destination path exists.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination path is null.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
        /// <summary>
        /// Asynchronously copies a file from a source path to a destination path while tracking the progress of the copy operation.
        /// </summary>
        /// <param name="sourceFilePath">The path to the source file to be copied.</param>
        /// <param name="destinationFilePath">The path to the destination file where the source file will be copied.</param>
        /// <returns>An IEnumerator that can be used to control the coroutine.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination file path is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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

                    UpdateProgress(totalBytesCopied);

                    yield return null; // Wait for the next frame to continue
                }
            }

            Debug.Log("File copy completed.");
        }
        /// <summary>
        /// Asynchronously copies a directory from a source path to a destination path, including all files and subdirectories, while tracking the progress of the copy operation.
        /// </summary>
        /// <param name="sourceDirectory">The path to the source directory to be copied.</param>
        /// <param name="destinationDirectory">The path to the destination directory where the source directory will be copied.</param>
        /// <returns>An IEnumerator that can be used to control the coroutine.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination directory path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the source directory does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
                UpdateProgress(totalBytesCopied);
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
        /// <summary>
        /// Copies a directory from a source path to a destination path, including all files and subdirectories, while tracking the progress of the copy operation.
        /// </summary>
        /// <param name="sourceDirectory">The path to the source directory to be copied.</param>
        /// <param name="destinationDirectory">The path to the destination directory where the source directory will be copied.</param>
        /// <param name="totalBytesCopied">A reference to a long variable that will be updated with the total number of bytes copied.</param>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination directory path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the source directory does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
        /// <summary>
        /// Copies a file from a source path to a destination path while tracking the progress of the copy operation.
        /// </summary>
        /// <param name="sourceFile">The path to the source file to be copied.</param>
        /// <param name="destinationFile">The path to the destination file where the source file will be copied.</param>
        /// <param name="totalBytesCopied">A reference to a long variable that will be updated with the total number of bytes copied.</param>
        /// <exception cref="ArgumentNullException">Thrown when the source or destination file path is null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the source file does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
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
        /// <summary>
        /// Calculates the total size of all files within a specified directory, including all subdirectories.
        /// </summary>
        /// <param name="directory">The path to the directory whose size is to be calculated.</param>
        /// <returns>The total size of all files in the directory and its subdirectories, in bytes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the directory path is null.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the specified directory does not exist.</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        private long GetDirectorySize(string directory)
        {
            return Directory.GetFiles(directory, "*", SearchOption.AllDirectories)
                            .Sum(file => new FileInfo(file).Length);
        }
        /// <summary>
        /// Updates the progress of a file or directory copy operation, including the current size, percentage progress, and copy speed.
        /// </summary>
        /// <param name="totalBytesCopied">The total number of bytes copied so far.</param>
        private void UpdateProgress(long totalBytesCopied)
        {
            CurrentSize = totalBytesCopied;
            PercentageProgress = totalBytesCopied / TotalSize;

            // Update speed
            long bytesCopiedThisSecond = totalBytesCopied - lastBytesCopied;
            lastBytesCopied = totalBytesCopied;

            currentSpeed = Utils.ByteConverter.GetReadableSize(bytesCopiedThisSecond);
        }
        #endregion
    }
}