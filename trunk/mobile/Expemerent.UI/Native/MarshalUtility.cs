using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using Expemerent.UI.Native;

namespace Expemerent.UI.Native
{
    internal static class MarshalUtility
    {
        /// <summary>
        /// Marshalls pointer to the wipe callback
        /// </summary>
        public static WipeCallback GetWipeDelegate(IntPtr wipe)
        {
            throw new NotImplementedException("Wipe not supported on mobile platform");
        }

        /// <summary>
        /// Marshal unmanaged buffer to managed array
        /// </summary>
        internal static byte[] MarshalData(IntPtr rawData, int dataSize)
        {
            #region Arguments checking
            if (rawData == IntPtr.Zero || dataSize == 0)
                return null;
            #endregion

            var data = new byte[dataSize];
            Marshal.Copy(rawData, data, 0, dataSize);

            return data;
        }

        /// <summary>
        /// Ansi string platform specific marshaller
        /// </summary>
        public static string PtrToStringAnsi(IntPtr source)
        {
            var result = new byte[GetUtf8StrLength(source)];
            Marshal.Copy(source, result, 0, result.Length);
            return Encoding.Default.GetString(result, 0, result.Length);
        }

        /// <summary>
        /// Ansi string platform specific marshaller
        /// </summary>
        public static string PtrToStringUtf8(IntPtr source)
        {
            var result = new byte[GetUtf8StrLength(source)];
            Marshal.Copy(source, result, 0, result.Length);
            return Encoding.UTF8.GetString(result, 0, result.Length);
        }

        /// <summary>
        /// Converts string to ansi byte buffer
        /// </summary>
        public static byte[] StringToByteUtf8(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// Converts string to ansi byte buffer
        /// </summary>
        public static byte[] StringToByteUtf8(string str, bool includePreamble)
        {
            if (includePreamble)
            {
                var enc = Encoding.UTF8;
                var preamble = enc.GetPreamble();
                var size = enc.GetByteCount(str);
                var bytes = new byte[preamble.Length + size];

                Buffer.BlockCopy(preamble, 0, bytes, 0, preamble.Length);
                enc.GetBytes(str, 0, str.Length, bytes, preamble.Length);
                return bytes;
            }
            
            return Encoding.UTF8.GetBytes(str);
        }

        /// Converts string to ansi byte buffer
        /// </summary>
        internal static byte[] StringToAnsi(string str)
        {
            var enc = Encoding.Default;
            var bytes = new byte[enc.GetByteCount(str) + 1];
            enc.GetBytes(str, 0, str.Length, bytes, 0);
            return bytes;
        }
        
        /// <summary>
        /// Implements interlocked exchange operation
        /// </summary>
        [DllImport("coredll.dll")]
        public static extern IntPtr InterlockedExchange(ref IntPtr Target, IntPtr value); 


        /// <summary>
        /// Returns length of the utf8 string
        /// </summary>
        private static unsafe int GetUtf8StrLength(IntPtr source)
        {
            var size = 0;
            var bytes = (byte*)source;
            while (bytes[size] != 0) ++size;
            return size;
        }
    }
}
