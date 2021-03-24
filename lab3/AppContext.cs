using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;

namespace Wpf_stk1
{
	class AppContext : DbContext
	{

		public DbSet<USER> USERS { get; set; }

		public AppContext() : base("DefaultConnection") { }

		//public AppContext(string dbname)
		//{

		//	SQLiteConnection sqlcon = new SQLiteConnection("Data Source=" + dbname);
		//}



	}
}
