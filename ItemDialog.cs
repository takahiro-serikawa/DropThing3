using System;
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
        /// <returns></returns>
        public bool Popup()
        {
            return (this.ShowDialog() == DialogResult.OK) && (this.FilePath != "");
        }

        private void select_Click(object sender, EventArgs e)
        {
            if (path.Text != "")
                openFileDialog1.FileName = path.Text;
            if (openFileDialog1.ShowDialog() == DialogResult.OK) {
                path.Text = openFileDialog1.FileName;
                if (caption.Text == "")
                    //caption.Text = Path.GetFileNameWithoutExtension(path.Text);
                    caption.Text = Path.GetFileName(path.Text);
            }
        }
    }
}
