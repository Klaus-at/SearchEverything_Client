using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;

namespace SearchEverything
{
    public partial class SearchForm : Form
    {
        // sync Background cancellation
        private AutoResetEvent _cancelWorkerEvent = new AutoResetEvent(false);

        private const int INPUT_DELAY = 500; // delay in ms after last input, before search happens

        private const string DEFAULT_SORT = "Visible ASC, Name ASC";


        private System.Windows.Forms.Timer InputTimer;

        // protected ResultDataTable ResultTable;
        protected SearchServerList ServerList;


        public SearchForm()
        {
            DataView ResultsVisibleView;
            InitializeComponent();
            
            ServerList = new SearchServerList();
            // gvSearchResults.DataSource = ServerList.ResultsTable;
            ResultsVisibleView = new DataView(ServerList.ResultsTable, "Visible = 1","",DataViewRowState.CurrentRows);

            gvSearchResults.DataSource = ResultsVisibleView;

            foreach (string uriString in SearchConfig.SearchServerList)
            {
                if (ServerList.AddServer(uriString) == -1)
                {
                    NotifyStatus(String.Format(Properties.Resources.infoConnectionToServerFailed_arg1, uriString));
                }
            }

            InputTimer = new System.Windows.Forms.Timer();
            InputTimer.Interval = INPUT_DELAY;
            InputTimer.Tick += new EventHandler(InputTimer_Tick);

        }

        private void InputTimer_Tick(Object myObject, EventArgs myEventArgs)
        {
            InputTimer.Stop();
            StartSearch();
        }

        private void tbSearchString_TextChanged(object sender, EventArgs e)
        {
            if (bgWorkerFileInfo.IsBusy)
            {
                bgWorkerFileInfo.CancelAsync();
            }

            lblStatusText.Text = "";
            InputTimer.Stop();
            InputTimer.Interval = INPUT_DELAY;
            InputTimer.Start();
        }

        private void gvSearchResults_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            Peter.ShellContextMenu ctxMenu = new Peter.ShellContextMenu();
            FileInfo[] arrFI = new FileInfo[1];
            DataGridViewRow curRow;
            String filePath;

            if (e.RowIndex >= 0 && gvSearchResults.Rows.Count>0)
            {
                curRow = gvSearchResults.Rows[e.RowIndex];
                filePath = (String)curRow.Cells["colPath"].Value + "\\" + (String)curRow.Cells["colName"].Value;
                arrFI[0] = new FileInfo(filePath);
                ctxMenu.ShowContextMenu(arrFI, MousePosition);
            }
        }

        private void gvSearchResults_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            String filePath;
            DataGridViewRow curRow;

            if (e.RowIndex >= 0 && gvSearchResults.Rows.Count > 0)
            {
                ProcessStartInfo procStartInfo = new ProcessStartInfo();
                curRow = gvSearchResults.Rows[e.RowIndex];
                filePath = (String)curRow.Cells["colPath"].Value + "\\" + (String)curRow.Cells["colName"].Value;

                procStartInfo.FileName = filePath;
                procStartInfo.WorkingDirectory = (String)curRow.Cells["colPath"].Value;
                procStartInfo.UseShellExecute = true;
                Process.Start(procStartInfo);
            }
        }


        private void SearchForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanUp();
        }

        private void gvSearchResults_DataError(object sender, DataGridViewDataErrorEventArgs anError)
        {
            MessageBox.Show("Error happened " + anError.Context.ToString());

            if (anError.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Commit error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.CurrentCellChange)
            {
                MessageBox.Show("Cell change");
            }
            if (anError.Context == DataGridViewDataErrorContexts.Parsing)
            {
                MessageBox.Show("parsing error");
            }
            if (anError.Context == DataGridViewDataErrorContexts.LeaveControl)
            {
                MessageBox.Show("leave control error");
            }

            if ((anError.Exception) is ConstraintException)
            {
                DataGridView view = (DataGridView)sender;
                view.Rows[anError.RowIndex].ErrorText = "an error";
                view.Rows[anError.RowIndex].Cells[anError.ColumnIndex].ErrorText = "an error";

                anError.ThrowException = false;
            }
        }

        #region Background Worker
        private void bgWorkerFileInfo_DoWork(object sender, DoWorkEventArgs e)
        {
            ResultDataTable ResultsTable;
            int cnt;
            int  progress = 0;
            int percentStep;

            // Console.WriteLine("DoWork: event={0} arg={1} res={2}", e.ToString(), e.Argument, e.Result);
            e.Result = new Int32();
            if ((e.Argument != null) && (e.Argument is ResultDataTable))
                ResultsTable = (ResultDataTable)(e.Argument);
            else
            {
                e.Cancel = true;
                return;
            }

            try
            {
                cnt = ResultsTable.Rows.Count;
                percentStep = (int) (cnt / 100);
                if (percentStep<1)
                    percentStep = 1;
                foreach (DataRow row in ResultsTable.Rows)
                {
                    if (this.bgWorkerFileInfo.CancellationPending)
                    {
                        e.Cancel = true;
                        throw new Exception("BackgroundWorker cancellation initiated");
                    }

                    if (progress % percentStep == 0)
                    {
                        this.bgWorkerFileInfo.ReportProgress(progress * 100 / cnt);
                    }
                    string localFile = "";

                    object fi = row["FileInfo"];
                    if (fi == null || fi is DBNull)
                    {
                        try
                        {
                            FileSystemInfo fInfo;
                            localFile = row["Path"] + "\\" + row["Name"];
                            if ((File.GetAttributes(localFile) & FileAttributes.Directory) != 0)
                                fInfo = new DirectoryInfo(localFile);
                            else
                                fInfo = new FileInfo(localFile);

                            ResultsTable.ChangeRowFileInfo(row, fInfo);
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            if (SearchConfig.HideNonAccessibleFiles)
                                row["Visible"] = false;
                            // Console.WriteLine("Unauth access on {0}", localFile);
                        }
                        catch (FileNotFoundException ex)
                        {
                            if (SearchConfig.HideNonAccessibleFiles)
                                row["Visible"] = false;
                            // Console.WriteLine("Not Found on {0}", localFile);
                        }
                        catch (AccessViolationException ex)
                        {
                            if (SearchConfig.HideNonAccessibleFiles)
                                row["Visible"] = false;
                            // Console.WriteLine("Access Violation on {0}", localFile);
                        }
                        catch (Exception ex)
                        {
                            row["Visible"] = false;
                            // Program.MainForm.NotifyError(ex);
                        }
                    }

                    progress++;
                }
            }
            catch (Exception ex)
            {
                this.bgWorkerFileInfo.ReportProgress(100, ex);
            }
            finally
            {
                // bgWorkerFileInfo.ReportProgress(100);
            }
            e.Result = 1;
            if (this.bgWorkerFileInfo.CancellationPending) // confirm cancellation
                _cancelWorkerEvent.Set();
        }

        private void bgWorkerFileInfo_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (sender != bgWorkerFileInfo)
                return;

            progbarSearch.Value = e.ProgressPercentage;
            if (progbarSearch.Value < 100)
                progbarSearch.ForeColor = Color.Red;
            else
                progbarSearch.ForeColor = Color.SeaGreen;

            if (e.UserState is Exception)
            {
                NotifyError((Exception) e.UserState);
            }
        }

        private void bgWorkerFileInfo_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (sender != bgWorkerFileInfo)
                return;

            if (!e.Cancelled)
            {
                progbarSearch.ForeColor = Color.SeaGreen;
                progbarSearch.Value = 100;
                RefreshView();
            }
        }
        #endregion

        private void StartSearch()
        {
            lock (this)
            {
                if (bgWorkerFileInfo.IsBusy)
                {
                    _cancelWorkerEvent.WaitOne(); /* wait for cancellation confirm */
                }

                gvSearchResults.DataSource = new DataView(ServerList.ResultsTable, "Visible = 1", "", DataViewRowState.CurrentRows);
                ServerList.QueryServersSynchronous(tbSearchString.Text);

                lblMatchcount.Text = ServerList.ResultsTable.Rows.Count.ToString();

                /* old BGWorker is already cancelled, start a new one */
                this.bgWorkerFileInfo = new BackgroundWorker();
                this.bgWorkerFileInfo.WorkerReportsProgress = true;
                this.bgWorkerFileInfo.WorkerSupportsCancellation = true;
                this.bgWorkerFileInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerFileInfo_DoWork);
                this.bgWorkerFileInfo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerFileInfo_RunWorkerCompleted);
                this.bgWorkerFileInfo.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerFileInfo_ProgressChanged);

                lblStatusText.Text = Properties.Resources.infoFetchingFileDetails;
                bgWorkerFileInfo.RunWorkerAsync(ServerList.ResultsTable);
            }
        }

        public void RefreshView()
        {
            //DataView ResultsVisibleView = new DataView(ServerList.ResultsTable, "Visible = 1", DEFAULT_SORT, DataViewRowState.CurrentRows);
            //gvSearchResults.DataSource = ResultsVisibleView;
            lblStatusText.Text = "";
        }

        public void NotifyStatus(string txt)
        {
            lblStatusText.Text = txt;
            lblStatusText.Image = null;
        }

        public void NotifyError(Exception ex)
        {
            lblStatusText.Text = ex.Message;
        }

        private void CleanUp()
        {
            InputTimer.Stop();
            bgWorkerFileInfo.CancelAsync();
            gvSearchResults.DataSource = null;
            ServerList.Close();
        }

        ~SearchForm()
        {
        }



        #region Menu
        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void menuItemMatchCase_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                SearchConfig.ToggleMatchCase();
                menuItemMatchCase.Checked = !menuItemMatchCase.Checked;
            }
        }

        private void menuItemMatchWholeWord_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                SearchConfig.ToggleMatchWholeWord();
                menuItemMatchWholeWord.Checked = !menuItemMatchWholeWord.Checked;
            }
        }

        private void menuItemMatchPath_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                SearchConfig.ToggleMatchPath();
                menuItemMatchPath.Checked = !menuItemMatchPath.Checked;
            }
        }

        /* only numbers and control chars allowed */
        private void menuItemMaxResultsPerServer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), "\\d+"))
                e.Handled = true;
        }

        private void menuItemShowRealIcons_Click(object sender, EventArgs e)
        {
            lock (this)
            {
                SearchConfig.ToggleShowRealIcons();
                menuItemShowRealIcons.Checked = !menuItemShowRealIcons.Checked;
            }
        }
        #endregion

        private void menuItemMaxResultsPerServer_Leave(object sender, EventArgs e)
        {
            lock (this)
            {
                int max = Int32.Parse(menuItemMaxResultsPerServer.Text);
                SearchConfig.SetMaxResultsPerServer(max);
                if (max != SearchConfig.MaxResultsPerServer) // outside of allowed range, change back
                    menuItemMaxResultsPerServer.Text = SearchConfig.MaxResultsPerServer.ToString();
            }
        }

    }
}