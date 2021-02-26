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
	/// Логика взаимодействия для UserPanel.xaml
	/// </summary>
	public partial class UserPanel : Window
	{
		AppContext db;
		private string userlogin;
		public string Userlogin
		{
			get { return userlogin; }
			set { userlogin = value; }
		}

		public UserPanel(string un)
		{
			InitializeComponent();

			db = new AppContext();
			NameBlock.Text = un;
			this.Userlogin = un;
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
	}
}

