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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutoProjectSystem.Controllers
{

    class AGVSController
    {
        new APIConfigs APIConfigs = new APIConfigs();
        ///車子定位
        public async Task<string> APIAGVLocate(string agvname , string location)
        {
            //string agvName = "AGV_001"; // 可依需求改為參數
            string url = $"http://localhost:5036/api/VmsManager/AGVLocating?agv_name={agvname}";

            // 建立 payload 物件
            var payload = new
            {
                Name = agvname,
                currentID = location,
                x = -0.65,
                y = -4.52,
                theata = 180,
                isAMCAGV = false,
                locateWith = "tag"
            };
            string json = System.Text.Json.JsonSerializer.Serialize(payload);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return $"發生錯誤: {ex.Message}";
            }
        }

        public async Task<string> AGVLocate(string url, string agvname, string location)
        {
            //string agvName = "AGV_001"; // 可依需求改為參數
            string urlpath = $"http://{url}/api/VmsManager/AGVLocating?agv_name={agvname}";

            // 建立 payload 物件
            var payload = new
            {
                Name = agvname,
                currentID = location,
                x = -0.65,
                y = -4.52,
                theata = 180,
                isAMCAGV = false,
                locateWith = "tag"
            };
            string json = System.Text.Json.JsonSerializer.Serialize(payload);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                return $"發生錯誤: {ex.Message}";
            }
        }
        //API功能測試
        public async Task<string> APITestAsync()
        {
            //string url = "localhost:5216";
            //string url = APIConfigs.url + APIConfigs.Path; // 使用 APIConfigs 中的 URL 和路徑
            string url = "http://localhost:5216/api/SecsGemConfiguration/saveSECSGemSetting";
            if (string.IsNullOrWhiteSpace(url))
            {
                return "請輸入 API URL";
            }
            try
            {
                string result = await CallApiAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                return $"發生錯誤: {ex.Message}";
            }
        }
        /// <summary>
        /// 重新啟動 AGVS 系統
        /// </summary>
        /// <returns></returns>
        public async Task<string> RestartAGVS()
        {
            //string url = "localhost:5216";
            //string url = APIConfigs.url + APIConfigs.Path; // 使用 APIConfigs 中的 URL 和路徑
            string url = "http://localhost:5216/api/SecsGemConfiguration/RestartAGVSystem";
            if (string.IsNullOrWhiteSpace(url))
            {
                return "請輸入 API URL";
            }
            try
            {
                string result = await CallApiAsync(url);
                return result;
            }
            catch (Exception ex)
            {
                return $"發生錯誤: {ex.Message}";
            }
        }
        private async Task<string> CallApiAsync(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.PostAsync(
                    url,
                    new StringContent("{}", Encoding.UTF8, "application/json")
                );
                response.EnsureSuccessStatusCode();
                //MessageBox.Show(response.EnsureSuccessStatusCode);
                return await response.Content.ReadAsStringAsync();

            }
        }
    }

}
