using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using Microsoft.Data.SqlClient;

namespace DBL
{
    public enum DbTypes
    {
        MSAccess,
        MySql,
        MSSql
    }
    public abstract class DB
    {
        //TODO these params to be read from some configuration file or env variable...
        protected const string DbName = "mystore";

        private const string mySqlConnSTR = @$"server=localhost;
                                    user id=root;
                                    password=1234;
                                    persistsecurityinfo=True;
                                    database={DbName}";
        
        protected const string accessConnSTR = $@"Provider=Microsoft.ACE.OLEDB.12.0;
                                    Data Source=D:\personal\school\{DbName}.accdb;
                                    Persist Security Info=False;";

        protected const string msqlConnStr = $@"Server=(localdb)\\mssqllocaldb;
                                    AttachDBFilename=D:\personal\school\{DbName}.mdf;
                                    Initial Catalog={DbName};
                                    Trusted_Connection=True;MultipleActiveResultSets=true";
        
        private const DbTypes dbType = DbTypes.MSAccess;

        //TODO - currently we support only one connection
        protected static DbConnection conn;
        protected readonly DbCommand cmd;
        protected DbDataReader reader;
        protected DB()
        {
            //TODO this to be done using Factory
            switch (dbType)
            {
                case DbTypes.MSAccess:
                    conn = new OleDbConnection(accessConnSTR);
                    cmd = new OleDbCommand();
                    break;
                case DbTypes.MySql:
                    conn = new MySqlConnection(mySqlConnSTR);
                    cmd = new MySqlCommand();
                    break;
                case DbTypes.MSSql:
                    conn = new SqlConnection(msqlConnStr);
                    cmd = new SqlCommand();
                    break;
                default:
                    throw new ArgumentException("Unsupported Db Type: ", dbType.ToString());
            }
            
            cmd.Connection = conn;
            reader = null;
        }
    }
}