using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AutoProjectSystem
{
    class TaskScript
    {
        public string TaskName { get; set; } 
        public int Action { get; set; }
        public string DesignatedAGVName { get; set; } = "-1";
        public string From_Station { get; set; }
        public string From_Slot { get; set; }   = "-1";
        public string To_Station { get; set; }
        public string To_Slot { get; set; } = "-1";
        public string Carrier_ID { get; set; } = "-1";
        public int Priority { get; set; } = 50;
        public bool bypass_eq_status_check { get; set; } = false;
        public bool need_change_agv { get; set; } = false;
        public string ChangeAGVMiddleStationTag { get; set; } = "0";
        public string TransferToDestineAGVName { get; set; }

    //        this.TaskName = `Move_${moment(Date.now()).format('yyMMDD_HHmmssSSS')}`
    //this.Action = 0
    //this.DesignatedAGVName = agv_name
    //this.From_Station = '-1'
    //this.From_Slot = '-1'
    //this.To_Station = to_tag + ''
    //this.To_Slot = '-1'
    //this.Carrier_ID = '-1'
    //this.Priority = Priority
    //this.bypass_eq_status_check = bypass_eq_status_check
    //this.need_change_agv = false
    //this.ChangeAGVMiddleStationTag = 0
    //this.TransferToDestineAGVName = ""
    }
}
