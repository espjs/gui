using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void 将目录写入设备(string 端口, string 项目目录)
        {

        }

        public static void 开发模式(string 端口, string 项目目录)
        {

        }

        public static void 发送代码(string 端口, string 代码)
        {

        }

    }
}
