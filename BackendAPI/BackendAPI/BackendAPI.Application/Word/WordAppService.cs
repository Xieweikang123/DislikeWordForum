﻿using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.LinqBuilder;
using Furion.RemoteRequest.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
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
        /// 获取最近7天单词数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public object GetRecentWordChartData(int howManyDays)
        {
            //var userId = CurrentUserInfo.UserId;
            //var db = DbContext.Instance;
            var wordlist = GetCacheEnglishWords();

            //最近七天
            var nowTime = DateTime.Now;
            var recDays = new List<string>();
            var wordCounts = new List<int>();
            for (var i = howManyDays; i >= 0; i--)
            {
                var curDay = nowTime.AddDays(-i);
                recDays.Add(curDay.ToString("MM-dd"));
                //当天单词数
                var wcount = wordlist.Count(x => x.Modifydate != null && SqlFunc.DateIsSame(curDay, x.Modifydate));
                wordCounts.Add(wcount);
            }
            return new { recDays, wordCounts };
        }



        /// <summary>
        /// 获取大于指定记录数的单词
        /// </summary>
        /// <param name="recordTimes"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetGreaterThanRecordWords(int recordTimes)
        {

            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;
            var wordlist = GetCacheEnglishWords();
            var list = wordlist.Where(x => x.BelongUserId == userId && x.RecordTimes >= recordTimes).OrderByDescending(x => x.Views).ToList();
            return list;
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<object> Translate(string word)
        {
            var obj = await $"http://fanyi.youdao.com/translate?&doctype=json&type=AUTO&i={word}".GetAsStringAsync();
            return obj;
        }

        [HttpPost]
        public async Task<object> GetMyWordList(PageInfoWord dto)
        {
            var allEnglishWordList = GetCacheEnglishWords();
            //var orderProp = "RecordTimes";

            //指定了排序列
            if (!string.IsNullOrEmpty(dto.prop))
            {
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
            //Func<EnglishWord, bool> whereExp = x => true;

            Expression<Func<EnglishWord, bool>> where = c => true;

            if (!string.IsNullOrEmpty(dto.searchContent))
            {
                dto.searchContent = dto.searchContent.Trim();
                //whereExp = x => x.Word.Contains(dto.searchContent) || (x.Translate ?? string.Empty).Contains(dto.searchContent);

                where = x => x.Word.Contains(dto.searchContent) || (x.Translate ?? string.Empty).Contains(dto.searchContent);
            }

            var today = DateTime.Now.Date;

            //搜索范围
            switch (dto.searchScop)
            {
                case SearchScope.全部:
                default:

                    break;
                case SearchScope.今日:

                    where = where.And(x => x.Createdate >= today || x.Modifydate >= today);
                    break;
                case SearchScope.昨天:
                    var yeasterDayStart = today.AddDays(-1);
                    where = where.And(x => x.Createdate >= yeasterDayStart && x.Createdate < today || x.Modifydate >= yeasterDayStart && x.Modifydate < today);
                    break;
                case SearchScope.最近7天:
                    var recent7DayStart = today.AddDays(-7);
                    where = where.And(x => x.Createdate >= recent7DayStart || x.Modifydate >= recent7DayStart);
                    break;
            }
            //筛选
            var filterEnumeralble = allEnglishWordList.Where(where.Compile());
            var allCount = filterEnumeralble.Count();
            var pageList = filterEnumeralble.Skip((dto.pageNumber - 1) * dto.pageSize).Take(dto.pageSize);
            //var pageList = allEnglishWordList.Where(whereExp).Skip((dto.pageNumber - 1) * dto.pageSize).Take(dto.pageSize);

            return new { pageList, allCount };
        }




        /// <summary>
        /// 保存单词
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<object> OnSaveWord(EnglishWord dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;
            //删除
            //await db.Deleteable<EnglishWord>(x => x.id == dto.id && x.BelongUserId == userId).ExecuteCommandAsync();
            var wordList = GetCacheEnglishWords();
            var findWord = wordList.FirstOrDefault(x => x.id == dto.id);
            //单词是否存在
            if (findWord == null)
            {
                //不存在
                dto.id = IDGen.NextID().ToString();
                await db.Insertable(dto).ExecuteCommandAsync();
            }
            else
            {
                //单词修改
                findWord.Word = dto.Word;
                findWord.Modifydate = DateTime.Now;
                findWord.Translate = dto.Translate;
                findWord.RecordTimes = dto.RecordTimes;
                await db.Updateable(findWord).ExecuteCommandAsync();
            }

            //刷新单词
            GetCacheEnglishWords(true);

            return "ok";
        }

        /// <summary>
        /// 删除单词
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> OnDelWord(EnglishWord dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;
            //删除
            await db.Deleteable<EnglishWord>(x => x.id == dto.id && x.BelongUserId == userId).ExecuteCommandAsync();

            //刷新单词缓存
            GetCacheEnglishWords(true);

            return "ok";
        }

        /// <summary>
        /// 增加单词浏览量
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> AddViews(EnglishWord dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;

            await db.Updateable<EnglishWord>().SetColumns(x => x.Views == (x.Views ?? 0) + 1)
               .Where(x => x.BelongUserId == CurrentUserInfo.UserId && x.id == dto.id).ExecuteCommandAsync();
            //刷新单词缓存
            //GetCacheEnglishWords(true);
            return "ok";
        }
        /// <summary>
        /// 获取落在各个次数的单词数 0 的多少个，1的多少个，2的多少个...
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<object> GetEachTimesWords()
        {
            var allWords = GetCacheEnglishWords();

            var result = await Task.Run(() =>
            {
                return allWords.GroupBy(x => x.RecordTimes)
                               .ToDictionary(g => g.First().RecordTimes, g => g.ToList())
                               .OrderBy(x=>x.Key)
                               .ToList();
            });
            return result;

        }

        /// <summary>
        /// 记录单词
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> RecordWord(EnglishWord dto)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;

            //获取缓存单词
            var cacheEnglishWordList = GetCacheEnglishWords();

            dto.Word = dto.Word.Trim();
            //查键入单词
            var findWord = cacheEnglishWordList.FirstOrDefault(x => x.Word == dto.Word);
            //如果存在
            if (findWord != null)
            {
                findWord.RecordTimes++;
                findWord.Modifydate = DateTime.Now;
                await db.Updateable(findWord).ExecuteCommandAsync();
            }
            else
            {
                //不存在，新增
                findWord = new EnglishWord()
                {
                    id = IDGen.NextID().ToString(),
                    BelongUserId = userId,
                    Createdate = DateTime.Now,
                    RecordTimes = 1,
                    Word = dto.Word,
                };
                await db.Insertable(findWord).ExecuteCommandAsync();
            }

            //获取最新单词
            GetCacheEnglishWords(true);
            return "ok";
        }

        /// <summary>
        /// 查询缓存单词
        /// </summary>
        /// <param name="isGetNew">重新获取</param>
        /// <returns></returns>
        public List<EnglishWord> GetCacheEnglishWords(bool isGetNew = false)
        {
            var userId = CurrentUserInfo.UserId;
            var db = DbContextStatic.Instance;
            var curUserWordCache = wordCacheKey + userId;
            //清缓存
            if (isGetNew)
            {
                Console.WriteLine("清缓存");
                _memoryCache.Remove(curUserWordCache);
            }
            //将单词查出来缓存
            var wordlist = _memoryCache.GetOrCreate(curUserWordCache, entry =>
            {
                Console.WriteLine("查缓存");
                entry.SlidingExpiration = TimeSpan.FromSeconds(15);
                var list = db.Queryable<EnglishWord>().Where(x => x.BelongUserId == userId).ToList();
                return list;
            });

            return wordlist;
        }


    }
}