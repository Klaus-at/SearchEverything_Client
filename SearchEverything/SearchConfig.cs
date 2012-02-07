using System;
using System.Collections.Generic;
using System.Text;

namespace SearchEverything
{
    sealed class SearchConfig
    {
        // public static System.Collections.Generic.SortedList<string, Array> PathMappings;
        public static System.Collections.Generic.Dictionary<string, SortedList<string, string>> PathMappings;
        public static bool HideNonMappedFolders;
        public static bool HideNonAccessibleFiles;
        public static System.Collections.Generic.List<string> SearchServerList;
        public static int MaxResultsPerServer;
        public static int MatchCase;
        public static int MatchPath;
        public static int MatchWholeWord;
        public static int MatchRegex;
        public static bool ShowRealIcons;


        static SearchConfig () {
            // Default Settings for local use
            PathMappings = new Dictionary<String, SortedList<string, string>>();
            HideNonMappedFolders = false;
            HideNonAccessibleFiles = false;
            SearchServerList = new List<string>();
            MaxResultsPerServer = 1000;
            MatchCase = 0;
            MatchPath = 0;
            MatchWholeWord = 0;
            MatchRegex = 0;
            ShowRealIcons = true;

            Properties.Settings.Default.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(Settings_PropertyChanged);
            InitConfig();
        }

        static void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "MatchCase": MatchCase = Properties.Settings.Default.MatchCase ? 1 : 0; break;
                case "MatchWholeWord": MatchWholeWord = Properties.Settings.Default.MatchWholeWord ? 1 : 0; break;
                case "MatchPath": MatchPath = Properties.Settings.Default.MatchPath ? 1 : 0; break;
                case "MatchRegex": MatchRegex = Properties.Settings.Default.MatchRegex ? 1 : 0; break;
                case "MaxResultsPerServer": MaxResultsPerServer = Int32.Parse(Properties.Settings.Default.MaxResultsPerServer); break;
                case "ShowRealIcons": ShowRealIcons = Properties.Settings.Default.ShowRealIcons; break;
                default: break;
            }
        }

        public static void SetMaxResultsPerServer(int i)
        {
            MaxResultsPerServer = ( i > 10 && i < 10000) ? i : 1000;
            Properties.Settings.Default.MaxResultsPerServer = MaxResultsPerServer.ToString();
        }

        public static void SetMatchWholeWord()
        {
            MatchWholeWord = 1;
            Properties.Settings.Default.MatchWholeWord = (MatchWholeWord == 1);
        }

        public static void ClearMatchWholeWord()
        {
            MatchWholeWord = 0;
            Properties.Settings.Default.MatchWholeWord = (MatchWholeWord == 1);
        }

        public static void ToggleMatchWholeWord()
        {
            MatchWholeWord = 1 - MatchWholeWord;
            Properties.Settings.Default.MatchWholeWord = (MatchWholeWord == 1);
        }

        public static void SetMatchPath()
        {
            MatchPath = 1;
            Properties.Settings.Default.MatchPath = (MatchPath == 1);
        }

        public static void ClearMatchPath()
        {
            MatchPath = 0;
            Properties.Settings.Default.MatchPath = (MatchPath == 1);
        }

        public static void ToggleMatchPath()
        {
            MatchPath = 1 - MatchPath;
            Properties.Settings.Default.MatchPath = (MatchPath == 1);
        }

        public static void SetMatchCase()
        {
            MatchCase = 1;
            Properties.Settings.Default.MatchCase = (MatchCase == 1);
        }

        public static void ClearMatchCase()
        {
            MatchCase = 0;
            Properties.Settings.Default.MatchCase = (MatchCase == 1);
        }

        public static void ToggleMatchCase()
        {
            MatchCase = 1 - MatchCase;
            Properties.Settings.Default.MatchCase = (MatchCase == 1);
        }


        public static void SetShowRealIcons()
        {
            ShowRealIcons = true;
            Properties.Settings.Default.ShowRealIcons = ShowRealIcons;
        }

        public static void ClearShowRealIcons()
        {
            ShowRealIcons = false;
            Properties.Settings.Default.ShowRealIcons = ShowRealIcons;
        }

        public static void ToggleShowRealIcons()
        {
            ShowRealIcons = !ShowRealIcons;
            Properties.Settings.Default.ShowRealIcons = ShowRealIcons;
        }


        public static void InitConfig() {
            string serverPath;
            string localPath;
            string uri;
            string[] parts;

            try
            {

                HideNonMappedFolders = SearchEverything.Properties.Settings.Default.HideNonMappedFolders;
                HideNonAccessibleFiles = SearchEverything.Properties.Settings.Default.HideInaccessible;
                MatchCase = SearchEverything.Properties.Settings.Default.MatchCase ? 1 : 0;
                MatchWholeWord = SearchEverything.Properties.Settings.Default.MatchWholeWord ? 1 : 0;
                MatchPath = SearchEverything.Properties.Settings.Default.MatchPath ? 1 : 0;
                MatchRegex = SearchEverything.Properties.Settings.Default.MatchRegex ? 1 : 0;
                MaxResultsPerServer = Int32.Parse(SearchEverything.Properties.Settings.Default.MaxResultsPerServer);
                ShowRealIcons = SearchEverything.Properties.Settings.Default.ShowRealIcons;

                foreach (string mappingEntry in SearchEverything.Properties.Settings.Default.PathMappings)
                {
                    SortedList<string, string> map;

                    // skip everything not starting with an etp:// URI
                    if (!mappingEntry.StartsWith("etp://"))
                        continue;

                    parts = mappingEntry.Split(';');
                    uri = parts[0];
                    serverPath = parts[1];
                    localPath = parts[2];
                    if (!serverPath.EndsWith("\\"))
                        serverPath += "\\";
                    if (!localPath.EndsWith("\\"))
                        localPath += "\\";

                    if (!PathMappings.ContainsKey(uri))
                    {
                        PathMappings.Add(uri, new SortedList<string, string>());
                    }
                    map = PathMappings[uri];
                    map.Add(serverPath, localPath);
                }

                foreach (string serverUri in SearchEverything.Properties.Settings.Default.ServerList)
                {
                    if (serverUri.StartsWith("etp://"))
                        SearchServerList.Add(serverUri);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in InitMappings: {0}\n", ex.ToString());
            }
        }
    }
}
