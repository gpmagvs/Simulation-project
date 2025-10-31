namespace AutoProjectSystem
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            try
            {
                //  可以在這裡進行資料庫初始化
                SQLDatabase.Initialize();
            }
            catch (Exception ex)
            {
                //  錯誤處理不要在 Initialize() 之前用 MessageBox.Show
                MessageBox.Show($" 無法連線資料庫：{ex.Message}", "DB 連線錯誤",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Application.Run(new FrmMain());
        }
    }
}