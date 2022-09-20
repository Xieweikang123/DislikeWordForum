using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;

namespace BackendAPI.Application
{
    /// <summary>
    /// 单词服务接口
    /// </summary>
    /// 
    public class WordAppService : IDynamicApiController
    {
        /// <summary>
        /// 单词缓存关键字
        /// </summary>
        private const string wordCacheKey = "word_Cache";
        private readonly IMemoryCache _memoryCache;
        public WordAppService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        //[ApiDescriptionSettings(KeepName = true)]
        //public DateTimeOffset GetOrCreate()
        //{
        //    return _memoryCache.GetOrCreate(wordCacheKey, entry =>
        //    {
        //        return DateTimeOffset.UtcNow;
        //    });
        //}

        [HttpPost]
        public async Task<object> GetMyWordList(PageInfo dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContext.Instance;
            //var whereExp = Expressionable.Create<EnglishWord>();
            ////当前用户
            //whereExp.And(x => x.BelongUserId == userId);

            //将单词查出来缓存
            var allEnglishWordList = _memoryCache.GetOrCreate(wordCacheKey, entry =>
             {
                 Console.WriteLine("查缓存");
                 entry.SlidingExpiration = TimeSpan.FromSeconds(5);
                 var list = db.Queryable<EnglishWord>().Where(x => x.BelongUserId == userId).ToList();
                 return list.OrderByDescending(x => x.RecordTimes);
             });

            var orderProp = "RecordTimes";
            var orderType = "desc";

            //指定了排序列
            if (!string.IsNullOrEmpty(dto.prop))
            {
                orderProp = dto.prop;
                switch (dto.order)
                {
                    case "descending":


                        break;
                    case "ascending":

                        break;
                }
            }
            //如果有搜索关键词
            //whereExp.AndIF(!string.IsNullOrEmpty(dto.searchContent), x => x.Word.Contains(dto.searchContent));
            Func<EnglishWord, bool> whereExp = x => true;
            if (!string.IsNullOrEmpty(dto.searchContent))
            {
                whereExp = x => x.Word.Contains(dto.searchContent) || (x.Translate??string.Empty).Contains(dto.searchContent);
            }
            var allCount = allEnglishWordList.Count();
            var pageList = allEnglishWordList.Where(whereExp).Skip((dto.pageNumber - 1) * dto.pageSize).Take(dto.pageSize);

            return new { pageList, allCount };
        }


    }
}