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

        private void ��ĿĿ¼�����_TextChanged(object sender, EventArgs e)
        {

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

        private void ��ĿĿ¼�����_MouseUp(object sender, MouseEventArgs e)
        {
            // ��ʾĿ¼ѡ��Ի���

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

        private void ��Ŀѡ���_TextChanged(object sender, EventArgs e)
        {
        }

        private void ��Ŀѡ���_SelectionChangeCommitted(object sender, EventArgs e)
        {

        }

        private void ��Ŀѡ���_DropDownClosed(object sender, EventArgs e)
        {

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

        private void д���豸��ť_Click(object sender, EventArgs e)
        {
            ��������.��Ŀ¼д���豸(ѡ�񴮿�.Text, ��Ŀѡ���.Text);
        }
    }
}