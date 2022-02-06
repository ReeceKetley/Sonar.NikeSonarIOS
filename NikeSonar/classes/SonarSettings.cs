using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using SlideDownMenu;

namespace NikeSonar
{
    class SonarSettings
    {
        public static List<NikeStoreAccounts> AccountList;
        public static bool TwitterActive;
        public static bool AccountsLoaded;
        private static string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string AccountsPath = Path.Combine(documents.ToString(), "AccountsList.txt");
        public static int MaxAccounts;
        public static bool loggedIn;

        public static void LoadAccounts()
        {
            SonarSettings.AccountList = new List<NikeStoreAccounts>();
            // IOS 8
            if (Environment.OSVersion.VersionString.Contains("8."))
            {
                documents =
                    NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory,
                        NSSearchPathDomain.User)[0].ToString();
                //Console.WriteLine("IOS 8 - Userfile system");
                AccountsPath = Path.Combine(documents, "AccountsList.txt");
            }
            //         
            if (File.Exists(AccountsPath))
            {
                int counter = 0;
                string line;
                System.IO.StreamReader file =
                    new System.IO.StreamReader(AccountsPath);
                while ((line = file.ReadLine()) != null)
                {
                    string[] splitStrings = line.Split(',');
                    NikeStoreAccounts account = new NikeStoreAccounts();
                    account.Id = Convert.ToInt32(splitStrings[0]);
                    account.UserName = splitStrings[1];
                    account.Password = splitStrings[2];
                    account.Size = splitStrings[3];
                    account.Active = Convert.ToBoolean(splitStrings[4]);
                    SonarSettings.AccountList.Add(account);
                    counter++;
                }
                file.Close();
                if (counter <= 0)
                {
                    SonarSettings.AccountsLoaded = false;
                    return;
                }
                SonarSettings.AccountsLoaded = true;
                return;
            }
            SonarSettings.AccountsLoaded = false;
        }

        public static void UpdateAccounts(List<NikeStoreAccounts> accounts)
        {
            // IOS 8
            if (Environment.OSVersion.VersionString.Contains("8."))
            {
                documents =
                    NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory,
                        NSSearchPathDomain.User)[0].ToString();
                //Console.WriteLine("IOS 8 - Userfile system");
                AccountsPath = Path.Combine(documents, "AccountsList.txt");
            }
            //

            if (File.Exists(AccountsPath))
            {
                //Console.WriteLine("File Detected. Deleteing");
                File.Delete(AccountsPath);
            }
            StreamWriter file = new System.IO.StreamWriter(AccountsPath);
            foreach (var account in accounts)
            {
                file.WriteLine(account.Id + "," + account.UserName + "," + account.Password + "," + account.Size + "," + account.Active);
            }
            file.Close();
            LoadAccounts();
        }
    }
}
