namespace SearchEverything
{
    partial class SearchForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblSearch = new System.Windows.Forms.Label();
            this.tbSearchString = new System.Windows.Forms.TextBox();
            this.gvSearchResults = new System.Windows.Forms.DataGridView();
            this.colIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colModified = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSize = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colServerPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colServerURI = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.resultDataTableBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.statusBarSearch = new System.Windows.Forms.StatusStrip();
            this.lblMatchcount = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.progbarSearch = new System.Windows.Forms.ToolStripProgressBar();
            this.bgWorkerFileInfo = new System.ComponentModel.BackgroundWorker();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.menuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemClose = new System.Windows.Forms.ToolStripMenuItem();
            this.menuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMatchCase = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMatchWholeWord = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMatchPath = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemShowRealIcons = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.menuItemMaxMatchesLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuItemMaxResultsPerServer = new System.Windows.Forms.ToolStripTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.gvSearchResults)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultDataTableBindingSource)).BeginInit();
            this.statusBarSearch.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSearch
            // 
            this.lblSearch.AutoSize = true;
            this.lblSearch.Location = new System.Drawing.Point(8, 30);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(44, 13);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "Search:";
            // 
            // tbSearchString
            // 
            this.tbSearchString.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tbSearchString.Location = new System.Drawing.Point(55, 27);
            this.tbSearchString.Name = "tbSearchString";
            this.tbSearchString.Size = new System.Drawing.Size(791, 20);
            this.tbSearchString.TabIndex = 1;
            this.tbSearchString.TextChanged += new System.EventHandler(this.tbSearchString_TextChanged);
            // 
            // gvSearchResults
            // 
            this.gvSearchResults.AllowUserToAddRows = false;
            this.gvSearchResults.AllowUserToDeleteRows = false;
            this.gvSearchResults.AllowUserToOrderColumns = true;
            this.gvSearchResults.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gvSearchResults.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.gvSearchResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gvSearchResults.AutoGenerateColumns = false;
            this.gvSearchResults.BackgroundColor = System.Drawing.Color.White;
            this.gvSearchResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvSearchResults.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colIcon,
            this.colName,
            this.colPath,
            this.colModified,
            this.colSize,
            this.colServerPath,
            this.colServerURI,
            this.colVisible});
            this.gvSearchResults.DataSource = this.resultDataTableBindingSource;
            this.gvSearchResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.gvSearchResults.Location = new System.Drawing.Point(12, 53);
            this.gvSearchResults.MinimumSize = new System.Drawing.Size(500, 200);
            this.gvSearchResults.MultiSelect = false;
            this.gvSearchResults.Name = "gvSearchResults";
            this.gvSearchResults.ReadOnly = true;
            this.gvSearchResults.RowHeadersWidth = 15;
            this.gvSearchResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gvSearchResults.ShowCellErrors = false;
            this.gvSearchResults.ShowEditingIcon = false;
            this.gvSearchResults.ShowRowErrors = false;
            this.gvSearchResults.Size = new System.Drawing.Size(834, 463);
            this.gvSearchResults.TabIndex = 2;
            this.gvSearchResults.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvSearchResults_CellDoubleClick);
            this.gvSearchResults.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.gvSearchResults_CellContextMenuStripNeeded);
            this.gvSearchResults.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvSearchResults_DataError);
            // 
            // colIcon
            // 
            this.colIcon.DataPropertyName = "Icon";
            this.colIcon.HeaderText = "";
            this.colIcon.MinimumWidth = 20;
            this.colIcon.Name = "colIcon";
            this.colIcon.ReadOnly = true;
            this.colIcon.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colIcon.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colIcon.Width = 20;
            // 
            // colName
            // 
            this.colName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colName.DataPropertyName = "Name";
            this.colName.HeaderText = global::SearchEverything.Properties.Resources.FileNameText;
            this.colName.MinimumWidth = 200;
            this.colName.Name = "colName";
            this.colName.ReadOnly = true;
            // 
            // colPath
            // 
            this.colPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPath.DataPropertyName = "Path";
            this.colPath.HeaderText = global::SearchEverything.Properties.Resources.PathNameText;
            this.colPath.Name = "colPath";
            this.colPath.ReadOnly = true;
            // 
            // colModified
            // 
            this.colModified.DataPropertyName = "Modified";
            this.colModified.HeaderText = global::SearchEverything.Properties.Resources.LastModifiedText;
            this.colModified.MinimumWidth = 100;
            this.colModified.Name = "colModified";
            this.colModified.ReadOnly = true;
            // 
            // colSize
            // 
            this.colSize.DataPropertyName = "Size";
            this.colSize.HeaderText = global::SearchEverything.Properties.Resources.FileSizeText;
            this.colSize.MinimumWidth = 100;
            this.colSize.Name = "colSize";
            this.colSize.ReadOnly = true;
            // 
            // colServerPath
            // 
            this.colServerPath.DataPropertyName = "ServerPath";
            this.colServerPath.HeaderText = global::SearchEverything.Properties.Resources.ServerPathText;
            this.colServerPath.Name = "colServerPath";
            this.colServerPath.ReadOnly = true;
            this.colServerPath.Visible = false;
            // 
            // colServerURI
            // 
            this.colServerURI.DataPropertyName = "ServerURI";
            this.colServerURI.HeaderText = global::SearchEverything.Properties.Resources.ServerURIText;
            this.colServerURI.Name = "colServerURI";
            this.colServerURI.ReadOnly = true;
            this.colServerURI.Visible = false;
            // 
            // colVisible
            // 
            this.colVisible.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colVisible.DataPropertyName = "Visible";
            this.colVisible.HeaderText = global::SearchEverything.Properties.Resources.RowVisibleText;
            this.colVisible.Name = "colVisible";
            this.colVisible.ReadOnly = true;
            this.colVisible.Visible = false;
            // 
            // resultDataTableBindingSource
            // 
            this.resultDataTableBindingSource.DataSource = typeof(SearchEverything.ResultDataTable);
            // 
            // statusBarSearch
            // 
            this.statusBarSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblMatchcount,
            this.lblStatusText,
            this.progbarSearch});
            this.statusBarSearch.Location = new System.Drawing.Point(0, 519);
            this.statusBarSearch.Name = "statusBarSearch";
            this.statusBarSearch.Size = new System.Drawing.Size(858, 22);
            this.statusBarSearch.TabIndex = 3;
            this.statusBarSearch.Text = "statusStrip1";
            // 
            // lblMatchcount
            // 
            this.lblMatchcount.AutoSize = false;
            this.lblMatchcount.Name = "lblMatchcount";
            this.lblMatchcount.Size = new System.Drawing.Size(50, 17);
            this.lblMatchcount.Text = "0";
            this.lblMatchcount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblStatusText
            // 
            this.lblStatusText.AutoSize = false;
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lblStatusText.Size = new System.Drawing.Size(691, 17);
            this.lblStatusText.Spring = true;
            // 
            // progbarSearch
            // 
            this.progbarSearch.ForeColor = System.Drawing.Color.Red;
            this.progbarSearch.Name = "progbarSearch";
            this.progbarSearch.Size = new System.Drawing.Size(100, 16);
            this.progbarSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // bgWorkerFileInfo
            // 
            this.bgWorkerFileInfo.WorkerReportsProgress = true;
            this.bgWorkerFileInfo.WorkerSupportsCancellation = true;
            this.bgWorkerFileInfo.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorkerFileInfo_DoWork);
            this.bgWorkerFileInfo.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorkerFileInfo_RunWorkerCompleted);
            this.bgWorkerFileInfo.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorkerFileInfo_ProgressChanged);
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuFile,
            this.menuOptions});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(858, 24);
            this.menuStripMain.TabIndex = 4;
            this.menuStripMain.Text = "menuStripMain";
            // 
            // menuFile
            // 
            this.menuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemClose});
            this.menuFile.Name = "menuFile";
            this.menuFile.Size = new System.Drawing.Size(35, 20);
            this.menuFile.Text = global::SearchEverything.Properties.Resources.menuItemFile;
            // 
            // menuItemClose
            // 
            this.menuItemClose.Name = "menuItemClose";
            this.menuItemClose.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
            this.menuItemClose.Size = new System.Drawing.Size(151, 22);
            this.menuItemClose.Text = global::SearchEverything.Properties.Resources.menuItemClose;
            this.menuItemClose.Click += new System.EventHandler(this.beendenToolStripMenuItem_Click);
            // 
            // menuOptions
            // 
            this.menuOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMatchCase,
            this.menuItemMatchWholeWord,
            this.menuItemMatchPath,
            this.menuItemShowRealIcons,
            this.toolStripSeparator1,
            this.menuItemMaxMatchesLabel});
            this.menuOptions.Name = "menuOptions";
            this.menuOptions.Size = new System.Drawing.Size(56, 20);
            this.menuOptions.Text = global::SearchEverything.Properties.Resources.menuItemOptions;
            // 
            // menuItemMatchCase
            // 
            this.menuItemMatchCase.Checked = global::SearchEverything.Properties.Settings.Default.MatchCase;
            this.menuItemMatchCase.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemMatchCase.Name = "menuItemMatchCase";
            this.menuItemMatchCase.Size = new System.Drawing.Size(197, 22);
            this.menuItemMatchCase.Text = global::SearchEverything.Properties.Resources.menuItemMatchCase;
            this.menuItemMatchCase.Click += new System.EventHandler(this.menuItemMatchCase_Click);
            // 
            // menuItemMatchWholeWord
            // 
            this.menuItemMatchWholeWord.Checked = global::SearchEverything.Properties.Settings.Default.MatchWholeWord;
            this.menuItemMatchWholeWord.Name = "menuItemMatchWholeWord";
            this.menuItemMatchWholeWord.Size = new System.Drawing.Size(197, 22);
            this.menuItemMatchWholeWord.Text = global::SearchEverything.Properties.Resources.menuItemMatchWholeWord;
            this.menuItemMatchWholeWord.Click += new System.EventHandler(this.menuItemMatchWholeWord_Click);
            // 
            // menuItemMatchPath
            // 
            this.menuItemMatchPath.Checked = global::SearchEverything.Properties.Settings.Default.MatchPath;
            this.menuItemMatchPath.Enabled = false;
            this.menuItemMatchPath.Name = "menuItemMatchPath";
            this.menuItemMatchPath.Size = new System.Drawing.Size(197, 22);
            this.menuItemMatchPath.Text = global::SearchEverything.Properties.Resources.menuItemMatchPath;
            this.menuItemMatchPath.Click += new System.EventHandler(this.menuItemMatchPath_Click);
            // 
            // menuItemShowRealIcons
            // 
            this.menuItemShowRealIcons.Checked = global::SearchEverything.Properties.Settings.Default.ShowRealIcons;
            this.menuItemShowRealIcons.CheckState = System.Windows.Forms.CheckState.Checked;
            this.menuItemShowRealIcons.Name = "menuItemShowRealIcons";
            this.menuItemShowRealIcons.Size = new System.Drawing.Size(197, 22);
            this.menuItemShowRealIcons.Text = global::SearchEverything.Properties.Resources.menuItemShowRealIcons;
            this.menuItemShowRealIcons.Click += new System.EventHandler(this.menuItemShowRealIcons_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // menuItemMaxMatchesLabel
            // 
            this.menuItemMaxMatchesLabel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuItemMaxResultsPerServer});
            this.menuItemMaxMatchesLabel.Name = "menuItemMaxMatchesLabel";
            this.menuItemMaxMatchesLabel.Size = new System.Drawing.Size(197, 22);
            this.menuItemMaxMatchesLabel.Text = global::SearchEverything.Properties.Resources.menuItemMaxHitsPerServer;
            // 
            // menuItemMaxResultsPerServer
            // 
            this.menuItemMaxResultsPerServer.AutoSize = false;
            this.menuItemMaxResultsPerServer.MaxLength = 5;
            this.menuItemMaxResultsPerServer.Name = "menuItemMaxResultsPerServer";
            this.menuItemMaxResultsPerServer.Size = new System.Drawing.Size(152, 21);
            this.menuItemMaxResultsPerServer.Text = global::SearchEverything.Properties.Settings.Default.MaxResultsPerServer;
            this.menuItemMaxResultsPerServer.Leave += new System.EventHandler(this.menuItemMaxResultsPerServer_Leave);
            this.menuItemMaxResultsPerServer.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.menuItemMaxResultsPerServer_KeyPress);
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(858, 541);
            this.Controls.Add(this.statusBarSearch);
            this.Controls.Add(this.menuStripMain);
            this.Controls.Add(this.gvSearchResults);
            this.Controls.Add(this.tbSearchString);
            this.Controls.Add(this.lblSearch);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "SearchForm";
            this.Text = "Filesearch";
            this.Activated += new System.EventHandler(this.SearchForm_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SearchForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.gvSearchResults)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resultDataTableBindingSource)).EndInit();
            this.statusBarSearch.ResumeLayout(false);
            this.statusBarSearch.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox tbSearchString;
        private System.Windows.Forms.DataGridView gvSearchResults;
        private System.Windows.Forms.BindingSource resultDataTableBindingSource;
        private System.Windows.Forms.StatusStrip statusBarSearch;
        private System.Windows.Forms.ToolStripStatusLabel lblMatchcount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colModified;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSize;
        private System.Windows.Forms.DataGridViewTextBoxColumn colServerPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colServerURI;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.ToolStripProgressBar progbarSearch;
        private System.ComponentModel.BackgroundWorker bgWorkerFileInfo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem menuFile;
        private System.Windows.Forms.ToolStripMenuItem menuItemClose;
        private System.Windows.Forms.ToolStripMenuItem menuOptions;
        private System.Windows.Forms.ToolStripMenuItem menuItemMatchCase;
        private System.Windows.Forms.ToolStripMenuItem menuItemMatchWholeWord;
        private System.Windows.Forms.ToolStripMenuItem menuItemMatchPath;
        private System.Windows.Forms.ToolStripMenuItem menuItemMaxMatchesLabel;
        private System.Windows.Forms.ToolStripTextBox menuItemMaxResultsPerServer;
        private System.Windows.Forms.DataGridViewImageColumn colIcon;
        private System.Windows.Forms.ToolStripMenuItem menuItemShowRealIcons;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;

    }
}

