﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;

namespace DropThing3
{
    public partial class ItemDialog: Form
    {
        public ItemDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string ItemCaption
        {
            get { return caption.Text; }
            set { caption.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FilePath
        {
            get { return path.Text; }
            set { path.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CommandOptions
        {
            get { return options.Text; }
            set { options.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string WorkingDirectory
        {
            get { return dir.Text; }
            set { dir.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dlg"></param>
        /// <returns></returns>
        public delegate bool DialogEvent(object sender);

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler OnOpen;

        /// <summary>
        /// 
        /// </summary>
        public event DialogEvent OnAccept;

        private void apply_Click(object sender, EventArgs e)
        {
            if (OnAccept != null)
                OnAccept(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public bool Popup(DialogEvent callback = null)
        {
            if (callback != null)
                this.OnAccept = callback;

            if (OnOpen != null)
                OnOpen(this, null);

            return this.ShowDialog() == DialogResult.OK
                && this.FilePath != ""
                && (OnAccept == null || OnAccept(this));
        }

        private void select_Click(object sender, EventArgs e)
        {
            if (path.Text != "") {
                try {
                    openFileDialog1.InitialDirectory = Path.GetDirectoryName(path.Text);
                    openFileDialog1.FileName = Path.GetFileName(path.Text);
                } catch (Exception ex) {
                    openFileDialog1.FileName = path.Text;
                    Console.WriteLine(ex.Message);
                }
            }

            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                path.Text = openFileDialog1.FileName;
                //if (caption.Text == "")
                    //caption.Text = Path.GetFileNameWithoutExtension(path.Text);
                //    caption.Text = Path.GetFileName(path.Text);
            }
        }

        [System.Runtime.InteropServices.DllImport("Powrprof.dll", SetLastError = true)]
        public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        private void suspend_Click(object sender, EventArgs e)
        {
            SetSuspendState(false, false, false);
        }
    }
}
