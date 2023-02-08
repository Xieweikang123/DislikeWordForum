using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace BackendAPI.Web.Core.Helper
{

    /// <summary>
    /// 缓存
    /// </summary>
    public class LogHelper
    {
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLog(string text)
        {
            var db = DbContext.Instance;
            var logItem = new SysLog()
            {
                Id = IDGen.GetStrId(),
                Content = text,
                Createdate = DateTime.Now
            };

            db.Insertable(logItem).ExecuteCommandAsync();
        }
    }
}
