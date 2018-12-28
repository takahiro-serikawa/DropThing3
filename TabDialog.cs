﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DropThing3
{
    public partial class TabDialog: Form
    {
        public TabDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public string TabTitle
        {
            get { return title.Text; }
            set { title.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color Color0
        {
            get { return color0.BackColor; }
            set { color0.BackColor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color Color1
        {
            get { return color1.BackColor; }
            set { color1.BackColor = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ShowItemCaption
        {
            get { return caption.Checked; }
            set { caption.Checked = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dlg"></param>
        /// <returns></returns>
        public delegate bool ApplyCallback(TabDialog dlg);

        ApplyCallback callback = null;

        private void apply_Click(object sender, EventArgs e)
        {
            this.callback(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Popup(ApplyCallback callback)
        {
            this.callback = callback;

            bool ret = (this.ShowDialog() == DialogResult.OK);
            return ret && this.callback(this);
        }

        private void color0_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Color0;
            if (colorDialog1.ShowDialog() == DialogResult.OK) {
                Color0 = colorDialog1.Color;
            }
        }

        private void color1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Color1;
            if (colorDialog1.ShowDialog() == DialogResult.OK) {
                Color1 = colorDialog1.Color;
            }
        }

    }
}
