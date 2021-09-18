using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Controllers
{
    [Route("api/server")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        FileServerDBContext _contetx;

        public ServerController(FileServerDBContext contetx)
        {
            _contetx = contetx;
        }
         
        [HttpGet("server")]
        public async Task<IActionResult> GetServer(string system,string subsystem,string ext,string size,string record)
        {
            var server = await _contetx.TblServers.Where(a => a.Active).OrderBy(a => a.Priority).FirstOrDefaultAsync();

            if (server == null)
            {
                return new JsonResult(new { data = "No server is active!" });
            }

            if((server.Capacity-server.Used) < (double.Parse(size)/1024/1024))
            {
                return new JsonResult(new { data = "No space available!" });
            }

            var file = new TblFile {
                System = system,
                SubSystem = subsystem,
                Extention = ext,
                Size = (double.Parse(size)/1024/1024),
                Record = Int64.Parse(record),
                CreatedDate = DateTime.Now,
                ServerId = server.Id
            };
            await _contetx.TblFiles.AddAsync(file);
            await _contetx.SaveChangesAsync();

            return new JsonResult(new { data = new { server = server.Address , id = file.Id } });
        }

        [HttpGet("result/{name}/{id}/{status}")]
        public async Task<IActionResult> GetResult(string name, string id, string status)
        {
            try
            {
                var file = await _contetx.TblFiles.FindAsync(Int64.Parse(id));

                if (status == "true")
                {
                    file.Name = name;
                    file.IsComplete = true;
                }
                else
                {
                    _contetx.TblFiles.Remove(file);
                }

                await _contetx.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception e)
            {
                return NotFound(e.Message);
            }
        }
    }
}
