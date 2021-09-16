using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileLoaderWebApi.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        public FileController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        [HttpPost("upload")]
        public async Task<IActionResult> Post([FromForm] UploadVm model)
        {
            try
            {
                string name = Guid.NewGuid().ToString();

                var filePath = Path.Combine("files", name + Path.GetExtension(model.file.FileName));

                HttpClient client = new HttpClient();

                //Upload Process
                if (model.file.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.file.CopyToAsync(stream);
                    }
                }
                else //Zero Byte File
                {
                    await client.GetAsync($"{Configuration["Address:MANAGER_SERVER_ADDRESS"]}/{name}/{model.id}/false");
                    return new JsonResult(new { data = new { result = "Zero byte file", status = false } });
                }

                //Tell ManageServer File Upload Complete
                //Response Client File Upload Complete
                try
                {
                    var res = await client.GetAsync($"{Configuration["Address:MANAGER_SERVER_ADDRESS"]}/{name}/{model.id}/true");
                    if (!res.IsSuccessStatusCode)
                    {
                        return new JsonResult(new { data = new { result = "ManageServer not response", status = false } });
                    }
                }
                catch (Exception e)
                {
                    return new JsonResult(new { data = new { result = e.Message, status = false } });
                }

                return new JsonResult(new { data = new { result = "Complete Successfully", status = true } });
            }
            catch (Exception e)
            {
                return new JsonResult(new { data = new { result = e.Message, status = false } });
            }

        }

        [HttpGet("download/{*name}")]
        public async Task<IActionResult> Get(string name)
        {
            try
            {
                Stream stream = new FileStream($"files/{name}", FileMode.OpenOrCreate);

                if (stream == null)
                    return NotFound();

                return File(stream, "application/octet-stream");
            }
            catch (Exception e)
            {
                return Ok(new { e.Message });
            }

        }

        [HttpGet("delete/{*name}")]
        public async Task<IActionResult> Delete(string name)
        {
            try
            {
                System.IO.File.Delete($"files/{name}");

                return Ok(true);
            }
            catch (Exception e)
            {
                return Ok(new { e.Message });
            }

        }

        public class UploadVm
        {
            public IFormFile file { set; get; }
            public string id { set; get; }
            
        }
    }


}

