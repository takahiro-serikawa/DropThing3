
// https://www.ipentec.com/document/csharp-shell-namespace-get-icon-from-file-path から抜粋

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Runtime.InteropServices;

namespace DropThing3
{
    static class GetIconAPI
    {
        public static Icon Get(string filename)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            IntPtr hImg = SHGetFileInfo(
              filename, 0, out shinfo, (uint)Marshal.SizeOf(typeof(SHFILEINFO)),
              SHGFI.SHGFI_ICON | SHGFI.SHGFI_LARGEICON);

            if (hImg == IntPtr.Zero || shinfo.hIcon == IntPtr.Zero)
                return null;
            return Icon.FromHandle(shinfo.hIcon);
        }

        [Flags]
        public enum SHGFI
        {
            SHGFI_ICON = 0x000000100,
            SHGFI_DISPLAYNAME = 0x000000200,
            SHGFI_TYPENAME = 0x000000400,
            SHGFI_ATTRIBUTES = 0x000000800,
            SHGFI_ICONLOCATION = 0x000001000,
            SHGFI_EXETYPE = 0x000002000,
            SHGFI_SYSICONINDEX = 0x000004000,
            SHGFI_LINKOVERLAY = 0x000008000,
            SHGFI_SELECTED = 0x000010000,
            SHGFI_ATTR_SPECIFIED = 0x000020000,
            SHGFI_LARGEICON = 0x000000000,
            SHGFI_SMALLICON = 0x000000001,
            SHGFI_OPENICON = 0x000000002,
            SHGFI_SHELLICONSIZE = 0x000000004,
            SHGFI_PIDL = 0x000000008,
            SHGFI_USEFILEATTRIBUTES = 0x000000010,
            SHGFI_ADDOVERLAYS = 0x000000020,
            SHGFI_OVERLAYINDEX = 0x000000040
        }

        [Flags]
        public enum CSIDL: uint
        {
            CSIDL_DESKTOP = 0x0000,
            CSIDL_WINDOWS = 0x0024
        }

        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public int iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("shell32.dll")]
        static extern IntPtr SHGetFileInfo(string pszPath,
           uint dwFileAttribs, out SHFILEINFO psfi, uint cbFileInfo, SHGFI uFlags);




        //

        const int SHGFI_ICON = 0x000000100;
        const int SHIL_EXTRALARGE = 2;
        static readonly Guid IID_IImageList = new Guid("46EB5926-582E-4017-9FDF-E8998DAA0950");

        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr SHGetFileInfo(string pszPath, int dwFileAttributes, ref SHFILEINFO psfi, int cbFileInfo, int uFlags);
        [DllImport("Shell32.dll", PreserveSig = false)]
        static extern void SHGetImageList(int iImageList, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IntPtr ppv);
        [DllImport("Comctl32.dll")]
        static extern IntPtr ImageList_GetIcon(IntPtr himl, int i, int flags);
        [DllImport("Comctl32.dll")]
        static extern bool ImageList_Destroy(IntPtr himl);
        [DllImport("User32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);

        public static Bitmap GetIconImage(string fileName, int shil)
        {
            var fi = new SHFILEINFO();
            var result = SHGetFileInfo(fileName, 0, ref fi, Marshal.SizeOf(typeof(SHFILEINFO)), SHGFI_ICON);
            //Debug.Assert(result != IntPtr.Zero);

            var himl = IntPtr.Zero;
            var hicon = IntPtr.Zero;
            try {
                SHGetImageList(shil/*SHIL_EXTRALARGE*/, IID_IImageList, out himl);
                hicon = ImageList_GetIcon(himl, fi.iIcon, 0);
                //Debug.Assert(hicon != IntPtr.Zero);

                return Icon.FromHandle(hicon).ToBitmap();
            } finally {
                if (hicon != IntPtr.Zero)
                    DestroyIcon(hicon);
                if (himl != IntPtr.Zero)
                    ImageList_Destroy(himl);
            }
        }

    }
}
