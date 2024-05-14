using Microsoft.AspNetCore.Mvc;
using System;
using System.IO.Compression;
using System.Runtime.InteropServices;
using VFM.Models.Help;
using VFM.Models.OperationSystem;

namespace VFM.Service
{
    public class FileManagerService
    {
        public readonly string os;
        private readonly string iconPathDriver;
        private readonly string iconPathDocument;
        private readonly string iconPathFolder;
        public readonly string host;

        private List<FileManager> files = new List<FileManager>();

        public FileManagerService(IHttpContextAccessor uriHelper)
        {
            var request = uriHelper.HttpContext.Request;
            host = $"{request.Scheme}://{request.Host.Value}";
            iconPathDriver = $"{host}/image/drive.svg";
            iconPathDocument = $"{host}/image/document.svg";
            iconPathFolder = $"{host}/image/folder.svg";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) os = "Windows";
            else os = "Unix";
        }
        public List<FileManager> GetDriversFilesAndDirectories(string? path)
        {
            if (os == "Windows") GetDriversFilesAndDirectoriesWindows(path);
            else GetFilesAndDirectoriesLinux(path);

            return files;
        }

        public FileManager Create(string path, bool isFile)
        {
            if (File.Exists(path)) throw new Exception(ErrorModel.FileIsExist);
            if (Directory.Exists(path)) throw new Exception(ErrorModel.DirectoryIsExist);

            if (isFile)
            {
                return CreateFile(path);
            }
            else
            {
                return CreateDirectory(path);
            }
        }

        public async Task<FileManager?> CreateAsync(string path, IFormFile file)
        {
            path = ChangeFileNameIfExist(path);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if (File.Exists(path))
            {
                return new FileManager
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(path),
                    fullPath = path,
                    dateCreate = File.GetCreationTime(path).ToString(),
                    dateChange = File.GetLastWriteTime(path).ToString(),
                };
            }

            return null;
        }

        public FileContentResult downloadFile(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            string exist = Path.GetExtension(path);
            exist = exist.Substring(1, exist.Length - 1);

            string contentType = "application/" + exist;

            return new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(path)
            };
        }

        public FileContentResult downloadAll([FromBody] List<string> paths)
        {
            string zipPath = Environment.CurrentDirectory + "/downloadAll.zip";
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipArchive archive;
            using (archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (string path in paths)
                {
                    archive.CreateEntryFromFile(path, Path.GetFileName(path));
                }
            }

            FileContentResult fileContentResult = downloadFile(zipPath);

            File.Delete(zipPath);

            return fileContentResult;
        }

        public FileContentResult downloadDirectory(string path)
        {
            string pathToZip = $"{Directory.GetParent(path).FullName}/{Path.GetFileNameWithoutExtension(path)}.zip";

            ZipFile.CreateFromDirectory(path, pathToZip);

            FileContentResult fileContentResult = downloadFile(pathToZip);

            File.Delete(pathToZip);

            return fileContentResult;
        }

        public void Delete(string path, bool isFile)
        {
            try
            {
                if (isFile) File.Delete(path);
                else Directory.Delete(path, true);
            }
            catch
            {
                throw new Exception(isFile ? ErrorModel.CanNotDeleteFile : ErrorModel.CanNotDeleteDirectories);
            }
        }

        public FileManager ChangeFileName(string fileName, string path)
        {
            if (File.Exists(path))
            {
                string newPath = path.Replace(Path.GetFileNameWithoutExtension(path), fileName);
                if (isExist(newPath)) throw new Exception(ErrorModel.FileIsExist);
                File.Move(path, newPath);
                return new FileManager
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(newPath),
                    fullPath = newPath,
                    dateCreate = File.GetCreationTime(newPath).ToString(),
                    dateChange = File.GetLastWriteTime(newPath).ToString(),
                };
            }
            else if (Directory.Exists(path))
            {
                string newPath = Path.Combine(Path.GetDirectoryName(path), fileName);
                if (isExist(newPath)) throw new Exception(ErrorModel.DirectoryIsExist);
                Directory.Move(path, newPath);
                return new FileManager
                {
                    icon = iconPathFolder,
                    fileName = Path.GetFileName(newPath),
                    fullPath = newPath,
                    dateCreate = File.GetCreationTime(newPath).ToString(),
                    dateChange = File.GetLastWriteTime(newPath).ToString(),
                };
            }

            throw new Exception(ErrorModel.WrongPath);
        }

        public IEnumerable<FileManager> GetOSModelsSize(IEnumerable<FileManager> osModels)
        {
            foreach (var element in osModels)
            {
                try
                {
                    element.size = GetSize(element);
                }
                catch { }
            }

            return osModels;
        }

        private FileManager CreateFile(string path)
        {
            try
            {
                FileStream file = File.Create(path);

                var fileModel = new FileManager
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(path),
                    fullPath = path,
                    dateCreate = File.GetCreationTime(path).ToString(),
                    dateChange = File.GetLastWriteTime(path).ToString(),
                };

                file.Close();

                return fileModel;
            }
            catch
            {
                throw new Exception(ErrorModel.CanNotCreateFile);
            }
        }

        private FileManager CreateDirectory(string path)
        {
            try
            {
                DirectoryInfo file = Directory.CreateDirectory(path);
                return new FileManager
                {
                    icon = iconPathFolder,
                    fileName = file.Name,
                    fullPath = path,
                    dateCreate = file.CreationTime.ToString(),
                    dateChange = file.CreationTime.ToString(),
                    isFile = false
                };
            }
            catch
            {
                throw new Exception(ErrorModel.CanNotCreateDirectory);
            }
        }

        private void GetFilesAndDirectoriesLinux(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) path = @"/";

            GetFiles(path);
            GetDirectories(path);
        }

        private void GetDriversFilesAndDirectoriesWindows(string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) GetDrivers();
            else
            {
                GetFiles(path);
                GetDirectories(path);
            }
        }

        private List<FileManager> GetFiles(string path)
        {
            string[] sfiles = Directory.GetFiles(path);
            foreach (var element in sfiles)
            {
                string fullPath = Path.GetFullPath(element);
                files.Add(new FileManager
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(fullPath),
                    fullPath = fullPath,
                    dateCreate = File.GetCreationTime(fullPath).ToString(),
                    dateChange = File.GetLastWriteTime(fullPath).ToString(),
                    isFile = true
                });
            }

            return files;
        }

        private List<FileManager> GetDirectories(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var element in dirs)
            {
                string fullPath = Path.GetFullPath(element);
                files.Add(new FileManager
                {
                    icon = iconPathFolder,
                    fileName = Path.GetFileName(fullPath),
                    fullPath = fullPath,
                    dateCreate = Directory.GetCreationTime(fullPath).ToString(),
                    dateChange = Directory.GetLastWriteTime(fullPath).ToString(),
                    isFile = false
                });
            }

            return files;
        }

        private void GetDrivers()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (var element in allDrives)
            {
                files.Add(new FileManager
                {
                    icon = iconPathDriver,
                    fileName = element.Name,
                    fullPath = element.RootDirectory.FullName,
                    dateCreate = element.RootDirectory.CreationTime.ToString(),
                    dateChange = element.RootDirectory.LastWriteTime.ToString(),
                    isFile = false
                });
            }
        }

        private string ChangeFileNameIfExist(string path)
        {
            if (File.Exists(path) || Directory.Exists(path))
            {
                var filePathArr = path.Split('.');
                filePathArr[0] += 1;
                path = string.Join('.', filePathArr);
                path = ChangeFileNameIfExist(path);
            }

            return path;
        }

        private bool isExist(string path)
        {
            if (File.Exists(path) || Directory.Exists(path)) return true;
            return false;
        }

        private string GetSize(FileManager element)
        {
            double size;
            DriveInfo? drive = DriveInfo.GetDrives().FirstOrDefault(drive => drive.Name == element.fullPath);
            if (drive == null)
                size = element.isFile ? new FileInfo(element.fullPath).Length : GetFolderSize(element.fullPath);
            else
                size = drive.TotalSize;

            string[] suffixes = { "Байт", "Кбайт", "Мбайт", "Гбайт" };
            int suffixIndex = 0;

            while (size >= 1024 && suffixIndex < suffixes.Length - 1)
            {
                size /= 1024;
                suffixIndex++;
            }

            return $"{size:F2} {suffixes[suffixIndex]}";
        }

        private double GetFolderSize(string path)
        {
            double size = 0;

            var files = Directory.GetFiles(path);
            foreach (var element in files)
            {
                size += element.Length;
            }

            var directoryes = Directory.GetDirectories(path);
            foreach (var element in directoryes)
            {
                size += GetFolderSize(element);
            }

            return size;
        }
    }
}