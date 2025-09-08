// MultiMapScriptConfig.cs
// 說明：
// 1) 多地圖 (maps) → 多腳本 (scripts) → 多任務 (tasks) 結構
// 2) 與 HotRunScript/HotRunAction 的轉換工具（支援單一/多台 AGV）
// 3) JSON 讀寫服務

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

// 若此檔與 HotRunScript 在不同專案/命名空間，請改成實際命名空間
using AGVSystem.Models.TaskAllocation.HotRun;

namespace AGVSystem.Config
{
    #region === JSON DTO (對應文件結構) ===

    /// <summary>
    /// 參數檔根節點：包含多個地圖
    /// </summary>
    public class MultiMapRoot   // ← 由 MultiMapRootorigin 改名為 MultiMapRoot
    {
        [JsonPropertyName("version")]
        public int Version { get; set; } = 1;

        [JsonPropertyName("maps")]
        public List<MapDto> Maps { get; set; } = new();
    }

    /// <summary>
    /// 地圖：包含 scripts
    /// </summary>
    public class MapDto
    {
        [JsonPropertyName("MapID")]
        public string MapID { get; set; } = "";

        // ★ 檔案是 mapName（小寫）
        [JsonPropertyName("mapName")]
        public string MapName { get; set; } = "";

        [JsonPropertyName("scripts")]
        public List<ScriptDto> Scripts { get; set; } = new();

        public override string ToString() => string.IsNullOrWhiteSpace(MapName) ? "(Unnamed Map)" : MapName;
    }

    /// <summary>
    /// 腳本：包含 Tasks
    /// </summary>
    public class ScriptDto
    {
        [JsonPropertyName("ScriptID")]
        public string ScriptID { get; set; } = "";

        // ★ 檔案是 scriptName（小寫）
        [JsonPropertyName("scriptName")]
        public string ScriptName { get; set; } = "";

        // ★ 檔案是 tasks（小寫）
        [JsonPropertyName("tasks")]
        public List<TaskItemDto> Tasks { get; set; } = new();

        public override string ToString() => string.IsNullOrWhiteSpace(ScriptName) ? "(Unnamed Script)" : ScriptName;
    }

    /// <summary>
    /// 任務（一步動作）
    /// Start/End 以字串儲存（相容既有檔案）；轉 HotRun 時再轉 int
    /// </summary>
    public class TaskItemDto
    {
        [JsonPropertyName("No")]
        public int No { get; set; }  // 你檔案目前沒有 No，沒關係，預設 0

        // ★ designatedAGVName -> AGVName
        [JsonPropertyName("designatedAGVName")]
        public string AGVName { get; set; } = "";

        // ★ from_Station -> Start
        [JsonPropertyName("from_Station")]
        public string Start { get; set; } = "";

        // ★ action 一樣是字串
        [JsonPropertyName("action")]
        public string Action { get; set; } = "move";

        // ★ to_Station -> End
        [JsonPropertyName("to_Station")]
        public string End { get; set; } = "";
    }

    #endregion

    #region === Mapping：ScriptDto <-> HotRunScript ===

    public static class HotRunMappingExtensions
    {
        /// <summary>
        /// 將 ScriptDto 轉成 HotRunScript（假設一個腳本只對應一台 AGV）
        /// </summary>
        public static HotRunScript ToHotRunScript_SingleAgv(this ScriptDto dto)
        {
            var hs = new HotRunScript
            {
                scriptID = dto.ScriptID,
                comment = dto.ScriptName,                         // 若之後有專屬欄位可改
                agv_name = dto.Tasks.FirstOrDefault()?.AGVName ?? "",
                actions = new List<HotRunAction>()
            };

            foreach (var t in dto.Tasks.OrderBy(x => x.No))
            {
                hs.actions.Add(new HotRunAction
                {
                    no = t.No,
                    action = string.IsNullOrWhiteSpace(t.Action) ? "move" : t.Action,
                    source_tag = TryParseInt(t.Start),
                    destine_tag = TryParseInt(t.End)
                });
            }

            hs.action_num = hs.actions.Count;
            return hs;
        }

        /// <summary>
        /// 將 ScriptDto 轉成 HotRunScript（同一腳本可混用多台 AGV）
        /// 需要 HotRunAction 類別具備 agv_name 欄位。
        /// 若你的 HotRunAction 尚未有 agv_name，請在你的 HotRunAction 加上：
        ///     public string agv_name { get; set; } = "";
        /// </summary>
        public static HotRunScript ToHotRunScript_MultiAgv(this ScriptDto dto)
        {
            var hs = new HotRunScript
            {
                scriptID = dto.ScriptID,
                comment = dto.ScriptName,
                agv_name = "", // 實際以每一步 action.agv_name 為準
                actions = new List<HotRunAction>()
            };

            foreach (var t in dto.Tasks.OrderBy(x => x.No))
            {
                var act = new HotRunAction
                {
                    no = t.No,
                    action = string.IsNullOrWhiteSpace(t.Action) ? "move" : t.Action,
                    source_tag = TryParseInt(t.Start),
                    destine_tag = TryParseInt(t.End),
                };

                // 若 HotRunAction 有 agv_name 欄位就塞進去（沒有就略過）
                var prop = typeof(HotRunAction).GetProperty("agv_name");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(act, t.AGVName);
                }

                hs.actions.Add(act);
            }

            hs.action_num = hs.actions.Count;
            return hs;
        }

        /// <summary>
        /// 將 HotRunScript 轉回 ScriptDto（單一 AGV 版）
        /// </summary>
        public static ScriptDto ToScriptDto(this HotRunScript hs)
        {
            var dto = new ScriptDto
            {
                ScriptID = hs.scriptID,
                ScriptName = string.IsNullOrWhiteSpace(hs.comment) ? "HotRun Script" : hs.comment,
                Tasks = new List<TaskItemDto>()
            };

            foreach (var a in hs.actions.OrderBy(x => x.no))
            {
                dto.Tasks.Add(new TaskItemDto
                {
                    No = a.no,
                    AGVName = hs.agv_name,
                    Start = a.source_tag.ToString(),
                    Action = string.IsNullOrWhiteSpace(a.action) ? "move" : a.action,
                    End = a.destine_tag.ToString()
                });
            }

            return dto;
        }

        private static int TryParseInt(string s) => int.TryParse(s, out var v) ? v : 0;
    }

    #endregion

    #region === JSON 讀寫服務（選用） ===

    public static class MultiMapJson
    {
        private static readonly JsonSerializerOptions _opt = new()
        {
            WriteIndented = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public static MultiMapRoot Load(string path)
        {
            if (!File.Exists(path)) return new MultiMapRoot();
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<MultiMapRoot>(json, _opt) ?? new MultiMapRoot();
        }

        public static void Save(string path, MultiMapRoot data)
        {
            var json = JsonSerializer.Serialize(data ?? new MultiMapRoot(), _opt);
            File.WriteAllText(path, json);
        }
    }

    #endregion
}
