using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopReader
{
    public partial class UC_TestConnection : UserControl
    {
        private PictureBox picPhoto;
        private HttpClient httpClient;

        // เก็บ Label สำหรับอัปเดตค่า
        private Label lblCidValue, lblThaiNameValue, lblEngNameValue,
                      lblGenderValue, lblBirthValue, lblIssueValue,
                      lblExpireValue, lblIssuerValue, lblAddressValue;

        public UC_TestConnection()
        {
            InitializeComponent();

            httpClient = new HttpClient(new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (msg, cert, chain, errors) => true
            });
        }

        private void InitializeComponent()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.WhiteSmoke;

            Label lblHeader = new Label()
            {
                Text = "ทดสอบการเชื่อมต่อ",
                Font = new Font("Sarabun", 16, FontStyle.Bold),
                Location = new Point(40, 25),
                AutoSize = true
            };
            Controls.Add(lblHeader);

            Button btnRead = new Button()
            {
                Text = "📇 อ่านข้อมูลจากบัตรประชาชน",
                Font = new Font("Sarabun", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(64, 120, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(280, 42),
                Location = new Point(40, 70)
            };
            btnRead.FlatAppearance.BorderSize = 0;
            btnRead.Click += async (s, e) => await BtnRead_ClickAsync(btnRead);
            Controls.Add(btnRead);

            // กรอบข้อมูลหลัก
            Panel infoPanel = new Panel()
            {
                Location = new Point(40, 130),
                Size = new Size(820, 420),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(infoPanel);

            // ================= ซ้าย: รูปผู้ถือบัตร =================
            picPhoto = new PictureBox()
            {
                Size = new Size(200, 260),
                Location = new Point(25, 60),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 250, 255)
            };

            Label lblPhotoTitle = new Label()
            {
                Text = "รูปผู้ถือบัตร",
                Font = new Font("Sarabun", 10, FontStyle.Bold),
                Location = new Point(80, 30),
                AutoSize = true
            };

            infoPanel.Controls.Add(lblPhotoTitle);
            infoPanel.Controls.Add(picPhoto);

            // ================= ขวา: ตารางข้อมูล =================
            TableLayoutPanel tbl = new TableLayoutPanel()
            {
                Location = new Point(250, 20),
                Size = new Size(540, 370),
                ColumnCount = 4,
                RowCount = 6,
                BackColor = Color.White
            };

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // Label ซ้าย
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));   // Value ซ้าย
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130)); // Label ขวา
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));   // Value ขวา

            AddRow(tbl, "เลขบัตร", out lblCidValue, "ชื่อ-สกุล (ไทย)", out lblThaiNameValue);
            AddRow(tbl, "ชื่อ-สกุล (อังกฤษ)", out lblEngNameValue, "เพศ", out lblGenderValue);
            AddRow(tbl, "วันเกิด", out lblBirthValue, "วันออกบัตร", out lblIssueValue);
            AddRow(tbl, "วันหมดอายุ", out lblExpireValue, "ผู้ออกบัตร", out lblIssuerValue);
            AddRow(tbl, "ที่อยู่", out lblAddressValue, "", out _, true);

            infoPanel.Controls.Add(tbl);
        }

        private void AddRow(TableLayoutPanel tbl,
            string labelLeft, out Label lblLeftValue,
            string labelRight, out Label lblRightValue,
            bool multiline = false)
        {
            int row = tbl.RowCount++;
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, multiline ? 60 : 35));

            var lblLeft = CreateLabel(labelLeft, true);
            lblLeftValue = CreateLabel("-", false, multiline);
            var lblRight = CreateLabel(labelRight, true);
            lblRightValue = CreateLabel("-", false, multiline);

            tbl.Controls.Add(lblLeft, 0, row);
            tbl.Controls.Add(lblLeftValue, 1, row);
            tbl.Controls.Add(lblRight, 2, row);
            tbl.Controls.Add(lblRightValue, 3, row);
        }

        private Label CreateLabel(string text, bool isTitle, bool multiline = false)
        {
            var lbl = new Label
            {
                Text = text,
                Font = new Font("Sarabun", isTitle ? 10F : 10F, isTitle ? FontStyle.Bold : FontStyle.Regular),
                Dock = DockStyle.Fill,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = isTitle ? new Padding(0, 0, 5, 0) : new Padding(10, 0, 0, 0)
            };

            lbl.Paint += (s, e) =>
            {
                if (!isTitle)
                {
                    using (var pen = new Pen(Color.Gainsboro) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                        e.Graphics.DrawLine(pen, 0, lbl.Height - 1, lbl.Width, lbl.Height - 1);
                }
            };

            if (multiline) lbl.AutoEllipsis = true;
            return lbl;
        }

        private async Task BtnRead_ClickAsync(Button btn)
        {
            btn.Enabled = false;
            btn.Text = "⏳ กำลังอ่าน...";
            ClearData();

            string apiUrl = "https://127.0.0.1:17890/idcard/read";

            try
            {
                var response = await httpClient.PostAsync(apiUrl, null);
                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"❌ ไม่สามารถเชื่อมต่อได้ ({(int)response.StatusCode})");
                    return;
                }

                var doc = JsonDocument.Parse(result);
                var root = doc.RootElement;

                lblCidValue.Text = TryGet(root, "cid");
                lblThaiNameValue.Text = TryGet(root, "fullname_th");
                lblEngNameValue.Text = TryGet(root, "fullname_en");
                lblGenderValue.Text = TryGet(root, "gender");
                lblBirthValue.Text = TryGet(root, "dob");
                lblIssueValue.Text = TryGet(root, "issue_date");
                lblExpireValue.Text = TryGet(root, "expire_date");
                lblIssuerValue.Text = TryGet(root, "issuer");
                lblAddressValue.Text = TryGet(root, "address");

                string photo = TryGet(root, "photoBase64");
                if (!string.IsNullOrEmpty(photo))
                {
                    byte[] bytes = Convert.FromBase64String(photo);
                    using (MemoryStream ms = new MemoryStream(bytes))
                        picPhoto.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}");
            }
            finally
            {
                btn.Enabled = true;
                btn.Text = "📇 อ่านข้อมูลจากบัตรประชาชน";
            }
        }

        private void ClearData()
        {
            lblCidValue.Text = lblThaiNameValue.Text = lblEngNameValue.Text =
                lblGenderValue.Text = lblBirthValue.Text = lblIssueValue.Text =
                lblExpireValue.Text = lblIssuerValue.Text = lblAddressValue.Text = "-";
            picPhoto.Image = null;
        }

        private string TryGet(JsonElement el, string key)
        {
            return el.TryGetProperty(key, out var v) ? v.GetString() ?? "-" : "-";
        }
    }
}
