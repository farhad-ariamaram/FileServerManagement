using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace FileServerManagementWepApp.Pages
{
    public class EditModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public EditModel(FileServerManagementWepApp.Models.FileServerDBContext context)
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
                .Include(t=>t.FileType)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (TblFile == null)
            {
                return NotFound();
            }
            ViewData["ServerId"] = new SelectList(_context.TblServers.Where(a => a.Active), "Id", "Name");
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TblFile).State = EntityState.Modified;

            try
            {
                //SERVER USED
                var server = await _context.TblServers.FindAsync(TblFile.ServerId);
                var serverCapacity = server.Capacity;
                var serverUsed = server.Used;
                _context.TblServers.Find(TblFile.ServerId).Used += (TblFile.Size);
                await _context.SaveChangesAsync();

                //CHANGE SERVER ACTIVITY IF FULL
                if (server.Capacity <= server.Used + 5)
                {
                    server.Active = false;
                    await _context.SaveChangesAsync();
                }

                //LOG EDIT FILE
                var logLogin = new TblLog
                {
                    UserId = int.Parse(HttpContext.Session.GetString("uid")),
                    Datetime = DateTime.Now,
                    Action = "CreateEdit-File",
                    FileId = TblFile.Id
                };
                await _context.TblLogs.AddAsync(logLogin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //DELETE IF ERROR ECCOURD
                var sever = await _context.TblServers.FindAsync(TblFile.ServerId);
                var name = TblFile.Name + "." + TblFile.FileType.Title;
                HttpClient _client = new HttpClient();
                var del = await _client.GetAsync("http://" + sever.Address + "/api/file/delete/" + name);
                if (!TblFileExists(TblFile.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TblFileExists(long id)
        {
            return _context.TblFiles.Any(e => e.Id == id);
        }

        public async Task<IActionResult> OnGetRemove(int serverId, string name, string ext, long id)
        {
            //DELETE IF ERROR ECCOURD
            var sever = await _context.TblServers.FindAsync(serverId);
            var fname = name + "." + ext;
            HttpClient _client = new HttpClient();
            var del = await _client.GetAsync("http://" + sever.Address + "/api/file/delete/" + fname);

            var file = await _context.TblFiles.FindAsync(id);
            _context.TblFiles.Remove(file);
            await _context.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
