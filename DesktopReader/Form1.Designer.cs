namespace DesktopReader
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Panel panelMenu;
        private Panel panelHeader;
        private Panel panelContent;
        private Button btnTestConnection;
        private Button btnWebConfig;
        private Button btnAbout;
        private Label lblHeader;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.panelMenu = new Panel();
            this.btnTestConnection = new Button();
            this.btnWebConfig = new Button();
            this.btnAbout = new Button();
            this.panelHeader = new Panel();
            this.lblHeader = new Label();
            this.panelContent = new Panel();

            // panelMenu
            this.panelMenu.BackColor = Color.FromArgb(232, 237, 245);
            this.panelMenu.Dock = DockStyle.Left;
            this.panelMenu.Width = 200;
            this.panelMenu.Controls.Add(this.btnAbout);
            this.panelMenu.Controls.Add(this.btnWebConfig);
            this.panelMenu.Controls.Add(this.btnTestConnection);

            // ปุ่มเมนู
            Button[] menuButtons = { btnTestConnection, btnWebConfig, btnAbout };
            string[] menuNames = { "ทดสอบการเชื่อมต่อ", "ตั้งค่าเชื่อมต่อเว็บ", "เกี่ยวกับโปรแกรม" };
            for (int i = 0; i < menuButtons.Length; i++)
            {
                menuButtons[i].Text = menuNames[i];
                menuButtons[i].FlatStyle = FlatStyle.Flat;
                menuButtons[i].FlatAppearance.BorderSize = 0;
                menuButtons[i].Font = new Font("Sarabun", 10F, FontStyle.Bold);
                menuButtons[i].BackColor = Color.FromArgb(232, 237, 245);
                menuButtons[i].ForeColor = Color.FromArgb(40, 40, 40);
                menuButtons[i].TextAlign = ContentAlignment.MiddleLeft;
                menuButtons[i].Padding = new Padding(20, 0, 0, 0);
                menuButtons[i].Height = 50;
                menuButtons[i].Width = 200;
                menuButtons[i].Location = new Point(0, 50 * i + 20);
                menuButtons[i].Cursor = Cursors.Hand;
                this.panelMenu.Controls.Add(menuButtons[i]);
            }

            // panelHeader
            this.panelHeader.BackColor = Color.FromArgb(245, 247, 252);
            this.panelHeader.Dock = DockStyle.Top;
            this.panelHeader.Height = 60;
            this.lblHeader.Text = "ตั้งค่าเครื่องอ่านบัตร";
            this.lblHeader.Font = new Font("Sarabun", 14F, FontStyle.Bold);
            this.lblHeader.Dock = DockStyle.Fill;
            this.lblHeader.TextAlign = ContentAlignment.MiddleLeft;
            this.lblHeader.Padding = new Padding(20, 0, 0, 0);
            this.panelHeader.Controls.Add(this.lblHeader);

            // panelContent
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.BackColor = Color.WhiteSmoke;

            // MainForm
            this.Text = "Thai ID - Desktop Reader";
            this.ClientSize = new Size(900, 600);
            this.Controls.Add(this.panelContent);
            this.Controls.Add(this.panelHeader);
            this.Controls.Add(this.panelMenu);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.StartPosition = FormStartPosition.CenterScreen;

            this.Load += MainForm_Load;
        }
    }
}
