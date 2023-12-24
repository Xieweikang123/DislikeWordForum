using BackendAPI.Core;
using BackendAPI.Web.Core.Filter;
using BackendAPI.Web.Core.Helper;
using BackendAPI.Web.Core.Middleware;
using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Text;

namespace BackendAPI.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("configureservices");

            // 默认授权机制，需授权的即可（方法）需贴 `[Authorize]` 特性
            //services.AddJwt<JwtHandler>();
            services.AddJwt<JwtHandler>(enableGlobalAuthorize: true);


            services.AddRemoteRequest();

            services.AddCorsAccessor();

            services.AddDistributedMemoryCache(); // 添加分布式内存缓存，或者你可以使用 Redis 等其他的分布式缓存
            services.AddSession();

            //注入 ，使用 session
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


            services.AddControllers(options =>
            {
                options.Filters.Add(new HttpGlobalExceptionFilter());
            }).AddInjectWithUnifyResult();


            //Startup.cs文件添加下面代码
            services.AddSqlsugarSetup();

            //LogHelper.WriteLog("startup ConfigureServices");
            //TimerHelper.Start();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();


            //app.UseExceptionHandler(builder =>
            //{
            //    builder.Run(async context =>
            //    {
            //        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            //        var error = context.Features.Get<IExceptionHandlerFeature>();
            //        if (error != null)
            //        {
            //            //if (error.Error is CustomException customException)
            //            //{
            //            //    context.Response.StatusCode = (int)customException.StatusCode;
            //            //}
            //            await context.Response.WriteAsync(error.Error.Message);
            //        }
            //    });
            //});


            app.UseMiddleware<HandleResponseMiddleware>();
            // 默认静态资源调用，wwwroot
            app.UseStaticFiles();

            app.UseCorsAccessor();

            //在添加授权服务之前，请先确保 Startup.cs 中 Configure 是否添加了以下两个中间件：
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}