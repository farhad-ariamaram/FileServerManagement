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
                .Include(t => t.Server).FirstOrDefaultAsync(m => m.Id == id);

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
                await _context.SaveChangesAsync();

                //LOG EDIT FILE
                var logLogin = new TblLog
                {
                    UserId = int.Parse(HttpContext.Session.GetString("uid")),
                    Datetime = DateTime.Now,
                    Action = "Edit-File",
                    FileId = TblFile.Id
                };
                await _context.TblLogs.AddAsync(logLogin);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
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
    }
}
