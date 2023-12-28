using BackendAPI.Application.File;
using BackendAPI.Application.User;
using BackendAPI.Core;
using Furion.DistributedIDGenerator;
using System;
using System.Security.Policy;
using System.Text.RegularExpressions;

namespace BackendAPI.Application
{
    /// <summary>
    /// 闪念服务接口
    /// </summary>
    /// 
    public class NoteService : IDynamicApiController
    {
        private readonly ISqlSugarClient _dbContext;

        public NoteService(ISqlSugarClient dbContext)
        {
            _dbContext = dbContext;
        }
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
        /// 将 <img 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public object GetBase64ImgContent(NoteDTO dto)
        {
            // 匹配 img 标签的正则表达式
            string regexPattern = @"<img\s+[^>]*src\s*=\s*[""']?([^'"" >]+?)[ '""][^>]*>";

            // 查找所有匹配的 img 标签
            MatchCollection matches = Regex.Matches(dto.sayContent, regexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            //http://localhost:..
            var urlPrefix = App.Configuration["OssConfig:UrlPrefix"];

            // 匹配 /Files/... 部分的正则表达式
            string filePatern = @"/Files/.*";



            // 遍历所有匹配的 img 标签，并将其 src 属性的值替换为 Base64 编码
            foreach (Match match in matches)
            {
                string imgSrc = match.Groups[1].Value;
                // 查找第一个匹配的结果
                Match matchfile = Regex.Match(imgSrc, filePatern);
                var filePath = App.WebHostEnvironment.WebRootPath + matchfile.Value;

                string base64String = ImageHelper.ImageToBase64(filePath);
                dto.sayContent = dto.sayContent.Replace(imgSrc, "data:image/png;base64," + base64String);
            }

            return dto;
        }

        /// <summary>
        /// 发表内容
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        [Obsolete]
        public async Task<object> SendAContent(NoteDTO dto)
        {
            var db = DbContextStatic.Instance;
            var userId = CurrentUserInfo.UserId;

            if (string.IsNullOrWhiteSpace(dto.sayContent))
            {
                throw new Exception("请输入发表内容");
            }
            try
            {
                db.BeginTran();
                var nowTime = DateTime.Now;
                dto.sayContent = ConvertBase64toUrl(dto.sayContent);
                var newNote = new Note()

                {
                    id = IDGen.NextID().ToString(),
                    userId = userId,
                    createTime = nowTime,
                    updateTime = nowTime,
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
        /// 记录笔记 日志
        /// </summary>
        /// <param name="dto"></param>
        private async void RecordOldNote(Note dto)
        {
            var noteRecord = new NoteRecord()
            {
                id = IDGen.GetStrId(),
                createTime = DateTime.Now,
                NoteId = dto.id,
                sayContent = dto.sayContent,
                status = 0,
            };
            await _dbContext.Insertable(noteRecord).ExecuteCommandAsync();
        }
        /// <summary>
        /// 添加标签
        /// </summary>
        /// <param name="addTags"></param>
        /// <param name="noteId"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        private async Task AddTags(List<NoteTag> addTags, string noteId, SqlSugarScope db)
        {

            if (addTags != null)
            {
                addTags.ForEach(x =>
                {
                    x.id = IDGen.GetStrId();
                    x.createTime = DateTime.Now;
                    x.status = 0;
                    x.userId = CurrentUserInfo.UserId;
                    x.noteId = noteId;
                });

                await db.Insertable(addTags).ExecuteCommandAsync();
            }
        }
        /// <summary>
        /// 编辑保存笔记标签
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> SaveNoteWithTag(Note dto)
        {
            var db = DbContextStatic.Instance;
            var nowTime = DateTime.Now;
            //新增
            if (string.IsNullOrEmpty(dto.id))
            {
                dto.id = IDGen.GetStrId();
                dto.createTime = nowTime;
                dto.updateTime = nowTime;
                dto.userId = CurrentUserInfo.UserId;
                try
                {
                    await db.BeginTranAsync();
                    await db.Insertable(dto).ExecuteCommandAsync();
                    //新增标签
                    await AddTags(dto.noteTags, dto.id, db);
                    await db.CommitTranAsync();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();
                    throw new Exception("新增失败:" + ex.Message);
                }

                return new { dto };
            }
            //修改
            else
            {

                var findNote = await db.Queryable<Note>().SingleAsync(x => x.id == dto.id);
                try
                {
                    db.BeginTran();
                    //内容一致，不记录日志
                    if (findNote.sayContent != dto.sayContent)
                    {
                        RecordOldNote(findNote);
                    }

                    //更新笔记内容
                    findNote.updateTime = nowTime;
                    //findNote.sayContent = dto.sayContent;
                    findNote.sayContent = ConvertBase64toUrl(dto.sayContent);
                    //更新
                    await db.Updateable(findNote).ExecuteCommandAsync();

                    //标签
                    //先删后加 删除成功数量
                    var delCount = await db.Deleteable<NoteTag>().Where(x => x.noteId == dto.id).ExecuteCommandAsync();
                    //新增标签
                    await AddTags(dto.noteTags, dto.id, db);

                    db.CommitTran();
                }
                catch (Exception ex)
                {
                    db.RollbackTran();

                    throw new Exception("更新失败:" + ex.Message);
                }

                return new { findNote };

            }


        }


        /// <summary>
        /// 获取所有标签
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetMyNoteTagList(NoteDTO dto)
        {
            var db = DbContextStatic.Instance;
            var tagList = await db.Queryable<NoteTag>().GroupBy(x => new { x.tagName, x.userId }).Select(x => new { x.tagName, x.userId, count = SqlFunc.AggregateCount(x.tagName), createTime = SqlFunc.AggregateMax(x.createTime) }).Where(x => x.userId == CurrentUserInfo.UserId).OrderByDescending(x => x.createTime).OrderByDescending(x => x.count).ToListAsync();

            return tagList;
        }

        /// <summary>
        /// 获取当前笔记的历史记录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<Object> GetCurNoteHisList(Note dto)
        {
            //找到此笔记的历史记录
            var noteRecordList = await _dbContext.Queryable<NoteRecord>().Where(x => x.status == 0 && x.NoteId == dto.id).ToListAsync();
            return noteRecordList;
        }
        /// <summary>
        /// 获取日历热图
        /// </summary>
        /// <returns></returns>

        [HttpPost]
        public async Task<object> GetCalendarHeatmapList(PageInfo dto)
        {

            var exp = Expressionable.Create<Note>(); //创建表达式
            exp.And(x => x.status == 0);
            SetPageWhere(dto, exp, 1);

            var result = await _dbContext.Queryable<Note>().Where(exp.ToExpression())
                .GroupBy(n => n.updateTime.Value.ToString("yyyy-MM-dd"))
                .Select(n => new
                {
                    UpdateDate = n.updateTime.Value.ToString("yyyy-MM-dd"),
                    NoteCount = SqlFunc.AggregateCount(n.id)
                })
                .ToListAsync();

            return result;

        }
        /// <summary>
        /// 封装pagewhere
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="exp"></param>
        /// <param name="type">type=1 assignTime不参与筛选</param>
        void SetPageWhere(PageInfo dto, Expressionable<Note> exp, int type = 0)
        {
            exp.And(x => x.userId == CurrentUserInfo.UserId);
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
                        case "status":
                            var status = Convert.ToInt16(item.value);
                            exp.And(x => x.status == status);
                            break;
                        case "assignTime":
                            if (type == 1)
                            {
                                break;
                            }
                            exp.And(x => x.updateTime.Value.ToString("yyyy-MM-dd") == item.value);
                            break;
                    }
                }
            });
        }
        /// <summary>
        /// 获取笔记分页列表
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetContentList(PageInfo dto)
        {
            var db = _dbContext;
            RefAsync<int> totalNumber = 0;
            var exp = Expressionable.Create<Note>(); //创建表达式
            SetPageWhere(dto, exp);
            var list = await db.Queryable<Note>().Includes(x => x.noteTags).Where(exp.ToExpression()).OrderByDescending(x => x.updateTime).ToPageListAsync(dto.pageNumber, dto.pageSize, totalNumber);
            return new { list, totalNumber };
        }

        /// <summary>
        /// 恢复笔记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> RecoveryAContent(Note dto)
        {
            await _dbContext.Updateable<Note>().SetColumns(x => new Note() { status = 0, updateTime = DateTime.Now })
                .Where(x => x.userId == CurrentUserInfo.UserId && x.id == dto.id).ExecuteCommandAsync();
            return "ok";
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
            var db = DbContextStatic.Instance;
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
                //await db.Deleteable<NoteTag>().Where(x => x.noteId == dto.id).ExecuteCommandAsync();

                db.CommitTran();
            }
            catch (Exception ex)
            {
                db.RollbackTran();
            }
            return "删除成功";
        }
        /// <summary>
        /// 获取指定笔记
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> GetNoteById(NoteDTO dto)
        {
            var db = DbContextStatic.Instance;
            var userId = CurrentUserInfo.UserId;

            var findItem = await db.Queryable<Note>().Includes(x => x.noteTags).FirstAsync(x => x.id == dto.id);
            if (findItem.userId != userId)
            {
                throw new Exception("没有权限查看");
            }

            return findItem;
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

            var db = DbContextStatic.Instance;
            var today = DateTime.Now.Date;
            var list = await db.Queryable<EnglishWord, CoreUser>((e, c) => e.BelongUserId == c.Id)
               .Where(e => !e.IsDel&&( e.Createdate >= today || e.Modifydate >= today))

               .GroupBy((e, c) => new { e.BelongUserId, c.NickName, c.Avatar })
               .Select((e, c) => new { Sum = SqlFunc.AggregateCount(e.BelongUserId), c.NickName, e.BelongUserId, c.Avatar })
               .OrderByDescending(e => SqlFunc.AggregateCount(e.BelongUserId))
               .ToPageListAsync(1, 50);

            return list;
        }






    }
}