using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("DBConfig")]
    public partial class DBConfig
    {
           public DBConfig(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:用户id
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int UserId {get;set;}

           /// <summary>
           /// Desc:连接字符串
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string ConnectionStr {get;set;}

    }
}
