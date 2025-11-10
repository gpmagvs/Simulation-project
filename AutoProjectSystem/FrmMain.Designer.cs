namespace AutoProjectSystem
{
    partial class FrmMain
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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            btn_chooseproject = new Button();
            textBox_appsetting = new TextBox();
            textBox_content = new TextBox();
            richTextBox_content = new RichTextBox();
            bnt_APItest = new Button();
            button1 = new Button();
            button2 = new Button();
            tabControl1 = new TabControl();
            tabPage4 = new TabPage();
            label9 = new Label();
            button8 = new Button();
            button12 = new Button();
            label8 = new Label();
            label6 = new Label();
            label7 = new Label();
            btn_DeleteMap = new Button();
            btn_AddMap = new Button();
            listMapBox = new ListBox();
            button11 = new Button();
            button10 = new Button();
            btn_saveScript = new Button();
            btn_removeScript = new Button();
            lstScripts = new ListBox();
            button7 = new Button();
            btn_removeTask = new Button();
            btn_addScript = new Button();
            btn_addTask = new Button();
            DGV_Script = new DataGridView();
            No = new DataGridViewTextBoxColumn();
            AGVName = new DataGridViewTextBoxColumn();
            Start = new DataGridViewTextBoxColumn();
            Action = new DataGridViewTextBoxColumn();
            End = new DataGridViewTextBoxColumn();
            tabPage5 = new TabPage();
            btn_taskquery = new Button();
            DGV_Tasks = new DataGridView();
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
            Timer = new System.Windows.Forms.Timer(components);
            login_status = new Button();
            btn_SQLstatus = new Button();
            tabControl1.SuspendLayout();
            tabPage4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Script).BeginInit();
            tabPage5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Tasks).BeginInit();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_HotRunlist).BeginInit();
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
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.ImeMode = ImeMode.NoControl;
            tabControl1.Location = new Point(11, 46);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1256, 619);
            tabControl1.TabIndex = 10;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(label9);
            tabPage4.Controls.Add(button8);
            tabPage4.Controls.Add(button12);
            tabPage4.Controls.Add(label8);
            tabPage4.Controls.Add(label6);
            tabPage4.Controls.Add(label7);
            tabPage4.Controls.Add(btn_DeleteMap);
            tabPage4.Controls.Add(btn_AddMap);
            tabPage4.Controls.Add(listMapBox);
            tabPage4.Controls.Add(button11);
            tabPage4.Controls.Add(button10);
            tabPage4.Controls.Add(btn_saveScript);
            tabPage4.Controls.Add(btn_removeScript);
            tabPage4.Controls.Add(lstScripts);
            tabPage4.Controls.Add(button7);
            tabPage4.Controls.Add(btn_removeTask);
            tabPage4.Controls.Add(btn_addScript);
            tabPage4.Controls.Add(btn_addTask);
            tabPage4.Controls.Add(DGV_Script);
            tabPage4.Location = new Point(4, 24);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(1248, 591);
            tabPage4.TabIndex = 3;
            tabPage4.Text = "腳本設定";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label9.Location = new Point(859, 333);
            label9.Name = "label9";
            label9.Size = new Size(91, 24);
            label9.TabIndex = 16;
            label9.Text = "功能測試:";
            // 
            // button8
            // 
            button8.Location = new Point(1068, 365);
            button8.Name = "button8";
            button8.Size = new Size(92, 23);
            button8.TabIndex = 15;
            button8.Text = "測試移動任務";
            button8.UseVisualStyleBackColor = true;
            // 
            // button12
            // 
            button12.Location = new Point(970, 365);
            button12.Name = "button12";
            button12.Size = new Size(92, 23);
            button12.TabIndex = 14;
            button12.Text = "測試定位功能";
            button12.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.Silver;
            label8.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label8.Location = new Point(540, 13);
            label8.Name = "label8";
            label8.Size = new Size(91, 24);
            label8.TabIndex = 13;
            label8.Text = "任務列表:";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.BackColor = Color.Silver;
            label6.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label6.Location = new Point(277, 13);
            label6.Name = "label6";
            label6.Size = new Size(91, 24);
            label6.TabIndex = 13;
            label6.Text = "腳本名稱:";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.BackColor = Color.Silver;
            label7.Font = new Font("Microsoft JhengHei UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 136);
            label7.Location = new Point(13, 13);
            label7.Name = "label7";
            label7.Size = new Size(91, 24);
            label7.TabIndex = 13;
            label7.Text = "場域名稱:";
            // 
            // btn_DeleteMap
            // 
            btn_DeleteMap.BackColor = Color.DeepSkyBlue;
            btn_DeleteMap.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_DeleteMap.Location = new Point(122, 333);
            btn_DeleteMap.Name = "btn_DeleteMap";
            btn_DeleteMap.Size = new Size(92, 28);
            btn_DeleteMap.TabIndex = 12;
            btn_DeleteMap.Text = "刪除場域";
            btn_DeleteMap.UseVisualStyleBackColor = false;
            btn_DeleteMap.Click += btnDeleteMap_Click;
            // 
            // btn_AddMap
            // 
            btn_AddMap.BackColor = Color.DeepSkyBlue;
            btn_AddMap.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_AddMap.Location = new Point(13, 333);
            btn_AddMap.Name = "btn_AddMap";
            btn_AddMap.Size = new Size(92, 28);
            btn_AddMap.TabIndex = 11;
            btn_AddMap.Text = "新增場域";
            btn_AddMap.UseVisualStyleBackColor = false;
            btn_AddMap.Click += btnAddMap_Click;
            // 
            // listMapBox
            // 
            listMapBox.FormattingEnabled = true;
            listMapBox.ItemHeight = 15;
            listMapBox.Location = new Point(13, 44);
            listMapBox.Name = "listMapBox";
            listMapBox.Size = new Size(201, 274);
            listMapBox.TabIndex = 10;
            // 
            // button11
            // 
            button11.BackColor = Color.Lime;
            button11.ForeColor = SystemColors.ControlText;
            button11.Location = new Point(966, 336);
            button11.Name = "button11";
            button11.Size = new Size(96, 23);
            button11.TabIndex = 9;
            button11.Text = "登入派車系統";
            button11.UseVisualStyleBackColor = false;
            button11.Click += btnLogin_Click;
            // 
            // button10
            // 
            button10.BackColor = Color.Red;
            button10.Location = new Point(1068, 336);
            button10.Name = "button10";
            button10.Size = new Size(96, 23);
            button10.TabIndex = 8;
            button10.Text = "測試執行任務";
            button10.UseVisualStyleBackColor = false;
            button10.Click += btn_MoveTask_Click;
            // 
            // btn_saveScript
            // 
            btn_saveScript.BackColor = Color.LimeGreen;
            btn_saveScript.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_saveScript.Location = new Point(13, 372);
            btn_saveScript.Name = "btn_saveScript";
            btn_saveScript.Size = new Size(92, 28);
            btn_saveScript.TabIndex = 5;
            btn_saveScript.Text = "儲存腳本";
            btn_saveScript.UseVisualStyleBackColor = false;
            btn_saveScript.Click += btnSaveJson_Click;
            // 
            // btn_removeScript
            // 
            btn_removeScript.BackColor = Color.OrangeRed;
            btn_removeScript.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_removeScript.Location = new Point(386, 372);
            btn_removeScript.Name = "btn_removeScript";
            btn_removeScript.Size = new Size(92, 28);
            btn_removeScript.TabIndex = 5;
            btn_removeScript.Text = "刪除腳本";
            btn_removeScript.UseVisualStyleBackColor = false;
            btn_removeScript.Click += btnDeleteScript_Click;
            // 
            // lstScripts
            // 
            lstScripts.FormattingEnabled = true;
            lstScripts.ItemHeight = 15;
            lstScripts.Location = new Point(277, 44);
            lstScripts.Name = "lstScripts";
            lstScripts.Size = new Size(201, 274);
            lstScripts.TabIndex = 3;
            // 
            // button7
            // 
            button7.BackColor = Color.DeepSkyBlue;
            button7.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            button7.ForeColor = SystemColors.ControlText;
            button7.Location = new Point(386, 333);
            button7.Margin = new Padding(2);
            button7.Name = "button7";
            button7.Size = new Size(92, 28);
            button7.TabIndex = 2;
            button7.Text = "執行腳本";
            button7.UseVisualStyleBackColor = false;
            button7.Click += btn_Scripts_Click;
            // 
            // btn_removeTask
            // 
            btn_removeTask.BackColor = Color.DeepSkyBlue;
            btn_removeTask.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_removeTask.Location = new Point(658, 333);
            btn_removeTask.Name = "btn_removeTask";
            btn_removeTask.Size = new Size(92, 28);
            btn_removeTask.TabIndex = 1;
            btn_removeTask.Text = "刪除任務";
            btn_removeTask.UseVisualStyleBackColor = false;
            btn_removeTask.Click += Delete_task;
            // 
            // btn_addScript
            // 
            btn_addScript.BackColor = Color.DeepSkyBlue;
            btn_addScript.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_addScript.Location = new Point(277, 333);
            btn_addScript.Name = "btn_addScript";
            btn_addScript.Size = new Size(92, 28);
            btn_addScript.TabIndex = 1;
            btn_addScript.Text = "新增腳本";
            btn_addScript.UseVisualStyleBackColor = false;
            btn_addScript.Click += btnAddScript_Click;
            // 
            // btn_addTask
            // 
            btn_addTask.BackColor = Color.DeepSkyBlue;
            btn_addTask.Font = new Font("Microsoft JhengHei UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 136);
            btn_addTask.Location = new Point(540, 333);
            btn_addTask.Name = "btn_addTask";
            btn_addTask.Size = new Size(92, 28);
            btn_addTask.TabIndex = 1;
            btn_addTask.Text = "新增任務";
            btn_addTask.UseVisualStyleBackColor = false;
            btn_addTask.Click += add_task;
            // 
            // DGV_Script
            // 
            DGV_Script.AllowUserToAddRows = false;
            DGV_Script.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Script.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Script.Columns.AddRange(new DataGridViewColumn[] { No, AGVName, Start, Action, End });
            DGV_Script.Location = new Point(540, 44);
            DGV_Script.Name = "DGV_Script";
            DGV_Script.RowHeadersWidth = 51;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            DGV_Script.RowsDefaultCellStyle = dataGridViewCellStyle2;
            DGV_Script.Size = new Size(593, 274);
            DGV_Script.TabIndex = 0;
            // 
            // No
            // 
            No.HeaderText = "No";
            No.MinimumWidth = 6;
            No.Name = "No";
            No.ReadOnly = true;
            // 
            // AGVName
            // 
            AGVName.HeaderText = "AGVName";
            AGVName.MinimumWidth = 6;
            AGVName.Name = "AGVName";
            // 
            // Start
            // 
            Start.HeaderText = "Start";
            Start.MinimumWidth = 6;
            Start.Name = "Start";
            // 
            // Action
            // 
            Action.HeaderText = "Action";
            Action.MinimumWidth = 6;
            Action.Name = "Action";
            // 
            // End
            // 
            End.HeaderText = "End";
            End.MinimumWidth = 6;
            End.Name = "End";
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(btn_taskquery);
            tabPage5.Controls.Add(DGV_Tasks);
            tabPage5.Location = new Point(4, 24);
            tabPage5.Margin = new Padding(2);
            tabPage5.Name = "tabPage5";
            tabPage5.Size = new Size(1248, 591);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "腳本任務狀態";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // btn_taskquery
            // 
            btn_taskquery.Location = new Point(24, 14);
            btn_taskquery.Name = "btn_taskquery";
            btn_taskquery.Size = new Size(92, 23);
            btn_taskquery.TabIndex = 1;
            btn_taskquery.Text = "查詢";
            btn_taskquery.UseVisualStyleBackColor = true;
            btn_taskquery.Click += btnLoadTasks_Click;
            // 
            // DGV_Tasks
            // 
            DGV_Tasks.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            DGV_Tasks.Location = new Point(24, 57);
            DGV_Tasks.Name = "DGV_Tasks";
            DGV_Tasks.Size = new Size(996, 338);
            DGV_Tasks.TabIndex = 0;
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
            tabPage1.Size = new Size(1248, 591);
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
            tabPage2.Size = new Size(1248, 591);
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
            tabPage3.Size = new Size(1248, 591);
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
            DGV_HotRunlist.RowHeadersWidth = 51;
            DGV_HotRunlist.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            DGV_HotRunlist.Size = new Size(1136, 453);
            DGV_HotRunlist.TabIndex = 0;
            DGV_HotRunlist.CellFormatting += DGV_HotRunlist_CellFormatting;
            // 
            // login_status
            // 
            login_status.Location = new Point(11, 10);
            login_status.Name = "login_status";
            login_status.Size = new Size(129, 30);
            login_status.TabIndex = 11;
            login_status.Text = "派車登入連線狀態";
            login_status.UseVisualStyleBackColor = true;
            // 
            // btn_SQLstatus
            // 
            btn_SQLstatus.Location = new Point(154, 10);
            btn_SQLstatus.Name = "btn_SQLstatus";
            btn_SQLstatus.Size = new Size(129, 30);
            btn_SQLstatus.TabIndex = 12;
            btn_SQLstatus.Text = "資料庫連線狀態";
            btn_SQLstatus.UseVisualStyleBackColor = true;
            // 
            // FrmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1299, 675);
            Controls.Add(btn_SQLstatus);
            Controls.Add(login_status);
            Controls.Add(tabControl1);
            Name = "FrmMain";
            Text = "Simulation Control Center";
            Load += Form1_Load;
            tabControl1.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)DGV_Script).EndInit();
            tabPage5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_Tasks).EndInit();
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            tabPage3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)DGV_HotRunlist).EndInit();
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
        private Button button9;
        private Button button8;
        private TabPage tabPage5;
        private Button login_status;
        private TabPage tabPage4;
        private ListBox listMapBox;
        private Button button11;
        private Button button10;
        private Button btn_saveScript;
        private Button btn_removeScript;
        private ListBox lstScripts;
        private Button button7;
        private Button btn_removeTask;
        private Button btn_addScript;
        private Button btn_addTask;
        private DataGridView DGV_Script;
        private DataGridViewTextBoxColumn No;
        private DataGridViewTextBoxColumn AGVName;
        private DataGridViewTextBoxColumn Start;
        private DataGridViewTextBoxColumn Action;
        private DataGridViewTextBoxColumn End;
        private Button btn_DeleteMap;
        private Button btn_AddMap;
        private Label label7;
        private Label label8;
        private Label label6;
        private Button button12;
        private Button btn_SQLstatus;
        private DataGridView DGV_Tasks;
        private Button btn_taskquery;
        private Label label9;
    }
}
