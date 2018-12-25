namespace DropThing3
{
    partial class DropForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DropForm));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exprolerItem = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.layerMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.addLayerToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.propertyLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settings = new System.Windows.Forms.ToolStripMenuItem();
            this.exportSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.quit = new System.Windows.Forms.ToolStripMenuItem();
            this.status = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.button2 = new System.Windows.Forms.Button();
            this.grid = new System.Windows.Forms.DataGridView();
            this.hamburger = new System.Windows.Forms.PictureBox();
            this.resize = new System.Windows.Forms.PictureBox();
            this.missing = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hamburger)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).BeginInit();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openItem,
            this.exprolerItem,
            this.propertyItem,
            this.deleteItem,
            this.toolStripMenuItem1,
            this.layerMenu,
            this.settings,
            this.exportSettingsToolStripMenuItem,
            this.importSettingsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.quit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(166, 214);
            // 
            // openItem
            // 
            this.openItem.Name = "openItem";
            this.openItem.ShortcutKeyDisplayString = "[Enter]";
            this.openItem.Size = new System.Drawing.Size(165, 22);
            this.openItem.Text = "&Open ...";
            this.openItem.Click += new System.EventHandler(this.openItem_Click);
            // 
            // exprolerItem
            // 
            this.exprolerItem.Name = "exprolerItem";
            this.exprolerItem.Size = new System.Drawing.Size(165, 22);
            this.exprolerItem.Text = "&Exproler ...";
            this.exprolerItem.Click += new System.EventHandler(this.exprolerItem_Click);
            // 
            // propertyItem
            // 
            this.propertyItem.Name = "propertyItem";
            this.propertyItem.Size = new System.Drawing.Size(165, 22);
            this.propertyItem.Text = "&Property ...";
            this.propertyItem.Click += new System.EventHandler(this.propertyItem_Click);
            // 
            // deleteItem
            // 
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteItem.Size = new System.Drawing.Size(165, 22);
            this.deleteItem.Text = "&Delete";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(162, 6);
            // 
            // layerMenu
            // 
            this.layerMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addLayerToolStripMenuItem1,
            this.propertyLayer,
            this.deleteToolStripMenuItem});
            this.layerMenu.Name = "layerMenu";
            this.layerMenu.Size = new System.Drawing.Size(165, 22);
            this.layerMenu.Text = "&Layer";
            // 
            // addLayerToolStripMenuItem1
            // 
            this.addLayerToolStripMenuItem1.Name = "addLayerToolStripMenuItem1";
            this.addLayerToolStripMenuItem1.Size = new System.Drawing.Size(119, 22);
            this.addLayerToolStripMenuItem1.Text = "Add";
            // 
            // propertyLayer
            // 
            this.propertyLayer.Name = "propertyLayer";
            this.propertyLayer.Size = new System.Drawing.Size(119, 22);
            this.propertyLayer.Text = "Property";
            this.propertyLayer.Click += new System.EventHandler(this.propertyLayer_Click);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // settings
            // 
            this.settings.Name = "settings";
            this.settings.Size = new System.Drawing.Size(165, 22);
            this.settings.Text = "&Settings ...";
            // 
            // exportSettingsToolStripMenuItem
            // 
            this.exportSettingsToolStripMenuItem.Name = "exportSettingsToolStripMenuItem";
            this.exportSettingsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.exportSettingsToolStripMenuItem.Text = "Export settings ...";
            // 
            // importSettingsToolStripMenuItem
            // 
            this.importSettingsToolStripMenuItem.Name = "importSettingsToolStripMenuItem";
            this.importSettingsToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
            this.importSettingsToolStripMenuItem.Text = "Import settings ...";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(162, 6);
            // 
            // quit
            // 
            this.quit.Name = "quit";
            this.quit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.quit.Size = new System.Drawing.Size(165, 22);
            this.quit.Text = "e&Xit";
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
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.BackColor = System.Drawing.Color.Lime;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Location = new System.Drawing.Point(1, -3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(42, 25);
            this.button1.TabIndex = 3;
            this.button1.Text = "layer";
            this.button1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button1.UseVisualStyleBackColor = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.BackColor = System.Drawing.Color.Blue;
            this.button2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.ForeColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(38, -3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(22, 25);
            this.button2.TabIndex = 5;
            this.button2.Text = "*";
            this.button2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.button2.UseVisualStyleBackColor = false;
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
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.BottomCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grid.DefaultCellStyle = dataGridViewCellStyle1;
            this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.grid.Location = new System.Drawing.Point(0, 17);
            this.grid.MultiSelect = false;
            this.grid.Name = "grid";
            this.grid.RowHeadersVisible = false;
            this.grid.RowTemplate.Height = 21;
            this.grid.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.grid.Size = new System.Drawing.Size(360, 72);
            this.grid.TabIndex = 0;
            this.grid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellDoubleClick);
            this.grid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellEnter);
            this.grid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseDown);
            this.grid.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.grid_CellMouseEnter);
            this.grid.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseMove);
            this.grid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grid_CellMouseUp);
            this.grid.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.grid_CellPainting);
            this.grid.DragDrop += new System.Windows.Forms.DragEventHandler(this.grid_DragDrop);
            this.grid.DragEnter += new System.Windows.Forms.DragEventHandler(this.grid_DragEnter);
            this.grid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grid_KeyDown);
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
            this.hamburger.Click += new System.EventHandler(this.hamburger_Click);
            // 
            // resize
            // 
            this.resize.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.resize.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.resize.Image = ((System.Drawing.Image)(resources.GetObject("resize.Image")));
            this.resize.Location = new System.Drawing.Point(343, 89);
            this.resize.Name = "resize";
            this.resize.Size = new System.Drawing.Size(16, 16);
            this.resize.TabIndex = 6;
            this.resize.TabStop = false;
            this.resize.MouseDown += new System.Windows.Forms.MouseEventHandler(this.DropForm_MouseDown);
            this.resize.MouseMove += new System.Windows.Forms.MouseEventHandler(this.resize_MouseMove);
            this.resize.MouseUp += new System.Windows.Forms.MouseEventHandler(this.resize_MouseUp);
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
            // DropForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Gray;
            this.ClientSize = new System.Drawing.Size(360, 106);
            this.ContextMenuStrip = this.contextMenuStrip1;
            this.ControlBox = false;
            this.Controls.Add(this.missing);
            this.Controls.Add(this.hamburger);
            this.Controls.Add(this.resize);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.status);
            this.Controls.Add(this.button2);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DropForm";
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
            ((System.ComponentModel.ISupportInitialize)(this.hamburger)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.resize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem settings;
        private System.Windows.Forms.ToolStripMenuItem quit;
        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem openItem;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
        private System.Windows.Forms.ToolStripMenuItem exprolerItem;
        private System.Windows.Forms.ToolStripMenuItem propertyItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.DataGridView grid;
        private System.Windows.Forms.PictureBox resize;
        private System.Windows.Forms.ToolStripMenuItem layerMenu;
        private System.Windows.Forms.ToolStripMenuItem addLayerToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem propertyLayer;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportSettingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importSettingsToolStripMenuItem;
        private System.Windows.Forms.PictureBox hamburger;
        private System.Windows.Forms.Label missing;
    }
}

