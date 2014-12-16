using System;
using System.Configuration;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Data.SQLite;

using AppedoLT.Core;
namespace AppedoLT.DataAccessLayer
{
    /// <summary>
    /// 
    /// </summary>
    public class SQLiteMasterAccess
    {
        #region Variable Declarations

        public static SQLiteConnection con = new SQLiteConnection(System.Configuration.ConfigurationManager.AppSettings["SQLiteConnectionString"]);
        protected internal SQLiteCommand com;
        protected internal SQLiteDataReader rd;
        protected internal SQLiteDataAdapter adp;
        private static SQLiteMasterAccess instance;
        public string _ConnectionString = System.Configuration.ConfigurationManager.AppSettings["SQLiteConnectionString"];
        #endregion

        #region Static

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static SQLiteMasterAccess GetInstance()
        {
            if (instance == null)
            {
                instance = new SQLiteMasterAccess();
            }
            return instance;
        }

        #endregion

        #region Private

        private string ConnectionString
        {
            set { _ConnectionString = value; }
            get { return _ConnectionString; }
        }

        /// <summary>
        /// 
        /// </summary>
        private SQLiteMasterAccess()
        {
        }

        /// <summary>
        /// Method to dispose SQLite Objects
        /// </summary>
        private void DisposeSQLiteObjects()
        {
            if (con != null)
                con.Dispose();
            if (com != null)
                com.Dispose();
            if (rd != null)
                rd.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StrConnectionString"></param>
        /// <returns></returns>
        private SQLiteConnection GetConnection(string StrConnectionString)
        {
            con = new SQLiteConnection(StrConnectionString);
            return con;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strQuery"></param>
        /// <param name="conConnectionnew"></param>
        /// <returns></returns>
        private SQLiteCommand GetCommand(string strQuery, SQLiteConnection conConnectionnew)
        {
            com = new SQLiteCommand();
            try
            {
                com.CommandText = strQuery;
                com.CommandType = CommandType.Text;
                com.Connection = conConnectionnew;
                if (conConnectionnew.State.ToString() != ConnectionState.Open.ToString())
                {
                    conConnectionnew.Open();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
            }
            return com;
        }
        #endregion

        #region Public

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public DataTable ExecuteReader(string query)
        {
            try
            {
                DataTable resultTable = new DataTable();
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, SQLiteMasterAccess.con);
                da.Fill(resultTable);

                //com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                //resultTable.Load(com.ExecuteReader());
                return resultTable;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            int result = 0;
            try
            {
                if (SQLiteMasterAccess.con.State == ConnectionState.Closed) SQLiteMasterAccess.con.Open();
                cmd.Connection = SQLiteMasterAccess.con;
                cmd.CommandText = query;
                cmd.CommandType = CommandType.Text;
                result=cmd.ExecuteNonQuery();
                 
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message + Environment.NewLine + query);
               
            }
           
            return result;
        }
        public int ExecuteNonQueryForFile(string query)
        {
            SQLiteCommand cmd = new SQLiteCommand();
            try
            {
                if (SQLiteMasterAccess.con.State == ConnectionState.Closed) SQLiteMasterAccess.con.Open();
                cmd.Connection = SQLiteMasterAccess.con;
                cmd.CommandText = query.Replace("\\", "/");
                cmd.CommandType = CommandType.Text;
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                cmd.Dispose();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public DataSet ReturnDataset(string query)
        {
            DataSet ds = new DataSet();
            try
            {
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                adp = new SQLiteDataAdapter();
                adp.SelectCommand = com;
                adp.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public object ExecuteScalar(string query)
        {
            try
            {
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                return com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public DataTable ExecuteReader(string query, Dictionary<string, object> param)
        {
            try
            {
                DataTable resultTable = new DataTable();
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                foreach (KeyValuePair<string, object> cmdParam in param)
                {
                    com.Parameters.AddWithValue(cmdParam.Key.ToString(), cmdParam.Value);
                }
                resultTable.Load(com.ExecuteReader());
                return resultTable;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        /// 

      
        public int ExecuteNonQuery(string query, List<Dictionary<string, object>> paramList)
        {
            try
            {
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                foreach (Dictionary<string, object> param in paramList)
                {
                    foreach (KeyValuePair<string, object> cmdParam in param)
                    {
                        com.Parameters.AddWithValue(cmdParam.Key.ToString(), cmdParam.Value);
                    }

                    com.ExecuteNonQuery();
                    com.Parameters.Clear();
                }
                return 1;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public DataSet ReturnDataset(string query, Dictionary<string, object> param)
        {
            DataSet ds = new DataSet();
            try
            {
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                foreach (KeyValuePair<string, object> cmdParam in param)
                {
                    com.Parameters.AddWithValue(cmdParam.Key.ToString(), cmdParam.Value);
                }
                adp = new SQLiteDataAdapter();
                adp.SelectCommand = com;
                adp.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public object ExecuteScalar(string query, Dictionary<string, object> param)
        {
            try
            {
                com = this.GetCommand(query, this.GetConnection(this.ConnectionString));
                foreach (KeyValuePair<string, object> cmdParam in param)
                {
                    com.Parameters.AddWithValue(cmdParam.Key.ToString(), cmdParam.Value.ToString());
                }
                return com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
            finally
            {
                this.DisposeSQLiteObjects();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ComCommand"></param>
        /// <returns></returns>
        public SQLiteDataReader ExecuteReaderForBlob(SQLiteCommand ComCommand)
        {
            try
            {
                return ComCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.Message);
                throw;
            }
        }
        #endregion Public
    }
}
