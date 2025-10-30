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

            // ✅ ปุ่มอ่านบัตร (ตำแหน่งเดิม)
            Button btnRead = new Button()
            {
                Text = "📇 อ่านข้อมูลจากบัตรประชาชน",
                Font = new Font("Sarabun", 11F, FontStyle.Bold),
                BackColor = Color.FromArgb(64, 120, 255),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(280, 42),
                Location = new Point(40, 30)
            };
            btnRead.FlatAppearance.BorderSize = 0;
            btnRead.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            btnRead.Click += async (s, e) => await BtnRead_ClickAsync(btnRead);
            Controls.Add(btnRead);

            // ✅ พื้นหลังห่อ box ให้ไม่หลุดเฟรม
            Panel wrapper = new Panel()
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(40, 80, 40, 20), // เว้นขอบจากปุ่มด้านบน
                BackColor = Color.WhiteSmoke
            };
            Controls.Add(wrapper);

            // ✅ กรอบข้อมูลหลัก (อยู่ในเฟรมพอดี)
            Panel infoPanel = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                Padding = new Padding(20)
            };
            wrapper.Controls.Add(infoPanel);

            // ✅ Layout ภายใน (responsive)
            FlowLayoutPanel contentLayout = new FlowLayoutPanel()
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                WrapContents = false
            };
            infoPanel.Controls.Add(contentLayout);

            // รูปและหัวข้อ
            Label lblPhotoTitle = new Label()
            {
                Text = "รูปผู้ถือบัตร",
                Font = new Font("Sarabun", 10, FontStyle.Bold),
                AutoSize = true
            };

            picPhoto = new PictureBox()
            {
                Size = new Size(150, 200),
                Margin = new Padding(0, 10, 0, 20),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.FromArgb(248, 250, 255)
            };

            FlowLayoutPanel photoLayout = new FlowLayoutPanel()
            {
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true
            };
            photoLayout.Controls.Add(lblPhotoTitle);
            photoLayout.Controls.Add(picPhoto);
            contentLayout.Controls.Add(photoLayout);

            // ✅ ตารางข้อมูล (ยืดหยุ่น)
            TableLayoutPanel tbl = new TableLayoutPanel()
            {
                AutoSize = true,
                Dock = DockStyle.Top,
                ColumnCount = 4,
                BackColor = Color.White
            };

            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40));

            AddRow(tbl, "เลขบัตร", out lblCidValue, "ชื่อ-สกุล (ไทย)", out lblThaiNameValue);
            AddRow(tbl, "ชื่อ-สกุล (อังกฤษ)", out lblEngNameValue, "เพศ", out lblGenderValue);
            AddRow(tbl, "วันเกิด", out lblBirthValue, "วันออกบัตร", out lblIssueValue);
            AddRow(tbl, "วันหมดอายุ", out lblExpireValue, "ผู้ออกบัตร", out lblIssuerValue);
            AddRow(tbl, "ที่อยู่", out lblAddressValue, "", out _, true);

            contentLayout.Controls.Add(tbl);
        }

        private void AddRow(TableLayoutPanel tbl,
            string labelLeft, out Label lblLeftValue,
            string labelRight, out Label lblRightValue,
            bool multiline = false)
        {
            int row = tbl.RowCount++;
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));

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
                AutoSize = false,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = isTitle ? new Padding(0, 0, 5, 0) : new Padding(10, 0, 0, 0)
            };

            if (multiline)
                lbl.MaximumSize = new Size(0, 60);

            lbl.Paint += (s, e) =>
            {
                if (!isTitle)
                {
                    using (var pen = new Pen(Color.Gainsboro) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dot })
                        e.Graphics.DrawLine(pen, 0, lbl.Height - 1, lbl.Width, lbl.Height - 1);
                }
            };
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

                lblCidValue.Text = TryGet(root, "citizenId");
                lblThaiNameValue.Text = TryGet(root, "thFullName");
                lblEngNameValue.Text = TryGet(root, "enFullName");
                lblGenderValue.Text = TryGet(root, "gender");
                lblBirthValue.Text = TryGet(root, "birthDate");
                lblIssueValue.Text = TryGet(root, "issueDate");
                lblExpireValue.Text = TryGet(root, "expireDate");
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
