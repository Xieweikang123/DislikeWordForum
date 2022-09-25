using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.LinqBuilder;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
namespace BackendAPI.Application
{
    /// <summary>
    /// 闪念服务接口
    /// </summary>
    /// 
    public class FlashContentService : IDynamicApiController
    {

        /// <summary>
        /// 获取闪念分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost]
        public async Task<object> GetContentList(PageInfo dto)
        {
            var db = DbContext.Instance;

            RefAsync<int> totalNumber = 0;

            var list = await db.Queryable<FlashContent, CoreUser>((f, c) => f.userId == c.Id).Where(f => (f.status ?? 0) != 1).OrderByDescending(f => f.id).Select((f, c) => new
            {
                f.id,
                f.userId,
                f.sayContent,
                f.createTime,
                c.NickName,
                c.Avatar
            }).ToPageListAsync(dto.pageNumber, dto.pageSize, totalNumber);

            return new { list, totalNumber };
        }

        /// <summary>
        /// 删除一条闪念
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<object> DelAContent(FlashContent dto)
        {
            var db = DbContext.Instance;
            var userId = CurrentUserInfo.UserId;

            //改闪念是否为当前人的
            var findItem = db.Queryable<FlashContent>().First(x => x.id == dto.id);
            if (findItem.userId != userId)
            {
                throw new Exception("没有权限删除");
            }
            findItem.status = 1;
            await db.Updateable(dto).ExecuteCommandAsync();
            return "删除成功";
        }


        /// <summary>
        /// 发送一个闪念
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> SendAContent(FlashContent dto)
        {
            var db = DbContext.Instance;
            var userId = CurrentUserInfo.UserId;

            if (string.IsNullOrWhiteSpace(dto.sayContent))
            {
                throw new Exception("请输入发表内容");
            }

            dto.id = IDGen.NextID().ToString();
            dto.userId = userId;
            dto.createTime = DateTime.Now;

            await db.Insertable(dto).ExecuteCommandAsync();
            return "ok";
        }

        /// <summary>
        /// 今日背单词排行  
        /// 头像 人 昵称---单词数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> GetTodayRanking(PageInfo dto)
        {

            var db = DbContext.Instance;
            var today = DateTime.Now.Date;
            var list = await db.Queryable<EnglishWord, CoreUser>((e, c) => e.BelongUserId == c.Id)
               .Where(e => e.Createdate >= today || e.Modifydate >= today)

               .GroupBy((e, c) => new { e.BelongUserId, c.NickName, c.Avatar })
               .Select((e, c) => new { Sum = SqlFunc.AggregateCount(e.BelongUserId), c.NickName, e.BelongUserId, c.Avatar })
               .OrderByDescending(e => SqlFunc.AggregateCount(e.BelongUserId))
               .ToPageListAsync(1, 50);

            return list;
        }






    }
}