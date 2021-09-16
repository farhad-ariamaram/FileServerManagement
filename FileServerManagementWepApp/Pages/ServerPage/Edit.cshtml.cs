using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FileServerManagementWepApp.Models;
using Microsoft.AspNetCore.Http;

namespace FileServerManagementWepApp.Pages.ServerPage
{
    public class EditModel : PageModel
    {
        private readonly FileServerManagementWepApp.Models.FileServerDBContext _context;

        public EditModel(FileServerManagementWepApp.Models.FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public TblServer TblServer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TblServer = await _context.TblServers.FirstOrDefaultAsync(m => m.Id == id);

            if (TblServer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var PriorityCheck = _context.TblServers.Where(a => a.Priority == TblServer.Priority && a.Id!= TblServer.Id);
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

            _context.Attach(TblServer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TblServerExists(TblServer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //LOG EIT SERVER
            var logLogin = new TblLog
            {
                UserId = int.Parse(HttpContext.Session.GetString("uid")),
                Datetime = DateTime.Now,
                Action = "Edit-Server",
                ServerId = TblServer.Id
            };
            await _context.TblLogs.AddAsync(logLogin);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private bool TblServerExists(int id)
        {
            return _context.TblServers.Any(e => e.Id == id);
        }
    }
}
