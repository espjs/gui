using Microsoft.VisualBasic.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace espjs_gui
{
    public partial class ������ : Form
    {
        public ������()
        {
            InitializeComponent();
        }

        private void ������_Load(object sender, EventArgs e)
        {
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
            ��������.�������(ѡ�񴮿�.Text, ������ѡ���.Text, (string ���) =>
            {
                ��ʾ��־(��� + "\r\n");
            });
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
            var ������ = �û�����.��ȡ������(������);
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
            var ������ = �û�����.��ȡ������(������);
            if (���ں� == "")
            {
                ��ʾ��־("�������ѡ�񴮿ں�\r\n");
                return;
            }
            var ���� = ���������.Text;
            ��ʾ��־(���� + "\r\n");
            Task.Run(async () =>
            {
                var ����ִ�н�� = await ��������.��ȡ�����ִ�н��(���ں�, ������, ����);
                ��ʾ��־(����ִ�н�� + "\r\n");
            });
            ���������.Text = "";
        }
    }
}