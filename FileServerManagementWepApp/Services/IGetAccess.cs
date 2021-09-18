using FileServerManagementWepApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Services
{
    public interface IGetAccess
    {
        List<TblFile> GetAccess { get; }
        void Renew();
    }

    public interface IGetAccessSingleton : IGetAccess
    {
    }
}
