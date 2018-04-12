using System;
using System.IO;
using System.Net;
using System.Text;

namespace TieBaRobot.TieBa
{
    /// <summary>
    /// Get,Post请求
    /// </summary>
    public class Request
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        public static string GetRequest(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";    
            request.CookieContainer = CookieHelper.m_cookieContainer;
            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string content = sr.ReadToEnd();

                sr.Close();

                return content;
            }
            catch (Exception ex)
            {
                return "Get请求失败:" + ex.Message;
            }
            finally
            {
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="postData">请求参数</param>
        /// <returns></returns>
        public static string PostRequest(string url, string postData)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/31.0.1650.63 Safari/537.36";
                request.Method = "POST"; 
                request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
                request.Accept = "application/json, text/javascript, */*; q=0.01";

                request.CookieContainer = CookieHelper.m_cookieContainer;
                request.KeepAlive = true;

                //提交请求  
                byte[] postdatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postdatabytes.Length;
                Stream stream;
                stream = request.GetRequestStream();
                
                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();

                //接收响应  
                response = (HttpWebResponse)request.GetResponse();
                var cookieCollection = response.Cookies;

                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string content = sr.ReadToEnd();
                sr.Close();

                return content;
            }
            catch (Exception ex)
            {
                return "post请求失败:" + ex.Message;
            }
            finally
            {
                if (request != null) request.Abort();
                if (response != null) response.Close();
            }
        }
    }
}
