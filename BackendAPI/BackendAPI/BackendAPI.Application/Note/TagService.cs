using BackendAPI.Application.File;
using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.LinqBuilder;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BackendAPI.Application
{
    /// <summary>
    /// 标签服务接口
    /// </summary>
    /// 
    public class TagService : IDynamicApiController
    {

        /// <summary>
        /// 发表内容
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> ChangeTagName(NoteTagDTO dto)
        {
            if (string.IsNullOrEmpty(dto.oriTagName))
            {
                throw new Exception("请输入原标签");
            }
            if (string.IsNullOrEmpty(dto.newTagName))
            {
                throw new Exception("请输入新标签");
            }
            var db = DbContext.Instance;
            var userId = CurrentUserInfo.UserId;
            try
            {
                db.BeginTran();
                var nowTime = DateTime.Now;

                await db.Updateable<NoteTag>().SetColumns(x => new NoteTag() { tagName = dto.newTagName, updateTime = nowTime })
                    .Where(x => x.userId == userId && x.tagName == dto.oriTagName).ExecuteCommandAsync();

                db.CommitTran();
            }
            catch (Exception ex)
            {

                db.RollbackTran();
                throw new Exception(ex.Message);
            }
            return "ok";
        }






    }
}