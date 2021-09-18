using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileServerManagementWepApp.Models;
using FileServerManagementWepApp.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FileServerManagementWepApp.Pages
{
    public class LoginModel : PageModel
    {
        private readonly FileServerDBContext _context;

        public LoginModel(FileServerDBContext context)
        {
            _context = context;
        }

        [BindProperty]
        public UserModel userModel { get; set; }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetString("uid") != null)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            string user = Request.Form["userModel.Username"];
            string pass = Request.Form["userModel.Password"];

            var LocalCheck = _context.TblUsers.Where(a => a.Username == user
                                                     && a.Password == pass);

            if (await LocalCheck.AnyAsync())
            {
                string uid = LocalCheck.FirstOrDefault().Id + "";
                HttpContext.Session.SetString("uid", uid);

                return RedirectToPage("./Index");
            }

            string key = Consts._CONST_KEY;
            string tk = ApiLogin.rndTransferKey();
            string token = ApiLogin.EncryptString(tk, key);
            string p1 = ApiLogin.EncryptString(Request.Form["userModel.Username"], key);
            string p2 = ApiLogin.EncryptString(Request.Form["userModel.Password"], key);
            string p3 = token;

            var theWebRequest = HttpWebRequest.Create("http://192.168.10.250/ExLogin.aspx/LI");
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=utf-8";
            theWebRequest.Headers.Add(HttpRequestHeader.Pragma, "no-cache");

            using (var writer = theWebRequest.GetRequestStream())
            {
                string send = null;
                send = "{\"p0\":\"1\",\"p1\":\"" + p1 + "\",\"p2\":\"" + p2 + "\",\"p3\":\"" + p3 + "\"}";

                var data = Encoding.UTF8.GetBytes(send);

                writer.Write(data, 0, data.Length);
            }

            var theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            var theResponseStream = new StreamReader(theWebResponse.GetResponseStream());

            string result = theResponseStream.ReadToEnd();

            try
            {
                result = "{" + result.Substring(28).Replace("}}", "}");
            }
            catch (Exception)
            {
                ModelState.AddModelError("WrongUP", "نام کاربری یا کلمه عبور اشتباه است");
                return Page();
            }

            var splashInfo = JsonConvert.DeserializeObject<clsExLogin>(result);

            string backTk = ApiLogin.DecryptString(splashInfo.Status, key);
            if (tk == ApiLogin.Reverse(backTk))
            {
                splashInfo.id = ApiLogin.DecryptString(splashInfo.id, key);
                splashInfo.name = ApiLogin.DecryptString(splashInfo.name, key);
                splashInfo.Status = ApiLogin.DecryptString(splashInfo.Status, key);

                bool withError = false;

                if (!withError)
                {
                    var preuser = await _context.TblUsers.FindAsync(Int32.Parse(splashInfo.id));
                    if (preuser != null)
                    {
                        _context.TblUsers.Remove(preuser);
                        await _context.SaveChangesAsync();
                    }
                    
                    await _context.TblUsers.AddAsync(new TblUser
                    {
                        Id = Int32.Parse(splashInfo.id),
                        Name = splashInfo.name,
                        Password = Request.Form["userModel.Password"],
                        Token = token,
                        Username = Request.Form["userModel.Username"]
                    });
                    await _context.SaveChangesAsync();

                    string uid = splashInfo.id;
                    HttpContext.Session.SetString("uid", uid);

                    //LOG LOGIN
                    var logLogin = new TblLog {
                        UserId = int.Parse(uid),
                        Datetime = DateTime.Now,
                        Action = "login"
                    };
                    await _context.TblLogs.AddAsync(logLogin);
                    await _context.SaveChangesAsync();

                    return RedirectToPage("./Index");
                }
                else
                {
                    ModelState.AddModelError("WrongUP", "در سیستم خطایی رخ داده است ! لطفا در زمان دیگری وارد شوید!");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError("WrongUP", "نام کاربری یا کلمه عبور اشتباه است");
                return Page();
            }

        }

        public class UserModel
        {
            [DisplayName("نام کاربری")]
            [Required(ErrorMessage = "نام کاربری را وارد کنید")]
            public string Username { get; set; }

            [DisplayName("کلمه عبور")]
            [Required(ErrorMessage = "کلمه عبور را وارد کنید")]
            public string Password { get; set; }
        }

        public class clsExLogin
        {
            public string Status;
            public string id;
            public string name;
        }
    }
}
