using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileServerManagementWepApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;

namespace FileServerManagementWepApp.Pages
{
    public class CreateModel : PageModel
    {
        private readonly FileServerDBContext _context;

        public CreateModel(FileServerDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            //NO SERVER ACTIVATE
            if (!(await _context.TblServers.Where(a => a.Active == true).AnyAsync()))
            {
                return Redirect("./NoServerActive");
            }

            ViewData["ServerId"] = new SelectList(_context.TblServers.Where(a => a.Active), "Id", "Name");
            return Page();
        }

        [BindProperty]
        public TblFile TblFile { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                ViewData["ServerId"] = new SelectList(_context.TblServers.Where(a => a.Active), "Id", "Name");
                return Page();
            }

            TblFile.CreatedDate = DateTime.Now;

            //check server capacity
            var server = await _context.TblServers.FindAsync(TblFile.ServerId);
            var serverCapacity = server.Capacity * 1024;
            var serverUsed = server.Used * 1024;

            if (serverCapacity < (serverUsed + (TblFile.Size)))
            {
                ViewData["ServerId"] = new SelectList(_context.TblServers.Where(a => a.Active), "Id", "Name");
                ModelState.AddModelError("serverFull", "ظرفیت سرور کمتر از حجم فایل انتخابی است");
                return Page();
            }

            if (_context.TblFiles.Where(a => a.ServerId == TblFile.ServerId  && a.Name == TblFile.Name).Any())
            {
                ViewData["ServerId"] = new SelectList(_context.TblServers.Where(a => a.Active), "Id", "Name");
                ModelState.AddModelError("duplicate", "فایل تکراری است");
                return Page();
            }

            //DATABASE
            _context.TblFiles.Add(TblFile);
            await _context.SaveChangesAsync();

            //LOG CREATE(UPLOAD) FILE
            var logLogin = new TblLog
            {
                UserId = int.Parse(HttpContext.Session.GetString("uid")),
                Datetime = DateTime.Now,
                Action = "Create-File",
                FileId = TblFile.Id
            };
            await _context.TblLogs.AddAsync(logLogin);
            await _context.SaveChangesAsync();

            //SERVER USED
            _context.TblServers.Find(TblFile.ServerId).Used += (TblFile.Size / 1024);
            await _context.SaveChangesAsync();

            //CHANGE SERVER ACTIVITY IF FULL
            var TargetServer = await _context.TblServers.FindAsync(TblFile.ServerId);
            if (TargetServer.Capacity <= TargetServer.Used + 5)
            {
                TargetServer.Active = false;
                await _context.SaveChangesAsync();

                //ENABLE SERVER WITH HIGH PIORITY
                var PreviousServer = _context.TblServers.Where(a => a.Active == true).OrderBy(a => a.Priority);
                if (await PreviousServer.AnyAsync())
                {
                    var next = await PreviousServer.FirstOrDefaultAsync();
                    next.Active = true;
                    await _context.SaveChangesAsync();
                }

                //IF NO SERVER ACTIVATE
                if (!(await _context.TblServers.Where(a => a.Active == true).AnyAsync()))
                {
                    return Redirect("./NoServerActive");
                }
            }

            return RedirectToPage("./Index");
        }
    }
}
