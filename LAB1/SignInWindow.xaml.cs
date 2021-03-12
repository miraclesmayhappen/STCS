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

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для SingInWindow.xaml
	/// </summary>
	public partial class SingInWindow : Window
	{
		public int counter = 0;
		public SingInWindow()
		{
			InitializeComponent();
		}

		private void SignInButton_Click(object sender, RoutedEventArgs e)
		{
			string login = Login_textbox.Text.Trim().ToLower();
			string password = Password_textbox.Password.Trim();
			

			USER authUser = null;
			using (AppContext db = new AppContext())
			{
				authUser = db.USERS.Where(u => u.Login == login /*&& u.Password == password*/).FirstOrDefault();
			}

			if (authUser != null )
			{
				NoUserAlert.Text = "";
				Password_textbox.ToolTip = "";
				Password_textbox.Background = Brushes.Transparent;
				if (authUser.Password == password)
				{

					if (authUser.Privileges == "user")
					{
						UserPanel up = new UserPanel(login);
						up.Show();
						this.Hide();
					}
					else if (authUser.Privileges == "admin")
					{
						AdminPanel ap = new AdminPanel(login);
						ap.Show();
						this.Hide();

					}
					else if (authUser.Privileges == "banned")
					{
						MessageBox.Show("You are banned.");
						this.Close();
					}
				}
				else
				{
					Password_textbox.ToolTip = "Wrong password";
					Password_textbox.Background = Brushes.DarkSalmon;
					counter++;
				if (counter == 3)
					{
						counter = 0;
						authUser.Privileges = "banned";
						AppContext db = new AppContext();
						db.SaveChanges();
						MessageBox.Show("Failed 3 password entries. Your account is banned. Contact with administrator");
						System.Windows.Application.Current.Shutdown();
					}
					
				}
			}

			else
			{
				NoUserAlert.Text = "No such user";
			}

		}



		private void SignUpTransfer_Click(object sender, RoutedEventArgs e)
		{
			MainWindow mainWindow = new MainWindow();
			mainWindow.Show();
			this.Hide();
		}

	}
}
