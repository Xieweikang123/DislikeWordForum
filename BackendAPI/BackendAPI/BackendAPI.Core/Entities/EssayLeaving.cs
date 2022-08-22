using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("EssayLeaving")]
    public partial class EssayLeaving
    {
           public EssayLeaving(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:文章id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? EssayId {get;set;}

           /// <summary>
           /// Desc:回复的留言id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? ReplyLeavingId {get;set;}

           /// <summary>
           /// Desc:回复给的用户id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? ReplyUserId {get;set;}

           /// <summary>
           /// Desc:留言内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Content {get;set;}

           /// <summary>
           /// Desc:用户id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? UserId {get;set;}

           /// <summary>
           /// Desc:0:正常  1 删除
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Status {get;set;}

           /// <summary>
           /// Desc:创建时间
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateTime {get;set;}

    }
}
