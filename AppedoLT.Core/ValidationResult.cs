using System;
using System.Collections.Generic;
using System.Net;
using System.Windows.Forms;
using System.Drawing;
namespace AppedoLT.Core
{
   public class ValidationResult
    {
       private static ValidationResult _instance;
       public static ValidationResult GetInstance(ListView resutGrid)
       {
           if (_instance == null)
           {
               _instance = new ValidationResult();
           }
           _instance.ResultGrid = resutGrid;
           return _instance;
       }
       public ListView ResultGrid;
       private ValidationResult()
       {
       }
       public void AddToList(RequestResponse requestResponse)
       {
           ListViewItem newItem = new ListViewItem(requestResponse.WebRequestResponseId.ToString());
           newItem.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
           if (requestResponse.RequestResult.HasError == true || requestResponse.RequestResult.Success == false)
           {
               newItem.StateImageIndex = 1;
           }
           else
           {
               newItem.StateImageIndex = 0;
           }
           //if (requestResponse.RequestResult.Success == false && requestResponse.RequestResult.HasError==false)
           //{
           //    newItem.StateImageIndex = 1;
           //}
           //else
           //{
           //    newItem.StateImageIndex = 0;
           //}
           newItem.Tag = requestResponse;
           newItem.SubItems.AddRange(new string[] { requestResponse.RequestResult.RequestId.ToString(), requestResponse.RequestResult.RequestName, requestResponse.RequestResult.StartTime.ToString(), requestResponse.RequestResult.EndTime.ToString(), requestResponse.RequestResult.ResponseTime.ToString(), requestResponse.RequestResult.ResponseCode.ToString(), requestResponse.RequestResult.Success.ToString() });

           //if (requestResponse.Request == null)
           //{
           //    newItem.SubItems.AddRange(new string[] { requestResponse.RequestResult.RequestId.ToString(), requestResponse.TcpIPRequest, requestResponse.StartTime.ToString(), requestResponse.EndTime.ToString(), requestResponse.Duration.ToString(), requestResponse.ResponseCode, requestResponse.IsSucess.ToString() });

           //}
           //else
           //{

           //}
           
           ResultGrid.Items.Add(newItem);
       }
       
       public void Clear()
       {
           ResultGrid.Items.Clear();
       }
       public int Count
       {
           private set { }
           get
           {
               return ResultGrid.Items.Count;
           }
       }
       public double Avg()
       {
           double result=0;
           foreach(ListViewItem item in this.ResultGrid.Items)
           {
               double temp=0;
               double.TryParse(item.SubItems[5].Text,out temp);
               result += temp;
           }
           if(this.ResultGrid.Items.Count>0)
           {
             result = result / this.ResultGrid.Items.Count;
           }
           return result;
       }
    }
   public class RequestResponse
   {
       public Request RequestResult;

       public int WebRequestResponseId { get; set; }
       public string RequestId { get; set; }
       public HttpWebRequest Request = null;
       public string PostData;
       public string ResponseCode = string.Empty;
       public string Response;
       public List<Parameter> Parameters = new List<Parameter>();
       public DateTime StartTime { get; set; }
       public DateTime EndTime
       { get; set; }
       public double Duration
       {
           get;
           set;
       }
       public bool IsSucess { get; set; }
       public string TcpIPRequest { get; set; }
       public string TcpIPResponse { get; set; }
   }
}
