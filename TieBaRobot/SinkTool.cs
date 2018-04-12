using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Threading;
using System.Net;
using TieBaRobot.Entity;
using TieBaRobot.Common;
using TieBaRobot.Enum;
using TieBaRobot.TieBa;

namespace TieBaRobot.Winform
{
    public partial class SinkTool : Form
    {
        /// <summary>
        /// 客户端用户
        /// </summary>
        private static string m_userName = string.Empty;

        /// <summary>
        /// 百度账号登录名
        /// </summary>
        private static string m_loginName = string.Empty;

        /// <summary>
        /// 百度密码
        /// </summary>
        private static string m_passWord = string.Empty;

        /// <summary>
        /// 一个百度账号回帖次数，回帖满之后换百度账号登陆
        /// </summary>
        private const int m_replyMaxNum = 10;

        /// <summary>
        /// 已经回帖次数，初始化设置为最大回帖次数，是为了首次加载登陆
        /// </summary>
        private static int m_replyNum = m_replyMaxNum;

        /// <summary>
        /// 回复帖子Post参数
        /// </summary>
        private static ReplyPostData m_replyPostData = new ReplyPostData();

        /// <summary>
        /// 数据库中待回复的帖子ID
        /// </summary>
        private static int m_replyId = 0;

        /// <summary>
        /// 所有回复的内容
        /// </summary>
        private static List<string> m_contentList = new List<string>();

        /// <summary>
        /// 回复帖子间隔
        /// </summary>
        private static int m_replySeconds = 5000;

        /// <summary>
        /// 登陆验证码解析的正则
        /// </summary>
        private const string m_loginVcodeRegex = "(?<=codeString=)[^&]+?(?=&)";

        /// <summary>
        /// 登陆错误
        /// </summary>
        private const string m_loginErrNo = "(?<=err_no=)[^&]+?(?=&)";

        /// <summary>
        /// 设定一个线程
        /// </summary>
        /// <param name="userName"></param>
        private Thread thread = null;

        /// <summary>
        /// 委托label
        /// </summary>
        /// <param name="t"></param>
        /// <param name="s"></param>
        private delegate void LableDelegate(Label t, string s);

        private LableDelegate labledelegate = null;

        /// <summary>
        /// 需要回复的主题数量
        /// </summary>
        private static int m_lableTotalNum = 0;

        /// <summary>
        /// 该客户端已经回复主题数量
        /// </summary>
        private static int m_lableReplyNum = 0;

        /// <summary>
        /// 百度登陆成功cookies
        /// </summary>
        private static string m_cookies = string.Empty;

        public SinkTool()
        {
            InitializeComponent();

            //获取所有的回帖帖子
            //var replyContent = ClientPost.GetReplyContent();

            //if (Convert.ToInt32(replyContent["Code"]) == ApiCode.Code200)
            //{
            //    m_contentList = JsonHelper.DeserializeObject<List<string>>(replyContent["ContentList"].ToString());
            //}

            //获取需回复和已回复的主题
            //m_userName = userName;

            //var json = ClientPost.GetTitleNum(m_userName);

            //if (Convert.ToInt32(json["Code"]) == ApiCode.Code200)
            //{
            //    labelTotalNum.Text = Convert.ToString(json["TotalNum"]);
            //    labelReplyNum.Text = Convert.ToString(json["ReplyNum"]);
            //    m_lableTotalNum = Convert.ToInt32(json["TotalNum"]);
            //    m_lableReplyNum = Convert.ToInt32(json["ReplyNum"]);
            //}

            labledelegate = new LableDelegate(LableText);

            SetBtnStyle(button1);
            SetBtnStyle(button2);
            SetBtnStyle(button3);
            SetBtnStyle(button4);
            SetBtnStyle(button5);
            SetBtnStyle(button6);
            SetBtnStyle(button7);
            SetBtnStyle(button8);
            SetBtnStyle(button9);
        }

        private void SetBtnStyle(Button btn)
        {
            int px = pictureBoxVcode2.Location.X;
            int py = pictureBoxVcode2.Location.Y;

            btn.FlatStyle = FlatStyle.Flat;//样式
            btn.ForeColor = Color.White;
            btn.BackColor = Color.Transparent;//去背景
            btn.FlatAppearance.BorderSize = 1;//去边线
            btn.FlatAppearance.MouseOverBackColor = Color.Transparent;//鼠标经过
            btn.FlatAppearance.MouseDownBackColor = Color.Transparent;//鼠标按下

            btn.Location = new Point(btn.Location.X - px, btn.Location.Y - py);

            btn.Parent = this.pictureBoxVcode2;
        }

        /// <summary>
        /// 更换百度账号
        /// </summary>
        private void ChangeBaiduLogin()
        {
            try
            {
                if (m_replyNum == m_replyMaxNum)
                {
                    //更换百度账号，再去获取一下需回复和已回复的数量
                    //var JsonTitleNum = ClientPost.GetTitleNum(m_userName);

                    //if (Convert.ToInt32(JsonTitleNum["Code"]) == ApiCode.Code200)
                    //{
                    //    m_lableTotalNum = Convert.ToInt32(JsonTitleNum["TotalNum"]);
                    //    m_lableReplyNum = Convert.ToInt32(JsonTitleNum["ReplyNum"]);
                                                
                    //    this.BeginInvoke(labledelegate, new object[] { labelReplyNum, m_lableReplyNum.ToString() });
                    //    this.BeginInvoke(labledelegate, new object[] { labelTotalNum, m_lableTotalNum.ToString() });
                    //}

                    //从数据库中随机取一个正常的百度账号,并更新是否登陆为是，并把之前的账号是否登陆清空为否
                    //var json = ClientPost.GetAccount(m_loginName);

                    //int code = Convert.ToInt32(json["Code"]);

                    //if (code == ApiCode.Code200)
                    //{
                    //    m_loginName = json["LoginName"].ToString();
                    //    m_passWord = json["PassWord"].ToString();
                    //    m_cookies = json["Cookies"].ToString();

                    //    if (!string.IsNullOrEmpty(m_cookies))
                    //    {
                    //        LoginByCookies();
                    //    }
                    //    else
                    //    {
                    //        Login();
                    //    }
                    //}
                    //else if (code == ApiCode.Code800)
                    //{
                    //    this.BeginInvoke(labledelegate, new object[] { labelWarning, "后台没有可用的百度账号！" });

                    //    if (thread != null)
                    //    {
                    //        thread.Abort();
                    //    }
                    //}
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.WriteLog(LogFileEnum.error, "异常ChangeBaiduLogin：" + ex.ToString());
            }
        }

        /// <summary>
        /// 百度登陆判断
        /// </summary>
        private void Login()
        {
            this.BeginInvoke(labledelegate, new object[] { labelWarning, "正在登陆百度账号..." });

            BaiduLogin baiduLogin = new BaiduLogin();

            //登陆
            string content = baiduLogin.Login(m_loginName, m_passWord);

            if (BaiduLogin.m_isLogin)
            {
                BaiduLogin.m_codeString = string.Empty;

                BaiduLogin.m_verifyCode = string.Empty;

                this.BeginInvoke(labledelegate, new object[] { labelWarning, "百度账号登陆成功！" });

                m_replyNum = 0;
            }
            else
            {
                int errNo = Convert.ToInt32(Regex.Match(content, m_loginErrNo).ToString());

                //验证码错误
                if (errNo == TieBaError.ErrNo257 || errNo == TieBaError.ErrNo6)
                {
                    BaiduLogin.m_codeString = Regex.Match(content, m_loginVcodeRegex).ToString();

                    pictureBoxVcode1.ImageLocation = Vcode.GetLoginVcodeUrl(BaiduLogin.m_codeString);

                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "请输入登陆验证码！" });

                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }
                //ip地址被禁
                else if (errNo == TieBaError.ErrNo13)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "IP地址被禁。" });

                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }
                //重新登陆
                else if (errNo == TieBaError.ErrNo2101 || errNo == TieBaError.ErrNo260005)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "登陆异常,正在重新登陆..." });

                    //重新登陆，必须回帖数等于账号可回最大回帖数
                    m_replyNum = m_replyMaxNum;

                    ChangeBaiduLogin();
                }
                else if (errNo == TieBaError.ErrNo100023)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "回帖异常，请结束回帖" });

                    LogManager.Instance.WriteLog(LogFileEnum.error, "异常:" + content);

                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }
                //异常
                else
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "百度账号异常,正在更换百度账号..." });

                    //更新账号异常
                    //ClientPost.UpdateAccountAbnormal(m_loginName, errNo);

                    LogManager.Instance.WriteLog(LogFileEnum.error, "异常:" + content);

                    //重新登陆，必须回帖数等于账号可回最大回帖数
                    m_replyNum = m_replyMaxNum;

                    ChangeBaiduLogin();
                }
            }
        }

        private void LoginByCookies()
        {
            this.BeginInvoke(labledelegate, new object[] { labelWarning, "正在登陆百度账号..." });

            Cookie cookie = StringToCookies();

            if (cookie == null)
            {
                this.BeginInvoke(labledelegate, new object[] { labelWarning, "百度账号异常,正在更换百度账号..." });

                //更新账号异常
                //ClientPost.UpdateAccountAbnormal(m_loginName, 10000);

                LogManager.Instance.WriteLog(LogFileEnum.error, "异常:cookies格式不对");

                //重新登陆，必须回帖数等于账号可回最大回帖数
                m_replyNum = m_replyMaxNum;

                ChangeBaiduLogin();
            }
            else
            {
                CookieContainer cookieContainer = new CookieContainer();

                cookieContainer.Add(cookie);

                CookieHelper.m_cookieContainer = cookieContainer;

                BaiduLogin.m_codeString = string.Empty;

                BaiduLogin.m_verifyCode = string.Empty;

                this.BeginInvoke(labledelegate, new object[] { labelWarning, "百度账号登陆成功！" });

                m_replyNum = 0;

                BaiduLogin.m_isLogin = true;
            }
        }

        /// <summary>
        /// 把string转成cookie
        /// </summary>
        private Cookie StringToCookies()
        {
            Cookie cookie = null;

            string value = m_cookies.Replace("BDUSS=", "");

            if(!string.IsNullOrEmpty(value))
            {
                cookie = new Cookie();
                cookie.Name = "BDUSS";
                cookie.Value = value;
                cookie.Path = "/";
                cookie.Domain = ".baidu.com";
            }

            return cookie;
        }

        /// <summary>
        /// 回帖
        /// </summary>
        private void ReplyTieZi()
        {
            try
            {
                this.BeginInvoke(labledelegate, new object[] { labelWarning, "请等待" + m_replySeconds / 1000 + "秒,继续回帖" });

                System.Threading.Thread.Sleep(m_replySeconds);

                //从数据库中取一条需要回帖的记录
                //var json = ClientPost.GetTieZi();

                //if (Convert.ToInt32(json["Code"]) == ApiCode.Code200)
                //{
                //    Dictionary<string, object> jsonReply = JsonHelper.DeserializeObject<Dictionary<string, object>>(json["replyTaskInfo"].ToString());

                //    m_replyId = Convert.ToInt32(jsonReply["ID"]);

                //    if (m_replyId == 0)
                //    {
                //        this.BeginInvoke(labledelegate, new object[] { labelWarning, "后台没有待回帖的信息！" });

                //        this.BeginInvoke(labledelegate, new object[] { labelTotalNum, "0" });

                //        if (thread != null)
                //        {
                //            thread.Abort();
                //        }
                //    }
                //    else
                //    {
                //        m_replyPostData.Kw = jsonReply["TieBaName"].ToString();
                //        m_replyPostData.Fid = Convert.ToDecimal(jsonReply["TieBaId"]);
                //        m_replyPostData.Tid = Convert.ToDecimal(jsonReply["TieZiId"]);

                //        Random random = new Random();
                //        int index = random.Next(0, m_contentList.Count() - 1);

                //        m_replyPostData.Content = m_contentList[index];

                //        Reply();
                //    }
                //}
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.WriteLog(LogFileEnum.error, "异常ReplyTieZi：" + ex.ToString());
            }
        }

        /// <summary>
        /// 回复帖子
        /// </summary>
        public void Reply()
        {
            try
            {
                this.BeginInvoke(labledelegate, new object[] { labelWarning, "正在回帖..." });

                TieBaReply tieBaReply = new TieBaReply();

                int replyNo = tieBaReply.Reply(m_replyPostData);

                //回帖成功
                if (replyNo == 0)
                {
                    //更新回帖状态
                    //ClientPost.UpdateReply(m_replyId, m_userName);

                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "回帖成功！" });

                    //检查百度账号是否需要切换
                    m_replyNum++;

                    m_lableReplyNum++;
                    m_lableTotalNum--;

                    this.BeginInvoke(labledelegate, new object[] { labelReplyNum, m_lableReplyNum.ToString() });

                    this.BeginInvoke(labledelegate, new object[] { labelTotalNum, m_lableTotalNum.ToString() });

                    ChangeBaiduLogin();

                    //继续回帖
                    ReplyTieZi();
                }
                else if (replyNo == TieBaError.ErrNo40)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "请输入回帖验证码！" });

                    pictureBoxVcode2.ImageLocation = Vcode.GetVcodeUrl(TieBaReply.m_vcodeMd5);

                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }
                //ip地址被禁
                else if (replyNo == TieBaError.ErrNo13)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "IP地址被禁！" });

                    if (thread != null)
                    {
                        thread.Abort();
                    }
                }
                //回帖太频繁
                else if (replyNo == TieBaError.ErrNo34)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "回帖太频繁,请稍等！" });

                    ReplyTieZi();
                }
                //账号问题 判断是否完全未知
                else if (replyNo == TieBaError.ErrNo2 || replyNo == TieBaError.ErrNo8 || replyNo == TieBaError.ErrNo9 
                    || replyNo == TieBaError.ErrNo12 || replyNo == TieBaError.ErrNo19 || replyNo == TieBaError.ErrNo200 
                    || replyNo == TieBaError.ErrNo201 || replyNo == TieBaError.ErrNo202
                    || replyNo == TieBaError.ErrNo4010 || replyNo == TieBaError.ErrNo7001 || replyNo == TieBaError.ErrNo120016
                    || replyNo == TieBaError.ErrNo120021 || replyNo == TieBaError.ErrNo260005 
                    || replyNo == TieBaError.ErrNo100023 || replyNo == TieBaError.ErrNo400031 || replyNo == TieBaError.ErrNo400032)
                {
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "异常百度账号，正在更换百度账号..." });

                    //ClientPost.UpdateAccountAbnormal(m_loginName, replyNo);

                    m_replyNum = m_replyMaxNum;

                    ChangeBaiduLogin();

                    ReplyTieZi();
                }
                //其他错误
                else
                {
                    //写入回帖错误ID和尝试次数
                    this.BeginInvoke(labledelegate, new object[] { labelWarning, "异常回帖信息，正在更换回帖信息和登陆信息..." });

                    //ClientPost.UpdateReplyAbnormal(m_replyId, replyNo);

                    //为了误杀回帖的帖子，重新换账号尝试
                    m_replyNum = m_replyMaxNum;

                    ChangeBaiduLogin();

                    ReplyTieZi();
                }
            }
            catch (Exception ex)
            {
                LogManager.Instance.WriteLog(LogFileEnum.error, ex.ToString());
            }
        }

        /// <summary>
        /// 登陆验证码刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llLoginVcode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBoxVcode1.Text = "";

            if (string.IsNullOrEmpty(BaiduLogin.m_codeString))
            {
                pictureBoxVcode1.ImageLocation = "";
            }
            else
            {
                pictureBoxVcode1.ImageLocation = Vcode.GetLoginVcodeUrl(BaiduLogin.m_codeString);
                llLoginVcode.Visible = true;
            }
        }

        /// <summary>
        /// 登陆验证码登陆
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSubmit1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(BaiduLogin.m_codeString))
                return;

            BaiduLogin.m_verifyCode = textBoxVcode1.Text.Trim();

            if (string.IsNullOrEmpty(BaiduLogin.m_verifyCode))
            {
                MessageBox.Show("输入的登陆验证码不能为空！");
            }

            //提交完成后清空数据
            textBoxVcode1.Text = "";
            pictureBoxVcode1.ImageLocation = "";
            thread = new Thread(new ThreadStart(LoginAndReply));
            thread.IsBackground = true;
            thread.Start();
        }

        private void LoginAndReply()
        {
            Login();

            ReplyTieZi();
        }

        //关闭时，需要去除登陆的账号
        private void SinkTool_FormClosed(object sender, FormClosedEventArgs e)
        {
            //ClientPost.UpdateNoLogin(m_loginName);

            Application.ExitThread();
        }

        /// <summary>
        /// 帖子验证码(九宫格)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void llTieziVcode_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            textBoxVcode2.Text = "";
            TieBaReply.m_vcode = string.Empty;

            if (string.IsNullOrEmpty(TieBaReply.m_vcodeMd5))
            {
                pictureBoxVcode2.ImageLocation = "";
            }
            else
            {
                pictureBoxVcode2.ImageLocation = Vcode.GetVcodeUrl(TieBaReply.m_vcodeMd5);
            }
        }

        /// <summary>
        /// 提交九宫格验证码回帖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSubmit2_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TieBaReply.m_vcodeMd5))
                return;

            TieBaReply.m_vcode = textBoxVcode2.Text.Trim();

            if (string.IsNullOrEmpty(TieBaReply.m_vcode))
            {
                MessageBox.Show("输入的登陆验证码不能为空！");
            }

            //提交完成后清空数据
            textBoxVcode2.Text = "";
            pictureBoxVcode2.ImageLocation = "";

            thread = new Thread(new ThreadStart(Reply));
            thread.IsBackground = true;
            thread.Start();
        }

        /// <summary>
        /// 开始回帖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonReply_Click(object sender, EventArgs e)
        {
            m_replyNum = m_replyMaxNum;

            thread = new Thread(new ThreadStart(AutoLoginAndReply));
            thread.IsBackground = true;
            thread.Start();

            this.buttonReply.Enabled = false;
            this.buttonStop.Enabled = true;
        }

        /// <summary>
        /// 自动登陆并回帖
        /// </summary>
        private void AutoLoginAndReply()
        {
            ChangeBaiduLogin();

            if (BaiduLogin.m_isLogin)
            {
                ReplyTieZi();
            }
        }

        private void LableText(Label label, string text)
        {
            label.Text = text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图1;";
            TieBaReply.m_vcode += "00000000";
            AutoClick(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图2;";
            TieBaReply.m_vcode += "00010000";
            AutoClick(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图3;";
            TieBaReply.m_vcode += "00020000";
            AutoClick(sender, e);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图4;";
            TieBaReply.m_vcode += "00000001";
            AutoClick(sender, e);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图5;";
            TieBaReply.m_vcode += "00010001";
            AutoClick(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图6;";
            TieBaReply.m_vcode += "00020001";
            AutoClick(sender, e);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图7;";
            TieBaReply.m_vcode += "00000002";
            AutoClick(sender, e);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图8;";
            TieBaReply.m_vcode += "00010002";
            AutoClick(sender, e);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBoxVcode2.Text += "图9;";
            TieBaReply.m_vcode += "00020002";
            AutoClick(sender, e);
        }

        private void AutoClick(object sender, EventArgs e)
        {
            if (TieBaReply.m_vcode.Length == 32)
            {
                buttonSubmit2_Click(sender, e);
            }
        }

        /// <summary>
        /// 结束回帖
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonStop_Click(object sender, EventArgs e)
        {
            this.BeginInvoke(labledelegate, new object[] { labelWarning, "已结束回帖，如要继续回帖，请按开始回帖按钮。" });

            //ClientPost.UpdateNoLogin(m_loginName);

            if (thread != null)
            {
                thread.Abort();
            }

            this.buttonReply.Enabled = true;
            this.buttonStop.Enabled = false;
            textBoxVcode1.Text = "";
            textBoxVcode2.Text = "";
            pictureBoxVcode1.ImageLocation = "";
            pictureBoxVcode2.ImageLocation = "";
        }

        private void SinkTool_Load(object sender, EventArgs e)
        {
            // 登录验证输入框失去焦点事件
            textBoxVcode1.LostFocus += delegate { buttonSubmit1.Focus(); };
        }
    }
}
