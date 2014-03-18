using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;

namespace miniORM
{
    /// <summary>
    /// 实体基类
    /// </summary>
    public abstract class BaseEntity
    {
        /// <summary>
        /// 实现将数据库字段映射到对象中字段
        /// </summary>
        /// <param name="reader"></param>
        internal abstract void SetEntity(IDataReader reader, Dictionary<string, int> lowerFields);

        /// <summary>
        /// 获取数据库字段数据
        /// </summary>
        /// <typeparam name="ObjectType"></typeparam>
        /// <param name="reader"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        internal ObjectType GetField<ObjectType>(IDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return default(ObjectType);
            object obj = reader.GetValue(index);
            Type dtype = typeof(ObjectType);
            if (!dtype.IsGenericType)
                return (ObjectType)Convert.ChangeType(obj, dtype);
            if (dtype.GetGenericTypeDefinition() == typeof(Nullable<>))
                return (ObjectType)Convert.ChangeType(obj, Nullable.GetUnderlyingType(dtype));
            throw new InvalidCastException(string.Format("Invalid cast from type \"{0}\" to type \"{1}\".", obj.GetType().FullName, dtype.FullName));
        }
    }
    /// <summary>
    /// 实体映射帮助类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityHelper<T>
    where T : BaseEntity, new()
    {
        public static List<T> SetValues(IDataReader reader)
        {
            try
            {
                List<T> list = new List<T>();
                Dictionary<string, int> lowerFields = new Dictionary<string, int>();
                bool first = true;
                while (reader.Read())
                {
                    if (first)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lowerFields.Add(reader.GetName(i).ToLower(), i);
                        }
                        first = false;
                    }
                    T item = new T();
                    item.SetEntity(reader, lowerFields);
                    list.Add(item);
                }
                return list;
            }
            finally
            {
                reader.Close();
            }
        }
        public static T SetValue(IDataReader reader)
        {
            try
            {
                Dictionary<string, int> lowerFieldIndex = new Dictionary<string, int>();
                bool first = true;
                if (reader.Read())
                {
                    if (first)
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            lowerFieldIndex.Add(reader.GetName(i).Trim().ToLower(), i);
                        }
                        first = false;
                    }
                    T item = new T();
                    item.SetEntity(reader, lowerFieldIndex);
                    return item;
                }
            }
            finally
            {
                reader.Close();
            }
            return null;
        }
    }
    /// <summary>
    /// reader映射帮助类
    /// </summary>
    public class EntityHelper
    {
        public static T To<T>(object reader) where T : IConvertible
        {
            if (reader == DBNull.Value)
            {
                return default(T);
            }
            string type = typeof(T).Name;
            TypeCode typecode;
            if (!Enum.TryParse(type, out typecode))
                throw new ArgumentException("could not convert!");
            return (T)Convert.ChangeType(reader, typecode);
        }
    }
    /// <summary>
    /// 数据库操作基类
    /// </summary>
    public abstract class DBHelper : IDisposable
    {
        private IDbConnection dbconnection;
        /// <summary>
        /// 实现构造函数
        /// </summary>
        /// <param name="conn">必须是集成自ConnString的类</param>
        public DBHelper(string connstring)
        {
            dbconnection = CreateConnection(connstring);
        }
        ~DBHelper()
        {
            if (dbconnection != null)
                dbconnection.Close();
        }
        public void Dispose()
        {
            if (dbconnection != null)
            {
                dbconnection.Close();
            }
        }
        protected abstract IDbConnection CreateConnection(string connstring);
        protected abstract IDbDataParameter CreateParameter();
        protected abstract IDbDataAdapter CreateDataAdapter();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public void ExecuteNonQuery(string cmdText, params IDbDataParameter[] commandParameters)
        {
            ExecuteNonQuery(CommandType.Text, cmdText, commandParameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public void ExecuteNonQuery(CommandType cmdType, string cmdText, params IDbDataParameter[] commandParameters)
        {
            try
            {
                using (IDbCommand _dbcommand = dbconnection.CreateCommand())
                {
                    PrepareCommand(_dbcommand, null, cmdType, cmdText, commandParameters);
                    _dbcommand.ExecuteNonQuery();
                    _dbcommand.Parameters.Clear();
                }
            }
            finally
            {

                dbconnection.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string cmdText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteReader(CommandType.Text, cmdText, commandParameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(CommandType cmdType, string cmdText, params IDbDataParameter[] commandParameters)
        {
            using (IDbCommand _dbcommand = dbconnection.CreateCommand())
            {
                PrepareCommand(_dbcommand, null, cmdType, cmdText, commandParameters);
                IDataReader rdr = _dbcommand.ExecuteReader(CommandBehavior.CloseConnection);
                _dbcommand.Parameters.Clear();
                return rdr;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(string cmdText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, cmdText, commandParameters);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public object ExecuteScalar(CommandType cmdType, string cmdText, params IDbDataParameter[] commandParameters)
        {
            try
            {
                using (IDbCommand _dbcommand = dbconnection.CreateCommand())
                {
                    PrepareCommand(_dbcommand, null, cmdType, cmdText, commandParameters);
                    object obj = _dbcommand.ExecuteScalar();
                    _dbcommand.Parameters.Clear();
                    return obj;
                }
            }
            finally
            {
                dbconnection.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdType"></param>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable ExecuteTable(CommandType cmdType, string cmdText, params IDbDataParameter[] commandParameters)
        {
            try
            {
                using (IDbCommand _dbcommand = dbconnection.CreateCommand())
                {
                    PrepareCommand(_dbcommand, null, cmdType, cmdText, commandParameters);
                    using (DataSet st = new DataSet())
                    {
                        IDbDataAdapter ap = CreateDataAdapter();
                        ap.SelectCommand = _dbcommand;
                        ap.Fill(st);
                        _dbcommand.Parameters.Clear();
                        return st.Tables[0];
                    }
                }
            }
            finally
            {
                dbconnection.Close();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cmdText"></param>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        public DataTable ExecuteTable(string cmdText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteTable(CommandType.Text, cmdText, commandParameters);
        }

        public DataSet ExecuteDataSet(string cmdText, params IDbDataParameter[] commandParameters)
        {
            return ExecuteDataSet(CommandType.Text, cmdText, commandParameters);
        }
        public DataSet ExecuteDataSet(CommandType cmdType, string cmdText, params IDbDataParameter[] commandParameters)
        {
            try
            {
                using (IDbCommand _dbcommand = dbconnection.CreateCommand())
                {
                    PrepareCommand(_dbcommand, null, cmdType, cmdText, commandParameters);
                    using (DataSet st = new DataSet())
                    {
                        IDbDataAdapter ap = CreateDataAdapter();
                        ap.SelectCommand = _dbcommand;
                        ap.Fill(st);
                        _dbcommand.Parameters.Clear();
                        return st;
                    }
                }
            }
            finally
            {
                dbconnection.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string parameterName)
        {
            IDbDataParameter p = CreateParameter();
            p.ParameterName = parameterName;
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string parameterName, object value)
        {
            IDbDataParameter p = CreateParameter();
            p.ParameterName = parameterName;
            p.Value = value ?? (object)DBNull.Value;
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="size"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string parameterName, DbType dbType, int size, object value)
        {
            IDbDataParameter p = CreateParameter();
            p.ParameterName = parameterName;
            p.DbType = dbType;
            p.Size = size;
            p.Value = value ?? (object)DBNull.Value;
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="dbType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IDbDataParameter CreateParameter(string parameterName, DbType dbType, object value)
        {
            IDbDataParameter p = CreateParameter();
            p.ParameterName = parameterName;
            p.DbType = dbType;
            p.Value = value ?? (object)DBNull.Value;
            return p;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        public void SetParameter(IDbDataParameter parameter, object value)
        {
            parameter.Value = value ?? (object)DBNull.Value;
        }
        /// <summary>
        ///  Prepare a command for execution
        /// </summary>
        /// <param name="cmd">IDbCommand Object</param>
        /// <param name="trans">IDbTransaction Object</param>
        /// <param name="cmdType">Cmd type e.g. stored procedure or text</param>
        /// <param name="cmdText">Command text, e.g. Select * from Products</param>
        /// <param name="cmdParms">IDbDataParameters to use in the command</param>
        private static void PrepareCommand(IDbCommand cmd, IDbTransaction trans, CommandType cmdType, string cmdText, IDbDataParameter[] cmdParms)
        {
            if (cmd.Connection.State != ConnectionState.Open)
            {
                cmd.Connection.Open();
            }
            cmd.CommandText = cmdText;
            if (trans != null)
            {
                cmd.Transaction = trans;
            }
            cmd.CommandType = cmdType;

            if (cmdParms != null && cmdParms.Length > 0)
            {
                foreach (IDbDataParameter parm in cmdParms)
                {
                    cmd.Parameters.Add(parm);
                }
            }
        }
    }
    /// <summary>
    /// mssql数据操作接口类
    /// </summary>
    public class MSSQLHelper : DBHelper
    {
        public MSSQLHelper(string connstring)
            : base(connstring)
        {

        }
        protected override IDbConnection CreateConnection(string connstring)
        {
            return new SqlConnection(connstring);
        }
        protected override IDbDataParameter CreateParameter()
        {
            return new SqlParameter();
        }

        protected override IDbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter();
        }
    }
    /// <summary>
    /// mysql数据操作接口类
    /// </summary>
    public class MYSQLHelper : DBHelper
    {
        public MYSQLHelper(string connstring)
            : base(connstring)
        {

        }
        protected override IDbConnection CreateConnection(string connstring)
        {
            return new MySqlConnection(connstring);
        }

        protected override IDbDataParameter CreateParameter()
        {
            return new MySqlParameter();
        }

        protected override IDbDataAdapter CreateDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
}
