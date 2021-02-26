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

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для AdminPanel.xaml
	/// </summary>
	public partial class AdminPanel : Window
	{

		AppContext db;
		private string userlogin;
		public string Userlogin
		{
			get { return userlogin; }
			set { userlogin = value; }
		}
		public AdminPanel(string un)
		{
			InitializeComponent();
			db = new AppContext();

			this.Userlogin = un;


			List<USER> ul = db.USERS.ToList();

			UserList.ItemsSource = ul;

		}

		private void SignOutButton_Click(object sender, RoutedEventArgs e)
		{
			SingInWindow siWindow = new SingInWindow();
			siWindow.Show();
			this.Hide();

		}

		private void ChangePassButton_Click(object sender, RoutedEventArgs e)
		{
			//MessageBox.Show("User login: " + this.Userlogin);

			string oldpass, newpass, newpass_re;
			oldpass = OldPassBox.Password.Trim();
			newpass = NewPassBox.Password.Trim();
			newpass_re = NewPassReBox.Password.Trim();

			USER thisuser = db.USERS.FirstOrDefault(b => b.Login == this.Userlogin);


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
							db.SaveChanges();
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
							db.SaveChanges();
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
			USER user = db.USERS.FirstOrDefault(b => b.Login == login);
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

				db.SaveChanges();
				List<USER> ul = db.USERS.ToList();

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
			USER user = db.USERS.FirstOrDefault(b => b.Login == login);

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

				db.SaveChanges();
				List<USER> ul = db.USERS.ToList();

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
			using (AppContext db = new AppContext())
			{
				u = db.USERS.Where(a => a.Login == login).FirstOrDefault();
			}

			if (u == null && !String.Equals(login, ""))
			{
				NameUserRights.ToolTip = "";
				NameUserRights.Background = Brushes.Transparent;
				USER user = new USER(login, "", "");
				db.USERS.Add(user);
				db.SaveChanges();
				MessageBox.Show("Succesfully registered!");
				List<USER> ul = db.USERS.ToList();

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
	}
}
