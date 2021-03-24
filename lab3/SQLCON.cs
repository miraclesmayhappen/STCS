using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Data.SQLite;

namespace Wpf_stk1
{
    class SQLCON
    {
        //public DbSet<USER> USERS { get; set; }
        public static SQLiteConnection connection;

        private static SQLiteConnection CreateConnection(string dbname)
        {

            SQLiteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SQLiteConnection("Data Source=" + dbname + ";");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        public static void OpenConnection(/*SQLiteConnection conn*/)
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public static void CloseConnection(/*SQLiteConnection conn*/)
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public static USER FindUser(/*SQLiteConnection conn,*/ string log)
        {
            USER user = new USER();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = connection.CreateCommand();
            // sqlite_cmd.CommandText = "SELECT * FROM USERS WHERE login=='" + log+"'";
            string c = "SELECT * FROM USERS WHERE login=='" + log + "'";
            sqlite_cmd.CommandText = c;

            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.HasRows)
            {
                while (sqlite_datareader.Read())
                {
                    // Console.WriteLine(sqlite_datareader.GetString(1));
                    user.Login = sqlite_datareader.GetString(1);
                    user.Email = sqlite_datareader.GetString(2);
                    user.Password = sqlite_datareader.GetString(3);
                    user.Privileges = sqlite_datareader.GetString(4);
                    user.Password_restrictions = sqlite_datareader.GetString(5);
                }

                // return true;
            }
            sqlite_datareader.Close();
            return user;


        }

        public static void UpdateTable(/*SQLiteConnection conn,*/ USER user)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText =
                    "update USERS set password = :password, privileges = :privileges, password_restrictions = :password_restrictions where login=:login";
            sqlite_cmd.Parameters.Add("password", System.Data.DbType.String).Value = user.Password;
            sqlite_cmd.Parameters.Add("privileges", System.Data.DbType.String).Value = user.Privileges;
            sqlite_cmd.Parameters.Add("password_restrictions", System.Data.DbType.String).Value = user.Password_restrictions;
            sqlite_cmd.Parameters.Add("login", System.Data.DbType.String).Value = user.Login;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static void NewUser(/*SQLiteConnection conn,*/ USER user)
        {
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText =
                    "INSERT INTO USERS (login, email, password, privileges, password_restrictions) VALUES(:login, :email, :password, :privileges, :password_restrictions); ";
            sqlite_cmd.Parameters.Add("login", System.Data.DbType.String).Value = user.Login;
            sqlite_cmd.Parameters.Add("email", System.Data.DbType.String).Value = user.Email;
            sqlite_cmd.Parameters.Add("password", System.Data.DbType.String).Value = user.Password;
            sqlite_cmd.Parameters.Add("privileges", System.Data.DbType.String).Value = user.Privileges;
            sqlite_cmd.Parameters.Add("password_restrictions", System.Data.DbType.String).Value = user.Password_restrictions;
            sqlite_cmd.ExecuteNonQuery();
        }

        public static List<USER> GetList()
        {
            
            List<USER> list = new List<USER>();
            SQLiteDataReader sqlite_datareader;
            SQLiteCommand sqlite_cmd;
            sqlite_cmd = connection.CreateCommand();
            // sqlite_cmd.CommandText = "SELECT * FROM USERS WHERE login=='" + log+"'";
            string c = "SELECT * FROM USERS";
            sqlite_cmd.CommandText = c;

            sqlite_datareader = sqlite_cmd.ExecuteReader();

            if (sqlite_datareader.HasRows)
            {
                while (sqlite_datareader.Read())
                {
                    // Console.WriteLine(sqlite_datareader.GetString(1));
                    USER user = new USER();
                    user.Login = sqlite_datareader.GetString(1);
                    user.Email = sqlite_datareader.GetString(2);
                    user.Password = sqlite_datareader.GetString(3);
                    user.Privileges = sqlite_datareader.GetString(4);
                    user.Password_restrictions = sqlite_datareader.GetString(5);
                    list.Add(user);
                }

                // return true;
            }
            sqlite_datareader.Close();
            return list;


        }

        public SQLCON(string dbname)
        {
            connection = CreateConnection(dbname);
        }

    }
}
