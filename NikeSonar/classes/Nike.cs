using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Net;
using System.Text;
using MonoTouch.Foundation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace NikeSonar
{
    class Nike
    {
        private int waitTimeMilliSeconds;

        public readonly HTTP Http;

        public Nike(string proxy = "")
        {
            Http = new HTTP("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/29.0.1547.76 Safari/537.36", proxy);
        }

        public bool Initialize()
        {
            return Http.Get("http://store.nike.com/us/en_us/") != string.Empty;
        }

        public NetResult Login(string username, string password, int timeout = 0)
        {
            string message = Http.Post("https://www.nike.com/profile/login", "login=" + Functions.UrlEncode(username) + "&rememberMe=false&password=" + Functions.UrlEncode(password), "application/x-www-form-urlencoded", true, timeout);
            if (Http.LastStatusCode == HttpStatusCode.OK)
            {
                if (message.Contains("nikePlusId"))
                {
                    return new NetResult(true, string.Empty, null);
                }
                return new NetResult(false, message, null);
            }
            return new NetResult(false, Http.ResponseExceptionString, null);
        }

        public NetResult DownloadProductData(string url, int timeout = 0)
        {
            string html = Http.Get(url, timeout);
            if (html == string.Empty)
            {
                if (Http.LastStatusCode == HttpStatusCode.Gone)
                {
                    return new NetResult(false, "THE PRODUCT YOU ARE LOOKING FOR IS NO LONGER AVAILABLE");
                }
                return new NetResult(false, "An unexpected HTTP error occurred");
            }
            string config = Functions.ExtractBetween(html, "<script id=\"product-data\" type=\"template-data\">", "</script>");
            if (config == string.Empty)
            {
                if (html.Contains("THE PRODUCT YOU ARE LOOKING FOR IS NO LONGER AVAILABLE"))
                {
                    return new NetResult(false, "THE PRODUCT YOU ARE LOOKING FOR IS NO LONGER AVAILABLE");
                }
                else
                {
                    return new NetResult(false, "Unable to extract JSON product configuration");
                }
            }

            // At this point we have the contents of the config.
            JObject jData = null;
            try
            {
                jData = JObject.Parse(config);
            }
            catch (Exception)
            {
                jData = null;
            }
            if (jData == null)
            {
                return new NetResult(false, "Unable to parse extracted JSON product configuration");
            }
            return new NetResult(true, "", jData);
        }

        public AddToCartResult AddToCart(string targetUrl, string postBody, string pil = "", string psh = "")
        {
            const string callbackString = "nike_Cart_hanleJCartResponse";
            long currentTime = Functions.CurrentTimeMillis();
            string tokens = "";

            if (!string.IsNullOrEmpty(pil))
            {
                tokens = "&pil=" + pil;
            }
            if (!string.IsNullOrEmpty(psh))
            {
                tokens += "&psh=" + psh;
            }

            string response;

            Http.Referer = targetUrl;
            bool requestResult = Http.GetTrueFalse("https://secure-store.nike.com/us/services/jcartService?callback=" + Functions.UrlEncode(callbackString) + postBody + tokens + "&_=" + Functions.UrlEncode(currentTime.ToString()), out response);
            Http.Referer = string.Empty;
            if (!requestResult)
            {
                // HttpError
                return new AddToCartResult(AddToCartCode.HttpError, response);
            }

            if (response == "" || Http.LastStatusCode != HttpStatusCode.OK)
            {
                // HttpError
                return new AddToCartResult(AddToCartCode.HttpError, response);
            }

            if (!response.Contains(callbackString + "({"))
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response);
            }

            if (response.Contains("\"status\" :\"success\","))
            {
                // JsonSuccess
                return new AddToCartResult(AddToCartCode.JsonSuccess);
            }

            // Process JSON
            string json = Functions.ExtractBetween(response, "({", "});");
            if (string.IsNullOrEmpty(json))
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response);
            }
            JObject jData;
            try
            {
                jData = JObject.Parse("{" + json + "}");
            }
            catch (Exception e)
            {
                jData = null;
            }
            if (jData == null)
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response);
            }

            string status = (string)jData["status"];
            if (status == null)
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response);
            }

            if (status == "failure")
            {
                // JsonFailure
                return new AddToCartResult(AddToCartCode.JsonFailure, response, jData);
            }

            if (!status.Contains("wait"))
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response, jData);
            }

            pil = (string)jData["pil"];
            if (pil == null)
            {
                // JsonInvalid
                return new AddToCartResult(AddToCartCode.JsonInvalid, response, jData);
            }
            psh = (string)jData["psh"];
            string ewt = (string)jData["ewt"];

            WaitInLineReply waitToken = new WaitInLineReply();
            waitToken.pil = pil;
            waitToken.psh = psh;
            waitToken.ewt = ewt;

            // JsonWait
            return new AddToCartResult(AddToCartCode.JsonWait, response, waitToken);
        }

        public NetResult BuildPostBody(JObject productData, string size)
        {
            string sizeType = "";
            // Let's locate the right size.
            JToken jSize = null;
            /*
            foreach (JToken tok in jData["skuContainer"]["productSkus"])
            {
                if (size == (string)tok["sizeDescription"])
                {
                    jSize = tok;
                    break;
                }
            }
            */

            //string sizeType = "";
            //string displaySize = "";
            // convert sizes to the same type
            // loop through until the right one is found
            // convert back to the product size type
            // begin add to cart process
            // determine page locale
            // check if locale is permitted
            // US/EU/GB/JP/CN
            //NikeLocale.US

            //displaySizeType = 
            try
            {
               sizeType = productData["skuContainer"]["displaySizeTypeLabel"] == null ||
                                  (string) productData["skuContainer"]["displaySizeTypeLabel"] == ""
                    ? null
                    : (string) productData["skuContainer"]["displaySizeTypeLabel"];
            }
            catch
            {
                
            }

            //if (jSize == null)
            //{
            try
            {
                foreach (JToken tok in productData["skuContainer"]["productSkus"])
                {
                    if (size == (string) tok["displaySize"])
                    {
                        jSize = tok;
                        break;
                    }
                }
                if (jSize == null)
                {
                    return new NetResult(false, "Unable to find matching size within product configuration");
                }
            }
            catch
            {
                return new NetResult(false, "Unable to find matching size within product configuration");
            }
            //}

            //displaySizeType

            // Build parameter strings and urlencode them 
            string[] postbody_params = new string[] { productData["crossSellConfiguration"]["language"] + "_" + productData["crossSellConfiguration"]["country"], (string)productData["crossSellConfiguration"]["country"], (string)productData["catalogId"], (string)productData["productId"], (string)productData["rawPrice"], (string)(((string)productData["trackingData"]["siteId"] == null) ? "null" : productData["trackingData"]["siteId"]), (string)productData["productTitle"], (string)productData["productSubTitle"], ((sizeType == null) ? "null" : sizeType), jSize["sku"] + ":" + jSize["displaySize"], (string)jSize["id"], (string)jSize["displaySize"] };
            for (int i = 0; i < postbody_params.Length; ++i)
            {
                postbody_params[i] = Functions.UrlEncode(postbody_params[i]);
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("&action=addItem&lang_locale={0}&country={1}&catalogId={2}&productId={3}&price={4}&siteId={5}&line1={6}&line2={7}&passcode=null&sizeType={8}&skuAndSize={9}&qty=1&rt=json&view=3&skuId={10}&displaySize={11}", postbody_params);
            return new NetResult(true, sb.ToString());
            //Clipboard.SetText(ret);
            //this.WriteLog(ret);
            //return new NetResult

        }
    
    }
    public class WaitInLineReply
    {
        public string pil;
        public string psh;
        public string ewt;

        public WaitInLineReply()
        {
            pil = string.Empty;
            psh = string.Empty;
        }
    }

}
