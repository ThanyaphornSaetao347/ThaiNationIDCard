namespace DesktopReader
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private Button btnRead;
        private TextBox txtOutput;
        private PictureBox picPhoto;


        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.btnRead = new Button();
            this.txtOutput = new TextBox();
            this.picPhoto = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).BeginInit();
            this.SuspendLayout();
            // btnRead
            this.btnRead.Text = "อ่านบัตร";
            this.btnRead.Location = new System.Drawing.Point(12, 12);
            this.btnRead.Size = new System.Drawing.Size(100, 34);
            this.btnRead.Click += new System.EventHandler(this.btnRead_Click);
            // txtOutput
            this.txtOutput.Multiline = true;
            this.txtOutput.ScrollBars = ScrollBars.Vertical;
            this.txtOutput.Location = new System.Drawing.Point(12, 60);
            this.txtOutput.Size = new System.Drawing.Size(420, 300);
            // picPhoto
            this.picPhoto.Location = new System.Drawing.Point(450, 60);
            this.picPhoto.Size = new System.Drawing.Size(200, 240);
            this.picPhoto.SizeMode = PictureBoxSizeMode.Zoom;
            // MainForm
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 380);
            this.Controls.Add(this.btnRead);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.picPhoto);
            this.Text = "Thai ID - Desktop Reader";
            ((System.ComponentModel.ISupportInitialize)(this.picPhoto)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}