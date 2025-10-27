using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using DesktopReader.Services;
//using DesktopReader.Utils;


namespace DesktopReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }


        private void btnRead_Click(object? sender, EventArgs e)
        {
            try
            {
                // ✅ บังคับใช้ TIS-620 encoding
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                Encoding tis620 = Encoding.GetEncoding("TIS-620");

                var svc = new QuickThaiIdService();
                var data = svc.ReadAll();

                // ✅ แสดงข้อมูลภาษาไทยปกติ
                txtOutput.Text =
                    $"เลขบัตรประชาชน: {data.CitizenId}\r\n" +
                    $"ชื่อ-นามสกุล (ไทย): {data.ThFullName}\r\n" +
                    $"ชื่อ-นามสกุล (อังกฤษ): {data.EnFullName}\r\n" +
                    $"เพศ: {data.Gender}\r\n" +
                    $"วันเกิด: {data.BirthDate:yyyy-MM-dd}\r\n" +
                    $"ที่อยู่: {data.Address}\r\n" +
                    $"ผู้ออกบัตร: {data.IssuePlace}\r\n" +
                    $"วันออกบัตร: {data.IssueDate:yyyy-MM-dd}\r\n" +
                    $"วันหมดอายุ: {data.ExpireDate:yyyy-MM-dd}\r\n";

                // ✅ แสดงภาพถ่าย
                if (data.PhotoJpeg != null)
                {
                    using (var ms = new MemoryStream(data.PhotoJpeg))
                    {
                        picPhoto.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    picPhoto.Image = null;
                    MessageBox.Show("⚠️ ไม่พบรูปภาพในบัตร", "No Photo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }
}