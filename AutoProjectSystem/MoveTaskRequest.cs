using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AutoProjectSystem
{
    public sealed class MoveTaskRequest
    {
            public string TaskName { get; set; } = "";
            public int Action { get; set; }
            public string DesignatedAGVName { get; set; } = "";
            public string From_Station { get; set; } = "-1";
            public string From_Slot { get; set; } = "-1";
            public string To_Station { get; set; } = "";
            public string To_Slot { get; set; } = "-1";
            public string Carrier_ID { get; set; } = "-1";
            public int Priority { get; set; } = 50;
            
            // 這兩個是 snake_case，需要用 JsonPropertyName 對應
            [JsonPropertyName("bypass_eq_status_check")]
            public bool BypassEqStatusCheck { get; set; } = false;

            [JsonPropertyName("need_change_agv")]
            public bool NeedChangeAgv { get; set; } = false;

            public int ChangeAGVMiddleStationTag { get; set; } = 0;
            public string TransferToDestineAGVName { get; set; } = "";
        
    }
    public sealed class MoveApiResponse
    {
        [JsonPropertyName("confirm")]
        public bool Confirm { get; set; }

        [JsonPropertyName("alarm_code")]
        public int AlarmCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = "";

        [JsonPropertyName("message_en")]
        public string MessageEn { get; set; } = "";

        [JsonPropertyName("showEmptyOrFullContentCheck")]
        public bool ShowEmptyOrFullContentCheck { get; set; }
    }
    public sealed class ApiResult<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }

    }

}
