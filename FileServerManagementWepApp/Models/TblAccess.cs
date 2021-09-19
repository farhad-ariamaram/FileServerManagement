using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblAccess
    {
        public int Id { get; set; }
        public int? SubSystemId { get; set; }
        public int ServerId { get; set; }
        public int? FileTypeId { get; set; }

        public virtual TblFileType FileType { get; set; }
        public virtual TblServer Server { get; set; }
        public virtual TblSubSystem SubSystem { get; set; }
    }
}
