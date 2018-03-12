using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PeriodService
    {

        private readonly PeriodManager manager = new PeriodManager();
        private static PeriodService instance = null;

        private PeriodService() { }

        public static PeriodService Current => instance ?? (instance = new PeriodService());

        public List<Period> GetPeriodByFilter(PeriodFilter periodFilter)
        {
            return manager.GetPeriodByFilter(periodFilter);
        }

        public List<PeriodSet> GetAllPeriodSets()
        {
            return manager.GetAllPeriodSets();
        }

        public List<PeriodType> GetPeriodTypes()
        {
            return manager.GetPeriodTypes();
        }

        public List<PeriodSet> GetMyPeriodSets()
        {
            return manager.GetMyPeriodSets();
        }

        public void UpdateMyPeriodSets(List<string> periodSetIDList)
        {
            manager.UpdateMyPeriodSets(periodSetIDList);
        }
    }
}
