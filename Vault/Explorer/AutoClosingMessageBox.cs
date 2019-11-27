// Copyright (c) Microsoft Corporation. All rights reserved. 
// Licensed under the MIT License. See License.txt in the project root for license information. 

using System;
using System.Windows.Forms;

namespace Microsoft.Vault.Explorer
{
    /// <summary>
    /// Implementation for closing the Message Box if no input from the user
    /// </summary>
    public sealed class AutoClosingMessageBox : IDisposable
    {
        private readonly System.Threading.Timer timeoutTimer;
        private const uint MB_TOPMOST = 0x40000;
        public AutoClosingMessageBox(int timeout)
        {
            if (timeout <= 0)
            {
                throw new ArgumentException(nameof(timeout));
            }

            this.timeoutTimer = new System.Threading.Timer(
                OnTimerElapsed,
                null,
                timeout,
                System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Displays the message box as the topmost element on the window
        /// </summary>
        /// <param name="buttons">Type of Message Box Buttons. In this case YesNo </param>
        /// <param name="icon">Icon of Message Box. In this case question</param>
        /// <param name="button">Type of button-Button1</param>
        /// <returns></returns>
        public DialogResult Show(
            String text,
            String caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton button)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentException(nameof(text));
            }
            if (string.IsNullOrWhiteSpace(caption))
            {
                throw new ArgumentException(nameof(caption));
            }

            // Display MessageBox in MB_TOPMOST mode so that it is on top of
            // other windows even when minimized.
            return MessageBox.Show(text, caption, buttons, icon, button, (MessageBoxOptions)MB_TOPMOST);
        }

        public void Dispose()
        {
            timeoutTimer.Dispose();
        }

        private void OnTimerElapsed(object state)
        {
            Application.Exit();
        }
    }
}
