using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using TestConsole;

namespace BackendAPI.Web.Core.Helper
{


    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimerHelper
    {
        private static System.Timers.Timer aTimer;

        public static IConfigurationRoot configuration;

        public static void Start()
        {
            SetTimer();

            //Console.WriteLine("\nPress the Enter key to exit the application...\n");
            //Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);
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
            var interval = int.Parse(configuration.GetSection("Settings")["RequestInterval"]);
            aTimer = new System.Timers.Timer(interval);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            //Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}",
            //                  e.SignalTime);
            try
            {
                Console.WriteLine("兔小巢 定时器运行  {0:HH:mm:ss.fff}", e.SignalTime);

                //请求消息
                //var configuration = App.Configuration;

                //configuration.
                var cookieStr = configuration.GetSection("Settings")["Cookie"];
                //var notifyService = new NotifyService();
                var res = GetTuReplyMsg(cookieStr);
                var resCn = DeUnicode(res);
                if (resCn.Contains("先登录"))
                {
                    throw new Exception("请先登录");
                }
                Console.WriteLine("获取到信息");
                var resJobj = JsonConvert.DeserializeObject(resCn) as JObject;
                var firstRepId = resJobj["data"][0]["reply_id"].ToString();
                //NoticeHelper.ShowNotification("兔小巢：", "定时器执行");
                //LogHelper.WriteLog("兔小巢调用执行");
                //是否有缓存 repid
                var chcheId = "cacheId";
                if (MyCache.TryGetValue(chcheId, out string cacheRepId))
                {
                    //新id和缓存id是否一致
                    if (firstRepId != cacheRepId)
                    {
                        //通知
                        NoticeHelper.ShowNotification("兔小巢：", "有新回复了");
                        MyCache.SetSliding(chcheId, firstRepId, TimeSpan.FromMinutes(300));
                    }
                }
                else
                {
                    Console.WriteLine($"没有缓存，存入{firstRepId}");

                    //没有缓存，存入
                    MyCache.SetSliding(chcheId, firstRepId, TimeSpan.FromMinutes(300));
                }
            }

            catch (Exception ex)
            {
                NoticeHelper.ShowNotification("兔小巢出错:", ex.Message);

            }

        }



        public static string GetTuReplyMsg(string cookieStr)
        {

            var nowTime = DateTime.Now;
            var getUrl = $"https://txc.qq.com/api/v2/481319/dashboard/reply/list?page=1&count=20&from={nowTime.AddMonths(-1)}&to={nowTime}&status=0&label=all&admin=0";
            var headerDic = new Dictionary<string, string>()
            {
                { "Cookie",cookieStr},
                { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/109.0.0.0 Safari/537.36"},
                 { "Referer", "https://txc.qq.com/dashboard/new-reply-posts"}
            };
            var res = HttpHelper.GetNew(getUrl, headerDic);
            //System.Web.HttpUtility.UrlDecode
            //var tt1 = await res.Content.ReadAsStringAsync();
            return res;
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
