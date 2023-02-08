using BackendAPI.Application;
using Furion;
using Furion.RemoteRequest.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static BackendAPI.Application.NotifyService;

namespace BackendAPI.Web.Core.Helper
{


    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimerHelper
    {
        private static Timer aTimer;

        public static void Start()
        {
            SetTimer();

            Console.WriteLine("\nPress the Enter key to exit the application...\n");
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
        }
        public static void StopTimer()
        {
            aTimer.Stop();
            aTimer.Dispose();

            Console.WriteLine("stop timer");
        }

        private static void SetTimer()
        {
            // Create a timer with a two second interval.
            aTimer = new Timer(8000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
            //                  e.SignalTime);
            try
            {
                Console.WriteLine("兔小巢 定时器运行  {0:HH:mm:ss.fff}", e.SignalTime);

                //请求消息
                var configuration = App.Configuration;
                var cookieStr = configuration["TuConfig:Cookie"];
                var notifyService = new NotifyService();
                var res = notifyService.GetTuReplyMsg(new RepEntity() { cookie = cookieStr }).Result.ToString();
                var resCn = DeUnicode(res);
                Console.WriteLine("获取到信息:" + resCn);
                var resJobj = JsonConvert.DeserializeObject(resCn) as JObject;
                var firstRepId = resJobj["data"][0]["reply_id"].ToString();
                //NoticeHelper.ShowNotification("兔小巢：", "定时器执行");
                //LogHelper.WriteLog("兔小巢调用执行");
                //是否有缓存 repid
                if (MyCache.TryGetValue(ConstValueMg.CacheReplyId, out string cacheRepId))
                {
                    //新id和缓存id是否一致
                    if (firstRepId != cacheRepId)
                    {
                        //通知
                        NoticeHelper.ShowNotification("兔小巢：", "有新回复了");
                    }
                }
                else
                {
                    //没有缓存，存入
                    MyCache.SetSliding(ConstValueMg.CacheReplyId, firstRepId, TimeSpan.FromMinutes(300));
                }
            }

            catch (Exception ex)
            {
                LogHelper.WriteLog($"兔小巢出错 , {ex.Message}");
                NoticeHelper.ShowNotification("兔小巢出错:", ex.Message);

            }
        }


        /// <summary>
        /// Unicode解码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DeUnicode(string str)
        {
            //最直接的方法Regex.Unescape(str);
            Regex reg = new Regex(@"(?i)\\[uU]([0-9a-f]{4})");
            return reg.Replace(str, delegate (Match m) { return ((char)Convert.ToInt32(m.Groups[1].Value, 16)).ToString(); });
        }
    }
}
