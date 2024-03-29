﻿using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("FlashContent")]
    public partial class FlashContent
    {
        public FlashContent()
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
        /// Desc:0正常  1删除 
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? status { get; set; }

        /// <summary>
        /// Desc:喜欢数
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int? likeCount { get; set; }

        /// <summary>
        /// Desc:点赞的人昵称，逗号分隔
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string likeNames { get; set; }
        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; }


    }
}
