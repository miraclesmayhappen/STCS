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
using System.Security.Cryptography;
using System.IO;
using System.Text.RegularExpressions;

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для AdminPanel.xaml
	/// </summary>
	public partial class AdminPanel : Window
	{

		//AppContext db;
		SQLCON conn;
		private string userlogin;
		public string Userlogin
		{
			get { return userlogin; }
			set { userlogin = value; }
		}
		public AdminPanel(string un)
		{
			InitializeComponent();
			//db = new AppContext();

			this.Userlogin = un;

			TmpFile_Encryption.Tempfile = TmpFile_Encryption.CreateTmpFile();
			TmpFile_Encryption.FileDecrypt("appdb.db.aes", TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);

			conn = new SQLCON(TmpFile_Encryption.Tempfile);
			SQLCON.OpenConnection();

			List<USER> ul = SQLCON.GetList();


			UserList.ItemsSource = ul;

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

			//USER thisuser = db.USERS.FirstOrDefault(b => b.Login == this.Userlogin);
			SQLCON.OpenConnection();
			USER thisuser = SQLCON.FindUser(Userlogin);
			SQLCON.CloseConnection();


			if (thisuser != null)
			{

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
							SQLCON.UpdateTable(thisuser);
							SQLCON.CloseConnection();
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
							SQLCON.OpenConnection();
							SQLCON.UpdateTable(thisuser);
							SQLCON.CloseConnection();
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

		private void PassRestrictions_Click(object sender, RoutedEventArgs e)
		{
			string login = NameUserRights.Text.Trim().ToLower();
			SQLCON.OpenConnection();
			USER user = SQLCON.FindUser(login);
			SQLCON.CloseConnection();
			if (user != null)
			{
				NameUserRights.ToolTip = "";
				NameUserRights.Background = Brushes.Transparent;
				if (user.Password_restrictions == "yes")
				{
					user.Password_restrictions = "no";
					MessageBox.Show(login + "'s password settings changed");
				}
				else
				{
					user.Password_restrictions = "yes";
					MessageBox.Show(login + "'s password settings changed");
				}

				SQLCON.OpenConnection();
				SQLCON.UpdateTable(user);


				List<USER> ul = SQLCON.GetList();
				SQLCON.CloseConnection();
				UserList.ItemsSource = ul;
			}
			else
			{
				NameUserRights.ToolTip = "No such user found";
				NameUserRights.Background = Brushes.DarkSalmon;
			}


		}

		private void Ban_Click(object sender, RoutedEventArgs e)
		{
			string login = NameUserRights.Text.Trim().ToLower();
			SQLCON.OpenConnection();
			USER user = SQLCON.FindUser(login);
			SQLCON.CloseConnection();

			if (user != null)
			{
				NameUserRights.ToolTip = "";
				NameUserRights.Background = Brushes.Transparent;
				if (user.Privileges == "banned")
				{
					user.Privileges = "user";
					MessageBox.Show(login + " is unbanned now");
				}
				else
				{
					user.Privileges = "banned";
					MessageBox.Show(login + " is banned now");
				}

				SQLCON.OpenConnection();
				SQLCON.UpdateTable(user);
				List<USER> ul = SQLCON.GetList();
				SQLCON.CloseConnection();
				UserList.ItemsSource = ul;


			}
			else
			{
				NameUserRights.ToolTip = "No such user found";
				NameUserRights.Background = Brushes.DarkSalmon;
			}

		}

		private void NewUser_Click(object sender, RoutedEventArgs e)
		{
			string login = NameUserRights.Text.Trim().ToLower();

			USER u = null;
			//using (AppContext db = new AppContext())
			//{
			//	u = db.USERS.Where(a => a.Login == login).FirstOrDefault();
			//}
			SQLCON.OpenConnection();
			u = SQLCON.FindUser(login);
			SQLCON.CloseConnection();

			if (u == null && !String.Equals(login, ""))
			{
				NameUserRights.ToolTip = "";
				NameUserRights.Background = Brushes.Transparent;
				USER user = new USER(login, "", "");
				//db.USERS.Add(user);
				//db.SaveChanges();
				SQLCON.OpenConnection();
				SQLCON.NewUser(user);
				MessageBox.Show("Succesfully registered!");
				List<USER> ul = SQLCON.GetList();
				SQLCON.CloseConnection();

				UserList.ItemsSource = ul;
			}
			else
			{
				if (String.Equals(login, ""))
				{
					NameUserRights.ToolTip = "Empty login";
				}
				else
				{ NameUserRights.ToolTip = "This login is already taken"; }
				NameUserRights.Background = Brushes.DarkSalmon;
			}
		}

		private void ChangeDBPassButton_Click(object sender, RoutedEventArgs e)
		{
			string oldpass, newpass, newpass_re;
			oldpass = OldDBPass.Password.Trim();
			newpass = NewDBPass.Password.Trim();
			newpass_re = ReNewDBPass.Password.Trim();

			if (TmpFile_Encryption.Bdpass != oldpass)
			{

				OldDBPass.ToolTip = "Wrong password";
				OldDBPass.Background = Brushes.DarkSalmon;
			}
			else
			{
				OldDBPass.ToolTip = "";
				OldDBPass.Background = Brushes.LightGreen;

				if (newpass.Length == 0)
				{
					NewDBPass.ToolTip = "Enter your password";
					NewDBPass.Background = Brushes.DarkSalmon;
				}
				else
				{
					NewDBPass.ToolTip = "";
					NewDBPass.Background = Brushes.LightGreen;
					if (newpass_re.Length == 0)
					{
						ReNewDBPass.ToolTip = "Re-enter your password";
						ReNewDBPass.Background = Brushes.DarkSalmon;
					}

					else
					{
						ReNewDBPass.ToolTip = "";
						ReNewDBPass.Background = Brushes.LightGreen;

						if (!String.Equals(newpass, newpass_re))
						{
							ReNewDBPass.ToolTip = "Doesn't match original password";
							ReNewDBPass.Background = Brushes.DarkSalmon;
						}
						else
						{
							ReNewDBPass.ToolTip = "";
							ReNewDBPass.Background = Brushes.LightGreen;
							TmpFile_Encryption.Bdpass = newpass;
							File.WriteAllText("dbpass.txt", TmpFile_Encryption.ComputeMDHash(TmpFile_Encryption.Bdpass));

						}
					}

				}

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

		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
			{
				SQLCON.CloseConnection();
				GC.Collect();
				GC.WaitForPendingFinalizers();
				TmpFile_Encryption.FileEncrypt(TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);
				TmpFile_Encryption.DeleteTmpFile(TmpFile_Encryption.Tempfile);
				System.Windows.Application.Current.Shutdown();
			}
		
	} 
} 
