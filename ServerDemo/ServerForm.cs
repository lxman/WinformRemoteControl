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
        private readonly WinformRemoteControl.RCServer RcServer = new WinformRemoteControl.RCServer("127.0.0.1", 9000);
        
        public ServerForm()
        {
            InitializeComponent();
            label1.Text = "Nothing yet.";
            comboBox2.DataSource = Cb2Source;
            RcServer.AddControl(button1);
            RcServer.AddControl(textBox1);
            RcServer.AddControl(label1);
            RcServer.AddControl(comboBox1);
            RcServer.AddControl(comboBox2);
            RcServer.AddControl(tabControl1);
            RcServer.AddControl(radioButton1);
            RcServer.AddControl(radioButton2);
            RcServer.AddControl(radioButton3);
            RcServer.AddControl(radioButton4);
            RcServer.AddControl(treeView1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Text = "The button was clicked.";
            RcServer.SendNotification("label1_TextChanged");
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RcServer?.Dispose();
        }
    }
}