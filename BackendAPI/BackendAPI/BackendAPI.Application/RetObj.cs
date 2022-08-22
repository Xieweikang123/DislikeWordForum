using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Application
{
    /// <summary>
    /// 返回对象
    /// </summary>
    public class RetObj
    {
        /// <summary>
        /// 返回码  200  
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        public static RetObj Error(string msg)
        {
            return new RetObj()
            {
                Code = 500,
                Msg = msg
            };
        }
    }
}
