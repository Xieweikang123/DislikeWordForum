﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Application.DbManager
{
    public class DbConfigDTO : DBConfig
    {

        /// <summary>
        /// sql名字
        /// </summary>
        public string sqlName { get; set; }

        /// <summary>
        /// 要执行的sql
        /// </summary>
        public string sql { get; set; }
    }
}
