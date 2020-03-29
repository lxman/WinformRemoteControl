using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public class RadioButtonWrapper : IControlWrapper
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }
        
        public Control Control { get; private set; }
        
        public RadioButton RadioButton { get; private set; }

        public RadioButtonWrapper(RadioButton rb)
        {
            Name = rb.Name;
            Control = rb;
            RadioButton = rb;
        }

        public void RadioButtonCheck()
        {
            if (RadioButton.InvokeRequired)
            {
                RadioButton.Invoke(new Action(() => RadioButton.Checked = true));
            }
            else RadioButton.Checked = true;
        }
        
        public void Dispose()
        {
            Control = null;
            RadioButton = null;
        }
    }
}