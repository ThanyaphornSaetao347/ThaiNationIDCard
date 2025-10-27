using System.Text;
using System.Windows.Forms;

namespace DesktopReader
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            // ✅ เปิดใช้ Encoding TIS-620 สำหรับ .NET 5+
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}
