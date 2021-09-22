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

            string key = Consts._CONST_KEY2;
            string tk = ApiLogin.rndTransferKey();
            string p0 = "4";

            string p1 = ApiLogin.EncryptString(userModel.Username, key);
            string p2 = ApiLogin.EncryptString(userModel.Password, key);
            string p3 = ApiLogin.EncryptString(tk, key);

            var theWebRequest = HttpWebRequest.Create("http://192.168.10.250/ExLogin.aspx/LI");
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=utf-8";
            theWebRequest.Headers.Add(HttpRequestHeader.Pragma, "no-cache");

            using (var writer = theWebRequest.GetRequestStream())
            {
                string send = null;
                send = "{\"p0\":\"" + p0 + "\",\"p1\":\"" + p1 + "\",\"p2\":\"" + p2 + "\",\"p3\":\"" + p3 + "\"}";

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
            catch (Exception e)
            {
                ModelState.AddModelError("WrongUP", "نام کاربری یا کلمه عبور اشتباه است");
                return Page();
            }

            ApiUser EncryptUserModel = JsonConvert.DeserializeObject<ApiUser>(result);

            string backTk = ApiLogin.Reverse(ApiLogin.DecryptString(EncryptUserModel.Status, key));
            ApiUser DecryptUserModel = new ApiUser();
            if (tk == backTk)
            {
                DecryptUserModel.id = ApiLogin.DecryptString(EncryptUserModel.id, key);
                DecryptUserModel.name = ApiLogin.DecryptString(EncryptUserModel.name, key);
                DecryptUserModel.Status = ApiLogin.DecryptString(EncryptUserModel.Status, key);
                DecryptUserModel.IsGuard = ApiLogin.DecryptString(EncryptUserModel.IsGuard, key);
                DecryptUserModel.IsGuardAdmin = ApiLogin.DecryptString(EncryptUserModel.IsGuardAdmin, key);
                DecryptUserModel.IsEmployeeRequest = ApiLogin.DecryptString(EncryptUserModel.IsEmployeeRequest, key);
                DecryptUserModel.IsGuardRecorder = ApiLogin.DecryptString(EncryptUserModel.IsGuardRecorder, key);
                DecryptUserModel.IsMould = ApiLogin.DecryptString(EncryptUserModel.IsMould, key);
                DecryptUserModel.token = ApiLogin.DecryptString(EncryptUserModel.token, key);

                var currentUser = _context.TblUsers.Where(a => a.Id == int.Parse(DecryptUserModel.id)).FirstOrDefault();

                if (currentUser != null)
                {
                    //check name
                    if (!currentUser.Name.Equals(DecryptUserModel.name))
                    {
                        currentUser.Name = DecryptUserModel.name;
                    }

                    //check pass
                    if (!currentUser.Password.Equals(ApiLogin.sha512(userModel.Username + Consts._CONST_SALT)))
                    {
                        currentUser.Password = ApiLogin.sha512(userModel.Password + Consts._CONST_SALT);
                    }

                    //token
                    currentUser.Token = DecryptUserModel.token;

                    _context.TblUsers.Update(currentUser);
                    _context.SaveChanges();

                    string uid = DecryptUserModel.id;
                    HttpContext.Session.SetString("uid", uid);

                    return RedirectToPage("./Index");

                }
                else
                {

                    TblUser newUser = new TblUser();

                    newUser.Id = int.Parse(DecryptUserModel.id);
                    newUser.Username = Request.Form["userModel.Username"];
                    newUser.Password = ApiLogin.sha512(Request.Form["userModel.Password"] + Consts._CONST_SALT);
                    newUser.Name = DecryptUserModel.name;
                    newUser.Token = DecryptUserModel.token;

                    _context.TblUsers.Add(newUser);
                    _context.SaveChanges();

                    string uid = newUser.Id+"";
                    HttpContext.Session.SetString("uid", uid);

                    return RedirectToPage("./Index");
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

        public class ApiUser
        {
            public string Status;
            public string id;
            public string name;
            public string IsGuard;
            public string IsGuardAdmin;
            public string IsEmployeeRequest;
            public string IsGuardRecorder;
            public string IsMould;
            public string token;
        }
    }
}
