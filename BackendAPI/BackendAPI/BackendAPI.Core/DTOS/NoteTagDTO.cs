using BackendAPI.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Core.Entities
{
    public class NoteTagDTO : NoteTag
    {
        /// <summary>
        /// 原标签名
        /// </summary>

        public string oriTagName { get; set; }
        /// <summary>
        /// 新标签名
        /// </summary>
        public string newTagName { get; set; }

    }
}
