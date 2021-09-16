using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblLog
    {
        public long Id { get; set; }
        public DateTime? Datetime { get; set; }
        public string Action { get; set; }
        public int? UserId { get; set; }
        public long? FileId { get; set; }
        public int? ServerId { get; set; }

        public virtual TblUser User { get; set; }
    }
}
