namespace espjs_gui
{
    partial class 主窗口
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(主窗口));
            label1 = new Label();
            选择串口 = new ComboBox();
            label2 = new Label();
            开发板选择框 = new ComboBox();
            烧录固件按钮 = new Button();
            写入设备按钮 = new Button();
            label3 = new Label();
            日志文本框 = new TextBox();
            清除设备代码按钮 = new Button();
            项目选择框 = new ComboBox();
            选择目录按钮 = new Button();
            发送代码按钮 = new Button();
            代码输入框 = new TextBox();
            重启设备按钮 = new Button();
            开发模式按钮 = new Button();
            热更新复选框 = new CheckBox();
            更新后自动重启复选框 = new CheckBox();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(56, 17);
            label1.TabIndex = 0;
            label1.Text = "选择端口";
            // 
            // 选择串口
            // 
            选择串口.FormattingEnabled = true;
            选择串口.Items.AddRange(new object[] { "COM1" });
            选择串口.Location = new Point(74, 12);
            选择串口.Name = "选择串口";
            选择串口.Size = new Size(74, 25);
            选择串口.TabIndex = 1;
            选择串口.Tag = "";
            选择串口.Text = "COM1";
            选择串口.DropDown += 选择串口_DropDown;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(154, 15);
            label2.Name = "label2";
            label2.Size = new Size(68, 17);
            label2.TabIndex = 2;
            label2.Text = "开发板类型";
            // 
            // 开发板选择框
            // 
            开发板选择框.FormattingEnabled = true;
            开发板选择框.Location = new Point(228, 12);
            开发板选择框.Name = "开发板选择框";
            开发板选择框.Size = new Size(98, 25);
            开发板选择框.TabIndex = 3;
            开发板选择框.Text = "esp8266";
            // 
            // 烧录固件按钮
            // 
            烧录固件按钮.Location = new Point(332, 12);
            烧录固件按钮.Name = "烧录固件按钮";
            烧录固件按钮.Size = new Size(75, 23);
            烧录固件按钮.TabIndex = 4;
            烧录固件按钮.Text = "烧录固件";
            烧录固件按钮.UseVisualStyleBackColor = true;
            烧录固件按钮.Click += 烧录固件按钮_Click;
            // 
            // 写入设备按钮
            // 
            写入设备按钮.Location = new Point(413, 43);
            写入设备按钮.Name = "写入设备按钮";
            写入设备按钮.Size = new Size(75, 23);
            写入设备按钮.TabIndex = 5;
            写入设备按钮.Text = "写入设备";
            写入设备按钮.UseVisualStyleBackColor = true;
            写入设备按钮.Click += 写入设备按钮_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 46);
            label3.Name = "label3";
            label3.Size = new Size(56, 17);
            label3.TabIndex = 6;
            label3.Text = "项目地址";
            // 
            // 日志文本框
            // 
            日志文本框.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            日志文本框.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            日志文本框.Location = new Point(12, 99);
            日志文本框.Multiline = true;
            日志文本框.Name = "日志文本框";
            日志文本框.ScrollBars = ScrollBars.Vertical;
            日志文本框.Size = new Size(563, 246);
            日志文本框.TabIndex = 10;
            // 
            // 清除设备代码按钮
            // 
            清除设备代码按钮.Location = new Point(413, 12);
            清除设备代码按钮.Name = "清除设备代码按钮";
            清除设备代码按钮.Size = new Size(75, 23);
            清除设备代码按钮.TabIndex = 11;
            清除设备代码按钮.Text = "清除代码";
            清除设备代码按钮.UseVisualStyleBackColor = true;
            清除设备代码按钮.Click += 清除设备代码按钮_Click;
            // 
            // 项目选择框
            // 
            项目选择框.FormattingEnabled = true;
            项目选择框.Location = new Point(74, 43);
            项目选择框.Name = "项目选择框";
            项目选择框.Size = new Size(252, 25);
            项目选择框.TabIndex = 13;
            项目选择框.DropDown += 项目选择框_DropDown;
            // 
            // 选择目录按钮
            // 
            选择目录按钮.Location = new Point(332, 43);
            选择目录按钮.Name = "选择目录按钮";
            选择目录按钮.Size = new Size(75, 23);
            选择目录按钮.TabIndex = 14;
            选择目录按钮.Text = "选择目录";
            选择目录按钮.UseVisualStyleBackColor = true;
            选择目录按钮.Click += 选择目录按钮_Click;
            // 
            // 发送代码按钮
            // 
            发送代码按钮.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            发送代码按钮.Location = new Point(500, 351);
            发送代码按钮.Name = "发送代码按钮";
            发送代码按钮.Size = new Size(75, 23);
            发送代码按钮.TabIndex = 15;
            发送代码按钮.Text = "发送代码";
            发送代码按钮.UseVisualStyleBackColor = true;
            发送代码按钮.Click += 发送代码按钮_Click;
            // 
            // 代码输入框
            // 
            代码输入框.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            代码输入框.Location = new Point(12, 351);
            代码输入框.Name = "代码输入框";
            代码输入框.Size = new Size(482, 23);
            代码输入框.TabIndex = 16;
            代码输入框.KeyUp += 代码输入框_KeyUp;
            // 
            // 重启设备按钮
            // 
            重启设备按钮.Location = new Point(494, 12);
            重启设备按钮.Name = "重启设备按钮";
            重启设备按钮.Size = new Size(75, 23);
            重启设备按钮.TabIndex = 17;
            重启设备按钮.Text = "重启设备";
            重启设备按钮.UseVisualStyleBackColor = true;
            重启设备按钮.Click += 重启设备按钮_Click;
            // 
            // 开发模式按钮
            // 
            开发模式按钮.Location = new Point(494, 43);
            开发模式按钮.Name = "开发模式按钮";
            开发模式按钮.Size = new Size(75, 23);
            开发模式按钮.TabIndex = 18;
            开发模式按钮.Text = "开发模式";
            开发模式按钮.UseVisualStyleBackColor = true;
            开发模式按钮.Click += 开发模式按钮_Click;
            // 
            // 热更新复选框
            // 
            热更新复选框.AutoSize = true;
            热更新复选框.Location = new Point(395, 72);
            热更新复选框.Name = "热更新复选框";
            热更新复选框.Size = new Size(63, 21);
            热更新复选框.TabIndex = 19;
            热更新复选框.Text = "热更新";
            热更新复选框.UseVisualStyleBackColor = true;
            热更新复选框.Visible = false;
            热更新复选框.CheckedChanged += 热更新复选框_CheckedChanged;
            // 
            // 更新后自动重启复选框
            // 
            更新后自动重启复选框.AutoSize = true;
            更新后自动重启复选框.Location = new Point(464, 72);
            更新后自动重启复选框.Name = "更新后自动重启复选框";
            更新后自动重启复选框.Size = new Size(111, 21);
            更新后自动重启复选框.TabIndex = 20;
            更新后自动重启复选框.Text = "更新后自动重启";
            更新后自动重启复选框.UseVisualStyleBackColor = true;
            更新后自动重启复选框.Visible = false;
            更新后自动重启复选框.CheckedChanged += 更新后自动重启复选框_CheckedChanged;
            // 
            // 主窗口
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(584, 386);
            Controls.Add(更新后自动重启复选框);
            Controls.Add(热更新复选框);
            Controls.Add(开发模式按钮);
            Controls.Add(重启设备按钮);
            Controls.Add(代码输入框);
            Controls.Add(发送代码按钮);
            Controls.Add(选择目录按钮);
            Controls.Add(项目选择框);
            Controls.Add(清除设备代码按钮);
            Controls.Add(日志文本框);
            Controls.Add(label3);
            Controls.Add(写入设备按钮);
            Controls.Add(烧录固件按钮);
            Controls.Add(开发板选择框);
            Controls.Add(label2);
            Controls.Add(选择串口);
            Controls.Add(label1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(600, 800);
            MinimumSize = new Size(600, 200);
            Name = "主窗口";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Espruino 助手";
            Load += 主窗口_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private ComboBox 选择串口;
        private Label label2;
        private ComboBox 开发板选择框;
        private Button 烧录固件按钮;
        private Button 写入设备按钮;
        private Label label3;
        private TextBox 日志文本框;
        private Button 清除设备代码按钮;
        private ComboBox 项目选择框;
        private Button 选择目录按钮;
        private Button 发送代码按钮;
        private TextBox 代码输入框;
        private Button 重启设备按钮;
        private Button 开发模式按钮;
        private CheckBox 热更新复选框;
        private CheckBox 更新后自动重启复选框;
    }
}