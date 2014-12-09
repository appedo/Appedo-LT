using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using System.IO;
using System.Threading;
using AppedoLT.Core;
using System.Text;
using System.Diagnostics;

namespace AppedoLT.DataAccessLayer
{
    public class JmeterReportMaster
    {
        private string _reportName;
        private Constants _constants = Constants.GetInstance();
        private DateTime _minDate = new DateTime();
        private DateTime _maxDate = new DateTime();
        private string _reportFolder = string.Empty;
        private string _databasePath = string.Empty;
        public static Dictionary<string, ReportRuningStatus> Status = new Dictionary<string, ReportRuningStatus>();
        private string _loadGenName = string.Empty;


        public JmeterReportMaster(string reportName)
        {
            try
            {
                _reportName = reportName;
                _reportFolder = _constants.DataFolderPath + "\\" + reportName + "\\Report";
                _databasePath = _constants.DataFolderPath + "\\" + reportName;
                Status = new Dictionary<string, ReportRuningStatus>();

                #region database Lock problem

                try
                {
                    GetRunEndTime();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    Thread.Sleep(5000);
                }

                #endregion

                using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
                {
                    try
                    {
                        _con.Open();
                        SQLiteDataReader reader = GetReader("select min(starttime),max(endtime) from jmeterdata", _con);
                        if (reader.HasRows)
                        {
                            reader.Read();
                            _minDate = reader.GetDateTime(0);
                            _maxDate = reader.GetDateTime(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }

        }

        public void GenerateReports()
        {
            Status = new Dictionary<string, ReportRuningStatus>();
            SetUserRunTime();
            SetChartSummary();
        }
        public void SetUserRunTime()
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
            {
                try
                {
                    _con.Open();
                    new SQLiteCommand(@"delete from userruntime", _con).ExecuteNonQuery();
                    new SQLiteCommand(@"insert into userruntime
                                                               select scriptid,userid,min(starttime) as starttime,max(endtime) as endtime
                                                               from jmeterdata
                                                               group by 
                                                                scriptid,userid", _con).ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {

                    if (_con != null && _con.State == ConnectionState.Open)
                    {
                        _con.Close();
                    }
                }
            }

        }
        public DateTime GetRunEndTime()
        {
            DateTime endtime = new DateTime();
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    SQLiteDataReader reader = GetReader(@"SELECT max(endtime)as endtime from jmeterdata", _con);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        endtime = reader.GetDateTime(0);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {
                    if (_con != null && _con.State == ConnectionState.Open)
                    {
                        _con.Close();
                    }
                }
            }
            return endtime;
        }

        #region Charts
        public void SetUserAvgResponse()
        {
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ChartsAvgResponse, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("userid,avg");

                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                    userid,
                                                                    responsetime
                                                                  FROM 
                                                                     userresponsetime", _con);
                            reader.Read();
                            while (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1}", reader.GetValue(0).ToString() == string.Empty ? 0 : reader.GetDouble(0), reader.GetValue(1)));
                                reader.Read();
                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
        }
        public void SetChartSummary()
        {
            lock (Status)
            {
                if (Status.ContainsKey(_constants.ChartsSummaryFileName) == false)
                {
                    Status.Add(_constants.ChartsSummaryFileName, new ReportRuningStatus(_minDate, _maxDate));
                }
            }
            ReportRuningStatus status = Status[_constants.ChartsSummaryFileName];

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    DateTime from = new DateTime(_minDate.Ticks);
                    DateTime to = new DateTime(_maxDate.Ticks);
                    DateTime tempTo = new DateTime(from.Ticks).AddSeconds(4);
                    int sample = 0;
                    int sampleRate = 5;
                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ChartsSummaryFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {

                            if (_loadGenName == string.Empty) writer.WriteLine("loadgenname,scenariotime,usercount,hitcount,avgrequestresponse,avgthroughput,errorcount,avgpageresponse");
                            while (from.Ticks <= to.Ticks)
                            {
                                try
                                {

                                    sample = sample + sampleRate;
                                    if (sample > status.TotalSecounts) sample = Convert.ToInt32(status.TotalSecounts);
                                    SQLiteDataReader reader = GetReader(@"SELECT 
                                                                               (select count(userid) 
                                                                                 from userruntime
                                                                                 where '" + tempTo.ToString("yyyy-MM-dd HH:mm:ss") + @"' BETWEEN starttime and endtime) as usercount,
                                                                                count(address) AS hits,
                                                                                AVG(diff) AS avgrequestresponse,
                                                                                ((sum(responsesize)*8.0)/1048576)/5 AS avgthroughput,
                                                                                (select count(*) 
                                                                                  from jmeterdata
                                                                                  where 
                                                                                    success='FALSE' and (endtime BETWEEN '" + from.ToString("yyyy-MM-dd HH:mm:ss") + @"' AND '" + tempTo.ToString("yyyy-MM-dd HH:mm:ss") + @"')) as errorcount,
                                                                               (select SUM(diff)/count(distinct containername) from containerresponse where  (endtime BETWEEN '" + from.ToString("yyyy-MM-dd HH:mm:ss") + @"' AND '" + tempTo.ToString("yyyy-MM-dd HH:mm:ss") + @"') ) AS avgpageresponse
                                                                              FROM 
                                                                                jmeterdata
                                                                              WHERE 
                                                                               (endtime BETWEEN '" + from.ToString("yyyy-MM-dd HH:mm:ss") + @"' AND '" + tempTo.ToString("yyyy-MM-dd HH:mm:ss") + @"')", _con);

                                    reader.Read();
                                    if (reader.HasRows)
                                    {
                                        writer.WriteLine(string.Format("{0},{1},{2},{3},{4:0.000},{5:0.000},{6},{7:0.000}",
                                            "'" + _loadGenName + "'",
                                            TimeSpan.FromSeconds(sample),
                                            reader["usercount"],
                                            reader["hits"],
                                            reader["avgrequestresponse"] == DBNull.Value ? 0 : reader["avgrequestresponse"],
                                            reader["avgthroughput"] == DBNull.Value ? 0 : reader["avgthroughput"],
                                            reader["errorcount"],
                                            reader["avgpageresponse"] == DBNull.Value ? 0 : reader["avgpageresponse"]));
                                    }

                                    from = from.AddSeconds(5);
                                    tempTo = new DateTime(from.Ticks).AddSeconds(4);
                                    status.CompetedSecounts = sample;

                                }
                                catch (Exception ex)
                                {
                                    ExceptionHandler.WritetoEventLog(ex.Message);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
        }
        public DataTable GetLoadGenNames()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    SQLiteDataReader reader = GetReader(@"SELECT loadgenname from jmeterdata group by loadgenname", _con);
                    dt.Load(reader);

                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
            return dt;
        }
        #endregion

        #region Reports

        
       
        #endregion

        public DataTable GetJMeterScriptList()
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    SQLiteDataReader reader = GetReader(@"SELECT * from scriptlist", _con);
                    dt.Load(reader);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
            return dt;
        }
        public void Executequery(string newReportName, string query)
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
            {
                try
                {
                    _con.Open();
                    SQLiteCommand cmd = new SQLiteCommand(query, _con);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
                finally
                {
                    try
                    {
                        _con.Close();
                    }
                    catch
                    {
                    }
                }
            }

        }
        private SQLiteDataReader GetReader(string query, SQLiteConnection _con)
        {
            SQLiteCommand cmd = new SQLiteCommand(query, _con);
            return cmd.ExecuteReader();
        }
    }
}
