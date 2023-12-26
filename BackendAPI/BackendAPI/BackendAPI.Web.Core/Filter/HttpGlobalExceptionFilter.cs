using BackendAPI.Core;
using BackendAPI.Web.Core.Helper;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace BackendAPI.Web.Core.Filter
{
    //internal class HttpGlobalExceptionFilter : IFilterMetadata
    internal class HttpGlobalExceptionFilter : IExceptionFilter
    {
        //ExceptionFilterAttribute


        public async void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var httpContext = context.HttpContext;
            //var user = httpContext.User.Identity.Name;
            var userId = httpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var request = httpContext.Request;

            await LogHelper.WriteLogAsync($@"{exception.Message}   异常堆栈：{exception.StackTrace}   请求路径：{request.Path}            用户id：{userId}");

        }
    }
}