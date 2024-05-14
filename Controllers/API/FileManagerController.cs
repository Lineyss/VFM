using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VFM.Models.Auth;
using VFM.Models.Help;
using VFM.Models.OperationSystem;
using VFM.Service;

namespace VFM.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase
    {
        private const int maxNumberItems = 20;
        private readonly FileManagerService fileManagerService;
        private readonly string host;

        public FileManagerController(FileManagerService fileManagerService)
        {
            this.fileManagerService = fileManagerService;
            host = fileManagerService.host;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get(string? path = null, int pageNumber = 1, bool isFile = false)
        {
            try
            {
                if (System.IO.File.Exists(path) && !isFile)
                {
                    string redirectUrl = $"{host}/VirtualFileManager?path={path}&pageNumber={pageNumber}&isFile={true}";
                    return StatusCode(418, redirectUrl);
                }
                else if (!isFile)
                {
                    var files = fileManagerService.GetDriversFilesAndDirectories(path);
                    int totalNumberPage = files.GetNumberPages(maxNumberItems);

                    files = files.Slice(maxNumberItems, pageNumber).ToList();
                    fileManagerService.GetOSModelsSize(files);

                    var fileManagerModel = new FileManagers
                    {
                        currentPage = pageNumber,
                        totalNumberItems = files.Count,
                        totalNumberPages = totalNumberPage,
                        currentItems = files,
                    };

                    return totalNumberPage < pageNumber ? NotFound(fileManagerModel) : Ok(fileManagerModel);
                }

                return Ok();
            }
            catch (DirectoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "createF")]
        public IActionResult Post(string path, bool isFile = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                return Ok(fileManagerService.Create(path, isFile));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPut]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "updateNameF")]
        public IActionResult Put(string path, string fileName)
        {
            try
            {
                Console.WriteLine(path);
                if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(fileName))
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                return Ok(fileManagerService.ChangeFileName(fileName, path));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpDelete]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "deleteF")]
        public IActionResult Delete([FromBody] List<string> paths)
        {
            try
            {
                if (paths == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var _paths = paths.Where(element => System.IO.File.Exists(element) || Directory.Exists(element));

                if (_paths.Count() == 0) throw new Exception(ErrorModel.WrongPath);

                foreach (string path in paths)
                {
                    if (System.IO.File.Exists(path)) fileManagerService.Delete(path, true);
                    else if (Directory.Exists(path)) fileManagerService.Delete(path, false);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("upload")]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "uploadF")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files, string path)
        {
            List<FileManager> osModels = new List<FileManager>();
            try
            {
                if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path) || files.Count == 0)
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                if (!Directory.Exists(path))
                    throw new Exception(ErrorModel.FileIsExist);

                foreach (var file in files)
                {
                    string filePath = Path.Combine(path, file.FileName);

                    FileManager? osModel = await fileManagerService.CreateAsync(filePath, file);

                    if (osModel != null) osModels.Add(osModel);
                }

                if (osModels.Count == files.Count) return Ok(osModels);
                else if (osModels.Count < files.Count && osModels.Count != 0) return StatusCode(206, osModels);
                else throw new Exception(ErrorModel.CanNotUploadFiles);
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

        [HttpPost("download")]
        [Auth(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "downloadF")]
        public IActionResult Download([FromBody] List<string> paths)
        {
            try
            {
                if (paths == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                paths = paths.Where(element => System.IO.File.Exists(element) || Directory.Exists(element)).ToList();

                if (paths.Count == 0) throw new Exception(ErrorModel.WrongPath);

                if (System.IO.File.Exists(paths[0]) && paths.Count == 1)
                {
                    return fileManagerService.downloadFile(paths[0]);
                }
                else if (Directory.Exists(paths[0]) && paths.Count == 1)
                {
                    return fileManagerService.downloadDirectory(paths[0]);
                }
                else if (paths.Count > 1)
                {
                    return fileManagerService.downloadAll(paths);
                }

                throw new Exception(ErrorModel.FilesOrDirectoriesIsNotExist);
            }
            catch (UnauthorizedAccessException e)
            {
                string filePath = e.Message.Split("'")[1];
                return BadRequest(new ErrorModel($"Не удалось скачать файл по пути: {filePath}"));
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorModel(e.Message));
            }
        }

    }
}
