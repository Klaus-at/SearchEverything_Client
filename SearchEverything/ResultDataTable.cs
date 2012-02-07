using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using System.IO;
using Etier.IconHelper;

namespace SearchEverything
{
    public class ResultDataTable : DataTable
    {
        // Icon management
        // http://www.codeproject.com/Articles/2532/Obtaining-and-managing-file-and-folder-icons-using
        private System.Windows.Forms.ImageList _smallImageList = new System.Windows.Forms.ImageList();
        private IconListManager _iconListManager;

        private Image FOLDER_ICON = IconReader.GetFolderIcon(IconReader.IconSize.Small, IconReader.FolderType.Closed).ToBitmap();
        private Image FILE_ICON = IconReader.GetDefaultFileIcon(IconReader.IconSize.Small).ToBitmap();

        public ResultDataTable() : base() {
            InitTable();
        }

        public ResultDataTable(string name) : base(name) {
            InitTable();
        }

        private void InitTable()
        {
            Columns.Add(new DataColumn("Icon", typeof(System.Drawing.Image)));
            Columns.Add(new DataColumn("Name", typeof(String)));
            Columns.Add(new DataColumn("Path", typeof(String)));
            Columns.Add(new DataColumn("Modified", typeof(DateTime)));
            Columns.Add(new DataColumn("Size", typeof(UInt64)));
            Columns.Add(new DataColumn("ServerPath", typeof(String)));  // full serverpath+filename from ETP Server
            Columns.Add(new DataColumn("ServerURI", typeof(Uri)));      // URI of the ETP Server (ftp://server:port)
            Columns.Add(new DataColumn("FileInfo", typeof(System.IO.FileSystemInfo)));
            Columns.Add(new DataColumn("Visible", typeof(bool)));
            PrimaryKey = new DataColumn[] { Columns["ServerURI"], Columns["Serverpath"] };

            _smallImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            _smallImageList.ImageSize = new System.Drawing.Size(16, 16);
            _iconListManager = new IconListManager(_smallImageList, IconReader.IconSize.Small);

        }

           

        public int AddRow(String fileName, String localPath, DateTime lastModified, UInt64 fileSize, String serverPath, Uri serverURI)
        {
            DataRow row = NewRow();
            row.BeginEdit();
            // row["Icon"] =;
            row["Name"] = fileName;
            row["Path"] = localPath;
            row["Modified"] = lastModified;
            row["Size"] = fileSize;
            row["ServerPath"] = serverPath;
            row["ServerURI"] = serverURI;
            row["FileInfo"] = null;
            row["Visible"] = true;
            row.EndEdit();
            Rows.Add(row);
            return 1;
        }

        public void ChangeRowFileInfo(Uri serverURI, String serverPath, FileSystemInfo fInfo)
        {
            DataRow row = Rows.Find(new Object[] { serverURI, serverPath });
            ChangeRowFileInfo(row, fInfo);   
        }

        public void ChangeRowFileInfo(DataRow row, FileSystemInfo fInfo)
        {
            row.BeginEdit();
            try
            {
                row["FileInfo"] = fInfo;
                row["Modified"] = fInfo.LastWriteTime;
                if (fInfo is DirectoryInfo)
                {
                    row["Icon"] = FOLDER_ICON;
                }
                else
                {
                    FileInfo fi = new FileInfo(fInfo.ToString());
                    row["Size"] = fi.Length;
                    if (SearchConfig.ShowRealIcons)
                    {
                        int index = _iconListManager.AddFileIcon(fInfo.FullName);
                        row["Icon"] = _iconListManager.GetImage(index);
                    }
                    else
                    {
                        row["Icon"] = FILE_ICON;
                    }
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                // ignore
            }
            row.EndEdit();
            row.AcceptChanges();
            row.SetModified();
        }
    }
}
