using Microsoft.VisualBasic.Logging;
using System.Collections;
using System.IO.Ports;
using System.Timers;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace espjs_gui
{
    public partial class ������ : Form
    {
        public ������()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void ������_Load(object sender, EventArgs e)
        {
            ��־�ı���.Location = new Point(12, 75);
            ˢ�´���();
            if (ѡ�񴮿�.Items.Count >= 1)
            {
                ѡ�񴮿�.Text = ѡ�񴮿�.Items[0].ToString();
            }
            else
            {
                ѡ�񴮿�.Text = "";
            }
            ���ؿ���������();
            ����ʹ��˵��();
        }

        private void ����ʹ��˵��()
        {
            if (File.Exists("ʹ��˵��.txt"))
            {
                ��־�ı���.Text = File.ReadAllText("ʹ��˵��.txt");
            }
        }

        private void ���ؿ���������()
        {
            ���� �û����� = ����.��������();
            foreach (var ���������� in �û�����.Flash)
            {
                ������ѡ���.Items.Add(����������.Key);
            }
        }

        private void ˢ�´���()
        {
            ѡ�񴮿�.Items.Clear();
            foreach (string ���ں� in ��������.��ȡ���ô����б�())
            {
                ѡ�񴮿�.Items.Add(���ں�);
            }
        }

        private void ѡ�񴮿�_DropDown(object sender, EventArgs e)
        {
            ˢ�´���();
        }

        private void ��¼�̼���ť_Click(object sender, EventArgs e)
        {
            �����־();
            if (ѡ�񴮿�.Text == "")
            {
                ��ʾ��־("�������ѡ�񴮿ں�\r\n");
                return;
            }
            if (������ѡ���.Text == "")
            {
                ��ʾ��־("�������ѡ�񿪷�������\r\n");
                return;
            }
            ������.��¼�̼�(ѡ�񴮿�.Text, ������ѡ���.Text, (string ���) =>
            {
                ��ʾ��־(��� + "\r\n");
            }, (�������) =>
            {
                ��ʾ��־(������� + "\r\n");
            });
        }

        private void �����־()
        {
            ��־�ı���.Text = "";
        }

        public void ��ʾ��־(string ��־)
        {
            if (Thread.CurrentThread.ManagedThreadId != 1) // ȷ��ʹ����ͬ���߳� ID
            {
                this.Invoke((MethodInvoker)delegate
                {
                    ��ʾ��־(��־);
                });
            }
            else
            {
                // �ڵ�ǰ�߳��ϲ����ؼ�
                ��־�ı���.Text += ��־;
                // ������������
                ��־�ı���.SelectionStart = ��־�ı���.Text.Length;
                ��־�ı���.ScrollToCaret();
            }
        }

        private void ����豸���밴ť_Click(object sender, EventArgs e)
        {
            �����־();
            var ���ں� = ѡ�񴮿�.Text;
            if (���ں� == "")
            {
                ��ʾ��־("�������ѡ�񴮿ں�\r\n");
                return;
            }
            if (����ģʽ���� != null)
            {
                ����ģʽ����.WriteLine("require('Storage').eraseAll();E.reboot();");
            }
            else
            {
                ��������.�������(���ں�, (string ���) =>
                {
                    ��ʾ��־(��� + "\r\n");
                });
            }

        }

        private void ��Ŀѡ���_DropDown(object sender, EventArgs e)
        {
            ��Ŀѡ���.Items.Clear();
            if (!File.Exists("��ĿĿ¼.txt"))
            {
                return;
            }
            string[] ������ĿĿ¼ = File.ReadAllLines("��ĿĿ¼.txt");
            Array.Reverse(������ĿĿ¼);
            foreach (string Ŀ¼ in ������ĿĿ¼)
            {
                if (Ŀ¼ != "")
                {
                    ��Ŀѡ���.Items.Add(Ŀ¼);
                }
            }
        }

        private void ѡ��Ŀ¼��ť_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog �ļ���ѡ��� = new FolderBrowserDialog();
            // �ļ���ѡ���.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (�ļ���ѡ���.ShowDialog() == DialogResult.OK)
            {
                ��Ŀѡ���.Text = �ļ���ѡ���.SelectedPath;
                string[] Ŀ¼ = Array.Empty<string>();
                if (File.Exists("��ĿĿ¼.txt"))
                {
                    Ŀ¼ = File.ReadAllLines("��ĿĿ¼.txt");
                    var �б� = Ŀ¼.ToList();
                    �б�.Add(�ļ���ѡ���.SelectedPath);
                    Ŀ¼ = �б�.ToArray();
                }
                else
                {
                    Ŀ¼ = new string[] { �ļ���ѡ���.SelectedPath };
                }
                string ���� = String.Join("\r\n", Ŀ¼.Distinct().ToArray());
                File.WriteAllText("��ĿĿ¼.txt", ����);
            }
        }

        private async void д���豸��ť_Click(object sender, EventArgs e)
        {
            �����־();
            var �û����� = ����.��������();
            var ���ں� = ѡ�񴮿�.Text;
            var ������ = ������ѡ���.Text;
            //var ������ = �û�����.��ȡ������(������);
            var ������ = 115200;
            var ��Ŀ��ַ = ��Ŀѡ���.Text;
            if (���ں� == "")
            {
                ��ʾ��־("�������ѡ�񴮿ں�\r\n");
                return;
            }
            if (������ == "")
            {
                ��ʾ��־("�������ѡ�񿪷�������\r\n");
                return;
            }
            if (��Ŀ��ַ == "")
            {
                ��ʾ��־("�������ѡ����ĿĿ¼\r\n");
                return;
            }


            ��ʾ��־("���ڼ��̼�...\r\n");
            var ����� = await ��������.���̼��Ƿ�����(���ں�, ������);
            if (!�����)
            {
                ��ʾ��־("û�м�⵽�̼�,������¼�̼�\r\n");
                return;
            }
            ��ʾ��־("�̼������ͨ��\r\n");

            if (��Ŀ��ַ.StartsWith("http"))
            {
                ��ʾ��־("��ʼ���ش���...\r\n");
                var ���� = await ��������.��ȡ��ҳԴ����(��Ŀ��ַ.ToString());
                ��ʾ��־("���ش������.\r\n");
                ��ʾ��־("����д���豸...\r\n");
                ��������.д���ļ�(���ں�, ������, ".bootcde", ����, (��Ϣ) =>
                {
                    ��ʾ��־(��Ϣ + "\r\n");
                });
                ��ʾ��־("�ļ�д�����.\r\n");
                return;
            }

            new Thread(async () =>
            {
                if (!��Ŀ��ַ.EndsWith(@"\"))
                {
                    ��Ŀ��ַ += @"\";
                }
                await ��������.��Ŀ¼д���豸(���ں�, ������, ��Ŀ��ַ, (��Ϣ) =>
                {
                    ��ʾ��־(��Ϣ + "\r\n");
                });
            }).Start();

        }

        private void ���������_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ���ʹ��밴ť_Click(sender, e);
            }
        }

        private void ���ʹ��밴ť_Click(object sender, EventArgs e)
        {
            var �û����� = ����.��������();
            var ���ں� = ѡ�񴮿�.Text;
            var ������ = ������ѡ���.Text;
            //var ������ = �û�����.��ȡ������(������);
            var ������ = 115200;
            if (���ں� == "")
            {
                ��ʾ��־("�������ѡ�񴮿ں�\r\n");
                return;
            }
            var ���� = ���������.Text;
            ��ʾ��־("> " + ���� + "\r\n");

            if (����ģʽ���� != null)
            {
                ����ģʽ����.WriteLine(����);
            }
            else
            {
                Task.Run(async () =>
                {
                    var ����ִ�н�� = await ��������.��ȡ�����ִ�н��(���ں�, ������, ����);
                    ��ʾ��־(����ִ�н�� + "\r\n");
                });
            }
            ���������.Text = "";
        }

        private void �����豸��ť_Click(object sender, EventArgs e)
        {
            var ���ں� = ѡ�񴮿�.Text;
            var ������ = 115200;
            var ���� = "E.reboot();";
            ��ʾ��־("\r\n" + "������������: " + ���� + "\r\n");
            if (����ģʽ���� != null)
            {
                ����ģʽ����.WriteLine(����);
                Task.Run(() => {
                    Thread.Sleep(1000);
                    ����ģʽ����.WriteLine("echo(false);");
                });
            }
            else
            {
                Task.Run(async () =>
                {
                    var ����ִ�н�� = await ��������.��ȡ�����ִ�н��(���ں�, ������, ����, 500);
                    ��ʾ��־(����ִ�н�� + "\r\n");
                });
            }

        }

        private SerialPort? ����ģʽ����;
        private FileSystemWatcher? ��Ŀ�ļ�������;
        private Queue �仯���ļ� = new Queue();
        public System.Timers.Timer д���ļ���ʱ�� = new System.Timers.Timer();
        private void ����ģʽ��ť_Click(object sender, EventArgs e)
        {
            if (����ģʽ��ť.Text == "����ģʽ")
            {
                var ���ں� = ѡ�񴮿�.Text;
                �����־();
                if (���ں� == "")
                {
                    ��ʾ��־("δ��⵽�豸!");
                    return;
                }
                if (��Ŀѡ���.Text == "")
                {
                    ��ʾ��־("��ѡ���������ĿĿ¼!");
                    return;
                }
                if (!Directory.Exists(��Ŀѡ���.Text))
                {
                    ��ʾ��־("��ĿĿ¼������!");
                    return;
                }
                ���뿪��ģʽ();
            }
            else
            {
                ȡ������ģʽ();
            }
        }

        private void ���뿪��ģʽ()
        {
            �仯���ļ�.Clear();
            ��Ŀ�ļ������� = new FileSystemWatcher
            {
                Path = ��Ŀѡ���.Text,
                IncludeSubdirectories = true,//ȫ���ļ���أ�������Ŀ¼
                EnableRaisingEvents = true   //�����ļ����
            };
            ��Ŀ�ļ�������.Changed += new FileSystemEventHandler((object sender, FileSystemEventArgs e) =>
            {
                if (�ȸ��¸�ѡ��.Checked)
                {
                    if (!�仯���ļ�.Contains(e.Name))
                    {
                        �仯���ļ�.Enqueue(e.Name);
                    }
                }
            });

            д���ļ���ʱ��.Interval = 1000;
            д���ļ���ʱ��.AutoReset = true;
            д���ļ���ʱ��.Elapsed += ���仯���ļ�д���豸;
            д���ļ���ʱ��.Start();
            ѡ�񴮿�.Enabled = false;
            ������ѡ���.Enabled = false;
            ��Ŀѡ���.Enabled = false;
            д���豸��ť.Enabled = false;
            ��¼�̼���ť.Enabled = false;
            ѡ��Ŀ¼��ť.Enabled = false;
            �ȸ��¸�ѡ��.Visible = true;
            ���º��Զ�������ѡ��.Visible = true;
            ��־�ı���.Location = new Point(12, 100);
            ����ģʽ��ť.Text = "ȡ��";
            ����ģʽ���� = ��������.������������(ѡ�񴮿�.Text, 115200, (string ����) =>
            {
                if (����.Trim() == "E.reboot();")
                {
                    return;
                }
                if (����.Trim() == "echo(false);")
                {
                    return;
                }
                ��ʾ��־(����.Trim() + "\r\n");
            });
            ����ģʽ��ť.Text = "ȡ��";

            if (����ģʽ���� == null)
            {
                ��ʾ��־("����ʧ��!" + "\r\n");
                ��ʾ��־("���뿪��ģʽʧ��!" + "\r\n");
                ȡ������ģʽ();
            }
            else
            {
                ��������.�رմ������(����ģʽ����);
                ��ʾ��־("���뿪��ģʽ" + "\r\n");
            }

        }

        private void ȡ������ģʽ()
        {
            if (��Ŀ�ļ������� != null)
            {
                ��Ŀ�ļ�������.EnableRaisingEvents = false;
                ��Ŀ�ļ�������.Dispose();
                ��Ŀ�ļ������� = null;
            }
            д���ļ���ʱ��.Stop();

            ѡ�񴮿�.Enabled = true;
            ������ѡ���.Enabled = true;
            ��Ŀѡ���.Enabled = true;
            ��¼�̼���ť.Enabled = true;
            ѡ��Ŀ¼��ť.Enabled = true;
            д���豸��ť.Enabled = true;
            �ȸ��¸�ѡ��.Visible = false;
            ���º��Զ�������ѡ��.Visible = false;
            ��־�ı���.Location = new Point(12, 75);
            ����ģʽ��ť.Text = "����ģʽ";
            if (����ģʽ���� != null && ����ģʽ����.IsOpen)
            {
                ����ģʽ����.Close();
                ����ģʽ���� = null;
            }
            ��ʾ��־("����ģʽ�ѹر�!");
        }

        private void �ȸ��¸�ѡ��_CheckedChanged(object sender, EventArgs e)
        {
            if (�ȸ��¸�ѡ��.Checked)
            {
                ��ʾ��־("�����ȸ���!\r\n");
            }
            else
            {
                ��ʾ��־("�ر��ȸ���!\r\n");
            }
        }

        private void ���º��Զ�������ѡ��_CheckedChanged(object sender, EventArgs e)
        {
            if (���º��Զ�������ѡ��.Checked)
            {
                ��ʾ��־("���ø��º��Զ�����!\r\n");
            }
            else
            {
                ��ʾ��־("�رո��º��Զ�����!\r\n");
            }
        }

        private void ���仯���ļ�д���豸(object? sender, ElapsedEventArgs e)
        {
            if (�仯���ļ�.Count == 0)
            {
                return;
            }
            var �ļ��� = �仯���ļ�.Dequeue().ToString();
            string �����ļ��� = ��Ŀѡ���.Text + @"\" + �ļ���;
            �ļ��� = �ļ���.Replace("\\", "/");

            if (�ļ��� == "index.js" || �ļ��� == "main.js")
            {
                �ļ��� = ".bootcde";
            }
            string ���� = File.ReadAllText(�����ļ���);
            if (����ģʽ���� != null && ����ģʽ����.IsOpen)
            {
                ��������.�رմ������(����ģʽ����);
                ��ʾ��־("�ļ��Ķ�: " + �ļ��� + "\r\n");
                ��ʾ��־("����д���ļ�: " + �ļ��� + "\r\n");
                try
                {
                    ��������.д���ļ�(����ģʽ����, �ļ���, ����);
                    if (�仯���ļ�.Count == 0 && ���º��Զ�������ѡ��.Checked)
                    {
                        ��������.�����豸(����ģʽ����);
                    }
                }
                catch (Exception)
                {
                    ��ʾ��־("д���ļ�����: " + �ļ��� + "\r\n");
                    ȡ������ģʽ();
                    return;
                    //throw;
                }

            }
            ��ʾ��־("�ļ�д�����: " + �ļ��� + "\r\n");

        }
    }
}