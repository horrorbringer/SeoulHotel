using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SeoulHotel.Coonnection;

namespace SeoulHotel.Forms
{
    public partial class Management_Form : Form
    {
        public Management_Form()
        {
            InitializeComponent();

            if (Properties.Settings.Default.UserType == "employee")
            {
                LoadDataHotelMG();
                this.guna2TabControl1.TabPages.RemoveAt(0);
            }
            else if (Properties.Settings.Default.UserType == "user")
            {
                LoadDataForTravaler("");
                this.guna2TabControl1.TabPages.RemoveAt(1);
            }
            else
                return;


        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void Move_Form(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void btn_mgmt_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_mgmt_logout_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Username = "";
            Properties.Settings.Default.Password = "";
            Properties.Settings.Default.RememberMe = false;
            Properties.Settings.Default.UserType = "";
            Properties.Settings.Default.FullName = "";
            Properties.Settings.Default.Save();

            this.Hide();
            Login form1 = new Login();
            form1.FormClosed += (s, args) => this.Close();
            form1.Show();
        }

        private void Management_Form_Load(object sender, EventArgs e)
        {
            guna2AnimateWindow1.SetAnimateWindow(this);
           
        }

        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Move_Form(sender, e);
        }

        private bool LoadDataForTravaler(string search)
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();
            SqlDataAdapter adapter = null;
            bool isFound = false;

            try
            {
                if (search != null && search.Length > 0)
                {
                    adapter = new SqlDataAdapter(db.selectDataSearchForTraveler(), conn);
                    adapter.SelectCommand.Parameters.AddWithValue("@title", $"%{search}%");
                    adapter.SelectCommand.Parameters.AddWithValue("@area", $"%{search}%");
                    adapter.SelectCommand.Parameters.AddWithValue("@type", $"%{search}%");
                }
                else
                { 
                     adapter = new SqlDataAdapter(db.selectDataTraveler(), conn);
                }

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                int item_count = dt.Rows.Count;

                if(item_count <= 0) 
                    labelStatusUser.Text = $"Item not found";
                else if(item_count  == 1)
                    labelStatusUser.Text = $"{dt.Rows.Count.ToString()} item found";
                else if (item_count >= 2)
                    labelStatusUser.Text = $"{dt.Rows.Count.ToString()} items found";

                DataGridViewTravler.DataSource = dt;

                if(adapter == null)
                    isFound = true;

                DataGridViewTravler.ColumnHeadersHeight = 40;
                DataGridViewTravler.RowTemplate.Height = 40;

                DataGridViewTravler.Columns["Title"].HeaderText = "Title";
                DataGridViewTravler.Columns["Capacity"].HeaderText = "Capacity";
                DataGridViewTravler.Columns["Area"].HeaderText = "Area";
                DataGridViewTravler.Columns["Type"].HeaderText = "Type";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eorror: {ex}");
            }
            conn.Close(); 
            return isFound;
        }
        private bool LoadDataHotelMG()
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();
            bool isFound = false;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter(db.selectDataManagementForm(), conn);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                int item_count = dt.Rows.Count;

                if (item_count <= 0)
                    labelStatusUser.Text = $"Item not found";
                else if (item_count == 1)
                    labelStatusUser.Text = $"{dt.Rows.Count.ToString()} item found";
                else if (item_count >= 2)
                    labelStatusUser.Text = $"{dt.Rows.Count.ToString()} items found";

                DataGridViewHotelMG.DataSource = dt;


                if (adapter == null)
                    isFound = true;

                DataGridViewHotelMG.ColumnHeadersHeight = 40;
                DataGridViewHotelMG.RowTemplate.Height = 40;

                DataGridViewHotelMG.Columns["ID"].Visible = false;
                DataGridViewHotelMG.Columns["Title"].HeaderText = "Title";
                DataGridViewHotelMG.Columns["Capacity"].HeaderText = "Capacity";
                DataGridViewHotelMG.Columns["Area"].HeaderText = "Area";
                DataGridViewHotelMG.Columns["Type"].HeaderText = "Type";

                if (!DataGridViewHotelMG.Columns.Contains("btnEdit"))
                {
                    DataGridViewLinkColumn btnEdit = new DataGridViewLinkColumn();

                    btnEdit.Name = "btnEdit";
                    btnEdit.HeaderText = "";
                    btnEdit.Text = "Edit Details";
                    btnEdit.UseColumnTextForLinkValue = true;

                    DataGridViewHotelMG.Columns.Add(btnEdit);
                    DataGridViewHotelMG.Columns["btnEdit"].Width = 100;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eorror: {ex}");
            }
            conn.Close();
            return isFound;
        }

        private void TravalerSearchEvent(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(TextBoxSearchTravaler.Text))
                LoadDataForTravaler(TextBoxSearchTravaler.Text.Trim());
            else
                LoadDataForTravaler("");
        }

        private void TextBoxSearchTravaler_MouseClick(object sender, MouseEventArgs e)
        {
            TravalerSearchEvent(sender, e);
        }

        private void btnAddListing_Click(object sender, EventArgs e)
        {
            this.Hide();
            Manage_Hotel mt = new Manage_Hotel();
            mt.Action = "add";
            mt.FormClosed += (s, args) => this.Close();
            mt.Show();
        }

        private void DataGridViewHotelMG_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DataGridViewHotelMG.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                DataGridViewRow selectedRow = DataGridViewHotelMG.Rows[e.RowIndex];

                string value = selectedRow.Cells[1].Value?.ToString();

                this.Hide();
                Manage_Hotel mt = new Manage_Hotel();
                mt.Action = "edit";
                mt.IdItems = Int16.Parse(value);
                mt.FormClosed += (s, args) => this.Close();
                mt.Show();

            }
        }
    }
}
