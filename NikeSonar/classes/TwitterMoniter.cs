using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using AlertView;
using Xamarin.Controls;
using Tweetinvi;
using Tweetinvi.Core;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Logic;


namespace NikeSonar
{
    class TwitterMoniter
    {
        private static string _userKey = "";
        private static string _userSecret = "";
        private static string _consumerKey = "";
        private static string _consumerSecret = "";
        private static BotRunningViewController _controller;
        private static IFilteredStream _stream;


        public static void IntalizeTwitter()
        {
            _userKey = NSUserDefaults.StandardUserDefaults.StringForKey("AccessToken");
            _userSecret = NSUserDefaults.StandardUserDefaults.StringForKey("AccessTokenSecret");
            _consumerKey = NSUserDefaults.StandardUserDefaults.StringForKey("ConsumerKey");
            _consumerSecret = NSUserDefaults.StandardUserDefaults.StringForKey("ConsumerSecret");
            TwitterCredentials.Credentials = TwitterCredentials.CreateCredentials(_userKey, _userSecret, _consumerKey, _consumerSecret);
        }

        public static void Start(BotRunningViewController botRunningViewController)
        {
            _controller = botRunningViewController;
            _stream = Tweetinvi.Stream.CreateFilteredStream();
            _stream.AddFollow(Tweetinvi.User.GetUserFromScreenName(NSUserDefaults.StandardUserDefaults.StringForKey("TwitterHandle")));
            _stream.MatchingTweetReceived += new EventHandler<Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs>(Stream_MatchingTweetReceived);
            _stream.StartStreamMatchingAnyConditionAsync();
            _stream.StreamStopped += new EventHandler<Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs>(_stream_StreamStopped);
        }

        static void _stream_StreamStopped(object sender, Tweetinvi.Core.Events.EventArguments.StreamExceptionEventArgs e)
        {
            
        }

        public static void Stop()
        {
            if (_stream != null)
            {
                _stream.StopStream();
                _stream = null;
            }
        }

        public static bool ContainsNegativeKeys(string tweet)
        {
            string a = ViewControllers.BotRunningViewController.NegativeKeywords.ToLower();
            if (a.Contains(' '))
            {
                string[] nKeywords = a.Split(' ');
                foreach (var v in nKeywords)
                {
                    if (tweet.Contains(v))
                    {
                        return true;
                    }
                }
                return false;
            }
            if (tweet.Contains(a))
            {
                return true;
            }
            return false;
        }

        static void Stream_MatchingTweetReceived(object sender, Tweetinvi.Core.Events.EventArguments.MatchedTweetReceivedEventArgs e)
        {
            //Console.WriteLine(e.Tweet.Text);
            string Keyword = ViewControllers.BotRunningViewController.Keywords;
            Keyword = Keyword.ToLower();
            string[] keywords = Keyword.Split(' ');
            string tweet = e.Tweet.Text.ToLower();
            if (tweet.Substring(0, 4).Contains("the"))
            {
                // All the tweets seem to contain "The"
                if (e.Tweet.Urls.Count > 0)
                {
                    // This stage we know the tweet has a url
                    if (!tweet.Contains("drops") || !tweet.Contains("arrives") ||
                        !tweet.Contains("launches"))
                    {
                        // Eliminate pre launch tweets
                        if (ViewControllers.BotRunningViewController.Keywords.Contains(" "))
                        {
                            ;
                            int keywordCount = keywords.Count();
                            int keywordsDetected = 0;
                            foreach (var key in keywords)
                            {
                                if (tweet.Contains(key))
                                {
                                    ++keywordsDetected;
                                }
                            }
                            if (keywordsDetected <= 0)
                            {
                                return;
                            }
                            if (ContainsNegativeKeys(tweet))
                            {
                                return;
                            }
                            // The tweet seems to be valid release
                            _controller.SetTaskUrl(e.Tweet.Urls[0].ExpandedURL);
                            //Console.WriteLine(e.Tweet.Urls[0].ExpandedURL);
                        }
                        else if (tweet.Contains(Keyword))
                        {
                            if (ContainsNegativeKeys(tweet))
                            {
                                return;
                            }
                            _controller.SetTaskUrl(e.Tweet.Urls[0].ExpandedURL);
                            //Console.WriteLine(e.Tweet.Urls[0].ExpandedURL);
                            // The tweet seems to be valid release
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
        }


        public static void ProcessTweet(ITweet tweet)
        {

        }

    }
}
