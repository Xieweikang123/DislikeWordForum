using BackendAPI.Application.File;
using BackendAPI.Application.User;
using BackendAPI.Core;
using BackendAPI.Core.Entities;
using Furion.DistributedIDGenerator;
using Furion.LinqBuilder;
using Furion.RemoteRequest.Extensions;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

namespace BackendAPI.Application
{
    /// <summary>
    /// 通知服务接口
    /// </summary>
    /// 
    public class NotifyService : IDynamicApiController
    {

        [AllowAnonymous]
        [HttpPost]
        public async Task<object> GetTuReplyMsg1()
        {

            return null;
        }

        public class RepEntity
        {
            public string cookie { get; set; }

        }


        /// <summary>
        /// 获取兔小巢回复消息
        /// </summary>
        /// <param name="repEntity">cookie</param>
        /// <returns></returns>
        /// 
        [AllowAnonymous]
        [HttpPost]
        public async Task<object> GetTuReplyMsg(RepEntity repEntity)
        {
            var nowTime = DateTime.Now;

            var getUrl = $"https://txc.qq.com/api/v2/481319/dashboard/reply/list?page=1&count=20&from={nowTime.AddMonths(-1)}&to={nowTime}&status=0&label=all&admin=0";
            var res = await getUrl.SetHeaders(new Dictionary<string, object> {
                                { "Cookie", repEntity.cookie},
                                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36"},
                                { "Referer", "https://txc.qq.com/dashboard/new-reply-posts"}
                            }).GetAsync();

            //System.Web.HttpUtility.UrlDecode
            var tt1 = await res.Content.ReadAsStringAsync();
            //var ttu = Uri.UnescapeDataString(tt1);

            return tt1;
            //using (var client = new HttpClient())
            //{
            //    // Set the base address of the HTTP client
            //    //client.BaseAddress = new Uri("https://www.example.com/");

            //    // Send a GET request to the specified URL
            //    HttpResponseMessage response = await client.GetAsync(getUrl);

            //    // Check if the request was successful
            //    if (response.IsSuccessStatusCode)
            //    {
            //        // Read the response content as a string
            //        string result = await response.Content.ReadAsStringAsync();

            //        // Print the response content
            //        Console.WriteLine(result);
            //    }

            //}

            //return res;
        }

    }
}