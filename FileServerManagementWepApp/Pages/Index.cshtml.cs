using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using FileServerManagementWepApp.Services;

namespace FileServerManagementWepApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public IndexModel(FileServerDBContext context)
        {
            _context = context;
        }

        public IList<TblFile> TblFile { get; set; }

        public int Status { get; set; }

        public async Task OnGetAsync(int status = 0)
        {

            TblFile = await _context.TblFiles
                    .Include(t => t.Server)
                    .ToListAsync();

            Status = status;

            if (status == 0)
            {
                TblFile = await _context.TblFiles
                .Include(t => t.Server).Where(a => a.Active == true && a.IsDeleted == false).ToListAsync();
            }
            else if (status == 1)
            {
                TblFile = await _context.TblFiles
                .Include(t => t.Server).Where(a => a.Active == false).ToListAsync();
            }
            else if (status == 2)
            {
                TblFile = await _context.TblFiles
                .Include(t => t.Server).Where(a => a.IsDeleted == true).ToListAsync();
            }
        }
    }
}
