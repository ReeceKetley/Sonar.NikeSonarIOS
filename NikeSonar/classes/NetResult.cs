using System;
using System.Collections.Generic;
using System.Text;

namespace NikeSonar
{
    public class NetResult
    {
        public string message;
        public object obj;
        public bool success;

        public NetResult(bool success, string message, object obj = null)
        {
            this.success = success;
            this.message = message;
            this.obj = obj;
        }
    }
}
