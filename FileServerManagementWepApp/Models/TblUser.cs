using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblUser
    {
        public TblUser()
        {
            TblLogs = new HashSet<TblLog>();
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Token { get; set; }

        public virtual ICollection<TblLog> TblLogs { get; set; }
    }
}
