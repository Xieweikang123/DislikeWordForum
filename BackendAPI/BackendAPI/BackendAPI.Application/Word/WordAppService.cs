﻿using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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


        /// <summary>
        /// 记录单词
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> RecordWord(PageInfo dto)
        {

            var userId = CurrentUserInfo.UserId;
            var db = DbContext.Instance;

            return null;
        }

        /// <summary>
        /// 查询缓存单词
        /// </summary>
        /// <returns></returns>
        public List<EnglishWord> GetCacheEnglishWords()
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContext.Instance;
            //将单词查出来缓存
            var wordlist = _memoryCache.GetOrCreate(wordCacheKey, entry =>
            {
                Console.WriteLine("查缓存");
                entry.SlidingExpiration = TimeSpan.FromSeconds(5);
                var list = db.Queryable<EnglishWord>().Where(x => x.BelongUserId == userId).ToList();
                return list;
            });

            return wordlist;
        }


        [HttpPost]
        public async Task<object> GetMyWordList(PageInfo dto)
        {
            var allEnglishWordList = GetCacheEnglishWords();
            //var orderProp = "RecordTimes";

            //var engli = new EnglishWord();
            //var ss = engli.GetType().GetProperty("word", BindingFlags.IgnoreCase);
            //var ss2 = engli.GetType().GetProperty("Word", BindingFlags.IgnoreCase);

            //指定了排序列
            if (!string.IsNullOrEmpty(dto.prop))
            {
                //BindingFlags flag = BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance;
                //var propertyInfo = typeof(EnglishWord).GetProperty(dto.prop, flag);

                //orderProp = dto.prop;
                var isDesc = true;
                switch (dto.order)
                {
                    case "descending":
                        isDesc = true;
                        break;
                    case "ascending":
                        isDesc = false;
                        break;
                }
                allEnglishWordList = allEnglishWordList.AsQueryable().OrderBy(dto.prop, isDesc).ToList();
            }
            else
            {
                allEnglishWordList = allEnglishWordList.OrderByDescending(x => x.RecordTimes).ToList();
            }
            //如果有搜索关键词
            //whereExp.AndIF(!string.IsNullOrEmpty(dto.searchContent), x => x.Word.Contains(dto.searchContent));
            Func<EnglishWord, bool> whereExp = x => true;
            if (!string.IsNullOrEmpty(dto.searchContent))
            {
                whereExp = x => x.Word.Contains(dto.searchContent) || (x.Translate ?? string.Empty).Contains(dto.searchContent);
            }
            var allCount = allEnglishWordList.Count();
            var pageList = allEnglishWordList.Where(whereExp).Skip((dto.pageNumber - 1) * dto.pageSize).Take(dto.pageSize);

            return new { pageList, allCount };
        }


    }
}