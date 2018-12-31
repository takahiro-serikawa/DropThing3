// DropThing 3 main form

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

// TODO

// hot key
// cell drawing too slow
// undo (delete item, ...)
// other icon size
// drop to folder cell
// multi drop files
// multiple dock
// TODO: 拡張子登録
// scan favicon in html
// double click item

// WM_DEVICECHANGE

namespace DropThing3
{
    public partial class DropMain: Form
    {
        // file extension for DropThing settings
        const string DROPTHING_EXT = "dtIII";
        string appdata; // application datacpath

        public DropMain()
        {
            InitializeComponent();
            main_form = this;

            string app = Path.GetFileNameWithoutExtension(Application.ExecutablePath);
            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            AppStatusText(Color.Black, "{0} version {1}.{2:D2}; {3}",
               app, ver.Major, ver.Minor, "application launcher");
            title.Text = string.Format("{0} version {1}.{2:D2}", app, ver.Major, ver.Minor);

            Directory.SetCurrentDirectory(@"C:\");

            var a = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appdata = a + "\\" + app;
            Directory.CreateDirectory(appdata);
            filename = Path.Combine(appdata, app+"."+DROPTHING_EXT);

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
            LoadSettings(new_settings ? "" : filename);
            sett.app_version = string.Format("{0}.{1:D2}", ver.Major, ver.Minor);

            // open other instances
            for (int i = 1; i < filenames.Count; i++)
                Process.Start(Application.ExecutablePath, filenames[i]);
#if DEBUG
            keep_old_settings = true;
#endif

            //this.Font.Name = "";
            RegStartup(true);

            cache_path = Path.Combine(appdata, "icon_cache");
            faviconFetch.RunWorkerAsync();
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
                faviconFetch.CancelAsync();

                if (Modified)
                    SaveSettings();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "FATAL",
                   MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // resize

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
                FitToGrid();
            }
            mouse_down_flag = false;
        }

        private void DropForm_Resize(object sender, EventArgs e)
        {
            GridSize(grid.Width / CellWidth, grid.Height / CellHeight);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        void GridSize(int cols, int rows)
        {
            Modified |= (grid.ColumnCount != cols) || (grid.RowCount != rows);

            CellWidth = sett.caption_visible ? 80 : M+32+M;
            CellHeight = sett.caption_visible ? M+32+17+M : M+32+M;

            MinimumSize = new Size(CellWidth, grid.Top + CellHeight + status.Height);

            cell_bitmap = CellBitmap();
            status.BackColor = color0;
            status.ForeColor = TextColor(color0, 250);
            //CurrentTab.button.BackColor = color0;
            //CurrentTab.button.ForeColor = TextColor(color0, 250);

            grid.ColumnCount = Math.Max(cols, 1);
            grid.RowCount = Math.Max(rows, 1);
            foreach (DataGridViewColumn col in grid.Columns) {
                col.Width = CellWidth;
            }
            foreach (DataGridViewRow row in grid.Rows) {
                row.Height = CellHeight;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void FitToGrid()
        {
            this.Size = new Size(
               grid.ColumnCount * CellWidth,
             Y0 + grid.RowCount * CellHeight + status.Height);
        }

        // message

        static DropMain main_form;

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

        public class CellItem: object
        {
            public string caption;
            public string path;
            public string options;
            public string dir;
            public string attr;
            public int row, col;
            public uint tab;

            [XmlIgnore]
            public Icon icon;

            public CellItem()
            {
                icon = null;
                attr = "";
            }

            public CellItem(string path)
            {
                this.path = path;
                //this.text = Path.GetFileNameWithoutExtension(path);
                //this.caption = Path.GetFileName(path);

                //this.UpdateIcon();
            }

            public bool HasAttr(char c)
            {
                return attr != null && attr.IndexOf(c) >= 0;
            }

            public void AddAttr(char c)
            {
                if (!HasAttr(c))
                    attr += c;
            }

            List<string> executables = new List<string>() { ".exe", ".com" };

            /// <summary>
            /// 
            /// </summary>
            public void UpdateIcon()
            {
                if (attr == null)
                    attr = "";

                // get cache if exists
                string cachename = MakeCacheName(path);
                if (this.icon == null && File.Exists(cachename))
                    try {
                        this.icon = new Icon(cachename);
                    } catch (Exception ex) {
                        this.icon = null;
                        Console.WriteLine("UpdateIcon(); "+ex.Message);
                    }

                //this.attr = "";
                if (path.StartsWith("http://") || path.StartsWith("https://")) {
                    this.AddAttr('U');
                    if (this.icon == null)
                        fetch_req.Enqueue(path);
                } else {
                    if (Directory.Exists(path))
                        this.AddAttr('d');
                    else if (File.Exists(path)) {
                        this.AddAttr('f');
                        string ext = Path.GetExtension(path);
                        if (executables.IndexOf(ext) >= 0)
                            this.AddAttr('x');
                    }

                    // removal?
                    string fullpath = this.path.StartsWith(@"\")
                        ? this.path : Path.GetFullPath(this.path);
                    if (fullpath.Length > 1 && fullpath[1] == ':')
                        try {
                            var drive = new DriveInfo(fullpath.Substring(0, 1));
                            switch (drive.DriveType) {
                            case DriveType.Removable:
                            case DriveType.CDRom:
                            case DriveType.Network:
                                this.AddAttr('J');
                                break;
                            }
                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                        }

                    if (this.icon == null)
                        try {
                            this.icon = Icon.ExtractAssociatedIcon(this.path);
                        } catch (Exception ex) {
                            this.icon = null;
                            Console.WriteLine("UpdateIcon(); "+ex.Message);
                        }

                    if (this.icon == null) {
                        this.icon = GetIconAPI.Get(this.path);
                    }

                    if (this.icon != null && HasAttr('J') && !File.Exists(cachename)) {
                        using (var stream = new FileStream(cachename, FileMode.Create, FileAccess.Write))
                            this.icon.Save(stream);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="args"></param>
            public void ProcessStart(params string[] args)
            {
                Cursor.Current = Cursors.WaitCursor;
                try {
                    var info = new ProcessStartInfo(this.path);
                    info.Arguments = this.options + escaped_join(args);
                    info.WorkingDirectory = this.dir;
                    //info.Environment
                    Process.Start(info);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //AppStatusText(Color.Fuchsia, ex.Message);
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public string GetCaption()
            {
                if (this.caption != "")
                    return this.caption;

                // return default caption if not specified
                if (this.HasAttr('U')) {
                    try {
                        // www.AAAA. ...co.jp
                        var u = new Uri(this.path);
                        string[] hh = u.Host.Split('.');
                        if (hh.Length > 1 && hh[0] == "www")
                            return hh[1];
                        return hh[0];
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                    }
                } else {
                    string filename = Path.GetFileName(this.path);
                    if (filename != "")
                        return filename;
                }

                // return path if something wrong
                return this.path;
            }

        }

        public class TabLayer: object
        {
            public uint id;

            string _title;
            public string title
            {
                get { return _title; }
                set
                {
                    _title = value;
                }
            }

            [XmlIgnore]
            public Color color0, color1;

            [EditorBrowsable(EditorBrowsableState.Never)]
            [Browsable(false)]
            public string xml_color0
            {
                get { return ConvertToString(color0); }
                set { color0 = ConvertFromString<Color>(value); }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            [Browsable(false)]
            public string xml_color1
            {
                get { return ConvertToString(color1); }
                set { color1 = ConvertFromString<Color>(value); }
            }

            /// <summary>
            /// 
            /// </summary>
            public bool draw_gradation;

            /// <summary>
            /// 
            /// </summary>
            public static uint serial = 0;

            /// <summary>
            /// 
            /// </summary>
            public TabLayer()
            {
                id = serial++;

                // default value
                title = "untitled";
                color0 = Color.Lime;
                color1 = Color.Green;
                draw_gradation = true;
            }

            static string ConvertToString<T>(T value)
            {
                return TypeDescriptor.GetConverter(typeof(T)).ConvertToString(value);
            }

            static T ConvertFromString<T>(string value)
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value);
            }
        }

        // app settings class (for serialization)

        /// <summary>
        /// 
        /// </summary>
        public class DropThingSettings: object
        {
            public string app_version;
            public List<CellItem> cell_list = new List<CellItem>();

            [XmlElement("tab_list2")]
            public List<TabLayer> tab_list = new List<TabLayer>();

            public uint tab_serial
            {
                get { return TabLayer.serial; }
                set { TabLayer.serial = value; }
            }

            public FormWindowState win_state;
            public int left, top;
            public int col_count, row_count;

            public bool caption_visible;
            public bool cell_border;
            public bool transparent;

            /// <summary>
            /// 
            /// </summary>
            public DropThingSettings()
            {
                // default values
                cell_border = true;
                col_count = 10;
                row_count = 2;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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
        void LoadSettings(string filename)
        {
            if (File.Exists(filename)) {
                try {
                    var serializer = new XmlSerializer(typeof(DropThingSettings));
                    using (var sr = new StreamReader(filename, Encoding.UTF8)) {
                        sett = (DropThingSettings)serializer.Deserialize(sr);
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }

                //this.WindowState = dock.win_state;
                this.StartPosition = FormStartPosition.Manual;
                this.Left = sett.left;
                this.Top = sett.top;
            } else
                sett = new DropThingSettings();

            if (sett.tab_list.Count > 0)
                CurrentTab = sett.tab_list[0];
            else
                CurrentTab = AddNewTab(null, null); // at least 1 tab

            // tabControl1.TabCount = sett.tab_list;
            RestoreTabs();

            GridSize(sett.col_count, sett.row_count);
            FitToGrid();
            RestoreTabs();
            Modified = false;
        }

        bool keep_old_settings = false;

        /// <summary>
        /// 
        /// </summary>
        void SaveSettings()
        {
            //if (sett == null)
            //    sett = new DropThingSettings();

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
                    AppStatusText(Color.Black, "auto save done.");
                }

                // hot corner
                if (HotCornerCheck(Control.MousePosition)) {
                    this.Opacity = 0;
                    this.TopMost = true;
                    this.TopMost = false;
                    for (; this.Opacity < 1.0; this.Opacity += 0.1)
                        System.Threading.Thread.Sleep(10);
                    this.Activate();
                }
            } catch (Exception ex) {
                Console.WriteLine("timer1: "+ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        TabLayer AddNewTab(object sender, EventArgs args)
        {
            TabLayer tab = new TabLayer();
            //tab.button = MakeTabButton(tab);
            sett.tab_list.Add(tab);
            var tabpage = new TabPage(tab.title);
            tabpage.Tag = tab;
            tabControl1.TabPages.Add(tabpage);
            //RestoreTabs();
            Modified = true;
            return tab;
        }

        void DeleteCurrentTab(object sender, EventArgs args)
        {
            if (sett.tab_list.Count > 1) {
                int index = sett.tab_list.IndexOf(CurrentTab);

                sett.cell_list.RemoveAll(cell => cell.tab == CurrentTab.id);
                sett.tab_list.Remove(CurrentTab);

                tabControl1.TabPages.RemoveAt(index);

                if (index >= sett.tab_list.Count) index--;
                CurrentTab = sett.tab_list[index];

                grid.Invalidate();
            }
        }

        private void tabDoubleTitle_Click(object sender, EventArgs e)
        {
            var tag = (sender as Button).Tag;
            if (tag != null) {
                //CurrentTab = tag as TabLayer;
                tabItem_Click(null, null);
            }
        }

        void RestoreTabs()
        {
            int x = 0;
            for (int i = 0; i < sett.tab_list.Count; i++) {
                var tab = sett.tab_list[i];
                TabPage tabpage;
                if (i < tabControl1.TabPages.Count) {
                    tabpage = tabControl1.TabPages[i];
                } else {
                    tabpage = new TabPage();
                    tabControl1.TabPages.Add(tabpage);
                }
                tabpage.Text = tab.title;
                tabpage.Tag = tab;
            }
            //addTab.Left = x;
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            Console.WriteLine("{0}", e.Action);

            var tab = e.TabPage.Tag as TabLayer;
            CurrentTab = tab;
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            Console.WriteLine("tabControl1_DrawItem(, {0})", e.Bounds);
            var tab = sett.tab_list[e.Index];

            e.Graphics.FillRectangle(new SolidBrush(tab.color0), e.Bounds);
            e.Graphics.DrawString(tab.title, this.Font, Brushes.Black, e.Bounds.Left+2, e.Bounds.Top+2);
            //e.DrawFocusRectangle();
        }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        CellItem LookupItem(int col, int row)
        {
            var found = sett.cell_list.Where(c => 
               c.tab == CurrentTab.id && c.col == col && c.row == row);
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
            item.tab = CurrentTab.id;
            sett.cell_list.Add(item);
            grid.InvalidateCell(col, row);
            Modified = true;
            return item;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        void MoveCell(CellItem item, int col, int row)
        {
            grid.InvalidateCell(item.col, item.row);
            item.col = col;
            item.row = row;
            grid.InvalidateCell(col, row);
            Modified = true;
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
        const int M = 3;
        int Y0 { get { return grid.Top; } }
        int CellWidth = M+32+M;
        int CellHeight = M+32+M;

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

        TabLayer curr_tab;

        /// <summary>
        /// 
        /// </summary>
        TabLayer CurrentTab
        {
            get { return curr_tab; }
            set {
                curr_tab = value;
                GridSize(grid.ColumnCount, grid.RowCount);
                grid.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Bitmap CellBitmap()
        {
            if (sett.transparent) {
                color0 = color1 = this.TransparencyKey = Color.FromArgb(12, 34, 56);
                this.AllowTransparency = true;
            } else {
                this.AllowTransparency = false;
                color0 = CurrentTab.color0;
                color1 = (CurrentTab.draw_gradation && !sett.transparent)
                    ? CurrentTab.color1 : color0;
            }
            brush0 = new SolidBrush(trim_color(color0, +20));
            brush1 = new SolidBrush(trim_color(color1, -20));

            resize.BackColor = color0;

            var bmp = new Bitmap(CellWidth, CellHeight);
            using (var g = Graphics.FromImage(bmp))
            using (var light = new Pen(trim_color(color0, +20)))
            using (var dark = new Pen(trim_color(color1, -20)))
            using (Brush brush = new LinearGradientBrush(g.VisibleClipBounds,
                color0, color1, LinearGradientMode.Vertical)) {
                g.FillRectangle(brush, g.VisibleClipBounds);
                if (sett.cell_border) {
                    g.DrawLine(light, 0, 0, CellWidth, 0);
                    g.DrawLine(light, 0, 0, 0, CellHeight);
                    g.DrawLine(dark, 0, CellHeight-1, CellWidth, CellHeight-1);
                    g.DrawLine(dark, CellWidth-1, 0, CellWidth-1, CellHeight-1);
                }
            }
            return bmp;
        }

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
        bool topmost_on_drag = true;

        private void grid_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
             || e.Data.GetDataPresent("UniformResourceLocator")) {
                e.Effect = DragDropEffects.Copy;

                if (topmost_on_drag) {
                    this.TopMost = true;
                    this.TopMost = false;
                }
            } else
                e.Effect = DragDropEffects.None;
        }

        static string escaped_join(string[] nn)
        {
            // zantei
            string s = "";
            foreach (var n in nn) {
                if (s.Length > 0)
                    s += " ";

                if (n.IndexOf(' ') >= 0)
                    s += "\"" + n + "\"";
                else
                    s += n;
            }
            return s;
        }

        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            string[] names;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                names = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            else if (e.Data.GetDataPresent("UniformResourceLocator")) {
                string url = e.Data.GetData(DataFormats.Text).ToString();
                if (!url.StartsWith("http"))    // http or https
                    url = "http://" + url;
                names = new string[] { url };
            } else
                return;

            var point = grid.PointToClient(new Point(e.X, e.Y));
            var hit = grid.HitTest(point.X, point.Y);
            if (hit.Type == DataGridViewHitTestType.Cell) {
                var item = LookupItem(hit.ColumnIndex, hit.RowIndex);
                if (item != null) {
                    if (item.HasAttr('d')) {
                        // drop files to directory
                        // copy ...? not yet
                    } else {
                        // drop files to app icon; execute application
                        item.ProcessStart(escaped_join(names));
                    }
                } else {
                    if (drag_item != null) {
                        // moving inner form
                        MoveCell(drag_item, hit.ColumnIndex, hit.RowIndex);
                    } else {
                        // drop file to empty cell; register file to cell
                        item = NewCellItem(names[0], hit.ColumnIndex, hit.RowIndex);
                        // TODO accept multiple files
                    }
                }
                AppStatusText(Color.Black, "drop {0}, {1}: {2}", hit.ColumnIndex, hit.RowIndex, names[0]);
            }

            drag_item = null;
        }

        CellItem point_item = null;

        private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            // show mouse over cell information
            point_item = LookupItem(e.ColumnIndex, e.RowIndex);
            AppStatusText(Color.Black, "[{0},{1}] {2}",
               e.ColumnIndex, e.RowIndex,
               (point_item != null) ? point_item.GetCaption() + "; " + point_item.path : "");
            grid.Cursor =  (point_item != null) ? Cursors.Hand : Cursors.Default;
        }

        private void grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            point_item = LookupItem(e.ColumnIndex, e.RowIndex);
            AppStatusText(Color.Black, "[{0},{1}] {2}",
               e.ColumnIndex, e.RowIndex,
               (point_item != null) ? point_item.GetCaption() + "; " + point_item.path : "");

            // update menu activity
            eject.Enabled = (CurrentItem != null) && CurrentItem.HasAttr('J');
            deleteItem.Enabled = (CurrentItem != null);
            openItem.Enabled = (CurrentItem != null);
        }

        //bool drag_flag = false;

        private void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            Console.WriteLine("grid_CellMouseDown({0})", estr(e));

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

#if DEBUG
        string estr(DataGridViewCellMouseEventArgs e)
        {
            return string.Format("clicks={0}, button={1}, delta={2}", e.Clicks, e.Button, e.Delta);
        }
#endif

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
            Console.WriteLine("grid_CellMouseUp({0})", estr(e));
            //if (!mouse_down_flag) {

            //} else 
            if (e.Button == MouseButtons.Left) {
                if (e.Clicks == 1) {
                    // cell click
                    if (ModifierKeys.HasFlag(Keys.Control))
                        explorerItem_Click(null, null);
                    else if (ModifierKeys.HasFlag(Keys.Shift))
                        propertyItem_Click(null, null);
                    else
                        openItem_Click(null, null);
                } else if (e.Clicks == 2) {
                    // cell double click
                    propertyItem_Click(null, null);
                }
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
                using (var pen = new Pen(TextColor(CurrentTab.color0, 255))) {
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

                if (item.icon != null) {
                    int ix = e.CellBounds.X + (e.CellBounds.Width - item.icon.Width)/2;
                    g.DrawIcon(item.icon, ix/*e.CellBounds.X+2*/, e.CellBounds.Y+2);
                } else {
                    string alt = item.HasAttr('U') ? "URL" : "?";
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
            if (item != null && sett.caption_visible) {
                var m = g.MeasureString(item.GetCaption(), this.Font);
                Color color = TextColor(color1, 250);
                var rect = new RectangleF(e.CellBounds.Left, e.CellBounds.Top + M+32, e.CellBounds.Width, m.Height);
                var f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                g.DrawString(item.GetCaption(), this.Font, new SolidBrush(color), rect, f);
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
                else if (e.Shift)
                    propertyItem_Click(null, null);
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

            if (dlg.Popup(ItemAcceptCallback)) {
                // ...
            }
        }

        bool ItemAcceptCallback(ItemDialog dlg)
        {
            CellItem item;
            if (CurrentItem != null)
                item = CurrentItem;
            else
                item = NewCellItem(dlg.FilePath,
                    grid.CurrentCell.ColumnIndex, grid.CurrentCell.RowIndex);
            item.caption = dlg.ItemCaption;
            item.options = dlg.CommandOptions;
            item.dir = dlg.WorkingDirectory;
            Modified = true;
            return true;
        }

        // tab settings
        //public bool draw_gradation = true;
        //Color color0 = Color.Lime, CurrentTab.color1 = Color.Green;
        Color color0, color1;
        Brush brush0, brush1;
        Bitmap cell_bitmap;

        Color TextColor(Color color, int threshold = 400)
        {
            var v = Math.Sqrt(color.R * color.R + color.G * color.G + color.B * color.B);
            return (v >= threshold) ? Color.Black : Color.White;
        }

        private void tabItem_Click(object sender, EventArgs e)
        {
            var dlg = new TabDialog();
            dlg.TabTitle = CurrentTab.title;
            dlg.Color0 = CurrentTab.color0;
            dlg.Color1 = CurrentTab.color1;
            dlg.DrawGradation = CurrentTab.draw_gradation;
            dlg.ShowItemCaption = sett.caption_visible;
            dlg.CellBorder = sett.cell_border;
            dlg.TrasnparentMode = sett.transparent;

            dlg.OnDelete += DeleteCurrentTab;
            //dlg.OnAddNew += AddNewTab;

            if (dlg.Popup(TabAcceptCallback)) {
                // ..
            }
        }

        bool TabAcceptCallback(TabDialog dlg)
        {
            CurrentTab.title = dlg.TabTitle;
            CurrentTab.color0 = dlg.Color0;
            CurrentTab.color1 = dlg.Color1;
            status.BackColor = CurrentTab.color0;
            status.ForeColor = TextColor(CurrentTab.color0, 250);
            //CurrentTab.button.BackColor = CurrentTab.color0;
            //CurrentTab.button.ForeColor = TextColor(CurrentTab.color0, 250);
            grid.BackgroundColor = CurrentTab.color1;
            CurrentTab.draw_gradation = dlg.DrawGradation;
            sett.caption_visible = dlg.ShowItemCaption;
            sett.cell_border = dlg.CellBorder;
            sett.transparent = dlg.TrasnparentMode;

            GridSize(grid.ColumnCount, grid.RowCount);
            FitToGrid();
            grid.Invalidate();
            Modified = true;

            return true;
        }

        private void quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void eject_Click(object sender, EventArgs e)
        {
            string fullpath = Path.GetFullPath(CurrentItem.path);
            if (fullpath.Length > 1 && fullpath[1] == ':')
                ParaParaView.Ejector.EjectMedia(fullpath[0]);
            else
                Console.WriteLine("eject error: no support path style");
        }

        private void hamburger_Click(object sender, EventArgs e)
        {
            var r = grid.GetCellDisplayRectangle(
               grid.CurrentCell.ColumnIndex, grid.CurrentCell.RowIndex, false);
            contextMenuStrip1.Show(grid, r.Left+10, r.Top+10);
        }

        private void addTab_Click(object sender, EventArgs e)
        {
            CurrentTab = AddNewTab(null, null);
            GridSize(sett.col_count, sett.row_count);
            grid.Invalidate();
        }

        // faviocn fetch in background

        static System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

        static string MakeHash(string path)
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(path);
            byte[] bs = md5.ComputeHash(data);
            string s = BitConverter.ToString(bs).Replace("-", "");
            return s;
        }

        static string MakeCacheName(string path)
        {
            return Path.Combine(cache_path, MakeHash(path)+".ico");
        }

        static string cache_path;
        static ConcurrentQueue<string> fetch_req = new ConcurrentQueue<string>();

        static WebClient wc = new WebClient();

        private void faviconFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                Directory.CreateDirectory(cache_path);
                string tempname = Path.Combine(cache_path, "downloading.ico");

                for (; ; ) {
                    if (faviconFetch.CancellationPending) {
                        e.Cancel = true;
                        return;
                    }

                    string path;
                    if (fetch_req.TryDequeue(out path)) {
                        try {
                            //if (path.StartsWith("http://") || path.StartsWith("https://"))

                            string cachename = MakeCacheName(path);
                            if (!File.Exists(cachename)) {
                                var u = new Uri(path);
                                string favicon = u.GetLeftPart(UriPartial.Authority) + "/favicon.ico";
                                wc.DownloadFile(favicon, tempname);

                                File.Move(tempname, cachename);
                            }

                            faviconFetch.ReportProgress(fetch_req.Count);
                        } catch (Exception ex) {
                            Console.WriteLine("fetch error: "+ex.Message);
                        }
                    } else
                        System.Threading.Thread.Sleep(100);
                }
            } catch (Exception ex) {
                Console.WriteLine("faviconFetch give up; "+ex.Message);
            }
        }

        private void faviconFetch_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Console.WriteLine("fetch {0} {1}", e.ProgressPercentage, e.UserState);
            if (e.ProgressPercentage == 0)
                grid.Invalidate();
        }

    }
}
