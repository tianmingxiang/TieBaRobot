using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Newtonsoft.Json;
using TieBaRobot.Common;
using TieBaRobot.Enum;

namespace TieBaRobot.TieBa
{
    /// <summary>
    /// 百度登陆
    /// </summary>
    public class BaiduLogin
    {
        /// <summary>
        /// 判断是否登陆成功
        /// </summary>
        public static bool m_isLogin = false;

        /// <summary>
        /// 验证码
        /// </summary>
        public static string m_codeString = string.Empty;

        /// <summary>
        /// 校验的验证码
        /// </summary>
        public static string m_verifyCode = string.Empty;

        private Regex _regex = new Regex(@"\{.*\}", RegexOptions.IgnoreCase);

        /// <summary>
        /// 百度登陆
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        public string Login(string username, string pwd)
        {
            var cookieContainer = new CookieContainer();
            string token = GetToken(cookieContainer);
            if (_regex.IsMatch(token))
                token = _regex.Match(token).Value;
            var resultToken = JsonConvert.DeserializeObject<ResultToken>(token);

            string pubKey = GetPubKey(resultToken.Data.Token, cookieContainer);
            if (_regex.IsMatch(pubKey))
                pubKey = _regex.Match(pubKey).Value;
            var publicKey = JsonConvert.DeserializeObject<PublicRsaKey>(pubKey);

            string rsakey = publicKey.Key;

            string pemToXml = RSAHelper.PemToXml(publicKey.Pubkey);
            string password = RSAHelper.RSAEncrypt(pemToXml, pwd);
            string url = "https://passport.baidu.com/v2/api/?login";
            string postdata = string.Format("staticpage=http%3A%2F%2Fapp.baidu.com%2Fsfile%2Fv3Jump.html&charset=UTF-8&token={0}&tpl=wise&subpro=&apiver=v3&tt={1}&safeflg=0&u=%0D%0A%0D%0Ahttp%3A%2F%2Fapp.baidu.com%2Findex%3Fregdev%3D1&isPhone=false&quick_user=0&logintype=dialogLogin&logLoginType=pc_loginDialog&idc=&loginmerge=true&splogin=newuser&username={2}&password={3}&mem_pass=on&rsakey={4}&crypttype=12&ppui_logintime=426406&callback=parent.bd__pcbs__mwrr8d", resultToken.Data.Token, GetJsTimeSeconds(), HttpUtility.UrlEncode(username), HttpUtility.UrlEncode(password), HttpUtility.UrlEncode(rsakey));

            postdata += string.Format("&codestring={0}&verifycode={1}", m_codeString, m_verifyCode);

            return PostRequest(url, cookieContainer, postdata);
        }

        /// <summary>
        /// Token
        /// </summary>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        private string GetToken(CookieContainer cookieContainer)
        {
            var url = string.Format(@"https://passport.baidu.com/v2/api/?getapi&tpl=wise&apiver=v3&tt={0}&class=login&logintype=dialogLogin&callback=bd__cbs__v5pvt1", GetJsTimeSeconds());

            cookieContainer.SetCookies(new Uri(url), "BAIDUID=:FG=; HOSUPPORT=");
            GetHtml(url, ref cookieContainer, "passport.baidu.com");
            return GetHtml(url, ref cookieContainer, "passport.baidu.com");
        }

        /// <summary>
        /// PubKey
        /// </summary>
        /// <param name="token"></param>
        /// <param name="cookieContainer"></param>
        /// <returns></returns>
        private string GetPubKey(string token, CookieContainer cookieContainer)
        {
            var url = string.Format("https://passport.baidu.com/v2/getpublickey?token={0}&tpl=wise&apiver=v3&tt={1}&callback=bd__cbs__fwnq4r", token, GetJsTimeSeconds());
            var html = GetHtml(url, ref cookieContainer, "passport.baidu.com");
            return html;
        }

        /// <summary>
        /// 现行时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetJsTimeSeconds()
        {
            return (DateTime.UtcNow - DateTime.Parse("1970-01-01 0:0:0")).TotalMilliseconds.ToString("0");
        }

        private string GetHtml(string url, ref CookieContainer cookie, string host)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Proxy = null;
                request.Credentials = CredentialCache.DefaultNetworkCredentials;
                request.Method = "GET";
                request.KeepAlive = true;
                request.Accept = "application/json, text/plain, */*";
                request.CookieContainer = cookie;
                request.AllowAutoRedirect = true;
                request.Host = host;
                response = (HttpWebResponse)request.GetResponse();
                var sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                string ss = sr.ReadToEnd();

                //string res = "";
                //foreach (var w in response.Cookies)
                //{
                //    res += "|" + w;
                //}

                //LogManager.WriteLog(LogFile.Data, string.Format("Cookie:{0}", res));
                //LogManager.WriteLog(LogFile.Data, string.Format("ss:{0}", ss));

                sr.Close();
                request.Abort();
                response.Close();
                return ss;
            }
            catch (WebException ex)
            {
                LogManager.Instance.WriteLog(LogFileEnum.error, ex.ToString());
                return "";
            }
        }

        /// <summary>
        /// 百度登陆Post
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookieContainer"></param>
        /// <param name="postData"></param>
        private string PostRequest(string url, CookieContainer cookieContainer, string postData)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);//实例化web访问类  
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "POST"; //数据提交方式为POST  
                request.ContentType = "application/x-www-form-urlencoded";
                request.AllowAutoRedirect = false; // 不用需自动跳转
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                request.CookieContainer = cookieContainer;
                request.KeepAlive = true;

                //提交请求  
                byte[] postdatabytes = Encoding.UTF8.GetBytes(postData);
                request.ContentLength = postdatabytes.Length;
                Stream stream;
                stream = request.GetRequestStream();
                //设置POST 数据
                stream.Write(postdatabytes, 0, postdatabytes.Length);
                stream.Close();
                //接收响应  
                response = (HttpWebResponse)request.GetResponse();
                var cookieCollection = response.Cookies;//拿到bduss 说明登录成功

                //判断百度登陆是否成功
                Dictionary<string, string> myDict = new Dictionary<string, string>();

                myDict.Add("BDUSS", string.Empty);

                foreach (Cookie C in cookieCollection)
                {
                    if (myDict.Keys.Contains(C.Name))
                    {
                        myDict[C.Name] = C.Value;
                    }
                }

                if (!string.IsNullOrEmpty(myDict["BDUSS"]))
                {
                    m_isLogin = true;
                }
                else
                {
                    m_isLogin = false;
                }

                //保存返回cookie
                cookieContainer.Add(cookieCollection);

                //将cookie保存到CookieHelper
                CookieHelper.m_cookieContainer = cookieContainer;

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

    public class PublicRsaKey
    {
        [JsonProperty("errno")]
        public string Errno { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("pubkey")]
        public string Pubkey { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }
    }

    public class ErrInfo
    {

        [JsonProperty("no")]
        public string No { get; set; }
    }

    public class Loginrecord
    {

        [JsonProperty("email")]
        public object[] Email { get; set; }

        [JsonProperty("phone")]
        public object[] Phone { get; set; }
    }

    public class Data
    {

        [JsonProperty("rememberedUserName")]
        public string RememberedUserName { get; set; }

        [JsonProperty("codeString")]
        public string CodeString { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("cookie")]
        public string Cookie { get; set; }

        [JsonProperty("usernametype")]
        public string Usernametype { get; set; }

        [JsonProperty("spLogin")]
        public string SpLogin { get; set; }

        [JsonProperty("disable")]
        public string Disable { get; set; }

        [JsonProperty("loginrecord")]
        public Loginrecord Loginrecord { get; set; }
    }

    public class ResultToken
    {
        [JsonProperty("errInfo")]
        public ErrInfo ErrInfo { get; set; }

        [JsonProperty("data")]
        public Data Data { get; set; }
    }
}
