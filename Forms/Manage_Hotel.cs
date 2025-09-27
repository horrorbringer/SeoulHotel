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
using Guna.UI2.WinForms.Suite;
using SeoulHotel.Coonnection;
using TheArtOfDevHtmlRenderer.Adapters;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

namespace SeoulHotel.Forms
{
    public partial class Manage_Hotel : Form
    {
        public int IdItems { get; set; }
        public string Action { get; set; }
        List<int> amentitie_items = new List<int>();
        List<int> amentitie_items_edit = new List<int>();

        int indexPage = 0;
        int typeId;
        int areaId;
        int itemId;
        public Manage_Hotel()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);

        bool isTab2Enabled = false;

        private void loadDataTap1Edit()
        {

            int typeId = 0;
            int areaId = 0;

            if (Action != "edit" && IdItems < 0)
                return;
             
            DB db = new DB();
            SqlConnection conn = db.GetConnection();

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(db.selectItem(),conn);
                cmd.Parameters.AddWithValue("@item_id", IdItems);
                SqlDataReader items = cmd.ExecuteReader();

                if (items == null)
                    return;

                while (items.Read())
                {
                    ndCapacity.Value = Convert.ToInt32(items["Capacity"]);
                    ndNumberOfBed.Value = Convert.ToInt32(items["NumberOfBeds"]);
                    ndNumerBedroom.Value = Convert.ToInt32(items["NumberOfBedrooms"]);
                    ndNumberBathroom.Value = Convert.ToInt32(items["NumberOfBathrooms"]);
                    ndMinimux.Value = Convert.ToInt32(items["MinimumNights"]);
                    ndMaximum.Value = Convert.ToInt32(items["MaximumNights"]);

                    typeId = Convert.ToInt32(items["ItemTypeID"]);
                    areaId =Convert.ToInt32(items["AreaID"]);

                    TextBoxTitle.Text = items["Title"].ToString();
                    TextBoxApproAdd.Text = items["ApproximateAddress"].ToString();
                    TextBoxExactAdd.Text = items["ExactAddress"].ToString();
                    TextBoxDes.Text = items["Description"].ToString();
                    TextBoxHostRule.Text = items["HostRules"].ToString();
                }

                foreach (ComboBoxItem item in ComboBoxItemType.Items)
                {
                    if (Convert.ToInt32(item.Value) == typeId)
                    {
                        ComboBoxItemType.SelectedItem = item;
                        break;
                    }
                }

                foreach (ComboBoxItem area in ComboBoxAreaID.Items)
                {
                    if (Convert.ToInt32(area.Value) == areaId)
                    {
                        ComboBoxAreaID.SelectedItem = area;
                        break;
                    }
                }

                items.Close();
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
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
                SqlDataAdapter adapter = new SqlDataAdapter(db.selectDataAmentity(), conn);

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
                    var cb = new DataGridViewCheckBoxColumn
                    {
                        Name = "amtCheckBox",
                        HeaderText = "       ✓",
                        ReadOnly = false,
                        TrueValue = true,
                        FalseValue = false,
                        ThreeState = false,
                        // 👇 ensures new cells start with a valid false value
                        DefaultCellStyle = { NullValue = false }
                    };

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
            if (indexPage > 2)
            {
                btnNextMG.Enabled = false;
                return;
            }

            //  Validate form only on first page
            if (TabControlManageHotel.SelectedTab == TabControlManageHotel.TabPages[0])
            {
                if (!ValidateForm(tabPageListingDetails))
                {
                    return; // stop if validation fails
                }

                string title = TextBoxTitle.Text.Trim();
                string approximateAddress = TextBoxApproAdd.Text.Trim();
                string exactAddress = TextBoxExactAdd.Text.Trim();
                string description = TextBoxDes.Text.Trim();
                string hostRules = TextBoxHostRule.Text.Trim();

                int capacity = (int)ndCapacity.Value;
                int beds = (int)ndNumberOfBed.Value;
                int bedrooms = (int)ndNumerBedroom.Value;
                int bathrooms = (int)ndNumberBathroom.Value;
                int minNights = (int)ndMinimux.Value;
                int maxNights = (int)ndMaximum.Value;

                int userId = Properties.Settings.Default.UserId; // current user id
                Guid guid = Guid.NewGuid();

                if (minNights > maxNights)
                {
                    MessageBox.Show("Minimum must be less than or equal to Maximum");
                    return;
                }

                DB db = new DB();
                SqlConnection conn = db.GetConnection();
                try
                {
                    conn.Open();
                    string sql = "";

                    if (Action == "add")
                        sql = db.insertItems();
                    else if (Action == "edit")
                        sql = db.updateItem();

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@guid", guid);
                        cmd.Parameters.AddWithValue("@user_id", userId);
                        cmd.Parameters.AddWithValue("@item_type_id", typeId);
                        cmd.Parameters.AddWithValue("@area_id", areaId);
                        cmd.Parameters.AddWithValue("@title", title);
                        cmd.Parameters.AddWithValue("@capacity", capacity);
                        cmd.Parameters.AddWithValue("@number_of_beds", beds);
                        cmd.Parameters.AddWithValue("@number_of_bedrooms", bedrooms);
                        cmd.Parameters.AddWithValue("@number_of_bathrooms", bathrooms);
                        cmd.Parameters.AddWithValue("@exact_address", exactAddress);
                        cmd.Parameters.AddWithValue("@approximate_address", approximateAddress);
                        cmd.Parameters.AddWithValue("@description", description);
                        cmd.Parameters.AddWithValue("@host_rule", hostRules);
                        cmd.Parameters.AddWithValue("@mimimum_night", minNights);
                        cmd.Parameters.AddWithValue("@maximum_night", maxNights);

                        if (Action == "edit")
                        {
                            cmd.Parameters.AddWithValue("@item_id", IdItems);
                            if (cmd.ExecuteNonQuery() > 0)
                            {
                                MessageBox.Show($"Item ID: {IdItems} updated success.");
                            }
                        }
                        else
                        {
                            itemId = Convert.ToInt32(cmd.ExecuteScalar() ?? 0); // gets inserted ID
                            if (itemId == 0)
                            {
                                // Insert failed
                                MessageBox.Show($"Something went wrong");
                                return;
                            }
                            MessageBox.Show($"User ID: {userId} inserted success.");
                        }

                        isTab2Enabled = true;
                        indexPage++;
                        TabControlManageHotel.SelectedTab = TabControlManageHotel.TabPages[indexPage];
                        //MessageBox.Show(itemId.ToString());
                    }

                    conn.Close();
                }catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (TabControlManageHotel.SelectedTab == TabControlManageHotel.TabPages[1])
            {
                bool inserted = false;
                DB db = new DB();
                SqlConnection conn = db.GetConnection();
                try
                {
                    conn.Open();
                    string sql = "";

                    if (Action == "add")
                    {
                        sql = db.selectDataItemAmentity();

                        foreach (int amenityId in amentitie_items)
                        {
                            using (SqlCommand cmd = new SqlCommand(db.selectDataItemAmentity(), conn))
                            {
                                cmd.Parameters.AddWithValue("@guid", Guid.NewGuid());
                                cmd.Parameters.AddWithValue("@item_id", itemId);   // itemId = your inserted Item ID
                                cmd.Parameters.AddWithValue("@amenity_id", amenityId);

                                if(cmd.ExecuteNonQuery() > 0)
                                {
                                    inserted = true;
                                }
                            }
                        }

                        if (inserted)
                        {
                            MessageBox.Show("Aemtities added success.");
                        }
                    }

                    if(Action == "edit")
                    {
                        //sql = db.updateItemAmentity();
                    }

                    isTab2Enabled = true;
                    indexPage++;
                    TabControlManageHotel.SelectedTab = TabControlManageHotel.TabPages[indexPage];
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                loadDataDistanceToAtrraction(itemId);

            }
            else if (TabControlManageHotel.SelectedTab == TabControlManageHotel.TabPages[2])
            {
                btnNextMG.Enabled = false;
                isTab2Enabled = false;
                // Show all checked IDs
                
            }
        }

        private void Manage_Hotel_Load(object sender, EventArgs e)
        {
            guna2AnimateWindow1.SetAnimateWindow(this);

            LoadDataItemTypes();
            LoadDataAreas();
            LoadDataAmenities();

            switch (Action)
            {
                case "add":
                    labelAddEdit.Text += " Add";
                    break;
                case "edit":
                    labelAddEdit.Text += " Edit";
                    loadDataTap1Edit();
                    loadDataSelectedAmenityForEdit();

                    labelItem.Text = $"Item ID: {IdItems} updating...";

                    break;
            }
        }
        private bool ValidateForm(TabPage gb)
        {
            bool isValideTxt = true;
            bool isValideNUP = true;
            bool isValideCB = true;

            foreach (Control ctl in gb.Controls)
            {
                if (ctl is Guna2TextBox txt)
                {
                    if (string.IsNullOrWhiteSpace(txt.Text))
                    {
                        txt.BorderColor = Color.Red;
                        MessageBox.Show($"Fields {txt.Tag} is require!");
                        isValideTxt = false;
                    }
                    else
                    {
                        txt.BorderColor = Color.FromArgb(213, 218, 223);
                    }
                }

                else if (ctl is Guna2NumericUpDown n)
                {
                    if (n.Value == 0)
                    {
                        n.BorderColor = Color.Red;
                        MessageBox.Show($"Fields {n.Tag} is require!");
                        isValideNUP = false;
                    }
                    else
                    {
                        n.BorderColor = Color.FromArgb(213, 218, 223);
                    }
                }

                else if (ctl is Guna2ComboBox cb)
                {
                    if (string.IsNullOrWhiteSpace(cb.Text))
                    {
                        cb.BorderColor = Color.Red;
                        MessageBox.Show($"Fields {cb.Tag} is require!");
                        isValideCB = false;
                    }
                    else
                    {
                        cb.BorderColor = Color.FromArgb(213, 218, 223);    
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
                SqlCommand cmd = new SqlCommand(db.selectItemType(), conn);
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
        private void LoadDataAreas()
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();

            try
            {
                SqlCommand cmd = new SqlCommand(db.selectArea(), conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                ComboBoxAreaID.Items.Clear(); // Clear existing items

                while (reader.Read())
                {
                    ComboBoxAreaID.Items.Add(new ComboBoxItem
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

        private void loadDataDistanceToAtrraction(int item_id)
        {
            DB db = new DB();
            SqlConnection conn = db.GetConnection();
            try
            {
                conn.Open();
                using (SqlDataAdapter adapter = new SqlDataAdapter(db.selectDataDistanceToAtrraction(), conn))
                {
                    adapter.SelectCommand.Parameters.AddWithValue("@item_id",item_id);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    DataGridViewDTA.DataSource = dt;


                    DataGridViewDTA.ColumnHeadersHeight = 40;
                    DataGridViewDTA.RowTemplate.Height = 40;

                    //DataGridViewDTA.Columns["ID"].Visible = false;

                    //DataGridViewDTA.Columns["Name"].HeaderText = "Amenity";
                    DataGridViewDTA.Columns["Name"].ReadOnly = true;
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ComboBoxItemType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxItemType.SelectedIndex >= 0)
            {
                if (ComboBoxItemType.SelectedItem is ComboBoxItem selectedItem)
                {
                    typeId = Convert.ToInt32(selectedItem.Value); // safe conversion
                    //MessageBox.Show($"Selected ID = {type_id}, Name = {selectedItem.Text}");
                }
            }
        }

        private void ComboBoxAreaID_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ComboBoxAreaID.SelectedIndex >= 0)
            {
                if (ComboBoxAreaID.SelectedItem is ComboBoxItem selectedItem)
                {
                    areaId = Convert.ToInt32(selectedItem.Value);
                }
            }
        }

        // Trigger when checkbox value actually changes
        private void DataGridViewAmenities_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && DataGridViewAmenities.Columns[e.ColumnIndex].Name == "amtCheckBox")
            {
                bool isChecked = Convert.ToBoolean(DataGridViewAmenities.Rows[e.RowIndex].Cells["amtCheckBox"].Value);
                
                int id = Convert.ToInt32(DataGridViewAmenities.Rows[e.RowIndex].Cells["ID"].Value);
                if (isChecked)
                {
                    amentitie_items.Add(id);
                }
            }
        }

        // This forces checkbox changes to commit immediately after click
        private void DataGridViewAmenities_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (DataGridViewAmenities.IsCurrentCellDirty)
            {
                DataGridViewAmenities.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void loadDataSelectedAmenityForEdit()
        {
            DB db = new DB();
            using (SqlConnection conn = db.GetConnection())
            {
                try
                {
                    // Get the list of selected Amenity IDs for this item
                    using (SqlCommand cmd = new SqlCommand(db.selectAmenityOfItem(), conn))
                    {
                        cmd.Parameters.AddWithValue("@item_id", IdItems);
                        conn.Open();

                        HashSet<int> selectedIds = new HashSet<int>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                selectedIds.Add(Convert.ToInt32(reader["AmenityID"]));
                            }
                        }

                        foreach (DataGridViewRow row in DataGridViewAmenities.Rows)
                        {
                            if (row.IsNewRow || row.Cells["ID"].Value == null) continue;

                            int id = Convert.ToInt32(row.Cells["ID"].Value);
                            row.Cells["amtCheckBox"].Value = selectedIds.Contains(id);
                        }
                        DataGridViewAmenities.EndEdit();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DataGridViewAmenities_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Make sure we are clicking the checkbox column and not the header
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var grid = (DataGridView)sender;

            if (grid.Columns[e.ColumnIndex].Name == "amtCheckBox")
            {
                // Commit the change immediately so CurrentCell.Value is updated
                grid.CommitEdit(DataGridViewDataErrorContexts.Commit);

                DataGridViewRow row = grid.Rows[e.RowIndex];

                if (row.Cells["ID"].Value == null) return;

                int amenityId = Convert.ToInt32(row.Cells["ID"].Value);
                bool isChecked = Convert.ToBoolean(row.Cells["amtCheckBox"].Value);

                if (isChecked)
                {
                    // Add only if not already present
                    if (!amentitie_items.Contains(amenityId))
                        amentitie_items.Add(amenityId);
                }
                else
                {
                    // Remove if unchecked
                    amentitie_items.Remove(amenityId);
                }

                // Optional: for debugging
                // MessageBox.Show(string.Join(", ", amentitie_items));
            }
        }
    }
}
