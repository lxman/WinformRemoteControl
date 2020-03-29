using System;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    internal interface IControlWrapper
    {
        Guid Identifier { get; }
        
        string Name { get; }
        
        Control Control { get; }

        void Dispose();
    }
}