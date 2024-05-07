﻿using Microsoft.AspNetCore.Mvc;
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

        public async Task<OSModel>? CreateAsync (string path, IFormFile file)
        {
            path = CheckExistFile(path);

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

            string exist = Path.GetExtension(path);
            exist = exist.Substring(1, exist.Length - 1);

            string contentType = "application/" + exist;

            return new FileContentResult(fileBytes, contentType)
            {
                FileDownloadName = Path.GetFileNameWithoutExtension(path)
            };
        }

        public FileContentResult downloadAll ([FromBody] List<string> paths)
        {
            string zipPath = Environment.CurrentDirectory + "/downloadAll.zip";
            if (File.Exists(zipPath)) File.Delete(zipPath);
            ZipArchive archive;
            using (archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach(string path in paths)
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
                if(isFile)
                    File.Delete(path);
                else
                    Directory.Delete(path, true);
            }
            catch
            {
                throw new Exception(isFile ? ErrorModel.CanNotDeleteFile : ErrorModel.CanNotDeleteDirectories);
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
                throw new Exception(ErrorModel.CanNotChangeFileName);
            }

            throw new Exception(ErrorModel.WrongPath);
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
                    fileName = Path.GetFileName(path),
                    fullPath = path,
                    dateCreate = File.GetCreationTime(path).ToString(),
                    dateChange = File.GetLastWriteTime(path).ToString(),
                    size = 0
                };

                file.Close();

                return fileModel;
            }
            catch
            {
                throw new Exception(ErrorModel.CanNotCreateFile);
            }
        }

        private OSModel CreateDirectory(string path)
        {
            try
            {
                DirectoryInfo file = Directory.CreateDirectory(path);
                return new OSModel
                {
                    icon = iconPathFolder,
                    fileName = file.Name,
                    fullPath = path,
                    dateCreate = file.CreationTime.ToString(),
                    dateChange = file.CreationTime.ToString(),
                    size = 0,
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
                    isFile = true
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
                    isFile = false
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
                    size = element.TotalSize,
                    isFile = false
                });
            }
        }

        private string CheckExistFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                var filePathArr = filePath.Split('.');
                filePathArr[0] += 1;
                filePath = string.Join('.', filePathArr);
                filePath = CheckExistFile(filePath);
            }

            return filePath;
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
