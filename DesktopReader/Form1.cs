using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // โหลดหน้าเริ่มต้น
            lblHeader.Text = "ทดสอบการเชื่อมต่อ";
            ShowPage(new UC_TestConnection());
            SetActiveMenu(btnTestConnection);

            // คลิกเมนู
            btnTestConnection.Click += (s, ev) =>
            {
                lblHeader.Text = "ทดสอบการเชื่อมต่อ";
                ShowPage(new UC_TestConnection());
                SetActiveMenu(btnTestConnection);
            };

            btnWebConfig.Click += (s, ev) =>
            {
                lblHeader.Text = "ตั้งค่าเชื่อมต่อเว็บ";
                ShowPage(new UC_WebConfig());
                SetActiveMenu(btnWebConfig);
            };

            btnAbout.Click += (s, ev) =>
            {
                lblHeader.Text = "เกี่ยวกับโปรแกรม";
                ShowPage(new UC_About());
                SetActiveMenu(btnAbout);
            };
        }

        // ✅ ฟังก์ชันเปลี่ยนหน้า
        private void ShowPage(UserControl page)
        {
            this.panelContent.Controls.Clear();
            page.Dock = DockStyle.Fill;
            this.panelContent.Controls.Add(page);
        }

        // ✅ ฟังก์ชันเปลี่ยน Style ของปุ่มเมนู
        private void SetActiveMenu(Button activeButton)
        {
            // กำหนด Font ของปุ่มทั้งหมดให้เป็นปกติ
            btnTestConnection.Font = new Font("Sarabun", 10F, FontStyle.Regular);
            btnWebConfig.Font = new Font("Sarabun", 10F, FontStyle.Regular);
            btnAbout.Font = new Font("Sarabun", 10F, FontStyle.Regular);

            // ปรับปุ่มที่เลือกให้เป็น Bold
            activeButton.Font = new Font("Sarabun", 10F, FontStyle.Bold);

            // (ถ้าต้องการ) เปลี่ยนพื้นหลังเล็กน้อยเพื่อเน้น
            btnTestConnection.BackColor = Color.FromArgb(232, 237, 245);
            btnWebConfig.BackColor = Color.FromArgb(232, 237, 245);
            btnAbout.BackColor = Color.FromArgb(232, 237, 245);
            activeButton.BackColor = Color.White;
        }
    }
}
