using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppedoLT.Core
{
    
   public class EvaluteException : Exception
    {
        private string _message = string.Empty;
        public override string Message
        {
            get
            {
                return _message;
            }            
        }

        public EvaluteException(string message)
        {
            _message = message;
        }

    }
}
