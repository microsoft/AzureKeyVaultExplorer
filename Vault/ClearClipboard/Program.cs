// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

namespace ClearClipboard
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows.Forms;

    class Program
    {
        public static string CalculateMd5(string value)
        {
            byte[] buff = Encoding.UTF8.GetBytes(value);
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(buff);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        [STAThread]
        static int Main(string[] args)
        {           
            Regex validMd5 = new Regex("^[0-9a-fA-F]{32}$", RegexOptions.Compiled | RegexOptions.Singleline);
            if (args.Length != 2)
            {
                Console.WriteLine("Clears clipboard after specified interval if current content in the clipboard matches specified MD5");
                Console.WriteLine("Usage: ClearClipboard.exe <interval> <md5>");
                Console.WriteLine("Example: ClearClipboard.exe 00:00:05 54e06ec0f15e3e5df6424bd77626bb49");
                return 0;
            }
            TimeSpan interval;
            if (!TimeSpan.TryParse(args[0], out interval))
            {
                Console.WriteLine("Error: Invalid interval {0}", args[0]);
                return 1;
            }
            if (!validMd5.IsMatch(args[1]))
            {
                Console.WriteLine("Error: Invalid md5 {0}", args[1]);
                return 1;
            }
            string md5 = args[1];

            // Create manual reset event handle in the CurrentUserSession
            bool createdNew;
            using (var ewh = new EventWaitHandle(false, EventResetMode.ManualReset, @"Local\ClearClipboard-{E8301137-C9A9-4666-B1C4-B8428E0A7596}", out createdNew))
            {
                if (!createdNew)
                {
                    Console.WriteLine("Old instance of ClearClipboard.exe is still running in this user session, signaling it to die");
                    ewh.Set();
                }
                ewh.Reset();
                if (ewh.WaitOne(interval))
                {
                    Console.WriteLine("New instance of ClearClipboard.exe just started, let us die");
                    return 0;
                }

                var dataObj = Clipboard.GetDataObject();
                if (!dataObj.GetDataPresent(DataFormats.Text))
                {
                    Console.WriteLine("Clipboard is already empty");
                    return 0;
                }
                if (0 == string.Compare(md5, CalculateMd5(dataObj.GetData(DataFormats.Text).ToString()), true))
                {
                    if (dataObj.GetDataPresent(DataFormats.FileDrop)) // In case clipboard has temp files ("cut" mode) we will delete them
                    {
                        string[] files = (string[])dataObj.GetData(DataFormats.FileDrop);
                        foreach (var file in files)
                        {
                            if (file.StartsWith(Path.GetTempPath().TrimEnd('\\'), StringComparison.CurrentCultureIgnoreCase))
                            {
                                File.Delete(file);
                            }
                        }
                    }
                    Clipboard.Clear();
                    Console.WriteLine("Clipboard cleared");
                }
                else
                {
                    Console.WriteLine("Clipboard left intact");
                }
                return 0;
            }
        }
    }
}
