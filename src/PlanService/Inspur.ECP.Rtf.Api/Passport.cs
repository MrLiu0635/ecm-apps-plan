using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Inspur.ECP.Rtf.Api
{
    public class Passport
    {

        public string ID { get; set; }

        public string UserID { get; set; }

        /// <summary>
        /// 护照号
        /// </summary>
        public string Number { get; set; }

        public Stream Picture { get; set; }

        public DateTime EffectDate { get; set; }



        /// <summary>
        /// 护照类型，默认是 1 表示普通护照，2表示 公务护照
        /// </summary>
        public int Type { get; set; }

    }
}
