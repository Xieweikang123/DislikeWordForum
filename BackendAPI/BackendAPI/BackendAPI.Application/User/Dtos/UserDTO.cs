using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BackendAPI.Application
{
    public class UserDTO : User
    {


        /// <summary>
        /// 再次输入密码
        ///</summary>
        public string PasswordAgain { get; set; }

    }
}
