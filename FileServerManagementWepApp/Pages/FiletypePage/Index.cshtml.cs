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
    public class IndexModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public IndexModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        public IList<TblFileType> TblFileType { get;set; }

        public async Task OnGetAsync()
        {
            TblFileType = await _context.TblFileTypes.ToListAsync();
        }
    }
}
