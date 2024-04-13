using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using VFM.Models;

namespace VFM.Services
{
    public class FileManager
    {
        public readonly string os;
        private readonly string iconPathDriver;
        private readonly string iconPathDocument;
        private readonly string iconPathFolder;

        private List<OSModel> files = new List<OSModel>();

        public FileManager(string host)
        {
            iconPathDriver = $"{host}/image/drive.svg";
            iconPathDocument = $"{host}/image/document.svg";
            iconPathFolder = $"{host}/image/folder.svg";

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) os = "Windows";
            else os = "Unix";
        }
        public List<OSModel> GetDriversFilesAndDirectories(string? path)
        {
            if (os == "Windows") GetDriversFilesAndDirectoriesWindows(path);
            else GetFilesAndDirectoriesLinux(path);

            return files;
        }

        public OSModel Create(string path, bool isFile)
        {
            if (isFile)
               return CreateFile(path);
            else
                return CreateDirectory(path);
        }

        public async Task<OSModel>? CreateAsync (string path, IFormFile file)
        {
            if (File.Exists(path)) throw new Exception("Такой файл уже существует");

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            if(File.Exists(path))
            {
                return new OSModel
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(path),
                    fullPath = path,
                    dateCreate = File.GetCreationTime(path).ToString(),
                    dateChange = File.GetLastWriteTime(path).ToString(),
                    size = file.Length
                };
            }

            return null;
        }

        public FileContentResult downloadFile(string path)
        {
            byte[] fileBytes = File.ReadAllBytes(path);

            string contentType = "application/" + Path.GetExtension(path);

            FileContentResult fileContentResult = new FileContentResult(fileBytes, contentType);
            fileContentResult.FileDownloadName = Path.GetFileName(path);

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
                if(isFile)
                    File.Delete(path);
                else
                    Directory.Delete(path, true);
            }
            catch
            {
                throw new Exception("Не удалось удалить" + (isFile ? "файл": "папку"));
            }
        }

        public OSModel ChangeFileName(string fileName, string path)
        {
            try
            {
                if(File.Exists(path))
                {
                    string newPath = path.Replace(Path.GetFileNameWithoutExtension(path), fileName);
                    File.Move(path, newPath);
                    return new OSModel
                    {
                        icon = iconPathDocument,
                        fileName = Path.GetFileName(newPath),
                        fullPath = newPath,
                        dateCreate = File.GetCreationTime(newPath).ToString(),
                        dateChange = File.GetLastWriteTime(newPath).ToString(),
                        size = new FileInfo(newPath).Length
                    };
                }
                else if(Directory.Exists(path))
                {
                    string newPath = Path.Combine(Path.GetDirectoryName(path), fileName);
                    Directory.Move(path, newPath);
                    Console.WriteLine(newPath);
                    return new OSModel
                    {
                        icon = iconPathFolder,
                        fileName = Path.GetFileName(newPath),
                        fullPath = newPath,
                        dateCreate = File.GetCreationTime(newPath).ToString(),
                        dateChange = File.GetLastWriteTime(newPath).ToString(),
                        size = GetFolderSize(newPath)
                    };
                }
            }
            catch
            {
                throw new Exception("Не удалось изменить название файла");
            }

            throw new Exception("Не верный путь");
        }

        public IEnumerable<OSModel> GetOSModelsSize(IEnumerable<OSModel> osModels)
        {
            foreach (var element in osModels)
            {
                try
                {
                    if (element.icon == iconPathDocument)
                        element.size = new FileInfo(element.fullPath).Length;
                    else
                        element.size = GetFolderSize(element.fullPath);
                }
                catch { }
            }

            return osModels;
        }

        private OSModel CreateFile(string path)
        {
            try
            {
                FileStream file = File.Create(path);

                var fileModel = new OSModel
                {
                    icon = iconPathDocument,
                    fileName = System.IO.Path.GetFileName(path),
                    fullPath = path,
                    dateCreate = File.GetCreationTime(path).ToString(),
                    dateChange = File.GetLastWriteTime(path).ToString(),
                    size = file.Length
                };

                file.Close();

                return fileModel;
            }
            catch
            {
                throw new Exception("Не удалось создать файл");
            }
        }

        private OSModel CreateDirectory(string path)
        {
            try
            {
                DirectoryInfo file = Directory.CreateDirectory(path);
                return new OSModel
                {
                    icon = iconPathDocument,
                    fileName = Path.GetDirectoryName(path),
                    fullPath = path,
                    dateCreate = Directory.GetCreationTime(path).ToString(),
                    dateChange = Directory.GetLastWriteTime(path).ToString(),
                    size = GetFolderSize(path),
                    isEmpty = true
                };
            }
            catch
            {
                throw new Exception("Не удалось создать папку");
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

        private List<OSModel> GetFiles(string path)
        {
            string[] sfiles = Directory.GetFiles(path);
            foreach(var element in sfiles)
            {
                string fullPath = Path.GetFullPath(element);
                files.Add(new OSModel
                {
                    icon = iconPathDocument,
                    fileName = Path.GetFileName(fullPath),
                    fullPath = fullPath,
                    dateCreate = File.GetCreationTime(fullPath).ToString(),
                    dateChange = File.GetLastWriteTime(fullPath).ToString(),
                });
            }

            return files;
        }

        private List<OSModel> GetDirectories(string path)
        {
            string[] dirs = Directory.GetDirectories(path);
            foreach (var element in dirs)
            {
                string fullPath = Path.GetFullPath(element);
                files.Add(new OSModel
                {
                    icon = iconPathFolder,
                    fileName = Path.GetFileName(fullPath),
                    fullPath = fullPath,
                    dateCreate = Directory.GetCreationTime(fullPath).ToString(),
                    dateChange = Directory.GetLastWriteTime(fullPath).ToString(),
                });
            }

            return files;
        }

        private void GetDrivers()
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach(var element in allDrives)
            {
                files.Add(new OSModel
                {
                    icon = iconPathDriver,
                    fileName = element.Name,
                    fullPath = element.RootDirectory.FullName,
                    dateCreate = element.RootDirectory.CreationTime.ToString(),
                    dateChange = element.RootDirectory.LastWriteTime.ToString(),
                    size = element.TotalSize
                });
            }
        }

        private long GetFolderSize(string path, long size = 0)
        {
            try
            {
                List<string> filesAndFolder = Directory.GetFiles(path).ToList();
                filesAndFolder = filesAndFolder.Concat(Directory.GetDirectories(path)).ToList();

                foreach(var element in filesAndFolder)
                {
                    if(Directory.Exists(element)) 
                        size += GetFolderSize(element, size);
                    else
                        size += new FileInfo(element).Length;
                }
            }
            catch (Exception e) 
            {
                Console.WriteLine(e.Message);
            }

            return size;
        }
    }
}
