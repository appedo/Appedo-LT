using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using AppedoLT.Core;
namespace AppedoLT.BusinessLogic
{
    class TcpRequest : Request
    {
        Connection con;
        public TcpRequest(XmlNode request, Connection con, bool storeResult)
        {
            RequestId = int.Parse(request.Attributes["id"].Value);
            HasError = false;
            RequestNode = request;
            responseTime = new Stopwatch();
            ResponseStream = new MemoryStream();
            StoreRequestBody = storeResult;
            this.con = con;
        }
        public override void GetResponse()
        {
            ResponseCode = 200;
            
            Encoding asen = Encoding.Default;
            byte[] requestBytes;
            byte[] receiveBuffer = new byte[1];
            int responseSize;
            string requestStr = RequestNode.Attributes["requestcontent"].Value;

            #region Parameter
            //try
            //{

            //    //EvalutionResult EvalutionResult = EvaluteExp(requestStr);
            //    //if (EvalutionResult.isSuccess == true)
            //    //{
            //    //    requestStr = EvalutionResult.value;
            //    //}
            //    //else
            //    //{
            //    //    LockException(tcpRequest.Attributes["id"].Value, EvalutionResult.value, "600");
            //    //}

            //    //foreach (XmlNode reqParam in tcpRequest.SelectNodes("params/param"))
            //    //{
            //    //    XmlNode param = reqParam.CloneNode(true);

            //    //    if (param.Attributes["value"].Value.Contains("$$") == true)
            //    //    {
            //    //        EvalutionResult = EvaluteExp(param.Attributes["value"].Value);
            //    //        if (EvalutionResult.isSuccess == true)
            //    //        {
            //    //            param.Attributes["value"].Value = EvalutionResult.value;
            //    //        }
            //    //        else
            //    //        {
            //    //            LockException(tcpRequest.Attributes["id"].Value, EvalutionResult.value, "600");
            //    //        }
            //    //    }

            //    //    #region Padding(Adding Char)
            //    //    if (param.Attributes["length"] != null && param.Attributes["length"].Value.Length > 0)
            //    //    {
            //    //        int totalWidth = Convert.ToInt32(param.Attributes["length"].Value);
            //    //        int difference = totalWidth - param.Attributes["value"].Value.Length;
            //    //        if (difference > 0)
            //    //        {
            //    //            if (param.Attributes["paddingtype"].Value == "L") param.Attributes["value"].Value = param.Attributes["value"].Value.PadLeft(totalWidth, param.Attributes["paddingchar"].Value[0]);
            //    //            else if (param.Attributes["paddingtype"].Value == "R") param.Attributes["value"].Value = param.Attributes["value"].Value.PadRight(totalWidth, param.Attributes["paddingchar"].Value[0]);
            //    //        }
            //    //        else if (difference < 0)
            //    //        {
            //    //            param.Attributes["value"].Value = param.Attributes["value"].Value.Substring(0, Convert.ToInt32(param.Attributes["length"].Value));
            //    //        }
            //    //    }
            //    //    #endregion

            //    //    if (IsValidation == true)
            //    //    {
            //    //        Parameter parm = new Parameter();
            //    //        parm.Name = param.Attributes["name"].Value;
            //    //        parm.Value = param.Attributes["value"].Value;
            //    //        requestResponse.Parameters.Add(parm);
            //    //    }
            //    //    requestStr = requestStr.Replace(requestStr.Substring(Convert.ToInt16(param.Attributes["startposition"].Value) - 1, Convert.ToInt16(param.Attributes["length"].Value)), param.Attributes["value"].Value);
            //    //}
            //}
            //catch (EvaluteException ex)
            //{
            //    requestResponse.RequestId = tcpRequest.Attributes["id"].Value;
            //    requestResponse.StartTime = DateTime.Now;
            //    requestResponse.EndTime = DateTime.Now;
            //    requestResponse.Duration = elapsedTimer.Elapsed.TotalMilliseconds;
            //    requestResponse.IsSucess = false;
            //    requestResponse.TcpIPResponse = ex.Message;
            //    requestResponse.TcpIPRequest = requestStr;
            //    requestResponse.ResponseCode = "400";

            //    RequestException exception = new RequestException();
            //    exception.reportname = _reportName;
            //    exception.scenarioname = Status.ScenarioName;
            //    exception.scriptname = _vuScriptXml.Attributes["name"].Value;
            //    exception.requestid = tcpRequest.Attributes["id"].Value;
            //    exception.request = requestStr;
            //    exception.iterationid = this._iterationid.ToString();
            //    exception.userid = this._userid.ToString();
            //    exception.requestexceptionid = Guid.NewGuid().ToString();
            //    exception.time = DateTime.Now;
            //    exception.from = "Tool";
            //    exception.message = ex.Message;
            //    errors.AddExeception(exception);
            //    requestResponse.IsSucess = false;
            //    responseCode = exception.errorcode = "400";
            //    return requestResponse;
            //}
            #endregion

            requestBytes = asen.GetBytes(requestStr);

            try
            {
                StartTime = DateTime.Now;

                responseTime.Start();

                lock (con)
                {
                    try
                    {
                        try
                        {
                            con.NetworkStream.Flush();
                            con.NetworkStream.Write(requestBytes, 0, requestBytes.Length);
                        }
                        catch
                        {
                            StartTime = DateTime.Now;
                            responseTime.Reset();
                            responseTime.Start();
                            con.Reconnect();
                            con.NetworkStream.Flush();
                            con.NetworkStream.Write(requestBytes, 0, requestBytes.Length);
                        }
                        con.NetworkStream.ReadTimeout = 120000;

                        con.Client.ReceiveTimeout = 120000;

                        int timeOut = 1000;
                        while (con.Client.Available < Convert.ToInt32(RequestNode.Attributes["responsesize"].Value))
                        {
                            Thread.Sleep(10);
                            timeOut = timeOut - 10;
                            if (timeOut <= 0) break;
                        }
                        timeOut = 60000;
                        while (con.Client.Available <= 0)
                        {
                            Thread.Sleep(10);
                            timeOut = timeOut - 10;
                            if (timeOut <= 0) break;
                        }

                        while (con.Client.Available != 0 && (responseSize = con.NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                        {
                            ResponseStream.Write(receiveBuffer, 0, responseSize);
                            if (con.Client.Available == 0) break;
                        }

                        if (Convert.ToInt32(RequestNode.Attributes["responsesize"].Value) > ResponseStream.Length)
                        {
                            Thread.Sleep(10);
                            while (con.Client.Available != 0 && (responseSize = con.NetworkStream.Read(receiveBuffer, 0, receiveBuffer.Length)) != 0)
                            {
                                ResponseStream.Write(receiveBuffer, 0, responseSize);
                                if (con.Client.Available == 0) break;
                            }
                        }
                        con.NetworkStream.Flush();
                        Success = true;
                        ResponseSize = ResponseStream.Length;
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                    finally
                    {
                        con.IsHold = false;
                        responseTime.Stop();
                    }
                }

            }
            catch (Exception ex)
            {
                if (ex.Message != "Thread was being aborted.")
                {
                    Success = false;
                    HasError = true;
                    ErrorMessage = ex.Message;
                    ErrorCode = "700";
                }
            }

        }
        public override void PerformAssertion()
        {
            bool isAssertionFaild = false;
            bool requestsizeconstant = Convert.ToBoolean(RequestNode.Attributes["requestsizeconstant"].Value);
            bool responsesizeconstant = Convert.ToBoolean(RequestNode.Attributes["responsesizeconstant"].Value);
            string requestStr = RequestNode.Attributes["requestcontent"].Value;
            if (ResponseStr.Length == 0)
            {
                isAssertionFaild = true;
                AssertionFaildMsg.Append("Request timeout.").Append(Environment.NewLine);

            }
            if (requestsizeconstant == true && requestStr.Length != Convert.ToInt16(RequestNode.Attributes["requestsize"].Value))
            {
                isAssertionFaild = true;
                AssertionFaildMsg.Append("Request size(" + requestStr.Length + ") is not match with actual request size(" + Convert.ToInt16(RequestNode.Attributes["requestsize"].Value) + ").").Append(Environment.NewLine);

            }
            if (responsesizeconstant == true && ResponseStr.Length != Convert.ToInt16(RequestNode.Attributes["responsesize"].Value))
            {
                isAssertionFaild = true;
                AssertionFaildMsg.Append("Reponse size(" + ResponseStr.Length + ") is not match with actual response size(" + Convert.ToInt16(RequestNode.Attributes["responsesize"].Value) + ")").Append(Environment.NewLine);
            }
            if (ResponseStr.Length > 0)
            {
                if (RequestNode.SelectSingleNode("assertions") != null)
                {
                    foreach (XmlNode assertion in RequestNode.SelectNodes("assertions/assertion"))
                    {

                        switch (assertion.Attributes["type"].Value)
                        {
                            case "0":
                                #region Req-NotContain
                                try
                                {
                                    int position = Convert.ToInt16(assertion.Attributes["reqposition"].Value);
                                    int length = Convert.ToInt16(assertion.Attributes["reqlength"].Value);
                                    string str = assertion.Attributes["reqtext"].Value;
                                    if (requestStr.Substring(position - 1, length) != str)
                                    {
                                        isAssertionFaild = true;
                                        AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    isAssertionFaild = true;
                                    AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                                #endregion
                                break;
                            case "1":
                                #region Res-NotContain
                                try
                                {
                                    int position = Convert.ToInt16(assertion.Attributes["resposition"].Value);
                                    int length = Convert.ToInt16(assertion.Attributes["reslength"].Value);


                                    string str = assertion.Attributes["restext"].Value;
                                    if (ResponseStr.ToString().Substring(position - 1, length) != str)
                                    {
                                        isAssertionFaild = true;
                                        AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    isAssertionFaild = true;
                                    AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                                #endregion
                                break;
                            case "2":
                            case "3":
                                #region Req-Res Equal or no equal
                                try
                                {
                                    int reqposition = Convert.ToInt16(assertion.Attributes["reqposition"].Value);
                                    int reqlength = Convert.ToInt16(assertion.Attributes["reqlength"].Value);

                                    int resposition = Convert.ToInt16(assertion.Attributes["resposition"].Value);
                                    int reslength = Convert.ToInt16(assertion.Attributes["reslength"].Value);

                                    string req = requestStr.Substring(reqposition - 1, reqlength);
                                    string res = ResponseStr.ToString().Substring(resposition - 1, reslength);
                                    if (assertion.Attributes["type"].Value == "2" && req == res)
                                    {
                                        isAssertionFaild = true;
                                        AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    }
                                    else if (assertion.Attributes["type"].Value == "3" && req != res)
                                    {
                                        isAssertionFaild = true;
                                        AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine);
                                    }

                                }
                                catch (Exception ex)
                                {
                                    isAssertionFaild = true;
                                    AssertionFaildMsg.Append(assertion.Attributes["message"].Value).Append(Environment.NewLine); ;
                                    ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                                }
                                #endregion
                                break;
                        }
                    }
                }
            }

            if (isAssertionFaild == false) AssertionResult = true;
            else AssertionResult = false;

            if (AssertionResult == false)
            {
                Success = false;
                HasError = true;
                ErrorMessage = AssertionFaildMsg.ToString();
                ErrorCode = "800";
            }

        }
        public override void Abort()
        {
            con = null;
        }
    }
}
