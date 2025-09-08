using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoProjectSystem
{
    class MapScripts
    {

        public class MultiMapRoot
        {
            [JsonPropertyName("maps")]
            public List<MapNode> Maps { get; set; } = new();
        }

        public class MapNode
        {
            [JsonPropertyName("mapName")]
            public string MapName { get; set; } = "";

            [JsonPropertyName("scripts")]
            public List<ScriptNode> Scripts { get; set; } = new();
        }

        public class ScriptNode
        {
            [JsonPropertyName("scriptName")]
            public string ScriptName { get; set; } = "";

            [JsonPropertyName("tasks")]
            public List<TaskNode> Tasks { get; set; } = new();
        }

        public class TaskNode
        {
            [JsonPropertyName("taskName")]
            public string TaskName { get; set; } = "";

            [JsonPropertyName("action")]
            public string Action { get; set; } = "";

            [JsonPropertyName("designatedAGVName")]
            public string DesignatedAGVName { get; set; } = "";

            [JsonPropertyName("from_Station")]
            public string Start { get; set; } = "";

            [JsonPropertyName("to_Station")]
            public string End { get; set; } = "";

            // 其他欄位如有需要可再補上（carrier_ID、priority...）
        }
    }
}