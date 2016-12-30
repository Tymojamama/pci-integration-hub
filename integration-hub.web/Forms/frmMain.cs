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
    public partial class frmMain : Form, IMessageFilter
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

        public User CurrentUser;

        public frmMain()
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
            controlsToMove.Add(this.pnlFormBody);

            LogInCurrentUser();

            cboViews.SelectedIndex = 0;
        }

        private void LogInCurrentUser()
        {
            string _userName = Environment.UserDomainName + "\\" + Environment.UserName;
            CurrentUser = new User(_userName);
        }

        public void LoadUserSecurityRoleDgv()
        {
            dgvUserSecurityRole.Columns.Clear();

            dgvUserSecurityRole.DataSource = UserSecurityRole.GetAllSecurityRoles();
            dgvUserSecurityRole.Columns[0].Visible = false;
            dgvUserSecurityRole.Columns[1].Visible = false;
            dgvUserSecurityRole.Columns[2].Visible = false;
            dgvUserSecurityRole.Columns["User"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvUserSecurityRole.Columns["SecurityRole"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvUserSecurityRole.Columns["CreatedOn"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            dgvUserSecurityRole.Columns["CreatedBy"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
        }

        private void cboViews_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUserSecurityRoleDgv();
        }

        private void lblFormClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblFormMinimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
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
            label.BackColor = System.Drawing.Color.Maroon;
        }

        private void dgvUserSecurityRole_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                dgvUserSecurityRole.ContextMenuStrip = cmsMain;
                cmsMain.Items["editToolStripMenuItem"].Visible = true;
                cmsMain.Items["deleteToolStripMenuItem"].Visible = true;

                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }
            }
        }

        private void dgvUserSecurityRole_MouseDown(object sender, MouseEventArgs e)
        {
            dgvUserSecurityRole.ContextMenuStrip = cmsMain;
            cmsMain.Items["editToolStripMenuItem"].Visible = false;
            cmsMain.Items["deleteToolStripMenuItem"].Visible = false;
        }

        private void roleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSecurityRole _frmSecurityRole = new frmSecurityRole();
            _frmSecurityRole.frmMain = this;
        }

        private void userRoleToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmUserSecurityRole _frmUserSecurityRole = new frmUserSecurityRole();
            _frmUserSecurityRole.frmMain = this;
        }

        private void securityRoleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvUserSecurityRole.Rows.Count == 0 || dgvUserSecurityRole.DataSource == null || dgvUserSecurityRole.CurrentCell == null)
                return;

            int _index = dgvUserSecurityRole.CurrentCell.RowIndex;
            Guid _securityRoleId = new Guid(dgvUserSecurityRole.Rows[_index].Cells["SecurityRoleId"].Value.ToString());
            SecurityRole _securityRole = new SecurityRole(_securityRoleId);

            frmSecurityRole _frmSecurityRole = new frmSecurityRole(_securityRole);
            _frmSecurityRole.frmMain = this;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            Button btnSender = (Button)sender;
            Point ptLowerLeft = new Point(0, btnSender.Height);
            ptLowerLeft = btnSender.PointToScreen(ptLowerLeft);
            cmsMain.Show(ptLowerLeft);

            cmsMain.Items["newToolStripMenuItem"].Visible = true;
            cmsMain.Items["editToolStripMenuItem"].Visible = false;
            cmsMain.Items["deleteToolStripMenuItem"].Visible = false;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            int index = dgvUserSecurityRole.CurrentCell.RowIndex;
            Guid userSecurityRoleId = new Guid(dgvUserSecurityRole.Rows[index].Cells["UserSecurityRoleId"].Value.ToString());
            UserSecurityRole userSecurityRole = new UserSecurityRole(userSecurityRoleId);

            frmUserSecurityRole frmUserSecurityRole = new frmUserSecurityRole(userSecurityRole);
            frmUserSecurityRole.frmMain = this;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you wish to delete the selected user security role?", "Attention", MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Yes)
            {
                int index = dgvUserSecurityRole.CurrentCell.RowIndex;
                Guid userSecurityRoleId = new Guid(dgvUserSecurityRole.Rows[index].Cells["UserSecurityRoleId"].Value.ToString());
                UserSecurityRole userSecurityRole = new UserSecurityRole(userSecurityRoleId);
                userSecurityRole.DeleteFromDatabase();
                LoadUserSecurityRoleDgv();
            }
        }
    }
}