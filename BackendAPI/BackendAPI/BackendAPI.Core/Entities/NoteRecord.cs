using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("NoteRecord")]
    public class NoteRecord
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string id { get; set; }

        /// <summary>
        /// Desc:发送内容
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string sayContent { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updateTime { get; set; }


        /// <summary>
        /// Desc:0正常  1删除 
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int status { get; set; }

        /// <summary>
        /// 笔记id
        /// </summary>
        public string NoteId { get; set; }

    }

}
