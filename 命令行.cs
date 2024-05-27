using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace espjs_gui
{
    internal class 命令行
    {
        public delegate void 回调(string 进程);

        public static void 执行命令(string 命令, string 参数, 回调 输出回调函数, 回调 错误处理函数)
        {
            new Thread(() =>
            {
                Process 进程 = new();
                进程.StartInfo.FileName = 命令;
                进程.StartInfo.Arguments = 参数;
                进程.StartInfo.UseShellExecute = false;
                进程.StartInfo.WorkingDirectory = Path.GetDirectoryName(命令);
                进程.StartInfo.RedirectStandardError = true;
                进程.StartInfo.RedirectStandardOutput = true;
                进程.StartInfo.CreateNoWindow = true;
                进程.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        输出回调函数(e.Data);
                    }
                });
                进程.ErrorDataReceived += new DataReceivedEventHandler((sender, e) =>
                {
                    if (e.Data != null)
                    {
                        错误处理函数(e.Data);
                    }
                });
                进程.Start();
                进程.BeginOutputReadLine();
                进程.BeginErrorReadLine();
                进程.WaitForExit();
            }).Start();

        }

        public static string 执行命令并返回输出结果(string 命令, string 参数)
        {
            Process 进程 = new Process();
            进程.StartInfo.FileName = Path.GetFullPath(命令);
            进程.StartInfo.Arguments = 参数;
            进程.StartInfo.UseShellExecute = false;
            进程.StartInfo.RedirectStandardError = true;
            进程.StartInfo.RedirectStandardOutput = true;
            进程.StartInfo.CreateNoWindow = true;
            进程.Start();
            string 返回结果 = 进程.StandardOutput.ReadToEnd();
            string 错误 = 进程.StandardError.ReadToEnd();
            进程.WaitForExit();
            return 返回结果 + 错误;
        }


        public static void 烧录固件(string 端口, string 开发板类型, 回调 输出回调, 回调 错误回调)
        {
            配置 用户配置 = 配置.加载配置();
            string 当前目录 = AppDomain.CurrentDomain.BaseDirectory;
            string 命令 = 当前目录 + @"resources\esptool.exe";
            string 参数 = 用户配置.Flash[开发板类型].Replace("[port]", 端口);
            执行命令(命令, 参数, 输出回调, 错误回调);
        }

    }
}
