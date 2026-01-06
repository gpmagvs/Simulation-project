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
using MapDto = AGVSystem.Config.MapDto;
using ScriptDto = AGVSystem.Config.ScriptDto;
using TaskItemDto = AGVSystem.Config.TaskItemDto;
using MultiMapRoot = AGVSystem.Config.MultiMapRoot;
using System.Text.Json.Serialization;
using System.Text;
using static AutoProjectSystem.MapScripts;
using System.Windows.Forms.Design;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Data;

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

        private AGVSController APIController = new AGVSController("localhost:5216", new HttpClient());
        private HotRunController HotRunController = new HotRunController();
        private void Form1_Load(object sender, EventArgs e)
        {
            InitGrid();          // 設定 DGV 欄位
            WireEvents();        // 綁定選取事件
            LoadData();          // 載入 JSON 並完成資料繫結
            CheckDatabaseConnection();
            //InitScriptList();        // 建立腳本清單與事件
            //背景自動登入

            //查詢任務才用到
            //ApplyTaskRowStyles();


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

        private void CheckDatabaseConnection()
        {
            try
            {
                using (var conn = new SqlConnection(SQLConfig.DBConnection))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand("SELECT 1", conn))
                    {
                        _ = cmd.ExecuteScalar();
                    }
                }

                // 成功：變綠色
                btn_SQLstatus.BackColor = System.Drawing.Color.Lime;
                //btn_SQLstatus.ForeColor = System.Drawing.Color.White;
                btn_SQLstatus.Text = "資料庫連線成功 ";
            }
            catch (Exception ex)
            {
                // 
                btn_SQLstatus.BackColor = System.Drawing.Color.Red;
                //btn_SQLstatus.ForeColor = System.Drawing.Color.White;
                btn_SQLstatus.Text = "資料庫連線失敗 ";

                // 可選：顯示詳細錯誤
                MessageBox.Show($"資料庫連線失敗：{ex.Message}",
                    "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private async void LoadTasks_Click(object sender, EventArgs e)
        {
            try
            {
                var dt = await SQLDatabase.QueryTasksTableAsync();
                DGV_Tasks.AutoGenerateColumns = true;
                DGV_Tasks.DataSource = dt;
                DGV_Tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                //  啟用自動排序
                foreach (DataGridViewColumn column in DGV_Tasks.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                }
                DGV_Tasks.Columns["TaskName"].HeaderText = "任務名稱";
                DGV_Tasks.Columns["Action"].HeaderText = "動作";
                DGV_Tasks.Columns["RecieveTime"].HeaderText = "接收時間";
                DGV_Tasks.Columns["StartTime"].HeaderText = "開始時間";
                DGV_Tasks.Columns["FinishTime"].HeaderText = "完成時間";
                DGV_Tasks.Columns["State"].HeaderText = "任務狀態";
            }
            catch (Exception ex)
            {
                MessageBox.Show("查詢失敗：" + ex.Message);
            }
        }


        // 3) 綁在 Form_Load（或建構子）
        //    確保每次排序都會套色

        // 4) 套色邏輯：>1分鐘未完成=紅；未完成(<=1分鐘)=黃；完成=綠
        private void ApplyRunningTask()
        {
            foreach (DataGridViewRow row in DGV_Tasks.Rows)
            {
                if (row.IsNewRow) continue;

                int state = GetCellInt(row, "State");

                if (state == 1)
                {
                    row.DefaultCellStyle.BackColor = Color.Lime;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (state == 5)
                {
                    row.DefaultCellStyle.BackColor = Color.Orange;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
            }
        }
        private void ApplyTaskRowStyles()
        {
            var threshold = TimeSpan.FromMinutes(1);
            var now = DateTime.Now;

            const string COL_RECIEVE = "RecieveTime";
            const string COL_STATE = "State";

            DGV_Tasks.SuspendLayout();

            foreach (DataGridViewRow row in DGV_Tasks.Rows)
            {
                if (row.IsNewRow) continue;

                DateTime? recieve = GetCellDateTime(row, COL_RECIEVE);
                int state = GetCellInt(row, COL_STATE);

                bool isOvertime = recieve.HasValue && (now - recieve.Value) > threshold;
                bool markRed = isOvertime && state == 1;

                // 預設顏色
                row.DefaultCellStyle.BackColor = Color.White;
                row.DefaultCellStyle.ForeColor = Color.Black;

                if (markRed)
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    row.DefaultCellStyle.ForeColor = Color.DarkRed;
                }
                row.Tag = markRed ? 1 : 0;
            }

            DGV_Tasks.ResumeLayout();

            //標記任務排在最上面
            if (DGV_Tasks.DataSource is DataTable dt)
            {
                //清除排序
                dt.DefaultView.Sort = "";
                //if( !dt.Columns.Contains("_IsOvertime"))
                //    dt.Columns.Add("_IsOvertime", typeof(int));
                foreach (DataRow dr in dt.Rows)
                {
                    var recieve = dr[COL_RECIEVE] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dr[COL_RECIEVE]);
                    var state = dr[COL_STATE] == DBNull.Value ? 0 : Convert.ToInt32(dr[COL_STATE]);
                    bool isOvertime = recieve.HasValue && (now - recieve.Value) > threshold;
                    bool markRed = isOvertime && state == 1;
                    //  dr["_IsOvertime"] = markRed ? 1 : 0;
                }
                // 讓紅色的在最上面，再依 RecieveTime DESC
                //dt.DefaultView.Sort = "_IsOvertime DESC, RecieveTime DESC";
                //DGV_Tasks.DataSource = dt.DefaultView;
            }
            else if (DGV_Tasks.DataSource is BindingSource bs && bs.DataSource is DataTable bdt)
            {
                // 若你的 DGV 用 BindingSource 包 DataTable
                //bdt.DefaultView.Sort = "_IsOvertime DESC, RecieveTime DESC";
                //bs.DataSource = bdt.DefaultView;
                //DGV_Tasks.DataSource = bs;
            }
        }


        // 小工具：安全取值
        private static DateTime? GetCellDateTime(DataGridViewRow row, string col)
        {
            if (!row.DataGridView.Columns.Contains(col)) return null;
            var val = row.Cells[col].Value;
            if (val == null || val == DBNull.Value) return null;
            if (val is DateTime dt) return dt;
            if (DateTime.TryParse(Convert.ToString(val, CultureInfo.InvariantCulture), out var parsed)) return parsed;
            return null;
        }

        private static int GetCellInt(DataGridViewRow row, string col)
        {
            if (!row.DataGridView.Columns.Contains(col)) return 0;
            var val = row.Cells[col].Value;
            if (val == null || val == DBNull.Value) return 0;
            if (val is int i) return i;
            int.TryParse(val.ToString(), out var parsed);
            return parsed;
        }
        private async void btnLoadTasks_Click(object sender, EventArgs e)
        {
            btn_taskquery.Enabled = false;
            var oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                // 1) 查資料（建議回傳 DataTable）
                var table = await SQLDatabase.QueryTasksTableAsync();

                // 2) 綁定到 DGV
                DGV_Tasks.SuspendLayout();
                DGV_Tasks.AutoGenerateColumns = true;
                DGV_Tasks.DataSource = table;
                DGV_Tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                // 3) 開啟每欄可點擊排序（升/降冪）
                foreach (DataGridViewColumn col in DGV_Tasks.Columns)
                    col.SortMode = DataGridViewColumnSortMode.Automatic;
                DGV_Tasks.ResumeLayout();

                // 4) 依規則：現在時間 - RecieveTime > 1 分鐘 且 State == 6
                //    → 標紅並「自動置頂」
                ApplyRunningTask();
                //ApplyTaskRowStyles();   // ← 這裡會同時上色與把紅色排最上面
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Cursor.Current = oldCursor;
                btn_taskquery.Enabled = true;
            }
        }
        private void establishSQL()
        {
            try
            {
                SQLDatabase.Initialize();
            }
            catch (Exception)
            {

                throw;
            }
            ApplicationConfiguration.Initialize();
            Application.Run(new FrmMain());
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

        private bool _isWiringOrLoading = false;

        private void WireEvents()
        {
            // Map → Scripts
            listMapBox.SelectedIndexChanged += (_, __) =>
            {
                if (_isWiringOrLoading) return;

                try
                {
                    _isWiringOrLoading = true;

                    var map = listMapBox.SelectedItem as MapDto;

                    var scripts = (map?.Scripts != null && map.Scripts.Count > 0)
                        ? new BindingList<ScriptDto>(map.Scripts)
                        : new BindingList<ScriptDto>();

                    _scriptBS.DataSource = scripts;
                    lstScripts.DataSource = _scriptBS;
                    lstScripts.DisplayMember = nameof(ScriptDto.ScriptName);

                    // 清空任務表（避免殘留上一個 map 的任務）
                    _tasksBS.DataSource = new BindingList<TaskItemDto>();
                    DGV_Script.DataSource = _tasksBS;

                    // ✅ 自動選第一個腳本（用 BindingSource 比較穩）
                    if (scripts.Count > 0)
                    {
                        _scriptBS.Position = 0; // 等同選第一筆
                        LoadTasksFromSelectedScript(); // ✅ 保證任務會顯示
                    }
                    else
                    {
                        lstScripts.SelectedIndex = -1;
                    }
                }
                finally
                {
                    _isWiringOrLoading = false;
                }
            };

            // Script → Tasks (DGV)
            lstScripts.SelectedIndexChanged += (_, __) =>
            {
                if (_isWiringOrLoading) return;
                LoadTasksFromSelectedScript();
            };
        }

        private void LoadTasksFromSelectedScript()
        {
            var script = lstScripts.SelectedItem as ScriptDto;

            var tasks = (script?.Tasks != null && script.Tasks.Count > 0)
                ? new BindingList<TaskItemDto>(script.Tasks)
                : new BindingList<TaskItemDto>();

            _tasksBS.DataSource = tasks;
            DGV_Script.DataSource = _tasksBS;
        }

        private void InitGrid()
        {
            DGV_Script.Columns.Clear();
            DGV_Script.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            DGV_Script.EditMode = DataGridViewEditMode.EditOnEnter;
            DGV_Script.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            var colNo = new DataGridViewTextBoxColumn
            {
                Name = "colNo",
                HeaderText = "No",
                DataPropertyName = nameof(TaskItemDto.No),          // 若你沒有這個屬性，改成自動編號見下方備註
                ReadOnly = true,
                FillWeight = 25
            };
            DGV_Script.Columns.Add(colNo);
            colNo.DisplayIndex = 0;
            DGV_Script.CellFormatting += (s, e) =>
            {
                if (DGV_Script.Columns[e.ColumnIndex].Name == "colNo" && e.RowIndex >= 0)
                    e.Value = (e.RowIndex + 1).ToString();
            };

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
                MessageBox.Show("請先選擇一筆欲執行腳本", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private string GetCellStringSafe(DataGridViewRow row, string wantedNameOrHeader)
        {
            var dgv = row.DataGridView;

            // 1) 先嘗試用欄位 Name
            if (dgv.Columns.Contains(wantedNameOrHeader))
            {
                var val = row.Cells[wantedNameOrHeader].Value;
                return val?.ToString();
            }

            // 2) 再嘗試以 HeaderText 比對（不分大小寫）
            var colByHeader = dgv.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.HeaderText, wantedNameOrHeader, StringComparison.OrdinalIgnoreCase));
            if (colByHeader != null)
            {
                var val = row.Cells[colByHeader.Index].Value;
                return val?.ToString();
            }

            // 3) 再嘗試以 DataPropertyName 比對（若是綁定資料來源）
            var colByDp = dgv.Columns
                .Cast<DataGridViewColumn>()
                .FirstOrDefault(c => string.Equals(c.DataPropertyName, wantedNameOrHeader, StringComparison.OrdinalIgnoreCase));
            if (colByDp != null)
            {
                var val = row.Cells[colByDp.Index].Value;
                return val?.ToString();
            }

            // 4) 都沒找到，回 null
            return null;
        }
        private async void btn_Scripts_Click(object snder , EventArgs e)
        {
            if (DGV_Script.Rows.Count == 0)
            {
                MessageBox.Show("任務列表無任務，請新增任務","任務列表錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if (isTaskNull())
            {
                MessageBox.Show("請確認任務列表是否有空值","任務列表錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (login_status.BackColor == Color.Red)
            {
                MessageBox.Show("派車系統未連線，無法執行任務","連線錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            Locate_task_AGV();
            await Task.Delay(2000);
            move_task_click();
        }
        private async void Auto_RunScripts_Click(object sender, EventArgs e)
        {
            if (DGV_Script.Rows.Count == 0)
            {
                MessageBox.Show("任務列表無任務，請新增任務","任務列表錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            if (isTaskNull())
            {
                 MessageBox.Show("請確認任務列表是否有空值","任務列表錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (login_status.BackColor == Color.Red)
            {
                MessageBox.Show("派車系統未連線，無法執行任務","連線錯誤",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            if (login_status.BackColor == Color.Lime)
            {
                Locate_task_AGV();
                await Task.Delay(3000);
                //Thread.Sleep(3000);
                move_task_click();
            }

            await RunCurrentScriptAsync();

            // ✅ 跑完後，問要不要跑下一個（腳本二）
            AskRunNextScriptIfAny();

        }
        private async Task RunCurrentScriptAsync()
        {
            // 送出任務（你現有流程）
            Locate_task_AGV();
            await Task.Delay(3000); // ✅ 不要 Thread.Sleep
            move_task_click();

            // ✅ 等待腳本完成（你要依你的系統實作完成判斷）
            // 例如：輪詢 DB state=1 的任務全部完成 or 逾時
            await WaitScriptFinishedAsync(timeout: TimeSpan.FromMinutes(5));
        }
        private async Task WaitScriptFinishedAsync(TimeSpan timeout)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                if (IsScriptFinished())
                    return;

                await Task.Delay(2000);
            }

            // 超時也視為結束（或你要提示）
            MessageBox.Show("腳本執行逾時（5分鐘），已結束等待。", "提示",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// /需要確認腳本是否完成
        /// </summary>
        /// <returns></returns>
        private bool IsScriptFinished()
        {
            // ✅ 你可以用：
            // 1) 查 DB：這次下的任務是否都 Finish/State!=1
            // 2) API 回報：所有任務完成事件都收到了
            // 3) 你自己的 HotRunManager：有沒有收到腳本結束訊號

            // 先給你假資料，請改成真正判斷
            return true;
        }
        private void AskRunNextScriptIfAny()
        {
            int idx = lstScripts.SelectedIndex;
            if (idx < 0) return;

            int nextIdx = idx + 1;
            if (nextIdx >= lstScripts.Items.Count) return; // 沒有下一個

            var nextScript = lstScripts.Items[nextIdx] as ScriptDto;
            var nextName = nextScript?.ScriptName ?? $"腳本{nextIdx + 1}";

            var result = MessageBox.Show(
                $"腳本已執行完畢。\n是否要執行下一個腳本：{nextName}？",
                "執行下一個腳本",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                lstScripts.SelectedIndex = nextIdx; // 切換到腳本二（任務列表會自動更新）
                Auto_RunScripts_Click(this, EventArgs.Empty); // 直接再跑一次（或改成呼叫 RunCurrentScriptAsync）
            }
        }
        private bool isTaskNull()
        {
            foreach(DataGridViewRow row in DGV_Script.Rows)
            {
                if (row.IsNewRow) continue;
                string agvName = GetCellStringSafe(row, "AGVName"); // 可以用欄位名稱或 header text
                string start = GetCellStringSafe(row, "Start");
                string End = GetCellStringSafe(row, "End");
                if (string.IsNullOrEmpty(agvName) || string.IsNullOrEmpty(start) || string.IsNullOrEmpty(End))
                {
                    return true;
                }
            }
            return false;
        }
        private async void Locate_task_AGV()
        {
            var dgv = this.DGV_Script; // 改成你實際的 control 名
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string agvName = GetCellStringSafe(row, "AGVName"); // 可以用欄位名稱或 header text
                string start = GetCellStringSafe(row, "Start");

                if (string.IsNullOrEmpty(agvName) || string.IsNullOrEmpty(start))
                {
                    continue;
                }
                try
                {
                    var res = await APIController.APIAGVLocate(agvName, start); // 你已有的 API helper
                }
                catch (Exception ex)
                {
                    // row.Cells["Status"].Value = $"錯誤: {ex.Message}";
                }
            }
        }


        private async void move_task_click()
        {
            var dgv = this.DGV_Script; // 改成你實際的 control 名
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string agvName = GetCellStringSafe(row, "AGVName"); // 可以用欄位名稱或 header text
                string End = GetCellStringSafe(row, "End");

                if (string.IsNullOrEmpty(agvName) || string.IsNullOrEmpty(End))
                {
                    continue;
                }
                try
                {
                    await AgvsClient.PostMoveAsync(agvName, End, false);
                }
                catch (Exception ex)
                {
                    // row.Cells["Status"].Value = $"錯誤: {ex.Message}";
                }
            }
        }
        private async Task ReloadTasklist()
        {
            //var table = await SQLDatabase.QueryTasksTableAsync();

            //// 2) 綁定到 DGV
            //DGV_Tasks.SuspendLayout();
            //DGV_Tasks.AutoGenerateColumns = true;
            //DGV_Tasks.DataSource = table;
            //DGV_Tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            // 3) 開啟每欄可點擊排序（升/降冪）

            // DGV_Tasks.ResumeLayout();
            try
            {
                //var table = await SQLDatabase.QueryTasksAsync();
                var table = await SQLDatabase.QueryTasksTableAsync();
                
                DGV_Tasks.SuspendLayout();
                DGV_Tasks.AutoGenerateColumns = true;
                DGV_Tasks.DataSource = table;
                DGV_Tasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                foreach (DataGridViewColumn col in DGV_Tasks.Columns)
                    col.SortMode = DataGridViewColumnSortMode.Automatic;
            }
            catch (Exception)
            {

                throw;
            }
        }
        private async void CancelUNdoneTask_Click(object sender, EventArgs e)
        {

            try
            {
                // 1) 向資料庫查詢 State = 1 的任務
                DataTable dt = await SQLDatabase.QueryUNdoneTasksAsync(1);
                var taskname = dt.AsEnumerable()
                                 .Select(r => r.Field<string>("TaskName"))
                                 .Where(s => !string.IsNullOrWhiteSpace(s))
                                 .ToList();
                //if (dt == 0)
                //{
                //    MessageBox.Show("查無狀態為 1 的任務。", "訊息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    return;
                //}
                if (taskname.Count != 0)
                {
                    // 2) 顯示確認（列出前幾筆）
                    var preview = string.Join("\r\n", taskname.Take(5));
                    var more = taskname.Count > 5 ? $"\r\n... 共 {taskname.Count} 筆" : $"（共 {taskname.Count} 筆）";
                    var confirm = MessageBox.Show(
                        $"確定要取消下列 State=1 的任務？\r\n{preview}\r\n{more}",
                        "確認取消",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question);

                    if (confirm != DialogResult.OK) return;

                    // 3) 逐一呼叫後端取消（建議序列化處理，避免後端過載）
                    int okCount = 0, failCount = 0;
                    var sbFail = new System.Text.StringBuilder();

                    foreach (var name in taskname)
                    {
                        var ret = await AgvsClient.CancelTaskAsync(name, reason: "取消交管失敗任務", raiserName: Environment.UserName);
                        if (ret.OK && ret.Data == true)
                        {
                            okCount++;
                        }
                        else
                        {
                            failCount++;
                            sbFail.AppendLine($"{name} -> {ret.Error ?? "後端回傳 false"}");
                        }
                    }

                    // 4) 顯示結果
                    var msg = $"取消完成：成功 {okCount} 筆，失敗 {failCount} 筆。";
                    if (failCount > 0) msg += "\r\n\r\n失敗清單：\r\n" + sbFail.ToString();
                    MessageBox.Show(msg, "批次取消結果", MessageBoxButtons.OK,
                        failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

                    // 5) 選擇性：刷新畫面
                    // await ReloadTasksAsync(); // 若你有查詢刷新方法就呼叫
                }
                if (taskname.Count == 0)
                {
                    var cancelbox = MessageBox.Show(
                       $"任務皆已經完成");
                    if (cancelbox != DialogResult.OK) return;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("執行取消時發生錯誤：\r\n" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            await Task.Delay(2000);
            await ReloadTasklist();
            //先加上去不要顯示
            //DGV_Tasks.Columns["DispatcherName"].Visible = false;
        }




        private List<TaskItemDto> GetTasksFromGrid()
        {
            if (DGV_Script.DataSource is BindingSource bs)
                return bs.Cast<TaskItemDto>().ToList();

            var list = new List<TaskItemDto>();
            foreach (DataGridViewRow r in DGV_Script.Rows)
            {
                if (r.IsNewRow) continue;
                if (r.DataBoundItem is TaskItemDto dto) list.Add(dto);
            }
            return list;
        }

        private static bool SkipTag(string? s)
            => string.IsNullOrWhiteSpace(s) || s.Trim() == "-1";

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
        //地圖加上腳本
        //private MultiMapRoot _scriptConfig;
        private MapDto _selectedMap;
        private ScriptDto _selectedScript;

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            // 取得程式執行目錄 (bin\Debug\netX.0)
            string debugFolder = Application.StartupPath;
            string filePath = Path.Combine(debugFolder, "multi-maps_test.json");

            var result = MessageBox.Show(
                $"是否要將腳本儲存到：\n{filePath}？",
                "確認儲存",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            var opt = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(filePath, JsonSerializer.Serialize(_data, opt), System.Text.Encoding.UTF8);

            MessageBox.Show("已成功儲存。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void add_task(object sender, EventArgs e)
        {
            //if (lstScripts.SelectedItem is not ScriptDto) { MessageBox.Show("請先選擇一個腳本"); return; }
            //if (_tasksBS.List is BindingList<TaskItemDto> list)
            //{
            //    list.Add(new TaskItemDto { Start = "0", Action = _actionOptions[0], End = "0" });
            //}
            if (lstScripts.SelectedItem is not ScriptDto s)
            {
                MessageBox.Show("請先選擇一個腳本");
                return;
            }

            var newTask = new TaskItemDto
            {
                Start = "0",
                Action = _actionOptions[0],
                End = "0"
            };

            // ✅ 先加到 _data 本體（真正會被 Serialize 的地方）
            s.Tasks.Add(newTask);

            // ✅ 再讓畫面更新：如果目前 DGV 的資料來源是 BindingList，就同步加進去
            //if (_tasksBS.List is BindingList<TaskItemDto> bl)
            //{
            //    bl.Add(newTask);
            //}

                // 如果不是 BindingList，乾脆直接重綁一次
                _tasksBS.DataSource = new BindingList<TaskItemDto>(s.Tasks);
                DGV_Script.DataSource = _tasksBS;
        }

        private void Delete_task(object sender, EventArgs e)
        {
            if (DGV_Script.CurrentRow?.DataBoundItem is TaskItemDto item &&
                _tasksBS.List is BindingList<TaskItemDto> list)
            {
                var dr = MessageBox.Show(
                $"確定要刪除任務 \" 嗎？\n（此動作無法復原）",
                "確認刪除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

                if (dr == DialogResult.Yes)
                {
                    list.Remove(item);
                }
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
            if (!(listMapBox.SelectedItem is MapDto m))
            {
                MessageBox.Show("請先選取要刪除的地圖。", "無選取項目", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 嘗試取得 Map 名稱屬性（容錯：若沒有 MapName 屬性就用 ToString()）
            var prop = m.GetType().GetProperty("MapName") ?? m.GetType().GetProperty("mapName");
            var mapName = prop != null ? (prop.GetValue(m)?.ToString() ?? "") : m.ToString();

            var dr = MessageBox.Show(
                $"確定要刪除地圖 \"{mapName}\" 嗎？\n（此動作無法復原）",
                "確認刪除",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
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

            // ✅ 檢查是否有相同名稱（不分大小寫）
            bool isDuplicated = m.Scripts.Any(s =>
                string.Equals(s.ScriptName, name, StringComparison.OrdinalIgnoreCase));

            if (isDuplicated)
            {
                MessageBox.Show(
                    $"已存在相同名稱的腳本：{name}",
                    "新增失敗",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            var s = new ScriptDto { ScriptName = name, Tasks = new List<TaskItemDto>() };
            m.Scripts.Add(s);
            _scriptBS.ResetBindings(false);
            lstScripts.SelectedItem = s;
        }

        private void btnDeleteScript_Click(object sender, EventArgs e)
        {
            if (!(listMapBox.SelectedItem is MapDto m))
            {
                MessageBox.Show("請先選取要操作的地圖。", "無選取項目", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 檢查是否有選 script
            if (!(lstScripts.SelectedItem is ScriptDto s))
            {
                MessageBox.Show("請先選取要刪除的腳本。", "無選取項目", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 嘗試取得腳本名稱（容錯：ScriptName 或 scriptName）
            var prop = s.GetType().GetProperty("ScriptName") ?? s.GetType().GetProperty("scriptName");
            var scriptName = prop != null ? (prop.GetValue(s)?.ToString() ?? "") : s.ToString();

            var dr = MessageBox.Show(
                $"確定要刪除腳本 \"{scriptName}\" 嗎？\n（此動作無法復原）",
                "確認刪除腳本",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                m.Scripts.Remove(s);
                _scriptBS.ResetBindings(false);

                // 刪除後調整選取（若還有腳本則選第一筆，否則清除選取）
                if (m.Scripts.Count > 0)
                    lstScripts.SelectedIndex = 0;
                else
                    lstScripts.SelectedIndex = -1;
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