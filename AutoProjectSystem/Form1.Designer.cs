namespace AutoProjectSystem
{
    partial class Form1
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
            btn_chooseproject = new Button();
            textBox_appsetting = new TextBox();
            textBox_content = new TextBox();
            richTextBox_content = new RichTextBox();
            bnt_APItest = new Button();
            button1 = new Button();
            button2 = new Button();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            label2 = new Label();
            label1 = new Label();
            tabPage2 = new TabPage();
            button3 = new Button();
            tabPage3 = new TabPage();
            richTextBox_AGVStatus = new RichTextBox();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // btn_chooseproject
            // 
            btn_chooseproject.BackColor = Color.DeepSkyBlue;
            btn_chooseproject.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_chooseproject.Location = new Point(14, 21);
            btn_chooseproject.Name = "btn_chooseproject";
            btn_chooseproject.Size = new Size(148, 23);
            btn_chooseproject.TabIndex = 1;
            btn_chooseproject.Text = "選擇AGVS參數檔";
            btn_chooseproject.UseVisualStyleBackColor = false;
            btn_chooseproject.Click += btn_chooseproject_Click;
            // 
            // textBox_appsetting
            // 
            textBox_appsetting.Location = new Point(191, 22);
            textBox_appsetting.Name = "textBox_appsetting";
            textBox_appsetting.Size = new Size(313, 23);
            textBox_appsetting.TabIndex = 4;
            // 
            // textBox_content
            // 
            textBox_content.Location = new Point(14, 91);
            textBox_content.Multiline = true;
            textBox_content.Name = "textBox_content";
            textBox_content.ScrollBars = ScrollBars.Vertical;
            textBox_content.Size = new Size(962, 338);
            textBox_content.TabIndex = 5;
            // 
            // richTextBox_content
            // 
            richTextBox_content.Location = new Point(14, 505);
            richTextBox_content.Name = "richTextBox_content";
            richTextBox_content.Size = new Size(962, 156);
            richTextBox_content.TabIndex = 6;
            richTextBox_content.Text = "";
            // 
            // bnt_APItest
            // 
            bnt_APItest.Location = new Point(136, 469);
            bnt_APItest.Name = "bnt_APItest";
            bnt_APItest.Size = new Size(84, 23);
            bnt_APItest.TabIndex = 7;
            bnt_APItest.Text = "api_test";
            bnt_APItest.UseVisualStyleBackColor = true;
            bnt_APItest.Click += btn_APItest_Click;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(255, 128, 0);
            button1.Location = new Point(539, 22);
            button1.Margin = new Padding(2);
            button1.Name = "button1";
            button1.Size = new Size(73, 23);
            button1.TabIndex = 8;
            button1.Text = "儲存";
            button1.UseVisualStyleBackColor = false;
            button1.Click += btn_ConfigSave_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.Red;
            button2.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            button2.Location = new Point(245, 466);
            button2.Name = "button2";
            button2.Size = new Size(130, 33);
            button2.TabIndex = 9;
            button2.Text = "重啟派車系統";
            button2.UseVisualStyleBackColor = false;
            button2.Click += btn_RestartAGVS_Click;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Location = new Point(12, 12);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1256, 806);
            tabControl1.TabIndex = 10;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(label2);
            tabPage1.Controls.Add(label1);
            tabPage1.Controls.Add(textBox_content);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(richTextBox_content);
            tabPage1.Controls.Add(textBox_appsetting);
            tabPage1.Controls.Add(button1);
            tabPage1.Controls.Add(bnt_APItest);
            tabPage1.Controls.Add(btn_chooseproject);
            tabPage1.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 136);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1248, 778);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "參數讀取";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label2.Location = new Point(14, 54);
            label2.Name = "label2";
            label2.Size = new Size(120, 20);
            label2.TabIndex = 11;
            label2.Text = "JSON_Content";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft JhengHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label1.Location = new Point(14, 469);
            label1.Name = "label1";
            label1.Size = new Size(105, 20);
            label1.TabIndex = 10;
            label1.Text = "API_Content";
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(richTextBox_AGVStatus);
            tabPage2.Controls.Add(button3);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1248, 778);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "車載設定";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(27, 16);
            button3.Name = "button3";
            button3.Size = new Size(167, 23);
            button3.TabIndex = 0;
            button3.Text = "取得AGV_Status";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btn_AGVStatus_Click;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1248, 778);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "任務清單";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // richTextBox_AGVStatus
            // 
            richTextBox_AGVStatus.Location = new Point(27, 54);
            richTextBox_AGVStatus.Name = "richTextBox_AGVStatus";
            richTextBox_AGVStatus.Size = new Size(362, 159);
            richTextBox_AGVStatus.TabIndex = 1;
            richTextBox_AGVStatus.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1299, 830);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Simulation Control Center";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Button btn_chooseproject;
        private TextBox textBox_appsetting;
        private TextBox textBox_content;
        private RichTextBox richTextBox_content;
        private Button bnt_APItest;
        private Button button1;
        private Button button2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Label label1;
        private Label label2;
        private TabPage tabPage3;
        private Button button3;
        private RichTextBox richTextBox_AGVStatus;
    }
}
