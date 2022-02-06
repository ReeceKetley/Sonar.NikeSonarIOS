using System;
using System.Collections.Generic;
using System.Text;

namespace NikeSonar
{
    public class AddToCartResult
    {
        public AddToCartCode Code;
        public string Response;
        public Object Data;

        public AddToCartResult(AddToCartCode code, string response = "", Object data = null)
        {
            Code = code;
            Response = response;
            Data = data;
        }
    }
    public enum AddToCartCode
    {
        HttpError,
        JsonInvalid,
        JsonFailure,
        JsonWait,
        JsonSuccess
    }
}
