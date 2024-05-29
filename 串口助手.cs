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
            catch (Exception)
            {
                return null;
            }
            return 串口;
        }

        public static void 清除代码(string 端口, string 开发板类型, 回调 回调函数)
        {
            配置 用户配置 = 配置.加载配置();
            int 波特率 = 用户配置.提取波特率(开发板类型);
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

        public static void 将目录写入设备(string 端口, int 波特率, string 项目目录, 回调 消息回调)
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
                写入文件(串口, 相对路径, 内容);
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

        public static void 开发模式(string 端口, string 项目目录)
        {

        }

        public static void 发送代码(string 端口, string 代码)
        {

        }

    }
}
