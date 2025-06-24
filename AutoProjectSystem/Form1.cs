using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
namespace AutoProjectSystem
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private string currentFolderPath = "";
        private void Form1_Load(object sender, EventArgs e)
        {
            //string folderPath = @"C:\Users\user\Desktop\測試環境";
            //if (Directory.Exists(folderPath))
            //{
            //    string[] files = Directory.GetFiles(folderPath);
            //    comboBox_project.Items.Clear();
            //    foreach (var file in files)
            //    {
            //        comboBox_project.Items.Add(Path.GetFileName(file));
            //    }
            //}
        }
        private void btn_chooseproject_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "選擇檔案";
                openFileDialog.Filter = "所有檔案 (*.*)|*.*"; // 可依需求調整檔案類型

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // 只顯示檔案名稱
                    //textBox_appsetting.Text = Path.GetFileName(openFileDialog.FileName);
                    textBox_appsetting.Text = openFileDialog.FileName;
                    // 若要顯示完整路徑，請改為 openFileDialog.FileName
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
       
        
        
    }
}
