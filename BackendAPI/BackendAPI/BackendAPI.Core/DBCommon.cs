using BackendAPI.Core.Entities;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace BackendAPI.Core
{


    /// <summary>
    /// 数据库公共类
    /// </summary>
    public class DBCommon
    {


        public static List<string> SysDbNameList = new List<string>()
        {
            "master",
            "model",
            "msdb",
            "tempdb",
        };
        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dbTypeStr"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DbType GetDbType(string dbTypeStr)
        {
            switch (dbTypeStr)
            {
                case "mssql":
                    return DbType.SqlServer;
                default:
                    throw new Exception($"暂未支持数据库类型 {dbTypeStr}");
            }
        }

        public static string GetDBConnStr(DBConfig dto)
        {
            var dbType = GetDbType(dto.DbType);
            return GetDBConnStr(dto, dbType);
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="dbType">数据库类型</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetDBConnStr(DBConfig dto, DbType dbType)
        {
            switch (dbType)
            {
                case DbType.SqlServer:
                    return $"Server={dto.DbServer};Database={dto.DbName};User ID={dto.DbUserId};Password={dto.DbPwd};Trusted_Connection=False;Connection Timeout=3";
                default:
                    throw new Exception($"暂未支持数据库类型 {dto.DbType}");
            }
        }
    }
}
