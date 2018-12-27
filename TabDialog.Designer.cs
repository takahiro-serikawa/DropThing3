﻿namespace DropThing3
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
            this.SuspendLayout();
            // 
            // cancel
            // 
            this.cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancel.Location = new System.Drawing.Point(190, 126);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 17;
            this.cancel.Text = "&Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            // 
            // ok
            // 
            this.ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.ok.Location = new System.Drawing.Point(109, 126);
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
            this.color0.Size = new System.Drawing.Size(75, 23);
            this.color0.TabIndex = 18;
            this.color0.UseVisualStyleBackColor = false;
            this.color0.Click += new System.EventHandler(this.color0_Click);
            // 
            // color1
            // 
            this.color1.BackColor = System.Drawing.Color.Green;
            this.color1.Location = new System.Drawing.Point(100, 56);
            this.color1.Name = "color1";
            this.color1.Size = new System.Drawing.Size(75, 23);
            this.color1.TabIndex = 19;
            this.color1.UseVisualStyleBackColor = false;
            this.color1.Click += new System.EventHandler(this.color1_Click);
            // 
            // gradation
            // 
            this.gradation.AutoSize = true;
            this.gradation.Checked = true;
            this.gradation.CheckState = System.Windows.Forms.CheckState.Checked;
            this.gradation.Location = new System.Drawing.Point(22, 64);
            this.gradation.Name = "gradation";
            this.gradation.Size = new System.Drawing.Size(77, 19);
            this.gradation.TabIndex = 20;
            this.gradation.Text = "gradation";
            this.gradation.UseVisualStyleBackColor = true;
            // 
            // apply
            // 
            this.apply.Location = new System.Drawing.Point(12, 126);
            this.apply.Name = "apply";
            this.apply.Size = new System.Drawing.Size(75, 23);
            this.apply.TabIndex = 21;
            this.apply.Text = "&Apply";
            this.apply.UseVisualStyleBackColor = true;
            // 
            // TabDialog
            // 
            this.AcceptButton = this.ok;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.cancel;
            this.ClientSize = new System.Drawing.Size(464, 161);
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
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MinimumSize = new System.Drawing.Size(300, 170);
            this.Name = "TabDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "tab property";
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
    }
}