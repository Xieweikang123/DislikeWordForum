using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("EssayBrowse")]
    public partial class EssayBrowse
    {
           public EssayBrowse(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int EssayId {get;set;}

           /// <summary>
           /// Desc:浏览量
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int Count {get;set;}

           /// <summary>
           /// Desc:ip字符串 ;号隔开
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string IPList {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateTime {get;set;}

    }
}
