using System;
using System.IO;

namespace AnimalShelter
{
    public static class PhotoHelper
    {
        // путь к папке с фото
        public static string PhotosFolder
        {
            get
            {
                string folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Photos");
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);
                return folder;
            }
        }

        // копирует выбранный файл в папку Photos и возвращает новое имя
        public static string ImportPhoto(string sourcePath, string nickname)
        {
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath))
                return null;

            string ext = Path.GetExtension(sourcePath).ToLower();
            // уникальное имя, чтобы не перезаписать
            string fileName = $"{nickname}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            string destPath = Path.Combine(PhotosFolder, fileName);

            File.Copy(sourcePath, destPath, true);
            return fileName;
        }

        // удаляет файл фото из папки Photos
        public static void DeletePhoto(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;

            string path = Path.Combine(PhotosFolder, fileName);
            if (File.Exists(path))
                File.Delete(path);
        }
    }
}