using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core.Entities
{
    public class User
    {
        /// <summary>
        ///  
        /// 默认值: 
        ///</summary>
        public int Id { get; set; }
        /// <summary>
        ///  
        /// 默认值: 
        ///</summary>
        public string Account { get; set; }
        /// <summary>
        ///  
        /// 默认值: 
        ///</summary>
        public string Password { get; set; }
    }
}
