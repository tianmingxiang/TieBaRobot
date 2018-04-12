using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using TieBaRobot.Entity;

namespace TieBaRobot.TieBa
{
    public class TieBaReply
    {
        /// <summary>
        /// 回帖验证码MD5
        /// </summary>
        public static string m_vcodeMd5 = string.Empty;

        /// <summary>
        /// 回帖验证码
        /// </summary>
        public static string m_vcode = string.Empty;

        /// <summary>
        /// 回复贴吧
        /// </summary>
        /// <returns></returns>
        public int Reply(ReplyPostData replyPostData)
        {
            string tbs = GetTbs();

            string url = "http://tieba.baidu.com/f/commit/post/add";

            Dictionary<string, string> postData = new Dictionary<string, string>();

            postData.Add("ie",  "utf-8");
            postData.Add("kw",  replyPostData.Kw);
            postData.Add("fid", replyPostData.Fid.ToString());
            postData.Add("tid", replyPostData.Tid.ToString());
            postData.Add("vcode_md5", m_vcodeMd5);
            postData.Add("tbs", tbs);
            postData.Add("content", replyPostData.Content);
            postData.Add("files", "[]");
            postData.Add("__type__", "reply");

            if (!string.IsNullOrEmpty(m_vcode))
            {
                postData.Add("vcode", m_vcode);
            }

            string formData = "";

            foreach (var item in postData.Keys)
            {
                formData += item + "=" + postData[item] + "&";
            }
            formData = formData.TrimEnd('&');

            string content = Request.PostRequest(url, formData);

            return JieXi(content);
        }

        /// <summary>
        /// 获取tbs
        /// </summary>
        /// <returns></returns>
        public static string GetTbs()
        {
            string url = "http://tieba.baidu.com/dc/common/tbs";

            string content = Request.GetRequest(url);

            JObject jObject = JObject.Parse(content);

            return jObject["tbs"].ToString();
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="replyResponse"></param>
        /// <returns></returns>
        private int JieXi(string replyResponse)
        {
            JObject jObject = JObject.Parse(replyResponse);
            
            int replyNo = Convert.ToInt32(jObject["no"]);

            m_vcode = string.Empty;

            //九宫格验证码
            if (replyNo == 40)
            {
                m_vcodeMd5 = jObject["data"]["vcode"]["captcha_vcode_str"].ToString();
            }
            else
            {  
                m_vcodeMd5 = string.Empty;
            }

            return replyNo;
        }
    }
}
