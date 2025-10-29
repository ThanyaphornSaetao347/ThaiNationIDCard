using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace DesktopReader
{
    public partial class UC_WebConfig : UserControl
    {
        private TextBox txtOrigins;
        private Button btnSave;
        private Button btnTest;
        private Label lblInfo;
        private TextBox txtLog;

        private string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "allowed-origins.txt");

        public UC_WebConfig()
        {
            InitializeComponent();
            LoadOrigins();
        }

        private void InitializeComponent()
        {
            this.txtOrigins = new TextBox();
            this.btnSave = new Button();
            this.btnTest = new Button();
            this.lblInfo = new Label();
            this.txtLog = new TextBox();

            // Label อธิบาย
            this.lblInfo.Text = "กรุณาระบุรายชื่อเว็บไซต์ที่อนุญาตให้เชื่อมต่อกับอุปกรณ์ (ทีละบรรทัด)";
            this.lblInfo.Font = new Font("Sarabun", 10F);
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new Point(20, 20);

            // TextBox สำหรับกรอกเว็บไซต์
            this.txtOrigins.Multiline = true;
            this.txtOrigins.ScrollBars = ScrollBars.Vertical;
            this.txtOrigins.Font = new Font("Consolas", 10F);
            this.txtOrigins.Location = new Point(20, 50);
            this.txtOrigins.Size = new Size(600, 200);

            // ปุ่มบันทึก
            this.btnSave.Text = "💾 บันทึกการตั้งค่า";
            this.btnSave.Font = new Font("Sarabun", 10F, FontStyle.Bold);
            this.btnSave.BackColor = Color.FromArgb(64, 120, 255);
            this.btnSave.ForeColor = Color.White;
            this.btnSave.FlatStyle = FlatStyle.Flat;
            this.btnSave.Size = new Size(180, 40);
            this.btnSave.Location = new Point(20, 270);
            this.btnSave.Click += BtnSave_Click;

            // ปุ่มทดสอบเชื่อมต่อ
            this.btnTest.Text = "🌐 ทดสอบการเชื่อมต่อ";
            this.btnTest.Font = new Font("Sarabun", 10F, FontStyle.Bold);
            this.btnTest.BackColor = Color.FromArgb(0, 180, 80);
            this.btnTest.ForeColor = Color.White;
            this.btnTest.FlatStyle = FlatStyle.Flat;
            this.btnTest.Size = new Size(200, 40);
            this.btnTest.Location = new Point(220, 270);
            this.btnTest.Click += async (s, e) => await TestConnectionsAsync();

            // กล่อง log แสดงผลการทดสอบ
            this.txtLog.Multiline = true;
            this.txtLog.ScrollBars = ScrollBars.Vertical;
            this.txtLog.Font = new Font("Consolas", 9F);
            this.txtLog.Location = new Point(20, 330);
            this.txtLog.Size = new Size(600, 150);

            // Layout
            this.BackColor = Color.FromArgb(250, 252, 255);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.txtOrigins);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.txtLog);
            this.Dock = DockStyle.Fill;
        }

        private void LoadOrigins()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    txtOrigins.Text = File.ReadAllText(filePath);
                }
                else
                {
                    txtOrigins.Text = "127.0.0.1\r\n";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถโหลดไฟล์ allowed-origins.txt ได้\n" + ex.Message,
                    "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllText(filePath, txtOrigins.Text.Trim());
                MessageBox.Show("บันทึกข้อมูลเรียบร้อยแล้ว ✅", "สำเร็จ",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถบันทึกไฟล์ได้\n" + ex.Message,
                    "ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task TestConnectionsAsync()
        {
            string[] lines = txtOrigins.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            txtLog.Clear();

            using (HttpClient client = new HttpClient())
            {
                foreach (string line in lines)
                {
                    string url = line.Trim();
                    if (!url.StartsWith("http"))
                        url = "https://" + url;

                    try
                    {
                        txtLog.AppendText($"กำลังทดสอบ {url} ... ");
                        HttpResponseMessage res = await client.GetAsync(url);
                        if (res.IsSuccessStatusCode)
                            txtLog.AppendText("✅ สำเร็จ\r\n");
                        else
                            txtLog.AppendText($"❌ ({(int)res.StatusCode})\r\n");
                    }
                    catch
                    {
                        txtLog.AppendText("❌ เชื่อมต่อไม่สำเร็จ\r\n");
                    }
                }
            }
        }
    }
}
