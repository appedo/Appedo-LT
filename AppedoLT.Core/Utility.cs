using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.VisualBasic.FileIO;

namespace AppedoLT.Core
{
  public  class Utility
    {
        
        public static DataSet StoredRequest = null;
        public static DataTable GetDataTableFromCSVFile1(string FilePath, string Delim)
        {
            DataTable tbl = new DataTable();
            string CsvData = string.Empty;
            CsvData = File.ReadAllText(FilePath,Encoding.Default);
            bool firstRow = true;
            foreach (string row in CsvData.Split("\n".ToCharArray()))
            {
                DataRow dr = tbl.NewRow();
                System.Text.RegularExpressions.RegexOptions options = (
                    System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace
                  | System.Text.RegularExpressions.RegexOptions.Multiline
                  | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                Regex reg = new Regex(Delim
                  + @"(?=(?:[^\""]*\""[^\""]*\"")*(?![^\""]*\""))", options);
               // var csvArray = reg.Split(row.Replace("\n", "").Replace("\r", ""));
                var csvArray = reg.Split(row.Trim());
                for (int i = 0; i < csvArray.Length; i++)
                {
                    csvArray[i] = csvArray[i].Replace("\"\"", "");
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = csvArray[i] });
                    else
                        dr[i] = csvArray[i];
                    
                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            return tbl;
        }
        public static DataTable GetDataTableFromCSVFile(string FilePath, string Delim)
        {
            bool firstRow = true;
            DataTable tbl = new DataTable();
            TextFieldParser parser = new TextFieldParser(FilePath, Encoding.Default);
            parser.TextFieldType = FieldType.Delimited;
            parser.TrimWhiteSpace = false;
         
            parser.SetDelimiters(Delim);
            while (!parser.EndOfData)
            {
               
                DataRow dr = tbl.NewRow();
                string[] fields = parser.ReadFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = fields[i] });
                    else
                        dr[i] = fields[i];
                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            parser.Close();
            return tbl;
        }
        public static DataTable GetDataTableFromCSVContent(string content, string Delim)
        {
            bool firstRow = true;
            DataTable tbl = new DataTable();
            TextFieldParser parser = new TextFieldParser(new MemoryStream(Encoding.Default.GetBytes(content)));
            parser.TextFieldType = FieldType.Delimited;
            parser.TrimWhiteSpace = false;

            parser.SetDelimiters(Delim);
            while (!parser.EndOfData)
            {

                DataRow dr = tbl.NewRow();
                string[] fields = parser.ReadFields();
                for (int i = 0; i < fields.Length; i++)
                {
                    if (firstRow)
                        tbl.Columns.Add(new DataColumn() { ColumnName = fields[i] });
                    else
                        dr[i] = fields[i];
                }
                if (!firstRow && !dr[0].ToString().Length.Equals(0)) tbl.Rows.Add(dr);
                firstRow = false;
            }
            parser.Close();
            return tbl;
        }
        public static void SaveDataSet(DataSet ds, string path)
        {
            if (System.IO.File.Exists(path) == true)
            {
                File.Delete(path);
            }
            ds.WriteXml(path);
        }
        public static void LoadRequest(DataSet requestData, string path)
        {
            StoredRequest = new DataSet();
            StoredRequest.ReadXml(path + "\\repository.xml");
        }
       
        public static DataTable GetExtractor(string requestid)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("name");
            dt.Columns.Add("regex");
            dt.Columns.Add("ordinal");
            if (Utility.StoredRequest.Tables.Contains("Variables") == true)
            {
                foreach (DataRow dr in Utility.StoredRequest.Tables["Variables"].Select("requestid='" + requestid + "'"))
                {
                    dt.Rows.Add(dr["name"], dr["regex"], dr["ordinal"]);
                }
            }
            return dt;
        }
     
        public static string SerializeObjectToXML(object item)
        {
            try
            {
                string xmlText;
                Type objectType = item.GetType();
                XmlSerializer xmlSerializer = new XmlSerializer(objectType);
                MemoryStream memoryStream = new MemoryStream();
                using (XmlTextWriter xmlTextWriter =
                new XmlTextWriter(memoryStream, Encoding.ASCII) { Formatting = Formatting.Indented })
                {
                    xmlSerializer.Serialize(xmlTextWriter, item);
                    memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                    xmlText = new ASCIIEncoding().GetString(memoryStream.ToArray());
                    memoryStream.Dispose();
                    return xmlText;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Write(e.ToString());
                return null;
            }
        }
        public static object DeserializeXMLToObject(string xmlText, Type objectType)
        {
            if (string.IsNullOrEmpty(xmlText)) return null;
            XmlSerializer xs = new XmlSerializer(objectType);
            using (MemoryStream memoryStream = new MemoryStream(new ASCIIEncoding().GetBytes(xmlText)))
            {
                return xs.Deserialize(memoryStream);
            }
        }
        public static string GetFileContent(string filename)
        {
            if (File.Exists(filename))
            {
                return System.Text.Encoding.Default.GetString(File.ReadAllBytes(filename));
            }
            else
            {
                return string.Empty;
            }
           
        }
        public static Object CloneType(Object objtype)
        {
            Object lstfinal = new Object();

            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
                binaryFormatter.Serialize(memStream, objtype); memStream.Seek(0, SeekOrigin.Begin);
                lstfinal = binaryFormatter.Deserialize(memStream);
            }

            return lstfinal;
        }
        
    }

}
