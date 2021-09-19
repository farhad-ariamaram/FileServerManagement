using System;
using System.Collections.Generic;

#nullable disable

namespace FileServerManagementWepApp.Models
{
    public partial class TblFileType
    {
        public TblFileType()
        {
            TblAccesses = new HashSet<TblAccess>();
            TblFiles = new HashSet<TblFile>();
        }

        public int Id { get; set; }
        public string Title { get; set; }

        public virtual ICollection<TblAccess> TblAccesses { get; set; }
        public virtual ICollection<TblFile> TblFiles { get; set; }
    }
}
