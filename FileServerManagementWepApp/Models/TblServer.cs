using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblServer
    {
        public TblServer()
        {
            TblFiles = new HashSet<TblFile>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Capacity { get; set; }
        public bool Active { get; set; }
        public int Priority { get; set; }
        public string ServerUsername { get; set; }
        public string ServerPassword { get; set; }
        public double Used { get; set; }
        public string Ext { get; set; }

        public virtual ICollection<TblFile> TblFiles { get; set; }
    }
}
