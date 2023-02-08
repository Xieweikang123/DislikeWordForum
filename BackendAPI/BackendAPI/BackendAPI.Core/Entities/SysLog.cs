using System;
using System.Collections.Generic;
using System.Linq;
namespace BackendAPI.Core.Entities
{
    /// <summary>
    /// 系统日志
    ///</summary>
    public class SysLog
    {
        /// <summary>
        ///  
        ///</summary>
        public string Id { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
        public DateTime? Createdate { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? Modifydate { get; set; }
        /// <summary>
        /// 日志内容 
        ///</summary>
        public string Content { get; set; }
    }
}
