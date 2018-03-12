/*==============================================================*/
/* Table: ModelType  日志组件类型                                           */
/*==============================================================*/
Drop table if exists ModelType;
create table ModelType 
(
   ID                   varchar(36)                    not null,
   Code                 varchar(36)                    not null,
   Name                 varchar(256)                   null,
   tenantId             varchar(256)                   null,
   constraint PK_MODELTYPE primary key (ID)
);

/*==============================================================*/
/* Table: WRComponentModel   日志组件模板                                   */
/*==============================================================*/
Drop table if exists WRComponentModel;
create table WRComponentModel 
(
   ID                   varchar(36)                    not null,
   Name                 varchar(256)                   null,
   ModelTypeID          varchar(36)                    null,
   ModelOrder           int                            null,
   WRTypeID             varchar(36)                    null,
   tenantId             varchar(256)                   null,
   constraint PK_WRCOMPONENTMODEL primary key (ID)
);
/*==============================================================*/
/* Table: WRComponents       日志组件内容                                  */
/*==============================================================*/
Drop table if exists WRComponents;
create table WRComponents 
(
   ID                   varchar(36)                    not null,
   WRComponentModelID   varchar(36)                    null,
   WRID                 varchar(36)                    null,
   Content              varchar(2048)                  null,
   tenantId             varchar(256)                   null,
   constraint PK_WRCOMPONENTS primary key (ID)
);

/*==============================================================*/
/* Table: WRRecipients     日志接收人关联关系                                     */
/*==============================================================*/
Drop table if exists WRRecipients;
create table WRRecipients 
(
   ID                   varchar(36)                    not null,
   RecipientID          varchar(36)                    null,
   WRID                 varchar(36)                    null,
   tenantId             varchar(256)                   null,
   constraint PK_WRRECIPIENTS primary key (ID)
);

/*==============================================================*/
/* Table: WRType       日志类型                                         */
/*==============================================================*/
Drop table if exists WRType;
create table WRType 
(
   ID                   varchar(36)                    not null,
   Code                   varchar(36)                    not null,
   Name                 varchar(256)                   null,
   tenantId             varchar(256)                   null,
   constraint PK_WRTYPE primary key (ID)
);

-- ----------------------------
-- Table structure for wrpictures 图片
-- ----------------------------
DROP TABLE IF EXISTS wrpictures;
CREATE TABLE wrpictures
(
  ID            varchar(36)         NOT NULL,
  wrid          varchar(36)         null,
  content       bytea               null,
  tenantid      varchar(36)         null,
  constraint PK_WRPICTURES primary key (ID)
);

/*==============================================================*/
/* Table: WorkReport      日志                                      */
/*==============================================================*/
Drop table if exists WorkReport;
create table WorkReport 
(
   ID                   varchar(36)                     not null,
   WRTypeID             varchar(36)                     null,
   DateOfFilling        timestamp(0) without time zone  null,
   Location             varchar(1024)                   null,
   Creator              varchar(36)                     null,
   CreatedTime          timestamp(0) without time zone  null,
   LastModifier         varchar(36)                     null,
   LastModifiedTime     timestamp(0) without time zone  null,
   tenantId             varchar(256)                   null,
   constraint PK_WORKREPORT primary key (ID)
);

CREATE INDEX IDX_WorkReport_DateOfFilling ON WorkReport(DateOfFilling);
/* 外键
alter table WRComponentModel
   add constraint FK_WRCOMPON_REFERENCE_FIELDTYP foreign key (ModelTypeID)
      references ModelType (ID)
      on update restrict
      on delete restrict;

alter table WRComponentModel
   add constraint FK_WRCOMPON_REFERENCE_WRTYPE foreign key (WRTypeID)
      references WRType (ID)
      on update restrict
      on delete restrict;

alter table WRComponents
   add constraint FK_WRCOMPON_REFERENCE_WORKREPO foreign key (WRID)
      references WorkReport (ID)
      on update restrict
      on delete restrict;

alter table WRComponents
   add constraint FK_WRCOMPON_REFERENCE_WRCOMPON foreign key (WRComponentModelID)
      references WRComponentModel (ID)
      on update restrict
      on delete restrict;

alter table WRRecipients
   add constraint FK_WRRECIPI_REFERENCE_WORKREPO foreign key (WRID)
      references WorkReport (ID)
      on update restrict
      on delete restrict;

alter table WorkReport
   add constraint FK_WORKREPO_REFERENCE_WRTYPE foreign key (WRTypeID)
      references WRType (ID)
      on update restrict
      on delete restrict;
*/

