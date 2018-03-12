using System;
using System.Collections.Generic;
using System.Text;
using Inspur.EcmCloud.Apps.Plan.Service.Entity;
//using Inspur.EcmCloud.Apps.PlanManager.Service.Entity;

namespace Inspur.EcmCloud.Apps.Plan.Service
{
    public class PlanService
    {
        private readonly PlanManager manager = new PlanManager();
        private static PlanService instance = null;

        /// <summary>
        /// 默认构造函数
        /// </summary>
        private PlanService()
        {
            // 测试时
            // PlanState.IsTest = true;
        }

        public static PlanService Current => instance ?? (instance = new PlanService());
        public List<WorkReport> GetWorkReportList(WRQueryFilter filterCondition)
        {
            return manager.GetWorkReportList(filterCondition);
        }

        public WorkReport GetWorkReportByID(string id)
        {
            return manager.GetWorkReportByID(id);
        }
        public WorkReportModel GetComponentModelList(string wrTypeCode)
        {
            return manager.GetComponentModelList(wrTypeCode);
        }
        public void DeleteWorkReports(List<string> workReportIDs)
        {
            manager.DeleteWorkReports(workReportIDs);
        }

        public List<string> AddWorkReports(List<WorkReport> workReports)
        {
            return manager.AddWorkReports(workReports);
        }

        public void UpdateWorkReport(WorkReport workReport, bool isReNotice)
        {
            manager.UpdateWorkReport(workReport, isReNotice);
        }
    }
}
