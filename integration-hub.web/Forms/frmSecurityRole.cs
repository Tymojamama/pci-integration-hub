using DataIntegrationHub.Business.Entities;
using DataIntegrationHub.Business.Components;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataIntegrationHub.UI.Forms
{
    public partial class frmSecurityRole : Form, IMessageFilter
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        public const int WM_LBUTTONDOWN = 0x0201;

        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private HashSet<Control> controlsToMove = new HashSet<Control>();

        public bool PreFilterMessage(ref Message m)
        {
            if (m.Msg == WM_LBUTTONDOWN &&
                 controlsToMove.Contains(Control.FromHandle(m.HWnd)))
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                return true;
            }
            return false;
        }

        public frmMain frmMain;
        private SecurityRole securityRole;

        public frmSecurityRole()
        {
            this.Enabled = false;

            InitializeComponent();

            if (Database.Succeeded())
                this.Enabled = true;
            else
                return;

            //Set maximum size
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            //Add controls to move the form
            Application.AddMessageFilter(this);
            controlsToMove.Add(this.lblFormHeader);
            controlsToMove.Add(this.pnlFormHeader);
            controlsToMove.Add(this.pnlBodySummary);

            this.securityRole = new SecurityRole();

            this.Show();
        }

        public frmSecurityRole(SecurityRole _securityRole)
        {
            this.Enabled = false;

            InitializeComponent();

            if (Database.Succeeded())
                this.Enabled = true;
            else
                return;

            //Set maximum size
            this.MaximumSize = Screen.PrimaryScreen.WorkingArea.Size;

            //Add controls to move the form
            Application.AddMessageFilter(this);
            controlsToMove.Add(this.lblFormHeader);
            controlsToMove.Add(this.pnlFormHeader);
            controlsToMove.Add(this.pnlBodySummary);

            this.securityRole = _securityRole;

            txtName.Text = this.securityRole.Name;
            txtDescription.Text = this.securityRole.Description;

            this.Show();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("A security role must have a name. Please correct and try again.", "Error!", MessageBoxButtons.OK);
                return;
            }

            this.securityRole.Name = txtName.Text;
            this.securityRole.Description = txtDescription.Text;

            this.securityRole.SaveToDatabase(frmMain.CurrentUser.UserId);
            frmMain.LoadUserSecurityRoleDgv();
            this.Close();
        }

        private void labelGray_MouseEnter(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.ForeColor = System.Drawing.Color.Black;
            label.BackColor = System.Drawing.Color.DarkGray;
        }

        private void labelGray_MouseLeave(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            label.ForeColor = System.Drawing.Color.White;
            label.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
        }

        private void lblFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblFormMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
