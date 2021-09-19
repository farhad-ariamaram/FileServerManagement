using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblFile
    {
        public long Id { get; set; }
        public int? SystemId { get; set; }
        public int? SubSystemId { get; set; }
        public int? FileTypeId { get; set; }
        public double Size { get; set; }
        public long? Record { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ServerId { get; set; }
        public bool IsComplete { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }

        public virtual TblFileType FileType { get; set; }
        public virtual TblServer Server { get; set; }
        public virtual TblSubSystem SubSystem { get; set; }
        public virtual TblSystem System { get; set; }
    }
}
