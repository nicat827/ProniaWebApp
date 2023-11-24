using Microsoft.Identity.Client;
using Pronia.Utilities.Enums;

namespace Pronia.Utilities.Extensions
{
    public static class FileHelper
    {
        public static bool IsValidType(this IFormFile file, FileType type)
        {
            if (type == FileType.Image)
            {
                if (file.ContentType.Contains("image/")) return true;
                return false;
            }
            else if (type == FileType.Video)
            {
                if (file.ContentType.Contains("video/")) return true;
                return false;
            }
            else if (type == FileType.Audio)
            {
                if (file.ContentType.Contains("audio/")) return true;
                return false;
            }
            else return false;
        }

        public static bool IsValidSize(this IFormFile file, int maxSize, FileSize size=FileSize.Kilobite)
        {
            if (size == FileSize.Kilobite)
            {
                if (file.Length <= maxSize * 1024) return true;
                else return false;

            }
            else if (size == FileSize.Megabite)
            {
                if (file.Length <= maxSize * 1024 * 1024) return true;
                else return false;

            }

            else if (size == FileSize.Gigabite)
            {
                if (file.Length <= maxSize * 1024 * 1024 * 1024) return true;
                else return false;
            }
            else return false;
        }

        public static async Task<string> CreateFileAsync(this  IFormFile file, string rootPath, params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName.Substring(file.FileName.LastIndexOf('.'));
            string path = fileName.GetPath(rootPath, folders);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }


        public static void DeleteFile(this string fileName, string rootPath, params string[] folders)
        {
            string path = fileName.GetPath(rootPath, folders);

            if (File.Exists(path))
            {
                File.Delete(path);
            }


        }

        public static string GetPath(this string  fileName, string rootPath, string[] folders)
        {
            string path = rootPath;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }
            return Path.Combine(path, fileName);

        }

    }
}
