using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.GSP.Gsf.DataAccess;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanDac
    {
        IGSPDatabase db;
        public PlanDac()
        {
            db = Utils.GetDb();
        }

        public PlanDac(IGSPDatabase db)
        {
            this.db = db;
        }

        public List<WorkReport> GetWorkReportList(WRQueryFilter filterCondition)
        {
            List<WorkReport> reports = new List<WorkReport>();
            StringBuilder queryStr = new StringBuilder($@"Select distinct a.id, a.wrtypeid, a.dateoffilling, a.location, a.creator, a.createdtime, a.lastmodifier, a.lastmodifiedtime, b.name as wrtypename, b.code as wrtypecode from WorkReport a join wrtype b on a.wrtypeid = b.id and b.tenantid='{Utils.GetTenantId()}' left join wrrecipients c on c.wrid = a.id and c.tenantid='{Utils.GetTenantId()}' where a.tenantid='{Utils.GetTenantId()}' ");
            DateTime defaultDateTime = new DateTime();
            if (filterCondition.StartTime != null && !filterCondition.StartTime.Equals(defaultDateTime))
            {
                queryStr.Append($@" AND a.dateoffilling >= '{filterCondition.StartTime}' ");
            }

            if (filterCondition.EndTime != null && !filterCondition.EndTime.Equals(defaultDateTime))
            {
                queryStr.Append($@" AND a.dateoffilling <= '{filterCondition.EndTime}' ");
            }

            if (filterCondition.WorkReportSenders != null && filterCondition.WorkReportSenders.Count > 0)
            {
                queryStr.Append(@" AND a.creator in ('");
                queryStr.Append(string.Join("','", filterCondition.WorkReportSenders.ToArray()));
                queryStr.Append(@"') ");
                queryStr.Replace("'self'", "'" + PlanState.UserId + "'");
            }
            if (filterCondition.WorkReportRecipients != null && filterCondition.WorkReportRecipients.Count > 0)
            {
                queryStr.Append(@" AND c.recipientid in ('");
                queryStr.Append(string.Join("','", filterCondition.WorkReportRecipients.ToArray()));
                queryStr.Append(@"') ");
                queryStr.Replace("'self'", "'" + PlanState.UserId + "'");
            }
            queryStr.Append(@" order by a.createdtime desc");
            using (var dbReader = db.ExecuteReader(queryStr.ToString()))
            {
                //将数据库信息封装到实体类
                while (dbReader.Read())
                {
                    reports.Add(AssemblyWorkReport(dbReader));
                }
            }

            return reports;
        }

        internal List<WRPicture> GetPicturesByID(string ID)
        {
            List<WRPicture> pics = new List<WRPicture>();
            var queryStr = $@"select a.id, a.wrid, encode(a.content, 'base64') as content from wrpictures a join workreport b on a.wrid = b.id and b.tenantid = '{Utils.GetTenantId()}' where a.tenantid = '{Utils.GetTenantId()}' and b.id = '{ID}'";
            using (var dbReader = db.ExecuteReader(queryStr))
            {
                //将数据库信息封装到实体类
                while (dbReader.Read())
                {
                    pics.Add(AssamblyPic(dbReader));
                }
            }
            return pics;
        }

        internal void UpdateWorkReportBaseInfo(WorkReport workReport)
        {
            //更新一条记录
            db.ExecSqlStatement($@"update WorkReport set dateoffilling = '{workReport.DateOfFilling.ToString("yyyy-MM-dd HH:mm:ss")}', lastmodifier='{ Utils.GetUserId()}',lastmodifiedtime={db.DBDataTimeFunc} where id='{workReport.ID}' and tenantid='{Utils.GetTenantId()}'");
        }

        internal string AddWorkReportBaseInfo(WorkReport workReport)
        {
            if (string.IsNullOrWhiteSpace(workReport.ID))
            {
                workReport.ID = Guid.NewGuid().ToString();
            }
            var insertStr = $@"INSERT INTO workreport(id, wrtypeid, dateoffilling,location, creator, createdtime, lastmodifier, lastmodifiedtime, tenantid) VALUES ('{workReport.ID}', '{workReport.WorkReportType.ID}', '{workReport.DateOfFilling.ToString("yyyy-MM-dd HH:mm:ss")}','{workReport.Location}', '{Utils.GetUserId()}', {db.DBDataTimeFunc}, '{Utils.GetUserId()}', {db.DBDataTimeFunc}, '{Utils.GetTenantId()}')";
            //新增一条数据
            db.ExecSqlStatement(insertStr);
            return workReport.ID;
        }

        internal void AddWorkReportPictures(List<WRPicture> pics, string wrid)
        {
            if (pics == null || pics.Count < 1)
            {
                return;
            }
            for (int i = 0; i < pics.Count; i++)
            {
                WRPicture pic = pics[i];
                if (string.IsNullOrWhiteSpace(pic.ID) || pic.ID.Length != 36)
                {
                    pic.ID = Guid.NewGuid().ToString();
                }
                //新增一条数据
                var insertStr = $@"INSERT INTO wrpictures(id, wrid, content, tenantid) VALUES ('{pic.ID}', '{wrid}', decode('{pic.Content}','base64'), '{Utils.GetTenantId()}')";
                db.ExecSqlStatement(insertStr);
            }
        }

        internal void AddWorkReportComps(List<WRComponent> comps, string wrid)
        {
            if (comps == null || comps.Count < 1)
            {
                return;
            }
            for (int i = 0; i < comps.Count; i++)
            {
                WRComponent comp = comps[i];
                if (string.IsNullOrWhiteSpace(comp.ID))
                {
                    comp.ID = Guid.NewGuid().ToString();
                }
                //新增一条数据
                var insertStr = $@"INSERT INTO wrcomponents(id, content, wrcomponentmodelid, wrid, tenantid) VALUES ('{comp.ID}', '{comp.Content}', '{comp.Model.ID}', '{wrid}', '{Utils.GetTenantId()}')";
                db.ExecSqlStatement(insertStr);
            }
        }

        internal void AddWorkReportRecipients(List<SysUser> users, string wrid)
        {
            if (users == null || users.Count < 1)
            {
                return;
            }
            for (int i = 0; i < users.Count; i++)
            {
                SysUser user = users[i];

                //新增一条数据
                db.ExecSqlStatement($@"INSERT INTO wrrecipients(id, recipientid, wrid, tenantid) VALUES ('{Guid.NewGuid().ToString()}', '{user.ID}', '{wrid}', '{Utils.GetTenantId()}')");
            }
        }

        internal List<WorkReport> GetWorkReportByRecipientID(List<string> Recipients)
        {
            List<WorkReport> reports = new List<WorkReport>();
            StringBuilder queryStr = new StringBuilder($@"Select distinct a.id, a.wrtypeid, a.dateoffilling,a.location, a.creator, a.createdtime, a.lastmodifier, a.lastmodifiedtime, b.name as wrtypename, b.code as wrtypecode from WorkReport a join wrtype b on a.wrtypeid = b.id and b.tenantid='{Utils.GetTenantId()}' left join wrrecipients c on c.wrid = a.id and c.tenantid='{Utils.GetTenantId()}' where a.tenantid='{Utils.GetTenantId()}' ");
            if (Recipients != null && Recipients.Count > 0)
            {
                queryStr.Append(@" AND c.recipientid in ('");
                queryStr.Append(string.Join("','", Recipients.ToArray()));
                queryStr.Append(@"') ");
            }

            using (var dbReader = db.ExecuteReader(queryStr.ToString()))
            {
                //将数据库信息封装到实体类
                while (dbReader.Read())
                {
                    reports.Add(AssemblyWorkReport(dbReader));
                }
            }
            return reports;
        }

        public WorkReport GetWorkReportByID(string workReportID)
        {
            var qeuryStr = $@"SELECT a.id, a.wrtypeid, a.dateoffilling,a.location, a.creator, a.createdtime, a.lastmodifier, a.lastmodifiedtime, a.tenantid, b.Code as wrtypecode, b.NAME as wrtypename,	b.tenantid FROM workreport AS a	JOIN wrtype AS b ON a.wrtypeid = b.id and b.tenantid='{Utils.GetTenantId()}' where a.id = '{workReportID}' and a.tenantid='{Utils.GetTenantId()}' ";
            WorkReport wr = null;
            using (var dbReader = db.ExecuteReader(qeuryStr))
            {
                //将数据库信息封装到实体类
                dbReader.Read();
                wr = AssemblyWorkReport(dbReader);
            }
            return wr;

        }
        public List<WRComponentModel> GetComponentModelList(string wrTypeCode)
        {
            List<WRComponentModel> list = new List<WRComponentModel>();
            var queryStr = @"SELECT
    a.id AS modelid,
    a.NAME AS modelname,
    a.modeltypeid,
    a.modelorder,
    a.wrtypeid,
    a.tenantid,
    b.CODE AS wrtypecode,
    b.NAME AS wrtypename,
    c.CODE AS modeltypecode,
    c.NAME AS modeltypename
FROM
    wrcomponentmodel a
    JOIN wrtype b ON a.wrtypeid = b.id AND a.tenantid = b.tenantid
    JOIN modeltype c ON a.modeltypeid = c.id AND a.tenantid = c.tenantid
WHERE
   a.tenantid={0} and  b.CODE = {1} order by modelorder";
            var ds = db.ExecuteDataSet(queryStr, Utils.GetTenantId(), wrTypeCode);
            if(ds!=null&&ds.Tables.Count!=0&&ds.Tables[0].Rows.Count>0)
            {
                foreach(DataRow row in ds.Tables[0].Rows)
                {
                    list.Add(AssemblyWRComponentModel(row));
                }
            }
            return list;
        }
        internal void DeleteWorkReportBaseInfo(string workReportID)
        {
            db.ExecSqlStatement($@"Delete from WorkReport where id ='{workReportID}' and tenantid='{Utils.GetTenantId()}'");
        }

        internal void DeletaWorkReportComps(string workReportID)
        {
            db.ExecSqlStatement($@"Delete from wrcomponents where wrid ='{workReportID}' and tenantid='{Utils.GetTenantId()}'");
        }

        internal void DeletaWorkReportPics(string workReportID)
        {
            db.ExecSqlStatement($@"Delete from wrpictures where wrid ='{workReportID}' and tenantid='{Utils.GetTenantId()}'");
        }

        internal void DeletaWorkReportPics(List<string> picIDs)
        {
            if (picIDs == null || picIDs.Count <= 0) return;
            StringBuilder delStr = new StringBuilder($@"Delete from wrpictures where tenantid='{Utils.GetTenantId()}' and id in('");
            delStr.Append(string.Join("','", picIDs.ToArray()));
            delStr.Append(@"') ");
            db.ExecSqlStatement(delStr.ToString());
        }

        internal void DeletaWorkReportRecipients(string workReportID)
        {
            db.ExecSqlStatement($@"Delete from wrrecipients where wrid ='{workReportID}' and tenantid='{Utils.GetTenantId()}'");
        }

        internal List<WRComponent> GetComponentsByID(string wrid)
        {
            List<WRComponent> comps = new List<WRComponent>();
            var queryStr = $@"SELECT a.id, a.content, a.wrcomponentmodelid,a.wrid,a.tenantid, b.id as modelid, b.NAME as modelname, b.modeltypeid, b.modelorder, b.wrtypeid, b.tenantid, c.CODE as wrtypecode, c.NAME as wrtypename, c.tenantid, d.CODE as modeltypecode, d.id as modeltypeid, d.NAME as modeltypename, d.tenantid FROM wrcomponents a JOIN wrcomponentmodel b ON a.wrcomponentmodelid = b.id AND b.tenantid = '{Utils.GetTenantId()}' JOIN wrtype c ON b.wrtypeid = c.id  AND c.tenantid = '{Utils.GetTenantId()}' JOIN modeltype d ON b.modeltypeid = d.id AND d.tenantid = '{Utils.GetTenantId()}' WHERE a.wrid = '{wrid}' AND a.tenantid = '{Utils.GetTenantId()}'";
            queryStr += " order by b.modelorder";
            using (var dbReader = db.ExecuteReader(queryStr))
            {
                //将数据库信息封装到实体类
                while (dbReader.Read())
                {
                    comps.Add(AssamblyComponent(dbReader));
                }
            }
            return comps;
        }

        internal List<string> GetRecipientsByID(string wrid)
        {
            List<string> users = new List<string>();
            
            using (var dbReader = db.ExecuteReader($@"Select * from wrrecipients where wrid = '{wrid}' AND tenantid = '{Utils.GetTenantId()}'"))
            {
                //将数据库信息封装到实体类
                while (dbReader.Read())
                {
                    string id = dbReader["recipientid"].ToString();
                    users.Add(id);
                }
            }
            return users;
        }

        private WRComponent AssamblyComponent(IDataReader reader)
        {
            WRComponentModel model = new WRComponentModel();
            model.ID = reader["modelid"].ToString();
            model.Name = reader["modelname"].ToString();
            int order = -1;
            string orderStr = Convert.ToString(reader["modelorder"]);
            if (!string.IsNullOrWhiteSpace(orderStr))
                int.TryParse(orderStr, out order);
            model.Order = order;
            model.WorkReportType = new WRType()
            {
                ID = reader["wrtypeid"].ToString(),
                Code = reader["wrtypecode"].ToString(),
                Name = reader["wrtypename"].ToString()
            };
            model.ModelType = new ModelType()
            {
                ID = reader["modeltypeid"].ToString(),
                Code = reader["modeltypecode"].ToString(),
                Name = reader["modeltypename"].ToString()
            };

            WRComponent comp = new WRComponent()
            {
                ID = reader["id"].ToString(),
                Content = reader["content"].ToString(),
                Model = model
            };

            return comp;
        }

        private WorkReport AssemblyWorkReport(IDataReader reader)
        {
            WorkReport wr = new WorkReport
            {
                ID = reader["id"].ToString(),
                WorkReportType = new WRType() { ID = reader["wrtypeid"].ToString(), Name = reader["wrtypename"].ToString(), Code = reader["wrtypecode"].ToString() },
                DateOfFilling = Convert.ToDateTime(reader["dateoffilling"].ToString()),
                Location=reader["location"].ToString(),
                Creator = reader["creator"].ToString(),
                CreatedTime = Convert.ToDateTime(reader["createdtime"].ToString()),
                LastModifier = reader["lastmodifier"].ToString(),
                LastModifiedTime = Convert.ToDateTime(reader["lastmodifiedtime"].ToString())
            };
            return wr;
        }

        private WRPicture AssamblyPic(IDataReader reader)
        {
            WRPicture wrPic = new WRPicture();
            wrPic.ID = reader["id"].ToString();
            string content = reader["content"].ToString();
            if (!string.IsNullOrEmpty(content)) wrPic.Content = content.Replace("\n", "");
            return wrPic;
        }


        private WRComponentModel AssemblyWRComponentModel(DataRow row)
        {
            WRComponentModel model = new WRComponentModel();
            model.ID = row["modelid"].ToString();
            model.Name = row["modelname"].ToString();
            int order = -1;
            string orderStr = Convert.ToString(row["modelorder"]);
            if (!string.IsNullOrWhiteSpace(orderStr))
                int.TryParse(orderStr, out order);
            model.Order = order;
            model.WorkReportType = new WRType()
            {
                ID = row["wrtypeid"].ToString(),
                Code = row["wrtypecode"].ToString(),
                Name = row["wrtypename"].ToString()
            };
            model.ModelType = new ModelType()
            {
                ID = row["modeltypeid"].ToString(),
                Code = row["modeltypecode"].ToString(),
                Name = row["modeltypename"].ToString()
            };
            return model;
        }
    }
}
