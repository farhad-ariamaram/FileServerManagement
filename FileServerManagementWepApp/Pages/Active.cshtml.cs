using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;

namespace FileServerManagementWepApp.Pages
{
    public class ActiveModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public ActiveModel(FileServerManagementWepApp.Models.FileServerDBContext context)
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
                .Include(t => t.Server).FirstOrDefaultAsync(m => m.Id == id);

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
                if(TblFile.Active == false)
                {
                    TblFile.Active = true;
                }
                else
                {
                    TblFile.Active = false;
                }
                
                await _context.SaveChangesAsync();
            }

            //LOG ACTIVE/DEACTIVE FILE
            var logLogin = new TblLog
            {
                UserId = int.Parse(HttpContext.Session.GetString("uid")),
                Datetime = DateTime.Now,
                Action = TblFile.Active ? "Active-File" : "Deactive-File",
                FileId = TblFile.Id
            };
            await _context.TblLogs.AddAsync(logLogin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
