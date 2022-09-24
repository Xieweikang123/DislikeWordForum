using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core
{

    public enum SearchScope
    {
        全部 = 0,
        今日 = 1,
        昨天 = 2,
        最近3天 = 3,
        最近7天 = 4
    }
    /// <summary>
    /// 单词分页
    /// </summary>
    public class PageInfoWord : PageInfo
    {
        /// <summary>
        /// 搜索范围
        /// </summary>
        public SearchScope searchScop { get; set; }  

    }
}
