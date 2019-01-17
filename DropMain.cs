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

//using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO; // FileSystem.
using System.Reflection;
using ParaParaView;

// TODO
// change tab order
// item move to other tab
// custom tab display
// bug: too small icon (youtube ...)
// modless settings dialog

// hot key
// cell drawing too slow
// other icon size
// multiple dock
// double click item

namespace DropThing3
{
    public partial class DropMain: Form
    {
        const string APP_NAME = "DropThing3";

        // file extension for DropThing settings
        const string DROPTHING_EXT = "dtIII";
        string appdata; // application data path C:\Users\ ..USER.. \AppData\Local\DropThing3

        public DropMain()
        {
            InitializeComponent();
            main_form = this;

            var asm = Assembly.GetExecutingAssembly();
            var ver = asm.GetName().Version;
            AppStatusText(STM.NORMAL, "{0} version {1}.{2:D2}; {3}",
               APP_NAME, ver.Major, ver.Minor, "application launcher");
            about.Text = string.Format("{0} ver{1}.{2:D2}", APP_NAME, ver.Major, ver.Minor);

            Directory.SetCurrentDirectory(@"C:\");

            string local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            appdata = Path.Combine(local, APP_NAME);
            Directory.CreateDirectory(appdata);
            sett_filename = Path.Combine(appdata, APP_NAME+"."+DROPTHING_EXT);

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
                sett_filename = filenames[0];
            LoadSettings(new_settings ? "" : sett_filename);
            sett.app_version = string.Format("{0}.{1:D2}", ver.Major, ver.Minor);

            // open other instances
            for (int i = 1; i < filenames.Count; i++)
                Process.Start(Application.ExecutablePath, filenames[i]);

            //this.Font.Name = "";
            RegStartup(true);

            cache_path = Path.Combine(appdata, "icon_cache");
            faviconFetch.RunWorkerAsync();

            //atchRemoval();
            Ejector.StartWatch(RemovalNotify);
#if DEBUG
            dbg_makbak = true;
            dbgSave.Visible = true;
#endif
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

                //if (Modified) // alt_infoを保尊するため常に　
                SaveSettings();
            } catch (Exception ex) {
                MessageBox.Show(ex.Message, "FATAL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // resize

        private void resize_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouse_down_flag) {
                // resizing now
                this.Width += e.Location.X - mouse_down_x;
                this.Height += e.Location.Y - mouse_down_y;
                //GridSize(grid.Width / CellWidth, grid.Height / CellHeight);
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

        private void DropMain_ResizeBegin(object sender, EventArgs e)
        {
            //Console.WriteLine("DropMain_ResizeBegin()");
        }

        private void DropMain_ResizeEnd(object sender, EventArgs e)
        {
            Console.WriteLine("DropMain_ResizeEnd()");
            //GridSize(grid.Width / CellWidth, grid.Height / CellHeight);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cols"></param>
        /// <param name="rows"></param>
        void GridSize(int cols, int rows)
        {
            for (int t = 0; t < tabControl1.TabCount; t++)
                if ((tabControl1.TabPages[t].Tag as TabLayer).id == sett.current_tab) {
                    tabControl1.SelectedIndex = t;
                    break;
                }

            if (grid.ColumnCount != cols || grid.RowCount != rows) {
                AppStatusText(STM.RESIZE, "{0} x {1}", cols, rows);
                Modified |= true;
            }

            CellWidth = sett.caption_visible ? 80 : M+32+M;
            CellHeight = sett.caption_visible ? M+32+17+M : M+32+M;

            MinimumSize = new Size(CellWidth, grid.Top + CellHeight + status.Height);

            if (CurrentTab.image != null)
                cell_bitmap = CurrentTab.image;
            else
                cell_bitmap = CellBitmap();
            status.BackColor = color0;
            status.ForeColor = ColorUtl.TextColor(color0);

            grid.BackgroundColor = CurrentTab.color1;
            grid.ColumnCount = Math.Max(cols, 1);
            grid.RowCount = Math.Max(rows, 1);
            foreach (DataGridViewColumn col in grid.Columns)
                col.Width = CellWidth;
            foreach (DataGridViewRow row in grid.Rows)
                row.Height = CellHeight;
            grid.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        void FitToGrid()
        {
            if (sett.titlebar) {
                this.FormBorderStyle = FormBorderStyle.Sizable;
            } else {
                this.FormBorderStyle = FormBorderStyle.None;
            }
            int mx = this.Width - this.ClientSize.Width;
            int my = this.Height - this.ClientSize.Height;
            this.Size = new Size(
               grid.ColumnCount * CellWidth + mx,
             Y0 + grid.RowCount * CellHeight + status.Height + my);
        }

        // message

        static DropMain main_form;

        /// <summary>
        /// 
        /// </summary>
        public enum STM
        {
            // basic color code
            NORMAL = 0x000000,  // black
            ERROR = 0xFF0000,   // fuchsia
            DEBUG = 0x808080,   // gray

            // others, text align ...
            RIGHT = 0x01000000, //

            // complex
            RESIZE = NORMAL | RIGHT
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public static void AppStatusText(STM mode, string message, params object[] args)
        {
            if (main_form != null) {
                //Color color = Color.FromArgb(mode & 0xFFFFFF);
                if (mode.HasFlag(STM.RIGHT))
                    main_form.status.TextAlign = ContentAlignment.TopRight;
                else
                    main_form.status.TextAlign = ContentAlignment.TopLeft;

                main_form.status.Text = string.Format(message, args);
            }
        }

        static void Application_ThreadException(object sender,
           System.Threading.ThreadExceptionEventArgs e)
        {
            DropMain.AppStatusText(STM.ERROR, e.Exception.Message);
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
            string _path;

            [XmlElement("path")]
            public string path
            {
                get { return _path; }
                set
                {
                    string ext = Path.GetExtension(value);
                    if (ext == ".lnk") {    // Windows shortcut
                        var shell = new IWshRuntimeLibrary.WshShell();
                        var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(value);
                        //Console.WriteLine("{0}, {1}", shortcut.FullName, shortcut.Description);
                        _path = shortcut.TargetPath.ToString();
                        this.Caption = Path.GetFileNameWithoutExtension(value);
                        this.options = shortcut.Arguments;
                        this.dir = shortcut.WorkingDirectory;
                        this.hotkey = shortcut.Hotkey;

                        // remove icon index? "YYY\XXX.ico?0"
                        this.icon_file = shortcut.IconLocation;
                        int k = this.icon_file.IndexOf(',');
                        if (k >= 0)
                            this.icon_file = this.icon_file.Substring(0, k);
                        if (this.icon_file.Length == 0)
                            this.icon_file = null;
                    } else if (ext == ".url") { // InternetShortcut
                        string[] lines = File.ReadAllLines(value);
                        foreach (string line in lines) {
                            if (line.StartsWith("URL="))
                                _path = line.Substring(4);
                            if (line.StartsWith("IconFile="))
                                this.icon_file = line.Substring(9);
                        }
                        this.Caption = Path.GetFileNameWithoutExtension(value);
                    } else {
                        _path = value;
                        if (Caption == "")
                            Caption = DefCaption();
                    }
                }
            }

            public string icon_file;
            public string options;
            public string dir;
            public string hotkey;
            public string attr = "";
            //public ATTR Attr;
            public string alt_info;

            public int row, col;
            public uint tab;

            [XmlIgnore]
            public Bitmap icon;

            /// <summary>
            /// 
            /// </summary>
            public CellItem()
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="source"></param>
            public CellItem(string source)
            {
                this.path = source;
                //this.UpdateIcon();
            }

            public CellItem Clone()
            {
                var item = new CellItem(this.path);
                item.CopyFrom(this);
                return item;
            }

            public void CopyFrom(CellItem item)
            {
                this.path = item.path;
                this.Caption = item.Caption;
                this.tab = item.tab;
                this.col = item.col;
                this.row = item.row;
                this.attr = item.attr;
                this.icon_file = item.icon_file;
                this.icon = item.icon;
                this.dir = item.dir;
            }

            [Flags]
            public enum ATTR {
                UNC = 0x0001,
                URL = 0x0002,
                FILE = 0x0020,
                DIR = 0x0080,
                EXE = 0x0010,
                REMOVAL = 0x4000, 
                NO_MEDIA = 0x8000
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            /// <returns></returns>
            public bool HasAttr(char c)
            {
                return attr.IndexOf(c) >= 0;
            }

            /// <summary>
            ///  
            /// </summary>
            /// <param name="c"></param>
            public void AddAttr(char c)
            {
                if (!HasAttr(c))
                    attr += c;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            public void RemoveAttr(char c)
            {
                this.attr = this.attr.Replace(c.ToString(), "");
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="c"></param>
            /// <param name="value"></param>
            public void ToggleAttr(char c, bool value)
            {
                if (value)
                    AddAttr(c);
                else
                    RemoveAttr(c);
            }

            public void ToggleAttr(char c)
            {
                if (HasAttr(c))
                    RemoveAttr(c);
                else
                    AddAttr(c);
            }

            List<string> executables = new List<string>() { ".exe", ".com", "*.bat" };

            /// <summary>
            /// 
            /// </summary>
            public void UpdateIcon()
            {
                string path = this.path;
                string cachename = MakeCacheName(path);

                // 1. get from cache if exists
                if (this.icon == null && File.Exists(cachename))
                    try {
                        this.icon = (Bitmap)Image.FromFile(cachename);
                    } catch (Exception ex) {
                        this.icon = null;
                        Console.WriteLine("UpdateIcon(); "+ex.Message);
                    }

                var u = new Uri(path);

                if (path.StartsWith(@"http://") || path.StartsWith(@"https://")) {
                    this.AddAttr('U');
                    // 2. from web (URL)
                    if (this.icon == null && this.icon_file == null)
                        fetch_req.Enqueue(path);
                } else {
                    if (u.IsUnc)
                        AddAttr('V');
                    else if (path.StartsWith(@"file:///"))
                        path = u.LocalPath;
                    else
                        path = Path.GetFullPath(path);

                    GetDriveInfo();

                    if (Directory.Exists(path))
                        this.AddAttr('d');
                    else if (File.Exists(path)) {
                        this.AddAttr('f');
                        string ext = Path.GetExtension(path);
                        if (executables.IndexOf(ext) >= 0)
                            this.AddAttr('x');
                    }
                }

                //string icon_file = (this.icon_file == null && !this.HasAttr('U'))
                //   ? path : this.icon_file;
                string icon_file = this.icon_file != null ? this.icon_file : path;

                if (this.icon == null)
                    try {
                        var icon = Icon.ExtractAssociatedIcon(icon_file);
                        this.icon = icon.ToBitmap();
#pragma warning disable CS0168 // 変数 'ex' は宣言されていますが、使用されていません。
                    } catch (Exception ex) {
#pragma warning restore CS0168 // 変数 'ex' は宣言されていますが、使用されていません。
                        this.icon = null;
                    }

                if (this.icon == null) {
                    var icon = GetIconAPI.Get(icon_file);
                    if (icon != null)
                        this.icon = icon.ToBitmap();
                }

                // save icon cache
                if ((HasAttr('J') || HasAttr('V'))
                 && this.icon != null && !File.Exists(cachename)) {
                    this.icon.Save(cachename);
                }
            }

            public string GetLocalPath()
            {
                if (HasAttr('U') || HasAttr('V'))
                    return null;
                if (path.StartsWith(@"file://"))
                    return Path.GetFileName(path.Substring(5));
                return Path.GetFullPath(this.path);
            }

            public DriveInfo GetDriveInfo()
            {
                try {
                    string fullpath = Path.GetFullPath(this.path);
                    if (fullpath[1] == ':') {
                        var d = new DriveInfo(fullpath.Substring(0, 1));

                        switch (d.DriveType) {
                        case DriveType.CDRom:
                        case DriveType.Network:
                        case DriveType.Removable:
                            AddAttr('J');
                            break;
                        case DriveType.Fixed:
                            RemoveAttr('J');
                            break;
                        }

                        if (HasAttr('J')) {
                            if (d.IsReady) {
                                float f = (float)d.TotalFreeSpace;
                                float t = (float)d.TotalSize;
                                string[] units = { "B", "KB", "MB", "GB", "TB" };
                                int u = 0;
                                for (; f >= 1024f && u+1 < units.Length;) {
                                    f /= 1024f;
                                    t /= 1024f;
                                    u++;
                                }
                                this.alt_info = string.Format(" \"{0}\", free {1:F1}/{2:F1} {3}",
                                   d.VolumeLabel, f, t, units[u]);

                                this.RemoveAttr('M');
                            } else
                                this.AddAttr('M');
                        }
                        return d;
                    }
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                }
                return null;
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
                    info.Arguments = this.options + escapedJoin(args);
                    info.WorkingDirectory = this.dir;
                    // TODO: expand environments

                    //info.Environment
                    Process.Start(info);
                } catch (Exception ex) {
                    MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //AppStatusText(Color.Fuchsia, ex.Message);
                }
            }

            static string escapedJoin(string[] nn)
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

            string caption = "";

            [XmlElement("caption")]
            public string Caption { get { return caption; } set { caption = value; } }

            string DefCaption()
            { 
                // return default caption if not specified
                if (this.HasAttr('U')) {
                    try {
                        var u = new Uri(this.path);
                        string[] ll = u.LocalPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                        int l = ll.Length-1;
                        if (l > 0 && ll[l] == "index.html")
                            l--;
                        if (l >= 0)
                            return ll[l];

                        IPAddress addr = IPAddress.Any;
                        if (IPAddress.TryParse(u.Host, out addr))
                            return u.Host;  // 192.168.yyy.xxx

                        // www.AAAA. ...co.jp -> AAAA
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

            string GetIconFile()
            {
                if (icon_file == null && !HasAttr('U'))
                    return path;
                return icon_file;
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

            /// <summary>
            /// 
            /// </summary>
            public string texture
            {
                get { return _texture; }
                set
                {
                    _texture = value;
                    image = null;
                    if (File.Exists(_texture)) {
                        image = Image.FromFile(_texture);
                    }
                }
            }

            string _texture;
            [XmlIgnore]
            public Image image { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public STYLE style = STYLE.NORMAL;
            public ORIGIN origin;

            /// <summary>
            /// 
            /// </summary>
            public enum STYLE
            {
                NORMAL = 0,
            }

            /// <summary>
            /// 
            /// </summary>
            public enum ORIGIN { UNKNOWN, SCREEN, TAB, CELL }
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

            public uint current_tab;

            public FormWindowState win_state;
            public int left, top;
            public int col_count, row_count;

            public bool caption_visible;
            public bool cell_border;
            public bool transparent;
            public bool titlebar;

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

            //Dictionary<string, string> alt_strings = new Dictionary<string, string>();
        }

        /// <summary>
        /// 
        /// </summary>
        public DropThingSettings sett;
        string sett_filename;
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
                    sett = new DropThingSettings();
                    dbg_makbak = true;
                }

                //this.WindowState = dock.win_state;
                this.StartPosition = FormStartPosition.Manual;
                this.Left = sett.left;
                this.Top = sett.top;

            } else
                sett = new DropThingSettings();

            if (sett.tab_list.Count < 1)
                AddNewTab(null, null); // at least 1 tab

            RestoreTabs();

            GridSize(sett.col_count, sett.row_count);
            FitToGrid();
            RestoreTabs();
            Modified = false;
        }

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

            string tmpname = Path.ChangeExtension(sett_filename, ".$$$");
            using (var sw = new StreamWriter(tmpname, false, Encoding.UTF8)) {
                var serializer = new XmlSerializer(typeof(DropThingSettings));
                serializer.Serialize(sw, sett);
            }

            if (dbg_makbak)
                try {
                    string bak = Path.ChangeExtension(sett_filename,
                       DateTime.Now.ToString("yyyyMMdd-HHmmss"))
                      + Path.GetExtension(sett_filename);
                    FileSystem.MoveFile(sett_filename, bak, true);
                } catch (Exception ex) {
                    Console.WriteLine("make .bak: " + ex.Message);
                }

            FileSystem.MoveFile(tmpname, sett_filename, true);
        }

        bool dbg_makbak = false;

        /// <summary>
        /// 
        /// </summary>
        const int AUTO_SAVE_DELAY = 30;

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
                    Modified = false;   // 
                    SaveSettings();
                    AppStatusText(STM.NORMAL, "auto save done.");
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
        void AddNewTab(object sender, EventArgs args)
        {
            var tab = new TabLayer();

            // random color
            tab.color0 = ColorUtl.RandomNamedColor();
            tab.color1 = ColorUtl.RandomNamedColor();
            sett.tab_list.Add(tab);
            var tabpage = new TabPage(tab.title);
            tabpage.Tag = tab;
            tabControl1.TabPages.Add(tabpage);
            //tabControl1.

            CurrentTab = tab;
        }

        void DeleteCurrentTab(object sender, EventArgs args)
        {
            if (sett.tab_list.Count > 1) {
                int index = sett.tab_list.IndexOf(CurrentTab);

                sett.cell_list.RemoveAll(cell => cell.tab == CurrentTab.id);
                sett.tab_list.Remove(CurrentTab);

                tabControl1.TabPages.RemoveAt(index);

                if (index >= sett.tab_list.Count)
                    index--;
                CurrentTab = sett.tab_list[index];

                //grid.Invalidate();
            }
        }

        void RestoreTabs()
        {
#pragma warning disable CS0219 // 変数 'x' は割り当てられていますが、その値は使用されていません。
            int x = 0;
#pragma warning restore CS0219 // 変数 'x' は割り当てられていますが、その値は使用されていません。
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
            CurrentTab = e.TabPage.Tag as TabLayer;
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            var tab = sett.tab_list[e.Index];

            using (var br = new SolidBrush(tab.color0))
                e.Graphics.FillRectangle(br, e.Bounds);
            using (var br = new SolidBrush(ColorUtl.TextColor(tab.color0)))
                e.Graphics.DrawString(tab.title, this.Font, br, e.Bounds.Left+2, e.Bounds.Top+3);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="col"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        CellItem GetItemAt(int col, int row)
        {
            var found = sett.cell_list.Where(c =>
               c.tab == CurrentTab.id && c.col == col && c.row == row);
            if (found.Count() > 0)
                return found.First();
            return null;
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

        CellItem NewCellItem(string path, DataGridViewCell cell)
        {
            return NewCellItem(path, cell.ColumnIndex, cell.RowIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="col"></param>
        /// <param name="row"></param>
        void MoveCell(CellItem item, int col, int row)
        {
            undo_buf = new ChangeCellUndo(item);

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
                int col = grid.CurrentCell.ColumnIndex,
                    row = grid.CurrentCell.RowIndex;
                return GetItemAt(col, row);
            }
            //set { }
        }

        /// <summary>
        /// 
        /// </summary>
        TabLayer CurrentTab
        {
            get 
            {
                try {
                    return sett.tab_list.First(t => t.id == sett.current_tab);
                } catch (Exception) { }
                return sett.tab_list[0];
            }
            set
            {
                if (sett.current_tab != value.id) {
                    sett.current_tab = value.id;
                    GridSize(grid.ColumnCount, grid.RowCount);
                    Modified = true;

                    if (dlg.Visible)
                        dlg.ShowModeless();
                }
            }
        }

        // drawing
        const int M = 3;
        int Y0 { get { return grid.Top; } }
        int CellWidth = M+32+M;
        int CellHeight = M+32+M;

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

            resize.BackColor = color0;

            var bmp = new Bitmap(CellWidth, CellHeight);
            using (var g = Graphics.FromImage(bmp))
            using (var light = new Pen(ColorUtl.TrimColor(color0, +20)))
            using (var dark = new Pen(ColorUtl.TrimColor(color1, -20)))
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

        int hover_col = -1, hover_row;

        void hover_effect(int col, int row)
        {
            // refresh leaved from
            if (hover_col >= 0)
                grid.InvalidateCell(hover_col, hover_row);

            hover_col = col;
            hover_row = row;

            // refresh enter to
            if (hover_col >= 0)
                grid.InvalidateCell(hover_col, hover_row);
        }

        private void grid_DragOver(object sender, DragEventArgs e)
        {
            var point = grid.PointToClient(new Point(e.X, e.Y));
            var hit = grid.HitTest(point.X, point.Y);
            if (e.Effect != DragDropEffects.None
             && hit.Type == DataGridViewHitTestType.Cell
             && (hover_col != hit.ColumnIndex || hover_row != hit.RowIndex)) {
                hover_effect(hit.ColumnIndex, hit.RowIndex);

                CellItem item = GetItemAt(hover_col, hover_row);
                string s = "";
                if (item == null)
                    s = " will be placed here";
                else if (item.HasAttr('x'))
                    s = string.Format("open by {0}", item.Caption);
                else if (item.HasAttr('M'))
                    s = string.Format("{0} is NOT ready", item.Caption);
                else if (item.HasAttr('d'))
                    s = string.Format("copy to {0}", item.Caption);
                else
                    s = info_text(item);

                AppStatusText(STM.NORMAL, "[{0},{1}] {2}", hover_col, hover_row, s);
            }
        }

        private void grid_DragDrop(object sender, DragEventArgs e)
        {
            string[] names;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                names = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            else if (e.Data.GetDataPresent("UniformResourceLocator")) {
                string url = e.Data.GetData(DataFormats.Text).ToString();
                if (url.IndexOf(@"://") < 0)
                    url = "http://" + url;
                names = new string[] { url };
            } else
                return;

            var point = grid.PointToClient(new Point(e.X, e.Y));
            var hit = grid.HitTest(point.X, point.Y);
            if (hit.Type == DataGridViewHitTestType.Cell) {
                var item = GetItemAt(hit.ColumnIndex, hit.RowIndex);
                if (item != null) {
                    if (item == drag_item)
#pragma warning disable CS0642 // empty ステートメントが間違っている可能性があります
                        ;// AppStatusText(STM.DEBUG, "cancel self drop");
#pragma warning restore CS0642 // empty ステートメントが間違っている可能性があります
                    else if (item.HasAttr('d')) {
                        // drop files to directory
                        foreach (string name in names)
                            try {
                                string dest = Path.Combine(item.path, Path.GetFileName(name));
                                if (Directory.Exists(name))
                                    FileSystem.CopyDirectory(name, dest, UIOption.AllDialogs);
                                else
                                    FileSystem.CopyFile(name, dest, UIOption.AllDialogs);
                            } catch (Exception ex) {
                                AppStatusText(STM.ERROR, "ERROR: " + ex.Message);
                                break;
                            }

                    } else {
                        // drop files to app icon; execute application
                        item.ProcessStart(names);
                    }
                } else {
                    if (drag_item != null) {
                        // moving inner form
                        MoveCell(drag_item, hit.ColumnIndex, hit.RowIndex);
                    } else {
                        // drop file to empty cell; register file to cell
                        int col = hit.ColumnIndex, row = hit.RowIndex;
                        var items = new List<CellItem>();
                        foreach (var name in names)
                            if (FindEmptyCell(ref col, ref row))
                                items.Add(NewCellItem(name, col, row));
                            else {
                                AppStatusText(STM.ERROR, "ERROR: give up too many files");
                                break;
                            }
                        undo_buf = new AddCellsUndo(items);
                    }
                }
                //AppStatusText(STM.NORMAL, "drop {0}, {1}: {2}", hit.ColumnIndex, hit.RowIndex, names[0]);
            }

            drag_item = null;
        }

        bool FindEmptyCell(ref int col, ref int row)
        {
            for (; ; ) {
                if (GetItemAt(col, row) == null)
                    return true;
                if (++col >= grid.ColumnCount) {
                    if (++row >= grid.RowCount)
                        return false;
                    col = 0;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        string info_text(CellItem item)
        {
            if (item == null)
                return ""; // "none"; or emepty

            // append original path
            string s = item.Caption;

            if (item.HasAttr('d')) {
                if (/*item.caption == null || */item.Caption == "")
                    s = item.path;
            } else if (s != item.path)
                s += "; " + item.path;

            // append removal media info
            if (item.HasAttr('J') && item.HasAttr('d')) {
                item.GetDriveInfo();
                s += item.alt_info;
            }

            return s;
        }

        private void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            // show mouse over cell information
            CellItem item = GetItemAt(e.ColumnIndex, e.RowIndex);
            AppStatusText(STM.NORMAL, "[{0},{1}] {2}", e.ColumnIndex, e.RowIndex, info_text(item));
            grid.Cursor =  (item != null) ? Cursors.Hand : Cursors.Default;
            hover_effect(e.ColumnIndex, e.RowIndex);
        }

        private void grid_MouseLeave(object sender, EventArgs e)
        {
            hover_effect(-1, -1);
        }

        int last_col = -1, last_row = -1;

        private void grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (last_col != e.ColumnIndex || last_row != e.RowIndex) {
                last_col = e.ColumnIndex;
                last_row = e.RowIndex;

                AppStatusText(STM.NORMAL, "[{0},{1}] {2}", e.ColumnIndex, e.RowIndex, info_text(CurrentItem));

                // update menu
                deleteItem.Enabled = (CurrentItem != null);
                openItem.Enabled = (CurrentItem != null);
                eject.Enabled = (CurrentItem != null) && CurrentItem.HasAttr('J');
                if (eject.Enabled) {
                    var drive = CurrentItem.GetDriveInfo();
                    if (drive.IsReady)
                        eject.Text = "e&Ject " + drive.Name + " " + drive.VolumeLabel;
                    else
                        eject.Enabled = false;
                }
                undo.Enabled = undo_buf != null;
            }
        }

        private void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
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
            if (e.Button == MouseButtons.Left) {
                var b = grid.CurrentCell.ContentBounds;
                int x, y;
                if (sett.caption_visible) {
                    x = b.Right-16-8;
                    y = b.Bottom-32-2;
                } else {
                    x = b.Right-16;
                    y = b.Bottom-16;
                }

                if (e.Clicks == 1) {
                    // eject mini button
                    if (CurrentItem != null && CurrentItem.HasAttr('J')
                     && e.X >= x && e.Y >= y)
                        eject_Click(null, null);
                    else

                    // cell click
                    if (ModifierKeys.HasFlag(Keys.Shift))
                        explorerItem_Click(null, null);
                    else if (ModifierKeys.HasFlag(Keys.Control))
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
            g.SetClip(e.CellBounds);
            if (CurrentTab.image != null)
                //DrawTile(g, CurrentTab.image, e.CellBounds, CurrentTab.origin);
                DrawTile(g, CurrentTab.image, e.CellBounds, TabLayer.ORIGIN.SCREEN);
            else
                g.DrawImage(cell_bitmap, e.CellBounds.X, e.CellBounds.Y);

            // draw focus rectangle
            if (e.State.HasFlag(DataGridViewElementStates.Selected))
                DrawFocusCell(g, e.CellBounds);
            if (hover_col == e.ColumnIndex && hover_row == e.RowIndex)
                DrawHoverCell(g, e.CellBounds);

            // draw item icon
            var item = GetItemAt(e.ColumnIndex, e.RowIndex);
            if (item != null) {
                if (item.icon == null)
                    item.UpdateIcon();

                if (item.icon != null) {
                    int w = item.icon.Width;
                    if (w > 32)
                        w = 32;
                    int ix = e.CellBounds.X + (e.CellBounds.Width - w)/2;
                    int iy = e.CellBounds.Y + (e.CellBounds.Height - w)/2;
                    if (sett.caption_visible)
                        iy -= 6;
                    if (!item.HasAttr('M')) {
                        if (w < 24)
                            w = 24;
                        g.DrawImage(item.icon, ix, iy, w, w);
                    } else
                        ControlPaint.DrawImageDisabled(g, item.icon, ix, iy, color1);
                } else {
                    string alt = item.HasAttr('U') ? "URL" : "?";
                    var f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;
                    ControlPaint.DrawStringDisabled(g, alt, missing.Font, color0, e.CellBounds, f);
                }

                // draw item caption
                if (sett.caption_visible)
                    DrawCaption(g, item.Caption, e.CellBounds);

                if (item.HasAttr('J')) {
                    int x, y;
                    if (sett.caption_visible) {
                        x = e.CellBounds.Right-16-8;
                        y = e.CellBounds.Bottom-32-2;
                    } else {
                        x = e.CellBounds.Right-16;
                        y = e.CellBounds.Bottom-16;
                    }

                    var eject_icon = item.HasAttr('M') ? Properties.Resources.eject_dis : Properties.Resources.eject;
                    g.DrawImage(eject_icon, x, y);
                }
            }

            //e.Paint(e.CellBounds, e.PaintParts & ~DataGridViewPaintParts.Background);
            e.Handled = true;
        }

        void DrawFocusCell(Graphics g, Rectangle r)
        {
            using (var br = new SolidBrush(Color.FromArgb(15, ColorUtl.TextColor(color0))))
            using (var pen = new Pen(Color.FromArgb(90, Color.Black), 1)) {
                g.FillRectangle(br, r.X+2, r.Y+2, r.Width-4, r.Height-4);
                pen.DashStyle = DashStyle.Dot;
                g.DrawLine(pen, r.Left+2, r.Top+1, r.Right-3, r.Top+1);
                g.DrawLine(pen, r.Left+2, r.Bottom-2, r.Right-3, r.Bottom-2);
                g.DrawLine(pen, r.Left+1, r.Top+2, r.Left+1, r.Bottom-3);
                g.DrawLine(pen, r.Right-2, r.Top+2, r.Right-2, r.Bottom-3);
            }
        }

        void DrawHoverCell(Graphics g, Rectangle r)
        {
            using (var br = new SolidBrush(Color.FromArgb(25, ColorUtl.TextColor(color0)))) {
                g.FillRectangle(br, r.X+3, r.Y+3, r.Width-6, r.Height-6);
            }
        }

        void DrawTile(Graphics g, Image image, Rectangle bounds, TabLayer.ORIGIN origin)
        {
            Point o = (origin == TabLayer.ORIGIN.CELL) ? bounds.Location : new Point(0, 0);
            if (origin == TabLayer.ORIGIN.SCREEN)
                o = grid.PointToClient(o);

            for (int y = o.Y; y < bounds.Bottom; y += image.Height)
                for (int x = o.X; x < bounds.Right; x += image.Width)
                    g.DrawImage(CurrentTab.image, x, y);
        }

        void DrawCaption(Graphics g, string caption, Rectangle bounds)
        {
            var m = g.MeasureString(caption, this.Font);
            Color color = ColorUtl.TextColor(color1);
            var rect = new RectangleF(bounds.Left, bounds.Top + 32 +6, bounds.Width, m.Height);
            var f = new StringFormat();
            f.Alignment = StringAlignment.Center;
            if (m.Width <= rect.Width) {
                using (var br = new SolidBrush(color))
                    g.DrawString(caption, this.Font, br, rect, f);
            } else {
                RectangleF rect0 = rect;
                rect0.Width -= 20;

                RectangleF rect1 = rect;
                rect1.X = rect0.Right;
                rect1.Width = 20;

                g.SetClip(rect0);
                using (var br0 = new SolidBrush(color))
                    g.DrawString(caption, this.Font, br0, rect, f);
                g.SetClip(rect1);
                using (var br1 = new LinearGradientBrush(rect1, color, color1, 30f))
                    g.DrawString(caption, this.Font, br1, rect, f);
                g.ResetClip();
            }
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
                if (e.Shift)
                    explorerItem_Click(null, null);
                else if (e.Control)
                    propertyItem_Click(null, null);
                else
                    openItem_Click(null, null);
                e.Handled = true;
            }
        }

        private void explorerItem_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            if (CurrentItem == null) {
                Process.Start("EXPLORER.EXE");
                AppStatusText(STM.NORMAL, "explorer");
            } else if (!CurrentItem.HasAttr('U')) {
                Process.Start("EXPLORER.EXE", @"/select,""" + CurrentItem.path + @"""");
                AppStatusText(STM.NORMAL, "explorer {0}", CurrentItem.path);
            } else {
                //
            }
        }

        private void deleteItem_Click(object sender, EventArgs e)
        {
            if (CurrentItem != null) {
                undo_buf = new DeleteCellUndo(CurrentItem);

                sett.cell_list.Remove(CurrentItem);
                grid.InvalidateCell(grid.CurrentCell);
                Modified = true;
            }
        }


        // UNDO

        abstract class UndoRec: Object
        {
            //public abstract object Undo();
        }

        class AddCellsUndo: UndoRec
        {
            public List<CellItem> Items { get; protected set; }

            public AddCellsUndo(List<CellItem> items)
            {
                this.Items = items;
            }
        }

        class CellUndo: UndoRec
        {
            public CellItem Item { get; protected set; }

            public CellUndo(CellItem item)
            {
                this.Item = item;
            }
        }

        class NewCellUndo: CellUndo
        {
            public NewCellUndo(CellItem item) : base(item) { }
        }

        class DeleteCellUndo: CellUndo
        {
            public DeleteCellUndo(CellItem item) : base(item) { }
        }

        class ChangeCellUndo: CellUndo
        {
            public CellItem Bak { get; protected set; }

            public ChangeCellUndo(CellItem item) : base(item)
            {
                this.Bak = item.Clone();
            }
        }

        UndoRec undo_buf = null;

        private void undo_Click(object sender, EventArgs e)
        {
            if (undo_buf == null)
                return;

            if (undo_buf is CellUndo) {
                var item = (undo_buf as CellUndo).Item;

                if (undo_buf is DeleteCellUndo)
                    sett.cell_list.Add(item);
                else if (undo_buf is ChangeCellUndo) {
                    grid.InvalidateCell(item.col, item.row);
                    var bak = (undo_buf as ChangeCellUndo).Bak;
                    item.CopyFrom(bak);
                } else if (undo_buf is NewCellUndo)
                    sett.cell_list.Remove(item);

                grid.InvalidateCell(item.col, item.row);
            } else if (undo_buf is AddCellsUndo) {
                foreach (var item in (undo_buf as AddCellsUndo).Items) {
                    sett.cell_list.Remove(item);
                    grid.InvalidateCell(item.col, item.row);
                }
            }

            undo_buf = null;
        }

        private void propertyItem_Click(object sender, EventArgs e)
        {
            var dlg = new ItemDialog();

            dlg.OnOpen += (_, __) =>
            {
                if (CurrentItem != null) {
                    dlg.ItemCaption = CurrentItem.Caption;
                    dlg.FilePath = CurrentItem.path;
                    dlg.CommandOptions = CurrentItem.options;
                    dlg.WorkingDirectory = CurrentItem.dir;
                } else {
                    dlg.ItemCaption = "";
                    dlg.FilePath = "";
                    dlg.CommandOptions = "";
                    dlg.WorkingDirectory = "";
                }
            };

            dlg.OnAccept += (_) =>
            {
                CellItem item;
                if (CurrentItem != null) {
                    item = CurrentItem;
                    undo_buf = new ChangeCellUndo(item);
                } else {
                    item = NewCellItem(dlg.FilePath, grid.CurrentCell);
                    undo_buf = new NewCellUndo(item);
                }
                item.Caption = dlg.ItemCaption;
                item.path = dlg.FilePath;
                item.options = dlg.CommandOptions;
                item.dir = dlg.WorkingDirectory;
                Modified = true;
                grid.InvalidateCell(grid.CurrentCell);
                return true;
            };

            dlg.Popup();
        }

        // tab settings
        Color color0, color1;
        //Bitmap cell_bitmap;
        Image cell_bitmap;

        TabDialog dlg = new TabDialog();

        private void tabItem_Click(object sender, EventArgs e)
        {

            dlg.OnOpen += (_, __) =>
            {
                dlg.TabTitle = CurrentTab.title;
                dlg.Color0 = CurrentTab.color0;
                dlg.Color1 = CurrentTab.color1;
                dlg.DrawGradation = CurrentTab.draw_gradation;
                dlg.TexturePath = CurrentTab.texture;
                dlg.ShowItemCaption = sett.caption_visible;
                dlg.CellBorder = sett.cell_border;
                dlg.TrasnparentMode = sett.transparent;
                dlg.TitleBar = sett.titlebar;
            };

            dlg.OnAccept += (_) =>
            {
                CurrentTab.title = dlg.TabTitle;
                CurrentTab.color0 = dlg.Color0;
                CurrentTab.color1 = dlg.Color1;
                CurrentTab.draw_gradation = dlg.DrawGradation;
                CurrentTab.texture = dlg.TexturePath;
                sett.caption_visible = dlg.ShowItemCaption;
                sett.cell_border = dlg.CellBorder;
                sett.transparent = dlg.TrasnparentMode;
                sett.titlebar = dlg.TitleBar;
                Modified = true;

                tabControl1.Invalidate();
                GridSize(grid.ColumnCount, grid.RowCount);
                FitToGrid();
                return true;
            };

            dlg.OnDelete += DeleteCurrentTab;

            dlg.OnAddNew += AddNewTab;

            //dlg.Popup();
            dlg.ShowModeless();
        }

        private void dbgSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
            Modified = false;
            AppStatusText(STM.NORMAL, "debug: save done.");
        }

        private void quit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void about_Click(object sender, EventArgs e)
        {
            MessageBox.Show("YET");
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


        void RemovalNotify(object sender, RemovalEventArgs e)
        {
            if (e.Status == RemovalStatus.INSERTED)
                AppStatusText(STM.DEBUG, "inserted {0}:", e.DriveLetter);
            else if (e.Status == RemovalStatus.EJECTED)
                AppStatusText(STM.DEBUG, "ejected {0}:", e.DriveLetter);

            sett.cell_list.ForEach((item) =>
            {
                if (item.HasAttr('J') && item.path.StartsWith(e.DriveLetter+":"))
                    item.ToggleAttr('M', e.Status != RemovalStatus.INSERTED);
            });
            grid.Invalidate();
        }

        // faviocn fetch in background

        static System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();

        static string MakeHash(string path)
        {
            byte[] data = Encoding.UTF8.GetBytes(path);
            byte[] bs = md5.ComputeHash(data);
            string s = BitConverter.ToString(bs);
            return s.Replace("-", "");
        }

        static string MakeCacheName(string path)
        {
            //return Path.Combine(cache_path, MakeHash(path)+".ico");
            return Path.Combine(cache_path, MakeHash(path)+".png");
        }

        static string cache_path;

        static ConcurrentQueue<string> fetch_req = new ConcurrentQueue<string>();
        static WebClient wc = new WebClient();

        private void faviconFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            try {
                Directory.CreateDirectory(cache_path);
                string tempname = Path.Combine(cache_path, "downloading.png");

                for (; !faviconFetch.CancellationPending;) {
                    string path;
                    if (fetch_req.TryDequeue(out path)) {
                        string cachename = MakeCacheName(path);
                        if (File.Exists(cachename))
                            continue;

                        try {
                            string favicon = "/favicon.ico";
                            try {
                                string html = wc.DownloadString(path);
                                string regex = @"<link\srel=[""']shortcut icon[""']\shref=[""'](.*?)[""']";
                                var m = Regex.Match(html, regex);
                                if (m != null && m.Groups.Count > 1) {
                                    favicon = m.Groups[1].ToString();
                                }
                            } catch (Exception ex) {
                                Console.WriteLine("" + ex.Message);
                            }

                            var u = new Uri(path);
                            if (!favicon.StartsWith("http"))
                                favicon = u.GetLeftPart(UriPartial.Authority) + favicon;
                            //wc.DownloadFile(favicon, tempname);
                            byte[] data = wc.DownloadData(favicon);
                            using (var ms = new MemoryStream(data))
                            using (var icon = new Icon(ms))
                            using (var bitmap = icon.ToBitmap())
                                bitmap.Save(tempname);

                            File.Move(tempname, cachename);
                            faviconFetch.ReportProgress(fetch_req.Count);
                        } catch (Exception ex) {
                            Console.WriteLine("fetch error: "+ex.Message);
                        }

                    } else
                        System.Threading.Thread.Sleep(100);
                }
                e.Cancel = true;
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

    /// <summary>
    /// 
    /// </summary>
    public static class ColorUtl
    {
        const int DEF_THRESHOLD = 150;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="back_color"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static Color TextColor(Color back_color, int threshold = DEF_THRESHOLD)
        {
            return IsDark(back_color, threshold) ? Color.White : Color.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public static bool IsDark(Color color, int threshold = DEF_THRESHOLD)
        {
            //return Math.Sqrt(color.R*color.R + color.G*color.G + color.B*color.B) < threshold;
            return (int)(0.2126*color.R + 0.7152*color.G + 0.0722*color.B) < threshold;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Color TrimColor(Color color, int d)
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

        static System.Random random = new System.Random();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Color RandomNamedColor()
        {
            var cc = typeof(Color).GetProperties(BindingFlags.Public | BindingFlags.Static);
            for (; ; ) {
                int i = random.Next(cc.Length);
                Color color = (Color)cc[i].GetValue(null, null);
                if (color.Name != "Transparent")
                    return color;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Color RandomColor()
        {
            return Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));
        }

    }
}