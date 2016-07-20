﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BCrypt_Password_Generator.Core.Database;

namespace BCrypt_Password_Generator
{
    delegate void SetTextCallBack(string text, bool isParallelTask, Color ForeColor, Color BackColor);
    public partial class Form1 : Form
    {
        private String Salt;
        private String HashedPassword;
        private String StatusText;
        private String PasswordErrorText;
        private String WorkFactorErrorText;

        int PasswordLength;
        
        public Form1()
        {
            InitializeComponent();
            Salt = "";
            HashedPassword = "";
            PasswordLength = 0;
            StatusText = "";
            
            Initialize();
            
        }

        private void Initialize()
        {
            txtplainPassword.Text = "";
            workFactor.Value = (int)12;
            txtSalt.Text = "";
            txthashedPassword.Text = "";
            txtStatus.Text = "";
            PasswordErrorText = "";
            WorkFactorErrorText = "";
            lblPasswordMatch.Text = "";

            SetStatusLabel_ThreadSafe("", false,default(Color), default(Color));
        }

        private void btn_generatePassword_Click(object sender, EventArgs e)
        {
            StatusText = "";
            txtStatus.Text = "";
            String loadingText = "";
            int WorkFactor = (int)workFactor.Value;
            PasswordLength = txtplainPassword.TextLength;

            SetLabelColor(Color.Red, Color.Black);

            if (txtplainPassword.TextLength > 0 && (int)workFactor.Value > 0)
            {
                SetLabelColor(Color.Red, Color.White);
                loadingText = "BCrypt Password is being Generated, Please wait";


                Salt = BCrypt.Net.BCrypt.GenerateSalt(WorkFactor);

                var Task_HashPassword = Task.Run(() => BCrypt.Net.BCrypt.HashPassword(txtplainPassword.Text, Salt));
                Cursor.Current = Cursors.WaitCursor;

                HashedPassword = Task_HashPassword.Result;

                txtSalt.Text = (string)Salt;
                txthashedPassword.Text = (string)HashedPassword;

                PasswordErrorText = "";
                WorkFactorErrorText = "";
                Task_HashPassword.ContinueWith(t => SetStatusLabel_ThreadSafe("BCrypt Password Successfully Generated", false, Color.Green, Color.White));
                SetLabelColor(Color.Green, Color.White);
            }
            else
            {
                txtplainPassword_Leave(sender, e);
                workFactor_Leave(sender, e);
            }
        }

        private void txtplainPassword_Leave(object sender, EventArgs e)
        {
            if (txtplainPassword.Text.Length > 0)
            {
                PasswordErrorText = "";
                SetStatusLabel_ThreadSafe(WorkFactorErrorText, false, Color.Red, Color.Black);
            }
            else
            {
                PasswordErrorText = "Please enter a valid password";
                SetStatusLabel_ThreadSafe(PasswordErrorText + "\n" + WorkFactorErrorText, false, Color.Red, Color.Black);

            }
        }
        private void workFactor_Leave(object sender, EventArgs e)
        {
            if ((int)workFactor.Value > 0)
            {
                WorkFactorErrorText = "";
                SetStatusLabel_ThreadSafe(PasswordErrorText, false, Color.Red, Color.Black);
            }
            else
            {
                WorkFactorErrorText = "Please Enter an appropriate Work Factor";
                SetStatusLabel_ThreadSafe(WorkFactorErrorText + "\n" + PasswordErrorText, false, Color.Red, Color.Black);
            }
        }

        /// <summary>
        /// sets the Status label of the Generate tab
        /// </summary>
        /// <param name="StatusText"></param>
        private void SetStatusLabel(String StatusText, bool isParallelTask, Color ForeColor, Color BackColor)
        {
            if (isParallelTask)
            {
                Cursor.Current = Cursors.Default;
            }
            else
            {

                txtStatus.Text = StatusText;
                txtStatus.Invalidate();
                txtStatus.Update();
                txtStatus.Refresh();
                SetLabelColor();
            }
            
        }


       private void SetStatusLabel_ThreadSafe(String StatusText, bool isParallelTask, Color ForeColor, Color BackColor)
        {
            if (this.txtStatus.InvokeRequired)
            {
                SetTextCallBack d = new SetTextCallBack(SetStatusLabel);
                this.Invoke(d, new object[] { StatusText, isParallelTask, ForeColor, BackColor });
            }
            else
            {
                SetStatusLabel(StatusText, isParallelTask, ForeColor, BackColor);
            }
        }

        /// <summary>
        /// sets the color and background properties of the status label
        /// </summary>
        private void SetLabelColor()
        {
            if (PasswordErrorText == "" && WorkFactorErrorText == "")
                SetLabelColor(Color.Green, Color.White);
            else
                SetLabelColor(Color.Red, Color.Black);
        }
        private void SetLabelColor(Color ForeColor, Color BackColor)
        {
            txtStatus.ForeColor = ForeColor;
            txtStatus.BackColor = BackColor;
        }

        private void SetLabelGreen()
        {
            txtStatus.ForeColor = System.Drawing.Color.Green;
            txtStatus.BackColor = System.Drawing.Color.White;
        }

        private void SetCheckLabel(String Status, Color ForeColor, Color BackColor)
        {
            lblPasswordMatch.Text = Status;
            lblPasswordMatch.ForeColor = ForeColor;
            lblPasswordMatch.BackColor = BackColor;
        }

        private async void btncheckPassword_Click(object sender, EventArgs e)
        {
            bool isMatch = false;
            try
            {
                isMatch = await Task.Run(() =>
                {
                    try
                    {
                        return BCrypt.Net.BCrypt.Verify(
                      (String)txtPlainPasswordCheck.Text,
                      (String)txtHashedPasswordCheck.Text);

                    }
                    catch
                    {
                        return false;
                    }
                }
           );

                if (isMatch)
                {
                    SetCheckLabel("Passwords Match", Color.Black, Color.Green);
                }
                else
                {
                    SetCheckLabel("Passwords Don't Match", Color.Red, Color.Black);
                }
        }
        catch (BCrypt.Net.SaltParseException e2)
        {
            SetCheckLabel(e2.Message.ToString(), Color.Red, Color.Black);
        }

        SetCursor(Cursors.Default);
        
}
        private void SetCursor(Cursor c)
        {
            Cursor.Current = c;
        }

        public NewUser GetNewUserInfo()
        {
            bool userActive_temp = false;
            if(userIsActive3_Check.Checked == true)
            {
                userActive_temp = true;
            }
            else
            {
                userActive_temp = false;
            }
            if (userID3_Text.Text == "")
            {
                generateGUID.PerformClick();
            }
            return new NewUser
            {
                UserID = new Guid(userID3_Text.Text),
                UserName = (string)userName3_text.Text,
                Password = (string)userPassword3_Text.Text,
                UserFirstName = (string)firstName3_Text.Text,
                UserLastName = (string)lastName3_Text.Text,
                UserAccessLevel = Convert.ToInt32(userAccessLevel_Text.Text),
                UserEmailAddress = (string)userEmailAddress3_Text.Text,
                UserCreatedOn = (DateTime)DateTime.Now,
                UserModifiedOn = (DateTime)DateTime.Now,
                UserActive = userActive_temp,
                UserCreatedByUserID = Guid.NewGuid()
            };
        }

        private void generateGUID_Click(object sender, EventArgs e)
        {
            userID3_Text.Text = Convert.ToString(Guid.NewGuid());

        }


        private void AddtoDB_Click(object sender, EventArgs e)
        {
          
            using (WebsiteDataContext db = new WebsiteDataContext())
            {
                User u = new User();
                NewUser user = new NewUser();
                try
                {

                    user = GetNewUserInfo();
                    u.UserID = user.UserID;
                    u.UserName = user.UserName;
                    u.Password = user.Password;
                    u.UserFirstName = user.UserFirstName;
                    u.UserLastName = user.UserLastName;
                    u.UserAccessLevel = user.UserAccessLevel;
                    u.UserEmailAddress = user.UserEmailAddress;
                    u.UserCreatedOn = user.UserCreatedOn;
                    u.UserModifiedOn = user.UserModifiedOn;
                    //u.UserTimeStamp = user.UserTimeStamp;
                    u.UserActive = user.UserActive;
                    u.UserCreatedByUserID = user.UserCreatedByUserID;
                    db.Users.InsertOnSubmit(u);
                    db.SubmitChanges();
                    errorText3.Text = "User Added Successfully";
                }
                catch (Exception error)
                {
                    errorText3.Text = (string)error.ToString();
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            Search search_object = new Search();
            if(userID3_Text.Text == "" && userName3_text.Text == "" && firstName3_Text.Text == "" && lastName3_Text.Text == "" && userEmailAddress3_Text.Text == "" )
            {
                errorText3.Text = "Please enter atleast one of the required boxes to search for record";
                MessageBox.Show("Enter atleast one field");
            }

            using (WebsiteDataContext db = new WebsiteDataContext())
            {
                IQueryable<User> found_user = null;
                int count = 0;
                try
                {
                    if(userID3_Text.Text != "")
                        found_user = search_object.SearchUser(new Guid(userID3_Text.Text));

                    found_user = search_object.SearchUser(userName3_text.Text, userEmailAddress3_Text.Text);


                    if (found_user != null)
                    {
                        count = found_user.Count();
                        errorText3.Text = "";
                        MessageBox.Show("No of records" +count);
                    }
                }
                catch (Exception error)
                {
                    errorText3.Text = error.ToString();
                }
            }
        }

        private void convertToBcrypt_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(userPassword3_Text.Text))
            {
                userPassword3_Text.Text = BCrypt.Net.BCrypt.HashPassword(userPassword3_Text.Text);
            }
        }
    }
}
