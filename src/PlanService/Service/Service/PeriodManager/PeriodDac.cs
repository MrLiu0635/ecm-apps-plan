using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.GSP.Caf.DataAccess;
using Newtonsoft.Json;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PeriodDac
    {
        IGSPDatabase db;
        public PeriodDac()
        {
            db = Utils.GetDb();
        }

        public PeriodDac(IGSPDatabase db)
        {
            this.db = db;
        }

        internal List<PeriodSet> GetAllPeriodSets()
        {
            List<PeriodSet> list = new List<PeriodSet>();
            var queryStr = $@"SELECT id, name FROM periodset where (tenantid = '0' or tenantid = '{Utils.GetTenantId()}')";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPeriodSet(row));
                }
            }
            return list;
        }

        internal void AddMyPeriodSets(List<string> periodSetIDList)
        {
            if (periodSetIDList == null || periodSetIDList.Count < 1) return;
            // 如果只选了默认周期集，就不用保存了
            if (periodSetIDList.Count == 1 && periodSetIDList[0].ToLower().Equals("default")) return;
            periodSetIDList.ForEach(periodSetID =>
            {
                var insertStr = $@"INSERT INTO periodsetallocation(id, periodsetid, tenantid) VALUES ('{Guid.NewGuid().ToString()}', '{periodSetID}', '{Utils.GetTenantId()}');";
                db.ExecSqlStatement(insertStr);
            });
        }

        internal void DeleteMyPeriodSets()
        {
            //新增一条数据
            var deleteStr = $@"Delete from periodsetallocation where tenantid = '{Utils.GetTenantId()}'";
            db.ExecSqlStatement(deleteStr);
        }

        internal List<PeriodSet> GetMyPeriodSets()
        {
            List<PeriodSet> list = new List<PeriodSet>();
            var queryStr = $@"SELECT b.id,b.name FROM periodsetallocation a inner join periodset b on b.id = a.periodsetid where a.tenantid = '{Utils.GetTenantId()}'";
            var ds = db.ExecuteDataSet(queryStr);
            if (ds != null && ds.Tables.Count != 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        list.Add(AssamblyPeriodSet(row));
                    }
                }
                else
                {
                    list.Add(new PeriodSet() { ID = "default", Name = "自然年" });
                }
            }
            return list;
        }

        private PeriodSet AssamblyPeriodSet(DataRow row)
        {
            PeriodSet set = new PeriodSet();
            set.ID = row["id"] as string;
            set.Name = row["name"] as string;
            return set;
        }

        internal Period GetParentPeriod(string periodID)
        {
            Period period = null;
            StringBuilder queryStr = new StringBuilder($@"select a.id,a.name,a.parentid,a.starttime,a.endtime,a.typeid, b.code as typecode, b.name as typename ,a.periodsetid,c.name as periodsetname, a.alias from period a inner join periodtype b on b.id = a.typeid inner join periodset c on c.id = a.periodsetid where a.id = (select parentid from period where id = '{periodID}')");

            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                period = AssamblyPeriod(ds.Tables[0].Rows[0]);
            }
            return period;
        }

        internal List<Period> GetChildPeriods(string periodID)
        {
            List<Period> list = new List<Period>();
            StringBuilder queryStr = new StringBuilder($@"select a.id,a.name,a.parentid,a.starttime,a.endtime,a.typeid, b.code as typecode, b.name as typename ,a.periodsetid,c.name as periodsetname, a.alias from period a inner join periodtype b on b.id = a.typeid inner join periodset c on c.id = a.periodsetid where a.parentid = '{periodID}'");

            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPeriod(row));
                }
            }
            return list;
        }

        internal List<Period> GetPeriodByFilter(PeriodFilter periodFilter)
        {
            List<Period> list = new List<Period>();
            StringBuilder queryStr = new StringBuilder($@"select a.id,a.name,a.parentid,a.starttime,a.endtime,a.typeid, b.code as typecode, b.name as typename ,a.periodsetid,c.name as periodsetname, a.alias from period a inner join periodtype b on b.id = a.typeid inner join periodset c on c.id = a.periodsetid where (a.tenantid = '0' or a.tenantid = '{Utils.GetTenantId()}') ");

            if (!string.IsNullOrEmpty(periodFilter.PeriodID))
                queryStr.Append($@" and a.id='{periodFilter.PeriodID}'");

            if (!string.IsNullOrEmpty(periodFilter.PeriodSetID))
                queryStr.Append($@" and a.periodsetid='{periodFilter.PeriodSetID}'");

            if (!string.IsNullOrEmpty(periodFilter.PeriodTypeID))
                queryStr.Append($@" and a.typeid='{periodFilter.PeriodTypeID}'");
            else if (!string.IsNullOrEmpty(periodFilter.PeriodTypeCode))
                queryStr.Append($@" and b.code='{periodFilter.PeriodTypeCode}'");

            DateTime defaultDateTime = new DateTime();
            if (periodFilter.Time != null && !periodFilter.Time.Equals(defaultDateTime))
            {
                queryStr.Append($@" and a.starttime <= '{periodFilter.Time}'");
                queryStr.Append($@" and a.endtime >= '{periodFilter.Time}'");
            }
            queryStr.Append($@" Order by a.name");
            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPeriod(row));
                }
            }
            return list;
        }

        private Period AssamblyPeriod(DataRow row)
        {
            Period period = new Period();
            period.ID = row["id"] as string;
            period.Name = row["name"] as string;
            period.ParentID = row["parentid"] as string;
            period.StartTime = Convert.ToDateTime(row["starttime"]);
            period.EndTime = Convert.ToDateTime(row["endtime"]);
            period.Type = new PeriodType() { ID = row["typeid"] as string, Name = row["typename"] as string, Code = row["typecode"] as string };
            period.Set = new PeriodSet() { ID = row["periodsetid"] as string, Name = row["periodsetname"] as string };
            period.Alias = row["alias"] as string;
            return period;
        }

        private PeriodType AssamblyPeriodType(DataRow row)
        {
            PeriodType pt = new PeriodType();
            pt.ID = row["id"] as string;
            pt.Code = row["code"] as string;
            pt.Name = row["name"] as string;
            return pt;
        }

        internal List<PeriodType> GetPeriodTypes()
        {
            List<PeriodType> list = new List<PeriodType>();
            StringBuilder queryStr = new StringBuilder($@"select a.id, a.code, a.name from periodtype a where (a.tenantid = '0' or a.tenantid = '{Utils.GetTenantId()}') ");

            var ds = db.ExecuteDataSet(queryStr.ToString());
            if (ds != null && ds.Tables.Count != 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssamblyPeriodType(row));
                }
            }
            return list;
        }

    }
}
