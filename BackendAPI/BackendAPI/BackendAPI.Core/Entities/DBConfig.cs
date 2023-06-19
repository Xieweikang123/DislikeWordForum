using System;
using System.Collections.Generic;
using System.Linq;
using SqlSugar;


namespace BackendAPI.Core.Entities
{
    public class DBConfig
    {
        /// <summary>
        ///  
        ///</summary>
        public int Id { get; set; }
        /// <summary>
        /// 用户id 
        ///</summary>
        public string UserId { get; set; }
        /// <summary>
        /// 创建时间 
        /// 默认值: (getdate())
        ///</summary>
        public string CreateTime { get; set; }
        /// <summary>
        /// 0正常  1删除  
        /// 默认值: ((0))
        ///</summary>
        public int Status { get; set; }
        /// <summary>
        /// 更新时间 
        ///</summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 数据库类型 mssql、mysql 
        ///</summary>
        public string DbType { get; set; }
        /// <summary>
        /// 数据库用户名 
        ///</summary>
        public string DbUserId { get; set; }
        /// <summary>
        /// 密码 
        ///</summary>
        public string DbPwd { get; set; }
        /// <summary>
        /// 服务器ip 
        ///</summary>
        public string DbServer { get; set; }
        /// <summary>
        /// 数据库名字 
        ///</summary>
        public string DbName { get; set; }
    }
}
