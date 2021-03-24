using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data.SQLite;

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для UserPanel.xaml
	/// </summary>
	public partial class UserPanel : Window
	{
		//AppContext db;
		private string userlogin;
		public string Userlogin
		{
			get { return userlogin; }
			set { userlogin = value; }
		}

		public UserPanel(string un)
		{
			InitializeComponent();

		//	db = new AppContext();
			NameBlock.Text = un;
			this.Userlogin = un;
		}


		private void SignOutButton_Click(object sender, RoutedEventArgs e)
		{
			SingInWindow siWindow = new SingInWindow();
			siWindow.Show();
			//this.Hide();
			this.Close();

		}

		private void ChangePassButton_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("User login: " + this.Userlogin);

			string oldpass, newpass, newpass_re;
			oldpass = OldPassBox.Password.Trim();
			newpass = NewPassBox.Password.Trim();
			newpass_re = NewPassReBox.Password.Trim();

			TmpFile_Encryption.Tempfile = TmpFile_Encryption.CreateTmpFile();
			TmpFile_Encryption.FileDecrypt("appdb.db.aes", TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);

			SQLCON conn = new SQLCON(TmpFile_Encryption.Tempfile);
			SQLCON.OpenConnection();

			//USER thisuser = db.USERS.FirstOrDefault(b => b.Login == this.Userlogin);
			USER thisuser = SQLCON.FindUser(Userlogin);
			SQLCON.CloseConnection();


			if (thisuser != null)
			{

				//WelcomeBlock.Text += ", " + thisuser.Login;

				if (thisuser.Password != oldpass)
				{
					OldPassBox.ToolTip = "Wrong password";
					OldPassBox.Background = Brushes.DarkSalmon;
				}
				else if (thisuser.Password_restrictions == "yes")
				{
					OldPassBox.ToolTip = "";
					OldPassBox.Background = Brushes.LightGreen;

					if (newpass_re.Length == 0)
					{
						NewPassReBox.ToolTip = "Re-enter your password";
						NewPassReBox.Background = Brushes.DarkSalmon;
					}

					else
					{
						NewPassReBox.ToolTip = "";
						NewPassReBox.Background = Brushes.LightGreen;
					}

					if (newpass.Length < 8)
					{
						NewPassBox.ToolTip = "Password length should be more than 8 symbols";
						NewPassBox.Background = Brushes.DarkSalmon;
					}

					else
					{
						NewPassBox.ToolTip = "";
						NewPassBox.Background = Brushes.LightGreen;
						bool containsAtLeastOneUppercase = newpass.Any(char.IsUpper);
						bool containsAtLeastOneLowercase = newpass.Any(char.IsLower);
						bool containsAtLeastOneSpecialChar = newpass.Any(ch => !Char.IsLetterOrDigit(ch));
						bool containsAtLeastOneDigit = newpass.Any(char.IsDigit);
						bool containsAtLeastOneCyrillyc = Regex.IsMatch(newpass, @"\p{IsCyrillic}");

						if (!(containsAtLeastOneCyrillyc && containsAtLeastOneDigit && containsAtLeastOneLowercase && containsAtLeastOneSpecialChar && containsAtLeastOneUppercase))
						{
							NewPassBox.ToolTip = "Password should contain lowercase, uppercase letters, cyrillic letters, digits and special symbols";
							NewPassBox.Background = Brushes.DarkSalmon;
						}
						else
							if (!String.Equals(newpass, newpass_re))
						{
							NewPassReBox.ToolTip = "Doesn't match original password";
							NewPassReBox.Background = Brushes.DarkSalmon;
						}
						else
						{
							NewPassReBox.ToolTip = "";
							NewPassReBox.Background = Brushes.LightGreen;
							thisuser.Password = newpass;
							SQLCON.OpenConnection();
							SQLCON.UpdateTable( thisuser);
							SQLCON.CloseConnection();
							GC.Collect();
							GC.WaitForPendingFinalizers();
							TmpFile_Encryption.FileEncrypt(TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);
							TmpFile_Encryption.DeleteTmpFile(TmpFile_Encryption.Tempfile);
							MessageBox.Show("Password changed");
						}

					}



				}
				else
				{
					if (newpass_re.Length == 0)
					{
						NewPassReBox.ToolTip = "Re-enter your password";
						NewPassReBox.Background = Brushes.DarkSalmon;
					}

					else
					{
						NewPassReBox.ToolTip = "";
						NewPassReBox.Background = Brushes.LightGreen;
						if (!String.Equals(newpass, newpass_re))
						{
							NewPassReBox.ToolTip = "Doesn't match original password";
							NewPassReBox.Background = Brushes.DarkSalmon;
						}
						else
						{
							NewPassReBox.ToolTip = "";
							NewPassReBox.Background = Brushes.LightGreen;
							thisuser.Password = newpass;
							//db.SaveChanges();
							SQLCON.OpenConnection();
							SQLCON.UpdateTable(thisuser);
							SQLCON.CloseConnection();
							GC.Collect();
							GC.WaitForPendingFinalizers();
							TmpFile_Encryption.FileEncrypt(TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);
							TmpFile_Encryption.DeleteTmpFile(TmpFile_Encryption.Tempfile);
							MessageBox.Show("Password changed");
						}

					}

				}

			}

			else
			{
				MessageBox.Show("Oops, something is wrong...");
			}
		}

		private void Help_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "If you are new here, click on Sign Up form, fill your information and click on Register button." + "\n" + " If you are already registered user, click on Sign In form and fill your credentials. " + "\n" + "Good luck!", "MyApp");
		}

		private void About_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "Created by Christine Chudo " + "\n" + " 19. Наявність рядкових і прописних латинських букв, цифр і символів кирилиці." + "\n" + " 2021©", "MyApp");
		}
	}
}

