using System.Text;
using System.Threading.Tasks;
using FileServerManagementWepApp.Models;
using FileServerManagementWepApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileServerManagementWepApp.Pages
{
    public class UpdateSystemModel : PageModel
    {
        private readonly FileServerDBContext _context;

        public UpdateSystemModel(FileServerDBContext context)
        {
            _context = context;
        }

        public async Task OnGet()
        {
            var s = HttpContext.Session.GetString("uid");
            var uid = int.Parse(s);
            await SystemService.fillSystem(uid);
            await SystemService.fillSubSystem(uid);
        }

    }
}
