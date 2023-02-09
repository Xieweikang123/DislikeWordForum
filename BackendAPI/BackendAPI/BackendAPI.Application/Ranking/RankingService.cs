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
    /// 排行接口
    /// </summary>
    /// 
    public class RankingService : IDynamicApiController
    {

        //[HttpPost]
        //public async Task<object> GetTodayRanking(PageInfoRanking dto)
        //{
        //    var db = DbContext.Instance;
        //    var today = DateTime.Now.Date;
        //    var list = await db.Queryable<EnglishWord, CoreUser>((e, c) => e.BelongUserId == c.Id)
        //       .GroupBy((e, c) => new { e.BelongUserId, c.Avatar })
        //       .Select((e, c) => new { Sum = SqlFunc.AggregateCount(e.BelongUserId), e.BelongUserId, c.Avatar })
        //       .OrderByDescending(e => SqlFunc.AggregateCount(e.BelongUserId))
        //       .ToPageListAsync(1, 50);

        //    return list;
        //}

        /// <summary>
        /// 今日背单词排行  
        /// 头像 人 昵称---单词数
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<object> GetTodayRanking(PageInfoRanking dto)
        {

            var db = DbContextStatic.Instance;
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