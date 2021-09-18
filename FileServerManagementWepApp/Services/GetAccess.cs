using FileServerManagementWepApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Services
{
    public class GetAccess : IGetAccessSingleton
    {
        static FileServerDBContext _Context = new FileServerDBContext();
        List<TblFile> _files;

        public GetAccess() : this(_Context.TblFiles.ToList())
        {

        }

        public GetAccess(List<TblFile> files)
        {
            _files = files;
        }

        List<TblFile> IGetAccess.GetAccess => _files;

        void IGetAccess.Renew()
        {
            _files = _Context.TblFiles.ToList();
        }
    }
}
