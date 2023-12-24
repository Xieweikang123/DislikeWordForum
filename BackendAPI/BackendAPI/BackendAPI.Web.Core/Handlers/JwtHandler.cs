using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.Authorization;
using Furion.DataEncryption;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BackendAPI.Web.Core
{
    public class JwtHandler : AppAuthorizeHandler
    {

        private IMemoryCache _cache;

        public JwtHandler(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        ///// <summary>
        ///// 重写 Handler 添加自动刷新收取逻辑
        ///// </summary>
        ///// <param name="context"></param>
        ///// <returns></returns>
        //public override async Task HandleAsync(AuthorizationHandlerContext context)
        //{
        //    // 自动刷新 token
        //    if (JWTEncryption.AutoRefreshToken(context, context.GetCurrentHttpContext()))
        //    {
        //        await AuthorizeHandleAsync(context);
        //    }
        //    else context.Fail();    // 授权失败
        //}

        public override async Task<bool> PipelineAsync(AuthorizationHandlerContext context, DefaultHttpContext httpContext)
        {

            //var token = httpContext.Request.Headers.Authorization.ToString();

            var token = JWTEncryption.GetJwtBearerToken(httpContext);
            var data = JWTEncryption.ReadJwtToken(token);
            var userId = data.GetClaim("UserId").Value;

            var user = context.User;

            //查询数据库 该用户token，如果token和请求token不一致，说明被顶掉了，授权失败
            var curUser = await DbContextStatic.Instance.Queryable<CoreUser>().FirstAsync(x => x.Id == userId);

            //是否支持多设备登录,多设备不校验
            if (curUser.IsSingleDevice && curUser.Token != token)
            {
                httpContext.Items["ErrorMessage"] = "账号已在其它地方登陆";
                return false;
            }

            // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false
            // 储存用户
            return true;
        }
    }
}