using System.Windows.Forms;
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

        private void Form1_Load(object sender, EventArgs e)
        {
            show_mapscripts();
            InitTaskGridColumns();   // 設定 DGV_Script 欄位與資料繫結
            InitScriptList();        // 建立腳本清單與事件
            UpdateRowNumbers();
            //Script_AGVName_Setting();
            SeedDemo();
            LoadScripts();
            DGV_Script.EditingControlShowing += DGV_Script_EditingControlShowing;

            // 1) 綁定 ListBox -> 腳本清單
            lstScripts.DataSource = _scripts;
            lstScripts.DisplayMember = nameof(Script.ScriptName);
            // 2) 綁定 TextBox -> 目前選到的腳本名稱（這行就是你問的那段）
            txtScriptName.DataBindings.Clear();
            txtScriptName.DataBindings.Add(
                "Text",
                lstScripts,
                "SelectedItem.ScriptName",
                true,
                DataSourceUpdateMode.OnPropertyChanged
            );
            // （可選）塞一筆測試資料，並選到第一筆
            if (_scripts.Count == 0)
            {
                _scripts.Add(new Script { ScriptName = "Script demo A" });
                _scripts.Add(new Script { ScriptName = "Script 3" });
                _scripts.Add(new Script { ScriptName = "Script 4" });
            }
            lstScripts.SelectedIndex = 0;
            // lstScripts.SelectedIndexChanged += lstScripts_SelectedIndexChanged;
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


            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            listBox2.SelectedIndexChanged += listBox2_SelectedIndexChanged;
        }

        private AGVSController APIController = new AGVSController();
        private HotRunController HotRunController = new HotRunController();
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
        private void UpdateRowNumbers()
        {
            int number = 1;
            for (int i = 0; i < DGV_Script.Rows.Count; i++)
            {
                if (!DGV_Script.Rows[i].IsNewRow)
                {
                    DGV_Script.Rows[i].Cells["No"].Value = number++;
                    DGV_Script.Rows[i].Cells["Action"].Value = "move";
                }
            }
        }

        private void lstScripts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem != null)
            {
                txtScriptName.Text = lstScripts.SelectedItem.ToString();
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
        private void btn_AddScript_Click(object sender, EventArgs e)
        {
            var s = new Script { ScriptName = $"Script {_scripts.Count + 1}" };
            _scripts.Add(s);
            lstScripts.SelectedIndex = _scripts.Count - 1;
        }

        private void btn_RemoveScript_Click(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem is Script s)
                _scripts.Remove(s);
        }

        private void btn_AddTasks_Click(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem is not Script s) return;
            var nextNo = s.Tasks.Count + 1;
            s.Tasks.Add(new TaskItem { No = nextNo }); // 預設 Action = "move"
        }

        private void btn_RemoveTasks_Click(object sender, EventArgs e)
        {
            if (lstScripts.SelectedItem is not Script s) return;
            if (DGV_Script.CurrentRow?.DataBoundItem is TaskItem item)
            {
                s.Tasks.Remove(item);
                ReindexTasks(s);
                _bsTasks.ResetBindings(false);
            }
        }
        private void btn_SaveScripts_Click(object sender, EventArgs e)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(_scripts, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText("scripts.json", json, System.Text.Encoding.UTF8);
            MessageBox.Show("已儲存 scripts.json");
        }
        private void LoadScripts()
        {
            if (!File.Exists("scripts.json")) return;
            var json = File.ReadAllText("scripts.json", System.Text.Encoding.UTF8);
            var data = System.Text.Json.JsonSerializer.Deserialize<BindingList<Script>>(json) ?? new();
            _scripts = data;

            // 重新繫結
            lstScripts.DataSource = _scripts;
            lstScripts.DisplayMember = nameof(Script.ScriptName);
            if (_scripts.Count > 0) lstScripts.SelectedIndex = 0;
        }

        private void btn_LoadScripts_Click(object sender, EventArgs e)
        {
            if (!File.Exists("scripts.json")) return;
            var json = File.ReadAllText("scripts.json", System.Text.Encoding.UTF8);
            var data = System.Text.Json.JsonSerializer.Deserialize<BindingList<Script>>(json) ?? new();
            _scripts = data;

            // 重新繫結
            lstScripts.DataSource = _scripts;
            lstScripts.DisplayMember = nameof(Script.ScriptName);
            if (_scripts.Count > 0) lstScripts.SelectedIndex = 0;
        }
        private void DisplayJsonNode(JsonNode node, string prefix)
        {
            if (node is JsonObject obj)
            {
                foreach (var kvp in obj)
                {
                    DisplayJsonNode(kvp.Value, $"{prefix}{kvp.Key}: ");
                }
            }
            else if (node is JsonArray arr)
            {
                int idx = 0;
                foreach (var item in arr)
                {
                    DisplayJsonNode(item, $"{prefix}[{idx}]: ");
                    idx++;
                }
            }
            else if (node is JsonValue val)
            {
                // 將內容加到 textBoxContent
                textBox_content.AppendText($"{prefix}{val.ToJsonString()}{Environment.NewLine}");
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
        private void DGV_Script_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            int startTagColIndex = DGV_Script.Columns["Start"].Index;
            int endTagColIndex = DGV_Script.Columns["End"].Index;

            if (DGV_Script.CurrentCell.ColumnIndex == startTagColIndex ||
                DGV_Script.CurrentCell.ColumnIndex == endTagColIndex)
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress -= OnlyAllowDigit_KeyPress; // 避免重複註冊
                    tb.KeyPress += OnlyAllowDigit_KeyPress;
                }
            }
            else
            {
                if (e.Control is TextBox tb)
                {
                    tb.KeyPress -= OnlyAllowDigit_KeyPress; // 清除先前綁定
                }
            }
        }

        private void OnlyAllowDigit_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 只允許數字與控制鍵（如 Backspace）
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
                
            }
        }
        //0811
        // 在 FrmMain 類別內加：
        private BindingList<Script> _scripts = new();
        private BindingSource _bsTasks = new();

        private static readonly string[] AgvOptions = { "AGV_001", "AGV_002", "AGV_003", "AGV_004" };
        private static readonly string[] ActionOptions = { "move", "load", "unload", "charge" };
        private void InitTaskGridColumns()
        {
            DGV_Script.AutoGenerateColumns = false;
            DGV_Script.AllowUserToAddRows = false;

            DGV_Script.Columns.Clear();

            // No（唯讀）
            DGV_Script.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "No",
                DataPropertyName = "No",
                HeaderText = "No",
                ReadOnly = true,
                Width = 60
            });

            // AGVName（下拉）
            var colAgv = new DataGridViewComboBoxColumn
            {
                Name = "AGVName",
                DataPropertyName = "AGVName",
                HeaderText = "AGVName",
                DataSource = AgvOptions
            };
            DGV_Script.Columns.Add(colAgv);

            // Start(Tag)
            DGV_Script.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Start",
                DataPropertyName = "Start",
                HeaderText = "Start(Tag)",
                Width = 90
            });

            // Action（下拉）
            var colAction = new DataGridViewComboBoxColumn
            {
                Name = "Action",
                DataPropertyName = "Action",
                HeaderText = "Action",
                DataSource = ActionOptions,
                Width = 90
            };
            DGV_Script.Columns.Add(colAction);

            // End(Tag)
            DGV_Script.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "End",
                DataPropertyName = "End",
                HeaderText = "End(Tag)",
                Width = 90
            });

            // 你的數字限制仍可沿用
            DGV_Script.EditingControlShowing += DGV_Script_EditingControlShowing;
        }
        private void InitScriptList()
        {
            // 假設你在 UI 放了一個 ListBox 叫 lstScripts 用來顯示所有腳本
            lstScripts.DataSource = _scripts;
            lstScripts.DisplayMember = nameof(Script.ScriptName);

            lstScripts.SelectedIndexChanged += (s, e) =>
            {
                var script = lstScripts.SelectedItem as Script;
                _bsTasks.DataSource = script?.Tasks;
                DGV_Script.DataSource = _bsTasks;
            };
        }
        private void SeedDemo()
        {
            var s1 = new Script { ScriptName = "Script demo A" };
            s1.Tasks.Add(new TaskItem { No = 1, AGVName = "AGV_001", Start = "1", Action = "move", End = "5" });
            _scripts.Add(s1);

            if (_scripts.Count > 0) lstScripts.SelectedIndex = 0;
        }
        private static void ReindexTasks(Script script)
        {
            for (int i = 0; i < script.Tasks.Count; i++)
                script.Tasks[i].No = i + 1;
        }
        private async void btn_StartScripts_Click(object sender, EventArgs e)
        {
            RunScripts();
        }

        private async Task RunScripts()
        {
            var rows = new List<(string Agv, string Start, string End)>();
            foreach (DataGridViewRow row in DGV_Script.Rows)
            {
                if (row.IsNewRow) continue;
                var agv = Convert.ToString(row.Cells["AGVName"]?.Value)?.Trim();
                var start = Convert.ToString(row.Cells["Start"]?.Value)?.Trim();
                var end = Convert.ToString(row.Cells["End"]?.Value)?.Trim();
                if (!string.IsNullOrWhiteSpace(agv) &&
                    !string.IsNullOrWhiteSpace(start) &&
                    !string.IsNullOrWhiteSpace(end))
                {
                    rows.Add((agv, start, end));
                }
            }
            // 第一階段：全部定位
            var locateOk = new bool[rows.Count];
            for (int i = 0; i < rows.Count; i++)
            {
                var (agv, start, _) = rows[i];
                try
                {
                    var resp = await APIController.APIAGVLocate(agv, start); // 你現有的定位 API
                                                                             // 你的 API 失敗時會回傳 "發生錯誤: ..." 字串；簡單判斷一下
                    locateOk[i] = !(resp?.StartsWith("發生錯誤") ?? true);
                    //Log($"[Locate] {agv} -> {start} : {(locateOk[i] ? "OK" : "FAIL")}");
                }
                catch (Exception ex)
                {
                    locateOk[i] = false;
                    //AppendLog($"[Locate] {agv} -> {start} 例外：{ex.Message}");
                }
            }

            // 第二階段：只對「定位成功」的列下移動任務
            for (int i = 0; i < rows.Count; i++)
            {
                if (!locateOk[i]) continue; // 定位失敗的略過
                var (agv, _, end) = rows[i];
                try
                {
                    var resp = await AgvsClient.PostMoveAsync(agv, end, bypass: false); // 你現有的移動 API
                                                                                        //   AppendLog($"[Move] {agv} -> {end} : OK");
                }
                catch (Exception ex)
                {
                    // AppendLog($"[Move] {agv} -> {end} 例外：{ex.Message}");
                    // 繼續跑下一列，不讓整體中斷
                }
            }
        }
        //地圖加上腳本
        private MultiMapRoot _scriptConfig;
        private MapDto _selectedMap;
        private ScriptDto _selectedScript;
        private string _configPath = "multi-maps.json";
        private void show_mapscripts()
        {
            string path = "multi-maps.json";
            if (!File.Exists(path))
            {
                MessageBox.Show("找不到參數檔");
                return;
            }

            _scriptConfig = ScriptConfigService.Load(path);

            listBox1.Items.Clear();
            foreach (var map in _scriptConfig.Maps)
                listBox1.Items.Add(map);

            listBox1.DisplayMember = "MapName";
            listBox1.ValueMember = "MapID";
        }
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem is not MapDto selectedMap) return;

            _selectedMap = selectedMap;
            _selectedScript = null;

            listBox2.Items.Clear();
            foreach (var script in _selectedMap.Scripts)
                listBox2.Items.Add(script);

            listBox2.DisplayMember = "ScriptName";
            listBox2.ValueMember = "ScriptID";

            DGV_test.DataSource = null;
        }
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem is not ScriptDto selectedScript) return;

            _selectedScript = selectedScript;

            var taskList = selectedScript.Tasks
                .OrderBy(t => t.No)
                .Select(t => new
                {
                    t.No,
                    t.AGVName,
                    t.Start,
                    t.Action,
                    t.End
                })
                .ToList();

            DGV_test.DataSource = null;
            DGV_test.AutoGenerateColumns = true;
            DGV_test.DataSource = taskList;
            DGV_test.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void add_task(object sender, EventArgs e)
        {
            if (_selectedScript == null)
            {
                MessageBox.Show("請先選擇一個腳本");
                return;
            }
            int nextNo = _selectedScript.Tasks.Count + 1;
            _selectedScript.Tasks.Add(new TaskItemDto
            {
                No = nextNo,
                AGVName = "AGV_001",
                Start = "0",
                Action = "move",
                End = "0"
            });
            DGV_test.DataSource = null;
            DGV_test.DataSource = _selectedScript.Tasks
                .OrderBy(t => t.No)
                .Select(t => new
                {
                    t.No,
                    t.AGVName,
                    t.Start,
                    t.Action,
                    t.End
                })
                .ToList();
        }
    }
}