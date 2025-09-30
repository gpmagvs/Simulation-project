using AGVSystem.Config;
using AutoProjectSystem.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoProjectSystem
{
    public class TaskRow
    {
        public int Start { get; set; } = 0;
        public int End { get; set; } = 0;

        public string AGVName { get; set; } = "";
        public string Action { get; set; } = "";  // 例如 "move"
        public int No { get; set; }               // 任務序號（可選）
    }
    class ScriptsExecutor
    {
        

    }
}
