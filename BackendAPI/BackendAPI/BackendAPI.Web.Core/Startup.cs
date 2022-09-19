using Furion;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace BackendAPI.Web.Core
{
    public class Startup : AppStartup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // 默认授权机制，需授权的即可（方法）需贴 `[Authorize]` 特性
            //services.AddJwt<JwtHandler>();
            services.AddJwt<JwtHandler>(enableGlobalAuthorize:true);


            services.AddCorsAccessor();

            services.AddControllers()
                //.AddJsonOptions(options =>
                //{
                //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                //})
                    .AddInjectWithUnifyResult();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 默认静态资源调用，wwwroot
            app.UseStaticFiles();

            app.UseCorsAccessor();

            //在添加授权服务之前，请先确保 Startup.cs 中 Configure 是否添加了以下两个中间件：
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseInject(string.Empty);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}