using BackendAPI.Core.Entities.Base;
using Furion;
using Furion.DistributedIDGenerator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        /// 扩展保存方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="db"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async static Task<object> SaveAsync<T>(this ISqlSugarClient db, T dto) where T : BaseEntity, new()
        {
            var nowTime = DateTime.Now;
            //新增
            if (string.IsNullOrEmpty(dto.Id))
            {
                dto.CreateTime = nowTime;
                dto.Id = IDGen.GetStrId();
                db.Insertable(dto).ExecuteCommand();
                return new { msg = "新增成功", id = dto.Id };
            }
            else
            {
                //修改
                dto.UpdateTime = nowTime;
                await db.Updateable(dto).ExecuteCommandAsync();
                return new { msg = "修改成功", id = dto.Id };
                //return "修改成功";
            }
        }
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