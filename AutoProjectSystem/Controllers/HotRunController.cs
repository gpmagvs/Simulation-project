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

namespace AutoProjectSystem.Controllers
{
   
    class HotRunController
    {
        private new APIConfigs APIConfigs = new APIConfigs();
        public async Task<List<HotRunScript>> GetHotRunScriptsAsync()
        {
            //string url = "localhost:5216";
            //string url = APIConfigs.url + APIConfigs.Path; // 使用 APIConfigs 中的 URL 和路徑
            //string url = "http://localhost:5216/api/HotRun/GetHotRunScripts";
            //using (HttpClient client = new HttpClient())
            //{
            //    var response = await client.GetAsync(url);
            //    response.EnsureSuccessStatusCode();
            //    var scripts = await response.Content.ReadFromJsonAsync<List<HotRunScript>>();
            //    return scripts ?? new List<HotRunScript>();
            //}
        }
        public class HotRunScript
        {
            public int Id { get; set; }
            public string Name { get; set; }
            // 依照你的資料結構補齊屬性
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
    }
}
