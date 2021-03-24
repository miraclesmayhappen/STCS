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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Data.SQLite;

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		//AppContext DB;

		public MainWindow()
		{
			InitializeComponent();

		//	DB = new AppContext();

		}

		private void Register_Click(object sender, RoutedEventArgs e)
		{
			//getting info
			string email = Email_textbox.Text.ToLower().Trim();
			string login = Login_textbox.Text.ToLower().Trim();
			string password = Password_textbox.Password.Trim();
			string password_re = Password_re_textbox.Password.Trim();


			//validating fields

			{
				
				if (login.Length == 0 || !login.Any(char.IsLetterOrDigit) || login.Any(char.IsWhiteSpace))
				{
					Login_textbox.ToolTip = "Login cannot be empty or contain whiteblanks and special symbols";
					Login_textbox.Background = Brushes.DarkSalmon;
				}
				else 
				{
					Login_textbox.ToolTip = "";
					Login_textbox.Background = Brushes.LightGreen; }
				
				
				if (email.Length == 0)
				{
					Email_textbox.ToolTip = "Incorrect email";
					Email_textbox.Background = Brushes.DarkSalmon;
				}
				else 
				{
					Email_textbox.ToolTip = "";
					Email_textbox.Background = Brushes.LightGreen;
					if (!EmailValid(email))
					{
						Email_textbox.ToolTip = "Incorrect email";
						Email_textbox.Background = Brushes.DarkSalmon;
					}
					else 
					{ 
						Email_textbox.ToolTip = ""; 
						Email_textbox.Background = Brushes.LightGreen; 
					}
				}

				if (password_re.Length == 0)
				{
					Password_re_textbox.ToolTip = "Re-enter your password";
					Password_re_textbox.Background = Brushes.DarkSalmon;
				}

				else
				{
					Password_re_textbox.ToolTip = "";
					Password_re_textbox.Background = Brushes.LightGreen;
				}

					if (password.Length < 8)
				{
					Password_textbox.ToolTip = "more than 8 symbols - lowercase, uppercase letters, cyrillic letters, digits and special symbols";
					Password_textbox.Background = Brushes.DarkSalmon;
				}

				else
				{
					
						Password_textbox.ToolTip = "";
						Password_textbox.Background = Brushes.LightGreen;
						bool containsAtLeastOneUppercase = password.Any(char.IsUpper);
						bool containsAtLeastOneLowercase = password.Any(char.IsLower);
						bool containsAtLeastOneSpecialChar = password.Any(ch => !Char.IsLetterOrDigit(ch));
						bool containsAtLeastOneDigit = password.Any(char.IsDigit);
						bool containsAtLeastOneCyrillyc = Regex.IsMatch(password, @"\p{IsCyrillic}");
					

					if (!(containsAtLeastOneCyrillyc && containsAtLeastOneDigit && containsAtLeastOneLowercase && containsAtLeastOneSpecialChar && containsAtLeastOneUppercase))
					{
						Password_textbox.ToolTip = "Password should contain lowercase, uppercase letters, cyrillic letters, digits and special symbols";
						Password_textbox.Background = Brushes.DarkSalmon;
					}
					else
						if (!String.Equals(password, password_re))
					{
						Password_re_textbox.ToolTip = "Doesn't match original password";
						Password_re_textbox.Background = Brushes.DarkSalmon;
					}
					else 
					{
						Password_re_textbox.ToolTip = "";
						Password_re_textbox.Background = Brushes.LightGreen;

					//	password = ComputeMDHash(password);




						//check if user already exists
						USER u = null;
						//using (AppContext db = new AppContext())
						//{
						//	u = DB.USERS.Where(a => a.Login == login).FirstOrDefault();
						//}
						TmpFile_Encryption.Tempfile = TmpFile_Encryption.CreateTmpFile();
						TmpFile_Encryption.FileDecrypt("appdb.db.aes", TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);

						SQLCON conn = new SQLCON(TmpFile_Encryption.Tempfile);
						SQLCON.OpenConnection();

						u = SQLCON.FindUser(login);
						SQLCON.CloseConnection();

						if (u == null)
						{
							Login_textbox.ToolTip = "";
							Login_textbox.Background = Brushes.LightGreen;
							USER user = new USER(login, email, password);
							//DB.USERS.Add(user);
							//DB.SaveChanges();
							SQLCON.OpenConnection();
							SQLCON.NewUser(u);
							SQLCON.CloseConnection();
							MessageBox.Show("Succesfully registered!");
						}
						else
						{
							Login_textbox.ToolTip = "This login is already taken";
							Login_textbox.Background = Brushes.DarkSalmon;
						}


						SQLCON.CloseConnection();
						GC.Collect();
						GC.WaitForPendingFinalizers();
						TmpFile_Encryption.FileEncrypt(TmpFile_Encryption.Tempfile, TmpFile_Encryption.Bdpass);
						TmpFile_Encryption.DeleteTmpFile(TmpFile_Encryption.Tempfile);
					}
				}
			}


			
			
		}


		

		public bool EmailValid(string emailaddress)
		{
			//try
			//{
			//	MailAddress m = new MailAddress(emailaddress);

			//	return true;
			//}
			//catch (FormatException)
			//{
			//	return false;
			//}


			return Regex.IsMatch(emailaddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
		}

		private void Help_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "If you are new here, click on Sign Up form, fill your information and click on Register button." + "\n" + " If you are already registered user, click on Sign In form and fill your credentials. " + "\n" + "Good luck!", "MyApp");
		}

		private void About_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "Created by Christine Chudo " + "\n" + " 19. Наявність рядкових і прописних латинських букв, цифр і символів кирилиці." + "\n" + " 2021©", "MyApp");
		}

		private void SignInTransfer_Click(object sender, RoutedEventArgs e)
		{
			SingInWindow signinWindow = new SingInWindow();
			signinWindow.Show();
			this.Hide();
		}

	
		//private void CloseProgram_Click(object sender, RoutedEventArgs e)
		//{

		//}
	}
}
