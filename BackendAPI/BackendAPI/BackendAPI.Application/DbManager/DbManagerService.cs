﻿using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.LinqBuilder;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace BackendAPI.Application
{
    /// <summary>
    /// 数据库服务接口
    /// </summary>
    public class DbManagerService : IDynamicApiController
    {

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public object TestConnectDb(DBConfig dto)
        {
            //var db = DbContextStatic.Instance;
            var userId = CurrentUserInfo.UserId;

            var connectStr = string.Empty;
            var dbType = DbType.SqlServer;
            switch (dto.DbType)
            {
                case "mssql":
                    connectStr = $"Server={dto.DbServer};Database={dto.DbName};User ID={dto.DbUserId};Password={dto.DbPwd};Trusted_Connection=False;Connection Timeout=3";
                    dbType = DbType.SqlServer;
                    break;
                default:
                    throw new Exception($"暂未支持数据库类型 {dto.DbType}");
            }

            var curDb = new SqlSugarScope(new ConnectionConfig()
            {
                ConnectionString = connectStr,//连接符字串
                DbType = dbType,//数据库类型
                IsAutoCloseConnection = true //不设成true要手动close
            }, db =>
                {
                    // 这里配置全局事件，比如拦截执行 SQL
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Console.WriteLine(sql);//输出sql
                    };
                });
            //sql超时
            //curDb.Ado.CommandTimeOut = 5;
            var isok = curDb.Ado.IsValidConnection(); //如果时间长，可以在连接字符串配置 连接超时时间

            return isok;
        }

    }
}