using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using SeoulHotel.Coonnection;
using SeoulHotel.Forms;
using SeoulHotel.Properties;

namespace SeoulHotel
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();

        }
       
        [DllImport("user32.dll", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
       
        private string _usn, _pwd,_user,_emp, _usn_type,_full_name;
        private byte _UserTypeID;

        private const byte EMPLOYEE = 1;
        private const byte USER = 2;

        private void Move_Form(object sender, MouseEventArgs e)
        {
            ReleaseCapture();

            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
        private void btn_login_exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void cbShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            if (!txtLoginPassword.UseSystemPasswordChar) { 
                txtLoginPassword.UseSystemPasswordChar = true;
            }else
                txtLoginPassword.UseSystemPasswordChar = false;
        }

        private void guna2Panel1_MouseDown(object sender, MouseEventArgs e)
        {
            Move_Form(sender, e);
        }

        private void guna2Panel2_MouseDown(object sender, MouseEventArgs e)
        {
            Move_Form(sender, e);
        }

        private void linkLabel_create_one_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Register register = new Register();
            this.Hide();
            register.FormClosed += (s, args) => this.Close();
            register.Show();
        }

        private void btn_login_Click(object sender, EventArgs e)
        {

            _user = txtLoginUser.Text.Trim();
            _pwd = txtLoginPassword.Text.Trim();
            _emp = txtLoginEmployee.Text.Trim();
            _usn = (!string.IsNullOrEmpty(_emp)) ? _emp : _user;

            if (string.IsNullOrEmpty(_emp) && string.IsNullOrEmpty(_user) && string.IsNullOrEmpty(_pwd))
            {
                txtLoginEmployee.BorderColor = Color.Red;
                txtLoginUser.BorderColor = Color.Red;
                txtLoginPassword.BorderColor = Color.Red;

                lbEmployee.ForeColor = Color.Red;
                lbUser.ForeColor = Color.Red;
                lbPassword.ForeColor = Color.Red;

                lbEmployee.Text = "Field Employee is reqired.";
                lbUser.Text = "Field User is reqired.";
                lbPassword.Text = "Field Password is reqired.";
            }
            else
            {
                txtLoginEmployee.BorderColor = Color.FromArgb(213, 218, 223);
                txtLoginUser.BorderColor = Color.FromArgb(213, 218, 223);
                txtLoginPassword.BorderColor = Color.FromArgb(213, 218, 223);

                lbEmployee.Text = "";
                lbUser.Text = "";
                lbPassword.Text = "";
            }

            if (!string.IsNullOrEmpty(_emp) && !string.IsNullOrEmpty(_user) && !string.IsNullOrEmpty(_pwd))
                MessageBox.Show("You can input only two field username and password.");
            else
            {
                if(!string.IsNullOrEmpty(_emp) && !string.IsNullOrEmpty(_user))
                    MessageBox.Show("Input incorrect! You can input only one Employee or User.");

                if (!string.IsNullOrEmpty(_emp) && string.IsNullOrEmpty(_pwd))
                {
                    txtLoginPassword.BorderColor = Color.Red;
                    lbPassword.ForeColor = Color.Red;
                    lbPassword.Text = "Field Password is reqired.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_emp) && !string.IsNullOrEmpty(_pwd))
                    {
                        // Valid Employee
                        txtLoginPassword.BorderColor = Color.FromArgb(213, 218, 223);
                        lbPassword.Text = "";
                        if (UserLogin())
                        {
                            if (_UserTypeID == EMPLOYEE)
                            {
                                Management_Form mf = new Management_Form();
                                this.Hide();
                                mf.FormClosed += (s, args) => this.Close();
                                mf.Show();
                            }
                            else
                                MessageBox.Show("You are not an Employee.");
                        }
                    }
                }
                    

                if (!string.IsNullOrEmpty(_user) && string.IsNullOrEmpty(_pwd))
                {
                    txtLoginPassword.BorderColor = Color.Red;
                    lbPassword.ForeColor = Color.Red;
                    lbPassword.Text = "Field Password is reqired.";
                }
                else
                {
                    if (!string.IsNullOrEmpty(_user) && !string.IsNullOrEmpty(_pwd))
                    {
                        // Valid User
                        txtLoginPassword.BorderColor = Color.FromArgb(213, 218, 223);
                        lbPassword.Text = "";

                        if (UserLogin())
                        {
                            if (_UserTypeID == USER)
                            {
                                Management_Form mf = new Management_Form();
                                this.Hide();
                                mf.FormClosed += (s, args) => this.Close();
                                mf.Show();
                            }
                            else
                                MessageBox.Show("You are not a User." + _UserTypeID);
                        }
                    }
                }
            }

            

            //if(!string.IsNullOrEmpty(_usn) || !string.IsNullOrEmpty(_pwd)) 
            //{
            //    DB db = new DB();
            //    SqlConnection conn = db.GetConnection();
            //    String query = "SELECT u.Username, u.Password , u.FullName, ut.Name  FROM Users AS u INNER JOIN UserTypes AS ut ON u.UserTypeID = ut.ID WHERE Username = @usn AND Password = @pwd";
            //    try
            //    {
            //        conn.Open();
            //        SqlCommand cmd = new SqlCommand(query,conn);
            //        cmd.Parameters.AddWithValue("@usn",_usn);
            //        cmd.Parameters.AddWithValue("@pwd",_pwd);
            //        SqlDataReader user = cmd.ExecuteReader();
            //        if (user.HasRows)
            //        {
            //            while (user.Read())
            //            {
            //                _usn = user["Username"].ToString();
            //                _pwd = user["Password"].ToString();
            //                _usn_type = user["Name"].ToString();
            //                _full_name = user["FullName"].ToString();
            //            }

            //            if (cbKeepMeSignedIn.Checked)
            //                Properties.Settings.Default.RememberMe = true;
            //            else
            //                Properties.Settings.Default.RememberMe = false;


            //            Properties.Settings.Default.Username = _user;
            //            Properties.Settings.Default.Password = _pwd;
            //            Properties.Settings.Default.UserType = _usn_type;
            //            Properties.Settings.Default.FullName = _full_name;
            //            Properties.Settings.Default.Save();

            //            Management_Form mf = new Management_Form();
            //            this.Hide();
            //            mf.FormClosed += (s, args) => this.Close();
            //            mf.Show();
            //        }
            //        else
            //        {
            //            MessageBox.Show("Invalid username or password!");
            //        }
            //        conn.Close();
            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }

            //}
        }

        private bool UserLogin()
        {
            bool isFound=false;

            DB db = new DB();
            SqlConnection conn = db.GetConnection();
            String query = "SELECT u.UserTypeID, u.Username, u.Password , u.FullName, ut.Name  FROM Users AS u INNER JOIN UserTypes AS ut ON u.UserTypeID = ut.ID WHERE Username = @usn AND Password = @pwd";
            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@usn", _usn);
                cmd.Parameters.AddWithValue("@pwd", _pwd);
                SqlDataReader user = cmd.ExecuteReader();
                if (user.HasRows)
                {
                    while (user.Read())
                    {
                        _UserTypeID = byte.Parse(user["UserTypeID"].ToString());
                        _usn = user["Username"].ToString();
                        _pwd = user["Password"].ToString();
                        _usn_type = user["Name"].ToString();
                        _full_name = user["FullName"].ToString();
                    }

                    if (cbKeepMeSignedIn.Checked)
                        Properties.Settings.Default.RememberMe = true;
                    else
                        Properties.Settings.Default.RememberMe = false;


                    Properties.Settings.Default.Username = _user;
                    Properties.Settings.Default.Password = _pwd;
                    Properties.Settings.Default.UserType = _usn_type;
                    Properties.Settings.Default.FullName = _full_name;
                    Properties.Settings.Default.Save();

                    isFound = true;
                }
                else
                {
                    MessageBox.Show("Invalid username or password!");
                }
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return isFound;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            guna2AnimateWindow1.SetAnimateWindow(this);
        }

        private void btn_login_power_off_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btn_login_minimize_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
