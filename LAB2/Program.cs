using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Windows;
using System.Management;

namespace SystemCheck
{
	class Program
	{
		const int SM_CYSCREEN = 1;
		const int KEYBOARDTYPE = 0;
		const int KEYBOARDSUBTYPE = 1;

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int GetSystemMetrics(int nIndex);

		[System.Runtime.InteropServices.DllImport("user32.dll")]
		public static extern int GetKeyboardType(int nTypeFlag);
		static void Main(string[] args)
		{

			string info = "";

			info += Environment.UserName; //имя пользователя

			info += Environment.MachineName; // имя комп

			info += Environment.SystemDirectory; //путь к папке с системными файлами

			info += GetSystemMetrics(SM_CYSCREEN).ToString(); //высота экрана


			string drive = Environment.GetCommandLineArgs()[0].ElementAt(0).ToString();
			DriveInfo dInfo = new DriveInfo(drive);
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

			//Console.WriteLine(hashedinfo);
			//Console.ReadKey();



			RegistryKey currentUserKey = Registry.CurrentUser;
			RegistryKey software = currentUserKey.OpenSubKey("SOFTWARE", true);
			RegistryKey chudo = software.CreateSubKey("CHUDO");
			chudo.SetValue("signature", hashedinfo);
			chudo.Close();
			software.Close();



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
	}
}



