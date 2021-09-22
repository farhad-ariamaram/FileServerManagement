using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;

namespace FileServerManagementWepApp.Pages.FiletypePage
{
    public class DeleteModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public DeleteModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblFileType TblFileType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblFileType = await _context.TblFileTypes.FirstOrDefaultAsync(m => m.Id == id);

            if (TblFileType == null)
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

            TblFileType = await _context.TblFileTypes.FindAsync(id);

            if (TblFileType != null)
            {
                try
                {
                    _context.TblFileTypes.Remove(TblFileType);
                    await _context.SaveChangesAsync();
                }
                catch (Exception)
                {
                    return RedirectToPage("../Error" , new { msg = "به علت وجود کلید خارجی حذف نشد" });
                }
                
            }

            return RedirectToPage("./Index");
        }
    }
}
