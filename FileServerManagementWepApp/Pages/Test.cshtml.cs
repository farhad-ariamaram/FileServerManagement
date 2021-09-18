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
        private readonly IGetAccessSingleton _singletonGetAccess;

        public List<TblFile> Moo { get; set; }

        public TestModel(IGetAccessSingleton singletonGetAccess)
        {
            _singletonGetAccess = singletonGetAccess;
        }

        public IActionResult OnGet()
        {
            // ViewBag contains controller-requested services
            Moo = _singletonGetAccess.GetAccess;
            return Page();
        }

        public IActionResult OnGetRenew()
        {
            // ViewBag contains controller-requested services
            _singletonGetAccess.Renew();
            return Page();
        }
    }
}
