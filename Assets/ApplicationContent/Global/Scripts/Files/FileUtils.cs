using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Global.Logger;

namespace Global.Files
{
    /// <summary>
    /// <para>Набор вспомогательных методов для работы с файлами.</para>
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// <para>Если файл не существует, то создать пустой файл.</para>
        /// </summary>
        /// <param name="path">путь к файлу</param>
        /// <param name="emptyFileContent">данные записываемые в файл при создании</param>
        /// <param name="createDirectory">если директории, в которой должен находиться файл не существует, нужно ли создавать эту директорию</param>
        public static void EnsureFileExists(string path, string emptyFileContent = "", bool createDirectory = false)
        {
            string directory = Path.GetDirectoryName(path);
            if (createDirectory)
            {
                EnsureDirectoryExists(directory);
            }
            else
            {
                if (!Directory.Exists(directory))
                {
                    AppLogger.Error($"Directory {directory} does not exist.");
                    return;
                }
            }

            if (!File.Exists(path))
            {
                File.WriteAllText(path, emptyFileContent);
            }
        }

        /// <summary>
        /// <para>Если директория не существует, то создать директорию.</para>
        /// </summary>
        /// <param name="path">путь к директории</param>
        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// <para>Удалить файл.</para>
        /// </summary>
        /// <param name="filePath">путь до файла</param>
        public static void RemoveFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    File.Delete(filePath);
                }
                catch (IOException ex)
                {
                    AppLogger.Warning($"Failed to delete file: {filePath}\n{ex.Message}");
                }
            }
        }

        /// <summary>
        /// <para>Получить имена файлов в указанной директории.</para>
        /// </summary>
        /// <param name="directory">директория</param>
        /// <param name="excludedNames">исключаемые имена файлов. Если в папке хранятся нежелательные файлы - то можно передать их имена в данном параметре и метод не вернет данные файлы</param>
        /// <param name="excludedExtensions">исключаемые расширения файлов. Если в папке хранятся нежелательные файлы - то можно передать их расширения в данном параметре и метод не вернет данные файлы</param>
        /// <returns>список имен файлов хранящихся в ней</returns>
        public static string[] GetFilesFromDirectory(string directory, ISet<string> excludedNames = default, ISet<string> excludedExtensions = default)
        {
            if (!Directory.Exists(directory))
            {
                return Array.Empty<string>();
            }

            var files = GetFileNames(directory);

            files = ExcludeFilesByName(excludedNames, files);

            files = ExcludeFilesByExtension(excludedExtensions, files);

            return files.ToArray();
        }

        private static IList<string> GetFileNames(string directory)
        {
            IList<string> files = new List<string>();
            string[] filesPaths = Directory.GetFiles(directory);
            foreach (string file in filesPaths)
            {
                files.Add(Path.GetFileName(file));
            }

            return files;
        }

        private static IList<string> ExcludeFilesByName(ISet<string> excludedNames, IList<string> files)
        {
            if (excludedNames != null && excludedNames.Count > 0)
            {
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    int pointIndex = files[i].LastIndexOf('.');
                    int length = pointIndex == -1 ? files[i].Length : pointIndex;
                    string fileName = files[i].Substring(0, length);
                    if (excludedNames.Contains(fileName))
                    {
                        files.RemoveAt(i);
                    }
                }
            }

            return files;
        }

        private static IList<string> ExcludeFilesByExtension(ISet<string> excludedExtensions, IList<string> files)
        {
            if (excludedExtensions != null && excludedExtensions.Count > 0)
            {
                for (int i = files.Count - 1; i >= 0; i--)
                {
                    int pointIndex = files[i].LastIndexOf('.') + 1;
                    int length = pointIndex == -1 ? files[i].Length : pointIndex;
                    string fileExtensions = files[i].Substring(length);
                    if (excludedExtensions.Contains(fileExtensions))
                    {
                        files.RemoveAt(i);
                    }
                }
            }

            return files;
        }
    }
}