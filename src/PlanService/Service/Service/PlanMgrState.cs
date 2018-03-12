using System;
using System.Collections.Generic;
using System.Text;
using Inspur.ECP.Rtf.Api;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanMgrState
    {
        public static bool IsTest
        {
            get;set;
        }

        public static string UserId => IsTest ? "9999" : EcpState.Current.User.ID;

        public static string TenantId => IsTest ? "10000" : EcpState.Current.TenantID;

        public static string UserName => IsTest ? "9999" : EcpState.Current.User.Name;

        public static string Telphone => IsTest ? "133333333" : EcpState.Current.User.Phone;
    }
}
