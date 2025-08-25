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
using TheArtOfDevHtmlRenderer.Adapters;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SeoulHotel.Forms
{
    public partial class Manage_Hotel : Form
    {
        public int IdItems { get; set; }
        public string Action { get; set; }

        int indexPage = 1;
        public Manage_Hotel()
        {
            InitializeComponent();
   
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        bool isTab2Enabled = false;
        private void PanelMGHotel_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();

            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void LoadDataAmenities()
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();
            try
            {
                String query = "select ID ,Name from Amenities";
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                DataTable dt = new DataTable();

                adapter.Fill(dt);

                DataGridViewAmenities.DataSource = dt;

                DataGridViewAmenities.ColumnHeadersHeight = 40;
                DataGridViewAmenities.RowTemplate.Height = 40;

                DataGridViewAmenities.Columns["ID"].Visible = false;

                DataGridViewAmenities.Columns["Name"].HeaderText = "Amenity";
                DataGridViewAmenities.Columns["Name"].ReadOnly = true;

                if (!DataGridViewAmenities.Columns.Contains("amtCheckBox"))
                {
                    DataGridViewCheckBoxColumn cb = new DataGridViewCheckBoxColumn();

                    cb.Name = "amtCheckBox";
                    cb.HeaderText = "       ✓";
                    cb.ReadOnly = false;

                    cb.TrueValue = true;
                    cb.FalseValue = false;
                    cb.ThreeState = false; // Optional: allows only true/false

                    DataGridViewAmenities.Columns.Add(cb);
                    DataGridViewAmenities.Columns["amtCheckBox"].Width = 100;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Eorror: {ex}");
            }
            conn.Close();
        }

        private void btnCloseFinish_Click(object sender, EventArgs e)
        {
            Management_Form mf = new Management_Form();
            this.Hide();
            mf.FormClosed += (s, args) => this.Close();
            mf.Show();
        }

        private void btnNextMG_Click(object sender, EventArgs e)
        {
            if(indexPage > 2)
            {
                btnNextMG.Enabled = false;
            }
            else
            {
                if (ValidateForm(tabPageListingDetails))
                {
                    string type, title, appro_add, ext_add, des, host_rule;
                    int capacity ,number_bed,number_bedromm,number_bathroom,minimux,maximux;

                    title = TextBoxTitle.Text.Trim();
                    appro_add = TextBoxApproAdd.Text.Trim();
                    ext_add = TextBoxExactAdd.Text.Trim();
                    des = TextBoxDes.Text.Trim();
                    host_rule = TextBoxHostRule.Text.Trim();

                    capacity = Int16.Parse(ndCapacity.Value.ToString()); 
                    number_bed = Int16.Parse(ndNumberOfBed.Value.ToString());
                    number_bedromm = Int16.Parse(ndNumerBedroom.Value.ToString());
                    number_bathroom = Int16.Parse(ndNumberBathroom.Value.ToString());
                    minimux = Int16.Parse(ndMinimux.Value.ToString());
                    maximux = Int16.Parse(ndMaximum.Value.ToString());

                    if (minimux <= maximux)
                    {
                        isTab2Enabled = true;
                        TabControlManageHotel.SelectedTab = TabControlManageHotel.TabPages[indexPage++];

                        MessageBox.Show($"{title}\n{appro_add}\n{ext_add}\n{des}\n" +
                        $"{host_rule}\n{capacity}\n{number_bed}\n{number_bedromm}" +
                        $"\n{number_bathroom}\n{minimux}\n{maximux}");
                    }
                    else
                        MessageBox.Show("Minimux must be less than or equal too  Maximum");
                }
            }
            isTab2Enabled = false;
        }

        private void Manage_Hotel_Load(object sender, EventArgs e)
        {
            guna2AnimateWindow1.SetAnimateWindow(this);

            LoadDataItemTypes();

            foreach (Control ctrl in tabPageListingDetails.Controls)
            {
                if (ctrl is Guna2TextBox tb)
                    tb.TextChanged += (s, ev) => ValidateForm(tabPageListingDetails);

                else if (ctrl is Guna2NumericUpDown n)
                    n.ValueChanged += (s, ev) => ValidateForm(tabPageListingDetails);

                else if (ctrl is Guna2ComboBox cb)
                    cb.SelectedValueChanged += (s, ev) => ValidateForm(tabPageListingDetails);

                else if (ctrl is Guna2RadioButton rb)
                    rb.CheckedChanged += (s, ev) => ValidateForm(tabPageListingDetails);
            }

            switch (Action)
            {
                case "add":
                    labelAddEdit.Text += " Add";
                    LoadDataAmenities();
                    break;
                case "edit":
                    btnNextMG.Visible = false;
                    labelAddEdit.Text += " Edit";


                    break;
            }
        }
        private bool ValidateForm(TabPage gb)
        {
            bool isValideTxt = false;
            bool isValideNUP = false;
            bool isValideCB = false;

            foreach (Control ctl in gb.Controls)
            {
                if (ctl is Guna2TextBox txt)
                {
                    if (string.IsNullOrWhiteSpace(txt.Text))
                    {
                        txt.BorderColor = Color.Red;
                    }
                    else
                    {
                        txt.BorderColor = Color.FromArgb(213, 218, 223);
                        isValideTxt = true;
                    }
                }

                if (ctl is Guna2NumericUpDown n)
                {
                    if (n.Value == 0)
                    {
                        n.BorderColor = Color.Red;
                    }
                    else
                    {
                        n.BorderColor = Color.FromArgb(213, 218, 223);
                        isValideNUP = true;

                    }
                }
                if(ctl is Guna2ComboBox cb)
                {
                    if (string.IsNullOrWhiteSpace(cb.Text))
                        cb.BorderColor = Color.Red;
                    else
                    { 
                        cb.BorderColor = Color.FromArgb(213, 218, 223);
                        isValideCB = true;
                    }
                }
            }
            return isValideCB && isValideNUP && isValideTxt;
        }

        private void TabControlManageHotel_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (e.TabPage == TabControlManageHotel.TabPages[e.TabPageIndex] && !isTab2Enabled)
            {
                e.Cancel = true;
                //MessageBox.Show("Please fieled all.");
            }
        }
        
        private void LoadDataItemTypes()
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();

            try
            {
                String query = "SELECT ID, Name FROM ItemTypes";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ComboBoxItemType.Items.Clear(); // Clear existing items

                while (reader.Read())
                {
                    ComboBoxItemType.Items.Add(new ComboBoxItem
                    {
                        Text = reader["Name"].ToString(),
                        Value = reader["ID"]
                    });
                }

                reader.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
