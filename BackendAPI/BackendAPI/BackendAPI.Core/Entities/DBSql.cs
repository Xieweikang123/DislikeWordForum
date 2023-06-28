using System;
using System.Collections.Generic;
using System.Linq;
using BackendAPI.Core.Entities.Base;
using SqlSugar;


namespace BackendAPI.Core.Entities
{
    public class DBSql: BaseEntity
    {
        /// <summary>
        /// sql命名
        /// </summary>
        public string sqlName { get; set; }
        /// <summary>
        /// sql内容
        /// </summary>
        public string sql { get; set; }
    }
}
