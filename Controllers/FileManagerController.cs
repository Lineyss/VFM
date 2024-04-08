using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Extensions;
using VFM.Controllers.Base;
using VFM.Models.View;
using VFM.Services;
using System;
using VFM.Models;
/*using SharpCompress.Archives;
using SharpCompress.Archives.SevenZip;*/


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
        public IActionResult Get([FromHeader] string? path = null, [FromHeader] int pageNumber = 1, [FromHeader] bool isFile = false)
        {
            try
            {
                if(isFile)
                {
                    return Ok();
                }

                var files = sFileManager.GetFilesAndDirectories(path);
                var fileManagerModel = new VFileManagerModel
                {
                    currentPage = pageNumber,
                    totalNumberItems = files.Count,
                    totalNumberPages = files.GetNumberPages(maxNumberItems),
                    currentItems = files.Slice(maxNumberItems, pageNumber).ToList(),
                };

                return Ok(fileManagerModel);

            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public IActionResult Post([FromHeader] string path, [FromHeader] bool isFile = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) new Exception("Путь не может быть пустым");

                string _url = $"{url + HttpContext.Request.Path}?Path={path}";
                return Created(path, sFileManager.Create(path, isFile));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{fileName}")]
        public IActionResult Put(string fileName, [FromHeader] string path)
        {
            try
            {
                Console.WriteLine(path);
                if (string.IsNullOrWhiteSpace(path)) new Exception("Путь не может быть пустым");
                if (string.IsNullOrWhiteSpace(fileName)) new Exception("Название не может быть пустым");

                return Ok(sFileManager.ChangeFileName(fileName, path));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete([FromHeader] string path, [FromHeader] bool isFile = true)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(path)) new Exception("Путь не может быть пустым");
                sFileManager.Delete(path, isFile);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFiles(IFormFileCollection files, [FromHeader] string path)
        {
            List<OSModel> osModels = new List<OSModel>();
            try
            {
                if (string.IsNullOrWhiteSpace(path)) throw new Exception("Путь не может быть пустым");
                if (System.IO.File.Exists(path)) throw new Exception("Должен быть указан путь до папки");
                if (!Directory.Exists(path)) throw new Exception("Директории с таким путем не существует");

                foreach (var file in files)
                {
                    string filePath = Path.Combine(path, file.FileName);

                    OSModel? osModel = await sFileManager.CreateAsync(filePath, file);

                    if (osModel != null) osModels.Add(osModel);
                }

                if (osModels.Count() == files.Count())
                    return Ok(osModels);
                else if (osModels.Count() < files.Count() && osModels.Count() != 0)
                    return StatusCode(206, osModels);
                else
                    throw new Exception("Не удалось загрузить файлы");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("download")]
        public IActionResult Download(string path)
        {
            try
            {
                if (System.IO.File.Exists(path))
                    return sFileManager.downloadFile(path);
                else if (Directory.Exists(path))
                    return sFileManager.downloadDirectory(path);

                throw new Exception("Файла или папки с таким путем не существует");
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
