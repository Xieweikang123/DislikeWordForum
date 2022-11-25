using System;
using System.Linq;
using System.Text;
using SqlSugar;
namespace BackendAPI.Core.Entities
{
    /// <summary>
    /// 笔记标签表
    ///</summary>
    public class NoteTag
    {
        /// <summary>
        ///  
        ///</summary>
        [SugarColumn(IsPrimaryKey = true)]
        public string id { get; set; }
        /// <summary>
        /// 创建时间 
        ///</summary>
        public DateTime? createTime { get; set; }
        /// <summary>
        /// 0正常  1删除  
        ///</summary>
        public int status { get; set; }
        /// <summary>
        /// 用户id 
        ///</summary>
        public string userId { get; set; }
        /// <summary>
        /// 标签内容 
        ///</summary>
        public string tagName { get; set; }
        /// <summary>
        /// 笔记id
        ///</summary>
        public string noteId { get; set; }
        /// <summary>
        /// 更新时间 
        ///</summary>
        public DateTime? updateTime { get; set; }
    }
}
