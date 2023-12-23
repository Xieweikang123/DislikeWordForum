using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendAPI.Web.Core.Middleware
{
    internal class HandleResponseMiddleware
    {

        private readonly RequestDelegate _next;

        public HandleResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 在此处进行请求处理

            // 调用管道中的下一个中间件
            await _next(context);

            // 在此处进行响应处理
            //如果是403，返回为什么是403
            if (context.Response.StatusCode == 403 && context.Items.ContainsKey("ErrorMessage"))
            {
                //context.Response.ContentType= "application/json";
                await context.Response.WriteAsync(context.Items["ErrorMessage"].ToString());
            }
        }
    }
}
