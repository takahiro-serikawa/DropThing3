using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ParaParaView
{
    /// <summary>
    /// 
    /// </summary>
    public enum RemovalStatus { NONE, EJECTED, INSERTED };

    /// <summary>
    /// 
    /// </summary>
    public class RemovalEventArgs: EventArgs
    {
        public RemovalStatus Status;
        public char DriveLetter;
    }

    /// <summary>
    /// 
    /// </summary>
    static class Ejector
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr CreateFile
            (string filename, uint desiredAccess,
                uint shareMode, IntPtr securityAttributes,
                int creationDisposition, int flagsAndAttributes,
                IntPtr templateFile);

        [DllImport("kernel32")]
        private static extern int DeviceIoControl
            (IntPtr deviceHandle, uint ioControlCode,
                IntPtr inBuffer, int inBufferSize,
                IntPtr outBuffer, int outBufferSize,
                ref int bytesReturned, IntPtr overlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        const uint FILE_SHARE_READ = 1;
        const uint FILE_SHARE_WRITE = 2;
        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint IOCTL_STORAGE_EJECT_MEDIA = 0x2D4808;    // WinIoCtrl.h
        const uint IOCTL_STORAGE_LOAD_MEDIA = 0x2D480C;
        const uint IOCTL_DISK_IS_WRITABLE = 0x70024;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drive"></param>
        static public void EjectMedia(char drive)
        {
            string path = @"\\.\" + drive + @":";
            IntPtr handle = CreateFile(path, GENERIC_READ|GENERIC_WRITE, FILE_SHARE_READ|FILE_SHARE_WRITE, IntPtr.Zero, 0x3, 0, IntPtr.Zero);

            if ((long)handle == -1)
                throw new IOException("unable to open drive " + drive);

            int dummy = 0;
            int ret = DeviceIoControl(handle, IOCTL_STORAGE_EJECT_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);

            CloseHandle(handle);
        }

        static public void LoadMedia(char drive)
        {
            string path = @"\\.\" + drive + @":";
            IntPtr handle = CreateFile(path, GENERIC_READ|GENERIC_WRITE, FILE_SHARE_READ|FILE_SHARE_WRITE, IntPtr.Zero, 0x3, 0, IntPtr.Zero);

            if ((long)handle == -1)
                throw new IOException("unable to open drive " + drive);

            int dummy = 0;
            int ret = DeviceIoControl(handle, IOCTL_STORAGE_LOAD_MEDIA, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);
            Console.WriteLine("DeviceContorl(); ret{0}", ret);

            CloseHandle(handle);

            // dos not work for SD card
        }

        static public bool IsReadOnly(char drive)
        {
            string path = @"\\.\" + drive + @":";
            IntPtr handle = CreateFile(path, GENERIC_READ|GENERIC_WRITE, FILE_SHARE_READ|FILE_SHARE_WRITE, IntPtr.Zero, 0x3, 0, IntPtr.Zero);

            if ((long)handle == -1)
                throw new IOException("unable to open drive " + drive);

            int dummy = 0;
            int ret = DeviceIoControl(handle, IOCTL_DISK_IS_WRITABLE, IntPtr.Zero, 0, IntPtr.Zero, 0, ref dummy, IntPtr.Zero);

            CloseHandle(handle);

            return ret == 0;
        }

        // watch removal

       

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public delegate void RemovalEvent(object sender, RemovalEventArgs e);

        /// <summary>
        /// 
        /// </summary>
        public static event RemovalEvent OnNotify;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="notify"></param>
        public static void StartWatch(RemovalEvent notify = null)
        {
            if (notify != null)
                OnNotify += notify;

            dummy = new DummyForm();
        }

        static DummyForm dummy = null;


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHChangeNotifyEntry
        {
            public IntPtr pIdl;
            [MarshalAs(UnmanagedType.Bool)]
            public Boolean Recursively;
        }

        [DllImport("shell32.dll", SetLastError = true, EntryPoint = "#2", CharSet = CharSet.Auto)]
        static extern IntPtr SHChangeNotifyRegister(IntPtr hWnd, SHCNF fSources, SHCNE fEvents, uint wMsg, int cEntries, ref SHChangeNotifyEntry pFsne);

        [DllImport("shell32.dll")]
        static extern Boolean SHChangeNotifyDeregister(IntPtr hNotify);

        enum SHCNF
        {
            IDLIST = 0x0000,
            PATHA = 0x0001,
            PRINTERA = 0x0002,
            DWORD = 0x0003,
            PATHW = 0x0005,
            PRINTERW = 0x0006,
            TYPE = 0x00FF,
            FLUSH = 0x1000,
            FLUSHNOWAIT = 0x2000
        }

        enum SHCNE: uint
        {
            MEDIAINSERTED = 0x00000020,
            MEDIAREMOVED = 0x00000040,
            //...
        }

        enum WM
        {
            DEVICECHANGE = 0x0219,
            SHNOTIFY = 0x0401,
        }

        class DummyForm: Form
        {
            public DummyForm()
            {
                var notifyEntry = new SHChangeNotifyEntry() { pIdl = IntPtr.Zero, Recursively = true };
                notifyId = SHChangeNotifyRegister(Handle,
                   SHCNF.TYPE | SHCNF.IDLIST, SHCNE.MEDIAINSERTED | SHCNE.MEDIAREMOVED,
                  (uint)WM.SHNOTIFY, 1, ref notifyEntry);

                drivemask = 0;
                var dd = DriveInfo.GetDrives();
                foreach (var d in dd) {
                    uint mask = 1u << (d.Name[0]-'A');
                    if (d.IsReady)
                        drivemask |= mask;
                }
            }

            protected override void Dispose(bool disposing)
            {
                if (notifyId != IntPtr.Zero)
                    SHChangeNotifyDeregister(notifyId);

                base.Dispose(disposing);
            }

            IntPtr notifyId = IntPtr.Zero;

            //

            enum DBT
            {
                DEVICEARRIVAL = 0x8000,
                DEVICEQUERYREMOVE = 0x8001,
                DEVICEQUERYREMOVEFAILED = 0x8002,
                DEVICEREMOVEPENDING = 0x8003,
                DEVICEREMOVECOMPLETE = 0x8004
            }

            protected override void WndProc(ref Message m)
            {
                //char driveLetter = '\0';
                var e = new RemovalEventArgs();

                switch ((WM)m.Msg) {
                case WM.SHNOTIFY:
                    switch ((SHCNE)m.LParam) {
                    case SHCNE.MEDIAINSERTED:
                        //Console.WriteLine("メディアがセットされた {0}", GetDrive1(m.WParam));
                        e.DriveLetter = GetDrive1(m.WParam);
                        e.Status = RemovalStatus.INSERTED;
                        break;

                    case SHCNE.MEDIAREMOVED:
                        //Console.WriteLine("メディアが取り外された {0}", GetDrive1(m.WParam));
                        e.DriveLetter = GetDrive1(m.WParam);
                        e.Status = RemovalStatus.EJECTED;
                        break;
                    }
                    break;

                case WM.DEVICECHANGE:
                    switch ((DBT)m.WParam) {
                    case DBT.DEVICEARRIVAL:
                        //Console.WriteLine("DEVICEARRIVAL");
                        e.DriveLetter = GetDrive2(m.LParam);
                        e.Status = RemovalStatus.INSERTED;
                        break;

                    case DBT.DEVICEREMOVECOMPLETE:
                        //Console.WriteLine("DEVICEREMOVECOMPLETE");
                        e.DriveLetter = GetDrive2(m.LParam);
                        e.Status = RemovalStatus.EJECTED;
                        break;
                    }
                    break;
                }

                if (e.Status != RemovalStatus.NONE) {
                    var info = new DriveInfo(e.DriveLetter+":");
                    uint mask = 1u << (e.DriveLetter - 'A'), last = drivemask;
                    if (info.IsReady)
                        drivemask |= mask;
                    else
                        drivemask &= ~mask;

                    if ((drivemask & mask) != (last & mask))
                        try {
                            if (OnNotify != null)
                                OnNotify(null, e);
                            Console.WriteLine("OnNotify({0}: {1})", e.DriveLetter, e.Status);
                        } catch (Exception ex) {
                            Console.WriteLine("OnNotify; "+ex.Message);
                        }
                }

                base.WndProc(ref m);
            }

            uint drivemask = 0;

            [DllImport("shell32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SHGetPathFromIDListW(IntPtr pidl, [MarshalAs(UnmanagedType.LPTStr)] StringBuilder pszPath);

            struct SHNOTIFYSTRUCT
            {
                public uint dwItem1;
                public uint dwItem2;
            }

            char GetDrive1(IntPtr WParam)
            {
                var shNotify = (SHNOTIFYSTRUCT)Marshal.PtrToStructure(WParam, typeof(SHNOTIFYSTRUCT));
                var driveRootPathBuffer = new StringBuilder("A:\\");
                SHGetPathFromIDListW((IntPtr)shNotify.dwItem1, driveRootPathBuffer);
                return driveRootPathBuffer.ToString()[0];
            }

            struct DEV_BROADCAST_VOLUME
            {
                public uint dbcv_size;
                public uint dbcv_devicetype;
                public uint dbcv_reserved;
                public uint dbcv_unitmask;
            }

            char GetDrive2(IntPtr LParam)
            {
                var volume = (DEV_BROADCAST_VOLUME)Marshal.PtrToStructure(LParam, typeof(DEV_BROADCAST_VOLUME));
                //Console.WriteLine("{0} {1}", volume.dbcv_devicetype, volume.dbcv_unitmask);
                uint m = 1;
                for (char c = 'A'; c <= 'Z'; c++, m <<= 1) {
                    if ((volume.dbcv_unitmask & m) != 0)
                        return c;
                }
                return '\0';    // not found
            }

        }

    }
}
