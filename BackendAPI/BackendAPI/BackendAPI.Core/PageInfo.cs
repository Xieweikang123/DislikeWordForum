using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core
{
    public class PageInfo
    {

        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalNumber { get; set; }



        /// <summary>
        /// 排序列
        /// </summary>
        public string prop { get; set; }
        /// <summary>
        /// 排序规则
        /// </summary>
        public string order { get; set; }


    }
}
