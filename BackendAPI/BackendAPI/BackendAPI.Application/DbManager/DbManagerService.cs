﻿using BackendAPI.Application.DbManager;
using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.JsonSerialization;
using Furion.LinqBuilder;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SqlSugar;
using StackExchange.Profiling.Internal;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using static Microsoft.AspNetCore.Razor.Language.TagHelperMetadata;

namespace BackendAPI.Application
{
    /// <summary>
    /// 数据库服务接口
    /// </summary>
    public class DbManagerService : IDynamicApiController
    {

        /// <summary>
        /// 获取我的sql配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetMyDbSql()
        {
            var db = DbContextStatic.Instance;
            var list = await db.Queryable<DBSql>().Where(x => x.Status == 0 && x.UserId == CurrentUserInfo.UserId).OrderByDescending(t => t.UpdateTime).ToListAsync();

            return list;
        }

        /// <summary>
        /// 保存sql
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<object> SaveSql(DBSql dto)
        {
            var db = DbContextStatic.Instance;
            dto.UserId = CurrentUserInfo.UserId;
            return await db.SaveAsync(dto);
        }
        /// <summary>
        /// 保存一条数据库配置
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Save(DBConfig dto)
        {

            var db = DbContextStatic.Instance;
            dto.UserId = CurrentUserInfo.UserId;
            return await db.SaveAsync(dto);
        }
        /// <summary>
        /// 执行sql
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> ExecuteSql(DbConfigDTO dto)
        {
            var configDb = GetSqlSugarClient(dto.Id, dto.DbName);
            var dt = configDb.Ado.GetDataTable(dto.sql); //上面列表中 SqlQuery 等方法都可以不一定是GetDataTable
            var json = JsonConvert.SerializeObject(dt); // this is the line causing issue
            var columns = JsonConvert.SerializeObject(dt.Columns);

            return new { json, columns };
        }

        /// <summary>
        /// 获取表分页数据
        /// </summary>
        /// <param name="pageInfo"></param>
        /// <param name="dbConfig"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetTableDataList(PageInfo pageInfo)
        {
            var dbConfig = new DBConfig();
            RefAsync<int> totalNumber = 0;
            //动态表名 ，动态条件 
            //var list = await configDb.Queryable<dynamic>().AS(tableName).Where("id=@id", new { id = 1 }).ToPageListAsync(pageInfo.pageNumber, pageInfo.pageSize, totalNumber);//没实体一样用
            var tableName = string.Empty;
            var dbConfigId = string.Empty;
            var dbName = string.Empty;
            pageInfo.searchKeyValues.ForEach(item =>
            {
                switch (item.key)
                {
                    case "tableName":
                        tableName = item.value;
                        break;
                    case "dbConfigId":
                        dbConfigId = item.value;
                        break;
                    case "dbName":
                        dbName = item.value;
                        break;
                }
            });
            var configDb = GetSqlSugarClient(dbConfigId, dbName);
            var list = await configDb.Queryable<dynamic>().AS(tableName).ToPageListAsync(pageInfo.pageNumber, pageInfo.pageSize, totalNumber);//没实体一样用

            var allColumns = configDb.DbMaintenance.GetColumnInfosByTableName(tableName);

            return new { list, totalNumber, allColumns };
        }


        /// <summary>
        /// 获取数据库对应表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetCurTables(DBConfig dto)
        {
            var configDb = GetSqlSugarClient(dto.Id, dto.DbName);
            //获取所有数据库名称
            var list1 = configDb.DbMaintenance.GetTableInfoList(false);
            return list1;
        }

        /// <summary>
        /// 获取对应数据库
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetCurDbs(DBConfig dto)
        {
            var configDb = GetSqlSugarClient(dto.Id);
            //获取所有数据库名称
            var list1 = configDb.DbMaintenance.GetDataBaseList().Where(x => !DBCommon.SysDbNameList.Contains(x));
            return list1;
        }

        /// <summary>
        /// 获取配置db
        /// </summary>
        /// <param name="dbConfigId"></param>
        /// <param name="dbName">指定数据库名字</param>
        /// <returns></returns>
        private SqlSugarClient GetSqlSugarClient(string dbConfigId, string dbName = "")
        {
            var db = DbContextStatic.Instance;
            var curDbConfig = db.Queryable<DBConfig>().First(x => x.Id == dbConfigId);
            if (!string.IsNullOrEmpty(dbName))
            {
                curDbConfig.DbName = dbName;
            }
            ConnectionConfig connectionConfig = new ConnectionConfig()
            {
                ConnectionString = DBCommon.GetDBConnStr(curDbConfig),
                DbType = DBCommon.GetDbType(curDbConfig.DbType)

            };
            return new SqlSugarClient(connectionConfig);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> DelById(DBConfig dto)
        {
            var db = DbContextStatic.Instance;
            await db.Updateable<DBConfig>().SetColumns(x => new DBConfig() { UpdateTime = DateTime.Now, Status = 1 }).Where(x => x.Id == dto.Id).ExecuteCommandAsync();
            return "ok";
        }

        /// <summary>
        /// 获取我的数据库配置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetMyDbConfigs()
        {
            var db = DbContextStatic.Instance;
            var list = await db.Queryable<DBConfig>().Where(x => x.Status == 0 && x.UserId == CurrentUserInfo.UserId).ToListAsync();
            return list;
        }



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

            var dbType = DBCommon.GetDbType(dto.DbType);
            var connectStr = DBCommon.GetDBConnStr(dto, dbType);
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