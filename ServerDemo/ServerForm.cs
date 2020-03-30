using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ServerDemo
{
    public partial class ServerForm : Form
    {
        private readonly List<string> Cb2Source = new List<string>
        {
            "A",
            "B",
            "C",
            "D"
        };
        private readonly WinformRemoteControl.Server RemoteControlServer = new WinformRemoteControl.Server("127.0.0.1", 9000);
        
        public ServerForm()
        {
            InitializeComponent();
            label1.Text = "Nothing yet.";
            comboBox2.DataSource = Cb2Source;
            RemoteControlServer.AddControl(button1);
            RemoteControlServer.AddControl(textBox1);
            RemoteControlServer.AddControl(label1);
            RemoteControlServer.AddControl(comboBox1);
            RemoteControlServer.AddControl(comboBox2);
            RemoteControlServer.AddControl(tabControl1);
            RemoteControlServer.AddControl(radioButton1);
            RemoteControlServer.AddControl(radioButton2);
            RemoteControlServer.AddControl(radioButton3);
            RemoteControlServer.AddControl(radioButton4);
            RemoteControlServer.AddControl(treeView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "The button was clicked.";
            RemoteControlServer.SendNotification("label1_TextChanged");
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoteControlServer?.Dispose();
        }
    }
}