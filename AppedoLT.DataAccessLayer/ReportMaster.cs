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
    public class ReportMaster
    {
        private string _reportName;
        private Constants _constants = Constants.GetInstance();
        private DateTime _minDate = new DateTime();
        private DateTime _maxDate = new DateTime();
        private string _reportFolder = string.Empty;
        private string _databasePath = string.Empty;
        public static Dictionary<string, ReportRuningStatus> Status = new Dictionary<string, ReportRuningStatus>();
        private string _loadGenName = string.Empty;


        public ReportMaster(string reportName)
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
                 //   ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                    Thread.Sleep(5000);
                }

                #endregion

                using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
                {
                    try
                    {
                        _con.Open();
                        SQLiteDataReader reader = GetReader("select min(starttime),max(endtime) from reportdata", _con);
                       
                        if (reader.HasRows)
                        {
                            reader.Read();
                            _minDate = reader.GetDateTime(0);
                            _maxDate = reader.GetDateTime(1);
                        }
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
            }

        }
        public ReportMaster(string reportName, DateTime ScenarioStartTime, string loagGenName)
        {
            try
            {
                _reportName = reportName;
                _loadGenName = loagGenName;
                _reportFolder = _constants.DataFolderPath + "\\" + reportName + "\\Report";
                _databasePath = _constants.DataFolderPath + "\\" + reportName;
                Status = new Dictionary<string, ReportRuningStatus>();
                _minDate = ScenarioStartTime;
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
                        SQLiteDataReader reader = GetReader("select max(endtime) from reportdata", _con);
                        if (reader.HasRows)
                        {
                            reader.Read();
                            _maxDate = reader.GetDateTime(0);
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
            //Status = new Dictionary<string, ReportRuningStatus>();
           // new Thread(() =>
         //   {
                
                SetChartSummary();
           // }).Start();
            //new Thread(() => { SetReportSummary(); }).Start();
            //new Thread(() => { SetUserAvgResponse(); }).Start();
            //new Thread(() => { SetRequestSummaryReport(); }).Start();
            //new Thread(() => { SetPageSummaryReport(); }).Start();
            //new Thread(() => { SetContainerSummaryReport(); }).Start();
            //new Thread(() => { SetTransactionSummaryReport(); }).Start();
        }
       
        public DateTime GetRunEndTime()
        {
            DateTime endtime = new DateTime();
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    SQLiteDataReader reader = GetReader(@"SELECT max(endtime)as endtime from reportdata", _con);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        endtime = reader.GetDateTime(0);
                    }
                }
                catch (Exception ex)
                {
                   // ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
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
                                                               from reportdata
                                                               group by 
                                                                scriptid,userid", _con).ExecuteNonQuery();

                    new SQLiteCommand(@"CREATE INDEX idx_userruntime ON userruntime (endtime);
                                        CREATE INDEX idx_endtime  ON reportdata(endtime );
                                        CREATE INDEX idx_time ON error(time );
                                        CREATE INDEX idx_starttime  ON userruntime(starttime );", _con).ExecuteNonQuery();

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
                                                                                count(requestid) AS hits,
                                                                                AVG(diff) AS avgrequestresponse,
                                                                                ((sum(responsesize)*8.0)/1048576)/5 AS avgthroughput,
                                                                                (select count(*) 
                                                                                  from error
                                                                                  where 
                                                                                   (time BETWEEN '" + from.ToString("yyyy-MM-dd HH:mm:ss") + @"' AND '" + tempTo.ToString("yyyy-MM-dd HH:mm:ss") + @"')) as errorcount,
                                                                                SUM(diff)/count(distinct pageid) AS avgpageresponse
                                                                              FROM 
                                                                                reportdata
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
                    SQLiteDataReader reader = GetReader(@"SELECT loadgenname from reportdata group by loadgenname", _con);
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

        public void SetReportSummary()
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ReportSummayReportFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("reportname,start_time,end_time,duration_sec,user_count,total_hits,avg_response,avg_hits,total_throughput,avg_throughput,total_errors,total_page,avg_page_response,reponse_200,reponse_300,reponse_400,reponse_500");
                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                   MIN(starttime) AS start_time,
                                                                   MAX(endtime) AS end_time,
                                                                   (strftime('%s',max(starttime)) - strftime('%s',min(starttime))) as duration_sec,
                                                                   (SELECT COUNT(*) AS user_count FROM (SELECT userid,scriptid FROM reportdata  GROUP BY userid,scriptid) AS a) AS user_count,
                                                                   count(*) AS total_hits,
                                                                   round(IFNULL(AVG(diff),0),3) AS avg_response,
                                                                   IFNULL(ROUND(SUM(responsesize),3),0) AS total_throughput,
                                                                   IFNULL(SUM(responsesize),0) AS avg_throughput ,
                                                                   (select count(*)from error ) AS total_errors,
                                                                   (select count(*) from (SELECT pageid from reportdata group by userid,iterationid,pageid))as total_page,
                                                                   (select IFNULL(AVG(pageresponse),0) from (select sum(diff)as pageresponse from reportdata  group by userid,iterationid,pageid))AS avg_page_response,
                                                                   (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=200 AND reponsecode<300 ) AS reponse_200,
                                                                   (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=300 AND reponsecode<400 ) AS reponse_300,
                                                                   (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=400 AND reponsecode<500 ) AS reponse_400,
                                                                   (SELECT COUNT(reponsecode) FROM reportdata WHERE reponsecode>=500 AND reponsecode<600 ) AS reponse_500
                                                                  FROM 
                                                                     reportdata", _con);



                            reader.Read();
                            if (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}", _reportName, reader["start_time"], reader["end_time"], reader["duration_sec"], reader["user_count"], reader["total_hits"],
                                    reader["avg_response"], Convert.ToDouble(reader["duration_sec"]) > 0 ? Convert.ToDouble(reader["total_hits"]) / Convert.ToDouble(reader["duration_sec"]) : 0, reader["total_throughput"], reader["avg_throughput"], reader["total_errors"], reader["total_page"], reader["avg_page_response"],
                                                                                           reader["reponse_200"], reader["reponse_300"], reader["reponse_400"], reader["reponse_500"]));
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
        public void SetRequestSummaryReport()
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ReportRequestSummayReportFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("Address,Min,Max,Avg");
                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                   address,
                                                                   ROUND(MIN(responsetime),3) AS min,
                                                                   ROUND(MAX(responsetime),3) AS max,
                                                                   ROUND(avg(responsetime),3) AS avg
                                                                  FROM 
                                                                     requestresponsetime
                                                                  group by
                                                                      requestid 
                                                                   ", _con);

                            reader.Read();
                            while (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1},{2},{3}", reader["address"], reader["min"], reader["max"], reader["avg"]));
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
        public void SetPageSummaryReport()
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ReportPageSummayReportFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("Pagename,Min,Max,Avg");
                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                   pagename,
                                                                   ROUND(min(responsetime),3) AS min,
                                                                   ROUND(max(responsetime),3) AS max,
                                                                   ROUND(avg(responsetime),3) AS avg
                                                                  FROM 
                                                                     pageresponsetime
                                                                 group by
                                                                     pageid
                                                                   ", _con);



                            reader.Read();

                            while (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1},{2},{3}", reader["pagename"], reader["min"], reader["max"], reader["avg"]));
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
        public void SetContainerSummaryReport()
        {
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ReportContainerSummayReportFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("Containername,Min,Max,Avg");
                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                   containername,
                                                                   Round(min(responsetime),3) AS min,
                                                                   Round(max(responsetime),3) AS max,
                                                                   Round(avg(responsetime),3) AS avg
                                                                  FROM 
                                                                     containerresponsetime
                                                                 group by
                                                                     containerid
                                                                   ", _con);

                            reader.Read();
                            while (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1},{2},{3}", reader["containername"], reader["min"], reader["max"], reader["avg"]));
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
        public void SetTransactionSummaryReport()
        {

            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();

                    using (FileStream stream = new FileStream(_reportFolder + "\\" + _constants.ReportTransactionSummayReportFileName, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        using (StreamWriter writer = new StreamWriter(stream))
                        {
                            writer.WriteLine("transactionname,Min,Max,Avg");
                            SQLiteDataReader reader = GetReader(@"SELECT 
                                                                   transactionname,
                                                                   round(min(responsetime),3) AS min,
                                                                   round(max(responsetime),3) AS max,
                                                                   round(avg(responsetime),3) AS avg
                                                                  FROM 
                                                                     transactionresponsetime
                                                                 group by
                                                                     transactionname
                                                                   ", _con);



                            reader.Read();
                            while (reader.HasRows)
                            {
                                writer.WriteLine(string.Format("{0},{1},{2},{3}", reader["transactionname"], reader["min"], reader["max"], reader["avg"]));
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

        #endregion

        public DataTable GetReportData(string reportName)
        {
            DataTable dt = new DataTable();
            using (SQLiteConnection _con = new SQLiteConnection("Data Source=" + _databasePath + "\\database.db;Version=3;New=True;Compress=True;Read Only=True;"))
            {
                try
                {
                    _con.Open();
                    //SQLiteDataReader reader = GetReader(@"SELECT * from reportdata", _con);
                   SQLiteDataReader reader = GetReader(@"SELECT loadgenname as LoadgenName,scenarioname as ScenarioName,scriptid as ScriptId,containerid as ContainerId,containername as ContainerName,pageid as PageId,requestid as RequestID,address as Address,userid as UserId,iterationid as IterationId,starttime as StartTime,endtime as EndTime,diff as Diff,reponsecode as ResponseCode,responsesize as ResponseSize from reportdata", _con);
                    dt.Load(reader);
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
            return dt;
        }
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
        public void Executequery(string reportName, string query)
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
