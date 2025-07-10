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

namespace AutoProjectSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string currentFolderPath = "";
        private AGVSController APIController = new AGVSController();
        private HotRunController HotRunController = new HotRunController();

        // 取得 HotRunScripts 並顯示在 DataGridView
        private async void btn_LoadHotRunScripts_Click(object sender, EventArgs e)
        {
            //var scripts = await HotRunController.GetHotRunScriptsAsync();
            //// 將資料繫結到 DataGridView（假設名稱為 DGV_HotRunlist）
            //textBox1.Text = scripts.ToString();
            // textBox1.Text = string.Join(Environment.NewLine , scripts.Select(s => $"Id: {s.Id}, Name: {s.Name}"));

            LoadHotRunScriptsToGrid();
        }
        private async void LoadHotRunScriptsToGrid()
        {
            var scripts = await HotRunController.GetHotRunScriptsAsync(); // 你原本的 API 呼叫

            var hoturn_list = scripts.Select(s => new
            {
                No = s.no,
                ScriptID = s.scriptID,
                AGV = s.agv_name,
                LoopNum = s.loop_num,
                FinishNum = s.finish_num,
                State = s.state,
                Comment = s.comment,
                RealTimeMessage = s.RealTimeMessage,
                ActionCount = s.actions?.Count ?? 0,
                IsRandom = s.IsRandomCarryRun,
                IsRegularUnload = s.IsRegularUnloadRequst
            }).ToList();

            DGV_HotRunlist.DataSource = hoturn_list;
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

            await HotRunController.CallHotRunApiAsync(scriptID);
            //if (selectedRow.DataBoundItem is HotRunScript hotRunScript)
            //{
            //    string scriptID = hotRunScript.scriptID;

            //    // 呼叫 API 執行 HotRun
            //    await HotRunController.CallHotRunApiAsync(scriptID);
            //}

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
        private void btn_chooseproject_Click2(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "選擇JSON檔案";
                openFileDialog.Filter = "JSON檔案 (*.json)|*.json|所有檔案 (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    textBox_appsetting.Text = Path.GetFileName(filePath);

                    // 讀取並解析JSON
                    try
                    {
                        string jsonText = File.ReadAllText(filePath);
                        textBox_content.Clear();

                        // 使用 JsonNode 解析並遞迴顯示
                        var node = JsonNode.Parse(jsonText);
                        DisplayJsonNode(node, "");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("讀取或解析JSON失敗: " + ex.Message);
                    }
                }
            }
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
            //APIController.APIAGVStatus();
            richTextBox_AGVStatus.AppendText(result + Environment.NewLine);
            //string result = await APIController.APITestAsync();
            //richTextBox_content.Text = result;
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
                //richTextBox_content.Text = response;
                richTextBox_content.AppendText(response + Environment.NewLine);
                //APIController.RestartAGVS();
                MessageBox.Show("已執行重啟派車系統。");
            }
            else
            {
                // 取消，不做任何事
                MessageBox.Show("已取消，下次別亂按");
            }
        }
    }
}