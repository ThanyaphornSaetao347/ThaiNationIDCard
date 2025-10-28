using System.Text;

namespace DesktopReader
{
    internal static class Program
    {
        public static void InitEncoding()
        {
            // ✅ เปิดใช้ Encoding TIS-620 สำหรับ .NET 5+
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }
    }
}
