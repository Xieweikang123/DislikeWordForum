using Furion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace BackendAPI.Core
{

    //建一个扩展类
    public static class SqlsugarSetup
    {
        public static void AddSqlsugarSetup(this IServiceCollection services)
        {

            var configs = App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs");

            // 读取 appsettings.json 中的 ConnectionConfigs 配置节点
            SqlSugarScope sqlSugar = new(configs, db =>
             {
                 // 这里配置全局事件，比如拦截执行 SQL
                 db.Aop.OnLogExecuting = (sql, pars) =>
                 {
                     Console.WriteLine(sql);//输出sql
                 };

             });
            services.AddSingleton<ISqlSugarClient>(sqlSugar);//这边是SqlSugarScope用AddSingleton
        }
    }

    /// <summary>
    /// 数据库上下文对象
    /// </summary>
    public static class DbContextStatic
    {
        /// <summary>
        /// SqlSugar 数据库实例
        /// </summary>
        public static readonly SqlSugarScope Instance = new(
            // 读取 appsettings.json 中的 ConnectionConfigs 配置节点
            App.GetConfig<List<ConnectionConfig>>("ConnectionConfigs")
            , db =>
            {
                // 这里配置全局事件，比如拦截执行 SQL
                db.Aop.OnLogExecuting = (sql, pars) =>
                {
                    Console.WriteLine(sql);//输出sql
                };

            });
    }
}