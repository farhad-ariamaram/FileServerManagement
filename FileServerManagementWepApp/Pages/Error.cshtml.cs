using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileServerManagementWepApp.Pages
{
    public class ErrorModel : PageModel
    {
        public string msgs { get; set; }
        public void OnGet(string msg)
        {
            msgs = msg;
        }
    }
}
