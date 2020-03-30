using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ClientDemo
{
    public partial class ClientForm : Form
    {
        private readonly WinformRemoteControl.RCClient RcClient;
        private bool IsComboSetup;
        
        public ClientForm()
        {
            InitializeComponent();
            RcClient = new WinformRemoteControl.RCClient("127.0.0.1", 9000);
            RcClient.Notification += RcClientNotification;
        }

        private void RcClientNotification(object sender, string e)
        {
            TsLblNotifications.Text = e;
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RcClient.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RcClient.Start();
            EnableControls();
        }

        private void EnableControls()
        {
            button1.Enabled = RcClient.Connected;
            button3.Enabled = RcClient.Connected;
            button4.Enabled = RcClient.Connected;
            button5.Enabled = RcClient.Connected;
            button6.Enabled = RcClient.Connected;
            button7.Enabled = RcClient.Connected;
            textBox1.Enabled = RcClient.Connected;
            textBox2.Enabled = RcClient.Connected;
            textBox3.Enabled = RcClient.Connected;
            textBox4.Enabled = RcClient.Connected;
            comboBox1.Enabled = RcClient.Connected;
            comboBox2.Enabled = RcClient.Connected;
            comboBox3.Enabled = RcClient.Connected;
            comboBox3.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RcClient.SendButtonClick("button1");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RcClient.SendTextBoxText("textBox1", textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RcClient.SendLabelSetText("label1", textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RcClient.ComboBoxRequestElements("comboBox1");
            WaitAndStock(comboBox1, "comboBox1");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RcClient.ComboBoxRequestElements("comboBox2");
            WaitAndStock(comboBox2, "comboBox2");
        }

        private void WaitAndStock(ComboBox cb, string name)
        {
            while (RcClient.GetItemsForCombo(name).Count == 0)
            {
                Thread.Sleep(100);
            }

            List<(int, string)> sorted = RcClient.GetItemsForCombo(name).OrderBy(l => l.Item1)
                .ToList();
            cb.Items.Clear();
            sorted.ForEach(s => cb.Items.Add(s.Item2));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RcClient.ComboBoxSetIndex("comboBox1", comboBox1.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RcClient.ComboBoxSetIndex("comboBox2", comboBox2.SelectedIndex);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text)) RcClient.TabControlSelectTab("tabControl1", textBox3.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsComboSetup) RcClient.RadioButtonCheck($"radioButton{comboBox3.SelectedIndex + 1}");
            else IsComboSetup = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text)) return;
            RcClient.TreeViewSelectNodeByText("treeView1", textBox4.Text);
        }
    }
}