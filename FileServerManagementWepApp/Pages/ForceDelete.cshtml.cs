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
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TblFile == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblFile = await _context.TblFiles.FindAsync(id);

            if (TblFile != null)
            {
                //REMOVE FROM SERVER
                var sever = TblFile.Server.Address;
                var name = TblFile.Name + "." + TblFile.Extention;
                HttpClient _client = new HttpClient();
                var del = await _client.GetAsync("hhtp://" + sever + "/api/file/delete/" + name);
                if (!del.IsSuccessStatusCode)
                {
                    ModelState.AddModelError("DeleteError","مشکل در حذف از سرور");
                    return Page();
                }

                var size = TblFile.Size;
                var server = TblFile.ServerId;

                _context.TblFiles.Remove(TblFile);
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

                _context.TblServers.Find(server).Used -= (size / 1024);
                await _context.SaveChangesAsync();

                var TargetServer = await _context.TblServers.FindAsync(server);
                if (TargetServer.Capacity > TargetServer.Used + 5)
                {
                    TargetServer.Active = true;
                    await _context.SaveChangesAsync();
                }

            }

            return RedirectToPage("./Index" , new { status = 2 });
        }
    }
}
