using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using NLog;
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
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
            logger.Info("畫面載入完成");
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
                logger.Info("資料庫斷線");
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
            if (btn_SQLstatus.BackColor == Color.Red)
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error("載入任務失敗");
            }
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
                logger.Warn(ex);
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                Cursor.Current = oldCursor;
                btn_taskquery.Enabled = true;
            }
        }
        private bool isSQL_Connected()
        {
            return btn_SQLstatus.BackColor == Color.Lime;
        }
        private bool isAGVS_Connected()
        {
            return login_status.BackColor == Color.Lime;
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
            logger.Info("載入地圖資料完成");
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
        private void btn_chooseproject_Click(object sender, EventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                logger.Warn(ex);
                throw;
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
                logger.Info("派車系統登入成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show("登入失敗：\r\n" + ex.Message, "錯誤",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex, "派車系統登入失敗");
            }
        }
        //移動任務測試
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
                logger.Warn(ex);
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
        private async void btn_Scripts_Click(object snder, EventArgs e)
        {
            if (DGV_Script.Rows.Count == 0)
            {
                MessageBox.Show("任務列表無任務，請新增任務", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (isTaskNull())
            {
                MessageBox.Show("請確認任務列表是否有空值", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行任務", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ///執行列表任務的第一筆任務
            await Task_runAsync();
        }
        //0204自動腳本任務
        private CancellationTokenSource? _autoCts;
        private bool _autoRunning = false;
        private async void Auto_RunScripts(object sender, EventArgs e)
        {
            //if (_autoRunning) return;
            var confirm = MessageBox.Show(
                "是否要啟動『自動執行腳本』功能？\n" +
                "（將依序執行目前選取腳本之後的所有腳本）",
                "啟動自動腳本",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                logger.Info("自動執行腳本任務取消(尚未開始)");
                return; // ❌ 不執行後續流程
            }
            if (_autoRunning)
            {
                MessageBox.Show("自動腳本執行中。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                logger.Info("自動腳本執行開始");
                return;
            }
            // 2) 啟動自動流程
            _autoRunning = true;
            btn_AutoRunTask.Enabled = false;

            _autoCts?.Cancel();
            _autoCts?.Dispose();
            _autoCts = new CancellationTokenSource();

            try
            {
                await RunAutoScriptsFlowAsync(_autoCts.Token);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("自動腳本已取消。", "取消", MessageBoxButtons.OK, MessageBoxIcon.Information);
                logger.Info("自動腳本任務取消");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "執行錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Warn(ex);
            }
            finally
            {
                _autoRunning = false;
                btn_AutoRunTask.Enabled = true;
            }
        }
        private async Task RunAutoScriptsFlowAsync(CancellationToken ct)
        {

            // 1) 基本檢查
            if (DGV_Script.Rows.Count == 0)
            {
                MessageBox.Show("任務列表無任務，請新增任務", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (isTaskNull())
            {
                MessageBox.Show("請確認任務列表是否有空值", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行任務", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!(login_status.BackColor == Color.Lime && btn_SQLstatus.BackColor == Color.Lime))
            {
                MessageBox.Show("AGVS 或 SQL 未連線", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //要選腳本
            if (lstScripts.SelectedIndex < 0)
            {
                MessageBox.Show("請先選擇一個腳本開始。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int idx = lstScripts.SelectedIndex;

            // 自動腳本開始：從目前選到的腳本一路往下跑
            while (idx < lstScripts.Items.Count)
            {
                ct.ThrowIfCancellationRequested();

                // 腳本A開始（或當前腳本）
                lstScripts.SelectedIndex = idx; // 這行會讓你的 DGV 任務清單切到該腳本資料（如果你已做 SelectedIndexChanged 綁定）
                var script = lstScripts.Items[idx] as ScriptDto;
                string scriptName = script?.ScriptName ?? $"腳本{idx + 1}";

                // 先下任務（你原本的 Task_runAsync）
                await Task_runAsync();

                // 監控任務是否結束：每 5~10 秒輪詢一次，超時 3 分鐘視為失敗並取消任務

                //記得標記LOG
                bool ok = await MonitorScriptTasksAsync(
                    scriptName,
                    timeout: TimeSpan.FromMinutes(3),
                    pollInterval: TimeSpan.FromSeconds(5),
                    ct: ct
                );

                if (!ok)
                {
                    // 腳本A任務取消（照你的圖）
                    await CancelTasksByStatesAsync(1, 5);

                    MessageBox.Show(
                        $"[{scriptName}] 超過 3 分鐘仍有 State=1 任務，已判定失敗並取消未完成任務，將詢問是否執行下一個腳本。",
                        "腳本失敗",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                // 腳本A任務結束（成功或失敗都算結束），跳出詢問視窗：是否執行腳本B
                int nextIdx = idx + 1;
                if (nextIdx >= lstScripts.Items.Count)
                {
                    MessageBox.Show("已無下一個腳本，自動腳本結束。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var nextScript = lstScripts.Items[nextIdx] as ScriptDto;
                var nextName = nextScript?.ScriptName ?? $"腳本{nextIdx + 1}";

                var result = MessageBox.Show(
                    $"[{scriptName}] 已結束。\n是否要執行下一個腳本：{nextName}？",
                    "執行下一個腳本",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    // 自動腳本結束（照圖）
                    MessageBox.Show("已停止自動腳本。", "結束", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 腳本B任務開始：回到 while 迴圈下一輪
                idx = nextIdx;
            }
        }
        private async Task<bool> MonitorScriptTasksAsync(
            string scriptName,
            TimeSpan timeout,
            TimeSpan pollInterval,
            CancellationToken ct)
        {
            var sw = System.Diagnostics.Stopwatch.StartNew();

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                // 查 State=1、State=5 各取 1 筆就好（只需要知道「有沒有」）
                var dt1 = await SQLDatabase.QueryUNdoneTasksAsync(state: 1, top: 1);
                var dt5 = await SQLDatabase.QueryUNdoneTasksAsync(state: 5, top: 1);

                bool hasState1 = dt1 != null && dt1.Rows.Count > 0;
                bool hasState5 = dt5 != null && dt5.Rows.Count > 0;

                bool hasRunning = hasState1 || hasState5;

                // ✅ 沒有 State=1 也沒有 State=5 → 腳本完成
                if (!hasRunning)
                    return true;

                // ❌ 超過 3 分鐘仍有 State=1 或 State=5 → 判定失敗
                if (sw.Elapsed >= timeout)
                {
                    //MessageBox.Show(
                    //    $"[{scriptName}] 超過 {timeout.TotalMinutes:0} 分鐘仍有 State=1/5 任務，判定此腳本失敗。",
                    //    "腳本失敗",
                    //    MessageBoxButtons.OK,
                    //    MessageBoxIcon.Warning);
                    ///需記LOG
                    logger.Warn(scriptName + "超過時間", "此腳本失敗");
                    return false;
                }

                // 每次輪詢間隔
                await Task.Delay(pollInterval, ct);
            }
        }
        private CancellationTokenSource _cancelCts;
        private async void btn_CancelTasks_Click(object sender, EventArgs e)
        {
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isSQL_Connected())
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            logger.Info("使用者點擊取消任務按鈕");
            // 先詢問使用者
            var confirm = MessageBox.Show(
                "確定要取消所有 State=1 與 State=5 的任務嗎？",
                "取消任務確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            // 防止重複點擊
            btn_CancelrunidleTask.Enabled = false;

            // 建立 CancellationTokenSource
            _cancelCts?.Cancel();
            _cancelCts?.Dispose();
            _cancelCts = new CancellationTokenSource();

            try
            {
                await CancelScriptTasksAsync(_cancelCts.Token);
                MessageBox.Show("任務取消請求已送出。", "完成",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                logger.Info("任務取消請求已送出");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("取消任務已被中止。", "取消",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                logger.Info("取消任務已被中止");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "執行錯誤",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Info(ex);
            }
            finally
            {
                btn_CancelrunidleTask.Enabled = true;
            }
        }
        private async Task CancelScriptTasksAsync(CancellationToken ct)
        {
            // 1) 先抓出目前所有 State=1 的 TaskName
            var dt1 = await SQLDatabase.QueryUNdoneTasksAsync(state: 1, top: null);
            var dt5 = await SQLDatabase.QueryUNdoneTasksAsync(state: 5, top: null);



            var allRows = new List<System.Data.DataRow>();
            if (dt1 != null && dt1.Rows.Count > 0)
                allRows.AddRange(dt1.AsEnumerable());

            if (dt5 != null && dt5.Rows.Count > 0)
                allRows.AddRange(dt5.AsEnumerable());

            if (allRows.Count == 0) return; // 沒有需要取消的任務

            // 2) 逐筆呼叫你現有的取消 API / Client（你自己替換成你實際的取消方法）
            foreach (var row in allRows)
            {
                ct.ThrowIfCancellationRequested();

                string taskName = row["TaskName"]?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(taskName)) continue;
                logger.Info("正在取消任務: " + taskName);
                try
                {
                    // TODO: 換成你實際的取消函數
                    // await AgvsClient.CancelTaskAsync(taskName);

                    await Task.Delay(10, ct); // 占位：避免你貼上後忘記替換仍可編譯
                }
                catch (Exception ex)
                {
                    logger.Info(ex);
                    // 取消失敗先不要整個流程炸掉，可視需求記 log
                }
            }
        }
        private async Task<bool> HasRunningTasksAsync(CancellationToken ct)
        {
            // 查詢 State = 1 或 State = 5 的任務
            var dt1 = await SQLDatabase.QueryUNdoneTasksAsync(state: 1, top: 1);
            var dt5 = await SQLDatabase.QueryUNdoneTasksAsync(state: 5, top: 1);

            bool hasState1 = dt1 != null && dt1.Rows.Count > 0;
            bool hasState5 = dt5 != null && dt5.Rows.Count > 0;

            return hasState1 || hasState5;
        }
        private async void Auto_RunScripts_Click(object sender, EventArgs e)
        {
            if (DGV_Script.Rows.Count == 0)
            {
                MessageBox.Show("任務列表無任務，請新增任務", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (isTaskNull())
            {
                MessageBox.Show("請確認任務列表是否有空值", "任務列表錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行任務", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (login_status.BackColor == Color.Lime && btn_SQLstatus.BackColor == Color.Lime)
            {
                await Task_runAsync();
            }

            //await RunCurrentScriptAsync();
            bool ok = await RunCurrentScriptAsync();

            if (!ok)
            {
                MessageBox.Show("腳本超過 3 分鐘仍有 State=1 任務，判定此腳本失敗，將執行下一個腳本。",
                    "腳本失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            // ✅ 跑完後，問要不要跑下一個（腳本二）
            AskRunNextScriptIfAny();

        }
        //執行任務列表任務
        private async Task Task_runAsync()
        {
            Locate_task_AGV();
            await Task.Delay(2000);
            //Thread.Sleep(3000);
            move_task_click();
            logger.Info("開始執行列表中的任務");
        }

        private async void test_hasrunningTASK(object sender, EventArgs e)
        {
            if (btn_SQLstatus.BackColor == Color.Red)
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                // 1) 查資料（建議回傳 DataTable）
                bool isRunning = await SQLDatabase.HasRunningOrIdleTaskAsync();

                if (isRunning)
                {

                    MessageBox.Show("有未完成的任務");
                }
                else if (isRunning == false)
                {
                    MessageBox.Show("無未執行的任務");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
        private async Task<bool> RunCurrentScriptAsync()
        {
            // TODO: 你原本這裡應該是「送出目前腳本的任務」
            // 例如：SendTasksToBackend(); / ExecuteSelectedScriptAsync();
            // 如果你已經在外層送出了，這裡就只負責等待即可。
            TimeSpan timeout = TimeSpan.FromMinutes(3);
            int pollMs = 2000; // 每 2 秒問一次（1~5 秒都可以）
            var start = DateTime.Now;

            while (DateTime.Now - start < timeout)
            {
                bool hasRunning = await SQLDatabase.HasTaskState1Async();
                if (!hasRunning)
                {
                    // ✅ 沒有 State=1 → 視為腳本完成
                    return true;
                }
                await Task.Delay(pollMs);
            }
            // ❌ 超過 3 分鐘還有 State=1 → 視為腳本失敗
            logger.Warn("腳本失敗，超過設定時間" + pollMs);
            return false;
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
                try
                {
                    lstScripts.SelectedIndex = nextIdx; // 切換到腳本二（任務列表會自動更新）
                    Auto_RunScripts_Click(this, EventArgs.Empty); // 直接再跑一次（或改成呼叫 RunCurrentScriptAsync）
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "執行下一個腳本失敗");
                    throw;
                }

            }
        }
        private bool isTaskNull()
        {
            foreach (DataGridViewRow row in DGV_Script.Rows)
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
                    logger.Info("將" + agvName + "定位在" + start);
                }
                catch (Exception ex)
                {
                    // row.Cells["Status"].Value = $"錯誤: {ex.Message}";
                    logger.Error(ex);
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
                    logger.Info("將" + agvName + "移動到" + End);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    // row.Cells["Status"].Value = $"錯誤: {ex.Message}";
                }
            }
        }
        private async Task ReloadTasklist()
        {
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
                logger.Info("重整任務列表");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "重整任務列表失敗");
                throw;
            }
        }
        private async void Cancel_idleTask_Click(object sender, EventArgs e)
        {
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isSQL_Connected())
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                DataTable dt = await SQLDatabase.QueryCancelTaskAsync(5);
                var taskname = dt.AsEnumerable().Select(r => r.Field<string>("TaskName"))
                                 .Where(s => !string.IsNullOrWhiteSpace(s))
                                 .ToList();
                if (taskname.Count == 0)
                {
                    var cancelbox = MessageBox.Show(
                       $"任務皆已經完成");
                    logger.Info("不用取消任務因為任務皆已完成");
                    if (cancelbox != DialogResult.OK) return;
                }
                if (taskname.Count != 0)
                {
                    // 2) 彈窗顯示取消的任務
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
                        logger.Info("取消idle任務，任務名稱:" + taskname);
                    }
                    // 4) 顯示結果
                    var msg = $"取消完成：成功 {okCount} 筆，失敗 {failCount} 筆。";
                    if (failCount > 0) msg += "\r\n\r\n失敗清單：\r\n" + sbFail.ToString();
                    MessageBox.Show(msg, "批次取消結果", MessageBoxButtons.OK,
                        failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "取消idle任務失敗");
                throw;
            }
            await Task.Delay(1000);
            ///刷新畫面
            await ReloadTasklist();
        }
        private async void Cancel_runningTask_Click(object sender, EventArgs e)
        {
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!isSQL_Connected())
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            try
            {
                // 1) 向資料庫查詢 State = 1 的任務
                DataTable dt = await SQLDatabase.QueryCancelTaskAsync(1);
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
                    logger.Info("取消running任務，任務名稱:" + taskname);
                    // 5) 選擇性：刷新畫面
                    // await ReloadTasksAsync(); // 若你有查詢刷新方法就呼叫
                }
                if (taskname.Count == 0)
                {
                    var cancelbox = MessageBox.Show(
                       $"任務皆已經完成");
                    logger.Info("不用取消任務因為任務皆已完成");
                    if (cancelbox != DialogResult.OK) return;
                }

            }

            catch (Exception ex)
            {
                MessageBox.Show("執行取消時發生錯誤：\r\n" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex, "取消running任務失敗");
            }
            await Task.Delay(2000);
            await ReloadTasklist();
            //先加上去不要顯示
            //DGV_Tasks.Columns["DispatcherName"].Visible = false;
        }
        private async void Cancel_runidleTask_Click(object sender, EventArgs e)
        {
            await CancelTasksByStatesAsync(1, 5);
        }
        private async Task CancelTasksByStatesAsync(params int[] states)
        {
            // 基本檢查
            if (!isAGVS_Connected())
            {
                MessageBox.Show("派車系統未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Warn("取消任務失敗，因為派車系統未連線");
                return;
            }
            if (!isSQL_Connected())
            {
                MessageBox.Show("資料庫未連線，無法執行動作", "連線錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Warn("取消任務失敗，因為資料庫未連線");
                return;
            }
            try
            {
                var taskNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var st in states.Distinct())
                {
                    DataTable dt = await SQLDatabase.QueryCancelTaskAsync(st);
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        continue;
                    }
                    foreach (DataRow row in dt.Rows)
                    {
                        var name = row.Field<string>("TaskName");
                        if (!string.IsNullOrWhiteSpace(name))
                            taskNames.Add(name.Trim());
                    }
                }
                ///tasknames 沒有取消的任務先繼續
                var list = taskNames.ToList();
                var preview = string.Join("\r\n", list.Take(5));
                var more = list.Count > 5 ? $"\r\n... 共 {list.Count} 筆" : $"（共 {list.Count} 筆）";

                string stateText = string.Join(", ", states.Distinct().OrderBy(x => x));
                var confirm = MessageBox.Show(
                    $"確定要取消下列 State={stateText} 的任務？\r\n{preview}\r\n{more}",
                    "確認取消",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question);
                if (confirm != DialogResult.OK) return;

                // 3) 逐筆取消
                int okCount = 0, failCount = 0;
                var sbFail = new System.Text.StringBuilder();

                foreach (var name in list)
                {
                    var ret = await AgvsClient.CancelTaskAsync(name, reason: "取消交管失敗任務", raiserName: Environment.UserName);
                    if (ret.OK && ret.Data == true)
                    {
                        okCount++;
                        logger.Info("取消任務" + name);
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
                logger.Info("取消任務數: {OkCount}", okCount);
                MessageBox.Show(
                    msg, "批次取消結果",
                    MessageBoxButtons.OK,
                    failCount == 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                // MessageBox.Show("執行取消時發生錯誤：\r\n" + ex, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                logger.Error(ex);
            }
            finally
            {
                await Task.Delay(1000);
                await ReloadTasklist();
            }
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
                        logger.Info("參數儲存成功，儲存路徑為:" + filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("保存配置文件時出錯: " + ex.Message);
                        logger.Error(ex, "保存配置文件失敗");
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
                logger.Info("重啟派車系統");
            }
            else
            {
                // 取消，不做任何事
                MessageBox.Show("已取消，下次別亂按");
                logger.Info("使用者取消重啟派車系統");
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
            if (lstScripts.SelectedItem is not ScriptDto s)
            {
                MessageBox.Show("請先選擇一個腳本");
                logger.Info("任務列表中新增任務");
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
                logger.Info("刪除地圖失敗，因為沒有選取地圖");
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
                logger.Info($"刪除地圖{m.GetType().Name}");
                _mapBS.ResetBindings(false);
            }
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
            logger.Info("新增腳本成功");
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
                logger.Info("刪除腳本:"+scriptName+"完成");
                // 刪除後調整選取（若還有腳本則選第一筆，否則清除選取）
                if (m.Scripts.Count > 0)
                    lstScripts.SelectedIndex = 0;
                else
                    lstScripts.SelectedIndex = -1;
            }

        }
    }
}