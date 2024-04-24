using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using VFM.Services;
using System;
using VFM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VFM.Controllers.Base;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace VFM.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileManagerController : ControllerBase, IAPIController<VFileManagerModel, string>
    {
        private const int maxNumberItems = 20;
        private readonly FileManager sFileManager;
        private readonly string url;

        public FileManagerController(IHttpContextAccessor uriHelper)
        {
            var request = uriHelper.HttpContext.Request;
            url = $"{request.Scheme}://{request.Host.Value}";
            sFileManager = new FileManager(url);
        }
        
        [HttpGet]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get(string? path = null, int pageNumber = 1, bool isFile = false)
        {
            try
            {
                if(isFile)
                {
                    return Ok();
                }

                var files = sFileManager.GetDriversFilesAndDirectories(path);
                int totalNumberPage = files.GetNumberPages(maxNumberItems);
                 
                files = files.Slice(maxNumberItems, pageNumber).ToList();
                if (path != null) files = sFileManager.GetOSModelsSize(files).ToList();

                var fileManagerModel = new VFileManagerModel
                {   
                    currentPage = pageNumber,
                    totalNumberItems = files.Count,
                    totalNumberPages = totalNumberPage,
                    currentItems = files,
                };
                
                return totalNumberPage < pageNumber ? NotFound(fileManagerModel):Ok(fileManagerModel);

            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception e) 
             {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "createF")]
        public IActionResult Post(string path, bool isFile = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) 
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                string _url = $"{url + HttpContext.Request.Path}?Path={path}";
                return Created(path, sFileManager.Create(path, isFile));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{fileName}")]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "updateNmaeF")]
        public IActionResult Put(string fileName, string path)
        {
            try
            {
                Console.WriteLine(path);
                if (string.IsNullOrWhiteSpace(path) || string.IsNullOrWhiteSpace(fileName)) 
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                return Ok(sFileManager.ChangeFileName(fileName, path));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "deleteF")]
        public IActionResult Delete(List<string> paths)
        {
            try
            {
                if (paths == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                var _paths = paths.Where(element => System.IO.File.Exists(element) || Directory.Exists(element));

                if (_paths.Count() == 0) throw new Exception(ErrorModel.WrongPath);

                foreach(string path in paths)
                {
                    if (System.IO.File.Exists(path)) sFileManager.Delete(path, true);
                    else sFileManager.Delete(path, false);
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("upload")]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "uploadF")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files, string path)
        {
            List<OSModel> osModels = new List<OSModel>();
            try
            {
                if (string.IsNullOrWhiteSpace(path) || System.IO.File.Exists(path))
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                if (!Directory.Exists(path)) 
                    throw new Exception(ErrorModel.FileIsExist);

                foreach (var file in files)
                {
                    string filePath = Path.Combine(path, file.FileName);

                    OSModel? osModel = await sFileManager.CreateAsync(filePath, file);

                    if (osModel != null) osModels.Add(osModel);
                }

                if (osModels.Count == files.Count) return Ok(osModels);
                else if (osModels.Count < files.Count && osModels.Count != 0) return StatusCode(206, osModels);
                else throw new Exception(ErrorModel.CanNotUploadFiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("download")]
        [UserAuthorization(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme + "," + JwtBearerDefaults.AuthenticationScheme, PropertyValue = "True", PropertyName = "downloadF")]
        public IActionResult Download(string paths)
        {
            try
            {
                return sFileManager.downloadFile(paths);
/*                if (paths == null) throw new Exception(ErrorModel.AllFieldsMostBeFields);

                paths = paths.Where(element => System.IO.File.Exists(element) || Directory.Exists(element)).ToList();

                if (paths.Count == 0) throw new Exception(ErrorModel.WrongPath);

                if (System.IO.File.Exists(paths[0]) && paths.Count == 1) return sFileManager.downloadFile(paths[0]);
                else if (Directory.Exists(paths[0]) && paths.Count == 1) return sFileManager.downloadDirectory(paths[0]);

                return sFileManager.downloadAll(paths);*/

                throw new Exception(ErrorModel.FilesOrDirectoriesIsNotExist);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Не используемые методы
        [NonAction]
        public IActionResult Delete(int ID)
        {
            throw new NotImplementedException();
        }

        [NonAction]
        public IActionResult Post(string FileName)
        {
            throw new NotImplementedException();
        }
        [NonAction]
        public IActionResult Put(VFileManagerModel model)
        {
            throw new NotImplementedException();
        }
        [NonAction]
        public IActionResult Get()
        {
            throw new NotImplementedException();
        }
    }
}
