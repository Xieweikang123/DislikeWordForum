using BackendAPI.Core;

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


            return null;

            //return _systemService.GetDescription();
        }
        
    }
}