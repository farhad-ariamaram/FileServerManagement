using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;

namespace FileServerManagementWepApp.Pages.ServerPage
{
    public class IndexModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public IndexModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        public IList<TblServer> TblServer { get; set; }
        public int Status { get; set; }

        public async Task OnGetAsync(int status = 0)
        {
            TblServer = await _context.TblServers.ToListAsync();

            Status = status;

            if (status == 0)
            {
                TblServer = await _context.TblServers.Where(a => a.Active == true).ToListAsync();
            }
            else if (status == 1)
            {
                TblServer = await _context.TblServers.Where(a => a.Active == false).ToListAsync();
            }
        }

        public JsonResult OnGetRest(int id)
        {
            var serverRest = _context.TblFiles.Where(a => a.ServerId == id).Sum(a => a.Size);
            var severCapacity = _context.TblServers.Find(id);
            return new JsonResult ( serverRest + "MB از " + severCapacity.Capacity + "GB" );
        }
    }
}
