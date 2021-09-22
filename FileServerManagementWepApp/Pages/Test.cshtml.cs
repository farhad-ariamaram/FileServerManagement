using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileServerManagementWepApp.Models;
using FileServerManagementWepApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileServerManagementWepApp.Pages
{
    public class TestModel : PageModel
    {


        public List<TblFile> Moo { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public IActionResult OnGetRenew()
        {
            return Page();
        }
    }
}
