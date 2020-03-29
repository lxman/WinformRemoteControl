﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public class ComboBoxWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }
        
        public Control Control { get; private set; }
        
        public ComboBox ComboBox { get; private set; }

        public ComboBoxWrapper(ComboBox cb)
        {
            Name = cb.Name;
            Control = cb;
            ComboBox = cb;
        }

        public List<(int, string)> GetElements()
        {
            List<(int, string)> elements = new List<(int, string)>();
            if (ComboBox.InvokeRequired)
                ComboBox.Invoke(new Action(() =>
                    elements = ComboBox.Items.Cast<object>().Select((t, x) => (x, t.ToString())).ToList()));
            else elements = ComboBox.Items.Cast<object>().Select((t, x) => (x, t.ToString())).ToList();
            return elements;
        }

        public void SetSelectedIndex(int index)
        {
            if (ComboBox.InvokeRequired) ComboBox.Invoke(new Action(() => ComboBox.SelectedIndex = index));
            else ComboBox.SelectedIndex = index;
        }

        public void Dispose()
        {
            Control = null;
            ComboBox = null;
        }
    }
}