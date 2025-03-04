using Microsoft.VisualBasic.Logging;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace espjs_gui
{
    public partial class 主窗口 : Form
    {
        public 主窗口()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void 主窗口_Load(object sender, EventArgs e)
        {
            日志文本框.Location = new Point(12, 75);
            刷新串口();
            if (选择串口.Items.Count >= 1)
            {
                选择串口.Text = 选择串口.Items[0].ToString();
            }
            else
            {
                选择串口.Text = "";
            }
            加载开发板配置();
            加载使用说明();
        }

        private void 加载使用说明()
        {
            if (File.Exists("使用说明.txt"))
            {
                日志文本框.Text = File.ReadAllText("使用说明.txt");
            }
        }

        private void 加载开发板配置()
        {
            配置 用户配置 = 配置.加载配置();
            foreach (var 开发板类型 in 用户配置.Flash)
            {
                开发板选择框.Items.Add(开发板类型.Key);
            }
        }

        private void 刷新串口()
        {
            选择串口.Items.Clear();
            foreach (string 串口号 in 串口助手.获取可用串口列表())
            {
                选择串口.Items.Add(串口号);
            }
        }

        private void 选择串口_DropDown(object sender, EventArgs e)
        {
            刷新串口();
        }

        private void 烧录固件按钮_Click(object sender, EventArgs e)
        {
            清空日志();
            if (选择串口.Text == "")
            {
                显示日志("请输入或选择串口号\r\n");
                return;
            }
            if (开发板选择框.Text == "")
            {
                显示日志("请输入或选择开发板类型\r\n");
                return;
            }
            命令行.烧录固件(选择串口.Text, 开发板选择框.Text, (string 输出) =>
            {
                显示日志(输出 + "\r\n");
            }, (错误输出) =>
            {
                显示日志(错误输出 + "\r\n");
            });
        }

        private void 清空日志()
        {
            日志文本框.Text = "";
        }

        public void 显示日志(string 日志)
        {
            if (Thread.CurrentThread.ManagedThreadId != 1) // 确保使用相同的线程 ID
            {
                this.Invoke((MethodInvoker)delegate
                {
                    显示日志(日志);
                });
            }
            else
            {
                // 在当前线程上操作控件
                日志文本框.Text += 日志;
                // 滚动到最下面
                日志文本框.SelectionStart = 日志文本框.Text.Length;
                日志文本框.ScrollToCaret();
            }
        }

        private void 清除设备代码按钮_Click(object sender, EventArgs e)
        {
            清空日志();
            var 串口号 = 选择串口.Text;
            if (串口号 == "")
            {
                显示日志("请输入或选择串口号\r\n");
                return;
            }
            if (开发模式串口 != null)
            {
                开发模式串口.WriteLine("require('Storage').eraseAll();E.reboot();");
            }
            else
            {
                串口助手.清除代码(串口号, (string 输出) =>
                {
                    显示日志(输出 + "\r\n");
                });
            }

        }

        private void 项目选择框_DropDown(object sender, EventArgs e)
        {
            项目选择框.Items.Clear();
            if (!File.Exists("项目目录.txt"))
            {
                return;
            }
            string[] 所有项目目录 = File.ReadAllLines("项目目录.txt");
            Array.Reverse(所有项目目录);
            foreach (string 目录 in 所有项目目录)
            {
                if (目录 != "")
                {
                    项目选择框.Items.Add(目录);
                }
            }
        }

        private void 选择目录按钮_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog 文件夹选择框 = new FolderBrowserDialog();
            // 文件夹选择框.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (文件夹选择框.ShowDialog() == DialogResult.OK)
            {
                项目选择框.Text = 文件夹选择框.SelectedPath;
                string[] 目录 = Array.Empty<string>();
                if (File.Exists("项目目录.txt"))
                {
                    目录 = File.ReadAllLines("项目目录.txt");
                    var 列表 = 目录.ToList();
                    列表.Add(文件夹选择框.SelectedPath);
                    目录 = 列表.ToArray();
                }
                else
                {
                    目录 = new string[] { 文件夹选择框.SelectedPath };
                }
                string 内容 = String.Join("\r\n", 目录.Distinct().ToArray());
                File.WriteAllText("项目目录.txt", 内容);
            }
        }

        private async void 写入设备按钮_Click(object sender, EventArgs e)
        {
            清空日志();
            var 用户配置 = 配置.加载配置();
            var 串口号 = 选择串口.Text;
            var 开发板 = 开发板选择框.Text;
            //var 波特率 = 用户配置.提取波特率(开发板);
            var 波特率 = 115200;
            var 项目地址 = 项目选择框.Text;
            if (串口号 == "")
            {
                显示日志("请输入或选择串口号\r\n");
                return;
            }
            if (开发板 == "")
            {
                显示日志("请输入或选择开发板类型\r\n");
                return;
            }
            if (项目地址 == "")
            {
                显示日志("请输入或选择项目目录\r\n");
                return;
            }


            显示日志("正在检测固件...\r\n");
            var 检测结果 = await 串口助手.检测固件是否正常(串口号, 波特率);
            if (!检测结果)
            {
                显示日志("没有检测到固件,请先烧录固件\r\n");
                return;
            }
            显示日志("固件检测已通过\r\n");

            if (项目地址.StartsWith("http"))
            {
                显示日志("开始下载代码...\r\n");
                var 代码 = await 网络助手.获取网页源代码(项目地址.ToString());
                显示日志("下载代码完成.\r\n");
                显示日志("正在写入设备...\r\n");
                串口助手.写入文件(串口号, 波特率, ".bootcde", 代码, (消息) =>
                {
                    显示日志(消息 + "\r\n");
                });
                显示日志("文件写入完成.\r\n");
                return;
            }

            new Thread(async () =>
            {
                if (!项目地址.EndsWith(@"\"))
                {
                    项目地址 += @"\";
                }
                await 串口助手.将目录写入设备(串口号, 波特率, 项目地址, (消息) =>
                {
                    显示日志(消息 + "\r\n");
                });
            }).Start();

        }

        private void 代码输入框_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                发送代码按钮_Click(sender, e);
            }
        }

        private void 发送代码按钮_Click(object sender, EventArgs e)
        {
            var 用户配置 = 配置.加载配置();
            var 串口号 = 选择串口.Text;
            var 开发板 = 开发板选择框.Text;
            //var 波特率 = 用户配置.提取波特率(开发板);
            var 波特率 = 115200;
            if (串口号 == "")
            {
                显示日志("请输入或选择串口号\r\n");
                return;
            }
            var 代码 = 代码输入框.Text;
            显示日志("> " + 代码 + "\r\n");

            if (开发模式串口 != null)
            {
                开发模式串口.WriteLine(代码);
            }
            else
            {
                Task.Run(async () =>
                {
                    var 代码执行结果 = await 串口助手.获取代码的执行结果(串口号, 波特率, 代码);
                    显示日志(代码执行结果 + "\r\n");
                });
            }
            代码输入框.Text = "";
        }

        private void 重启设备按钮_Click(object sender, EventArgs e)
        {
            var 串口号 = 选择串口.Text;
            var 波特率 = 115200;
            var 代码 = "E.reboot();";
            显示日志("\r\n" + "发送重启命令: " + 代码 + "\r\n");
            if (开发模式串口 != null)
            {
                开发模式串口.WriteLine(代码);
                Task.Run(() => {
                    Thread.Sleep(1000);
                    开发模式串口.WriteLine("echo(false);");
                });
            }
            else
            {
                Task.Run(async () =>
                {
                    var 代码执行结果 = await 串口助手.获取代码的执行结果(串口号, 波特率, 代码, 500);
                    显示日志(代码执行结果 + "\r\n");
                });
            }

        }

        private SerialPort? 开发模式串口;
        private FileSystemWatcher? 项目文件监听器;
        private Queue 变化的文件 = new Queue();
        public System.Timers.Timer 写入文件定时器 = new System.Timers.Timer();
        private void 开发模式按钮_Click(object sender, EventArgs e)
        {
            if (开发模式按钮.Text == "开发模式")
            {
                var 串口号 = 选择串口.Text;
                清空日志();
                if (串口号 == "")
                {
                    显示日志("未检测到设备!");
                    return;
                }
                if (项目选择框.Text == "")
                {
                    显示日志("请选择或输入项目目录!");
                    return;
                }
                if (!Directory.Exists(项目选择框.Text))
                {
                    显示日志("项目目录不存在!");
                    return;
                }
                进入开发模式();
            }
            else
            {
                取消开发模式();
            }
        }

        private void 进入开发模式()
        {
            变化的文件.Clear();
            项目文件监听器 = new FileSystemWatcher
            {
                Path = 项目选择框.Text,
                IncludeSubdirectories = true,//全局文件监控，包括子目录
                EnableRaisingEvents = true   //启用文件监控
            };
            项目文件监听器.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs e) =>
            {
                if (热更新复选框.Checked)
                {
                    if (!变化的文件.Contains(e.Name))
                    {
                        变化的文件.Enqueue(e.Name);
                    }
                }
            });

            写入文件定时器.Interval = 1000;
            写入文件定时器.AutoReset = true;
            写入文件定时器.Elapsed += 将变化的文件写入设备;
            写入文件定时器.Start();
            选择串口.Enabled = false;
            开发板选择框.Enabled = false;
            项目选择框.Enabled = false;
            写入设备按钮.Enabled = false;
            烧录固件按钮.Enabled = false;
            选择目录按钮.Enabled = false;
            热更新复选框.Visible = true;
            更新后自动重启复选框.Visible = true;
            日志文本框.Location = new Point(12, 100);
            开发模式按钮.Text = "取消";
            开发模式串口 = 串口助手.监听串口数据(选择串口.Text, 115200, (string 数据) =>
            {
                if (数据.Trim() == "E.reboot();")
                {
                    return;
                }
                if (数据.Trim() == "echo(false);")
                {
                    return;
                }
                显示日志(数据.Trim() + "\r\n");
            });
            开发模式按钮.Text = "取消";

            if (开发模式串口 == null)
            {
                显示日志("串口失败!" + "\r\n");
                显示日志("进入开发模式失败!" + "\r\n");
                取消开发模式();
            }
            else
            {
                串口助手.关闭代码回显(开发模式串口);
                显示日志("进入开发模式" + "\r\n");
            }

        }

        private void 取消开发模式()
        {
            if (项目文件监听器 != null)
            {
                项目文件监听器.EnableRaisingEvents = false;
                项目文件监听器.Dispose();
                项目文件监听器 = null;
            }
            写入文件定时器.Stop();

            选择串口.Enabled = true;
            开发板选择框.Enabled = true;
            项目选择框.Enabled = true;
            烧录固件按钮.Enabled = true;
            选择目录按钮.Enabled = true;
            写入设备按钮.Enabled = true;
            热更新复选框.Visible = false;
            更新后自动重启复选框.Visible = false;
            日志文本框.Location = new Point(12, 75);
            开发模式按钮.Text = "开发模式";
            if (开发模式串口 != null && 开发模式串口.IsOpen)
            {
                开发模式串口.Close();
                开发模式串口 = null;
            }
            显示日志("开发模式已关闭!");
        }

        private void 热更新复选框_CheckedChanged(object sender, EventArgs e)
        {
            if (热更新复选框.Checked)
            {
                显示日志("启用热更新!\r\n");
            }
            else
            {
                显示日志("关闭热更新!\r\n");
            }
        }

        private void 更新后自动重启复选框_CheckedChanged(object sender, EventArgs e)
        {
            if (更新后自动重启复选框.Checked)
            {
                显示日志("启用更新后自动重启!\r\n");
            }
            else
            {
                显示日志("关闭更新后自动重启!\r\n");
            }
        }

        private void 将变化的文件写入设备(object? sender, ElapsedEventArgs e)
        {
            if (变化的文件.Count == 0)
            {
                return;
            }
            var 文件名 = 变化的文件.Dequeue().ToString();
            string 完整文件名 = 项目选择框.Text + @"\" + 文件名;
            文件名 = 文件名.Replace("\\", "/");

            if (文件名 == "index.js" || 文件名 == "main.js")
            {
                文件名 = ".bootcde";
            }
            string 代码 = File.ReadAllText(完整文件名);
            if (开发模式串口 != null && 开发模式串口.IsOpen)
            {
                串口助手.关闭代码回显(开发模式串口);
                显示日志("文件改动: " + 文件名 + "\r\n");
                显示日志("正在写入文件: " + 文件名 + "\r\n");
                try
                {
                    串口助手.写入文件(开发模式串口, 文件名, 代码);
                    if (变化的文件.Count == 0 && 更新后自动重启复选框.Checked)
                    {
                        串口助手.重启设备(开发模式串口);
                    }
                }
                catch (Exception)
                {
                    显示日志("写入文件出错: " + 文件名 + "\r\n");
                    取消开发模式();
                    return;
                    //throw;
                }

            }
            显示日志("文件写入完成: " + 文件名 + "\r\n");

        }
    }
}