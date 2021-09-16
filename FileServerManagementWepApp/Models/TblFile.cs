using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblFile
    {
        public long Id { get; set; }
        public string System { get; set; }
        public string SubSystem { get; set; }
        public string Extention { get; set; }
        public double Size { get; set; }
        public long? Record { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ServerId { get; set; }
        public bool IsComplete { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool IsDeleted { get; set; }

        public virtual TblServer Server { get; set; }
    }
}
