using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
///0821 新增
public record ApiResult<T>(bool OK, T? Data = default, string? Error = null, HttpStatusCode? StatusCode = null);
static class AgvsClient
{
    // 若後端是 HTTPS，請用 https://
    private static readonly HttpClient Http = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5216/"),
        Timeout = TimeSpan.FromSeconds(15)
    };

    private static string _jwt; // 暫存 token

    // 對齊後端回傳格式：{ Success, token, Role, UserName, Permission }
    private class LoginResponse
    {
        public bool Success { get; set; }
        public string token { get; set; }

        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public int Role { get; set; }        // ← 原本 string 會炸

        public string UserName { get; set; }
        public JsonElement Permission { get; set; } // ← 接任意 JSON
    }
    public static async Task<ApiResult<string>> LoginAsync(string userName, string password, CancellationToken ct = default)
    {
        try
        {
            var payload = new { UserName = userName, Password = password };
            var json = JsonSerializer.Serialize(payload);
            using var content = new StringContent(json, Encoding.UTF8, "application/json");

            var resp = await Http.PostAsync("api/Auth/login", content, ct);
            var body = await resp.Content.ReadAsStringAsync(ct);

            if (!resp.IsSuccessStatusCode)
                return new ApiResult<string>(false, null, $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase}: {body}", resp.StatusCode);

            var login = JsonSerializer.Deserialize<LoginResponse>(
                body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true, NumberHandling = JsonNumberHandling.AllowReadingFromString });

            if (login is null || !login.Success || string.IsNullOrWhiteSpace(login.token))
                return new ApiResult<string>(false, null, "登入失敗或未取得 token。");

            _jwt = login.token;
            Http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwt);
            return new ApiResult<string>(true, _jwt, null, resp.StatusCode);
        }
        catch (TaskCanceledException)
        {
            return new ApiResult<string>(false, null, "連線逾時。");
        }
        catch (HttpRequestException ex)
        {
            return new ApiResult<string>(false, null, "連線失敗（伺服器未啟動或網址錯誤）。\r\n" + ex.Message);
        }
        catch (Exception ex)
        {
            return new ApiResult<string>(false, null, "未知錯誤：" + ex.Message);
        }
    }
    //public static async Task LoginAsync(string userName, string password)
    //{
    //    // 和前端一致：UserName / Password（注意大小寫）
    //    var payload = new { UserName = userName, Password = password };
    //    var json = JsonSerializer.Serialize(payload);
    //    using var content = new StringContent(json, Encoding.UTF8, "application/json");
    //    try
    //    {
    //        var resp = await Http.PostAsync("api/Auth/login", content);
    //        var body = await resp.Content.ReadAsStringAsync();
    //        if (!resp.IsSuccessStatusCode)
    //            throw new HttpRequestException($"Login 失敗：{(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");

    //        var login = JsonSerializer.Deserialize<LoginResponse>(
    //            body,
    //            new JsonSerializerOptions
    //            {
    //                PropertyNameCaseInsensitive = true,
    //                NumberHandling = JsonNumberHandling.AllowReadingFromString
    //            });

    //        if (login is null || !login.Success || string.IsNullOrWhiteSpace(login.token))
    //            throw new Exception("登入失敗或未取得 token：" + body);

    //        _jwt = login.token;
    //        Http.DefaultRequestHeaders.Authorization =
    //            new AuthenticationHeaderValue("Bearer", _jwt);
    //    }
    //    catch (Exception)
    //    {

    //        throw;
    //    }

    //}

    public static async Task<string> PostMoveAsync(string agvName, string toTag, bool bypass = false)
    {
        if (string.IsNullOrEmpty(_jwt))
            throw new InvalidOperationException("尚未登入，請先呼叫 LoginAsync。");

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
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        var resp = await Http.PostAsync("api/Task/Move", content);
        var body = await resp.Content.ReadAsStringAsync();
        if (!resp.IsSuccessStatusCode)
            throw new HttpRequestException($"Move 失敗：{(int)resp.StatusCode} {resp.ReasonPhrase}\n{body}");
        return body;
    }
}
