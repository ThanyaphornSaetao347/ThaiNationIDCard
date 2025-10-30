using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DesktopReader
{
    internal static class TrayHost
    {
        private static NotifyIcon? tray;
        private static MainForm? mainForm;

        [STAThread]
        static void Main()
        {
            // ✅ รองรับภาษาไทย (TIS-620)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            ApplicationConfiguration.Initialize();

            // ✅ สร้าง Registry AutoStart ถ้ายังไม่มี
            RegisterAutoStart();

            // ✅ เปิด WebAgent ถ้ายังไม่ได้รัน
            EnsureWebAgentRunning();

            // ✅ สร้าง Tray Icon
            tray = new NotifyIcon()
            {
                Icon = SystemIcons.Information,
                Text = "Thai ID Reader Agent",
                Visible = true,
                ContextMenuStrip = BuildMenu()
            };

            tray.DoubleClick += (s, e) => ShowMainWindow();

            tray.ShowBalloonTip(3000, "Thai ID Reader Agent", "พร้อมใช้งานแล้ว ✅", ToolTipIcon.Info);

            // 🟢 เพิ่มบรรทัดนี้ให้เปิดหน้าหลักทันทีเมื่อเปิดเครื่อง
            ShowMainWindow();

            Application.Run();
        }

        private static ContextMenuStrip BuildMenu()
        {
            var menu = new ContextMenuStrip();
            menu.Items.Add("เปิดหน้าต่างหลัก", null, (s, e) => ShowMainWindow());
            menu.Items.Add("เปิด WebAgent", null, (s, e) => OpenWebAgent());
            menu.Items.Add("เปิดหน้าเว็บทดสอบ (WebDemo)", null, (s, e) => OpenWebDemo());
            menu.Items.Add("-");
            menu.Items.Add("ออก", null, (s, e) => ExitApp());
            return menu;
        }

        private static void RegisterAutoStart()
        {
            try
            {
                string exePath = Application.ExecutablePath;
                string appName = "ThaiIDReader";

                using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                if (key != null)
                {
                    var currentValue = key.GetValue(appName) as string;
                    if (string.IsNullOrEmpty(currentValue) || currentValue != exePath)
                    {
                        key.SetValue(appName, exePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ตั้งค่า AutoStart ล้มเหลว:\n" + ex.Message,
                    "Thai ID Agent", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private static void EnsureWebAgentRunning()
        {
            try
            {
                var processes = Process.GetProcessesByName("WebAgent");
                if (processes.Length > 0) return;

                string agentPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\..\WebAgent\bin\Debug\net8.0\WebAgent.exe");

                if (File.Exists(agentPath))
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = agentPath,
                        UseShellExecute = true,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Minimized
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("เปิด WebAgent ไม่ได้:\n" + ex.Message,
                    "Thai ID Agent", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static void ShowMainWindow()
        {
            if (mainForm == null || mainForm.IsDisposed)
            {
                mainForm = new MainForm();
                mainForm.FormClosed += (s, e) => mainForm = null;
                mainForm.Show();
            }
            else
            {
                if (mainForm.WindowState == FormWindowState.Minimized)
                    mainForm.WindowState = FormWindowState.Normal;

                mainForm.BringToFront();
                mainForm.Focus();
            }
        }

        private static void OpenWebAgent()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://127.0.0.1:17890/idcard/ping",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private static void OpenWebDemo()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://127.0.0.1:18000",
                    UseShellExecute = true
                });
            }
            catch { }
        }

        private static void ExitApp()
        {
            tray!.Visible = false;
            tray.Dispose();
            Application.Exit();
        }
    }
}
