/*
 Navicat Premium Data Transfer

 Source Server         : 27_PG
 Source Server Type    : PostgreSQL
 Source Server Version : 100000
 Source Host           : 10.24.13.27:5432
 Source Catalog        : plan
 Source Schema         : public

 Target Server Type    : PostgreSQL
 Target Server Version : 100000
 File Encoding         : 65001

 Date: 21/12/2017 13:51:14
*/


-- ----------------------------
-- Table structure for carboncopyplanrecipients
-- ----------------------------
DROP TABLE IF EXISTS carboncopyplanrecipients;
CREATE TABLE carboncopyplanrecipients (
  id varchar(36) NOT NULL,
  recipientid varchar(36),
  planid varchar(36),
  tenantid varchar(256)
)
;

-- ----------------------------
-- Table structure for carboncopywrrecipients
-- ----------------------------
DROP TABLE IF EXISTS carboncopywrrecipients;
CREATE TABLE carboncopywrrecipients (
  id varchar(36) NOT NULL,
  recipientid varchar(36),
  wrid varchar(36),
  tenantid varchar(256)
)
;

-- ----------------------------
-- Table structure for period
-- ----------------------------
DROP TABLE IF EXISTS period;
CREATE TABLE period (
  id varchar(36) NOT NULL,
  name varchar(255),
  parentid varchar(36),
  starttime timestamp(6),
  endtime timestamp(6),
  typeid varchar(36),
  periodsetid varchar(36),
  tenantid varchar(36),
  alias varchar(255)
)
;

-- ----------------------------
-- Table structure for periodallocation
-- ----------------------------
DROP TABLE IF EXISTS periodallocation;
CREATE TABLE periodallocation (
  id varchar(36) NOT NULL,
  periodsetid varchar(36),
  trenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for periodset
-- ----------------------------
DROP TABLE IF EXISTS periodset;
CREATE TABLE periodset (
  id varchar(36) NOT NULL,
  name varchar(1024),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for periodtype
-- ----------------------------
DROP TABLE IF EXISTS periodtype;
CREATE TABLE periodtype (
  id varchar(36) NOT NULL,
  code varchar(255),
  name varchar(1024),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for plan
-- ----------------------------
DROP TABLE IF EXISTS plan;
CREATE TABLE plan (
  id varchar(36) NOT NULL,
  name varchar(36),
  periodid varchar(36),
  mainrecipient varchar(36),
  state int4,
  stage int4,
  tenantid varchar(36),
  userid varchar(36),
  createdtime timestamp(0),
  lastmodifiedtime timestamp(0),
  approvalinstance varchar(36),
  plandefineid varchar(36)
)
;
COMMENT ON COLUMN plan.state IS '0:制单
1:审批中
2:审批通过
3:审批不通过';
COMMENT ON COLUMN plan.stage IS '0:制定中
1:计划执行
2:自评完成
3:上级评价完成
4:归档';

-- ----------------------------
-- Table structure for plandefine
-- ----------------------------
DROP TABLE IF EXISTS plandefine;
CREATE TABLE plandefine (
  id varchar(36) NOT NULL,
  name varchar(255),
  state int4,
  modelid varchar(36),
  setid varchar(36),
  typeid varchar(36),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for plandefineallocation
-- ----------------------------
DROP TABLE IF EXISTS plandefineallocation;
CREATE TABLE plandefineallocation (
  id varchar(36) NOT NULL,
  roleid varchar(36),
  plandefineid varchar(36),
  tenantid varchar(36),
  orgid varchar(36)
)
;

-- ----------------------------
-- Table structure for plandefinetempstate
-- ----------------------------
DROP TABLE IF EXISTS plandefinetempstate;
CREATE TABLE plandefinetempstate (
  id varchar(36) NOT NULL,
  plandefineid varchar(36),
  periodid varchar(36),
  state int4,
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for planitem
-- ----------------------------
DROP TABLE IF EXISTS planitem;
CREATE TABLE planitem (
  id varchar(36) NOT NULL,
  name varchar(36),
  planid varchar(36),
  starttime timestamp(0),
  endtime timestamp(0),
  itemorder int4,
  parentplanitemid varchar(36),
  sourceplanitemid varchar(36),
  planitemcontent varchar(1024),
  createdtime timestamp(0),
  lastmodifiedtime timestamp(0),
  selfassessment varchar(1024),
  assessmentofsuperior varchar(1024),
  selfassessmentscore varchar(36),
  assessmentofsuperiorscore varchar(36),
  summarycontent varchar(1024),
  tenantid varchar(36),
  weight varchar(36)
)
;
-- ----------------------------
-- Table structure for planitemcustomization
-- ----------------------------
DROP TABLE IF EXISTS planitemcustomization;
CREATE TABLE planitemcustomization (
  id varchar(36) NOT NULL,
  tenantid varchar(36),
  modeldesc varchar(1024),
  plandefineid varchar(36)
)
;

-- ----------------------------
-- Table structure for planitemmodel
-- ----------------------------
DROP TABLE IF EXISTS planitemmodel;
CREATE TABLE planitemmodel (
  id varchar(36) NOT NULL,
  name varchar(36),
  modelcontent varchar(1024),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for role
-- ----------------------------
DROP TABLE IF EXISTS role;
CREATE TABLE role (
  id varchar(36) NOT NULL,
  name varchar(255),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for workreport
-- ----------------------------
DROP TABLE IF EXISTS workreport;
CREATE TABLE workreport (
  id varchar(36) NOT NULL,
  content varchar(2048),
  wrmodelid varchar(36),
  dateoffilling timestamp(0),
  state int4,
  location varchar(1024),
  mainrecipient varchar(36),
  creator varchar(36),
  createtime timestamp(0),
  lastmodifier varchar(36),
  lastmodifiedtime timestamp(0),
  tenantid varchar(36),
  name varchar(255)
)
;

-- ----------------------------
-- Table structure for wrmodel
-- ----------------------------
DROP TABLE IF EXISTS wrmodel;
CREATE TABLE wrmodel (
  id varchar(36) NOT NULL,
  content varchar(2048),
  name varchar(256),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for wrmodelofrole
-- ----------------------------
DROP TABLE IF EXISTS wrmodelofrole;
CREATE TABLE wrmodelofrole (
  id varchar(36) NOT NULL,
  roleid varchar(36),
  modelid varchar(36),
  tenantid varchar(36)
)
;

-- ----------------------------
-- Table structure for wrpictures
-- ----------------------------
DROP TABLE IF EXISTS wrpictures;
CREATE TABLE wrpictures (
  id varchar(36) NOT NULL,
  workreportid varchar(36),
  content bytea,
  tenantid varchar(36)
)
;

-- ----------------------------
-- Primary Key structure for table carboncopyplanrecipients
-- ----------------------------
ALTER TABLE carboncopyplanrecipients ADD CONSTRAINT pk_carboncopyplanrecipients PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table carboncopywrrecipients
-- ----------------------------
ALTER TABLE carboncopywrrecipients ADD CONSTRAINT pk_carboncopywrrecipients PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table period
-- ----------------------------
ALTER TABLE period ADD CONSTRAINT pk_timespec PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table periodset
-- ----------------------------
ALTER TABLE periodset ADD CONSTRAINT timetag_pkey PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table plan
-- ----------------------------
ALTER TABLE plan ADD CONSTRAINT pk_plan PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table plandefineallocation
-- ----------------------------
ALTER TABLE plandefineallocation ADD CONSTRAINT pk_modelofrole PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table plandefinetempstate
-- ----------------------------
ALTER TABLE plandefinetempstate ADD CONSTRAINT plandefinetempstate_pkey PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table planitem
-- ----------------------------
ALTER TABLE planitem ADD CONSTRAINT pk_planitem PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table planitemcustomization
-- ----------------------------
ALTER TABLE planitemcustomization ADD CONSTRAINT pk_customizedmodel PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table planitemmodel
-- ----------------------------
ALTER TABLE planitemmodel ADD CONSTRAINT pk_planmodel PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table workreport
-- ----------------------------
ALTER TABLE workreport ADD CONSTRAINT pk_workreport PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table wrmodel
-- ----------------------------
ALTER TABLE wrmodel ADD CONSTRAINT pk_wrmodel PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table wrmodelofrole
-- ----------------------------
ALTER TABLE wrmodelofrole ADD CONSTRAINT pk_wrtype PRIMARY KEY (id);

-- ----------------------------
-- Primary Key structure for table wrpictures
-- ----------------------------
ALTER TABLE wrpictures ADD CONSTRAINT pk_wrpictures PRIMARY KEY (id);
