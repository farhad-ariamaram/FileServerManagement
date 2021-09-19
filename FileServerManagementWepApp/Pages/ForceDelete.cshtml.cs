using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FileServerManagementWepApp.Pages
{
    public class ForceDeleteModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public ForceDeleteModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblFile TblFile { get; set; }

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblFile = await _context.TblFiles
                .Include(t => t.Server)
                .Include(t => t.System)
                .Include(t => t.SubSystem)
                .Include(t => t.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TblFile == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            try
            {
                if (id == null)
                {
                    return NotFound();
                }

                TblFile = await _context.TblFiles
                .Include(t => t.Server)
                .Include(t => t.System)
                .Include(t => t.SubSystem)
                .Include(t => t.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);

                if (TblFile != null)
                {
                    //REMOVE FROM SERVER
                    var sever = await _context.TblServers.FindAsync(TblFile.ServerId);
                    var name = TblFile.Name + "." + TblFile.FileType.Title;
                    HttpClient _client = new HttpClient();
                    var del = await _client.GetAsync("http://" + sever.Address + "/api/file/delete/" + name);
                    if (!del.IsSuccessStatusCode)
                    {
                        var data = await del.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject(data);
                        ModelState.AddModelError("DeleteError", "مشکل در حذف از سرور" + data);
                        return Page();
                    }

                    var size = TblFile.Size;
                    var server = TblFile.ServerId;

                    _context.TblFiles.Remove(TblFile);
                    await _context.SaveChangesAsync();

                    //Reduce server used
                    var s = await _context.TblServers.FindAsync(server);
                    s.Used -= size;
                    await _context.SaveChangesAsync();

                    //LOG FORCE DELETE FILE
                    var logLogin = new TblLog
                    {
                        UserId = int.Parse(HttpContext.Session.GetString("uid")),
                        Datetime = DateTime.Now,
                        Action = "ForceDelete-File",
                        FileId = TblFile.Id
                    };
                    await _context.TblLogs.AddAsync(logLogin);
                    await _context.SaveChangesAsync();

                    var TargetServer = await _context.TblServers.FindAsync(server);
                    if (TargetServer.Capacity > TargetServer.Used + 5)
                    {
                        TargetServer.Active = true;
                        await _context.SaveChangesAsync();
                    }

                }

                return RedirectToPage("./Index", new { status = 2 });
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteError", "مشکل در حذف از سرور");
                return Page();
            }

        }
    }
}
