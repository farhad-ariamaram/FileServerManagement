using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileServerManagementWepApp.Services
{
    public class GetAccessService
    {
        public IGetAccessSingleton SingletonGetAccess { get; }

        public GetAccessService(IGetAccessSingleton singletonGetAccess)
        {
            SingletonGetAccess = singletonGetAccess;
        }
    }
}
