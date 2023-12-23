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

            //var cacheUserTokenKey = "UserToken_" + userId;
            //_cache

            //查询数据库 该用户token，如果token和请求token不一致，说明被顶掉了，授权失败
            var curUser = await DbContextStatic.Instance.Queryable<CoreUser>().FirstAsync(x => x.Id == userId);

            //token不一致
            if (curUser.Token != token)
            {
                Console.WriteLine("token 不一致");

                httpContext.Items["ErrorMessage"] = "账号已在其它地方登陆";
                //token不一致
                //用户在其它地方登录
                //var result = new { code = 403, message = "您的账号已在其它地方登陆" };
                //await httpContext.Response.WriteAsync(JsonConvert.SerializeObject(result));
                //await context.GetCurrentHttpContext().Response.WriteAsync(JsonConvert.SerializeObject(result));
                //context.
                return false;
            }

            // 你也可以在这里获取缓存
            //string cachedToken;
            //if (_cache.TryGetValue(cacheUserTokenKey, out cachedToken))
            //{
            //    //有缓存，比较缓存token 和当前用户token是否一致
            //    //
            //    // Do something with cachedToken
            //    Console.WriteLine("Cached token: " + cachedToken);
            //}
            //else
            //{
            //    Console.WriteLine("设置绝对缓存");
            //    //没有缓存用户token,设置绝对缓存
            //    _cache.Set(cacheUserTokenKey, token, new MemoryCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60)
            //    }); ;
            //}


            //var (isValid, tokenData, validationResult) = JWTEncryption.Validate(token);


            // 这里写您的授权判断逻辑，授权通过返回 true，否则返回 false
            // 储存用户
            Console.WriteLine("pipline:" + token);

            return true;
            //return await Task.FromResult(true);
        }
    }
}