using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using MonoTouch.StoreKit;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NikeSonar
{
    [Serializable]
    public class JSONLoginData
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DeviceId { get; set; }
        public JSONLoginData(string username, string password, string deviceid)
        {
            Username = username;
            Password = password;
            DeviceId = deviceid;
        }
    }
    class ApiLogin
    {

        private static string _ky = "7E6FC60178275B1C7A0E4727F9EEC19F";
        private static string _iv = "F91CEE9F7274E0A7C1B57287106CF6E7";
        public static LoginResponseCode Login(string devId, string user, string pass)
        {
            HTTP http = new HTTP("");
            string salt = Functions.RandomString(10);
            string postData = Functions.RandomString(50) + "{" + string.Format("\"username\": \"{0}\", \"password\": \"{1}\", \"device\": \"{2}\", \"salt\": \"{3}\"", new[] { user, pass, devId, salt }) + "}";
            try
            {
                postData = Crypto.EncryptRJ256(postData);
            }
            catch
            {
                return LoginResponseCode.UnkownFail;
            }
            http.IncludeHeaderInResponse = false;
            string postRequest = http.Post("http://nikesonar.com/Nike/api.php", "v=" + Functions.UrlEncode(postData));
            Console.WriteLine(postRequest);
            if (postRequest == "")
            {
                return LoginResponseCode.HttpError;
            }

            try
            {
                postRequest = Crypto.DecryptRJ256(postRequest);
            }
            catch
            {
                //Console.WriteLine("'Decrypt Fail!'");
                return LoginResponseCode.DecryptFail;
            }
            int len = postRequest.IndexOf("\0");
            postRequest = postRequest.Substring(0, len);
            int start = postRequest.IndexOf("{");
            if (start < 0)
            {
                return LoginResponseCode.DecryptFail;
            }

            postRequest = postRequest.Substring(start);
            //Console.WriteLine(postRequest);
            JObject responseData;
            try
            {
                responseData = JObject.Parse(postRequest);
            }
            catch
            {
                return LoginResponseCode.JsonFail;
            }
            if (responseData["salt"] == null || (string)responseData["salt"] != salt)
            {
                return LoginResponseCode.JsonFail;
            }
            if (responseData["status"] == null)
            {
                return LoginResponseCode.JsonFail;
            }
            if (responseData["accountlimit"] != null)
            {
                SonarSettings.MaxAccounts = Convert.ToInt32((string) responseData["accountlimit"]);
            }
            else
            {
                SonarSettings.MaxAccounts = 0;
            }
            switch ((string)responseData["status"])
            {
                case "success":
                    return LoginResponseCode.Sucess;
                case "activated":
                    return LoginResponseCode.LinkedDevice;
                case "maxdevices":
                    return LoginResponseCode.MaxDevices;
                default:
                    return LoginResponseCode.InvalidCredentials;
            }
        }
    }
}
