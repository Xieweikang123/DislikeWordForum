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
    public class TestService : IDynamicApiController
    {

        [HttpGet]
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
                var imgSrcRegex = "(?<=src=\").*?(?=\")";

                var matches = Regex.Matches(dto.sayContent, imgSrcRegex);
                //将base64加密转为图片并且替换
                if (matches.Any())
                {
                    foreach (var matItem in matches)
                    {
                        var image = ImageHelper.Base64StringToImage(matItem.ToString());
                        var filePathName = FileService.GetCurrentFilePathName(".png", out var relativePath);

                        var urlPrefix = App.Configuration["OssConfig:UrlPrefix"];

                        image.Save(filePathName);
                        //替换图片路径
                        dto.sayContent = dto.sayContent.Replace(matItem.ToString(), urlPrefix + relativePath);
                    }
                }

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
            }
            return "ok";
        }




    }
}