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
    /// 闪念服务接口
    /// </summary>
    /// 
    public class NoteService : IDynamicApiController
    {

        /// <summary>
        /// 将内容里的bsse64转图片
        /// </summary>
        /// <param name="oriContent"></param>
        /// <returns></returns>
        private string ConvertBase64toUrl(string oriContent)
        {
            //var retStr = string.Empty;
            var imgSrcRegex = "(?<=src=\").*?(?=\")";

            var matches = Regex.Matches(oriContent, imgSrcRegex);
            //将base64加密转为图片并且替换
            if (matches.Any())
            {
                foreach (var matItem in matches)
                {
                    var imgStr = matItem.ToString();
                    //不是base64图片不处理
                    if (!imgStr.Contains("data:image/png;base64"))
                    {
                        continue;
                    }
                    var image = ImageHelper.Base64StringToImage(imgStr);
                    var filePathName = FileService.GetCurrentFilePathName(".png", out var relativePath);

                    var urlPrefix = App.Configuration["OssConfig:UrlPrefix"];

                    image.Save(filePathName);
                    //替换图片路径
                    oriContent = oriContent.Replace(imgStr, urlPrefix + relativePath);
                }
            }

            return oriContent;
        }

        /// <summary>
        /// 发表内容
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> SendAContent(NoteDTO dto)
        {
            var db = DbContext.Instance;
            var userId = CurrentUserInfo.UserId;

            if (string.IsNullOrWhiteSpace(dto.sayContent))
            {
                throw new Exception("请输入发表内容");
            }
            try
            {
                db.BeginTran();
                var nowTime = DateTime.Now;
                //var imgSrcRegex = "(?<=src=\").*?(?=\")";

                //var matches = Regex.Matches(dto.sayContent, imgSrcRegex);
                ////将base64加密转为图片并且替换
                //if (matches.Any())
                //{
                //    foreach (var matItem in matches)
                //    {
                //        var imgStr = matItem.ToString();
                //        //不是base64图片不处理
                //        if (!imgStr.Contains("data:image/png;base64"))
                //        {
                //            continue;
                //        }
                //        var image = ImageHelper.Base64StringToImage(imgStr);
                //        var filePathName = FileService.GetCurrentFilePathName(".png", out var relativePath);

                //        var urlPrefix = App.Configuration["OssConfig:UrlPrefix"];

                //        image.Save(filePathName);
                //        //替换图片路径
                //        dto.sayContent = dto.sayContent.Replace(matItem.ToString(), urlPrefix + relativePath);
                //    }
                //}
                dto.sayContent = ConvertBase64toUrl(dto.sayContent);


                var newNote = new Note()

                {
                    id = IDGen.NextID().ToString(),
                    userId = userId,
                    createTime = nowTime,
                    sayContent = dto.sayContent,

                };

                await db.Insertable(newNote).ExecuteCommandAsync();
                //如果有标签要插入
                if (!string.IsNullOrEmpty(dto.tagName))
                {
                    var newTag = new NoteTag()
                    {
                        id = IDGen.GetStrId(),
                        createTime = nowTime,
                        status = 0,
                        userId = userId,
                        tagName = dto.tagName,
                        noteId = newNote.id
                    };
                    await db.Insertable(newTag).ExecuteCommandAsync();
                }

                db.CommitTran();
            }
            catch (Exception ex)
            {

                db.RollbackTran();
                throw new Exception(ex.Message);

                //return ex.Message;

            }
            return "ok";
        }

        /// <summary>
        /// 编辑保存笔记标签
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> SaveNoteWithTag(Note dto)
        {

            var db = DbContext.Instance;
            var findNote = await db.Queryable<Note>().SingleAsync(x => x.id == dto.id);

            try
            {
                db.BeginTran();
                var nowTime = DateTime.Now;
                var userId = CurrentUserInfo.UserId;
                //更新笔记内容
                findNote.updateTime = nowTime;
                //findNote.sayContent = dto.sayContent;
                findNote.sayContent = ConvertBase64toUrl(dto.sayContent);
                //更新
                await db.Updateable(findNote).ExecuteCommandAsync();

                //标签
                //先删后加 删除成功数量
                var delCount = await db.Deleteable<NoteTag>().Where(x => x.noteId == dto.id).ExecuteCommandAsync();
                //新增
                var addTags = dto.noteTags;
                addTags.ForEach(x =>
                {
                    x.id = IDGen.GetStrId();
                    x.createTime = nowTime;
                    x.status = 0;
                    x.userId = userId;
                    x.noteId = dto.id;
                });

                await db.Insertable(addTags).ExecuteCommandAsync();
                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();

                throw new Exception("更新失败:" + ex.Message);
            }

            return new { findNote };
        }


        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetMyNoteTagList()
        {
            var db = DbContext.Instance;
            //var tagList = await db.Queryable<NoteTag>().Distinct().Select(x => new { x.tagName, x.userId, count = SqlFunc.AggregateDistinctCount(x.tagName) }).Where(x => x.userId == CurrentUserInfo.UserId).OrderByDescending(x => x.count).ToListAsync();

            var tagList = await db.Queryable<NoteTag>().GroupBy(x => new { x.tagName, x.userId }).Select(x => new { x.tagName, x.userId, count = SqlFunc.AggregateCount(x.tagName) }).Where(x => x.userId == CurrentUserInfo.UserId).OrderByDescending(x => x.count).ToListAsync();

            return tagList;
        }


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
            var exp = Expressionable.Create<Note>(); //创建表达式
            exp.And(x => x.status == 0 && x.userId == CurrentUserInfo.UserId);
            dto.searchKeyValues.ForEach(item =>
            {
                if (!string.IsNullOrWhiteSpace(item.value))
                {
                    switch (item.key)
                    {
                        case "tagName":
                            exp.And(x => x.noteTags.Any(c => c.tagName == item.value));
                            break;
                        case "sayContent":
                            exp.And(x => x.sayContent.Contains(item.value));
                            break;
                    }
                }

            });


            var list = await db.Queryable<Note>().Includes(x => x.noteTags).Where(exp.ToExpression()).OrderByDescending(x => x.id).ToPageListAsync(dto.pageNumber, dto.pageSize, totalNumber);
            return new { list, totalNumber };
        }

        /// <summary>
        /// 删除一条笔记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<object> DelAContent(Note dto)
        {
            var db = DbContext.Instance;
            var userId = CurrentUserInfo.UserId;

            try
            {
                db.BeginTran();

                //改闪念是否为当前人的
                var findItem = db.Queryable<Note>().First(x => x.id == dto.id);
                if (findItem.userId != userId)
                {
                    throw new Exception("没有权限删除");
                }

                findItem.status = 1;
                findItem.updateTime = DateTime.Now;
                await db.Updateable(findItem).ExecuteCommandAsync();
                //删除标签
                await db.Deleteable<NoteTag>().Where(x => x.noteId == dto.id).ExecuteCommandAsync();

                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
            }
            return "删除成功";
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