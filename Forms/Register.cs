using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
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
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
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
        private void btn_return_login_Click(object sender, EventArgs e)
        {
            Login form1 = new Login();
            this.Hide();
            form1.FormClosed += (s, args) => this.Close();
            form1.Show();
        }
        private bool ValidateForm(GroupBox gb)
        {
            bool isValideTxt = false;
            bool isValideNUP = false;
            bool isValideG = false;

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
                if (ctl is Guna2RadioButton rb)
                {
                    if (!rb.Checked)
                    {
                        rb.ForeColor = Color.Red;
                    }
                    else
                    {
                        rdFemale.ForeColor = Color.Black;
                        rdMale.ForeColor = Color.Black;
                        isValideG = true;
                    }
                }
            }
            return isValideG && isValideNUP && isValideTxt;
        }
        private void btn_register_login_Click(object sender, EventArgs e)
        {
            string usn, pwd, rpwd, fln, dob;
            byte g;
            decimal nf;
            byte userType = 2;

            usn = TextBoxRgUsn.Text.Trim();
            fln = TextBoxRgFullName.Text.Trim();
            dob = dateTimePickerRg.Value.ToString("yyyy-MM-dd");
            pwd = TextBoxRgPwd.Text.Trim();
            rpwd = TextBoxRgRePwd.Text.Trim();
            nf = NumericUpDownRgNFM.Value;

            if (rdFemale.Checked)
                g = 1;
            else if(rdMale.Checked)
                g = 0;
            else
                g = 2;

            if (!ValidateForm(groupBox1))
                return;
            else
            {
                if (pwd.Length > 5)
                {
                    if (pwd == rpwd)
                    {
                        if (CheckBoxRgAgree.Checked)
                        {
                            Properties.Settings.Default.Username = usn;
                            Properties.Settings.Default.Password = pwd;
                            Properties.Settings.Default.UserType = "user";
                            Properties.Settings.Default.FullName = fln;
                            Properties.Settings.Default.Save();

                            DB db = new DB();
                            SqlConnection conn = db.GetConnection();
                            Guid newUserId = Guid.NewGuid();
                            String query = @"INSERT INTO Users (GUID,UserTypeID,Username,Password,FullName,Gender,BirthDate,FamilyCount)
                                            VALUES (@guid,@user_type,@usn,@pwd,@fln,@g,@dob,@flc)";
                            try
                            {
                                SqlCommand cmd = new SqlCommand(query,conn);

                                cmd.Parameters.AddWithValue("@guid",newUserId);
                                cmd.Parameters.AddWithValue("@user_type",userType);
                                cmd.Parameters.AddWithValue("@usn",usn);
                                cmd.Parameters.AddWithValue("@pwd",pwd);
                                cmd.Parameters.AddWithValue("@fln",fln);
                                cmd.Parameters.AddWithValue("@g",g);
                                cmd.Parameters.AddWithValue("@dob",dob);
                                cmd.Parameters.AddWithValue("@flc",nf);

                                conn.Open();
                                if (cmd.ExecuteNonQuery() > 0)
                                    MessageBox.Show("Account created success fully!");
                                else
                                    MessageBox.Show("Couldn't created!");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message );
                            }
                            finally
                            {
                                conn.Close();
                                Management_Form mf = new Management_Form();
                                this.Hide();
                                mf.FormClosed += (s, args) => this.Close();
                                mf.Show();
                            }
                        }
                        else
                            MessageBox.Show("Please agree Terms and Conditions");
                    }
                    else
                        MessageBox.Show("Password and Retype password not match.");
                }
                else
                    MessageBox.Show("Password must be greater than 5 charecters");
            }
        }

        private void Register_Load(object sender, EventArgs e)
        {
            guna2AnimateWindow1.SetAnimateWindow(this);

            foreach (Control ctrl in groupBox1.Controls)
            {
                if (ctrl is Guna2TextBox tb)
                    tb.TextChanged += (s, ev) => ValidateForm(groupBox1);

                else if (ctrl is Guna2NumericUpDown n)
                    n.ValueChanged += (s, ev) => ValidateForm(groupBox1);

                //else if (ctrl is DateTimePicker dtp)
                //    dtp.ValueChanged += (s, ev) => ValidateForm(groupBox1);

                else if (ctrl is Guna2RadioButton rb)
                    rb.CheckedChanged += (s, ev) => ValidateForm(groupBox1);

                // You can add more control types if needed
            }
        }

        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Move_Form(sender, e);
        }

        private void linkLabelVTC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                // Open the file with Notepad
                Process.Start("notepad.exe", "Terms.txt");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open Notepad: " + ex.Message);
            }
        }

    }
}
