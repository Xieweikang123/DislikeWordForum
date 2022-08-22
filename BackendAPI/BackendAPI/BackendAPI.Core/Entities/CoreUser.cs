using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    public class CoreUser
    {
        /// <summary>
        ///  
        ///</summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户名 
        ///</summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码 
        ///</summary>
        public string Password { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public byte? UserSex { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? UserBirthday { get; set; }
        /// <summary>
        /// 邮箱 
        ///</summary>
        public string UserEmail { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public short? UserLocked { get; set; }
        /// <summary>
        /// 1:激活 
        ///</summary>
        public short? UserStatus { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? Createdate { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public string Createuser { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? Modifydate { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public string Modifyuser { get; set; }
        /// <summary>
        ///  
        ///</summary>
        public DateTime? LastLoginTime { get; set; }
        /// <summary>
        /// token 
        ///</summary>
        public string Token { get; set; }
    }
}
