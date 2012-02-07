using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using com.enterprisedt.net.ftp;

namespace SearchEverything
{

    class SingleServerSearch
    {
        private FTPClient srvFTP;
        private Uri myServerUri;
        private string userName = "";
        private string userPassword = "";

        public Uri ServerUri
        {
            get
            {
                return myServerUri;
            }
        }

        public string Hostname
        {
            get
            {
                return myServerUri.Host;
            }
        }

        public int Port
        {
            get
            {
                return myServerUri.Port;
            }
        }

        public string Username
        {
            get
            {
                return userName;
            }
        }

        public string Password
        {
            get
            {
                return userPassword;
            }
        }

        // URI string without user and password
        public string baseUriString
        {
            get
            {
                string s;
                s = myServerUri.Scheme + "://" + Hostname + ":" + Port.ToString() + "/";
                return s;
            }
        }

        public SingleServerSearch(Uri uri)
        {
            InitSingleServer(uri);
        }

        protected void InitSingleServer(Uri uri)
        {
            string user;
            string pwd;
            string[] parts;

            userName = new UriBuilder(uri).UserName;
            userPassword = new UriBuilder(uri).Password;
            myServerUri = uri;
        }


        public bool Connect() {
            string user;
            bool loggedIn = false;
            try
            {
                srvFTP = new FTPClient(Hostname, Port);
                srvFTP.ConnectMode = FTPConnectMode.PASV;
                user = Username == "" ? "etp" : Username;
                srvFTP.Login(user, Password);
                loggedIn = (srvFTP.LastValidReply.ReplyCode == "230"); // login successful ?
            }
            catch (Exception ex)
            {

            }
            return loggedIn;
        }

        // TODO: ?
        public ResultDataTable QueryNew(String search)
        {
            // TODO: Umschreiben von ServerPathen auf lokale Pfade
            List<String> qryResult = QueryRaw(search);
            ResultDataTable result = new ResultDataTable();

            foreach (String srvPath in qryResult)
            {

            }

            return result;
        }

        public String[] Query(String search)
        {
            List<String> qryResult = QueryRaw(search);
            return qryResult.ToArray();
        }


        protected List<String> QueryRaw(String search) {
            List<String> result = new List<string>();
            FTPReply reply;
            string[] files;

            // try reconnect
            if (!srvFTP.IsConnected())
                Connect();

            // failed
            if (!srvFTP.IsConnected())
                return result;
            
            srvFTP.Query(search, 0, SearchConfig.MatchCase, SearchConfig.MatchWholeWord, SearchConfig.MatchPath, SearchConfig.MaxResultsPerServer);
            reply = srvFTP.LastValidReply;

            files = reply.ReplyText.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            result.AddRange(files);

            return result;
        }

        public void Close()
        {
            try
            {
                srvFTP.Quit();
            }
            catch (Exception ex)
            {

            }
        }
    }
}
