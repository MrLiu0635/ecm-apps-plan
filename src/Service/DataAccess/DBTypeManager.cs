using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Inspur.GSP.Gsf.DataAccess
{
    internal class DBTypeManager
    {
        //本来是用-1表示不存在，但Sybase非常变态，居然有-1表示的类型，所以改成-255表示不存在
        //private static int[,] Map = 
        //{
        //    //SQLServer,			    Oracle,						OracleSelf,	                    DB2		    OleDB,					    PostgreSQL ,    Sybase                          Unknown
        //    {(int)SqlDbType.Char,		(int)OracleType.Char,		(int)OracleDbType.Char,         -255,		(int)OleDbType.Char,	    -255,			(int)AseDbType.Char,			-255},//Char
        //    {(int)SqlDbType.VarChar,	(int)OracleType.VarChar,	(int)OracleDbType.Varchar2,     -255,		(int)OleDbType.VarChar,	    -255,			(int)AseDbType.VarChar,			-255},//VarChar
        //    {(int)SqlDbType.Int,		(int)OracleType.Int32,		(int)OracleDbType.Int32,        -255,		(int)OleDbType.Integer,	    -255,			(int)AseDbType.Integer,			-255},//Int
        //    {(int)SqlDbType.Decimal,	(int)OracleType.Number,		(int)OracleDbType.Decimal,      -255,		(int)OleDbType.Decimal,	    -255,			(int)AseDbType.Decimal,			-255},//Decimal
        //    {(int)SqlDbType.Image,	(int)OracleType.Blob,		(int)OracleDbType.Blob,         -255,		(int)OleDbType.Binary,	    -255,			(int)AseDbType.Image,	        -255},//Blob
        //    {(int)SqlDbType.DateTime,	(int)OracleType.Timestamp,	(int)OracleDbType.TimeStamp,    -255,		(int)OleDbType.Date,	    -255,			(int)AseDbType.Date,			-255}, //DateTime
        //    {-255,					(int)OracleType.Cursor,		(int)OracleDbType.RefCursor,    -255,		-255,					    -255,			-255,							-255}, //Cursor
        //    {(int)SqlDbType.Text,		(int)OracleType.Clob,		(int)OracleDbType.Clob,         -255,		(int)OleDbType.LongVarChar,	-255,			(int)AseDbType.Text,		    -255}  //Text		
        //};

        /// <summary>
        /// 
        /// </summary>
        private static int[,] Map =
        {
             //	PostgreSQL ,              MYSQL,			         SQLServer,						    Oracle,		              Sybase                        DB2                SQLServerCe                 SQLITE             MYSQL               Unknown
            {(int)NpgsqlDbType.Char,      (int)SqlDbType.Char,       (int)SqlDbType.Char,          (int)SqlDbType.Char,         (int)SqlDbType.Char,            -255,              (int)SqlDbType.NChar,      -255,         (int)SqlDbType.VarChar ,    -255},//Char
            
            {(int)NpgsqlDbType.Char,      (int)SqlDbType.NChar,      (int)SqlDbType.NChar,         (int)SqlDbType.NChar,        (int)SqlDbType.NChar,           -255,              -255,                      -255,         (int)SqlDbType.VarChar ,    -255}, //NChar
            
            {(int)NpgsqlDbType.Varchar,   (int)SqlDbType.VarChar,    (int)SqlDbType.VarChar,       (int)SqlDbType.VarChar,      (int)SqlDbType.VarChar,         -255,              (int)SqlDbType.NVarChar,   -255,         (int)SqlDbType.VarChar ,    -255},//VarChar
           
            {(int)NpgsqlDbType.Varchar,   (int)SqlDbType.Text,       (int)SqlDbType.Text,          (int)SqlDbType.Text,         (int)SqlDbType.NVarChar,        -255,              (int)SqlDbType.NText,      -255,         (int)SqlDbType.VarChar ,    -255},  //NVarChar	

            {(int)NpgsqlDbType.Integer,   (int)SqlDbType.Int,        (int)SqlDbType.Int,           (int)SqlDbType.Int,          (int)SqlDbType.Int,             -255,              (int)SqlDbType.Int,        -255,         (int)SqlDbType.Int,         -255},//Int
            
            {(int)NpgsqlDbType.Double,    (int)SqlDbType.Decimal,    (int)SqlDbType.Decimal,       (int)SqlDbType.Decimal,      (int)SqlDbType.Decimal,         -255,              (int)SqlDbType.Decimal,    -255,         (int)SqlDbType.Decimal ,    -255},//Decimal
            
            {(int)NpgsqlDbType.Timestamp, (int)SqlDbType.DateTime,   (int)SqlDbType.Timestamp,     (int)SqlDbType.Date,         (int)SqlDbType.Date,            -255,              (int)SqlDbType.DateTime,   -255,         (int)SqlDbType.DateTime,    -255}, //DateTime

            {(int)NpgsqlDbType.Bytea,     (int)SqlDbType.Image,      (int)SqlDbType.Image,         (int)SqlDbType.Binary,       (int)SqlDbType.Image,           -255,              (int)SqlDbType.Image,      -255,         (int)SqlDbType.Image,       -255},//Blob
            
            {(int)NpgsqlDbType.Text,      (int)SqlDbType.Text,       (int)SqlDbType.Text,          (int)SqlDbType.Text,         (int)SqlDbType.Text,            -255,              (int)SqlDbType.NText,      -255,         (int)SqlDbType.Text,        -255}, //Text		

            {(int)NpgsqlDbType.Refcursor, -255,                      -255,                         -255,                        -255,                           -255,              -255,                      -255,         -255,                       -255}, //Cursor
        };


        public static int GetDBType(GSPDbType dbType, GSPDbDataType commonType)
        {
            int type = Map[(int)commonType, (int)dbType];
            if (type < -254)
                throw new ArgumentException("没有" + commonType.ToString() + "对应的数据类型");
            return type;
        }
    }
}
