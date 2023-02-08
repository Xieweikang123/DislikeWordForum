using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace TestConsole
{
    public class HttpHeader
    {
        /// <summary>
        /// 代理ip
        /// </summary>
        public static string ProxyIp { get; set; }
        public static int ProxyPort { get; set; }
        public static string UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
        public static string Orgin { get; set; }
        public static string Token { get; set; }
        public static string ContentType { get; set; }
        public static string Refer { get; set; }
        public static string Cookie { get; set; }
    }
    public class HttpHelper
    {
        public static string AcceptEncoding = "gzip, deflate";
        public static string CookieStr = string.Empty;
        public static string referer = string.Empty;
        public static string accept =
            "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";

        public static CookieContainer cookieContainer = new CookieContainer();

        public static string encoding = "utf-8";
        public static string authorization = string.Empty;

        public static string Post(string url, string postStr, Dictionary<string, string> headerDic = null)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.UserAgent = HttpHeader.UserAgent;
            req.Accept = accept;
            req.Method = "POST";

            //代理ip
            if (!string.IsNullOrWhiteSpace(HttpHeader.ProxyIp))
            {
                req.Proxy = new WebProxy(HttpHeader.ProxyIp, HttpHeader.ProxyPort);
            }

            if (string.IsNullOrWhiteSpace(HttpHeader.ContentType))
            {
                req.ContentType = "application/x-www-form-urlencoded";
            }
            else
            {
                req.ContentType = HttpHeader.ContentType;
            }
            if (!string.IsNullOrWhiteSpace(HttpHeader.Token))
            {
                req.Headers.Add("token", HttpHeader.Token);
            }
            if (!string.IsNullOrWhiteSpace(HttpHeader.Orgin))
            {
                req.Headers.Add("Origin", HttpHeader.Orgin);
            }
            if (!string.IsNullOrWhiteSpace(HttpHeader.Refer))
            {
                req.Headers.Add("referer", HttpHeader.Refer);
            }
            if (!string.IsNullOrWhiteSpace(HttpHeader.Cookie))
            {
                req.Headers.Add("cookie", HttpHeader.Cookie);
            }

            if (headerDic != null && headerDic.Count > 0)
            {
                foreach (var kitem in headerDic)
                {
                    req.Headers.Add(kitem.Key, kitem.Value);
                }
            }

            req.Headers.Add("Accept-Encoding", AcceptEncoding);
            req.Referer = referer;
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                req.Headers.Add("authorization", authorization);
            }

            //if (cookieContainer != null)
            //{
            //    req.CookieContainer = cookieContainer;
            //}

            req.AllowAutoRedirect = false;
            req.KeepAlive = true;

            #region 添加Post 参数
            byte[] data = Encoding.UTF8.GetBytes(postStr);
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();

            ////获取响应内容
            //using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            //{
            //    result = reader.ReadToEnd();
            //}


            var responseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(responseStream);
            if (response.ContentEncoding == "gzip")
            {
                myStreamReader =
                    new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress),
                        Encoding.GetEncoding(encoding));
            }
            else
            {
                myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding(encoding));
            }

            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            responseStream.Close();

            return retString;
        }

        /// <summary>
        /// 指定Post地址使用Get 方式获取全部字符串 application/x-www-form-urlencoded
        /// </summary>
        /// <param name="url">请求后台地址</param>
        /// <returns></returns>
        public static string Post(string url, Dictionary<string, string> fromDataDic, Dictionary<string, string> headerDic = null)
        {
            StringBuilder builder = new StringBuilder();
            int i = 0;
            if (fromDataDic != null)
            {
                foreach (var item in fromDataDic)
                {
                    if (i > 0)
                        builder.Append("&");
                    //builder.AppendFormat("{0}={1}", item.Key, System.Web.HttpUtility.UrlEncode(item.Value));
                    builder.AppendFormat("{0}={1}", item.Key, item.Value);
                    i++;
                }
            }
            return Post(url, builder.ToString(), headerDic);
        }



        /// <summary>
        /// 以Post 形式提交数据到 uri
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="files"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static string Post(Uri uri, IEnumerable<UploadFile> files, NameValueCollection values)
        {
            string boundary = "----WebKitFormBoundary" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
            request.ContentType = "multipart/form-data; boundary=" + boundary;
            request.Method = "POST";
            request.KeepAlive = true;
            request.Credentials = CredentialCache.DefaultCredentials;

            MemoryStream stream = new MemoryStream();

            byte[] line = Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            //提交文本字段
            if (values != null)
            {
                string format = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in values.Keys)
                {
                    string s = string.Format(format, key, values[key]);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);
                }
                stream.Write(line, 0, line.Length);
            }
            request.Headers.Add("Cookie", CookieStr);
            //提交文件
            if (files != null)
            {
                string fformat = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type:{2}\r\n\r\n";
                foreach (UploadFile file in files)
                {
                    string s = string.Format(fformat, file.Name, file.Filename, file.ContentType);
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    stream.Write(data, 0, data.Length);
                    stream.Write(file.Data, 0, file.Data.Length);
                    stream.Write(line, 0, line.Length);
                }
            }

            request.ContentLength = stream.Length;

            Stream requestStream = request.GetRequestStream();


            stream.Position = 0L;
            stream.CopyTo(requestStream);
            stream.Close();


            requestStream.Close();


            using (var response = request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            using (var streamReader = new StreamReader(responseStream))
            {
                return streamReader.ReadToEnd();
            }
            //using (var mstream = new MemoryStream())
            //{
            //    responseStream.CopyTo(mstream);
            //    return mstream.ToArray();
            //}
        }
        /// <summary>
        /// 上传文件
        /// </summary>
        public class UploadFile
        {
            public UploadFile()
            {
                ContentType = "application/octet-stream";
            }
            public string Name { get; set; }
            public string Filename { get; set; }
            public string ContentType { get; set; }
            public byte[] Data { get; set; }
        }
        #region post example
        //var postUrl = "http://king.smith.man.ac.uk/mcpred/results.jsp";
        //var filePath = @"F:\360MoveData\Users\WanSanSoft\Desktop\test_mcpred.txt";
        //var postStr = FileHelper.ReadFromFile(filePath, Encoding.UTF8);

        //var uploadFiles = new List<HttpHelper.UploadFile>() {
        //    new HttpHelper.UploadFile() {
        //        Name = "seqfile",
        //        Filename = "test_mcpred.txt",
        //        Data = File.ReadAllBytes(filePath),
        //        ContentType = "text/plain"
        //    }
        //};
        //var nameValueCollection = new NameValueCollection();
        //nameValueCollection["predictor"] = "0";
        //var result = HttpHelper.Post(new Uri(postUrl), uploadFiles, nameValueCollection); 
        #endregion
        public static string GetHtml(string url, string encoding = "utf-8")
        {

            //发送Get请求
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = HttpHeader.UserAgent;
            request.Method = "GET";

            //request.ContentType = "text/html";
            request.Headers.Add("Accept-Encoding", AcceptEncoding);

            request.Referer = referer;
            //request.Timeout = 5000;
            request.ContentType = "application/x-www-form-urlencoded;";
            if (!string.IsNullOrEmpty(CookieStr))
            {
                request.Headers.Add("Cookie", CookieStr);
            }
            //if (cookieContainer != null)
            //{
            //    request.CookieContainer = cookieContainer;
            //}
            //request.Headers.Add("authority", "home.cnblogs.com");
            //request.Headers.Add("path", "/u/xieweikang/followees/");
            //request.Headers.Add("scheme", "https");

            //request.Headers.Add("cache-control", "max-age=0");
            //request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
            if (!string.IsNullOrWhiteSpace(authorization))
            {
                request.Headers.Add("authorization", authorization);
            }
            //request.Headers.Add("accept-language", "zh-CN,zh;q=0.9");
            //request.Headers.Add("cache-control", "max-age=0");
            //request.Referer = "https://home.cnblogs.com/u/xieweikang/";
            //request.Headers.Add("sec-fetch-dest", "document");
            //request.Headers.Add("sec-fetch-mode", "navigate");
            //request.Headers.Add("sec-fetch-site", "same-origin");
            //request.Headers.Add("sec-fetch-user", "?1");
            //request.Headers.Add("upgrade-insecure-requests", "1");


            //request.ContinueTimeout = 5000;
            var response = (HttpWebResponse)request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream);
            if (response.ContentEncoding == "gzip")
            {
                myStreamReader =
                    new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress),
                        Encoding.GetEncoding(encoding));
            }
            else
            {
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));
            }

            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        public static string GetNew(string url, Dictionary<string, string> headerDic)
        {

            //发送Get请求
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = HttpHeader.UserAgent;
            request.Method = "GET";

            foreach (var item in headerDic)
            {
                request.Headers.Add(item.Key, item.Value);
            }

            //request.ContinueTimeout = 5000;
            var response = (HttpWebResponse)request.GetResponse();
            var myResponseStream = response.GetResponseStream();
            var myStreamReader = new StreamReader(myResponseStream);
            if (response.ContentEncoding == "gzip")
            {
                myStreamReader =
                    new StreamReader(new GZipStream(response.GetResponseStream(), CompressionMode.Decompress),
                        Encoding.GetEncoding(encoding));
            }
            else
            {
                myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding(encoding));
            }

            var retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            myResponseStream.Close();
            return retString;
        }
        public static bool DownLoad(string url, string path, string contentType = "application/octet-stream")
        {

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.ServicePoint.Expect100Continue = false;
            req.Method = "GET";
            req.KeepAlive = true;
            req.ContentType = contentType;// "image/png";
            using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
            {
                using (Stream reader = rsp.GetResponseStream())
                {


                    using (FileStream writer = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        byte[] buff = new byte[512];
                        int c = 0; //实际读取的字节数
                        while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                        {
                            writer.Write(buff, 0, c);
                        }
                    }
                }
            }
            return true;
        }
        public static CookieCollection PostAndGetCookieCollection(string url, Dictionary<string, string> dic)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.UserAgent = HttpHeader.UserAgent;
            req.Accept = accept;
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.Headers.Add("Accept-Encoding", AcceptEncoding);
            req.Referer = referer;
            if (cookieContainer != null)
            {
                req.CookieContainer = cookieContainer;
            }
            //req.Headers.Add("Cookie", cookieStr);
            //req.CookieContainer=new CookieContainer();
            req.AllowAutoRedirect = false;
            //req.KeepAlive = true;
            #region 添加Post 参数
            StringBuilder builder = new StringBuilder();
            int i = 0;
            foreach (var item in dic)
            {
                if (i > 0)
                    builder.Append("&");
                builder.AppendFormat("{0}={1}", item.Key, item.Value);
                i++;
            }
            byte[] data = Encoding.UTF8.GetBytes(builder.ToString());
            req.ContentLength = data.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(data, 0, data.Length);
                reqStream.Close();
            }
            #endregion
            HttpWebResponse response = (HttpWebResponse)req.GetResponse();
            return response.Cookies;
        }


    }


}
