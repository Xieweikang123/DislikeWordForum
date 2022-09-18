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
        /// 消息
        /// </summary>
        public string Msg { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public object Data { get; set; }

        public static RetObj Success(object data,string msg="")
        {
            return new RetObj()
            {
                Msg = msg,
                Data=data
            };
        }

        //public static RetObj Error(string msg)
        //{
        //    return new RetObj()
        //    {
        //        Msg = msg
        //    };
        //}
    }
}
