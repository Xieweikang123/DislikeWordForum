using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core.Entities.Base
{
    public class BaseEntity
    {

        public BaseEntity()
        {

        }
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 用户id 
        ///</summary>
        public string UserId { get; set; }
        /// <summary>
        /// 创建时间 
        /// 默认值: (getdate())
        ///</summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 0正常  1删除  
        /// 默认值: ((0))
        ///</summary>
        public int Status { get; set; }
        /// <summary>
        /// 更新时间 
        ///</summary>
        public DateTime? UpdateTime { get; set; }
    }
}
