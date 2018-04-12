using System;
using System.IO;
using System.Text;
using TieBaRobot.Enum;

namespace TieBaRobot.Common
{
    public class LogManager
    {
        public static readonly LogManager Instance = new LogManager();
        private string _logPath = string.Empty;
        private string _logFielPrefix = string.Empty;
        private object _locker = new object();

        public string LogPath
        {
            get
            {
                if (_logPath == string.Empty)
                {
                    _logPath = AppDomain.CurrentDomain.BaseDirectory;
                }
                if (_logPath.Substring(_logPath.Length - 1) == "\\" || _logPath.Substring(_logPath.Length - 1) == "/")
                {
                    return _logPath;
                }
                else
                {
                    _logPath = _logPath + @"\";
                    return _logPath;
                }
            }

            set
            {
                _logPath = value;
            }
        }

        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public string LogFielPrefix
        {
            get
            {
                return _logFielPrefix;
            }
            set
            {
                _logFielPrefix = value;
            }
        }

        public void WriteLog(string LogFile, string msg, bool only, Encoding encoding, string fileExt)
        {
            lock (_locker)
            {
                var filePath = LogPath + LogFielPrefix + LogFile + " " + DateTime.Now.ToString("yyyyMMdd") + fileExt;
                if (!File.Exists(filePath))
                {
                    if (!Directory.Exists(LogPath))
                        Directory.CreateDirectory(LogPath);

                    File.Create(filePath).Close();
                }

                StreamWriter sw = new StreamWriter(filePath, true, encoding);

                if (only)
                {
                    sw.WriteLine(msg);
                }
                else
                {
                    sw.WriteLine(DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]: ") + msg);
                }
                sw.Close();
            }
        }

        public void WriteLog(string logFile, string msg)
        {
            WriteLog(logFile, msg, false);
        }

        public void WriteLog(LogFileEnum logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }

        public void WriteLog(string logFile, string msg, bool only)
        {
            WriteLog(logFile, msg, only, Encoding.GetEncoding("GB2312"), ".log");
        }
    }
}