using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;

namespace FileServerManagementWepApp.Pages.AccessPage
{
    public class DeleteModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public DeleteModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblAccess TblAccess { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblAccess = await _context.TblAccesses
                .Include(t => t.FileType)
                .Include(t => t.Server)
                .Include(t => t.SubSystem).FirstOrDefaultAsync(m => m.Id == id);

            if (TblAccess == null)
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

            TblAccess = await _context.TblAccesses.FindAsync(id);

            if (TblAccess != null)
            {
                _context.TblAccesses.Remove(TblAccess);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
