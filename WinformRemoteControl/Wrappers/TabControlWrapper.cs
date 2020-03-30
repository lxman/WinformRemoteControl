using System;
using System.Linq;
using System.Windows.Forms;

namespace WinformRemoteControl.Wrappers
{
    internal class TabControlWrapper : IControlWrapper, IDisposable
    {
        public Guid Identifier { get; } = Guid.NewGuid();
        
        public string Name { get; }
        
        public Control Control { get; private set; }

        private TabControl TabControl { get; set; }

        public TabControlWrapper(TabControl tc)
        {
            Name = tc.Name;
            Control = tc;
            TabControl = tc;
        }

        public void SelectTabByText(string name)
        {
            if (TabControl.InvokeRequired)
            {
                TabControl.Invoke(new Action(() =>
                {
                    TabControl.SelectedTab = TabControl.TabPages.OfType<TabPage>().FirstOrDefault(tp => tp.Text == name);
                }));
            }
            else TabControl.SelectedTab = TabControl.TabPages.OfType<TabPage>().FirstOrDefault(tp => tp.Text == name);
        }

        public void Dispose()
        {
            Control = null;
            TabControl = null;
        }
    }
}