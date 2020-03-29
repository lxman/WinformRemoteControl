using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    internal class TextBoxWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }
        
        public Control Control { get; private set; }
        
        public TextBox TextBox { get; private set; }

        public TextBoxWrapper(TextBox tb)
        {
            Name = tb.Name;
            Control = tb;
            TextBox = tb;
        }

        public void SetText(string text)
        {
            if (TextBox.InvokeRequired) TextBox.Invoke(new Action(() => TextBox.Text = text));
            else TextBox.Text = text;
        }

        public void Dispose()
        {
            Control = null;
            TextBox = null;
        }
    }
}