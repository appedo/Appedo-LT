using System;
using System.Data;
using System.Windows.Forms;
using AppedoLT.Core;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AppedoLT.DataAccessLayer
{

    public class RunTimeException
    {
        private static RunTimeException _instance;
        public bool storeErrors = true;
        public static RunTimeException GetInstance()
        {
            if (_instance == null)
            {
                _instance = new RunTimeException();
            }
            return _instance;
        }
        private RunTimeException()
        {
        }
        public void AddExeception(RequestException requestException)
        {
            if (storeErrors == true)
            {
                lock (DataServer.GetInstance().errors)
                {
                    requestException.message = requestException.message.Replace("\r\n", " ");
                    DataServer.GetInstance().errors.Enqueue(requestException);
                }
            }
        }
    }
   
    
}
