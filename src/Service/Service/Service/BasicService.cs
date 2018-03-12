using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class BasicService
    {
        private readonly BasicManager manager = new BasicManager();        
        private static BasicService instance = null;
        public static BasicService Current => instance ?? (instance = new BasicService());

        public List<User> GetUsersByUserName(string userName)
        {
            return manager.GetUsersByUserName(userName);
        }
    }
}
