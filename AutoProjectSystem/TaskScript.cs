using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Runtime.CompilerServices;

namespace AutoProjectSystem
{
    //static readonly HttpClient http = new HttpClient
    //{
    //    BaseAddress = new Uri("https://localhost:5216/"),
    //    Timeout = TimeSpan.FromSeconds(15)
    //};
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string token { get; set; }
        public string Role { get; set; }
        public string UserName { get; set; }
        public string Permission { get; set; }
    }
    class TaskScript
    {
        public string TaskName { get; set; }
        public int Action { get; set; }
        public string DesignatedAGVName { get; set; } = "-1";
        public string From_Station { get; set; }
        public string From_Slot { get; set; } = "-1";
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
    public class TaskItem
    {
        public int No { get; set; }
        public string AGVName { get; set; } = "AGV_001";
        public string Start { get; set; }
        public string Action { get; set; } = "move";
        public string End { get; set; }

    }
    //public class  Script
    //{
    //    public string ScriptID { get; set; } = Guid.NewGuid().ToString("N");
    //    public string ScriptName { get; set; } = "New Script";
    //    public BindingList<TaskItem> Tasks { get; set; } = new();
    //    public override string ToString() => ScriptName;

    //}
    public class Script : INotifyPropertyChanged
    {
        private string _scriptName = "New Script";

        public string ScriptID { get; set; } = Guid.NewGuid().ToString("N");

        public string ScriptName
        {
            get => _scriptName;
            set
            {
                if (_scriptName != value)
                {
                    _scriptName = value;
                    OnPropertyChanged();
                }
            }
        }

        public BindingList<TaskItem> Tasks { get; set; } = new();

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public override string ToString() => ScriptName;
    }


}
