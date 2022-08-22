using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("UserNotice")]
    public partial class UserNotice
    {
           public UserNotice(){


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
           public int UserId {get;set;}

           /// <summary>
           /// Desc:消息内容
           /// Default:
           /// Nullable:True
           /// </summary>           
           public byte[] Msg {get;set;}

           /// <summary>
           /// Desc:留言文章id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? EssayId {get;set;}

           /// <summary>
           /// Desc:留言id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? LeavingMsgId {get;set;}

           /// <summary>
           /// Desc:触发消息的用户id
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? FromUserId {get;set;}

           /// <summary>
           /// Desc:是否已读 0否 1是
           /// Default:
           /// Nullable:True
           /// </summary>           
           public byte? IsRead {get;set;}

           /// <summary>
           /// Desc:0:回复留言  1:点赞  2 回复楼主
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? Type {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? UpdateTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? UpdateUserId {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public DateTime? CreateTime {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           public int? CreateUserId {get;set;}

    }
}
