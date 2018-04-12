namespace TieBaRobot.Entity
{
    public class ReplyPostData
    {
        /// <summary>
        /// 贴吧名称
        /// </summary>
        public string Kw { get; set; }

        /// <summary>
        /// 贴吧Id
        /// </summary>
        public decimal Fid { get; set; }

        /// <summary>
        /// 帖子Id
        /// </summary>
        public decimal Tid { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; }
    }
}
