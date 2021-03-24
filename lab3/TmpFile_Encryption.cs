using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace Wpf_stk1
{
	class TmpFile_Encryption
	{
		private static string bdpass;

		public static string Bdpass
		{
			get { return bdpass; }
			set { bdpass = value; }
		}

		private static string tempfile;

		public static string Tempfile
		{
			get { return tempfile; }
			set { tempfile = value; }
		}
		public static byte[] GenerateRandomSalt()
		{
			byte[] data = new byte[32];

			using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
			{
				for (int i = 0; i < 10; i++)
				{
					// Fille the buffer with the generated data
					rng.GetBytes(data);
				}
			}

			return data;
		}
		public static void FileEncrypt(string inputFile, string password, string outputFile = "appdb.db")
		{
			//http://stackoverflow.com/questions/27645527/aes-encryption-on-large-files

			//generate random salt
			byte[] salt = GenerateRandomSalt();

			//create output file name
			FileStream fsCrypt = new FileStream(outputFile + ".aes", FileMode.Create);

			//convert password string to byte arrray
			byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

			//Set Rijndael symmetric encryption algorithm
			RijndaelManaged AES = new RijndaelManaged();
			AES.KeySize = 256;
			AES.BlockSize = 128;
			AES.Padding = PaddingMode.PKCS7;

			//http://stackoverflow.com/questions/2659214/why-do-i-need-to-use-the-rfc2898derivebytes-class-in-net-instead-of-directly
			//"What it does is repeatedly hash the user password along with the salt." High iteration counts.
			var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
			AES.Key = key.GetBytes(AES.KeySize / 8);
			AES.IV = key.GetBytes(AES.BlockSize / 8);

			//Cipher modes: http://security.stackexchange.com/questions/52665/which-is-the-best-cipher-mode-and-padding-mode-for-aes-encryption
			AES.Mode = CipherMode.ECB;

			// write salt to the begining of the output file, so in this case can be random every time
			fsCrypt.Write(salt, 0, salt.Length);

			CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateEncryptor(), CryptoStreamMode.Write);

			FileStream fsIn = new FileStream(inputFile, FileMode.Open);

			//create a buffer (1mb) so only this amount will allocate in the memory and not the whole file
			byte[] buffer = new byte[1048576];
			int read;

			try
			{
				while ((read = fsIn.Read(buffer, 0, buffer.Length)) > 0)
				{
					//Application.DoEvents(); // -> for responsive GUI, using Task will be better!
					cs.Write(buffer, 0, read);
				}

				// Close up
				fsIn.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}
			finally
			{
				cs.Close();
				fsCrypt.Close();
			}
		}

		public static void FileDecrypt(string inputFile, string outputFile, string password)
		{
			byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
			byte[] salt = new byte[32];

			FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);
			fsCrypt.Read(salt, 0, salt.Length);

			RijndaelManaged AES = new RijndaelManaged();
			AES.KeySize = 256;
			AES.BlockSize = 128;
			var key = new Rfc2898DeriveBytes(passwordBytes, salt, 50000);
			AES.Key = key.GetBytes(AES.KeySize / 8);
			AES.IV = key.GetBytes(AES.BlockSize / 8);
			AES.Padding = PaddingMode.PKCS7;
			AES.Mode = CipherMode.ECB;

			CryptoStream cs = new CryptoStream(fsCrypt, AES.CreateDecryptor(), CryptoStreamMode.Read);

			FileStream fsOut = new FileStream(outputFile, FileMode.Create);

			int read;
			byte[] buffer = new byte[1048576];

			try
			{
				while ((read = cs.Read(buffer, 0, buffer.Length)) > 0)
				{
					//Application.DoEvents();
					fsOut.Write(buffer, 0, read);
				}
			}
			catch (CryptographicException ex_CryptographicException)
			{
				Console.WriteLine("CryptographicException error: " + ex_CryptographicException.Message);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: " + ex.Message);
			}

			try
			{
				cs.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error by closing CryptoStream: " + ex.Message);
			}
			finally
			{
				fsOut.Close();
				fsCrypt.Close();
			}
		}


		public static string CreateTmpFile()
		{
			string fileName = string.Empty;

			try
			{
				fileName = System.IO.Path.GetTempPath() + Guid.NewGuid().ToString() + ".db";

			//	Console.WriteLine("TEMP file created at: " + fileName);
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Unable to create TEMP file or set its attributes: " + ex.Message);
			}

			return fileName;
		}

		public static void DeleteTmpFile(string tmpFile)
		{
			try
			{
				// Delete the temp file (if it exists)
				if (System.IO.File.Exists(tmpFile))
				{
					System.IO.File.Delete(tmpFile);
					//Console.WriteLine("TEMP file deleted.");
				}
			}
			catch (Exception ex)
			{
				//Console.WriteLine("Error deleteing TEMP file: " + ex.Message);
			}
		}
		public static string ComputeMDHash(string rawData)
		{

			// Create a MD5   
			using (MD5 md5hash = MD5.Create())
			{
				// ComputeHash - returns byte array  
				byte[] bytes = md5hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

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
