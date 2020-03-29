using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    public interface IControlWrapper
    {
        Guid Identifier { get; }
        
        string Name { get; }
        
        Control Control { get; }

        void Dispose();
    }
}