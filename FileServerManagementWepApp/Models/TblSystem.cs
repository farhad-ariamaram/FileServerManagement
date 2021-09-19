using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblSystem
    {
        public TblSystem()
        {
            TblFiles = new HashSet<TblFile>();
            TblSubSystems = new HashSet<TblSubSystem>();
        }

        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<TblFile> TblFiles { get; set; }
        public virtual ICollection<TblSubSystem> TblSubSystems { get; set; }
    }
}
