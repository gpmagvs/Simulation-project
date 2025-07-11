using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoProjectSystem.Controllers
{
    class APIConfigs
    {
        /// <summary>
        /// API 基本網址
        /// </summary>
        public string url { get; set; } = "http://localhost:5216";

        /// <summary>
        /// API 路徑
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// API Token 或授權資訊（如有需要）
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 其他自訂參數（如有需要）
        /// </summary>
        public Dictionary<string, string>? Parameters { get; set; }

    }
}
