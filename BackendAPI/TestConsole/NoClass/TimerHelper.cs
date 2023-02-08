using BackendAPI.Application;
using Furion.RemoteRequest.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using TestConsole;
using static BackendAPI.Application.NotifyService;

namespace BackendAPI.Web.Core.Helper
{


    /// <summary>
    /// 定时任务
    /// </summary>
    public class TimerHelper
    {
        private static System.Timers.Timer aTimer;

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
            aTimer = new System.Timers.Timer(15000);
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
                //var configuration = App.Configuration;
                var cookieStr = "RK=tGegLQOheq; ptcz=fb0573fe12f545362c01f557a0c7d6d589f44c85706cb16134728c365290ae33; iip=0; fqm_pvqid=4ae0d1fd-bf4c-43a4-9101-3ff012386983; o_cookie=570312124; pac_uid=1_570312124; eas_sid=F1l615A2e8D5q5e2Y2h0b9Q188; ied_qq=o0570312124; _ga=GA1.1.200419157.1658224352; _ga_MRRHVE420B=GS1.1.1658468877.2.1.1658468879.0; _ga_2DYB6E8NZY=GS1.1.1659421079.6.0.1659421079.0; tvfe_boss_uuid=f66b00feb0d2c239; _clck=3872325012|1|f5b|0; pgv_pvid=8084224854; _tc_unionid=32f97205-093b-4696-98fb-684c7f22fc74; LW_sid=U1P6C7q100m7x6h8g3o8O9u3q9; LW_uid=71X657C1l0T786c8k3B8I9A440; pgv_info=ssid=s8394825560; pvpqqcomrouteLine=index_newsdetail; tokenParams=?tid=588238; _horizon_uid=2e68502a-75a5-43d7-8fee-427685096a7f; _qpsvr_localtk=0.8987932736083446; verifysession=h0196eb5dca65ce22c8f137a10ee733bd3c3b6ea3116811adf05a01cfb9714b897c3351fc0cf2a7bf27; ptui_loginuin=53354351; skey=@5OKDLk1vE; uin=o570312124; _horizon_sid=59ea8bf0-5bc4-494c-a9a4-a1240ad4683c; _tucao_session=WHFJSEQxamxzV25Ock5PU244bjBlSENUb05IR0JLT2JyUEgzRGV2YTZ3WHg0UEw3NDE3QWhIUHhFUTYvczZKbHZaem94QXVWSDU0UXlwc3pYR0JwbVJyWTBjUXkwSEFsQUd2SHV4RWJ6bXFDMXExVmZDQmd0ZE15U0pMSXhXM1Y=--NZQhkxMHSE7g3ZisK35bzA==";
                //var notifyService = new NotifyService();
                var res = GetTuReplyMsg(cookieStr);
                var resCn = DeUnicode(res);
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
