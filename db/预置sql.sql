Delete from modeltype where id='000250ee-33a1-4470-b56d-4bcbcb114f5d';
Insert into modeltype(id,code,name,tenantid) values('000250ee-33a1-4470-b56d-4bcbcb114f5d','label','文本','10000');
Delete from modeltype where id='000569ca-b80d-4ec9-beb1-652080d48a64';
Insert into modeltype(id,code,name,tenantid) values('000569ca-b80d-4ec9-beb1-652080d48a64','number','数字','10000');
Delete from modeltype where id='0043F97B-8E11-48A1-98B3-2A8780736C3A';
Insert into modeltype(id,code,name,tenantid) values('0043F97B-8E11-48A1-98B3-2A8780736C3A','date/time','日期/时间','10000');


Delete from wrtype where id='006a90d9-a6ff-46b4-81be-14070748aa20';
INSERT into wrtype(id,code,name,tenantid) values('006a90d9-a6ff-46b4-81be-14070748aa20','day','日报','10000');
Delete from wrtype where id='00747108-f574-4366-9937-c8ff8bfc78ed';
INSERT into wrtype(id,code,name,tenantid) values('00747108-f574-4366-9937-c8ff8bfc78ed','week','周报','10000');
Delete from wrtype where id='0084de6a-7e43-465a-ab7d-47ea39bc0f4f';
INSERT into wrtype(id,code,name,tenantid) values('0084de6a-7e43-465a-ab7d-47ea39bc0f4f','month','月报','10000');


Delete from wrcomponentmodel where id='00b8d3df-4cdc-4bb8-85eb-e0c8abcdd1a8';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('00b8d3df-4cdc-4bb8-85eb-e0c8abcdd1a8','今日完成工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','0','006a90d9-a6ff-46b4-81be-14070748aa20','10000');
Delete from wrcomponentmodel where id='00c7a5d7-7cfc-4915-90df-0ab6225910b7';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('00c7a5d7-7cfc-4915-90df-0ab6225910b7','今日未完成工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','1','006a90d9-a6ff-46b4-81be-14070748aa20','10000');
Delete from wrcomponentmodel where id='00dc0469-c7c5-4826-95f4-8db8ff6c24ae';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('00dc0469-c7c5-4826-95f4-8db8ff6c24ae','需协调工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','2','006a90d9-a6ff-46b4-81be-14070748aa20','10000');
Delete from wrcomponentmodel where id='00e3248a-025d-4e20-a89d-27f2c4e0b249';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('00e3248a-025d-4e20-a89d-27f2c4e0b249','备注','000250ee-33a1-4470-b56d-4bcbcb114f5d','3','006a90d9-a6ff-46b4-81be-14070748aa20','10000');
Delete from wrcomponentmodel where id='00F87D9F-BA8F-4C10-BBF5-6DADBDAF5017';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('00F87D9F-BA8F-4C10-BBF5-6DADBDAF5017','本周完成工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','0','00747108-f574-4366-9937-c8ff8bfc78ed','10000');
Delete from wrcomponentmodel where id='01036883-0997-4C0B-9B8C-C6C0185ED45D';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('01036883-0997-4C0B-9B8C-C6C0185ED45D','本周工作总结','000250ee-33a1-4470-b56d-4bcbcb114f5d','1','00747108-f574-4366-9937-c8ff8bfc78ed','10000');
Delete from wrcomponentmodel where id='011e9929-9a0c-4dbf-a6be-b4a862cfed92';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('011e9929-9a0c-4dbf-a6be-b4a862cfed92','下周计划工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','2','00747108-f574-4366-9937-c8ff8bfc78ed','10000');
Delete from wrcomponentmodel where id='0125BF30-B7F5-4BCF-A588-63C9A45DCDF7';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('0125BF30-B7F5-4BCF-A588-63C9A45DCDF7','需协调与帮助','000250ee-33a1-4470-b56d-4bcbcb114f5d','3','00747108-f574-4366-9937-c8ff8bfc78ed','10000');
Delete from wrcomponentmodel where id='013caa34-42e4-41a0-a88f-e0f44717edb6';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('013caa34-42e4-41a0-a88f-e0f44717edb6','备注','000250ee-33a1-4470-b56d-4bcbcb114f5d','4','00747108-f574-4366-9937-c8ff8bfc78ed','10000');
Delete from wrcomponentmodel where id='0142965b-e032-4a45-b36d-eb7af8c5ec5f';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('0142965b-e032-4a45-b36d-eb7af8c5ec5f','本月工作内容','000250ee-33a1-4470-b56d-4bcbcb114f5d','0','0084de6a-7e43-465a-ab7d-47ea39bc0f4f','10000');
Delete from wrcomponentmodel where id='0157eed7-7d8f-4c17-ba9f-3094310941f1';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('0157eed7-7d8f-4c17-ba9f-3094310941f1','本月工作总结','000250ee-33a1-4470-b56d-4bcbcb114f5d','1','0084de6a-7e43-465a-ab7d-47ea39bc0f4f','10000');
Delete from wrcomponentmodel where id='01607e84-e54d-4206-8bed-57c6bf8cc7df';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('01607e84-e54d-4206-8bed-57c6bf8cc7df','下月计划工作','000250ee-33a1-4470-b56d-4bcbcb114f5d','2','0084de6a-7e43-465a-ab7d-47ea39bc0f4f','10000');
Delete from wrcomponentmodel where id='01765D4C-74F9-49D1-BBB9-166759ED3427';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('01765D4C-74F9-49D1-BBB9-166759ED3427','需帮助与支持','000250ee-33a1-4470-b56d-4bcbcb114f5d','3','0084de6a-7e43-465a-ab7d-47ea39bc0f4f','10000');
Delete from wrcomponentmodel where id='018dd79c-1c97-4b4c-b16e-2b5a8950f960';
Insert into wrcomponentmodel(id,name,modeltypeid,modelorder,wrtypeid,tenantid) values('018dd79c-1c97-4b4c-b16e-2b5a8950f960','备注','000250ee-33a1-4470-b56d-4bcbcb114f5d','4','0084de6a-7e43-465a-ab7d-47ea39bc0f4f','10000');
