namespace DropThing3
{
    partial class DropMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropMain));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.title = new System.Windows.Forms.ToolStripMenuItem();
            this.openItem = new System.Windows.Forms.ToolStripMenuItem();
            this.explorerItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eject = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tabItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settings = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.quit = new System.Windows.Forms.ToolStripMenuItem();
            this.status = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.addTab = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();
            this.missing = new System.Windows.Forms.Label();
            this.faviconFetch = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.resize = new System.Windows.Forms.PictureBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.hamburger = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hamburger)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.title,
            this.openItem,
            this.explorerItem,
            this.propertyItem,
            this.eject,
            this.deleteItem,
            this.toolStripMenuItem1,
            this.tabItem,
            this.settings,
            this.toolStripMenuItem2,
            this.quit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(205, 214);
            // 
            // title
            // 
            this.title.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.title.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.title.ForeColor = System.Drawing.Color.White;
            this.title.Image = ((System.Drawing.Image)(resources.GetObject("title.Image")));
            this.title.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(204, 22);
            this.title.Text = "DropThing";
            // 
            // openItem
            // 
            this.openItem.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.openItem.Name = "openItem";
            this.openItem.ShortcutKeyDisplayString = "ENTER";
            this.openItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openItem.Size = new System.Drawing.Size(204, 22);
            this.openItem.Text = "&Open ...";
            this.openItem.ToolTipText = "Open the file of this cell.";
            this.openItem.Click += new System.EventHandler(this.openItem_Click);
            // 
            // explorerItem
            // 
            this.explorerItem.Name = "explorerItem";
            this.explorerItem.ShortcutKeyDisplayString = "Ctrl+ENTER";
            this.explorerItem.Size = new System.Drawing.Size(204, 22);
            this.explorerItem.Text = "&Explorer ...";
            this.explorerItem.ToolTipText = "Open Explorer and select the file.";
            this.explorerItem.Click += new System.EventHandler(this.explorerItem_Click);
            // 
            // propertyItem
            // 
            this.propertyItem.Name = "propertyItem";
            this.propertyItem.ShortcutKeyDisplayString = "Shift+ENTER";
            this.propertyItem.Size = new System.Drawing.Size(204, 22);
            this.propertyItem.Text = "&Property ...";
            this.propertyItem.ToolTipText = "Open form of CELL settings";
            this.propertyItem.Click += new System.EventHandler(this.propertyItem_Click);
            // 
            // eject
            // 
            this.eject.Name = "eject";
            this.eject.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.J)));
            this.eject.Size = new System.Drawing.Size(204, 22);
            this.eject.Text = "e&Ject";
            this.eject.ToolTipText = "Try to eject removal media.";
            this.eject.Click += new System.EventHandler(this.eject_Click);
            // 
            // deleteItem
            // 
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteItem.Size = new System.Drawing.Size(204, 22);
            this.deleteItem.Text = "&Delete";
            this.deleteItem.ToolTipText = "Unregister the file of cell.  File is not deleted.";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(201, 6);
            // 
            // tabItem
            // 
            this.tabItem.Name = "tabItem";
            this.tabItem.Size = new System.Drawing.Size(204, 22);
            this.tabItem.Text = "&Tab ...";
            this.tabItem.ToolTipText = "Open form of TAB settings.";
            this.tabItem.Click += new System.EventHandler(this.tabItem_Click);
            // 
            // settings
            // 
            this.settings.Enabled = false;
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(204, 22);
            this.settings.Text = "&Settings ...";
            this.settings.ToolTipText = "Open form of DropThing3 settings.";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(201, 6);
            // 
            // quit
            // 
            this.quit.Name = "quit";
            this.quit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quit.Size = new System.Drawing.Size(204, 22);
            this.quit.Text = "e&Xit";
            this.quit.ToolTipText = "Quit DropThing3.";
            this.quit.Click += new System.EventHandler(this.quit_Click);
            // 
            // status
            // 
            this.status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.status.BackColor = System.Drawing.Color.Lime;
            this.status.Cursor = System.Windows.Forms.Cursors.SizeAll;
            this.status.Location = new System.Drawing.Point(0, 89);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(360, 17);
            this.status.TabIndex = 2;
            this.status.Text = "status text";
            this.status.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseDown);
            this.status.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseMove);
            this.status.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // addTab
            // 
            this.addTab.AutoSize = true;
            this.addTab.BackColor = System.Drawing.Color.Blue;
            this.addTab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.addTab.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.addTab.ForeColor = System.Drawing.Color.White;
            this.addTab.Location = new System.Drawing.Point(321, 1);
            this.addTab.Name = "addTab";
            this.addTab.Size = new System.Drawing.Size(22, 25);
            this.addTab.TabIndex = 5;
            this.addTab.Text = "*";
            this.addTab.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.addTab, "Add new tab.");
            this.addTab.UseVisualStyleBackColor = false;
            this.addTab.Click += new System.EventHandler(this.addTab_Click);
            // 
            // grid
            // 
            this.grid.AllowDrop = true;
            this.grid.AllowUserToAddRows = false;
            this.grid.AllowUserToDeleteRows = false;
            this.grid.AllowUserToResizeColumns = false;
            this.grid.AllowUserToResizeRows = false;
            this.grid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grid.BackgroundColor = System.Drawing.Color.Green;
            this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grid.ColumnHeadersVisible = false;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle10;
            this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grid.Location = new System.Drawing.Point(0, 17);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 21;
            this.grid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.grid.Size = new System.Drawing.Size(360, 72);
            this.grid.TabIndex = 0;
            this.grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEnter);
            this.grid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseDown);
            this.grid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            this.grid.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseMove);
            this.grid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseUp);
            this.grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.grid_CellPainting);
            this.grid.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.grid.DragEnter += new System.Windows.Forms.DragEventHandler(this.grid_DragEnter);
            this.grid.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.grid_QueryContinueDrag);
            this.grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_KeyDown);
            // 
            // missing
            // 
            this.missing.AutoSize = true;
            this.missing.Font = new System.Drawing.Font("Arial", 11.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.missing.Location = new System.Drawing.Point(307, 20);
            this.missing.Name = "missing";
            this.missing.Size = new System.Drawing.Size(17, 17);
            this.missing.TabIndex = 8;
            this.missing.Text = "?";
            this.missing.Visible = false;
            // 
            // faviconFetch
            // 
            this.faviconFetch.WorkerReportsProgress = true;
            this.faviconFetch.WorkerSupportsCancellation = true;
            this.faviconFetch.DoWork += new System.ComponentModel.DoWorkEventHandler(this.faviconFetch_DoWork);
            this.faviconFetch.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.faviconFetch_ProgressChanged);
            // 
            // resize
            // 
            this.resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resize.BackColor = System.Drawing.Color.Lime;
            this.resize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resize.Image = ((System.Drawing.Image)(resources.GetObject("resize.Image")));
            this.resize.InitialImage = null;
            this.resize.Location = new System.Drawing.Point(343, 89);
            this.resize.Name = "resize";
            this.resize.Size = new System.Drawing.Size(16, 16);
            this.resize.TabIndex = 6;
            this.resize.TabStop = false;
            this.toolTip1.SetToolTip(this.resize, "resize window");
            this.resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseDown);
            this.resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resize_MouseMove);
            this.resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resize_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(0, -5);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(320, 42);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.tabControl1_DrawItem);
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // hamburger
            // 
            this.hamburger.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.hamburger.Cursor = System.Windows.Forms.Cursors.Hand;
            this.hamburger.ErrorImage = null;
            this.hamburger.Image = ((System.Drawing.Image)(resources.GetObject("hamburger.Image")));
            this.hamburger.Location = new System.Drawing.Point(343, 1);
            this.hamburger.Name = "hamburger";
            this.hamburger.Size = new System.Drawing.Size(16, 16);
            this.hamburger.TabIndex = 7;
            this.hamburger.TabStop = false;
            this.toolTip1.SetToolTip(this.hamburger, "popup menu");
            this.hamburger.Click += new System.EventHandler(this.hamburger_Click);
            // 
            // DropMain
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(360, 106);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.ControlBox = false;
            this.Controls.Add(this.hamburger);
            this.Controls.Add(this.missing);
            this.Controls.Add(this.resize);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.status);
            this.Controls.Add(this.addTab);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropMain";
            this.Text = "DropThing3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DropForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DropForm_FormClosed);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseUp);
            this.Move += new System.EventHandler(this.DropForm_Move);
            this.Resize += new System.EventHandler(this.DropForm_Resize);
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hamburger)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settings;
        private System.Windows.Forms.ToolStripMenuItem quit;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button addTab;
        private System.Windows.Forms.ToolStripMenuItem openItem;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
        private System.Windows.Forms.ToolStripMenuItem explorerItem;
        private System.Windows.Forms.ToolStripMenuItem propertyItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.PictureBox resize;
        private System.Windows.Forms.ToolStripMenuItem tabItem;
        private System.Windows.Forms.Label missing;
        private System.Windows.Forms.ToolStripMenuItem title;
        private System.ComponentModel.BackgroundWorker faviconFetch;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolStripMenuItem eject;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.PictureBox hamburger;
    }
}

