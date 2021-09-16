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

namespace FileServerManagementWepApp.Pages.ServerPage
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
            return Page();
        }

        [BindProperty]
        public TblServer TblServer { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var PriorityCheck = _context.TblServers.Where(a => a.Priority == TblServer.Priority);
            if (await PriorityCheck.AnyAsync())
            {
                ModelState.AddModelError("PriorityError", "اولویت انتخاب شده از قبل موجود است");
                return Page();
            }

            switch (HttpContext.Request.Form["sizeType"])
            {
                case "KB":
                    TblServer.Capacity /= 1048576;
                    break;
                case "MB":
                    TblServer.Capacity /= 1024;
                    break;
                case "GB":
                    TblServer.Capacity *= 1;
                    break;
                case "TB":
                    TblServer.Capacity *= 1024;
                    break;
                default:
                    break;
            }

            _context.TblServers.Add(TblServer);
            await _context.SaveChangesAsync();

            //LOG CREATE SERVER
            var logLogin = new TblLog
            {
                UserId = int.Parse(HttpContext.Session.GetString("uid")),
                Datetime = DateTime.Now,
                Action = "Create-Server",
                ServerId = TblServer.Id
            };
            await _context.TblLogs.AddAsync(logLogin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
