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

        public IQueryable<TblFile> TblFileIQ { get; set; }
        public IList<TblFile> TblFile { get; set; }

        public int Status { get; set; }

        public async Task OnGetAsync(int status = 0)
        {

            TblFileIQ = _context.TblFiles
                    .Include(t => t.Server)
                    .Include(t => t.System)
                    .Include(t => t.SubSystem)
                    .Include(t => t.FileType);

            Status = status;

            switch (status)
            {
                case 0:
                    TblFile = await TblFileIQ.Where(a => a.Active == true && a.IsDeleted == false).ToListAsync();
                    break;
                case 1:
                    TblFile = await TblFileIQ.Where(a => a.Active == false).ToListAsync();
                    break;
                case 2:
                    TblFile = await TblFileIQ.Where(a => a.IsDeleted == true).ToListAsync();
                    break;
                default:
                    TblFile = await TblFileIQ.Where(a => a.Active == true && a.IsDeleted == false).ToListAsync();
                    break;
            }
        }
    }
}
