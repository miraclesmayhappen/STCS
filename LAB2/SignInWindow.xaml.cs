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
using Microsoft.Win32;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Management;

namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для SingInWindow.xaml
	/// </summary>
	public partial class SingInWindow : Window
	{
		public int counter = 0;
		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int nIndex);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int GetKeyboardType(int nTypeFlag);
		public SingInWindow()
		{
			if (CheckSignature())
			{ InitializeComponent(); }

			else
			{
				MessageBox.Show("Signatures don't match");
				System.Windows.Application.Current.Shutdown();

			}

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
		private void Help_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "If you are new here, click on Sign Up form, fill your information and click on Register button." + "\n" + " If you are already registered user, click on Sign In form and fill your credentials. " + "\n" + "Good luck!", "MyApp");
		}

		private void About_menu_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(Application.Current.MainWindow, "Created by Christine Chudo " + "\n" + " 19. Наявність рядкових і прописних латинських букв, цифр і символів кирилиці." + "\n" + " 2021©", "MyApp");
		}

		static string ComputeSha256Hash(string rawData)
		{
			// Create a SHA256   
			using (SHA256 sha256Hash = SHA256.Create())
			{
				// ComputeHash - returns byte array  
				byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

				// Convert byte array to a string   
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < bytes.Length; i++)
				{
					builder.Append(bytes[i].ToString("x2"));
				}
				return builder.ToString();
			}
		}

		string GetSysInfo()
		{
			const int SM_CYSCREEN = 1;
			const int KEYBOARDTYPE = 0;
			const int KEYBOARDSUBTYPE = 1;


			string info = "";

			info += Environment.UserName; //имя пользователя

			info += Environment.MachineName; // имя комп

			info += Environment.SystemDirectory; //путь к папке с системными файлами

			info += GetSystemMetrics(SM_CYSCREEN).ToString(); //высота экрана


			DriveInfo dInfo = new DriveInfo("C");
			info += dInfo.TotalSize.ToString(); //объем памяти

			info += GetKeyboardType(KEYBOARDTYPE).ToString();
			info += GetKeyboardType(KEYBOARDSUBTYPE).ToString(); //тип и подтип клавиатуры


			ManagementObjectSearcher moSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
			foreach (ManagementObject wmi_HD in moSearcher.Get())
			{
				info += wmi_HD["SerialNumber"].ToString(); //серийный номер диска
			}

			//Console.WriteLine(info);

			//Console.ReadKey();

			string hashedinfo = ComputeSha256Hash(info);

			return hashedinfo;
		}

		bool CheckSignature ()
		{
			string sysinfo = GetSysInfo();
			string reginfo;

			RegistryKey currentUserKey = Registry.CurrentUser;
			RegistryKey software = currentUserKey.OpenSubKey("SOFTWARE");
			RegistryKey chudo = software.OpenSubKey("CHUDO");
			reginfo = chudo.GetValue("signature").ToString();

			return String.Equals(sysinfo, reginfo);

		}

	}
}
