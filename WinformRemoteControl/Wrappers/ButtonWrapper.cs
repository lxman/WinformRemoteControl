using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public class ButtonWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }

        public Control Control { get; private set; }
        
        public Button Button { get; private set; }

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