using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using AutoProjectSystem.Controllers;

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
            public AutoProjectDbContext(DbContextOptions<AutoProjectDbContext> options): base(options){ }

            // 範例：之後可以放資料表對應
            // public DbSet<TaskEntity> Tasks { get; set; }
        }
    }

}
