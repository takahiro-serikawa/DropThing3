using System;
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

        bool loading = false;

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
            set {
                color0.BackColor = value;
                color0.Text = value.Name;
                color0.ForeColor = ColorUtl.TextColor(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Color Color1
        {
            get { return color1.BackColor; }
            set {
                color1.BackColor = value;
                color1.Text = value.Name;
                color1.ForeColor = ColorUtl.TextColor(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool DrawGradation
        {
            get { return gradation.Checked; }
            set { gradation.Checked = value; }
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
        public bool TrasnparentMode
        {
            get { return transparent.Checked; }
            set { transparent.Checked = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool CellBorder
        {
            get { return border.Checked; }
            set { border.Checked = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool TitleBar
        {
            get { return titlebar.Checked; }
            set { titlebar.Checked = value; }
        }

        public bool MediumIcon
        {
            get { return medium.Checked; }
            set { medium.Checked = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string TexturePath
        {
            get { return texture.Text; }
            set { texture.Text = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
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
            if (!loading && OnAccept != null)
                OnAccept(this);
        }

        void ApplyImm()
        {
            if (!loading && OnAccept != null)
                OnAccept(this);
        }

        private void ok_Click(object sender, EventArgs e)
        {
            ApplyImm();
            this.Close();
        }

        public event EventHandler OnUndo;

        private void undo_Click(object sender, EventArgs e)
        {
            if (OnUndo != null)
                OnUndo(this, null);
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            if (OnUndo != null)
                OnUndo(this, null);
            this.Close();
        }

        private void TabDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Visible = false;
            e.Cancel = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /*public bool Popup()
        {
            if (OnOpen != null)
                OnOpen(this, null);

            return this.ShowDialog() == DialogResult.OK
               && (OnAccept == null || OnAccept(this));
        }*/

        /// <summary>
        /// 
        /// </summary>
        public void ShowModeless()
        {
            if (OnOpen != null) {
                loading = true;
                OnOpen(this, null);
                loading = false;
            }
            Show();
            BringToFront();
            Focus();
        }

        private void color0_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Color0;
            if (colorDialog1.ShowDialog() == DialogResult.OK) {
                Color0 = colorDialog1.Color;
                ApplyImm();
            }
        }

        private void color1_Click(object sender, EventArgs e)
        {
            colorDialog1.Color = Color1;
            if (colorDialog1.ShowDialog() == DialogResult.OK) {
                Color1 = colorDialog1.Color;
                ApplyImm();
            }
        }

        public event EventHandler OnAddNew;

        private void addButton_Click(object sender, EventArgs e)
        {
            if (OnAddNew != null)
                OnAddNew(this, null);
        }

        public event EventHandler OnDelete;

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (OnDelete != null)
                OnDelete(this, null);
        }

        private void random_Click(object sender, EventArgs e)
        {
            Color0 = ColorUtl.RandomNamedColor();
            Color1 = ColorUtl.RandomNamedColor();

            ApplyImm();
        }

        private void swap_Click(object sender, EventArgs e)
        {
            Color temp = Color0;
            Color0 = Color1;
            Color1 = temp;

            ApplyImm();
        }

        private void texture_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void texture_DragDrop(object sender, DragEventArgs e)
        {
            string[] names = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            texture.Text = names[0];
        }

    }
}
