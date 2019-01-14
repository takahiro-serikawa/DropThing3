namespace DropThing3
{
    partial class TabDialog
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
            if (disposing && (components != null)) {
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
            this.cancel = new System.Windows.Forms.Button();
            this.ok = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.title = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.color0 = new System.Windows.Forms.Button();
            this.color1 = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.gradation = new System.Windows.Forms.CheckBox();
            this.apply = new System.Windows.Forms.Button();
            this.caption = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.transparent = new System.Windows.Forms.CheckBox();
            this.border = new System.Windows.Forms.CheckBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.randomButton = new System.Windows.Forms.Button();
            this.titlebar = new System.Windows.Forms.CheckBox();
            this.swap = new System.Windows.Forms.Button();
            this.texture = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(281, 246);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 17;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(200, 246);
            this.ok.Name = "ok";
            this.ok.Size = new System.Drawing.Size(75, 23);
            this.ok.TabIndex = 16;
            this.ok.Text = "&OK";
            this.ok.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(65, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 11;
            this.label2.Text = "color";
            // 
            // title
            // 
            this.title.Location = new System.Drawing.Point(100, 12);
            this.title.Name = "title";
            this.title.Size = new System.Drawing.Size(100, 23);
            this.title.TabIndex = 10;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(70, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "title";
            // 
            // color0
            // 
            this.color0.BackColor = System.Drawing.Color.Lime;
            this.color0.Location = new System.Drawing.Point(100, 36);
            this.color0.Name = "color0";
            this.color0.Size = new System.Drawing.Size(100, 23);
            this.color0.TabIndex = 18;
            this.color0.UseVisualStyleBackColor = false;
            this.color0.Click += new System.EventHandler(this.color0_Click);
            // 
            // color1
            // 
            this.color1.BackColor = System.Drawing.Color.Green;
            this.color1.Location = new System.Drawing.Point(100, 56);
            this.color1.Name = "color1";
            this.color1.Size = new System.Drawing.Size(100, 23);
            this.color1.TabIndex = 19;
            this.color1.UseVisualStyleBackColor = false;
            this.color1.Click += new System.EventHandler(this.color1_Click);
            // 
            // gradation
            // 
            this.gradation.AutoSize = true;
            this.gradation.Checked = true;
            this.gradation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gradation.Location = new System.Drawing.Point(21, 60);
            this.gradation.Name = "gradation";
            this.gradation.Size = new System.Drawing.Size(77, 19);
            this.gradation.TabIndex = 20;
            this.gradation.Text = "gradation";
            this.gradation.UseVisualStyleBackColor = true;
            // 
            // apply
            // 
            this.apply.Location = new System.Drawing.Point(100, 246);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 21;
            this.apply.Text = "&Apply";
            this.apply.UseVisualStyleBackColor = true;
            this.apply.Click += new System.EventHandler(this.apply_Click);
            // 
            // caption
            // 
            this.caption.AutoSize = true;
            this.caption.Checked = true;
            this.caption.CheckState = System.Windows.Forms.CheckState.Checked;
            this.caption.Location = new System.Drawing.Point(264, 177);
            this.caption.Name = "caption";
            this.caption.Size = new System.Drawing.Size(92, 19);
            this.caption.TabIndex = 22;
            this.caption.Text = "item caption";
            this.caption.UseVisualStyleBackColor = true;
            // 
            // transparent
            // 
            this.transparent.AutoSize = true;
            this.transparent.Checked = true;
            this.transparent.CheckState = System.Windows.Forms.CheckState.Checked;
            this.transparent.Location = new System.Drawing.Point(100, 202);
            this.transparent.Name = "transparent";
            this.transparent.Size = new System.Drawing.Size(86, 19);
            this.transparent.TabIndex = 23;
            this.transparent.Text = "transparent";
            this.transparent.UseVisualStyleBackColor = true;
            // 
            // border
            // 
            this.border.AutoSize = true;
            this.border.Checked = true;
            this.border.CheckState = System.Windows.Forms.CheckState.Checked;
            this.border.Location = new System.Drawing.Point(264, 202);
            this.border.Name = "border";
            this.border.Size = new System.Drawing.Size(82, 19);
            this.border.TabIndex = 24;
            this.border.Text = "cell border";
            this.border.UseVisualStyleBackColor = true;
            // 
            // addButton
            // 
            this.addButton.Location = new System.Drawing.Point(377, 15);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(75, 23);
            this.addButton.TabIndex = 25;
            this.addButton.Text = "Add &New Tab";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(377, 44);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 26;
            this.deleteButton.Text = "&Delete Tab";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // randomButton
            // 
            this.randomButton.Location = new System.Drawing.Point(206, 36);
            this.randomButton.Name = "randomButton";
            this.randomButton.Size = new System.Drawing.Size(75, 23);
            this.randomButton.TabIndex = 27;
            this.randomButton.Text = "random";
            this.randomButton.UseVisualStyleBackColor = true;
            this.randomButton.Click += new System.EventHandler(this.random_Click);
            // 
            // titlebar
            // 
            this.titlebar.AutoSize = true;
            this.titlebar.Checked = true;
            this.titlebar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.titlebar.Location = new System.Drawing.Point(100, 177);
            this.titlebar.Name = "titlebar";
            this.titlebar.Size = new System.Drawing.Size(66, 19);
            this.titlebar.TabIndex = 28;
            this.titlebar.Text = "title bar";
            this.titlebar.UseVisualStyleBackColor = true;
            this.titlebar.Visible = false;
            // 
            // swap
            // 
            this.swap.Location = new System.Drawing.Point(206, 56);
            this.swap.Name = "swap";
            this.swap.Size = new System.Drawing.Size(75, 23);
            this.swap.TabIndex = 29;
            this.swap.Text = "swap";
            this.swap.UseVisualStyleBackColor = true;
            this.swap.Click += new System.EventHandler(this.swap_Click);
            // 
            // texture
            // 
            this.texture.AllowDrop = true;
            this.texture.Location = new System.Drawing.Point(100, 131);
            this.texture.Name = "texture";
            this.texture.Size = new System.Drawing.Size(326, 23);
            this.texture.TabIndex = 30;
            this.texture.DragDrop += new System.Windows.Forms.DragEventHandler(this.texture_DragDrop);
            this.texture.DragEnter += new System.Windows.Forms.DragEventHandler(this.texture_DragEnter);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(53, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(44, 15);
            this.label3.TabIndex = 31;
            this.label3.Text = "texture";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(428, 130);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 23);
            this.button1.TabIndex = 32;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // TabDialog
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(464, 281);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.texture);
            this.Controls.Add(this.swap);
            this.Controls.Add(this.titlebar);
            this.Controls.Add(this.randomButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.border);
            this.Controls.Add(this.transparent);
            this.Controls.Add(this.caption);
            this.Controls.Add(this.apply);
            this.Controls.Add(this.gradation);
            this.Controls.Add(this.color1);
            this.Controls.Add(this.color0);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.ok);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.title);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 170);
            this.Name = "TabDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "tab property - DropThing";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Button ok;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox title;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button color0;
        private System.Windows.Forms.Button color1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.CheckBox gradation;
        private System.Windows.Forms.Button apply;
        private System.Windows.Forms.CheckBox caption;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox transparent;
        private System.Windows.Forms.CheckBox border;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button randomButton;
        private System.Windows.Forms.CheckBox titlebar;
        private System.Windows.Forms.Button swap;
        private System.Windows.Forms.TextBox texture;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}