﻿using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    internal class ButtonWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }

        public Control Control { get; private set; }

        private Button Button { get; set; }

        public ButtonWrapper(Button b)
        {
            Name = b.Name;
            Control = b;
            Button = b;
        }

        public void RemoteClick()
        {
            if (Button.InvokeRequired) Button.Invoke(new Action(() => Button.PerformClick()));
            else Button.PerformClick();
        }

        public void Dispose()
        {
            Control = null;
            Button = null;
        }
    }
}