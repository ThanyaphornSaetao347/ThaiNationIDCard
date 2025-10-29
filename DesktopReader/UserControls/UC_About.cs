using System;
using System.Drawing;
using System.Windows.Forms;

namespace DesktopReader
{
    public partial class UC_About : UserControl
    {
        private TextBox txtAbout;

        public UC_About()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.txtAbout = new TextBox();

            // ช่องข้อมูล
            this.txtAbout.Multiline = true;
            this.txtAbout.ReadOnly = true;
            this.txtAbout.BackColor = Color.WhiteSmoke;
            this.txtAbout.Font = new Font("Sarabun", 10F);
            this.txtAbout.ScrollBars = ScrollBars.Vertical;
            this.txtAbout.Location = new Point(30, 30);
            this.txtAbout.Size = new Size(650, 400);

            // เนื้อหา
            this.txtAbout.Text =
@"รองรับอุปกรณ์เครื่องอ่านบัตร ดังนี้
1. ACS ACR39
2. ThaiID Smart Card Reader
3. Omnikey
4. ...
5. ...

Software พัฒนาโดย บริษัท คนทำเว็บ ดอท คอม จำกัด
อีเมล: support@khontamweb.com
เว็บไซต์: https://khontamweb.com
เวอร์ชันโปรแกรม: 1.0.0 (Desktop Reader)";

            this.BackColor = Color.FromArgb(250, 252, 255);
            this.Controls.Add(this.txtAbout);
            this.Dock = DockStyle.Fill;
        }
    }
}
