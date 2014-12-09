using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using AppedoLT.Core;
using System.IO;
namespace AppedoLT.BusinessLogic
{
    public class VariableManager
    {
        public static VariableManager dataCenter = null;
        private static DataSet datas = null;

        private Dictionary<string, string> _VaraibleStartPosition = new Dictionary<string, string>();
        private Dictionary<string, XmlNode> _variableInfo = new Dictionary<string, XmlNode>();
        private Random random = new Random();

        private XmlDocument _doc = VariableXml.GetInstance().doc;
        public VariableManager()
        {
            //_doc = VariableXml.GetInstance().doc;
            ValidateVariableVersion();
            datas = new DataSet();
            foreach (XmlNode variable in _doc.SelectNodes("//variables//variable[@from='variablemanager']"))
            {
                try
                {
                    _variableInfo.Add(variable.Attributes["name"].Value, variable);
                    if (variable.Attributes["type"].Value == "file")
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt = Utility.GetDataTableFromCSVFile(Constants.GetInstance().ExecutingAssemblyLocation  + variable.Attributes["vituallocation"].Value, variable.Attributes["delimiter"].Value);
                            dt.TableName = variable.Attributes["name"].Value;
                            datas.Tables.Add(dt);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.TableName = variable.Attributes["name"].Value;
                        dt.Columns.Add("userid");
                        dt.Columns.Add("iterationid");
                        dt.Columns.Add("value");
                        dt.Rows.Add("0", "0", GetValue(variable.Attributes["name"].Value, variable.Attributes["type"].Value));
                        datas.Tables.Add(dt);

                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
        }
        public VariableManager(XmlNode variableNode)
        {
            //_doc = VariableXml.GetInstance().doc;
            datas = new DataSet();
            foreach (XmlNode variable in variableNode.ChildNodes)
            {
                try
                {
                    _variableInfo.Add(variable.Attributes["name"].Value, variable);
                    if (variable.Attributes["type"].Value == "file")
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt = Utility.GetDataTableFromCSVContent(variable.ChildNodes[0].InnerText, variable.Attributes["delimiter"].Value);
                            dt.TableName = variable.Attributes["name"].Value;
                            datas.Tables.Add(dt);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        DataTable dt = new DataTable();
                        dt.TableName = variable.Attributes["name"].Value;
                        dt.Columns.Add("userid");
                        dt.Columns.Add("iterationid");
                        dt.Columns.Add("value");
                        dt.Rows.Add("0", "0", GetValue(variable.Attributes["name"].Value, variable.Attributes["type"].Value));
                        datas.Tables.Add(dt);

                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + ex.Message);
                }
            }
        }
        public object GetVariableValue(int userid, int iterationid, string variableName, int totalUser)
        {
            object result = new object();
            DataTable data = new DataTable();
            if (variableName.Contains('.') == true)
            {
                data = datas.Tables[variableName.Split('.')[0]];
                variableName = variableName.Split('.')[1];
            }
            else
            {
                data = datas.Tables[variableName];
            }
            string VariablePolicy = _variableInfo[data.TableName].Attributes["policy"].Value;
            string variableType = _variableInfo[data.TableName].Attributes["type"].Value;

            if (variableType == "file")
            {
                int rowId;
                int startPosition = Convert.ToInt32(_variableInfo[data.TableName].Attributes["start"].Value);
                if (VariablePolicy == "eachuser")
                {
                    rowId = ((userid - 1) + (startPosition - 1));
                    if (variableType == "file") rowId %= data.Rows.Count;
                }
                else if (VariablePolicy == "eachiteration")
                {
                    rowId = (((iterationid - 1) * totalUser) + userid + startPosition - 2);
                    if (variableType == "file") rowId %= data.Rows.Count;
                }
                else
                {
                    rowId = 0;
                }
                result = data.Rows[rowId][variableName];
            }
            else
            {
                DataRow[] dr = null;
                if (VariablePolicy == "eachuser") dr = data.Select("userid=" + userid);
                else if (VariablePolicy == "eachiteration") dr = data.Select("userid=" + userid + " and iterationid=" + iterationid);
                else dr = data.Select("userid=0 and iterationid=0");
                if (dr.Length > 0)
                {
                    result = dr[0]["value"];
                }
                else
                {
                    DataRow newItem = data.NewRow();
                    newItem["userid"] = userid.ToString();
                    newItem["iterationid"] = iterationid.ToString();
                    newItem["value"] = GetValue(data.TableName, _variableInfo[data.TableName].Attributes["type"].Value);
                    result = newItem["value"].ToString();
                    data.Rows.Add(newItem);
                }
            }
            return result;
        }
        public void SetVariableValue(int userid, int iterationid, string variableName, object value, int totalUser)
        {
            object result = new object();
            DataTable data = new DataTable();
            if (variableName.Contains('.') == true)
            {
                data = datas.Tables[variableName.Split('.')[0]];
                variableName = variableName.Split('.')[1];
            }
            else
            {
                data = datas.Tables[variableName];
            }
            string VariablePolicy = _variableInfo[data.TableName].Attributes["policy"].Value;
            string variableType = _variableInfo[data.TableName].Attributes["type"].Value;

            if (variableType == "file")
            {
                int rowId;
                int startPosition = Convert.ToInt32(_variableInfo[data.TableName].Attributes["start"].Value);
                if (VariablePolicy == "eachuser")
                {
                    rowId = ((userid - 1) + (startPosition - 1));
                    if (variableType == "file") rowId %= data.Rows.Count;
                }
                else if (VariablePolicy == "eachiteration")
                {
                    rowId = (((iterationid - 1) * totalUser) + userid + startPosition - 2);
                    if (variableType == "file") rowId %= data.Rows.Count;
                }
                else
                {
                    rowId = 0;
                }
                data.Rows[rowId][variableName] = value;
            }
            else
            {
                DataRow[] dr = null;
                if (VariablePolicy == "eachuser") dr = data.Select("userid=" + userid);
                else if (VariablePolicy == "eachiteration") dr = data.Select("userid=" + userid + " and iterationid=" + iterationid);
                else dr = data.Select("userid=0 and iterationid=0");
                if (dr.Length > 0)
                {
                    dr[0]["value"] = value;
                }
                else
                {
                    DataRow newItem = data.NewRow();
                    newItem["userid"] = userid.ToString();
                    newItem["iterationid"] = iterationid.ToString();
                    switch (variableType)
                    {
                        case "randomstring":
                            newItem["value"] = RandomString(Convert.ToInt32(_variableInfo[data.TableName].Attributes["start"].Value), Convert.ToInt32(_variableInfo[data.TableName].Attributes["end"].Value));
                            break;
                        case "randomnumber":
                            newItem["value"] = new Random().Next(Convert.ToInt32(_variableInfo[data.TableName].Attributes["start"].Value), Convert.ToInt32(_variableInfo[data.TableName].Attributes["end"].Value));
                            break;
                        default:
                            newItem["value"] = _variableInfo[data.TableName].Attributes["value"].Value;
                            break;
                    }
                    newItem["value"] = value;
                    data.Rows.Add(newItem);
                }
            }
        }
        public string GetVariableType(string variableName)
        {
            return _variableInfo[variableName].Attributes["type"].Value;
        }
        private string RandomString(int min, int max)
        {
            StringBuilder builder = new StringBuilder();
            max = random.Next(min, max);
            int index;

            for (index = 0; index < max; index++)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
            }
            return builder.ToString().ToLower();
        }
        private object GetValue(string variableName, string variableType)
        {
            string result = string.Empty;
            switch (variableType)
            {
                case "randomstring":
                    result = RandomString(Convert.ToInt32(_variableInfo[variableName].Attributes["start"].Value), Convert.ToInt32(_variableInfo[variableName].Attributes["end"].Value) + 1);
                    break;
                case "randomnumber":
                    result = random.Next(Convert.ToInt32(_variableInfo[variableName].Attributes["start"].Value), Convert.ToInt32(_variableInfo[variableName].Attributes["end"].Value) + 1).ToString();
                    break;
                case "currentdate":
                    result = DateTime.Now.ToString(_variableInfo[variableName].Attributes["dateformat"].Value);
                    break;
                default:
                    result = _variableInfo[variableName].Attributes["value"].Value;
                    break;
            }
            return result;
        }
        private void ValidateVariableVersion()
        {
            foreach (XmlNode variable in VariableXml.GetInstance().doc.SelectNodes("//variables/variable[@type='file']"))
            {
                try
                {
                    if (File.Exists(variable.Attributes["location"].Value))
                    {
                        FileInfo source = new FileInfo(variable.Attributes["location"].Value);
                        if (source.LastWriteTime.Ticks != Convert.ToDouble(variable.Attributes["modified"].Value))
                        {
                            string ticks = source.LastWriteTime.Ticks.ToString();
                            File.Copy(variable.Attributes["location"].Value, Constants.GetInstance().ExecutingAssemblyLocation + "\\" + variable.Attributes["vituallocation"].Value, true);
                            variable.Attributes["modified"].Value = ticks;
                        }
                    }
                }
                catch (Exception ex)
                {
                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                }
            }
        }
    }
    public class VariableInfo
    {
        public string VaraiblePolicy { get; set; }
        public string VaraibleType { get; set; }
        public string StartPosition { get; set; }
        public VariableInfo(string varaiblePolicy, string varaibleType, string startPosition)
        {
            VaraiblePolicy = varaiblePolicy;
            VaraibleType = varaibleType;
            StartPosition = startPosition;
        }
    }

}
