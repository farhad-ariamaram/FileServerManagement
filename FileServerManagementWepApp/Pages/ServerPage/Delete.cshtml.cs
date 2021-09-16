using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;

namespace FileServerManagementWepApp.Pages.ServerPage
{
    public class DeleteModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public DeleteModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblServer TblServer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblServer = await _context.TblServers.FirstOrDefaultAsync(m => m.Id == id);

            if (TblServer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblServer = await _context.TblServers.FindAsync(id);

            if (TblServer != null)
            {
                if (TblServer.Active == true)
                {
                    TblServer.Active = false;
                }
                else
                {
                    TblServer.Active = true;
                }
                
                await _context.SaveChangesAsync();
            }

            //LOG DELETE SERVER
            var logLogin = new TblLog
            {
                UserId = int.Parse(HttpContext.Session.GetString("uid")),
                Datetime = DateTime.Now,
                Action = "Delete-Server",
                ServerId = TblServer.Id
            };
            await _context.TblLogs.AddAsync(logLogin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
