using System.Windows.Forms;

namespace WinFormsDemo
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.Text = "TimingBar – WinForms Demo";
            this.ClientSize = new System.Drawing.Size(640, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
        }
    }
}
