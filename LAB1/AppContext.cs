using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Wpf_stk1
{
	class AppContext : DbContext
	{

		public DbSet<USER> USERS { get; set; }

		public AppContext() : base("DefaultConnection") { }



	}
}
