using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using VFM.Services;
using System;
using VFM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VFM.Controllers.Base;

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
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public IActionResult Get(string? path = null, [FromHeader] int pageNumber = 1, [FromHeader] bool isFile = false)
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
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, PropertyValue = "createF", PropertyName ="True")]
        public IActionResult Post([FromHeader] string path, [FromHeader] bool isFile = true)
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
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, PropertyValue = "updateNmaeF", PropertyName = "True")]
        public IActionResult Put(string fileName, [FromHeader] string path)
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
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, PropertyValue = "deleteF", PropertyName = "True")]
        public IActionResult Delete([FromHeader] string path, [FromHeader] bool isFile = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) 
                    throw new Exception(ErrorModel.AllFieldsMostBeFields);

                sFileManager.Delete(path, isFile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("upload")]
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, PropertyValue = "uploadF", PropertyName = "True")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files, [FromHeader] string path)
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
        [UserAuthorization(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, PropertyValue = "downloadF", PropertyName = "True")]
        public IActionResult Download(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                    return sFileManager.downloadFile(path);
                else if (Directory.Exists(path))
                    return sFileManager.downloadDirectory(path);

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
