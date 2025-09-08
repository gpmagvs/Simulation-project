﻿using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

using System.Net.Http;
using System.Threading.Tasks;
using AutoProjectSystem.Controllers;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;
using static AutoProjectSystem.Controllers.HotRunController;
using AGVSystem.Models.TaskAllocation.HotRun;
using System.Net.Http.Json;
using System.ComponentModel;
using Microsoft.VisualBasic.Logging;
using AGVSystem.Config;
using MapDto = AGVSystem.Config.MapDto;
using ScriptDto = AGVSystem.Config.ScriptDto;
using TaskItemDto = AGVSystem.Config.TaskItemDto;
using MultiMapRoot = AGVSystem.Config.MultiMapRoot;
using System.Text.Json.Serialization;
//using static AutoProjectSystem.MapScripts;

namespace AutoProjectSystem
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
        private System.Windows.Forms.Timer Timer;
        private bool _isChecking;
        private CancellationTokenSource? _pollCts;

        private readonly BindingSource _mapBS = new();
        private readonly BindingSource _scriptBS = new();
        private readonly BindingSource _tasksBS = new();

        private MultiMapRoot _data = new();
        private string _configPath = "multi-maps_test.json";

        private readonly string[] _agvOptions = { "AGV_001", "AGV_002", "AGV_003", "AGV_004" };
        private readonly string[] _actionOptions = { "move", "load", "unload" };

        private AGVSController APIController = new AGVSController();
        private HotRunController HotRunController = new HotRunController();
        private void Form1_Load(object sender, EventArgs e)
        {
            InitGrid();          // 設定 DGV 欄位
            WireEvents();        // 綁定選取事件
            LoadData();          // 載入 JSON 並完成資料繫結
            //InitScriptList();        // 建立腳本清單與事件
            //背景自動登入

            this.Shown += async (_, __) =>
            {
                try
                {
                    var res = await AgvsClient.LoginAsync("dev", "12345678");
                    SetLoginStatus(res?.OK == true);
                }
                catch
                {
                    SetLoginStatus(false);
                }

                _pollCts = new CancellationTokenSource();
                _ = PollConnectionAsync(TimeSpan.FromSeconds(5), _pollCts.Token);
            };
        }

        public class MultiMapRoot
        {
            [JsonPropertyName("version")]
            public int Version { get; set; } = 1;

            [JsonPropertyName("maps")]
            public List<MapDto> Maps { get; set; } = new();
        }




        private void LoadData()
        {
            var json = File.ReadAllText(_configPath);
            _data = JsonSerializer.Deserialize<MultiMapRoot>(json, new JsonSerializerOptions
            {
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            }) ?? new MultiMapRoot();

            _mapBS.DataSource = _data.Maps;
            listMapBox.DataSource = _mapBS;
            listMapBox.DisplayMember = nameof(MapDto.MapName);

            if (listMapBox.Items.Count > 0)
                listMapBox.SelectedIndex = 0;
        }

        private void WireEvents()
        {
            // Map → Scripts
            listMapBox.SelectedIndexChanged += (_, __) =>
            {
                var map = listMapBox.SelectedItem as MapDto;

                var scripts = map?.Scripts != null
                    ? new BindingList<ScriptDto>(map.Scripts)
                    : new BindingList<ScriptDto>();

                _scriptBS.DataSource = scripts;
                lstScripts.DataSource = _scriptBS;
                lstScripts.DisplayMember = nameof(ScriptDto.ScriptName);

                lstScripts.SelectedIndex = lstScripts.Items.Count > 0 ? 0 : -1;
            };

            // Script → Tasks (DGV)
            lstScripts.SelectedIndexChanged += (_, __) =>
            {
                var script = lstScripts.SelectedItem as ScriptDto;

                var tasks = script?.Tasks != null
                    ? new BindingList<TaskItemDto>(script.Tasks)   // ⬅️ 這行就不會再報你那個錯
                    : new BindingList<TaskItemDto>();

                _tasksBS.DataSource = tasks;
                DGV_Script.DataSource = _tasksBS;
            };
        }

        private void InitGrid()
        {
            DGV_Script.AutoGenerateColumns = false;
            DGV_Script.Columns.Clear();
            DGV_Script.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Script.AllowUserToAddRows = true;
            DGV_Script.AllowUserToDeleteRows = true;
            DGV_Script.EditMode = DataGridViewEditMode.EditOnEnter;
            DGV_Script.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DGV_Script.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colAGVName",
                HeaderText = "AGVName",
                DataPropertyName = nameof(TaskItemDto.AGVName),
                DataSource = _agvOptions
            });

            DGV_Script.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colStart",
                HeaderText = "Start",
                DataPropertyName = nameof(TaskItemDto.Start)
            });

            DGV_Script.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "colAction",
                HeaderText = "Action",
                DataPropertyName = nameof(TaskItemDto.Action),
                DataSource = _actionOptions
            });

            DGV_Script.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colEnd",
                HeaderText = "End",
                DataPropertyName = nameof(TaskItemDto.End)
            });

            DGV_Script.DataError += (_, __) => { }; // 吃掉 ComboBox 綁定錯誤

            DGV_Script.EditingControlShowing += (s, e) =>
            {
                if (DGV_Script.CurrentCell == null) return;
                var name = DGV_Script.Columns[DGV_Script.CurrentCell.ColumnIndex].Name;
                if ((name == "colStart" || name == "colEnd") && e.Control is TextBox tb)
                {
                    tb.KeyPress -= IntOnly_KeyPress;
                    tb.KeyPress += IntOnly_KeyPress;
                }
            };
        }
        private void IntOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;
        }

        private void SetLoginStatus(bool ok)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => login_status.BackColor = ok ? Color.Lime : Color.Red));
            }
            else
            {
                login_status.BackColor = ok ? Color.Lime : Color.Red;
            }
        }
        private async Task PollConnectionAsync(TimeSpan interval, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                bool ok = false;
                try
                {
                    var res = await AgvsClient.LoginAsync("dev", "12345678").ConfigureAwait(false);
                    ok = res?.OK == true;
                }
                catch
                {
                    ok = false; // 失敗就視為斷線
                }

                SetLoginStatus(ok);

                try
                {
                    await Task.Delay(interval, ct).ConfigureAwait(false);
                }
                catch (TaskCanceledException)
                {
                    // 被取消就跳出
                    break;
                }
            }
        }
        private async void AutoLogin(object sender, EventArgs e)
        {
            try
            {
                await AgvsClient.LoginAsync("dev", "12345678");
                login_status.BackColor = Color.Lime;
            }
            catch (Exception)
            {
                login_status.BackColor = Color.Red;
                throw;
            }
        }
        private async void btn_StartHotRun_Click(object sender, EventArgs e)
        {
            if (DGV_HotRunlist.SelectedRows.Count == 0)
            {
                MessageBox.Show("請先選擇一筆 HotRun 資料", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var selectedRow = DGV_HotRunlist.SelectedRows[0];
            string scriptID = selectedRow.Cells["ScriptID"].Value.ToString();
            var result = MessageBox.Show($"確定要執行 HotRun 腳本：{scriptID}？", "執行確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await HotRunController.StartHotRunApiAsync(scriptID);
            }
        }
        private async void btn_StopHotRun_Click(object sender, EventArgs e)
        {
            if (DGV_HotRunlist.SelectedRows.Count == 0)
            {
                MessageBox.Show("請先選擇一筆 HotRun 資料", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var selectedRow = DGV_HotRunlist.SelectedRows[0];
            string scriptID = selectedRow.Cells["ScriptID"].Value.ToString();
            var result = MessageBox.Show($"確定要執行 HotRun 腳本：{scriptID}？", "執行確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                await HotRunController.StopHotRunApiAsync(scriptID);
            }
        }
        private void DGV_HotRunlist_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // 確保索引範圍合法
            if (DGV_HotRunlist.Columns[e.ColumnIndex].Name == "State" && e.Value != null)
            {
                string stateValue = e.Value.ToString()?.ToUpper();
                if (stateValue == "RUNNING")
                {
                    e.CellStyle.BackColor = Color.LightGreen;
                }
                else if (stateValue == "IDLE")
                {
                    e.CellStyle.BackColor = Color.Orange;
                }
            }
        }
        private void btn_chooseproject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "選擇JSON檔案";
                openFileDialog.Filter = "JSON檔案 (*.json)|*.json|所有檔案 (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    // 修正：顯示完整路徑
                    textBox_appsetting.Text = filePath;
                    // 讀取並解析JSON
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        textBox_content.Text = content;
                        // 使用 JsonNode 解析並遞迴顯示
                        //var node = JsonNode.Parse(jsonText);
                        //DisplayJsonNode(node, "");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("讀取或解析JSON失敗: " + ex.Message);
                    }
                }
            }
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            // btnLogin.Enabled = false;
            try
            {
                await AgvsClient.LoginAsync("dev", "12345678");
                MessageBox.Show("登入成功！", "成功",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("登入失敗：\r\n" + ex.Message, "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                //   btnLogin.Enabled = true;
            }
        }
        private async void btn_MoveTask_Click(object sender, EventArgs e)
        {
            // btnMove.Enabled = false;
            try
            {
                var result = await AgvsClient.PostMoveAsync("AGV_001", "97", false);
                //  txtOutput.Text = "Move 回應：\r\n" + result;
            }
            catch (Exception ex)
            {
                //    txtOutput.Text = "Move 失敗：\r\n" + ex.Message;
            }
            finally
            {
                //   btnMove.Enabled = true;
            }
        }

        private async void btn_APItest_Click(object sender, EventArgs e)
        {
            // string net = "locaohost:5216";  // 預設 URL，可以從其他控制項獲取用戶輸入的 URL;
            APIController.APITestAsync();
            string result = await APIController.APITestAsync();
            richTextBox_content.Text = result;

        }
        private async void btn_AGVSLocate_Click(object sender, EventArgs e)
        {
            string agvname = textBox_AGVName.Text.Trim();
            string location = textBox_Location.Text.Trim();
            string result = await APIController.APIAGVLocate(agvname, location);
            richTextBox_AGVStatus.AppendText(result + Environment.NewLine);
        }
        private async void btn_ConfigSave_Click(object sender, EventArgs e)
        {
            // 取得剛剛讀取的檔案路徑
            string filePath = textBox_appsetting.Text;
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                var result = MessageBox.Show($"確定要覆蓋並儲存此檔案嗎？\n{filePath}", "確認儲存", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        File.WriteAllText(filePath, textBox_content.Text);
                        MessageBox.Show("儲存成功: " + filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("保存配置文件時出錯: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("請選擇一個有效的配置文件。");
            }
        }
        private async void btn_RestartAGVS_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("確定是否重啟派車系統?", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // 在這裡執行重啟派車系統的程式碼
                // 例如：await APIController.RestartAGVSAsync();
                string response = await APIController.RestartAGVS();
                richTextBox_content.AppendText(response + Environment.NewLine);
                MessageBox.Show("已執行重啟派車系統。");
            }
            else
            {
                // 取消，不做任何事
                MessageBox.Show("已取消，下次別亂按");
            }
        }

        private static void ReindexTasks(Script script)
        {
            for (int i = 0; i < script.Tasks.Count; i++)
                script.Tasks[i].No = i + 1;
        }
        //地圖加上腳本
        //private MultiMapRoot _scriptConfig;
        private MapDto _selectedMap;
        private ScriptDto _selectedScript;

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            using var sfd = new SaveFileDialog { Filter = "JSON|*.json", FileName = "multi-maps_test.json" };
            if (sfd.ShowDialog() != DialogResult.OK) return;
            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(sfd.FileName, JsonSerializer.Serialize(_data, opt));
            MessageBox.Show("已儲存。");
        }

        private void btnLoadJson_Click(object sender, EventArgs e)
        {
            using var ofd = new OpenFileDialog { Filter = "JSON|*.json" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _configPath = ofd.FileName;
            LoadData();
        }
        private static string Prompt(string text, string title, string defaultValue = "")
        {
            var form = new Form { Width = 380, Height = 160, Text = title, StartPosition = FormStartPosition.CenterParent };
            var lbl = new Label { Left = 15, Top = 15, Width = 330, Text = text };
            var tb = new TextBox { Left = 15, Top = 45, Width = 340, Text = defaultValue };
            var ok = new Button { Text = "OK", Left = 200, Width = 70, Top = 80, DialogResult = DialogResult.OK };
            var cancel = new Button { Text = "Cancel", Left = 285, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };
            form.Controls.AddRange(new Control[] { lbl, tb, ok, cancel });
            form.AcceptButton = ok; form.CancelButton = cancel;
            return form.ShowDialog() == DialogResult.OK ? tb.Text : null;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _pollCts?.Cancel();
            base.OnFormClosing(e);
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listMapBox.SelectedItem is not MapDto selectedMap) return;

            _selectedMap = selectedMap;
            _selectedScript = null;

            lstScripts.Items.Clear();
            foreach (var script in _selectedMap.Scripts)
                lstScripts.Items.Add(script);

            lstScripts.DisplayMember = "ScriptName";
            lstScripts.ValueMember = "ScriptID";

            DGV_Script.DataSource = null;
        }

        private void add_task(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem is not ScriptDto) { MessageBox.Show("請先選擇一個腳本"); return; }
            if (_tasksBS.List is BindingList<TaskItemDto> list)
            {
                list.Add(new TaskItemDto { AGVName = _agvOptions[0], Start = "-1", Action = _actionOptions[0], End = "-1" });
            }
        }

        private void Delete_task(object sender, EventArgs e)
        {
            if (DGV_Script.CurrentRow?.DataBoundItem is TaskItemDto item &&
                _tasksBS.List is BindingList<TaskItemDto> list)
            {
                list.Remove(item);
            }
        }
        private void btnAddMap_Click(object sender, EventArgs e)
        {
            var name = Prompt("地圖名稱：", "新增地圖", "NewMap");
            if (string.IsNullOrWhiteSpace(name)) return;
            var map = new MapDto { MapName = name, Scripts = new List<ScriptDto>() };
            _data.Maps.Add(map);
            _mapBS.ResetBindings(false);
            listMapBox.SelectedItem = map;
        }
        private void btnDeleteMap_Click(object sender, EventArgs e)
        {
            if (listMapBox.SelectedItem is MapDto m)
            {
                _data.Maps.Remove(m);
                _mapBS.ResetBindings(false);
            }
        }

        private void btnRenameMap_Click(object sender, EventArgs e)
        {
            if (listMapBox.SelectedItem is not MapDto m) return;
            var name = Prompt("新地圖名稱：", "重新命名地圖", m.MapName);
            if (string.IsNullOrWhiteSpace(name)) return;
            m.MapName = name;
            _mapBS.ResetBindings(false);
        }

        // Script
        private void btnAddScript_Click(object sender, EventArgs e)
        {
            if (listMapBox.SelectedItem is not MapDto m) return;
            var name = Prompt("腳本名稱：", "新增腳本", "NewScript");
            if (string.IsNullOrWhiteSpace(name)) return;

            var s = new ScriptDto { ScriptName = name, Tasks = new List<TaskItemDto>() };
            m.Scripts.Add(s);
            _scriptBS.ResetBindings(false);
            lstScripts.SelectedItem = s;
        }

        private void btnDeleteScript_Click(object sender, EventArgs e)
        {
            if (listMapBox.SelectedItem is MapDto m && lstScripts.SelectedItem is ScriptDto s)
            {
                m.Scripts.Remove(s);
                _scriptBS.ResetBindings(false);
            }
        }

        private void btnRenameScript_Click(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem is not ScriptDto s) return;
            var name = Prompt("新腳本名稱：", "重新命名腳本", s.ScriptName);
            if (string.IsNullOrWhiteSpace(name)) return;
            s.ScriptName = name;
            _scriptBS.ResetBindings(false);
        }
    }
}