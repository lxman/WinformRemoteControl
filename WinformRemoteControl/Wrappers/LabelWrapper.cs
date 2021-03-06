﻿using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    internal class LabelWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }
        
        public Control Control { get; private set; }

        private Label Label { get; set; }

        public LabelWrapper(Label l)
        {
            Name = l.Name;
            Control = l;
            Label = l;
        }

        public void SetText(string text)
        {
            if (Label.InvokeRequired) Label.Invoke(new Action(() => Label.Text = text));
            else Label.Text = text;
        }

        public void Dispose()
        {
            Control = null;
            Label = null;
        }
    }
}