using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace BackendAPI.Core.Entities
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("VerifyInfo")]
    public partial class VerifyInfo
    {
           public VerifyInfo(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true)]
           public int Id {get;set;}

           /// <summary>
           /// Desc:guid 用来激活邮箱
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string VerifyCode {get;set;}

           /// <summary>
           /// Desc:发送至邮箱
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string ToEmail {get;set;}

           /// <summary>
           /// Desc:验证类型:VerifyEmail 激活邮箱; ForgetPsw:找回密码
           /// Default:
           /// Nullable:True
           /// </summary>           
           public string Type {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public int UserId {get;set;}

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
           public DateTime? CreateTime {get;set;}

    }
}
