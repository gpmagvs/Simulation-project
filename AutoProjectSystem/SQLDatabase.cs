using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic.Logging;
using static System.Windows.Forms.AxHost;


namespace AutoProjectSystem
{
    public static class SQLDatabase
    {
        /// <summary>
        /// 可在 Program.cs 啟動時呼叫一次，用來測試與初始化（可選）。
        /// </summary>
        public static void Initialize()
        {
            // 將連線字串也存成環境變數（比照你 AGVSystem 的做法）
            try
            {
                Environment.SetEnvironmentVariable("AGVSDatabaseConnection", SQLConfig.DBConnection,
                                                   EnvironmentVariableTarget.Process);
            }
            catch
            {
                // 不重要就忽略
            }

            // 簡單測試可否連線
            EnsureCanConnect();
        }
        public class TaskRow
        {
            public string TaskName { get; set; }
            public int Action { get; set; }
            public DateTime RecieveTime { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? FinishTime { get; set; }
            public int State { get; set; }
            public string DispatcherName { get; set; }
        }
        /// <summary>
        /// 失敗會丟例外，成功則無事。
        /// </summary>
        public static void EnsureCanConnect()
        {
            using var conn = new SqlConnection(SQLConfig.DBConnection);
            conn.Open();
            using var cmd = new SqlCommand("SELECT 1", conn);
            _ = cmd.ExecuteScalar();
        }

        /// <summary>
        /// 取得一個「已開啟」的 SqlConnection（呼叫端負責 Dispose）。
        /// </summary>
        public static SqlConnection GetOpenConnection()
        {
            var conn = new SqlConnection(SQLConfig.DBConnection);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// 建立 EF Core 的 DbContext（若你選擇用 EF）。
        /// </summary>
        /// 
        public class AutoProjectDbContext : DbContext
        {
            public AutoProjectDbContext(DbContextOptions<AutoProjectDbContext> options) : base(options) { }

            // 範例：之後可以放資料表對應
            // public DbSet<TaskEntity> Tasks { get; set; }

        }

        public static async Task<DataTable> QueryCancelTaskAsync(int? top = null)
        {
            var sql = $@"
            SELECT {(top.HasValue ? "TOP (@top)" : "")}
                   TaskName, 
                   Action, 
                   RecieveTime, 
                   StartTime, 
                   FinishTime, 
                   State
            FROM Tasks
            WHERE State = 1
            ORDER BY RecieveTime DESC;";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (top.HasValue) cmd.Parameters.Add(new SqlParameter("@top", SqlDbType.Int) { Value = top.Value });

            using var rd = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(rd);                 // ← 一次載入全部欄位
            return dt;
            ///回傳要取消的任務
        }
        public static async Task<DataTable> QueryTasksTableAsync(int? top = null)
        {
            var sql = $@"
            SELECT {(top.HasValue ? "TOP (@top)" : "")}
                   TaskName, Action, RecieveTime, StartTime, FinishTime, State 
            FROM Tasks
            ORDER BY RecieveTime DESC;";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (top.HasValue) cmd.Parameters.Add(new SqlParameter("@top", SqlDbType.Int) { Value = top.Value });

            using var rd = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            dt.Load(rd);                 // ← 一次載入全部欄位
            return dt;
        }
        public static async Task<DataTable> QueryUNdoneTasksAsync(int state = 1, int? top = null)
        {
            var sql = $@"
            SELECT {(top.HasValue ? "TOP (@top)" : "")} TaskName
            FROM Tasks WITH (NOLOCK)
            WHERE [State] = @state
            ORDER BY RecieveTime DESC;";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@state", state);
            if (top.HasValue) cmd.Parameters.AddWithValue("@top", top.Value);

            using var reader = await cmd.ExecuteReaderAsync();
            var dt = new DataTable();
            //dt.Columns.Add("TaskName", typeof(string)); // Ensure the DataTable has the correct schema

            //while (await reader.ReadAsync())
            //{
            //    var name = reader["TaskName"]?.ToString();
            //    if (!string.IsNullOrWhiteSpace(name))
            //    {
            //        var row = dt.NewRow();
            //        row["TaskName"] = name;
            //        dt.Rows.Add(row);
            //    }
            //}
            dt.Load(reader);
            return dt;
        }
        public static async Task<List<TaskRow>> QueryTasksAsync(int? top = null)
        {
            var sql = $@"
            SELECT {(top.HasValue ? "TOP (@top)" : "")}
                   TaskName, Action, RecieveTime, StartTime, FinishTime, State 
            FROM Tasks
            ORDER BY RecieveTime DESC;";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (top.HasValue) cmd.Parameters.Add(new SqlParameter("@top", SqlDbType.Int) { Value = top.Value });

            var list = new List<TaskRow>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new TaskRow
                {
                    TaskName = rd["TaskName"] as string,
                    Action = Convert.ToInt32(rd["Action"]),
                    RecieveTime = Convert.ToDateTime(rd["RecieveTime"]),
                    StartTime = rd["StartTime"] is DBNull ? null : Convert.ToDateTime(rd["StartTime"]),
                    FinishTime = rd["FinishTime"] is DBNull ? null : Convert.ToDateTime(rd["FinishTime"]),
                    State = Convert.ToInt32(rd["State"]),
                    // DispatcherName = rd["DispatcherName"] as string
                });
            }
            return list;
        }
        /// <summary>
        /// 查「超過指定時間仍未完成」的任務（預設 1 分鐘）。
        /// 完成判定：FinishTime 為 NULL 或 0001-01-01 視為未完成。
        /// </summary>
        public static async Task<List<TaskRow>> QueryOvertimeUnfinishedAsync(TimeSpan? threshold = null)
        {
            threshold ??= TimeSpan.FromMinutes(1);

            // 以 FinishTime NULL 或 0001-01-01 當作未完成；必要時可再加上 State 判定。
            var sql = @"
                SELECT TaskName, Action, RecieveTime, StartTime, FinishTime, State
                FROM dbo.Tasks
                WHERE (FinishTime IS NULL OR FinishTime = '0001-01-01T00:00:00')
                  AND DATEADD(SECOND, @sec, RecieveTime) < GETDATE()
                ORDER BY RecieveTime DESC;";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(new SqlParameter("@sec", SqlDbType.Int) { Value = (int)threshold.Value.TotalSeconds });

            var list = new List<TaskRow>();
            using var rd = await cmd.ExecuteReaderAsync();
            while (await rd.ReadAsync())
            {
                list.Add(new TaskRow
                {
                    TaskName = rd["TaskName"] as string,
                    Action = Convert.ToInt32(rd["Action"]),
                    RecieveTime = Convert.ToDateTime(rd["RecieveTime"]),
                    StartTime = rd["StartTime"] is DBNull ? null : Convert.ToDateTime(rd["StartTime"]),
                    FinishTime = rd["FinishTime"] is DBNull ? null : Convert.ToDateTime(rd["FinishTime"]),
                    State = Convert.ToInt32(rd["State"]),
                    //DispatcherName = rd["DispatcherName"] as string
                });
            }
            return list;
        }

        /// <summary>
        /// 依需求對超時未完成任務做標記（示範：寫入 Log；若有欄位可更新可在這裡UPDATE）。
        /// </summary>
        //public static async Task MarkOvertimeUnfinishedAsync(TimeSpan? threshold = null)
        //{
        //    var overtime = await QueryOvertimeUnfinishedAsync(threshold);
        //    var log = NLog.LogManager.GetCurrentClassLogger();
        //    foreach (var t in overtime)
        //        log.Warn($"[OVERTIME] {t.TaskName} | RecieveTime={t.RecieveTime:yyyy-MM-dd HH:mm:ss}");
        //    // 若你有 Tasks 表的「Remark/IsOvertime」欄位，可在這裡用 UPDATE 對它們加標記。
        //}
    }

}
