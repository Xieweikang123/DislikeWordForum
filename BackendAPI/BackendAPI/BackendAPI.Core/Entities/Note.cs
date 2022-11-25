using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("Note")]
    public class Note
    {
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true)]
        public string id { get; set; }

        /// <summary>
        /// Desc:发送内容
        /// Default:
        /// Nullable:True
        /// </summary>           
        public string sayContent { get; set; }

        /// <summary>
        /// Desc:创建时间
        /// Default:
        /// Nullable:True
        /// </summary>           
        public DateTime? createTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? updateTime { get; set; }


        /// <summary>
        /// Desc:0正常  1删除 
        /// Default:
        /// Nullable:True
        /// </summary>           
        public int status { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; }


        [Navigate(NavigateType.OneToMany, nameof(NoteTag.noteId))]

        public List<NoteTag> noteTags { get; set; }



    }
}
