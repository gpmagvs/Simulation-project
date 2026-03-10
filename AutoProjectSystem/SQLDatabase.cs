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
using System.Formats.Asn1;
using NLog;


namespace AutoProjectSystem
{
    public static class SQLDatabase
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
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
        //todo查詢的任務可以參數化
        //是否有正在執行中的任務
        public static  async Task<bool> HasTaskState1Async()
        {
            var sql = @"SELECT COUNT(*) FROM Tasks WHERE State IN (1, 5);";
            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);
            //var result = await cmd.ExecuteScalarAsync();
            //return result != null;
            int count = (int)await cmd.ExecuteScalarAsync();
            return count > 0;
        }
        public static async Task<DataTable> QueryCancelTaskAsync(int state ,int? top = null)
        {
            bool HasTask = false;

            var sql = $@"
            SELECT {(top.HasValue ? "TOP (@top)" : "")}
                   TaskName, Action, RecieveTime, StartTime, FinishTime, State, DesignatedAGVName
            FROM Tasks
            WHERE State = {state}
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
                   TaskName, Action, RecieveTime, StartTime, FinishTime, State  ,DesignatedAGVName
            FROM Tasks
            ORDER BY RecieveTime DESC;";
            try
            {
                using var conn = GetOpenConnection();
                using var cmd = new SqlCommand(sql, conn);
                if (top.HasValue) cmd.Parameters.Add(new SqlParameter("@top", SqlDbType.Int) { Value = top.Value });

                using var rd = await cmd.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(rd);                 // ← 一次載入全部欄位
                return dt;
            }
            catch (Exception ex)
            {
                logger.Info(ex, "查詢任務失敗");
                throw;
            }

        }
        public static async Task<DataTable> QueryUNdoneTasksAsync(int state , int? top = null)
        {
            string topClause = top.HasValue ? "TOP (@top) " : "";   // 🔥 注意後面有空白

            var sql = $@"
            SELECT {topClause}
                   TaskName, Action, RecieveTime, StartTime, FinishTime, State, DesignatedAGVName
            FROM Tasks
            WHERE State = @state
            ORDER BY RecieveTime DESC;";
            System.Diagnostics.Debug.WriteLine("==== SQL ====");
            System.Diagnostics.Debug.WriteLine(sql);
            System.Diagnostics.Debug.WriteLine($"state={state}, top={(top?.ToString() ?? "null")}");
            System.Diagnostics.Debug.WriteLine("=============");
            try
            {
                using var conn = GetOpenConnection();
                using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@state", SqlDbType.Int).Value = state;

                if (top.HasValue)
                    cmd.Parameters.Add("@top", SqlDbType.Int).Value = top.Value;

                using var reader = await cmd.ExecuteReaderAsync();
                var dt = new DataTable();
                dt.Load(reader);
                return dt;
            }
            catch (Exception ex)
            {
                logger.Error(ex, $"查詢 State={state} 的任務失敗");
                throw;
            }

        }
        public static async Task<bool> HasRunningOrIdleTaskAsync()
        {
            var sql = @"SELECT 1WHERE EXISTS (SELECT 1 FROM Tasks WITH (NOLOCK) WHERE State IN (1, 5));";

            using var conn = GetOpenConnection();
            using var cmd = new SqlCommand(sql, conn);  
            
            // var result = await cmd.ExecuteNonQueryAsync();
            var result = await cmd.ExecuteScalarAsync();


            return result != null;
        }
    }

}
