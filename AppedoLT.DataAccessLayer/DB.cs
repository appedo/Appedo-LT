using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using System.Data;

namespace AppedoLT.DataAccessLayer
{
   public class DB
    {
        private static DB _instance;
        private static object _lockObj = new object();
        private string _connectionString = string.Empty;
        private IDbConnection GetConnection()
        {
            return new NpgsqlConnection(_connectionString);
        }

        public static DB Connector
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {

                            _instance = new DB();
                            _instance.ConnectionString = System.Configuration.ConfigurationManager.AppSettings["databasesetting"];

                        }
                    }
                }
                return _instance;
            }
            private set { }
        }
        public string ConnectionString { get { return _connectionString; } set { _connectionString = value; } }
        public IDbCommand GetCommand(string query)
        {
            IDbConnection con = GetConnection();
            IDbCommand cmd = con.CreateCommand();
            cmd.CommandText = query;
            return cmd;
        }
        public Int32 ExecuteNonQuery(string query, Dictionary<string, object> parameters = null)
        {
            IDbCommand cmd = GetCommand(query);
            int result = 0;
            try
            {
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        IDbDataParameter param = cmd.CreateParameter();
                        param.ParameterName = key;
                        param.Value = key;
                        cmd.Parameters.Add(param);

                    }
                }
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                cmd = null;
            }
            return result;
        }
        public DataTable ExecuteReader(string query, Dictionary<string, object> parameters = null)
        {
            IDbCommand cmd = GetCommand(query);
            DataTable data = new DataTable();
            IDataReader reader = null;

            try
            {
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        IDbDataParameter param = cmd.CreateParameter();
                        param.ParameterName = key;
                        param.Value = parameters[key];
                        cmd.Parameters.Add(param);

                    }
                }
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                data.Load(reader);
                cmd.Connection.Close();
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                cmd = null;
                reader = null;
            }
            return data;
        }
        public object ExecuteScalar(string query, Dictionary<string, object> parameters = null)
        {
            IDbCommand cmd = GetCommand(query);
            object data = new object();
            try
            {
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        IDbDataParameter param = cmd.CreateParameter();
                        param.ParameterName = key;
                        param.Value = parameters[key];
                        cmd.Parameters.Add(param);

                    }
                }
                cmd.Connection.Open();
                data = cmd.ExecuteScalar();

                cmd.Connection.Close();
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                cmd = null;

            }
            return data;
        }
        public DataTable ExecuteReader(List<string> coloums, string tableName, string where = null, Dictionary<string, object> parameters = null)
        {
            StringBuilder query = new StringBuilder();
            query.Append("select ");
            if (coloums.Count == 0)
            {
                query.Append("*");

            }
            else
            {
                foreach (string key in coloums)
                {
                    query.Append(key).Append(",");

                }
                query.Remove(query.Length - 1, 1);
            }
            query.Append(" from ").Append(tableName);
            if (where != null && where.Trim() != string.Empty)
            {
                query.Append(" Where ").Append(where);
            }
            IDbCommand cmd = GetCommand(query.ToString());
            DataTable data = new DataTable();
            IDataReader reader = null;

            try
            {
                if (parameters != null)
                {
                    foreach (string key in parameters.Keys)
                    {
                        IDbDataParameter param = cmd.CreateParameter();
                        param.ParameterName = key;
                        param.Value = parameters[key];
                        cmd.Parameters.Add(param);

                    }
                }
                cmd.Connection.Open();
                reader = cmd.ExecuteReader();
                data.Load(reader);
                cmd.Connection.Close();
            }
            finally
            {
                if (cmd.Connection.State == ConnectionState.Open) cmd.Connection.Close();
                cmd = null;
                reader = null;
                query = null;
            }
            return data;
        }

    }
}
