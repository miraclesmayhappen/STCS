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
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace RGR_wpf
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	/// 

	class Person
	{
		public string username;
		public double internal_average, external_average, math_exp, disp;
		public Person()
		{
			username = "";
			internal_average = 0.0;
			external_average = 0.0;
			math_exp = 0.0;
			disp = 0.0;
		}
		public Person(string un)
		{
			username = un;
			internal_average = 0.0;
			external_average = 0.0;
			math_exp = 0.0;
			disp = 0.0;
		}
		public Person GetPerson(string username)
		{
			Person person = new Person();
			string line;
			// Read the file line by line.  
			// username	internal	external	math_exp	disp
			System.IO.StreamReader file = new System.IO.StreamReader(MainWindow.database);
			while ((line = file.ReadLine()) != null)
			{
				string[] words = line.Split('	');
				if (words[0] == username)
				{
					person.username = words[0];
					person.internal_average = Convert.ToDouble(words[1]);
					person.external_average = Convert.ToDouble(words[2]);
					person.math_exp = Convert.ToDouble(words[3]);
					person.disp = Convert.ToDouble(words[4]);
					break;
				}
			}
			file.Close();

			return person;
		}

		public bool Exists(string username)
		{
			string line;
			bool exists = false;
			System.IO.StreamReader file = new System.IO.StreamReader(MainWindow.database);
			while ((line = file.ReadLine()) != null)
			{
				string[] words = line.Split('	');
				if (words[0] == username)
				{
					exists = true;
					break;
				}
			}
			file.Close();
			return exists;
		}

		public void AddToDB()
		{
			using (StreamWriter sw = File.AppendText(MainWindow.database))
			{
				sw.WriteLine(username + "	" + internal_average.ToString() + "	" + external_average.ToString() + "	" + math_exp.ToString() + "	" + disp.ToString() + "\n");
			}
		}
	}


	public partial class MainWindow : Window
	{
		public static Stopwatch stopwatch_internal;
		public static Stopwatch stopwatch_external;


		public static TimeSpan internal_ts, external_ts;

		public static List<double> internal_time_ls;
		public static List<double> external_time_ls;

		int i = 0;

		public const string database = "users.txt";

		static Person person;

		bool userexists = false;
		bool trainingcomleted = false;

		string username = "";

		public MainWindow()
		{
			InitializeComponent();
			stopwatch_internal = new Stopwatch();
			stopwatch_external = new Stopwatch();
			person = new Person();
			internal_time_ls = new List<double>();
			external_time_ls = new List<double>();
			
		}



		static double Average(List<double> list)
		{
			double av = 0.0;
			double sum = 0.0;
			foreach (var v in list)
			{
				sum += v;
			}
			av = sum / list.Count;
			return av;
		}

		static List<double> MathExpectation(List<double> list)
		{
			List<double> ME = new List<double>();
			double current = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (j != i)
					{
						current += list[j];
					}
				}
				current /= list.Count - 1;
				ME.Add(current);
				current = 0.0;
			}

			return ME;
		}

		static List<double> Dispersion(List<double> list)
		{
			List<double> Disp = new List<double>();
			List<double> ME = MathExpectation(list);
			double current = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				for (int j = 0; j < list.Count; j++)
				{
					if (j != i)
					{
						current += Math.Pow((list[j] - ME[i]), 2);
					}
				}
				current /= list.Count - 1;
				Disp.Add(current);
				current = 0.0;
			}


			return Disp;
		}

		static List<double> StandartDeviation(List<double> list)
		{
			List<double> Dev = new List<double>();
			List<double> Disp = Dispersion(list);
			foreach (var v in Disp)
			{
				Dev.Add(Math.Sqrt(v));
			}

			return Dev;
		}

		static List<double> StudentCoeficient(List<double> list)
		{
			List<double> Student = new List<double>();
			List<double> ME = MathExpectation(list);
			List<double> SD = StandartDeviation(list);

			for (int i = 0; i < list.Count; i++)
			{
				Student.Add(Math.Abs((list[i] - ME[i]) / SD[i]));
			}

			return Student;
		}

		static double FindInTableStudent(int df, double a=0.05)
		{
			double sc = 100;
			string line;
			System.IO.StreamReader file = new System.IO.StreamReader("student.txt");
			while ((line = file.ReadLine()) != null)
			{
				string[] words = line.Split('	');
				if (words[0] == "df")
				{
					continue;
				}
				else
				if (Convert.ToInt32(words[0]) == df)
				{
					if (a == 0.05)
					{ sc = Convert.ToDouble(words[1]); break; }
					else
					{

						if (a == 0.01)
						{ sc = Convert.ToDouble(words[2]); break; }
						else
						{

							if (a == 0.001)
							{ sc = Convert.ToDouble(words[3]); break; }
						}
					}
					
				}
			}
			file.Close();
			return sc;
		}

		static List<double> CheckWithStudent(List<double> list)
		{
			if (list.Count > 2)
			{
				int df = list.Count - 2; //degree of freedom
				double student_table = FindInTableStudent(df);
				List<double> SC = StudentCoeficient(list);
				List<int> delete_index = new List<int>();

				int i = 0;
				while (i < list.Count)
				{
					if (SC[i] > student_table)
					{

						SC.RemoveAt(i);
					}
					else i++;
				}


				return list;
			}
			else return new List<double>();
		}


		static double FindInTableFisher(int k1, int k2)
		{
			double fc = 100;
			string line;
			if (k1>6&&k1!=8&&k1!=12&&k1!=24)
			{
				int to6, to8, to12, to24;
				to6 = Math.Abs(k1 - 6);
				to8 = Math.Abs(k1 - 8);
				to12 = Math.Abs(k1 - 12);
				to24 = Math.Abs(k1 - 24);
				if (to6 <= to8)
				{
					k1 = 6;
				}
				else if (to8 <= to12)
				{
					k1 = 7;
				}
				else if (to12 <= to24)
				{
					k1 = 8;
				}
				else k1 = 9;

			}
			System.IO.StreamReader file = new System.IO.StreamReader("fisher.txt");
			while ((line = file.ReadLine()) != null)
			{
				string[] words = line.Split(' ');
				if(words[0] == "k1:")
				{
					continue;
				}
				else
				if (Convert.ToInt32(words[0]) == k2)
				{
					//for p=0.05
					fc = Convert.ToDouble(words[k1]);
					break;
				}
			}
			file.Close();
			return fc;
		}


		static bool Fisher(List<double> intern, List<double> ext)
		{
			double fc, table_fisher;
			int k1, k2;
			List<double> Disp_int = Dispersion(intern);
			List<double> Disp_ext = Dispersion(ext);
			double int_av = Average(Disp_int);
			double ext_av = Average(Disp_ext);
			double Smax, Smin;
			if (int_av > ext_av)
			{
				Smax = int_av;
				Smin = ext_av;
				k1 = intern.Count - 1;
				k2 = ext.Count - 1;
			}
			else
			{
				Smin = int_av;
				Smax = ext_av;
				k2 = intern.Count - 1;
				k1 = ext.Count - 1;
			}
			table_fisher = FindInTableFisher(k1, k2);
			fc = Smax / Smin;
			if (fc > table_fisher)
			{
				return false;
			}
			else
				return true;
		}

		static double EqualCenters (List<double> list, double a=0.001)
		{
			double D_ref = person.disp;
			List<double> D_cur = Dispersion(list);
			double M_ref = person.math_exp;
			List<double> M_cur = MathExpectation(list);
			double p = 0;
			int positive = 0;
			double t_table = FindInTableStudent(list.Count - 1, a);

			//for (int i=0; i<D_cur.Count;i++)
			//{
			//	D_cur[i] /= 1000;
			//	M_cur[i] /= 1000;
			//}
			//D_ref /= 1000;
			//M_ref /= 1000;

			double S = 0, t = 0;

			for (int i = 0; i < list.Count; i++)
			{
				 S = Math.Sqrt(((list.Count - 1) * (D_ref * D_ref + D_cur[i] * D_cur[i])) / (2 * list.Count - 1));

				 t = Math.Abs(M_ref - M_cur[i])/(S * Math.Sqrt(2) / Math.Sqrt(list.Count));
				
				if (t < t_table)
				{
					positive++;
				}
			}

			p = positive / list.Count;

			return p;
		}




		private void SignInButton_Click(object sender, RoutedEventArgs e)
		{
			if (userexists)
			{
				person = person.GetPerson(username);
				

				double internal_average = Average(internal_time_ls);
				double external_average = Average(external_time_ls);
				double math_exp = Average(MathExpectation(internal_time_ls));
				double disp = Average(Dispersion(internal_time_ls));
				double alpha = 0.05;
				
				if(a0001.IsPressed)
				{ alpha = 0.001; }
				if (a001.IsPressed)
				{ alpha = 0.01; }
				if (a005.IsPressed)
				{ alpha = 0.05; }


				if (Fisher(internal_time_ls, external_time_ls) && EqualCenters(internal_time_ls, alpha) >= 0.55)
				{
					MessageBox.Show("Success!");
					
					
				}
				else
				{
					MessageBox.Show("Failed to authenticate \n probability: " + EqualCenters(internal_time_ls));

				}

				string message = "";
				message +="	Reference: ";
				message += ("\n Internal time: " + person.internal_average);
				message += ("\n External time: " + person.external_average);
				message += ("\n Math expectation: " + person.math_exp);
				message += ("\n Dispersion: " + person.disp);
				message += ("\n");
				message += ("\n Your results: ");
				message += ("\n Internal time: " + internal_average /*+ "	delta = " + (internal_average - person.internal_average)*/);
				message += ("\n External time: " + external_average /*+ "	delta = " + (external_average - person.external_average)*/);
				message += ("\n Math expectation: " + math_exp /*+ "	delta = " + (math_exp - person.math_exp)*/);
				message += ("\n	Dispersion: " + disp /*+ "	delta = " + (disp - person.disp)*/);


				MessageBox.Show(message);


				internal_time_ls.Clear();
				external_time_ls.Clear();
				Password_textbox.Text = "";
				stopwatch_external.Stop();
				stopwatch_internal.Stop();

			}
		}

		private void Password_textbox_KeyDown(object sender, KeyEventArgs e)
		{
			//MessageBox.Show("Key Down");
			//Console.WriteLine("	Key  down");
				stopwatch_internal.Start();
				if (stopwatch_external.IsRunning)
				{
					stopwatch_external.Stop();
					external_ts = stopwatch_external.Elapsed;
					//double miliseconds = external_ts.TotalMilliseconds;
					external_time_ls.Add(external_ts.TotalMilliseconds);
				}
			
			if (e.Key==Key.Enter)
			{
				if (!userexists)
				{
					i++;
					if (!(CheckWithStudent(internal_time_ls).Count == internal_time_ls.Count && CheckWithStudent(external_time_ls).Count == external_time_ls.Count && i >= 10 && internal_time_ls.Count > 3 && external_time_ls.Count > 3))
					{
						Password_textbox.Text = "";
						TrainigAlert.Visibility = Visibility.Visible;
						TrainigAlert.Text = "Training in progress. Enter passfrase once more";
					}
					else
					{
						
					TrainigAlert.Text = "Training completed";
						trainingcomleted = true;
					}

				}
				else
				{
					TrainigAlert.Visibility = Visibility.Collapsed;
					SignInButton_Click(sender, e);
				}
			}
		}

		private void Password_textbox_KeyUp(object sender, KeyEventArgs e)
		{
			//	Console.WriteLine("	Key  up");
		//	MessageBox.Show("Key Up");
			
				stopwatch_external.Start();
				if (stopwatch_internal.IsRunning)
				{
					stopwatch_internal.Stop();
					internal_ts = stopwatch_internal.Elapsed;
					//double miliseconds = external_ts.TotalMilliseconds;
					internal_time_ls.Add(internal_ts.TotalMilliseconds);
				}
			
		}

		private void Login_textbox_LostFocus(object sender, RoutedEventArgs e)
		{
			username = Login_textbox.Text;
			if (!person.Exists(username))
			{
				NoUserAlert.Text = "No such user found";
				userexists = false;
			}
			else 
			{
				NoUserAlert.Text = "";
				userexists = true;
				person = person.GetPerson(username);
			}
		}

		private void Password_textbox_GotFocus(object sender, RoutedEventArgs e)
		{
			username = Login_textbox.Text;
			if (username == "")
			{
				NoUserAlert.Text = "No login";
			}
			else
			{
				NoUserAlert.Text = "";

			}
		}

		private void RegisterButton_Click(object sender, RoutedEventArgs e)
		{
			if (trainingcomleted && !userexists)
			{
				person.username = Login_textbox.Text;
				person.internal_average = Average(internal_time_ls);
				person.external_average = Average(external_time_ls);
				person.math_exp = Average(MathExpectation(internal_time_ls));
				person.disp = Average(Dispersion(internal_time_ls));




				person.AddToDB();

				string message = "Internal count: " + internal_time_ls.Count;
				message += "\n" + "Student internal count: " + CheckWithStudent(internal_time_ls).Count;
				message += "\n" + "Student external count: " + CheckWithStudent(external_time_ls).Count;
				message += "\n" + "External count: " + external_time_ls.Count;
				message += "\n\n" + "internal " + person.internal_average;
				message += "\n" + "external " + person.external_average;
				message += "\n" + "math exp " + person.math_exp;
				message += "\n" + "disp " + person.disp;

				MessageBox.Show(message);

			}
			internal_time_ls.Clear();
			external_time_ls.Clear();
			stopwatch_external.Stop();
			stopwatch_internal.Stop();
			Password_textbox.Text = "";
		}
	}
}
