using BackendAPI.Application.File;
using BackendAPI.Application.User;
using BackendAPI.Core;
using Furion.DistributedIDGenerator;
using System.Text;
using System.Text.RegularExpressions;

namespace BackendAPI.Application
{
    /// <summary>
    /// 闪念服务接口
    /// </summary>
    /// 
    public class TestService : IDynamicApiController
    {

        [AllowAnonymous]
        [HttpGet("/sse")]
        public async Task<IActionResult> GetSSE()
        {

            //Response.Headers.Add("Content-Type", "text/event-stream");

            //for (int i = 0; true; ++i)
            //{
            //    await Response.WriteAsync($"data: {i}\n\n");
            //    await Response.Body.FlushAsync();
            //    await Task.Delay(1000);
            //}


            ////var rep=HttpContent

            //var httpContext = HttpContext;

            var HttpContext = App.HttpContext;
            var Response = App.HttpContext.Response;


            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            Response.Headers.Add("X-Accel-Buffering", "no");

            //var writer = new StreamWriter(Response.Body);

            using var writer = new StreamWriter(Response.Body, Encoding.UTF8);


            var random = new Random();


            var str = "如果在非控制器类（例如仓储类或服务类）中使用SSE，那么你需要通过`IHttpContextAccessor`来获取当前HTTP上下文，以便访问响应对象。\r\n\r\n首先，在`Startup.cs`文件的`ConfigureServices`方法中，注册`IHttpContextAccessor`服务：\r\n\r\n```csharp\r\nservices.AddHttpContextAccessor();\r\n```\r\n\r\n然后，在需要访问响应对象的类中注入`IHttpContextAccessor`：\r\n\r\n```csharp\r\nprivate readonly IHttpContextAccessor _httpContextAccessor;\r\n\r\npublic MyService(IHttpContextAccessor httpContextAccessor)\r\n{\r\n    _httpContextAccessor = httpContextAccessor;\r\n}\r\n```\r\n\r\n接下来，在需要使用响应对象的方法中，通过`_httpContextAccessor.HttpContext.Response`属性来获取响应对象。例如：\r\n\r\n```csharp\r\npublic async Task SendSSEData(string data)\r\n{\r\n    var response = _httpContextAccessor.HttpContext.Response;\r\n    response.Headers.Add(\"Content-Type\", \"text/event-stream\");\r\n    response.Headers.Add(\"Cache-Control\", \"no-cache\");\r\n    response.Headers.Add(\"X-Accel-Buffering\", \"no\");\r\n\r\n    using var writer = new StreamWriter(response.Body);\r\n    await writer.WriteAsync($\"data: {data}\\n\\n\");\r\n    await writer.FlushAsync();\r\n}\r\n```\r\n\r\n在上面的代码中，我们使用了`_httpContextAccessor.HttpContext.Response`属性来获取响应对象。然后我们设置响应头，创建`StreamWriter`对象，并将数据作为SSE数据推送到客户端。\r\n\r\n需要注意的是，要及时对响应对象进行清理，否则可能会导致内存泄漏或其他问题。在这个例子中，我们使用了`using`语句来确保及时释放资源。\r\n\r\n希望这个解决方案可以帮助你在非控制器类中使用SSE。";


            var txtIndex = 0;

            while (!HttpContext.RequestAborted.IsCancellationRequested)
            {
                //await writer.WriteAsync($"data: {random.Next()} \n\n");
                await writer.WriteAsync($"{str[txtIndex++]}");
                await writer.FlushAsync();
                await Task.Delay(10);
            }

            return new EmptyResult();



        }




        [HttpGet]
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