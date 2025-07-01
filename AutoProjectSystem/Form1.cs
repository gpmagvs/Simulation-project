using System.Windows.Forms;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

using System.Net.Http;
using System.Threading.Tasks;
using AutoProjectSystem.Controllers;
using System.Drawing.Text;
using System.Security.Cryptography.X509Certificates;

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





        private void Form1_Load(object sender, EventArgs e)
        {
            //string folderPath = @"C:\Users\user\Desktop\��������";
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
        private async void Load_HotRunlist(object sender, EventArgs e)
        {
           // HotRunController.Load_hotrunlist();
            var controller = new HotRunController();
            var scripts = await controller.GetHotRunScriptsAsync();
            DGV_HotRunlist.DataSource = scripts;
        }
        private void btn_chooseproject_Click(object sender, EventArgs e)
        {
            //if (listBoxFiles.SelectedItem is string filePath && File.Exists(filePath))
            //{
            //    string content = File.ReadAllText(filePath);
            //    textBoxContent.Text = content;
            //}
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "���JSON�ɮ�";
                openFileDialog.Filter = "JSON�ɮ� (*.json)|*.json|�Ҧ��ɮ� (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    // �ץ��G��ܧ�����|
                    textBox_appsetting.Text = filePath;

                    // Ū���øѪRJSON
                    try
                    {
                        string content = File.ReadAllText(filePath);
                        textBox_content.Text = content;

                        // �ϥ� JsonNode �ѪR�û��j���
                        //var node = JsonNode.Parse(jsonText);
                        //DisplayJsonNode(node, "");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ū���θѪRJSON����: " + ex.Message);
                    }
                }
            }
        }
        private void btn_chooseproject_Click2(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "���JSON�ɮ�";
                openFileDialog.Filter = "JSON�ɮ� (*.json)|*.json|�Ҧ��ɮ� (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    textBox_appsetting.Text = Path.GetFileName(filePath);

                    // Ū���øѪRJSON
                    try
                    {
                        string jsonText = File.ReadAllText(filePath);
                        textBox_content.Clear();

                        // �ϥ� JsonNode �ѪR�û��j���
                        var node = JsonNode.Parse(jsonText);
                        DisplayJsonNode(node, "");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ū���θѪRJSON����: " + ex.Message);
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
                // �N���e�[�� textBoxContent
                textBox_content.AppendText($"{prefix}{val.ToJsonString()}{Environment.NewLine}");
            }
        }
        private async void btn_APItest_Click(object sender, EventArgs e)
        {
            // string net = "locaohost:5216";  // �w�] URL�A�i�H�q��L�������Τ��J�� URL;
            APIController.APITestAsync();


            string result = await APIController.APITestAsync();
            richTextBox_content.Text = result;

        }
        private async void btn_AGVSLocate_Click(object sender, EventArgs e)
        {
            string agvname = textBox_AGVName.Text.Trim();
            string location = textBox_Location.Text.Trim();
            string result = await APIController.APIAGVLocate(agvname , location);
            //APIController.APIAGVStatus();
            richTextBox_AGVStatus.AppendText(result + Environment.NewLine);


            //string result = await APIController.APITestAsync();
            //richTextBox_content.Text = result;
        }
        private async void btn_ConfigSave_Click(object sender, EventArgs e)
        {
            // ���o���Ū�����ɮ׸��|
            string filePath = textBox_appsetting.Text;
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                var result = MessageBox.Show($"�T�w�n�л\���x�s���ɮ׶ܡH\n{filePath}", "�T�{�x�s", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        File.WriteAllText(filePath, textBox_content.Text);
                        MessageBox.Show("�x�s���\: " + filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("�O�s�t�m���ɥX��: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("�п�ܤ@�Ӧ��Ī��t�m���C");
            }
        }
        private async void btn_RestartAGVS_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("�T�w�O�_���Ҭ����t��?", "�T�{", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                // �b�o�̰��歫�Ҭ����t�Ϊ��{���X
                // �Ҧp�Gawait APIController.RestartAGVSAsync();
                string response = await APIController.RestartAGVS();
                //richTextBox_content.Text = response;
                richTextBox_content.AppendText(response + Environment.NewLine);
                //APIController.RestartAGVS();
                MessageBox.Show("�w���歫�Ҭ����t�ΡC");
            }
            else
            {
                // �����A���������
                MessageBox.Show("�w�����A�U���O�ë�");
            }


        }
    }
}