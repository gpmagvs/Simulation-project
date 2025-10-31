using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Data.SqlClient;

namespace AutoProjectSystem
{
    public static class SQLConfig
    {
        public static string DBConnection { get; set; } = "Server=127.0.0.1;Database=GPMAGVs;User Id=sa;Password=12345678;Encrypt=False;MultipleActiveResultSets=True;Connection Lifetime=1;Min Pool Size=5;Max Pool Size=250;MultipleActiveResultSets=True;";
        /// <summary>
        /// 若你想在程式啟動時覆蓋（例如讀設定檔），就呼叫這個方法。
        /// </summary>
        //public static void SetConnectionString(string connStr)
        //{
        //    if (string.IsNullOrWhiteSpace(connStr))
        //        throw new ArgumentException("Connection string cannot be empty.", nameof(connStr));

        //    DBConnection = connStr;
        //}
    }

}
