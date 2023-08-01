using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///??
    ///</summary>
    [SugarTable("EnglishWord")]
    public partial class EnglishWord
    {
        public EnglishWord()
        {


        }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string id { get; set; }

        /// <summary>
        /// Desc:??
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string Word { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>        
        #nullable enable
        public string? Translate { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        public int RecordTimes { get; set; }
        /// <summary>
        /// 浏览次数
        /// </summary>
        public int? Views { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? Createdate { get; set; }


        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? Modifydate { get; set; }

        /// <summary>
        /// Desc:所属用户
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string BelongUserId { get; set; }



    }
}
