using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Runtime.InteropServices;

namespace ParaParaView
{
    class Ejector
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
    }
}
