using LogHelper = BackendAPI.Core.LogHelper;

namespace BackendAPI.Application
{
    /// <summary>
    /// 日志接口
    /// </summary>
    /// 
    public class LogInfoService : IDynamicApiController
    {
        private readonly ISqlSugarClient _dbContext;

        public LogInfoService(ISqlSugarClient dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        ///  记录日志
        /// </summary>
        /// <param name="sysLog"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task WriteLogAsync([FromBody] SysLog sysLog)
        {
            if (string.IsNullOrEmpty(sysLog.Content))
            {
                return;
            }
            await LogHelper.WriteLogAsync(sysLog.Content);
        }
    }
}