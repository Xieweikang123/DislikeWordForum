using BackendAPI.Core;
using BackendAPI.Core.Entities;

namespace BackendAPI.Application
{
    /// <summary>
    /// 用户服务接口
    /// </summary>
    public class UserAppService : IDynamicApiController
    {




        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        public object Register(UserDTO dto)
        {

            var res = DbContext.Instance.Queryable<User>().ToList();
            return dto;

            //return _systemService.GetDescription();
        }

    }
}