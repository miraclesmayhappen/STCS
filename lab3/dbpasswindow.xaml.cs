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


namespace Wpf_stk1
{
	/// <summary>
	/// Логика взаимодействия для dbpasswindow.xaml
	/// </summary>
	public partial class dbpasswindow : Window
	{
		public dbpasswindow()
		{
			InitializeComponent();
		}

		private void Enter_Click(object sender, RoutedEventArgs e)
		{
			string password = Password_textbox.Password.Trim();
			string truepass = File.ReadAllText("dbpass.txt");
			if (TmpFile_Encryption.ComputeMDHash(password) != truepass)
			{
				MessageBox.Show("Wrong password. Program will be closed");
				System.Windows.Application.Current.Shutdown();
			}
			else
			{
				TmpFile_Encryption.Bdpass = password;
				SingInWindow signinWindow = new SingInWindow();
				signinWindow.Show();
				this.Close();
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
