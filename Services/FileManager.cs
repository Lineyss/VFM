using System.Drawing;
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
        public List<OSModel> GetFilesAndDirectories(string? path)
        {
            if (os == "Windows") GetFilesAndDirectoriesWindows(path);
            else GetFilesAndDirectoriesLinux(path);

            return files;
        }

        public OSModel Create(string Path, bool isFile)
        {
            if (isFile)
               return CreateFile(Path);
            else
                return CreateDirectory(Path);
        }

        public void Delete(string Path, bool isFile)
        {
            try
            {
                if(isFile)
                {
                    File.Delete(Path);
                }
                else
                {
                    Directory.Delete(Path, true);
                }
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

        private OSModel CreateFile(string Path)
        {
            try
            {
                FileStream file = File.Create(Path);

                var fileModel = new OSModel
                {
                    icon = iconPathDocument,
                    fileName = System.IO.Path.GetFileName(Path),
                    fullPath = Path,
                    dateCreate = File.GetCreationTime(Path).ToString(),
                    dateChange = File.GetLastWriteTime(Path).ToString(),
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

        private OSModel CreateDirectory(string Path)
        {
            try
            {
                DirectoryInfo file = Directory.CreateDirectory(Path);
                return new OSModel
                {
                    icon = iconPathDocument,
                    fileName = System.IO.Path.GetDirectoryName(Path),
                    fullPath = Path,
                    dateCreate = Directory.GetCreationTime(Path).ToString(),
                    dateChange = Directory.GetLastWriteTime(Path).ToString(),
                    size = GetFolderSize(Path),
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
            if (string.IsNullOrWhiteSpace(path)) path = @"\";

            GetFiles(path);
            GetDirectories(path);
        }

        private void GetFilesAndDirectoriesWindows(string? path)
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
                    size = new FileInfo(fullPath).Length
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
                    size = GetFolderSize(fullPath),
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

        private long GetFolderSize(string path)
        {
            long size = 0;
            try
            {
                foreach(var element in Directory.GetFiles(path))
                {
                    size += new FileInfo(element).Length;
                }
            }
            catch { }

            return size;
        }
    }
}
