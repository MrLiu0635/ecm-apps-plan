/*==============================================================*/
/* Table: CycleDefine                                           */
/*==============================================================*/
Drop table if exists CycleDefine;
create table CycleDefine
(
   ID                   varchar(36)                    not null,
   Code                 varchar(36)                    null,
   Name                 varchar(128)                   null,
   ParentCycleID        varchar(36)                    null,
   TenantID             varchar(36)                    null,
   constraint PK_CYCLEDEFINE primary key (ID)
);

/*==============================================================*/
/* Table: PlanCategory                                          */
/*==============================================================*/
Drop table if exists PlanCategory;
create table PlanCategory
(
   ID                   varchar(36)                     not null,
   Code                 varchar(36)                     null,
   Name                 varchar(128)                    null,
   TenantID             varchar(36)                     null,
   constraint PK_PLANCATEGORY primary key (ID)
);

/*==============================================================*/
/* Table: ItemModelOfCategory                                   */
/*==============================================================*/
Drop table if exists ItemModelOfCategory;
create table ItemModelOfCategory
(
   ID                   varchar(36)                     not null,
   PlanCategoryID       varchar(36)                     null,
   PlanItemModelID      varchar(36)                     null,
   TenantID             varchar(36)                     null,
   constraint PK_ITEMMODELOFCATEGORY primary key (ID)
);

/*==============================================================*/
/* Table: PlanItemModel                                         */
/*==============================================================*/
Drop table if exists PlanItemModel;
create table PlanItemModel
(
   ID                   varchar(36)                    not null,
   Name                 varchar(256)                   null,
   Type                 int                            null,
   ModelContent         varchar(1024)                  null,
   TenantID             varchar(36)                    null,
   constraint PK_PLANITEMMODEL primary key (ID)
);

/*==============================================================*/
/* Table: Plan                                                  */
/*==============================================================*/
Drop table if exists Plan;
create table Plan 
(
   ID                   varchar(36)                    not null,
   Name                 varchar(128)                    null,
   PlanCategoryID       varchar(36)                    null,
   CycleID              varchar(36)                    null,
   StartTime            timestamp(0) without time zone  null,
   EndTime              timestamp(0) without time zone  null,
   MainRecipient        varchar(36)                    null,
   State                int                            null,
   Stage                int                            null,
   TenantID             varchar(36)                    null,
   Creator              varchar(36)                    null,
   CreatedTime          timestamp(0) without time zone  null,
   LastModifiedTime     timestamp(0) without time zone  null,
   constraint PK_PLAN primary key (ID)
);

CREATE INDEX IDX_PLAN_CREATOR ON Plan(Creator);

/*==============================================================*/
/* Table: WRRecipients     计划接收人关联关系                   */
/*==============================================================*/
Drop table if exists CarbonCopyPlanRecipients;
create table CarbonCopyPlanRecipients 
(
   ID                   varchar(36)                    not null,
   RecipientID          varchar(36)                    null,
   WRID                 varchar(36)                    null,
   tenantId             varchar(256)                   null,
   constraint PK_CARBONCOPYPLANRECIPIENTS primary key (ID)
);

/*==============================================================*/
/* Table: PlanItem                                              */
/*==============================================================*/
Drop table if exists PlanItem;
create table PlanItem
(
   ID                   varchar(36)                    not null,
   Name                 varchar(128)                    null,
   PlanID               varchar(36)                    null,
   StartTime            timestamp(0) without time zone  null,
   EndTime              timestamp(0) without time zone  null,
   PlanItemOrder        int                            null,
   ParentPlanItemID     varchar(36)                    null,
   SourcePlanItemID     varchar(36)                    null,
   PlanItemModelID      varchar(36)                    null,
   PlanItemContent      varchar(1024)                  null,
   SelfAssessment                   varchar(1024)                  null,
   SelfAssessmentScore              int                            null,
   AssessmentOfSuperior             varchar(1024)                  null,
   AssessmentOfSuperiorScore        int                            null,
   SummaryContent                   varchar(1024)                  null,
   TenantID                         varchar(36)                    null,
   constraint PK_PLANITEM primary key (ID)

);

CREATE INDEX IDX_PLANITEM_PLANID ON PlanItem(PlanID);

/*==============================================================*/
/* Table: 数值型日报内容                                        */
/*==============================================================*/
Drop table if exists WRNumericContent;
create table WRNumericContent
(
   ID                   varchar(36)                    not null,
   WorkReportID         varchar(36)                    null,
   PlanItemID           varchar(36)                    null,
   Content              varchar(1024)                  null,
   TenantID             varchar(36)                    null,
   constraint PK_WRNUMERICCONTENT primary key (ID)
);

/*==============================================================*/
/* Table: WorkReportDaily                                            */
/*==============================================================*/
Drop table if exists WorkReportDaily;
create table WorkReportDaily
(
   ID                   varchar(36)                    not null,
   FinishedWork         varchar(1024)                  null,
   UnfinishedWork       varchar(1024)                  null,
   Note                 varchar(1024)                  null,
   Location             varchar(1024)                  null,
   MainRecipient        varchar(36)                    null,
   FillTime             timestamp(0) without time zone  null,
   userid               varchar(36)                     null,
   CreatedTime          timestamp(0) without time zone  null,
   LastModifiedTime     timestamp(0) without time zone  null,
   TenantID             varchar(36)                    null,
   constraint PK_WORKREPORTDAILY primary key (ID)
);

/*==============================================================*/
/* Table: WRRecipients     日志接收人关联关系                                     */
/*==============================================================*/
Drop table if exists CarbonCopyWRRecipients;
create table CarbonCopyWRRecipients 
(
   ID                   varchar(36)                    not null,
   RecipientID          varchar(36)                    null,
   WRID                 varchar(36)                    null,
   tenantId             varchar(256)                   null,
   constraint PK_CARBONCOPYWRRECIPIENTS primary key (ID)
);

-- ----------------------------
-- Table structure for wrpictures 图片
-- ----------------------------
DROP TABLE IF EXISTS wrpictures;
CREATE TABLE wrpictures
(
  ID            varchar(36)         NOT NULL,
  WorkReportID  varchar(36)         null,
  content       bytea               null,
  tenantid      varchar(36)         null,
  constraint PK_WRPICTURES primary key (ID)
);