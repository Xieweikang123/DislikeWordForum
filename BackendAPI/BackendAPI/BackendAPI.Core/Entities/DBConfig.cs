using System;
using System.Collections.Generic;
using System.Linq;
using BackendAPI.Core.Entities.Base;
using SqlSugar;


namespace BackendAPI.Core.Entities
{
    public class DBConfig: BaseEntity
    {
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
        /// <summary>
        /// 连接名
        /// </summary>
        public string connectName { get; set; }
    }
}
