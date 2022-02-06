using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Runtime;
using AlertView;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;
using Xamarin.Controls;
namespace NikeSonar
{
    [Serializable]
    public class ReleaseTask
    {
        private readonly object _mutex = new object();
        private readonly ManualResetEvent _waitEvent = new ManualResetEvent(false);
        private BotRunningViewController _viewController;
        private Thread _thread;
        private Nike _nike;
        private bool _cancellationPending;
        private bool _isSuccessful;
        private bool _stopRelease;
        private string _productName;
        private TaskCell _taskCell;
        public string Proxy;
        public string ProductUrl { get; set; }
        public readonly string SizeCode;
        public readonly string NikeUsername;
        public readonly string NikePassword;
        public bool IsRunning { get; private set; }
        private string _log;
        long timeTaken = Functions.CurrentTimeMillis();

        public ReleaseTask(string sizeCode, string nikeUsername, string nikePassword, BotRunningViewController viewController, TaskCell taskCell, string proxy = "")
        {
            NikeUsername = nikeUsername;
            NikePassword = nikePassword;
            SizeCode = sizeCode;
            Proxy = proxy;
            _thread = new Thread(thread_DoWork);
            _viewController = viewController;
            _taskCell = taskCell;
        }

        public void SetStatus(string status)
        {
            _viewController.InvokeOnMainThread(() =>
            {
                _taskCell.Status = status;
                _viewController.ReloadData();
            });
        }

        public void SetTargetUrl(string url)
        {
            if (url != "")
            {
                ProductUrl = url;

                _waitEvent.Set();
            }
        }

        public void StartAsync()
        {
            lock (_mutex)
            {
                if (IsRunning)
                {
                    //Console.WriteLine("Task is running");
                    return;
                }
                IsRunning = true;
                //Console.WriteLine("Starting task");
                _thread = new Thread(thread_DoWork);
                _thread.Start();
            }
        }

        public void StopAsync()
        {
            lock (_mutex)
            {
                if (IsRunning && !_cancellationPending && !_isSuccessful)
                {
                    _cancellationPending = true;
                }
            }
        }

        public void PrintAlert(string Title, string Message, string Image = "")
        {
            //Console.WriteLine("Alert: " + Title + " -  " + Message);
            if (Image != "")
            {
                UIImage img = UIImage.FromBundle("images/added.png");
                _viewController.InvokeOnMainThread(() =>
                {
                    AlertCenter.Default.PostMessage(Title, Message, img);
                });
            }
            else
            {
                _viewController.InvokeOnMainThread(() =>
                {
                    AlertCenter.Default.PostMessage(Title, Message);
                });
            }
        }

        private void AppendLog(string str)
        {
            //_log += str;
            //_viewController.UpdateLog(str);
        }
        private void WriteLog(string str)
        {
            //str = NikeUsername + " - " + DateTime.Now.ToString("mm:ss.fff") + ":" + str;
            //_viewController.UpdateStatus(str, _viewController.GetTaskCellID(NikeUsername));
            //SetStatus(str);
            //_log += str;
            //_viewController.UpdateLog(str);
        }

        private bool CancelPending()
        {
            lock (_mutex)
            {
                return _cancellationPending;
            }
        }
        private bool CancelPendingSleep(int millisecondsTimeout, int sleepTimeout = 100)
        {
            Int32 iEnd = Environment.TickCount + millisecondsTimeout;
            while (iEnd > Environment.TickCount)
            {
                lock (_mutex)
                {
                    if (_cancellationPending)
                    {
                        return true;
                    }
                }
                Thread.Sleep(sleepTimeout);
            }
            return false;
        }

        // Bot Routines
        private void routine_Perform()
        {
            if (Proxy != "")
            {
                _nike = new Nike(Proxy);
            }
            else
            {
                _nike = new Nike();
            }
            //Init Nike
            if (!routine_Setup())
            {
                return;
            }
            //Login Nike
            if (!routine_Login())
            {
                return;
            }
            //Wait for link 
            SetStatus("Waiting for link");
            _waitEvent.WaitOne();
            // check link != ""
            if (ProductUrl == "")
            {
                return;
            }
            string targetUrl = ProductUrl;
            JObject productData = null;
            if (!routine_DownloadProductData(ref productData, targetUrl))
            {
                return;
            }
            // Get size
            // Build post body
            NetResult result;
            result = _nike.BuildPostBody(productData, SizeCode);
            if (!result.success)
            {
                if (result.message.Contains("Unable to find matching size"))
                {
                    PrintAlert("NikeStore Error", "Product is unavaliable in your selected size.");
                }
                return;
            }

            string postBody = result.message;

            if (!routine_WaitForCountdown(productData))
            {
                return;
            }

            // wait for countdown (not all have countdown)
            if (!routine_AddToCart(postBody))
            {
                return;
            }
            SetStatus("Total Time: " + (Functions.CurrentTimeMillis() - timeTaken) + "ms\r\n");
            // Add to cart
            lock (_mutex)
            {
                if (_cancellationPending)
                {
                    return;
                }
                else
                {
                    _isSuccessful = true;
                }
            }
            routine_Success();
        }
        private void routine_Success()
        {
           SetStatus(_productName + " Added to cart");
           PrintAlert(NikeUsername, _productName + " - Added to cart");
            _viewController.InvokeOnMainThread(() =>
            {
                UILocalNotification notification = new UILocalNotification();
                notification.FireDate = DateTime.Now.AddSeconds(30);
                notification.AlertAction = NikeUsername;
                notification.AlertBody = _productName + " - Added to cart";
                notification.SoundName = UILocalNotification.DefaultSoundName;
                notification.AlertLaunchImage = "Resources/added.png";
            });
            var postSucess = new HTTP("");
            try
            {
                postSucess.Get(
                    Functions.UrlEncode(ViewControllers.APIServer + "Nike\\core.php?c=sucess&user=" +
                                        ViewControllers.APIUser +
                                        "&product=" + _productName));
            }
            catch
            {

            }
            _taskCell.Complete = true;
        }

        // General Routines
        private bool routine_Setup()
        {
            //Console.WriteLine("Routine setup");
            for (int i = 0; ; ++i)
            {
                if (i > 0)
                {
                    if (CancelPendingSleep(5000))
                    {
                        return false;
                    }
                }
                //WriteLog("[" + (i + 1) + "] Initializing Nike session... ");
                if (_nike.Initialize())
                {
                    break;
                }
                //AppendLog("failed.\r\n");
            }
            //AppendLog("done.\r\n");
            return true;
        }
        private bool routine_Login()
        {
            NetResult result;
            for (int i = 0; ; ++i)
            {
                if (i > 0)
                {
                    if (CancelPendingSleep(1000))
                    {
                        return false;
                    }
                }
                else
                {
                    if (CancelPending())
                    {
                        return false;
                    }
                }
                if (i == 0)
                {

                }
                SetStatus("Logging in");
                result = _nike.Login(NikeUsername, NikePassword, 5000);
                if (result.success)
                {
                    SetStatus("Logged in");
                    return true;
                }
                if (result.message.Contains("<H1>Access Denied</H1>"))
                {
                    SetStatus("Logging in failed. (402 AD)");
                    //WriteLog("Waiting for 10 seconds before trying again.\r\n");
                    if (CancelPendingSleep(10000))
                    {
                        return false;
                    }
                    continue;
                }
                if (result.message.Contains("<h1>Service Temporarily Unavailable</h1>"))
                {
                    SetStatus("Service Unavailable");
                    //WriteLog("Waiting for 5 seconds before trying again.\r\n");
                    if (CancelPendingSleep(5000))
                    {
                        return false;
                    }
                    continue;
                }
                if (result.message.Contains("Your email or password was entered incorrectly"))
                {
                    SetStatus("Invalid Credintails");
                    return false;
                }
                break;
            }
            if (result.message.Contains("DBException"))
            {
                AppendLog("failed. (DBException)\r\n");
            }
            else if (result.message.Contains("processing your request"))
            {
                AppendLog("failed. (nike request error)\r\n");
            }
            else if (result.message == string.Empty)
            {
                AppendLog("failed. (unknown)\r\n");
            }
            else
            {
                AppendLog("failed. (" + result.message + ")\r\n");
            }
            if (!result.success)
            {
                return false;
            }
            SetStatus("Logged in");
            return true;
        }
        private bool routine_DownloadProductData(ref JObject productData, string targetUrl)
        {
            NetResult result;
            for (; ; )
            {
                SetStatus("Downloading product data");
                result = _nike.DownloadProductData(ProductUrl);
                if (CancelPending())
                {
                    return false;
                }
                if (result.success)
                {
                    SetStatus("Downloaded product data");
                    productData = (JObject)result.obj;
                    _productName = productData["displayName"].ToString();
                    return true;
                }
                if (result.message.Contains("NO LONGER AVAILABLE"))
                {
                    SetStatus("Product no longer available");
                }
                else if (result.message.Contains("unexpected HTTP error"))
                {
                    SetStatus("HTTP Error");
                }
                else if (result.message.Contains("Unable to extract JSON"))
                {
                    SetStatus("JSON Error");
                }
                else if (result.message.Contains("parse"))
                {
                    SetStatus("JSON Error");
                }
                if (CancelPendingSleep(2000))
                {
                    return false;
                }
            }
        }

        private bool routine_WaitForCountdown(JObject productData)
        {
            if ((bool)productData["comingSoonCountdownClock"])
            {
                long lRelease = (long)productData["startDate"];
                if (lRelease >= Functions.CurrentTimeMillis())
                {
                    SetStatus("Waiting for countdown");
                    for (; ; Thread.Sleep(50))
                    {
                        if (CancelPending())
                        {
                            AppendLog("failed. (aborted)\r\n");
                            return false;
                        }
                        long lNow = Functions.CurrentTimeMillis();
                        if (lNow >= lRelease) // - 1200)
                        {
                            break;
                        }
                    }
                    SetStatus("Countdown link arived");
                }
                else
                {
                    SetStatus("Countdown link expired");
                }
            }
            else
            {
                //WriteLog("No countdown... (" + ((long)jProduct["startDate"]).ToString() + ")\r\n");
            }
            return true;
        }

        private bool routine_AddToCart(string postBody)
        {
            bool wasSucess;
            WaitInLineReply waitToken;
            if (!routine_InitialAddToCart(postBody, out wasSucess, out waitToken))
            {
                return false;

            }
            if (wasSucess)
            {
                return true;
            }
            SetStatus("Waiting in line");
            if (!routine_AdvancedQueueLoop(postBody, waitToken))
            {
                return false;
            }
            return true;
        }

        private bool routine_AdvancedQueueLoop(string postBody, WaitInLineReply waitToken)
        {
            AddToCartResult result;
            int sleepTime = 0;

            int ewt = Convert.ToInt32(waitToken.ewt);
            if (ewt < 1 || ewt >= 60)
            {
                ewt = 5;
            }
            sleepTime = ewt;

            for (; ; )
            {
                if (CancelPendingSleep(sleepTime * 1000))
                {
                    return false;
                }
                if (NSUserDefaults.StandardUserDefaults.BoolForKey("OverideSleep"))
                {
                    ewt = Convert.ToInt32(NSUserDefaults.StandardUserDefaults.StringForKey("WaitTime"));
                    SetStatus("Wait time overided (" + ewt + "/" + waitToken.ewt + ")");
                    waitToken.ewt = ewt.ToString();
                }
                //sleepTime = 5;
                SetStatus("Waiting in line [" + waitToken.pil + "/" + waitToken.psh + "/" + waitToken.ewt + "]... ");
                result = _nike.AddToCart(ProductUrl, postBody, waitToken.pil, waitToken.psh);
                if (result.Code == AddToCartCode.JsonSuccess)
                {
                    //SetStatus("Product added to cart");
                    return true;
                }
                if (result.Response.Contains("<H1>Access Denied</H1>"))
                {
                    SetStatus("failed. (Access Denied)");
                    continue;
                }
                if (result.Response.Contains("<h1>Service Temporarily Unavailable</h1>"))
                {
                    SetStatus("failed. (Service Unavailable)");
                    continue;
                }
                if (result.Code == AddToCartCode.HttpError)
                {
                    SetStatus("Failed. HTTP Error");
                    continue;
                    //return false;
                }
                if (result.Code == AddToCartCode.JsonInvalid)
                {
                    SetStatus("Failed. JSON Error");
                    continue;
                    //return false;
                }
                if (result.Code == AddToCartCode.JsonFailure)
                {
                    if (result.Response.Contains("systemException"))
                    {
                        AppendLog("Failed. (systemException - Unknown Nike error.)\r\n");
                    }
                    if (result.Response.Contains("OrderAmountLimitExeeded"))
                    {
                        SetStatus("Failed. Order limit exeeded");
                        return false;
                    }
                    else if (result.Response.Contains("errorAddingToOrder"))
                    {
                        SetStatus("Error adding to cart");
                    }
                    else
                    {
                        SetStatus("Adding to cart failed");
                    }
                    continue;
                }
                if (result.Code == AddToCartCode.JsonWait)
                {
                    AppendLog("\r\n");
                    waitToken = (WaitInLineReply)result.Data;

                    ewt = Convert.ToInt32(waitToken.ewt);
                    if (ewt < 1 || ewt >= 60)
                    {
                        ewt = 5;
                    }
                    sleepTime = ewt;
                }
            }
        }

        private bool routine_InitialAddToCart(string postBody, out bool wasSuccess, out WaitInLineReply waitToken)
        {
            wasSuccess = false;
            waitToken = null;

            AddToCartResult result;
            for (int i = 0; ; ++i)
            {
                if (i > 0)
                {
                    if (i < 20)
                    {
                        if (CancelPendingSleep(1000))
                        {
                            //AppendLog("failed. (aborted)\r\n");
                            return false;
                            //bAborted = true;
                            //break;
                        }
                    }
                    else
                    {
                        if (CancelPendingSleep(2000))
                        {
                            //AppendLog("failed. (aborted)\r\n");
                            return false;
                        }
                    }
                }
                SetStatus("Adding to cart");
                result = _nike.AddToCart(ProductUrl, postBody);
                if (result.Code == AddToCartCode.JsonSuccess)
                {
                    wasSuccess = true;
                    //SetStatus("Product added to cart");
                    return true;
                }
                if (result.Response.Contains("<H1>Access Denied</H1>"))
                {
                    SetStatus("failed. (Access Denied)");
                    //WriteLog("Waiting for 15 seconds before trying again.\r\n");
                    if (CancelPendingSleep(15000))
                    {
                        return false;
                    }
                    continue;
                }
                if (result.Response.Contains("<h1>Service Temporarily Unavailable</h1>"))
                {
                    SetStatus("failed. (Service Unavailable)");
                    //WriteLog("Waiting for 5 seconds before trying again.\r\n");
                    if (CancelPendingSleep(5000))
                    {
                        return false;
                    }
                    continue;
                }
                if (result.Code == AddToCartCode.HttpError)
                {
                    SetStatus("failed. HTTP Error");
                    continue;
                }
                if (result.Code == AddToCartCode.JsonInvalid)
                {
                    SetStatus("failed. JSON Error");
                    continue;
                }
                if (result.Code == AddToCartCode.JsonFailure)
                {
                    if (result.Response.Contains("genericATPMessage"))
                    {
                        SetStatus("failed. genericATPMessage");
                    }
                    else if (result.Response.Contains("InvalidItemInCart"))
                    {
                        SetStatus("failed. (InvalidItemInCart)");
                    }
                    else if (result.Response.Contains("systemException"))
                    {
                        SetStatus("failed. (systemException)");
                    }
                    else if (result.Response.Contains("noItemsToAddInStock"))
                    {
                        SetStatus("failed. (noItemsToAddInStock)");
                    }
                    else if (result.Response.Contains("OrderAmountLimitExeeded"))
                    {
                        SetStatus("failed. (OrderAmountLimitExeeded)");
                        return false;
                    }
                    else if (result.Response.Contains("ProductLimitExeeded"))
                    {
                        SetStatus("failed. (ProductLimitExeeded)");
                    }
                    else if (result.Response.Contains("errorAddingToOrder"))
                    {
                        SetStatus("failed. (errorAddingToOrder)");
                    }
                    else
                    {
                        SetStatus("failed.");
                    }
                    continue;
                }
                if (result.Code == AddToCartCode.JsonWait)
                {
                    waitToken = (WaitInLineReply)result.Data;
                    if (waitToken.pil == "-1")
                    {
                        AppendLog("-1 pil\r\n");
                        continue;
                    }
                    SetStatus("Waiting in line. (" + waitToken.pil + "/" + (waitToken.psh ?? "null") + "/" + (waitToken.ewt ?? "null") + ")");
                    return true;
                }
            }
        }

        // Control Events
        private void thread_DoWork()
        {
            //Console.WriteLine("Task thread started");
            routine_Perform();
            // call thread_Completed
            _viewController.BeginInvokeOnMainThread(() =>
            {
                thread_DoWorkCompleted();
            });
        }
        private void thread_DoWorkCompleted()
        {
            _thread.Join();
            lock (_mutex)
            {
                if (_cancellationPending)
                {
                    _cancellationPending = false;
                    IsRunning = false;
                    _isSuccessful = false;
                    _nike = null;
                    WriteLog("Adding to cart failed (user aborted)");
                }
                else if (_isSuccessful)
                {
                    var cookies = _nike.Http.Cookies;
                    //SetStatus("Product added to cart");
                    /*UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                    var alert = MBAlertView.AlertWithBody(
                        body: "",
                        buttonTitle: "Express Checkout",
                        handler: () => _viewController.LoadCheckout(cookies)
                        );
                    alert.AddToDisplayQueue();
                     */
                }
                else
                {
                    UILocalNotification notification = new UILocalNotification();
                    notification.FireDate = DateTime.Now.AddMinutes(1);
                    notification.AlertAction = "Adding to cart error";
                    notification.AlertBody = "Adding to cart failed.";
                    notification.SoundName = UILocalNotification.DefaultSoundName;
                    UIApplication.SharedApplication.ScheduleLocalNotification(notification);
                    WriteLog("Adding to cart failed (internal error)");
                }
                _cancellationPending = false;
                IsRunning = false;
                _isSuccessful = false;
                _nike = null;
                if (_stopRelease)
                {
                    _stopRelease = false;
                    // _TODO
                }
            }
        }
    }
}
