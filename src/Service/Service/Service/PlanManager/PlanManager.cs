using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Core;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanManager
    {    
        EcpUserService userService = new EcpUserService();

        public List<WorkReport> GetWorkReportList(WRQueryFilter filterCondition)
        {
            if (filterCondition == null) return null;
            PlanDac planDac = new PlanDac();
            List<WorkReport> reports = planDac.GetWorkReportList(filterCondition);
            if (reports != null && reports.Count > 0)
            {
                for (int i = 0; i < reports.Count; i++)
                {
                    WorkReport wr = reports[i];
                    if (!string.IsNullOrEmpty(wr.Creator))
                    {
                        SysUser user = userService.GetUserByID(wr.Creator);
                        wr.CreatorName = user == null? "某某" : user.Name;
                    }
                }
            }
            return reports;
        }


        public WorkReport GetWorkReportByID(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;
            PlanDac planDac = new PlanDac();
            WorkReport wr = planDac.GetWorkReportByID(id);

            wr.WorkReportComps = planDac.GetComponentsByID(wr.ID);
            wr.WorkReportPics = planDac.GetPicturesByID(wr.ID);
            List<string> userIDs = planDac.GetRecipientsByID(wr.ID);
            if (userIDs != null && userIDs.Count > 0)
            {
                wr.WorkReportRecipients = GetUserInfos(userIDs);
            }
            if(!string.IsNullOrEmpty(wr.Creator))
            {
                SysUser user = userService.GetUserByID(wr.Creator);
                wr.CreatorName = user == null ? "某某" : user.Name;
            }
            return wr;
        }
        public WorkReportModel GetComponentModelList(string wrTypeCode)
        {
            if (string.IsNullOrWhiteSpace(wrTypeCode)) return null;
            PlanDac planDac = new PlanDac();
            List<WRComponentModel> list = new List<WRComponentModel>();
            list = planDac.GetComponentModelList(wrTypeCode);
            WorkReportModel model = new WorkReportModel();
            model.ComponentModels = list;
            if (list != null && list.Count > 0)
            {
                model.WRType = list[0].WorkReportType;
                model.ID = Guid.NewGuid().ToString();
            }
            return model;
        }
        private List<SysUser> GetUserInfos(List<string> users)
        {
            List<SysUser> userInfos = new List<SysUser>();
            users.ForEach(userID=> 
            {
                Console.WriteLine($"TODO 获取了用户{userID}信息");
                SysUser user = userService.GetUserByID(userID);
                userInfos.Add(user ?? new SysUser() { ID = "9999", Name = "某某某" });
            });
            return userInfos;
        }

        internal List<string> AddWorkReports(List<WorkReport> workReports)
        {
            if (workReports == null || workReports.Count < 1) return null;
            var db = Utils.GetDb();
            PlanDac planDac = new PlanDac(db);
            List<string> users = new List<string>();
            List<string> wrids = new List<string>();
            db.BeginTransaction();
            try
            {

                foreach (var workReport in workReports)
                {
                    string wrid = planDac.AddWorkReportBaseInfo(workReport);
                    wrids.Add(wrid);
                    planDac.AddWorkReportComps(workReport.WorkReportComps, wrid);
                    planDac.AddWorkReportPictures(workReport.WorkReportPics, wrid);
                    planDac.AddWorkReportRecipients(workReport.WorkReportRecipients, wrid);
                    if (workReport.WorkReportRecipients != null && workReport.WorkReportRecipients.Count > 0)
                    {
                        workReport.WorkReportRecipients.ForEach(user =>
                        {
                            if (!users.Contains(user.ID))
                                users.Add(user.ID);
                        });
                    }
                }
                db.Commit();
                if (users.Count > 0)
                    NoticeRecipients(users);
                return wrids;
            }
            catch
            {
                db.Rollback();
                return null;
            }
        }

        internal void DeleteWorkReports(List<string> workReportIDs)
        {
            if (workReportIDs == null || workReportIDs.Count < 1) return;
            var db = Utils.GetDb();
            foreach (var workReportID in workReportIDs)
            {
                PlanDac planDac = new PlanDac(db);
                db.BeginTransaction();
                try
                {
                    planDac.DeleteWorkReportBaseInfo(workReportID);
                    planDac.DeletaWorkReportComps(workReportID);
                    planDac.DeletaWorkReportRecipients(workReportID);
                    planDac.DeletaWorkReportPics(workReportID);
                    db.Commit();
                }
                catch
                {
                    db.Rollback();
                }
            }
        }

        internal void UpdateWorkReport(WorkReport workReport ,bool isReNotice)
        {
            if (workReport == null || string.IsNullOrWhiteSpace(workReport.ID)) return;
            var db = Utils.GetDb();
            PlanDac planDac = new PlanDac(db);
            db.BeginTransaction();
            try
            {
                planDac.UpdateWorkReportBaseInfo(workReport);
                planDac.DeletaWorkReportComps(workReport.ID);
                // planDac.DeletaWorkReportRecipients(workReport.ID);
                planDac.AddWorkReportComps(workReport.WorkReportComps, workReport.ID);
                // planDac.AddWorkReportRecipients(workReport);

                List<WRPicture> newPics = workReport.WorkReportPics;
                if (newPics == null || newPics.Count <= 0)
                    planDac.DeletaWorkReportPics(workReport.ID);
                else
                {
                    List<WRPicture> oldPics = planDac.GetPicturesByID(workReport.ID);
                    if (oldPics == null || oldPics.Count <= 0)
                        planDac.AddWorkReportPictures(newPics, workReport.ID);
                    else
                    {
                        List<WRPicture> newAddPics = new List<WRPicture>();
                        List<string> deletedPics = new List<string>();
                        List<string> oldPicIDs = new List<string>();
                        List<string> newPicIDs = new List<string>();
                        oldPics.ForEach(pic =>
                        {
                            oldPicIDs.Add(pic.ID);
                        });
                        newPics.ForEach(pic =>
                        {
                            newPicIDs.Add(pic.ID);
                            if (string.IsNullOrEmpty(pic.ID) || !oldPicIDs.Contains(pic.ID))
                                newAddPics.Add(pic);
                        });
                        oldPics.ForEach(pic =>
                        {
                            if (!newPicIDs.Contains(pic.ID))
                                deletedPics.Add(pic.ID);
                        });
                        planDac.DeletaWorkReportPics(deletedPics);
                        planDac.AddWorkReportPictures(newAddPics, workReport.ID);
                    }
                }

                db.Commit();
                // 推送通知
                if (isReNotice)
                {
                    List<string> recipients = planDac.GetRecipientsByID(workReport.ID);
                    if (recipients != null && recipients.Count > 0)
                    {
                        NoticeRecipients(recipients);
                    }
                }
            }
            catch
            {
                db.Rollback();
            }
        }

        private void NoticeRecipients(List<string> recipients)
        {
            if (recipients != null && recipients.Count > 0)
            {
                recipients = userService.GetInspurIdByUserId(recipients, PlanState.TenantId);
                NoticeByInspurID(recipients);
            }
        }

        private void NoticeRecipients(List<SysUser> recipients)
        {
            if(recipients != null && recipients.Count > 0)
            {
                List<string> recipientsTemp = new List<string>();
                recipients.ForEach(recipient =>
                {
                    if (!recipientsTemp.Contains(recipient.ID))
                        recipientsTemp.Add(recipient.ID);
                });
                recipientsTemp = userService.GetInspurIdByUserId(recipientsTemp, PlanState.TenantId);
                NoticeByInspurID(recipientsTemp);
            }
        }

        private void NoticeByInspurID(List<string> recipients)
        {
            MessageInfo messageInfo = new MessageInfo();
            messageInfo.Subject = "您有新的消息。";

            var content = new StringBuilder(100);
            content.AppendLine("您有新的日志推送,请及时查看！");

            messageInfo.Content = content.ToString();
            messageInfo.UserList = recipients;

            Utils.SendMessage(messageInfo.Subject, messageInfo.Content, messageInfo.UserList);
        }
    }
}
