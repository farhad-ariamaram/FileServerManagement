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
    public class IndexModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public IndexModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        public IList<TblAccess> TblAccess { get;set; }

        public async Task OnGetAsync()
        {
            TblAccess = await _context.TblAccesses
                .Include(t => t.FileType)
                .Include(t => t.Server)
                .Include(t => t.SubSystem).ToListAsync();
        }
    }
}
