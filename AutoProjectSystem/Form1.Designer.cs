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
            components = new System.ComponentModel.Container();
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
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            textBox_Location = new TextBox();
            textBox_AGVName = new TextBox();
            richTextBox_AGVStatus = new RichTextBox();
            button3 = new Button();
            tabPage3 = new TabPage();
            button6 = new Button();
            button4 = new Button();
            button5 = new Button();
            btn_startHotRun = new Button();
            DGV_HotRunlist = new DataGridView();
            tabPage4 = new TabPage();
            btn_removeTask = new Button();
            btn_addTask = new Button();
            DGV_Script = new DataGridView();
            Script_no = new DataGridViewTextBoxColumn();
            script_agvname = new DataGridViewTextBoxColumn();
            script_startpoint = new DataGridViewTextBoxColumn();
            script_action = new DataGridViewTextBoxColumn();
            script_endpoint = new DataGridViewTextBoxColumn();
            Timer = new System.Windows.Forms.Timer(components);
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_HotRunlist).BeginInit();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Script).BeginInit();
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
            tabControl1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
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
            tabPage2.Controls.Add(label5);
            tabPage2.Controls.Add(label4);
            tabPage2.Controls.Add(label3);
            tabPage2.Controls.Add(textBox_Location);
            tabPage2.Controls.Add(textBox_AGVName);
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
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(838, 17);
            label5.Name = "label5";
            label5.Size = new Size(31, 15);
            label5.TabIndex = 4;
            label5.Text = "結果";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(264, 17);
            label4.Name = "label4";
            label4.Size = new Size(55, 15);
            label4.TabIndex = 3;
            label4.Text = "定位座標";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(81, 17);
            label3.Name = "label3";
            label3.Size = new Size(56, 15);
            label3.TabIndex = 3;
            label3.Text = "AGV名稱";
            // 
            // textBox_Location
            // 
            textBox_Location.Location = new Point(233, 34);
            textBox_Location.Name = "textBox_Location";
            textBox_Location.Size = new Size(119, 23);
            textBox_Location.TabIndex = 2;
            // 
            // textBox_AGVName
            // 
            textBox_AGVName.Location = new Point(56, 35);
            textBox_AGVName.Name = "textBox_AGVName";
            textBox_AGVName.Size = new Size(119, 23);
            textBox_AGVName.TabIndex = 2;
            // 
            // richTextBox_AGVStatus
            // 
            richTextBox_AGVStatus.Location = new Point(679, 35);
            richTextBox_AGVStatus.Name = "richTextBox_AGVStatus";
            richTextBox_AGVStatus.Size = new Size(362, 159);
            richTextBox_AGVStatus.TabIndex = 1;
            richTextBox_AGVStatus.Text = "";
            // 
            // button3
            // 
            button3.Location = new Point(389, 34);
            button3.Name = "button3";
            button3.Size = new Size(167, 23);
            button3.TabIndex = 0;
            button3.Text = "AGV_Locate";
            button3.UseVisualStyleBackColor = true;
            button3.Click += btn_AGVSLocate_Click;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(button6);
            tabPage3.Controls.Add(button4);
            tabPage3.Controls.Add(button5);
            tabPage3.Controls.Add(btn_startHotRun);
            tabPage3.Controls.Add(DGV_HotRunlist);
            tabPage3.Location = new Point(4, 24);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1248, 778);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "任務清單";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            button6.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 136);
            button6.Location = new Point(244, 18);
            button6.Name = "button6";
            button6.Size = new Size(92, 23);
            button6.TabIndex = 4;
            button6.Text = "新增腳本";
            button6.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            button4.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 136);
            button4.Location = new Point(131, 18);
            button4.Name = "button4";
            button4.Size = new Size(92, 23);
            button4.TabIndex = 3;
            button4.Text = "取消HotRun";
            button4.UseVisualStyleBackColor = true;
            button4.Click += btn_StopHotRun_Click;
            // 
            // button5
            // 
            button5.Location = new Point(1076, 555);
            button5.Name = "button5";
            button5.Size = new Size(75, 23);
            button5.TabIndex = 2;
            button5.Text = "button5";
            button5.UseVisualStyleBackColor = true;
            // 
            // btn_startHotRun
            // 
            btn_startHotRun.Font = new Font("Microsoft JhengHei UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_startHotRun.Location = new Point(22, 18);
            btn_startHotRun.Name = "btn_startHotRun";
            btn_startHotRun.Size = new Size(92, 23);
            btn_startHotRun.TabIndex = 1;
            btn_startHotRun.Text = "執行HotRun";
            btn_startHotRun.UseVisualStyleBackColor = true;
            btn_startHotRun.Click += btn_StartHotRun_Click;
            // 
            // DGV_HotRunlist
            // 
            DGV_HotRunlist.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_HotRunlist.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            DGV_HotRunlist.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_HotRunlist.Location = new Point(22, 61);
            DGV_HotRunlist.MultiSelect = false;
            DGV_HotRunlist.Name = "DGV_HotRunlist";
            DGV_HotRunlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV_HotRunlist.Size = new Size(1136, 453);
            DGV_HotRunlist.TabIndex = 0;
            DGV_HotRunlist.CellFormatting += DGV_HotRunlist_CellFormatting;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(btn_removeTask);
            tabPage4.Controls.Add(btn_addTask);
            tabPage4.Controls.Add(DGV_Script);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(1248, 778);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "腳本設定";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // btn_removeTask
            // 
            btn_removeTask.Location = new Point(138, 21);
            btn_removeTask.Name = "btn_removeTask";
            btn_removeTask.Size = new Size(92, 23);
            btn_removeTask.TabIndex = 1;
            btn_removeTask.Text = "刪除任務";
            btn_removeTask.UseVisualStyleBackColor = true;
            btn_removeTask.Click += btn_RemoveTask_Click;
            // 
            // btn_addTask
            // 
            btn_addTask.Location = new Point(21, 21);
            btn_addTask.Name = "btn_addTask";
            btn_addTask.Size = new Size(92, 23);
            btn_addTask.TabIndex = 1;
            btn_addTask.Text = "新增任務";
            btn_addTask.UseVisualStyleBackColor = true;
            btn_addTask.Click += btn_AddTask_Click;
            // 
            // DGV_Script
            // 
            DGV_Script.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Script.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Script.Columns.AddRange(new DataGridViewColumn[] { Script_no, script_agvname, script_startpoint, script_action, script_endpoint });
            DGV_Script.Location = new Point(21, 61);
            DGV_Script.Name = "DGV_Script";
            DGV_Script.Size = new Size(716, 351);
            DGV_Script.TabIndex = 0;
            // 
            // Script_no
            // 
            Script_no.HeaderText = "No";
            Script_no.Name = "Script_no";
            Script_no.ReadOnly = true;
            // 
            // script_agvname
            // 
            script_agvname.HeaderText = "AGVName";
            script_agvname.Name = "script_agvname";
            // 
            // script_startpoint
            // 
            script_startpoint.HeaderText = "Start(Tag)";
            script_startpoint.Name = "script_startpoint";
            // 
            // script_action
            // 
            script_action.HeaderText = "Action";
            script_action.Name = "script_action";
            // 
            // script_endpoint
            // 
            script_endpoint.HeaderText = "End(Tag)";
            script_endpoint.Name = "script_endpoint";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1299, 830);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "Simulation Control Center";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_HotRunlist).EndInit();
            tabPage4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_Script).EndInit();
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
        private TextBox textBox_Location;
        private TextBox textBox_AGVName;
        private Label label4;
        private Label label3;
        private Label label5;
        private DataGridView DGV_HotRunlist;
        private Button btn_startHotRun;
        private Button button5;
        private Button button4;
        private System.Windows.Forms.Timer refreshTimer;
        private Button button6;
        private TabPage tabPage4;
        private DataGridView DGV_Script;
        private Button btn_removeTask;
        private Button btn_addTask;
        private DataGridViewTextBoxColumn Script_no;
        private DataGridViewTextBoxColumn script_agvname;
        private DataGridViewTextBoxColumn script_startpoint;
        private DataGridViewTextBoxColumn script_action;
        private DataGridViewTextBoxColumn script_endpoint;
    }
}
