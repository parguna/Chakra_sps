using DI.Chakra.PasswordReset;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DI.Chakra.ResetPasswordUtility
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                string message = string.Empty;

                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("Please enter user name(ADID).", "Error", MessageBoxButtons.OK);
                    return;
                }

                if (!string.IsNullOrEmpty(txtNewPassword.Text) && !string.IsNullOrEmpty(txtConfirmPassword.Text))
                {
                    if ((txtNewPassword.Text == txtConfirmPassword.Text))
                    {
                        LoginHelper loginHelper = new LoginHelper(txtUserName.Text, txtNewPassword.Text);
                        string bRet = loginHelper.ResetPassword(out message);
                        MessageBox.Show(bRet, "Info", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("New Password and Confirm Password Do Not Match,Please reenter.", "Error", MessageBoxButtons.OK);
                    }
                }
                else
                {
                    MessageBox.Show("New Password and Confirm Password should not be empty.", "Error", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                string msg = ex.InnerException != null ? ex.Message + ex.InnerException.ToString() : ex.Message;
                MessageBox.Show(msg);
            }
        }

        private void txtNewPassword_TextChanged(object sender, EventArgs e)
        {
            txtNewPassword.PasswordChar = '*';
        }

        private void txtConfirmPassword_TextChanged(object sender, EventArgs e)
        {
            txtConfirmPassword.PasswordChar = '*';
        }
    }
}
