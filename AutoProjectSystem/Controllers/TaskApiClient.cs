using AutoProjectSystem;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.AccessControl;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public sealed class TaskApiClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _json;
    private new MoveApiResponse moveApiResponse = new MoveApiResponse();
    public TaskApiClient(string baseUrl)
    {
        _http = new HttpClient { BaseAddress = new Uri(baseUrl), Timeout = TimeSpan.FromSeconds(15) };
        _json = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // 保留你的鍵名（含 snake_case）
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    static readonly HttpClient Http = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5216/"),
        Timeout = TimeSpan.FromSeconds(15)
    };
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string token { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
    }
    /// <summary>
    /// WinForms 等價於：CallAPI('/api/Task/Move', data)
    /// </summary>
    /// 
    public static async Task<string> LoginAsync(string username, string password)
    {
        var payload = new { Username = username, Password = password };
        var json = JsonSerializer.Serialize(payload);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // ★ 路徑請改成你的 Controller 路由，例如 "api/auth/login" 或 "api/user/login"
        var resp = await Http.PostAsync("api/auth/login", content);
        var body = await resp.Content.ReadAsStringAsync();
        resp.EnsureSuccessStatusCode();

        var login = JsonSerializer.Deserialize<LoginResponse>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (login is null || !login.Success || string.IsNullOrWhiteSpace(login.token))
            throw new Exception("登入失敗或未取得 token：" + body);

        return login.token;
    }
    // 2) 帶著 Bearer token 呼叫 Move 端點
    public static async Task<string> PostMoveAsync(string agvname, string toTag, bool bypass = false)
    {
        // 設定 TaskName = 現在時間
        var payload = new
        {
            Action = 0,
            Carrier_ID = "-1",
            ChangeAGVMiddleStationTag = 0,
            DesignatedAGVName = agvname,
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
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        // ★ 路徑請改成你的實際 Move 端點，例如 "api/Task/Move"
        var resp = await Http.PostAsync("api/Task/Move", content);
        var body = await resp.Content.ReadAsStringAsync();

        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"Move 失敗: {(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");

        return body;
    }
    public async Task<MoveApiResponse> MoveTaskAsync(MoveTaskRequest data, CancellationToken ct = default)
    {
        // 對應你的前端路徑
        const string endpoint = "/api/Task/Move";

        // 用 PostAsJsonAsync + 自訂 options，避免 camelCase 破壞鍵名
        using var resp = await _http.PostAsJsonAsync(endpoint, data, _json, ct);
        string raw = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
            throw new Exception($"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}\n{raw}");

        // 反序列化成你的回應格式
        var result = JsonSerializer.Deserialize<MoveApiResponse>(raw, _json)
                     ?? throw new Exception("回應內容為空或非預期格式。");
        return result;
    }


    public void Dispose() => _http.Dispose();
}
