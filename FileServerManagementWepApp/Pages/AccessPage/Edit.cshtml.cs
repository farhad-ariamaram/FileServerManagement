using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;

namespace FileServerManagementWepApp.Pages.AccessPage
{
    public class EditModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public EditModel(FileServerManagementWepApp.Models.FileServerDBContext context)
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
           ViewData["FileTypeId"] = new SelectList(_context.TblFileTypes, "Id", "Title");
           ViewData["ServerId"] = new SelectList(_context.TblServers, "Id", "Id");
           ViewData["SubSystemId"] = new SelectList(_context.TblSubSystems, "Id", "Title");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(TblAccess).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblAccessExists(TblAccess.Id))
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

        private bool TblAccessExists(int id)
        {
            return _context.TblAccesses.Any(e => e.Id == id);
        }
    }
}
