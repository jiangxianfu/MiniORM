    //新版本的DBHelper
    public abstract class Database
    {
        private string connString = "";
        protected string ConnectionString
        {
            get
            {
                return connString;
            }
        }
        public Database(string connectionString)
        {
            connString = connectionString;
        }

        protected abstract DbConnection CreateConnection();
        public abstract DbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 注意一定要写在using代码块中,因为我直接就open了一个连接
        /// </summary>
        /// <returns></returns>
        public DbConnection GetConnection()
        {
            DbConnection connection = CreateConnection();
            connection.Open();
            return connection;
        }
        public DbCommand GetCommand(DbConnection connection, CommandType commandType)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandType = commandType;
            return command;
        }
        public DbCommand GetSqlCommand(DbConnection connection, string sql)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = sql;
            command.CommandType = CommandType.Text;
            return command;
        }
        public DbCommand GetStoredProcCommand(DbConnection connection, string storedProcedureName)
        {
            DbCommand command = connection.CreateCommand();
            command.CommandText = storedProcedureName;
            command.CommandType = CommandType.StoredProcedure;
            return command;
        }

        public void AddParameter(DbCommand command, string name)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            command.Parameters.Add(param);
        }
        public void AddParameter(DbCommand command, string name, DbType dbType)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            command.Parameters.Add(param);
        }
        public void AddParameter(DbCommand command, string name, object value)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? (object)DBNull.Value;
            command.Parameters.Add(param);
        }

        public void AddParameter(DbCommand command, string name, DbType dbType, object value)
        {
            DbParameter param = command.CreateParameter();
            param.ParameterName = name;
            param.DbType = dbType;
            param.Value = value ?? (object)DBNull.Value;
            command.Parameters.Add(param);
        }


        public DataTable ExecuteTable(DbCommand cmd)
        {
            using (DataSet st = new DataSet())
            {
                DbDataAdapter ap = CreateDataAdapter();
                ap.SelectCommand = cmd;
                ap.Fill(st);
                return st.Tables[0];
            }
        }
        public DataSet ExecuteSet(DbCommand cmd)
        {
            using (DataSet st = new DataSet())
            {
                DbDataAdapter ap = CreateDataAdapter();
                ap.SelectCommand = cmd;
                ap.Fill(st);
                return st;
            }
        }
    }
    public class MYSQLDataBase : Database
    {
        public MYSQLDataBase(string conn)
            : base(conn)
        {

        }
        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection(base.ConnectionString);
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
    public class MSSQLDataBase : Database
    {
        public MSSQLDataBase(string conn)
            : base(conn)
        {

        }

        protected override DbConnection CreateConnection()
        {
            return new MySqlConnection(base.ConnectionString);
        }

        public override DbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
