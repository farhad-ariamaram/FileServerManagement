using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileServerManagementWepApp.Models;

namespace FileServerManagementWepApp.Pages.AccessPage
{
    public class CreateModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public CreateModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["FileTypeId"] = new SelectList(_context.TblFileTypes, "Id", "Title");
        ViewData["ServerId"] = new SelectList(_context.TblServers, "Id", "Name");
        ViewData["SubSystemId"] = new SelectList(_context.TblSubSystems, "Id", "Title");
            return Page();
        }

        [BindProperty]
        public TblAccess TblAccess { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.TblAccesses.Add(TblAccess);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
