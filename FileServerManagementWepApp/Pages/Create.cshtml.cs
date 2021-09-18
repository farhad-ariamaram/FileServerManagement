using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FileServerManagementWepApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;

namespace FileServerManagementWepApp.Pages
{
    public class CreateModel : PageModel
    {
        private readonly FileServerDBContext _context;

        public CreateModel(FileServerDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            //NO SERVER ACTIVATE
            if (!(await _context.TblServers.Where(a => a.Active == true).AnyAsync()))
            {
                return Redirect("./NoServerActive");
            }

            return Page();
        }
    }
}
