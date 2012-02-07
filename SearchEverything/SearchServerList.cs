using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data;

namespace SearchEverything
{
    public class SearchServerList : System.Collections.SortedList
    {
        public ResultDataTable ResultsTable;

        public SearchServerList()
            : base(10)
        {
            ResultsTable = new ResultDataTable("SearchResults");
        }

        // returns the index of the new server
        public int AddServer(string hostname, int port)
        {
            Uri uri = new UriBuilder("etp", hostname, port).Uri;
            return AddServer(uri);
        }

        public int AddServer(string uriString)
        {
            Uri uri = new UriBuilder(uriString).Uri;
            return AddServer(uri);
        }

        public int AddServer(Uri serverUri)
        {
            SingleServerSearch s;

            // uri already in list
            if (this.ContainsKey(serverUri.ToString()))
                return 0;

            s = new SingleServerSearch(serverUri);
            if (s.Connect())
            {
                Add(serverUri.ToString(), s);
                return IndexOfKey(serverUri.ToString());
            }
            else
            {
                return -1;
            }
        }

        public void QueryServersSynchronous(string query)
        {
            try
            {
                    ResultsTable.Clear();
                    for (int i = 0; i < this.Count; i++)
                        QueryServer(i, query);

            }
            catch (Exception ex)
            {
            }
        }

        protected void QueryServer(int idx, string query)
        {
            SingleServerSearch srv;
            string[] files;
            bool mapped = false;

            srv = (SingleServerSearch) GetByIndex(idx);
            files = srv.Query(query);

            // i=1: first element is part of FTP status, skip
            for (int i = 1; i < files.Length; i++)
            {
                string filepath;
                string localPath;
                string serverPath;
                string fName;

                filepath = files[i];
                // first attempt with full URI (including user+pass), if unsuccessful second pass with base URI
                mapped = SplitAndMapServerpath(filepath, srv.ServerUri.ToString(), out fName, out localPath, out serverPath);
                if (!mapped)
                {
                    mapped = SplitAndMapServerpath(filepath, srv.baseUriString, out fName, out localPath, out serverPath);
                }
                
                try
                {
                    if ( !(localPath.StartsWith("\\\\") && SearchConfig.HideNonMappedFolders) )
                        ResultsTable.AddRow(fName, localPath, DateTime.Now, 0, filepath, srv.ServerUri);
                }
                catch (Exception ex)
                {
                    ResultsTable.AddRow("####error####", ex.ToString(), DateTime.Now, 0, filepath, srv.ServerUri);
                }
            }
        }


        // splits the full file path from the server and maps the directory to a locally availabe one, using PathMappings
        public bool SplitAndMapServerpath(string serverFilePath, string uriStr, out string fName, out string localPath, out string serverPath)
        {
            string[] parts;
            SortedList<string, string> serverMaps;
            bool success = false;

            if (SearchConfig.PathMappings.ContainsKey(uriStr)) 
                serverMaps = SearchConfig.PathMappings[uriStr];
            else
                serverMaps = new SortedList<string,string>();

            parts = serverFilePath.Split(';');

            fName = Path.GetFileName(serverFilePath);
            serverPath = Path.GetDirectoryName(serverFilePath);
            localPath = "\\\\" + new Uri(uriStr).Host + "\\" + serverPath.Substring(0,1) + "$" + serverPath.Substring(2); // default:  \\<urihost>\<Driveletter>$\<path>

            foreach (string sPath in serverMaps.Keys)
            {
                if (serverPath.StartsWith(sPath))
                {
                    localPath = serverPath.Replace(sPath, serverMaps[sPath]);
                    // first matching mapping is used!!
                    success = true;
                    break;
                }
            }

            return success;
        }


        public void Close()
        {
            foreach (SingleServerSearch s in this.Values)
            {
                // SingleServerSearch s = (SingleServerSearch) entry;
                try
                {
                    s.Close();
                }
                catch (Exception ex)
                {

                }
            }
            this.Clear();

        }
    }
}
