using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.Security;

namespace NikeSonar
{
    class Functions
    {
        private static readonly DateTime Jan1st1970 = new DateTime(0x7b2, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                input.CopyTo(stream);
                return stream.ToArray();
            }
        }

        public static void LogToConsole(string input)
        {
            //Console.WriteLine(" ");
            //Console.WriteLine(" ");
            //Console.WriteLine("----------------------------");
            //Console.WriteLine(input);
            //Console.WriteLine("----------------------------");
            //Console.WriteLine(" ");
            //Console.WriteLine(" ");
        }

 

        public static long CurrentTimeMillis()
        {
            TimeSpan span = (TimeSpan)(DateTime.UtcNow - Jan1st1970);
            return (long)span.TotalMilliseconds;
        }

        public static string CalculateMD5Hash(string input)
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = MD5.Create();
            byte[] inputBytes = Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public static long CurrentTimeSeconds()
        {
            TimeSpan span = (TimeSpan)(DateTime.UtcNow - Jan1st1970);
            return (long)span.TotalSeconds;
        }

        public static string ExtractBetween(string original, string first, string second)
        {
            if (String.IsNullOrEmpty(original))
            {
                return String.Empty;
            }
            try
            {
                int startIndex = original.IndexOf(first) + first.Length;
                int index = original.IndexOf(second, startIndex);
                return original.Substring(startIndex, index - startIndex);
            }
            catch (Exception)
            {
                return String.Empty;
            }
        }

        public static string UrlEncode(string input)
        {
            return UrlHelper.Encode(input);
        }
        public static byte[] DecompressGZIP(byte[] gzip)
        {
            byte[] buffer2;
            using (GZipStream stream = new GZipStream(new MemoryStream(gzip), CompressionMode.Decompress))
            {
                byte[] buffer = new byte[0x1000];
                using (MemoryStream stream2 = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, 0x1000);
                        if (count > 0)
                        {
                            stream2.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    buffer2 = stream2.ToArray();
                }
            }
            return buffer2;
        }
        public static string UniqueID
        {
            get
            {
                var query = new SecRecord(SecKind.GenericPassword);
                query.Service = NSBundle.MainBundle.BundleIdentifier;
                query.Account = "UniqueID";

                NSData uniqueId = SecKeyChain.QueryAsData(query);
                if (uniqueId == null)
                {
                    query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
                    var err = SecKeyChain.Add(query);
                    if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
                        throw new Exception("Cannot store Unique ID");

                    return query.ValueData.ToString();
                }
                else
                {
                    return uniqueId.ToString();
                }
            }
        }

        public static string RandomString(int size)
        {  
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));                 
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
