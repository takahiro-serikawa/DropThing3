// DropThing 3 main form

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
using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Xml.Serialization;

// TODO
// multiple tab 
// application icon
// cell drawing too slow
// undo (delete item, ...)

// multiple dock
// other icon size
// URL item's icon
// drop to folder cell
// TODO: 拡張子登録
// hot key
// multi drop files

namespace DropThing3
{
    public partial class DropForm: Form
    {
        // file extension for DropThing settings
        const string DROPTHING_EXT = "dtIII";
        string root;            // application data

        public DropForm()
        {
            InitializeComponent();
            main_form = this;

            string app = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            AppStatusText(Color.Black, "{0} ver{1}.{2:D2}; {3}",
               app, ver.Major, ver.Minor, "application launcher");
            title.Text = string.Format("{0} ver{1}.{2:D2}", app, ver.Major, ver.Minor);

            root = Environment.GetFolderPath(
                Environment.SpecialFolder.LocalApplicationData)
               + "\\" + app;
            Directory.CreateDirectory(root);
            filename = Path.Combine(root, app+"."+DROPTHING_EXT);

            // parse command line
            string[] aa = Environment.GetCommandLineArgs();
            var filenames = new List<string>();
            bool new_settings = false;
            for (int i = 1; i<aa.Length; i++) {
                if (aa[i][0] == '-') {
                    switch (aa[i]) {
                    case "-f":
                    case "--file":
                        if (++i < aa.Length)
                            filenames.Add(aa[i]);
                        break;
                    case "--new":
                        new_settings = true;
                        break;
                    default:
                        Console.WriteLine("unknown option {0}", aa[i]);
                        break;
                    }
                } else
                    filenames.Add(aa[++i]);
            }

            // restore last settings
            if (filenames.Count > 0)
                filename = filenames[0];
            if (File.Exists(filename) && !new_settings)
                LoadSettings();
            else
                sett = new DropThingSettings();
            sett.app_version = string.Format("{0}.{1:D2}", ver.Major, ver.Minor);

            GridSize(sett.col_count, sett.row_count);
            cell_bitmap = CellBitmap();
            Modified = false;

            // open other instances
            for (int i = 1; i<filenames.Count; i++)
                Process.Start(Application.ExecutablePath, filenames[i]);
#if DEBUG
            keep_old_settings = true;
#endif

            //this.Font.Name = "";
            RegStartup(true);


        }

        private void DropForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
                switch (MessageBox.Show("DropThing3 quitting\r\n"
                  + "open next time again?", "DropThing3",
                   MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)) {
                case DialogResult.No:
                    RegStartup(false);
                    break;
                case DialogResult.Cancel:
                    e.Cancel = true;
                    break;
                }
        }

        private void DropForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try {
                if (Modified)
                    SaveSettings();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "FATAL", 
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down_flag) {
                // resizing now
                this.Width += e.Location.X - mouse_down_x;
                this.Height += e.Location.Y - mouse_down_y;
            }
        }

        private void resize_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                // resize end
                this.Width = grid.ColumnCount*W;
                this.Height = grid.RowCount*H + Y0 + status.Height;
            }
            mouse_down_flag = false;
        }

        private void DropForm_Resize(object sender, EventArgs e)
        {
            GridSize(grid.Width/W, grid.Height/H);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        void GridSize(int cols, int rows)
        {
            Modified |= (grid.ColumnCount != cols) || (grid.RowCount != rows);

            this.MinimumSize = new Size(W, grid.Top + H + status.Height);

            grid.ColumnCount = Math.Max(cols, 1);
            grid.RowCount = Math.Max(rows, 1);
            foreach (DataGridViewColumn col in grid.Columns) {
                col.Width = W;
            }
            foreach (DataGridViewRow row in grid.Rows) {
                row.Height = H;
            }
        }


        // message

        static DropForm main_form;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void AppStatusText(Color color, string message, params object[] args)
        {
            if (main_form != null) {
                //main_form.status.ForeColor = color;
                main_form.status.Text = string.Format(message, args);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        const string STARTUP_KEY = @"TSERIKAWA_DROP_THING3";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        static void RegStartup(bool value)
        {
            using (var reg = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
               @"Software\Microsoft\Windows\CurrentVersion\Run", true)) {
                if (value)
                    reg.SetValue(STARTUP_KEY, Application.ExecutablePath + " --resident");
                else
                    reg.DeleteValue(STARTUP_KEY);
            }
        }


        // application settings

        public class CellItem: Object
        {
            public string caption;
            public string path;
            public string options;
            public string dir;
            public string attr;
            public int row, col;

            [XmlIgnore]
            public Icon icon;

            public CellItem()
            {
                icon = null;
            }

            List<string> executables = new List<string>() { ".exe", ".com" };

            public CellItem(string path)
            {
                this.path = path;
                //this.text = Path.GetFileNameWithoutExtension(path);
                this.caption = Path.GetFileName(path);

                //this.UpdateIcon();
            }

            /// <summary>
            /// 
            /// </summary>
            public void UpdateIcon()
            {
                this.attr = "";
                if (path.StartsWith("http://") || path.StartsWith("https://"))
                    this.attr = "U";
                else if (Directory.Exists(path))
                    this.attr = "d";
                else if (File.Exists(path)) {
                    this.attr = "f";
                    string ext = Path.GetExtension(path);
                    if (executables.IndexOf(ext) >= 0)
                        this.attr += "x";
                }

                try {
                    this.icon = Icon.ExtractAssociatedIcon(this.path);
                } catch (Exception ex) {
                    this.icon = null;
                    Console.WriteLine("UpdateIcon(); "+ex.Message);
                }

                if (this.icon == null) {
                    this.icon = GetIconAPI.Get(this.path);
                }
            }

            public bool IsUrl()
            {
                return path.StartsWith("http://") || path.StartsWith("https://");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public void ProcessStart(params string[] args)
            {
                Cursor.Current = Cursors.WaitCursor;
                try {
                    var info = new ProcessStartInfo(this.path, this.options + string.Join(" ", args));
                    info.WorkingDirectory = this.dir;
                    Process.Start(info);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //AppStatusText(Color.Fuchsia, ex.Message);
                }
            }
        }

        public class TabLayer: Object
        {
            public uint id;
            public string title;
            public Color color0, color1;
        }

        public class DropThingSettings: object
        {
            public string app_version;
            public List<CellItem> cell_list = new List<CellItem>();
            public List<TabLayer> tab_list = new List<TabLayer>();
            public uint tab_serial;

            public FormWindowState win_state;
            public int left, top, col_count, row_count;

            public DropThingSettings()
            {
                TabLayer def_tab = new TabLayer();
                def_tab.id = tab_serial++;
                def_tab.title = "untitled";
                def_tab.color0 = Color.Lime;
                def_tab.color1 = Color.Green;

                tab_list.Add(def_tab);
            }
        }

        public DropThingSettings sett;
        string filename;
        bool modified = false;
        DateTime modified_time;

        /// <summary>
        /// 
        /// </summary>
        bool Modified
        {
            get { return modified; }
            set
            {
                modified = value;
                if (modified)
                    modified_time = DateTime.Now;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void LoadSettings()
        {
            var serializer = new XmlSerializer(typeof(DropThingSettings));
            using (var sr = new StreamReader(filename, Encoding.UTF8)) {
                sett = (DropThingSettings)serializer.Deserialize(sr);
            }

            this.StartPosition = FormStartPosition.Manual;
            //this.WindowState = dock.win_state;
            this.Left = sett.left;
            this.Top = sett.top;
            this.Width = sett.col_count * W;
            this.Height = sett.row_count * H + 17 + 17;
        }

        string DefSettFilename()
        {
            string dir = Environment.GetFolderPath(
               Environment.SpecialFolder.LocalApplicationData);
            string app = Path.GetFileNameWithoutExtension(Application.ExecutablePath);            
            Directory.CreateDirectory(dir+"\\"+app);
            return dir+"\\"+app+"\\"+app+"."+DROPTHING_EXT;
        }

        bool keep_old_settings = false;

        /// <summary>
        /// 
        /// </summary>
        void SaveSettings()
        {
            if (sett == null)
                sett = new DropThingSettings();

            //filename = DefSettFilename();

            sett.win_state = this.WindowState;
            sett.left = this.Left;
            sett.top = this.Top;
            sett.col_count = grid.ColumnCount;
            sett.row_count = grid.RowCount;

            if (keep_old_settings)
                try {
                    string bak_name = Path.ChangeExtension(filename, 
                       DateTime.Now.ToString("yyyyMMdd-hhmmss") + "." + DROPTHING_EXT);
                    File.Move(filename, bak_name);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

            var serializer = new XmlSerializer(typeof(DropThingSettings));
            using (var sw = new StreamWriter(filename, false, Encoding.UTF8)) {
                serializer.Serialize(sw, sett);
            }

            AppStatusText(Color.Black, "auto save done.");
        }

        /// <summary>
        /// 
        /// </summary>
        const int AUTO_SAVE_DELAY = 60;

        /// <summary>
        /// 
        /// </summary>
        Rectangle HotCorner = new Rectangle(0, 0, 100, 50);

        /// <summary>
        /// 
        /// </summary>
        double HotDelay = 1.5;

        DateTime hot_time;
        bool last_hot_flag = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        bool HotCornerCheck(Point p)
        {
            bool hot_flag = HotCorner.Contains(p);
            if (!last_hot_flag && hot_flag) {
                hot_time = DateTime.Now.AddSeconds(HotDelay);
            }
            last_hot_flag = hot_flag;

            if (last_hot_flag && DateTime.Now >= hot_time) {
                last_hot_flag = false;
                return true;
            }
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try {
                // auto save
                if (Modified && DateTime.Now > modified_time.AddSeconds(AUTO_SAVE_DELAY)) {
                    SaveSettings();
                    Modified = false;
                }

                // hot corner
                if (HotCornerCheck(Control.MousePosition)) {
                    this.Opacity = 0;
                    this.TopMost = true;
                    this.TopMost = false;
                    for (double o = 0; o < 1.0; o+= 0.01) {
                        System.Threading.Thread.Sleep(10);
                        this.Opacity = o;
                    }
                    this.Activate();
                }
            } catch (Exception ex) {
                Console.WriteLine("timer1: "+ex.Message);
            }
            //Console.WriteLine(Control.MousePosition.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        CellItem LookupItem(int col, int row)
        {
            var found = sett.cell_list.Where(c => c.col == col && c.row == row);
            if (found.Count() > 0)
                return found.First();
            return null;
        }

        CellItem LookupItem(DataGridViewCell cell)
        {
            return LookupItem(cell.ColumnIndex, cell.RowIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        CellItem NewCellItem(string path, int col, int row)
        {
            var item = new CellItem(path);
            item.col = col;
            item.row = row;
            sett.cell_list.Add(item);
            grid.InvalidateCell(col, row);
            Modified = true;
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        CellItem CurrentItem
        {
            get
            {
                //return curr_item; 
                return LookupItem(grid.CurrentCell);
            }
            //set { }
        }

        // drawing
        bool caption_visible = false;

        //const int TAB_HEIGHT = 17;
        int Y0 { get { return grid.Top; } }
        //const int W = 2+32+2;
        //const int H = 2+32+2;
        int W { get { return caption_visible ? 80 : 2+32+2; } }
        int H { get { return caption_visible ? 2+32+17+2 : 2+32+2; } }
        //const int FOOTER_HEIGHT = 17;

        Color trim_color(Color color, int d)
        {
            int r = (int)color.R + d;
            if (r < 0)
                r = 0;
            else if (r > 255)
                r = 255;

            int g = (int)color.G + d;
            if (g < 0)
                g = 0;
            else if (g > 255)
                g = 255;

            int b = (int)color.B + d;
            if (b < 0)
                b = 0;
            else if (b > 255)
                b = 255;

            return Color.FromArgb(r, g, b);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Bitmap CellBitmap()
        {
            brush0 = new SolidBrush(trim_color(color0, +20));
            brush1 = new SolidBrush(trim_color(color1, -20));
            resize.BackColor = color0;

            var bmp = new Bitmap(W, H);
            using (var g = Graphics.FromImage(bmp))
            using (var light = new Pen(trim_color(color0, +20)))
            using (var dark = new Pen(trim_color(color1, -20)))
            using (var brush = new LinearGradientBrush(g.VisibleClipBounds,
               color0, color1, LinearGradientMode.Vertical)) {
                g.FillRectangle(brush, g.VisibleClipBounds);
                g.DrawLine(light, 0, 0, W, 0);
                g.DrawLine(light, 0, 0, 0, H);
                g.DrawLine(dark, 0, H-1, W, H-1);
                g.DrawLine(dark, W-1, 0, W-1, H-1);
            }
            return bmp;
        }

        Bitmap cell_bitmap;

        int mouse_down_x, mouse_down_y;
        bool mouse_down_flag = false;

        private void DropForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                // start form moving
                mouse_down_flag = true;
                mouse_down_x = e.Location.X;
                mouse_down_y = e.Location.Y;
            }
        }

        private void DropForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down_flag) {
                // moving now
                this.Left += e.Location.X - mouse_down_x;
                this.Top += e.Location.Y - mouse_down_y;
            }
        }

        private void DropForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) {
                // move end
                mouse_down_flag = false;
            }
        }

        private void DropForm_Move(object sender, EventArgs e)
        {
            Modified = true;
        }

        // drag and drop to grid

        private void grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
             || e.Data.GetDataPresent("UniformResourceLocator"))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            string[] names;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                names = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            else if (e.Data.GetDataPresent("UniformResourceLocator"))
                names = new string[] { e.Data.GetData(DataFormats.Text).ToString() };
            else
                return;

            var point = grid.PointToClient(new Point(e.X, e.Y));
            var hit = grid.HitTest(point.X, point.Y);
            if (hit.Type == DataGridViewHitTestType.Cell) {
                var item = LookupItem(hit.ColumnIndex, hit.RowIndex);
                if (item != null) {
                    // drop file to icon; execute application
                    item.ProcessStart(names[0]);
                } else {
                    if (drag_item != null) {
                        // moving inner form
                        grid.InvalidateCell(drag_item.col, drag_item.row);
                        drag_item.col = hit.ColumnIndex;
                        drag_item.row = hit.RowIndex;
                        grid.InvalidateCell(hit.ColumnIndex, hit.RowIndex);
                        Modified = true;
                    } else {
                        // drop file to empty cell; register file to cell
                        item = NewCellItem(names[0], hit.ColumnIndex, hit.RowIndex);
                    }
                }
                AppStatusText(Color.Black, "drop {0}, {1}: {2}", hit.ColumnIndex, hit.RowIndex, names[0]);
            }
        }        

        CellItem point_item = null;

        private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            // show mouse over cell information
            point_item = LookupItem(e.ColumnIndex, e.RowIndex);
            AppStatusText(Color.Black, "[{0},{1}] {2}",
               e.ColumnIndex, e.RowIndex,
               (point_item != null) ? point_item.caption + "; " + point_item.path : "");
            grid.Cursor =  (point_item != null) ? Cursors.Hand : Cursors.Default;
        }

        private void grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            point_item = LookupItem(e.ColumnIndex, e.RowIndex);
            AppStatusText(Color.Black, "[{0},{1}] {2}",
               e.ColumnIndex, e.RowIndex,
               (point_item != null) ? point_item.caption + "; " + point_item.path : "");
        }

        bool drag_flag = false;

        private void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Console.WriteLine("grid_CellMouseDown({0})", estr(e));
            
            // select cell on right click too
            if (e.Button == MouseButtons.Right)
                grid.CurrentCell = grid[e.ColumnIndex, e.RowIndex];

            if (e.Button == MouseButtons.Left) {
                // prepare cell drag
                mouse_down_flag = true;
                mouse_down_x = e.Location.X;
                mouse_down_y = e.Location.Y;
            }
        }

        CellItem drag_item = null;

        private void grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (mouse_down_flag && CurrentItem != null
             && (e.X < mouse_down_x-5 || e.X > mouse_down_x+5
              || e.Y < mouse_down_y-5 || e.Y > mouse_down_y+5)) {
                drag_item = CurrentItem;

                mouse_down_flag = false;
                string[] files = { CurrentItem.path };
                var data = new DataObject(DataFormats.FileDrop, files);
                var dde = grid.DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void grid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            //Console.WriteLine("grid_CellMouseUp({0})", estr(e));
            if (!mouse_down_flag) {

            } else if (/*CurrentItem != null && */e.Button == MouseButtons.Left && e.Clicks == 1) {
                // cell click
                if (ModifierKeys.HasFlag(Keys.Control))
                    explorerItem_Click(null, null);
                else
                    openItem_Click(null, null);
            }

            mouse_down_flag = false;
        }

        private void grid_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel)
                drag_item = null;
        }

        private void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var g = e.Graphics;

            // draw cell background
            g.DrawImage(cell_bitmap, e.CellBounds.X, e.CellBounds.Y);

            // draw focus rectangle
            if (e.State.HasFlag(DataGridViewElementStates.Selected)) {
                using (var pen = new Pen(BlackOrWhite(color0, 255))) {
                    pen.DashStyle = DashStyle.Dot;
                    var r = e.CellBounds;
                    r.Inflate(-2, -2);
                    r.Offset(-1, -1);
                    g.DrawRectangle(pen, r);
                }
            }

            // draw item icon
            var item = LookupItem(e.ColumnIndex, e.RowIndex);
            if (item != null) {
                if (item.icon == null)
                    item.UpdateIcon();

                if (item.icon != null)
                    g.DrawIcon(item.icon, e.CellBounds.X+2, e.CellBounds.Y+2);
                else {
                    //g.DrawIcon(SystemIcons.Question, e.CellBounds.X+2, e.CellBounds.Y+2);

                    string alt = item.IsUrl() ? "URL" : "?";
                    var f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;

                    var r = e.CellBounds;
                    int x = r.X + r.Width/2;
                    int y = r.Y + r.Height/2;

                    g.DrawString(alt, missing.Font, brush0, x, y, f);
                    g.DrawString(alt, missing.Font, brush1, x-1, y-1, f);
                }
            }

            // draw item caption
            if (item != null && caption_visible) {
                var m = g.MeasureString(item.caption, this.Font);
                Color color = BlackOrWhite(color1);
                float x = e.CellBounds.Left + (e.CellBounds.Width-m.Width)/2;
                float y = e.CellBounds.Top + 2+32;
                g.DrawString(item.caption, this.Font, new SolidBrush(color), x, y);
            }

            e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.Background);
            e.Handled = true;
        }

        // menu handlers

        private void openItem_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null)
                CurrentItem.ProcessStart();
        }

        private void grid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                if (e.Control)
                    explorerItem_Click(null, null);
                //else if (e.Alt)
                //    propertyItem_Click(null, null);
                else
                    openItem_Click(null, null);
                e.Handled = true;
            }
        }

        private void explorerItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (CurrentItem != null) {
                Process.Start("EXPLORER.EXE", @"/select,""" + CurrentItem.path + @"""");
                AppStatusText(Color.Black, "explorer {0}", CurrentItem.path);
            } else {
                Process.Start("EXPLORER.EXE");
                AppStatusText(Color.Black, "explorer");
            }
        }

        private void deleteItem_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null) {
                sett.cell_list.Remove(CurrentItem);
                grid.InvalidateCell(grid.CurrentCell);
                Modified = true;
            }
        }

        private void propertyItem_Click(object sender, EventArgs e)
        {
            var dlg = new ItemDialog();
            if (CurrentItem != null) {
                dlg.ItemCaption = CurrentItem.caption;
                dlg.FilePath = CurrentItem.path;
                dlg.CommandOptions = CurrentItem.options;
                dlg.WorkingDirectory = CurrentItem.dir;
            }

            if (dlg.Popup()) {
                CellItem item;
                if (CurrentItem != null)
                    item = CurrentItem;
                else 
                    item = NewCellItem(dlg.FilePath,
                        grid.CurrentCell.ColumnIndex, grid.CurrentCell.RowIndex);
                item.caption = dlg.ItemCaption;
                item.options = dlg.CommandOptions;
                CurrentItem.dir = dlg.WorkingDirectory;
                Modified = true;
            }
        }

        private void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (CurrentItem == null)
                propertyItem_Click(sender, null);
        }

        Color color0 = Color.Lime, color1 = Color.Green;
        Brush brush0, brush1;

        Color BlackOrWhite(Color color, int threshold = 400)
        {
            var v = Math.Sqrt(color.R * color.R + color.G * color.G + color.B * color.B);
            return (v >= threshold) ? Color.Black : Color.White;
        }

        private void resize_Paint(object sender, PaintEventArgs e)
        {
        }

        private void tabItem_Click(object sender, EventArgs e)
        {
            var dlg = new TabDialog();
            dlg.Color0 = color0;
            dlg.Color1 = color1;

            if (dlg.Popup()) {
                color0 = dlg.Color0;
                color1 = dlg.Color1;
                status.BackColor = color0;
                status.ForeColor = BlackOrWhite(color0);
                button1.BackColor = color0;
                button1.ForeColor = BlackOrWhite(color0);
                grid.BackgroundColor = color1;

                cell_bitmap = CellBitmap();
                grid.Invalidate();
                Modified = false;
            }
        }

        private void quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void hamburger_Click(object sender, EventArgs e)
        {
            var r = grid.GetCellDisplayRectangle(
               grid.CurrentCell.ColumnIndex, grid.CurrentCell.RowIndex, false);
            contextMenuStrip1.Show(grid, r.Left+10, r.Top+10);
        }

    }
}
