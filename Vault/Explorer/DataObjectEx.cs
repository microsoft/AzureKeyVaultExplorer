using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;

namespace Microsoft.PS.Common.Vault.Explorer
{
    /// <summary>
    /// Transferring Virtual Files to Windows Explorer in C#
    /// http://www.codeproject.com/Articles/23139/Transferring-Virtual-Files-to-Windows-Explorer-in
    /// </summary>
    public class DataObjectEx : DataObject, System.Runtime.InteropServices.ComTypes.IDataObject
    {
        private static readonly TYMED[] ALLOWED_TYMEDS =
            new TYMED[] { 
                TYMED.TYMED_HGLOBAL,
                TYMED.TYMED_ISTREAM, 
                TYMED.TYMED_ENHMF,
                TYMED.TYMED_MFPICT,
                TYMED.TYMED_GDI};

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct FILEDESCRIPTOR
        {
            public UInt32 dwFlags;
            public Guid clsid;
            public System.Drawing.Size sizel;
            public System.Drawing.Point pointl;
            public UInt32 dwFileAttributes;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
            public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
            public UInt32 nFileSizeHigh;
            public UInt32 nFileSizeLow;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public String cFileName;
        }

        public struct SelectedItem
        {
            public String FileName;
            public DateTime WriteTime;
            public Int64 FileSize;
            public byte[] FileContents;
        }

        private SelectedItem[] m_SelectedItems;
        private Int32 m_lindex;

        public DataObjectEx(SelectedItem[] selectedItems)
        {
            m_SelectedItems = selectedItems;

            SetData(NativeMethods.CFSTR_FILEDESCRIPTORW, null);
            SetData(NativeMethods.CFSTR_FILECONTENTS, null);
            SetData(NativeMethods.CFSTR_PERFORMEDDROPEFFECT, null);
        }

        public override object GetData(string format, bool autoConvert)
        {
            if (String.Compare(format, NativeMethods.CFSTR_FILEDESCRIPTORW, StringComparison.OrdinalIgnoreCase) == 0 && m_SelectedItems != null)
            {
                base.SetData(NativeMethods.CFSTR_FILEDESCRIPTORW, GetFileDescriptor(m_SelectedItems));
            }
            else if (String.Compare(format, NativeMethods.CFSTR_FILECONTENTS, StringComparison.OrdinalIgnoreCase) == 0)
            {
                base.SetData(NativeMethods.CFSTR_FILECONTENTS, GetFileContents(m_SelectedItems, m_lindex));
            }
            else if (String.Compare(format, NativeMethods.CFSTR_PERFORMEDDROPEFFECT, StringComparison.OrdinalIgnoreCase) == 0)
            {
                //TODO: Cleanup routines after paste has been performed
            }
            return base.GetData(format, autoConvert);
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void System.Runtime.InteropServices.ComTypes.IDataObject.GetData(ref System.Runtime.InteropServices.ComTypes.FORMATETC formatetc, out System.Runtime.InteropServices.ComTypes.STGMEDIUM medium)
        {
            if (formatetc.cfFormat == (Int16)DataFormats.GetFormat(NativeMethods.CFSTR_FILECONTENTS).Id)
                m_lindex = formatetc.lindex;

            medium = new System.Runtime.InteropServices.ComTypes.STGMEDIUM();
            if (GetTymedUseable(formatetc.tymed))
            {
                if ((formatetc.tymed & TYMED.TYMED_HGLOBAL) != TYMED.TYMED_NULL)
                {
                    medium.tymed = TYMED.TYMED_HGLOBAL;
                    medium.unionmember = NativeMethods.GlobalAlloc(NativeMethods.GHND | NativeMethods.GMEM_DDESHARE, 1);
                    if (medium.unionmember == IntPtr.Zero)
                    {
                        throw new OutOfMemoryException();
                    }
                    try
                    {
                        ((System.Runtime.InteropServices.ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
                        return;
                    }
                    catch
                    {
                        NativeMethods.GlobalFree(new HandleRef((STGMEDIUM)medium, medium.unionmember));
                        medium.unionmember = IntPtr.Zero;
                        throw;
                    }
                }
                medium.tymed = formatetc.tymed;
                ((System.Runtime.InteropServices.ComTypes.IDataObject)this).GetDataHere(ref formatetc, ref medium);
            }
            else
            {
                Marshal.ThrowExceptionForHR(NativeMethods.DV_E_TYMED);
            }
        }

        private static Boolean GetTymedUseable(TYMED tymed)
        {
            for (Int32 i = 0; i < ALLOWED_TYMEDS.Length; i++)
            {
                if ((tymed & ALLOWED_TYMEDS[i]) != TYMED.TYMED_NULL)
                {
                    return true;
                }
            }
            return false;
        }

        private MemoryStream GetFileDescriptor(SelectedItem[] SelectedItems)
        {
            MemoryStream FileDescriptorMemoryStream = new MemoryStream();
            // Write out the FILEGROUPDESCRIPTOR.cItems value
            FileDescriptorMemoryStream.Write(BitConverter.GetBytes(SelectedItems.Length), 0, sizeof(UInt32));

            FILEDESCRIPTOR FileDescriptor = new FILEDESCRIPTOR();
            foreach (SelectedItem si in SelectedItems)
            {
                FileDescriptor.cFileName = si.FileName;
                Int64 FileWriteTimeUtc = si.WriteTime.ToFileTimeUtc();
                FileDescriptor.ftLastWriteTime.dwHighDateTime = (Int32)(FileWriteTimeUtc >> 32);
                FileDescriptor.ftLastWriteTime.dwLowDateTime = (Int32)(FileWriteTimeUtc & 0xFFFFFFFF);
                FileDescriptor.nFileSizeHigh = (UInt32)(si.FileSize >> 32);
                FileDescriptor.nFileSizeLow = (UInt32)(si.FileSize & 0xFFFFFFFF);
                FileDescriptor.dwFlags = NativeMethods.FD_WRITESTIME | NativeMethods.FD_FILESIZE | NativeMethods.FD_PROGRESSUI;

                // Marshal the FileDescriptor structure into a byte array and write it to the MemoryStream.
                Int32 FileDescriptorSize = Marshal.SizeOf(FileDescriptor);
                IntPtr FileDescriptorPointer = Marshal.AllocHGlobal(FileDescriptorSize);
                Marshal.StructureToPtr(FileDescriptor, FileDescriptorPointer, true);
                Byte[] FileDescriptorByteArray = new Byte[FileDescriptorSize];
                Marshal.Copy(FileDescriptorPointer, FileDescriptorByteArray, 0, FileDescriptorSize);
                Marshal.FreeHGlobal(FileDescriptorPointer);
                FileDescriptorMemoryStream.Write(FileDescriptorByteArray, 0, FileDescriptorByteArray.Length);
            }
            return FileDescriptorMemoryStream;
        }

        private MemoryStream GetFileContents(SelectedItem[] SelectedItems, Int32 FileNumber)
        {
            MemoryStream FileContentMemoryStream = null;
            if (SelectedItems != null && FileNumber < SelectedItems.Length)
            {
                SelectedItem si = SelectedItems[FileNumber];
                FileContentMemoryStream = new MemoryStream(si.FileContents);
            }
            return FileContentMemoryStream;
        }

    }

    public class NativeMethods
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GlobalAlloc(int uFlags, int dwBytes);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GlobalFree(HandleRef handle);

        // Clipboard formats used for cut/copy/drag operations
        public const string CFSTR_PREFERREDDROPEFFECT = "Preferred DropEffect";
        public const string CFSTR_PERFORMEDDROPEFFECT = "Performed DropEffect";
        public const string CFSTR_FILEDESCRIPTORW = "FileGroupDescriptorW";
        public const string CFSTR_FILECONTENTS = "FileContents";

        // File Descriptor Flags
        public const Int32 FD_CLSID = 0x00000001;
        public const Int32 FD_SIZEPOINT = 0x00000002;
        public const Int32 FD_ATTRIBUTES = 0x00000004;
        public const Int32 FD_CREATETIME = 0x00000008;
        public const Int32 FD_ACCESSTIME = 0x00000010;
        public const Int32 FD_WRITESTIME = 0x00000020;
        public const Int32 FD_FILESIZE = 0x00000040;
        public const Int32 FD_PROGRESSUI = 0x00004000;
        public const Int32 FD_LINKUI = 0x00008000;

        // Global Memory Flags
        public const Int32 GMEM_MOVEABLE = 0x0002;
        public const Int32 GMEM_ZEROINIT = 0x0040;
        public const Int32 GHND = (GMEM_MOVEABLE | GMEM_ZEROINIT);
        public const Int32 GMEM_DDESHARE = 0x2000;

        // IDataObject constants
        public const Int32 DV_E_TYMED = unchecked((Int32)0x80040069);
    }
}
