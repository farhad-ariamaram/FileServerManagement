using FileServerManagementWepApp.Models;
using FileServerManagementWepApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Controllers
{
    [Route("api")]
    [ApiController]
    public class ServerController : ControllerBase
    {
        FileServerDBContext _contetx;

        public ServerController(FileServerDBContext contetx)
        {
            _contetx = contetx;
        }

        [HttpGet("server")]
        public async Task<IActionResult> GetServer(string system, string subsystem, string ext, string size, string record)
        {
            try
            {
                //Check file type and get FileTypeId from it
                var fileType = await _contetx.TblFileTypes.Where(a => a.Title == ext).FirstOrDefaultAsync();
                var fileTypeId = 0;
                if (fileType == null)
                {
                    return new JsonResult(new { data = new { Code = "1", Msg = "Empty/Invalid File Type!" } });
                }
                else
                {
                    fileTypeId = fileType.Id;
                }

                //Check system and get SystemId from it
                var systemm = await _contetx.TblSystems.Where(a => a.Id == int.Parse(system)).FirstOrDefaultAsync();
                var systemmId = 0;
                if (systemm == null)
                {
                    return new JsonResult(new { data = new { Code = "2", Msg = "Empty/Invalid System!" } });
                }
                else
                {
                    systemmId = systemm.Id;
                }

                //Check subSystem and get subSystemId from it
                var subSystemm = await _contetx.TblSubSystems.Where(a => a.Id == int.Parse(subsystem)).FirstOrDefaultAsync();
                var subSystemmId = 0;
                if (subSystemm == null)
                {
                    return new JsonResult(new { data = new { Code = "3", Msg = "Empty/Invalid SubSystem!" } });
                }
                else
                {
                    subSystemmId = subSystemm.Id;
                }

                //Check subSystem and get subSystemId from it
                if (string.IsNullOrEmpty(size))
                {
                    return new JsonResult(new { data = new { Code = "4", Msg = "Zero byte file!" } });
                }
                else
                {
                    subSystemmId = subSystemm.Id;
                }

                //Find servers based on file attributes
                var access = await _contetx.TblAccesses
                    .Where(a => (a.FileTypeId == fileTypeId && a.SubSystemId == subSystemmId)
                              || (a.FileTypeId == fileTypeId && a.SubSystemId == null)
                              || (a.FileTypeId == null && a.SubSystemId == subSystemmId))
                    .Select(b => b.ServerId)
                    .ToArrayAsync();
                var server = await _contetx.TblServers
                    .Where(a => access.Contains(a.Id) && a.Active)
                    .OrderBy(a => a.Priority)
                    .ToListAsync();

                //Check if no server available based on file extention and subsystem
                if (server.Count < 1)
                {
                    return new JsonResult(new { data = new { Code = "5", Msg = "No server Active!" } });
                }

                //Check if no server available based on file size
                TblServer selectedServer = null;
                for (int i = 0; i < server.Count(); i++)
                {
                    if ((server[i].Capacity - server[i].Used) < (double.Parse(size) / 1024 / 1024))
                    {
                        continue;
                    }
                    else
                    {
                        selectedServer = server[i];
                        break;
                    }
                }
                if (selectedServer == null)
                {
                    return new JsonResult(new { data = new { Code = "6", Msg = "No space available!" } });
                }

                //Create a record on database
                var file = new TblFile
                {
                    SystemId = systemmId,
                    SubSystemId = subSystemmId,
                    FileTypeId = fileTypeId,
                    Size = (double.Parse(size) / 1024 / 1024),
                    Record = Int64.Parse(record),
                    CreatedDate = DateTime.Now,
                    ServerId = selectedServer.Id
                };
                await _contetx.TblFiles.AddAsync(file);
                await _contetx.SaveChangesAsync();

                //Return Server.Address and File.Id
                return new JsonResult(new { data = new { Code = "7", Msg = "OK!", server = selectedServer.Address, id = file.Id } });
            }
            catch (Exception e)
            {
                return new JsonResult(new { data = new { Code = "8", Msg = e.Message } });
            }
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

                    //INCREASE SERVER.USED
                    var server = await _contetx.TblServers.FindAsync(file.ServerId);
                    server.Used += file.Size;
                }
                else
                {
                    _contetx.TblFiles.Remove(file);
                }

                await _contetx.SaveChangesAsync();

                return new JsonResult(new { data = new { Code = "7", Msg = "OK!" } });
            }
            catch (Exception e)
            {
                return new JsonResult(new { data = new { Code = "8", Msg = e.Message } });
            }
        }

        [HttpGet("download")]
        public async Task<IActionResult> GetDownload(int record, int system, int subsystem)
        {
            List<LinkList> linklist = new List<LinkList>();
            try
            {
                //Check file existence
                var file = await _contetx.TblFiles.Include(t => t.Server).Include(t => t.FileType).Where(a => a.Record == record && a.SystemId == system && a.SubSystemId == subsystem).ToListAsync();
                if (!file.Any())
                {
                    return new JsonResult(new { data = new { Code = "10", Msg = "File Not Exist!" } });
                }

                foreach (var item in file)
                {
                    if (item.Active && !item.IsDeleted)
                    {
                        linklist.Add(new LinkList { link = $"http://{item.Server.Address}/api/file/download/{item.Name}.{item.FileType.Title}",record=item.Record.Value});
                    }
                }

                return new JsonResult(new { data = linklist });

            }
            catch (Exception e)
            {
                return new JsonResult(new { data = new { Code = "8", Msg = e.Message } });
            }

        }

        public class LinkList
        {
            public string link { get; set; }
            public long record { get; set; }
        }
    }
}
