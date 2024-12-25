using Microsoft.VisualBasic.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace espjs_gui
{
    public partial class 主窗口 : Form
    {
        public 主窗口()
        {
            InitializeComponent();
        }

        private void 主窗口_Load(object sender, EventArgs e)
        {
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
            串口助手.清除代码(选择串口.Text, 开发板选择框.Text, (string 输出) =>
            {
                显示日志(输出 + "\r\n");
            });
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
            var 波特率 = 用户配置.提取波特率(开发板);
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
            var 波特率 = 用户配置.提取波特率(开发板);
            if (串口号 == "")
            {
                显示日志("请输入或选择串口号\r\n");
                return;
            }
            var 代码 = 代码输入框.Text;
            显示日志(代码 + "\r\n");
            Task.Run(async () =>
            {
                var 代码执行结果 = await 串口助手.获取代码的执行结果(串口号, 波特率, 代码);
                显示日志(代码执行结果 + "\r\n");
            });
            代码输入框.Text = "";
        }
    }
}