using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using static System.Net.WebRequestMethods;

namespace AutoProjectSystem.Controllers
{

    class AGVSController
    {

        //new APIConfigs APIConfigs = new APIConfigs();
        public readonly HttpClient _http;
        public readonly string  AGVSUrl = "localhost:5216";
        public readonly string   VMSUrl = "localhost:5036";


        public AGVSController(string baseUrl, HttpClient? http = null)
        {
            AGVSUrl = baseUrl.TrimEnd('/');
            _http = http ?? new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
        }
        ///車子定位
        public async Task<string> AGVLocateAsync(string agvName, string tagId)
        {

            //string  url = $"{_baseUrl}/api/VmsManager/AGVLocating?agv_name={WebUtility.UrlEncode(agvName)}";
            string url = $"http://localhost:5036/api/VmsManager/AGVLocating?agv_name={Uri.EscapeDataString(agvName)}";
            var payload = new
            {
                Name = agvName,
                currentID = tagId,
                x = -0.65,
                y = -4.52,
                theta = 180,         // ← 原程式拼成 theata
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

        public async Task<string> APIAGVLocate(  string agvname, string location)
        {
            //string agvName = "AGV_001"; // 可依需求改為參數
            string url = $"http://localhost:5036/api/VmsManager/AGVLocating?agv_name={Uri.EscapeDataString(agvname)}";
            // 建立 payload 物件
            var payload = new
            {
                Name = agvname,
                currentID = location,
                isAMCAGV = false,
                locateWith = "tag",
                theata = 0,
                x = -0.65,
                y = -4.52
            };
            string json = System.Text.Json.JsonSerializer.Serialize(payload);

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // <-- 指定 media type 為 application/json
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
        public static async Task<string> PostMoveAsync(string agvName, string toTag, bool bypass = false)
        {
            // 確認已登入（如果 LoginAsync 是非同步，這裡要 await）

                try
                {
                    await AgvsClient.LoginAsync("dev", "12345678"); // 確保等待登入完成
                }
                catch (Exception)
                {
                    return "發生錯誤: 尚未登入，請先呼叫 LoginAsync。";
                }

            var payload = new
            {
                Action = 0,
                Carrier_ID = "-1",
                ChangeAGVMiddleStationTag = 0,
                DesignatedAGVName = agvName,
                From_Slot = "-1",
                From_Station = "-1",
                Priority = 50,
                TaskName = $"Move_{DateTime.Now:yyyyMMdd_HHmmssfff}",
                To_Station = toTag,
                To_Slot = "-1",
                TransferToDestineAGVName = "",
                bypass_eq_status_check = bypass,
                need_change_agv = false
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = null });

            try
            {
                // 改成完整 URL（跟 APIAGVLocate 類似）
                string url = "http://localhost:5216/api/Task/Move";

                using (var client = new HttpClient())
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    //// 如果有 jwt，就放 Authorization header
                    //if (!string.IsNullOrEmpty(_jwt))
                    //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);

                    HttpResponseMessage resp = await client.PostAsync(url, content);
                    var body = await resp.Content.ReadAsStringAsync();

                    if (!resp.IsSuccessStatusCode)
                        return $"發生錯誤: HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}";

                    return body;
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
