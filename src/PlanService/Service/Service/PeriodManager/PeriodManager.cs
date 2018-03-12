using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PeriodManager
    {

        PeriodDac dac = new PeriodDac();
        internal List<Period> GetPeriodByFilter(PeriodFilter periodFilter)
        {
            List<Period> cycles = dac.GetPeriodByFilter(periodFilter);
            return cycles;
        }

        internal List<PeriodType> GetPeriodTypes()
        {
            return dac.GetPeriodTypes();
        }

        internal List<PeriodSet> GetAllPeriodSets()
        {
            return dac.GetAllPeriodSets();
        }

        internal List<PeriodSet> GetMyPeriodSets()
        {
            return dac.GetMyPeriodSets();
        }

        internal Period GetParentPeriod(string periodID)
        {
            return dac.GetParentPeriod(periodID);
        }

        internal List<Period> GetChildPeriods(string periodID)
        {
            return dac.GetChildPeriods(periodID);
        }

        internal void UpdateMyPeriodSets(List<string> periodSetIDList)
        {
            var db = Utils.GetDb();
            PeriodDac dac = new PeriodDac(db);
            db.BeginTransaction();
            try
            {
                dac.DeleteMyPeriodSets();
                dac.AddMyPeriodSets(periodSetIDList);
                db.Commit();
            }
            catch
            {
                db.Rollback();
                throw;
            }
        }
    }
}
