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

 Date: 12/12/2017 14:01:36
*/


-- ----------------------------
-- Table structure for carboncopyplanrecipients
-- ----------------------------
DROP TABLE IF EXISTS "carboncopyplanrecipients";
CREATE TABLE "carboncopyplanrecipients" (
  "id" varchar(36) COLLATE  NOT NULL,
  "recipientid" varchar(36) COLLATE ,
  "planid" varchar(36) COLLATE ,
  "tenantid" varchar(256) COLLATE 
)
;

-- ----------------------------
-- Table structure for carboncopywrrecipients
-- ----------------------------
DROP TABLE IF EXISTS "carboncopywrrecipients";
CREATE TABLE "carboncopywrrecipients" (
  "id" varchar(36) COLLATE  NOT NULL,
  "recipientid" varchar(36) COLLATE ,
  "wrid" varchar(36) COLLATE ,
  "tenantid" varchar(256) COLLATE 
)
;

-- ----------------------------
-- Table structure for customizedmodel
-- ----------------------------
DROP TABLE IF EXISTS "customizedmodel";
CREATE TABLE "customizedmodel" (
  "id" varchar(36) COLLATE  NOT NULL,
  "tenantid" varchar(36) COLLATE ,
  "modeldesc" varchar(1024) COLLATE ,
  "modelid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for plan
-- ----------------------------
DROP TABLE IF EXISTS "plan";
CREATE TABLE "plan" (
  "id" varchar(36) COLLATE  NOT NULL,
  "name" varchar(36) COLLATE ,
  "modelid" varchar(36) COLLATE ,
  "timespecid" varchar(36) COLLATE ,
  "mainrecipient" varchar(36) COLLATE ,
  "state" int4,
  "stage" int4,
  "tenantid" varchar(36) COLLATE ,
  "userid" varchar(36) COLLATE ,
  "createdtime" timestamp(0),
  "lastmodifiedtime" timestamp(0)
)
;

-- ----------------------------
-- Table structure for planitem
-- ----------------------------
DROP TABLE IF EXISTS "planitem";
CREATE TABLE "planitem" (
  "id" varchar(36) COLLATE  NOT NULL,
  "name" varchar(36) COLLATE ,
  "planid" varchar(36) COLLATE ,
  "starttime" timestamp(0),
  "endtime" timestamp(0),
  "itemorder" int4,
  "parentplanitemid" varchar(36) COLLATE ,
  "sourceplanitemid" varchar(36) COLLATE ,
  "planitemcontent" varchar(1024) COLLATE ,
  "createdtime" timestamp(0),
  "lastmodifiedtime" timestamp(0),
  "selfassessment" varchar(1024) COLLATE ,
  "assessmentofsuperior" varchar(1024) COLLATE ,
  "selfassessmentscore" varchar(36) COLLATE ,
  "assessmentofsuperiorscore" varchar(36) COLLATE ,
  "summarycontent" varchar(1024) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for planmodel
-- ----------------------------
DROP TABLE IF EXISTS "planmodel";
CREATE TABLE "planmodel" (
  "id" varchar(36) COLLATE  NOT NULL,
  "name" varchar(36) COLLATE ,
  "modelcontent" varchar(1024) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for planmodelofrole
-- ----------------------------
DROP TABLE IF EXISTS "planmodelofrole";
CREATE TABLE "planmodelofrole" (
  "id" varchar(36) COLLATE  NOT NULL,
  "roleid" varchar(36) COLLATE ,
  "planmodelid" varchar(36) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for timeallocation
-- ----------------------------
DROP TABLE IF EXISTS "timeallocation";
CREATE TABLE "timeallocation" (
  "id" varchar(36) COLLATE  NOT NULL,
  "tagid" varchar(36) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for timespec
-- ----------------------------
DROP TABLE IF EXISTS "timespec";
CREATE TABLE "timespec" (
  "id" varchar(36) COLLATE  NOT NULL,
  "name" varchar(128) COLLATE ,
  "parentid" varchar(128) COLLATE ,
  "starttime" timestamp(6),
  "endtime" timestamp(6),
  "type" varchar(36) COLLATE ,
  "tagid" varchar(36) COLLATE ,
  "tagname" varchar(128) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for workreport
-- ----------------------------
DROP TABLE IF EXISTS "workreport";
CREATE TABLE "workreport" (
  "id" varchar(36) COLLATE  NOT NULL,
  "content" varchar(2048) COLLATE ,
  "wrmodelid" varchar(36) COLLATE ,
  "dateoffilling" timestamp(0),
  "state" int4,
  "location" varchar(1024) COLLATE ,
  "mainrecipient" varchar(36) COLLATE ,
  "creator" varchar(36) COLLATE ,
  "createtime" timestamp(0),
  "lastmodifier" varchar(36) COLLATE ,
  "lastmodifiedtime" timestamp(0),
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for wrmodel
-- ----------------------------
DROP TABLE IF EXISTS "wrmodel";
CREATE TABLE "wrmodel" (
  "id" varchar(36) COLLATE  NOT NULL,
  "content" varchar(2048) COLLATE ,
  "name" varchar(256) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for wrmodelofrole
-- ----------------------------
DROP TABLE IF EXISTS "wrmodelofrole";
CREATE TABLE "wrmodelofrole" (
  "id" varchar(36) COLLATE  NOT NULL,
  "roleid" varchar(36) COLLATE ,
  "modelid" varchar(36) COLLATE ,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Table structure for wrpictures
-- ----------------------------
DROP TABLE IF EXISTS "wrpictures";
CREATE TABLE "wrpictures" (
  "id" varchar(36) COLLATE  NOT NULL,
  "workreportid" varchar(36) COLLATE ,
  "content" bytea,
  "tenantid" varchar(36) COLLATE 
)
;

-- ----------------------------
-- Primary Key structure for table carboncopyplanrecipients
-- ----------------------------
ALTER TABLE "carboncopyplanrecipients" ADD CONSTRAINT "pk_carboncopyplanrecipients" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table carboncopywrrecipients
-- ----------------------------
ALTER TABLE "carboncopywrrecipients" ADD CONSTRAINT "pk_carboncopywrrecipients" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table customizedmodel
-- ----------------------------
ALTER TABLE "customizedmodel" ADD CONSTRAINT "pk_customizedmodel" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table plan
-- ----------------------------
ALTER TABLE "plan" ADD CONSTRAINT "pk_plan" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table planitem
-- ----------------------------
ALTER TABLE "planitem" ADD CONSTRAINT "pk_planitem" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table planmodel
-- ----------------------------
ALTER TABLE "planmodel" ADD CONSTRAINT "pk_planmodel" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table planmodelofrole
-- ----------------------------
ALTER TABLE "planmodelofrole" ADD CONSTRAINT "pk_modelofrole" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table timeallocation
-- ----------------------------
ALTER TABLE "timeallocation" ADD CONSTRAINT "pk_timeallocation" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table timespec
-- ----------------------------
ALTER TABLE "timespec" ADD CONSTRAINT "pk_timespec" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table workreport
-- ----------------------------
ALTER TABLE "workreport" ADD CONSTRAINT "pk_workreport" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table wrmodel
-- ----------------------------
ALTER TABLE "wrmodel" ADD CONSTRAINT "pk_wrmodel" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table wrmodelofrole
-- ----------------------------
ALTER TABLE "wrmodelofrole" ADD CONSTRAINT "pk_wrtype" PRIMARY KEY ("id");

-- ----------------------------
-- Primary Key structure for table wrpictures
-- ----------------------------
ALTER TABLE "wrpictures" ADD CONSTRAINT "pk_wrpictures" PRIMARY KEY ("id");
