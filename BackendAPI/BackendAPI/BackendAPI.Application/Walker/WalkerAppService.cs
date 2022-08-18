using BackendAPI.Core;
using BackendAPI.Core.Entities;

namespace BackendAPI.Application
{
    /// <summary>
    /// 系统服务接口
    /// </summary>
    public class WalkerAppService : IDynamicApiController
    {
        //private readonly IWalkerService _systemService;
        //public WalkerAppService(IWalkerService systemService)
        //{
        //    _systemService = systemService;
        //}

        /// <summary>
        /// 获取系统描述
        /// </summary>
        /// <returns></returns>
        public object GetDescription()
        {

            var res = DbContext.Instance.Queryable<User>().ToList();
            return res;

            //return _systemService.GetDescription();
        }
        
    }
}