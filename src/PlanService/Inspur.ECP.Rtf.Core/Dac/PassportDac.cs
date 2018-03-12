using Inspur.ECP.Rtf.Api;
using Inspur.ECP.Rtf.Common;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Core
{
    internal class PassportDac
    {
        private static PGDatabase Database
        {
            get
            {
                return PGDatabase.GetDatabase("sysdb");
            }
        }
        #region 添加

        public static void AddPassport(Passport passport)
        {
            string sql = "insert into user_passport (user_id, number, picture, effect_date, type,state) values (@user_id, @number, @picture, @effect_date, @type,'1')";

            NpgsqlParameter userid = new NpgsqlParameter("@user_id", NpgsqlDbType.Varchar, 50);
            userid.Value = passport.UserID;
            NpgsqlParameter number = new NpgsqlParameter("@number", NpgsqlDbType.Varchar, 50);
            number.Value = passport.Number;

            NpgsqlParameter picture = new NpgsqlParameter("@picture", NpgsqlDbType.Bytea);
            BinaryReader br = new BinaryReader(passport.Picture);
            Byte[] bytes = br.ReadBytes((Int32)passport.Picture.Length);
            picture.Value = bytes;


            NpgsqlParameter effect_date = new NpgsqlParameter("@effect_date", NpgsqlDbType.Date);
            effect_date.Value = passport.EffectDate;

            NpgsqlParameter type = new NpgsqlParameter("@type", NpgsqlDbType.Integer, 1);
            type.Value = Convert.ToChar(passport.Type);

            Database.ExcuteNonQuery(sql, userid, number, picture, effect_date, type);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="date"></param>
        /// <param name="pic"></param>
        /// <param name="userId"></param>
        /// <param name="type"></param>
        public static void UpdatePassport(string userId, int type, string newNumber = null, DateTime newDate = default(DateTime), Stream newPic = null)
        {
            if (newNumber == null && newDate == null && newPic == null)
            {
                return;
            }
            List<NpgsqlParameter> objs = new List<NpgsqlParameter>();
            string sql = "update user_passport set number=@number,effect_date=@effect_date,picture=@picture  where user_id=@user_id and type=@type";

            if (newNumber == null)
            {
                sql= sql.Replace("number=@number,", string.Empty);
            }
            else
            {
                NpgsqlParameter number = new NpgsqlParameter("@number", NpgsqlDbType.Varchar, 50);
                number.Value = newNumber;
                objs.Add(number);
            }

            if (newDate == default(DateTime))
            {
                sql= sql.Replace("effect_date=@effect_date,", string.Empty);
            }
            else
            {
                NpgsqlParameter effect_date = new NpgsqlParameter("@effect_date", NpgsqlDbType.Date);
                effect_date.Value = newDate;
                objs.Add(effect_date);
            }

            if (newPic == null)
            {
                sql= sql.Replace(",picture=@picture", string.Empty);
            }
            else
            {
                NpgsqlParameter picture = new NpgsqlParameter("@picture", NpgsqlDbType.Bytea);
                BinaryReader br = new BinaryReader(newPic);
                Byte[] bytes = br.ReadBytes((Int32)newPic.Length);
                picture.Value = bytes;
                objs.Add(picture);
            }

            NpgsqlParameter _userid = new NpgsqlParameter("@user_id", NpgsqlDbType.Varchar, 50);
            _userid.Value = userId;
            objs.Add(_userid);
            NpgsqlParameter _type = new NpgsqlParameter("@type", DbType.Byte, 1);
            _type.Value = type;
            objs.Add(_type);

            Database.ExcuteNonQuery(sql, objs);

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pic"></param>
        /// <param name="type">1 是普通护照，2 是公务护照</param>
        public static void UpdatePicture(string userId, Stream pic, int type = 1)
        {
            string sql = "update user_passport set picture=@picture  where user_id=@userId and type=@type";

            NpgsqlParameter userid = new NpgsqlParameter("@user_id", NpgsqlDbType.Varchar, 50);
            userid.Value = userId;

            NpgsqlParameter picture = new NpgsqlParameter("@picture", NpgsqlDbType.Bytea);
            BinaryReader br = new BinaryReader(pic);
            Byte[] bytes = br.ReadBytes((Int32)pic.Length);
            picture.Value = bytes;

            NpgsqlParameter ptype = new NpgsqlParameter("@type", DbType.Byte, 1);
            ptype.Value = type;

            Database.ExcuteNonQuery(sql, picture, userid, type);

        }

        public static void UpdatePassportNumber(string userId, string newNumber, int type = 1)
        {
            string sql = "update user_passport set number={0}  where user_id={1} and type={2}";

            Database.ExcuteNonQuery(sql, newNumber, userId, type);

        }

        #endregion


        #region 查询
        public static string GetPassportNmb(string userId)
        {
            string sql = @"select number from  user_passport where user_id={0}";
            string nmb = string.Empty;
            using (IDataReader reader = Database.ExcuteDataReader(sql, userId))
            {
                if (reader.Read())
                {
                    nmb = reader.GetString(0);
                }
            }
            return nmb;
        }

        /// <summary>
        /// 根据用户ID获取护照信息
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static Passport GetPassport(string userId)
        {
            string sql = @"select id,user_id, number, picture, effect_date, type from  user_passport where user_id={0}";
            Passport ppt = default(Passport);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, userId))
            {
                if (reader.Read())
                {
                    ppt = new Passport()
                    {
                        ID = reader.GetString(0),
                        UserID = reader.GetString(1),
                        Number = reader.GetString(2),
                        Picture = ToStream(reader.GetValue(3)),
                        EffectDate = reader.GetDateTime(4),
                        Type = reader.GetInt32(5)
                    };
                }
            }

            return ppt;
        }

        public static Stream GetPassportPic(string userId)
        {
            string sql = @"select picture from  user_passport where user_id={0}";
            Stream sm = default(Stream);
            PGDatabase db = PGDatabase.GetDatabase("sysdb");
            using (IDataReader reader = db.ExcuteDataReader(sql, userId))
            {
                if (reader.Read())
                {
                    object obj = reader.GetValue(0);
                    sm = ToStream(obj);
                }
            }
            return sm;
        }


        private static Stream ToStream(object obj)
        {
            Stream sm = default(Stream);
            byte[] b = default(byte[]);
            if (obj != null)
            {
                b = obj as byte[];
            }
            if (b != null)
            {
                sm = new MemoryStream(b);
            }

            return sm;
        }

        #endregion

    }
}
