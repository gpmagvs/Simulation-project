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
            SuspendLayout();
            // 
            // btn_chooseproject
            // 
            btn_chooseproject.BackColor = Color.DeepSkyBlue;
            btn_chooseproject.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_chooseproject.Location = new Point(30, 21);
            btn_chooseproject.Name = "btn_chooseproject";
            btn_chooseproject.Size = new Size(148, 23);
            btn_chooseproject.TabIndex = 1;
            btn_chooseproject.Text = "選擇AGVS參數檔";
            btn_chooseproject.UseVisualStyleBackColor = false;
            btn_chooseproject.Click += btn_chooseproject_Click;
            // 
            // textBox_appsetting
            // 
            textBox_appsetting.Location = new Point(184, 22);
            textBox_appsetting.Name = "textBox_appsetting";
            textBox_appsetting.Size = new Size(313, 23);
            textBox_appsetting.TabIndex = 4;
            // 
            // textBox_content
            // 
            textBox_content.Location = new Point(30, 50);
            textBox_content.Multiline = true;
            textBox_content.Name = "textBox_content";
            textBox_content.ScrollBars = ScrollBars.Vertical;
            textBox_content.Size = new Size(962, 305);
            textBox_content.TabIndex = 5;
            // 
            // richTextBox_content
            // 
            richTextBox_content.Location = new Point(30, 397);
            richTextBox_content.Name = "richTextBox_content";
            richTextBox_content.Size = new Size(962, 156);
            richTextBox_content.TabIndex = 6;
            richTextBox_content.Text = "";
            // 
            // bnt_APItest
            // 
            bnt_APItest.Location = new Point(30, 360);
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
            button1.Location = new Point(517, 21);
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
            button2.Location = new Point(630, 12);
            button2.Name = "button2";
            button2.Size = new Size(130, 33);
            button2.TabIndex = 9;
            button2.Text = "重啟派車系統";
            button2.UseVisualStyleBackColor = false;
            button2.Click += btn_RestartAGVS_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1089, 603);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(bnt_APItest);
            Controls.Add(richTextBox_content);
            Controls.Add(textBox_content);
            Controls.Add(textBox_appsetting);
            Controls.Add(btn_chooseproject);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btn_chooseproject;
        private TextBox textBox_appsetting;
        private TextBox textBox_content;
        private RichTextBox richTextBox_content;
        private Button bnt_APItest;
        private Button button1;
        private Button button2;
    }
}
