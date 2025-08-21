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
        private async Task<string> CallApi(string url, HttpMethod method = null)
        {
            using (HttpClient client = new HttpClient())
            {
                method ??= HttpMethod.Post;
                HttpRequestMessage request = new HttpRequestMessage(method, url);

                if (method == HttpMethod.Post)
                    request.Content = new StringContent("{}", Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
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
        public async Task<string> testAPIAGVMoveTask()
        {
            //string agvName = "AGV_001"; // 可依需求改為參數
            //string url = $"http://localhost:5216/api/Task/Move?user=dev&";
            string url = $"http://localhost:5216/api/Task/Move";
            // Move? user = dev &
            // 建立 payload 物件
            var payload = new
            {
                TaskName = $"Move_{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}",
                Action = 0,        
                DesignatedAGVName = "AGV_001",
                From_Station = "-1",
                From_Slot = "-1",
                To_Station = "66",
                To_Slot = "-1",
                Carrier_ID = "-1",
                Priority = 50,
                bypass_eq_status_check = false,
                need_change_agv = false,
                ChangeAGVMiddleStationTag = 0,
                TransferToDestineAGVName = ""
            };
            string json = System.Text.Json.JsonSerializer.Serialize(payload);
            //string json = System.Text.Json.JsonSerializer.Serialize(payload);

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
        public async Task<string> APIAGVMoveTask(string agvname, string to_tag , int Priority = 50, bool bypass_eq_status_check = false)
        {
            //string agvName = "AGV_001"; // 可依需求改為參數
            string url = $"http://localhost:5216/api/Task/Move?user=dev&";
            // Move? user = dev &
            // 建立 payload 物件
            var payload = new
            {
                Action = 0,
                Carrier_ID = "-1",
                ChangeAGVMiddleStationTag =0,
                DesignatedAGVName = agvname,
                From_Slot = "-1",
                From_Station = "-1",
                Priority = 50 , 
                TaskName = $"Move_{DateTime.Now.ToString("yyyyMMdd_HHmmssfff")}",
                To_Slot = "-1",
                To_Station = to_tag,
                TransferToDestineAGVName = "",
                bypass_eq_status_check = false ,
                need_change_agv = false

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
    }

}
