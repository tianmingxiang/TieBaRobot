namespace TieBaRobot.TieBa
{
    public class Vcode
    {
        /// <summary>
        /// 得到回帖验证码图片
        /// </summary>
        /// <param name="captchaVcodeStr"></param>
        /// <returns></returns>
        public static string GetVcodeUrl(string captchaVcodeStr)
        {
            return "http://tieba.baidu.com/cgi-bin/genimg?" + captchaVcodeStr;             
        }

        /// <summary>
        /// 得到百度登陆验证码图片
        /// </summary>
        /// <param name="captchaVcodeStr"></param>
        /// <returns></returns>
        public static string GetLoginVcodeUrl(string captchaVcodeStr)
        {
            return "https://passport.baidu.com/cgi-bin/genimage?" + captchaVcodeStr;
        }
    }
}
