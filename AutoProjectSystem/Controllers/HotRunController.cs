using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using AGVSystem.Models.TaskAllocation.HotRun;


namespace AutoProjectSystem.Controllers
{

    class HotRunController
    {
        private new APIConfigs APIConfigs = new APIConfigs();
        public new HotRunScript hotRunScript = new HotRunScript();
        private List<HotRunScript> hotRunScripts = new List<HotRunScript>();


        public async Task<List<HotRunScript>> GetHotRunScriptsAsync()
        {
            string url = $"http://localhost:5216/api/HotRun";

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"API 請求失敗: {response.StatusCode}\n{errorContent}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<HotRunScript>();
                }

                var hotRunScripts = await response.Content.ReadFromJsonAsync<List<HotRunScript>>();
                return hotRunScripts ?? new List<HotRunScript>();
            }
        }
        public async Task StopHotRunApiAsync(string scriptID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://localhost:5216/api/HotRun/Stop?scriptID={Uri.EscapeDataString(scriptID)}";

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    { 
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"API 呼叫失敗：{response.StatusCode}\n{error}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("發生例外錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task StartHotRunApiAsync(string scriptID)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"http://localhost:5216/api/HotRun/Start?scriptID={Uri.EscapeDataString(scriptID)}";

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"API 呼叫失敗：{response.StatusCode}\n{error}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("發生例外錯誤：" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        public async Task<List<HotRunScript>> StartHotRunScriptsAsync(string scriptID)
        {
            string url = $"http://localhost:5216/api/HotRun/Start?scriptID=" +scriptID;

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"API 請求失敗: {response.StatusCode}\n{errorContent}", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return new List<HotRunScript>();
                }

                var hotRunScripts = await response.Content.ReadFromJsonAsync<List<HotRunScript>>();
                return hotRunScripts ?? new List<HotRunScript>();
            }
        }
    }
}
