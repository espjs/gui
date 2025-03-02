using System.Collections;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

namespace espjs_gui
{
    internal class 串口助手
    {
        public delegate void 回调(string 输出);
        public static string[] 获取可用串口列表()
        {
            return SerialPort.GetPortNames();
        }

        public static string 获取一个可用的串口()
        {
            string[] 所有串口 = 获取可用串口列表();
            return 所有串口.Length >= 1 ? 所有串口[所有串口.Length - 1] : "";
        }

        public static bool 有可用串口()
        {
            string[] 列表 = 获取可用串口列表();
            return 列表.Length >= 1;
        }

        public static byte[] 字符串转字节(string 字符串)
        {
            string[] 字符串数组 = 字符串.Trim().Split(' ');
            byte[] 返回值 = new byte[字符串数组.Length];
            //逐个字符变为16进制字节数据
            for (int i = 0; i < 字符串数组.Length; i++)
            {
                返回值[i] = Convert.ToByte(字符串数组[i], 16);
            }
            //按照指定编码将字节数组变为字符串
            return 返回值;
        }

        public static SerialPort? 打开串口(string 端口, int 波特率)
        {
            SerialPort 串口 = new()
            {
                PortName = 端口,
                BaudRate = 波特率,
                DataBits = 8
            };
            try
            {
                串口.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
            return 串口;
        }

        public static void 清除代码(string 端口, string 开发板类型, 回调 回调函数)
        {
            配置 用户配置 = 配置.加载配置();
            //int 波特率 = 用户配置.提取波特率(开发板类型);
            int 波特率 = 115200;
            new Thread(() =>
            {

                var 串口 = 打开串口(端口, 波特率);
                if (串口 == null)
                {
                    回调函数("端口被占用: " + 端口);
                    return;
                }
                串口.DataReceived += new SerialDataReceivedEventHandler((object sender, SerialDataReceivedEventArgs e) =>
                {
                    if (!串口.IsOpen)
                    {
                        return;
                    }
                    try
                    {
                        回调函数(串口.ReadLine());
                    }
                    catch (Exception)
                    {

                    }
                });
                串口.WriteLine("require('Storage').eraseAll();E.reboot();");
                Thread.Sleep(2000);
                回调函数("清除代码完成!");
                串口.Close();
            }).Start();

        }

        public static async Task 将目录写入设备(string 端口, int 波特率, string 项目目录, 回调 消息回调)
        {
            // 检测目录是否存在
            if (!Directory.Exists(项目目录))
            {
                消息回调("项目目录不存在!");
                return;
            }

            if (!File.Exists(项目目录 + @"\index.js"))
            {
                消息回调("提示: 项目目录没有检测到[index.js] 可能导致设备无法工作!");
            }

            消息回调("正在检测单片机中内置的模块...\r\n");
            var 模块列表 = await 串口助手.获取固件中内置的模块(端口, 波特率);
            消息回调("已内置模块: " + 模块列表 + "\r\n");
            var 内置模块 = 模块列表.Split(",");
            var 提取到的模块 = new List<string>();

            // 检测端口是否占用
            var 串口 = 打开串口(端口, 波特率);

            if (串口 == null)
            {
                消息回调("串口" + 端口 + "打开失败, 可能被占用了! ");
                return;
            }

            写入启动文件(串口);

            // 读取所有文件
            var 文件列表 = Directory.GetFiles(项目目录);
            var 忽略文件 = new ArrayList();
            var 默认配置 = 配置.加载配置();
            foreach (var 文件名 in 默认配置.Ignore)
            {
                忽略文件.Add(文件名);
            }
            if (File.Exists(项目目录 + @"\ignore.txt"))
            {
                var 用户忽略文件 = File.ReadLines(项目目录 + @"\ignore.txt", Encoding.UTF8).ToArray();
                foreach (var 文件名 in 用户忽略文件)
                {
                    忽略文件.Add(文件名);
                }
            }
            foreach (var 文件路径 in 文件列表)
            {
                var 忽略当前文件 = false;
                var 相对路径 = 文件路径.Replace(项目目录, "").Replace(@"\", "/");
                foreach (string 文件名 in 忽略文件)
                {
                    if (相对路径.StartsWith(文件名.ToString()))
                    {
                        忽略当前文件 = true;
                        break;
                    }
                }
                if (忽略当前文件)
                {
                    continue;
                }

                消息回调("正在写入文件: " + 相对路径);
                var 内容 = File.ReadAllText(文件路径, Encoding.UTF8);
                var 当前文件用到的模块 = 提取模块(内容);
                foreach (var 模块 in 当前文件用到的模块)
                {
                    if (内置模块.Contains(模块))
                    {
                        // 内置模块不处理
                        continue;
                    }
                    if (File.Exists(项目目录 + 模块))
                    {
                        // 自己写的模块也不处理 
                        continue;
                    }
                    // 最后应该剩下在线模块, 这里先加入列表, 后面一起处理
                    提取到的模块.Add(模块);
                }
                写入文件(串口, 相对路径, 内容);
            }
            // 提取到的模块去重
            提取到的模块 = 提取到的模块.Distinct().ToList();

            foreach (var 模块 in 提取到的模块)
            {
                if (模块.StartsWith("http:") || 模块.StartsWith("https:"))
                {
                    消息回调("暂不支持加载远程模块:  " + 模块);
                    continue;
                }
                var 模块代码 = await 网络助手.获取模块源代码(模块);
                写入文件(串口, 模块, 模块代码);
                消息回调("模块 " + 模块 + " 写入完成! ");
            }
            消息回调("全部文件写入完成! ");
            重启设备(串口);
            串口.Close();

        }

        public static void 写入启动文件(SerialPort 串口)
        {
            写入文件(串口, ".bootcde", "require('index.js')");
        }

        public static void 重启设备(SerialPort 串口)
        {
            串口.WriteLine("E.reboot();");
        }

        public static void 写入文件(SerialPort 串口, string 文件名, string 文件内容)
        {
            var 代码块列表 = 将内容转换成代码块(文件名, 文件内容);
            foreach (string 代码块 in 代码块列表)
            {
                串口.WriteLine(代码块);
                Thread.Sleep(100);
            }
        }

        public static void 写入文件(string 端口, int 波特率, string 文件名, string 文件内容, 回调 消息回调)
        {
            var 串口 = 打开串口(端口, 波特率);
            if (串口 == null)
            {
                消息回调("串口" + 端口 + "打开失败, 可能被占用了! ");
                return;
            }
            写入文件(串口, 文件名, 文件内容);
        }

        public static int 获取字符串长度(string 字符串)
        {
            int 中文个数 = 0;
            Regex 中文正则 = new Regex(@"^[\u4E00-\u9FA5]{1,}$");
            for (int i = 0; i < 字符串.Length; i++)
            {
                if (中文正则.IsMatch(字符串[i].ToString()))
                {
                    中文个数++;
                }
            }

            return 字符串.Length + 中文个数 * 2;
        }

        public static string Btoa(string value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
        }

        public static ArrayList 将内容转换成代码块(string 文件名, string 代码)
        {
            var 代码分割块长度 = 30;
            代码 = new Regex("\r").Replace(代码, "");
            代码 = new Regex("\n +").Replace(代码, "\n");
            代码 = 代码.Trim();

            int 代码总长度 = 获取字符串长度(代码);
            文件名 = 文件名.Trim();
            ArrayList 返回值 = new ArrayList
            {
                "global._f=require('Storage');",
                "(_=>{global._c=_=>{if(_f.getFree()<1000){console.log('Storage overflow');return false;}return true;}})();",
                "(_=>{global._w=function(code,offset){_c()&&global._f.write('" + 文件名 + "',code,offset)}})();",
            };

            // 这里设置每次写入的代码长度
            if (代码.Length <= 代码分割块长度)
            {
                返回值.Add("_f.write('" + 文件名 + "',atob('" + Btoa(代码) + "'));");
            }
            else
            {
                int 当前位置 = 0;
                int 字符串长度 = 0;
                string 代码块 = 代码.Substring(当前位置, 代码分割块长度);
                int 代码块长度 = 获取字符串长度(代码块);
                返回值.Add("_f.write('" + 文件名 + "',atob('" + Btoa(代码块) + "'),0," + 代码总长度 + ");");
                当前位置 += 代码分割块长度;
                字符串长度 += 代码块长度;
                while (true)
                {
                    if (代码.Length >= 当前位置 + 代码分割块长度)
                    {
                        代码块 = 代码.Substring(当前位置, 代码分割块长度);
                        代码块长度 = 获取字符串长度(代码块);
                        返回值.Add("_w(atob('" + Btoa(代码块) + "')," + 字符串长度 + ");");
                        当前位置 += 代码分割块长度;
                        字符串长度 += 代码块长度;
                    }
                    else
                    {
                        代码块 = 代码.Substring(当前位置);
                        返回值.Add("_w(atob('" + Btoa(代码块) + "')," + 字符串长度 + ");");
                        break;
                    }

                }
            }
            return 返回值;
        }

        public static async Task<bool>? 检测固件是否正常(string 端口, int 波特率)
        {
            var 检测结果 = await 获取代码的执行结果(端口, 波特率, "console.log('hello')");
            if (检测结果.Trim() == "hello")
            {
                return true;
            }
            return false;
        }

        public static async Task<string> 获取代码的执行结果(string 端口, int 波特率, string 代码)
        {
            return await Task.Run(() =>
            {
                var 串口 = 打开串口(端口, 波特率);
                if (串口 == null)
                {
                    return "";
                }
                var 返回结果 = "";
                串口.DataReceived += new SerialDataReceivedEventHandler((sender, e) =>
                   {
                       if (!串口.IsOpen)
                       {
                           return;
                       }
                       try
                       {
                           var 数据 = 串口.ReadLine().Trim();
                           if (数据 == 代码 || 数据 == "" || 数据 == "=undefined")
                           {
                               return;
                           }
                           else
                           {
                               返回结果 = 返回结果.Trim() + "\r\n" + 数据;
                           }
                       }
                       catch (Exception)
                       {

                       }

                   });
                串口.WriteLine(代码);
                Thread.Sleep(1000);
                串口.Close();
                return 返回结果.Trim();
            });
        }

        public static async Task<string> 获取固件中内置的模块(string 串口号, int 波特率)
        {
            var 代码 = "console.log(process.env.MODULES)";
            string 单片机返回结果 = await 获取代码的执行结果(串口号, 波特率, 代码);
            return 单片机返回结果;
        }

        public static string[] 提取模块(string 代码)
        {
            var 正则 = new Regex("require\\(['\"](.*?)['\"]\\)");
            var 匹配结果 = 正则.Matches(代码);
            var 模块列表 = new List<string>();
            foreach (var 匹配结果项 in 匹配结果)
            {
                var 模块名 = 匹配结果项.ToString()
                    .Replace("require('", "")
                    .Replace("')", "")
                    .Replace("require(\"", "")
                    .Replace("\")", "");
                if (!模块列表.Contains(模块名))
                {
                    模块列表.Add(模块名);
                }
            }
            return 模块列表.ToArray();
        }

        public static async void 下载并写入模块(SerialPort 串口, string[] 模块列表)
        {
            var 默认配置 = 配置.加载配置();
            var 模块地址 = 默认配置.Modules;

            foreach (var 模块 in 模块列表)
            {
                var 下载地址 = 模块地址.Replace("[name]", 模块);
                if (模块.StartsWith("http:") || 模块.StartsWith("https:"))
                {
                    下载地址 = 模块;
                }
                else if (模块.EndsWith(".js"))
                {
                    continue;
                }
                if (缓存助手.缓存存在(下载地址))
                {
                    写入文件(串口, 模块, 缓存助手.读取缓存(下载地址));
                }
                else
                {
                    var 模块代码 = await 网络助手.获取网页源代码(下载地址);
                    写入文件(串口, 模块, 模块代码);
                    缓存助手.写入缓存(下载地址, 模块代码);
                }
            }

        }

        public static void 开发模式(string 端口, string 项目目录)
        {

        }

        public static void 发送代码(string 端口, string 代码)
        {

        }

    }
}
