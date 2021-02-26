using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wpf_stk1
{
	class USER
	{
		public int id { get; set; }
		private string login, email, password, privileges, password_restrictions;

		public string Login
		{
			get { return login; }
			set { login = value; }
		}
		public string Email
		{
			get { return email; }
			set { email = value; }
		}
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		public string Privileges
		{
			get { return privileges; }
			set { privileges = value; }
		}

		public string Password_restrictions
		{
			get { return password_restrictions; }
			set { password_restrictions = value; }
		}


		public USER() {}

		public USER(string login, string email, string password)
		{
			this.login = login;
			this.email = email;
			this.password = password;
			this.privileges = "user";
			this.password_restrictions = "yes";
		}


	}
}
