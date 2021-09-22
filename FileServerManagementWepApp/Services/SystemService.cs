using FileServerManagementWepApp.Models;
using FileServerManagementWepApp.Utilities;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Services
{
    public static class SystemService
    {
        public static async Task fillSystem(int uid)
        {
            FileServerDBContext _context = new FileServerDBContext();

            var t = await _context.TblUsers.FindAsync(uid);

            string key = Consts._CONST_KEY2;
            string tk = ApiLogin.rndTransferKey(8) + ApiLogin.Reverse(t.Token);
            string p1 = ApiLogin.EncryptString(uid+"", key);
            string p2 = ApiLogin.EncryptString(tk, key);
            string p3 = "";

            var theWebRequest = HttpWebRequest.Create("http://192.168.10.250/ExLogin.aspx/SystemSearch");
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=utf-8";
            theWebRequest.Headers.Add(HttpRequestHeader.Pragma, "no-cache");

            using (var writer = theWebRequest.GetRequestStream())
            {
                string send = null;
                send = "{\"p0\":\"4\",\"p1\":\"" + p1 + "\",\"p2\":\"" + p2 + "\",\"p3\":\"" + p3 + "\"}";

                var data = Encoding.UTF8.GetBytes(send);

                writer.Write(data, 0, data.Length);
            }

            var theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            var theResponseStream = new StreamReader(theWebResponse.GetResponseStream());

            string result = theResponseStream.ReadToEnd();

            var splashInfo = JsonConvert.DeserializeObject<RSystem>(result);

            foreach (var item in splashInfo.d)
            {
                var v = await _context.TblSystems.FindAsync(item.id);
                if (v == null)
                {
                    await _context.TblSystems.AddAsync(new TblSystem
                    {
                        Id = item.id,
                        Title = item.txt
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }

        public static async Task fillSubSystem(int uid)
        {
            FileServerDBContext _context = new FileServerDBContext();

            var t = await _context.TblUsers.FindAsync(uid);

            string key = Consts._CONST_KEY2;
            string tk = ApiLogin.rndTransferKey(8) + ApiLogin.Reverse(t.Token);
            string p1 = ApiLogin.EncryptString(uid+"", key);
            string p2 = ApiLogin.EncryptString(tk, key);
            string p3 = "";

            var theWebRequest = HttpWebRequest.Create("http://192.168.10.250/ExLogin.aspx/SubSystemSearch");
            theWebRequest.Method = "POST";
            theWebRequest.ContentType = "application/json; charset=utf-8";
            theWebRequest.Headers.Add(HttpRequestHeader.Pragma, "no-cache");

            using (var writer = theWebRequest.GetRequestStream())
            {
                string send = null;
                send = "{\"p0\":\"4\",\"p1\":\"" + p1 + "\",\"p2\":\"" + p2 + "\",\"p3\":\"" + p3 + "\"}";

                var data = Encoding.UTF8.GetBytes(send);

                writer.Write(data, 0, data.Length);
            }

            var theWebResponse = (HttpWebResponse)theWebRequest.GetResponse();
            var theResponseStream = new StreamReader(theWebResponse.GetResponseStream());

            string result = theResponseStream.ReadToEnd();

            var splashInfo = JsonConvert.DeserializeObject<RSubSystem>(result);

            foreach (var item in splashInfo.d)
            {
                var v = await _context.TblSubSystems.FindAsync(item.id);
                if (v == null)
                {
                    await _context.TblSubSystems.AddAsync(new TblSubSystem
                    {
                        Id = item.id,
                        Title = item.txt,
                        SystemId = item.sid
                    });
                    await _context.SaveChangesAsync();
                }
            }
        }

        public class DSystem
        {
            public string __type { get; set; }
            public int id { get; set; }
            public string txt { get; set; }
        }

        public class RSystem
        {
            public List<DSystem> d { get; set; }
        }

        public class DSubSystem
        {
            public string __type { get; set; }
            public int id { get; set; }
            public int sid { get; set; }
            public string txt { get; set; }
        }

        public class RSubSystem
        {
            public List<DSubSystem> d { get; set; }
        }
    }
}
