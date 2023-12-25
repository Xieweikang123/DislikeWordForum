using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Application.User
{
    /// <summary>
    /// 当前用户信息读取
    /// </summary>
    public  class CurrentUserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public static string UserId => App.User.FindFirst("UserId")?.Value;

    }
}
