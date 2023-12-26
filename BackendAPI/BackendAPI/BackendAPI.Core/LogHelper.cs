using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BackendAPI.Core
{

    /// <summary>
    /// 日志记录
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text"></param>
        public static async Task WriteLogAsync(string text)
        {
            var db = DbContextStatic.Instance;
            var logItem = new SysLog()
            {
                Id = IDGen.GetStrId(),
                Content = text,
                Createdate = DateTime.Now
            };

            await db.Insertable(logItem).ExecuteCommandAsync();
        }
    }
}
