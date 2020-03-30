﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace ClientDemo
{
    public partial class ClientForm : Form
    {
        private readonly WinformRemoteControl.Client RemoteControlClient;
        private bool IsComboSetup;
        
        public ClientForm()
        {
            InitializeComponent();
            RemoteControlClient = new WinformRemoteControl.Client("127.0.0.1", 9000);
            RemoteControlClient.Notification += RemoteControlClientNotification;
        }

        private void RemoteControlClientNotification(object sender, string e)
        {
            TsLblNotifications.Text = e;
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoteControlClient.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            RemoteControlClient.Start();
            EnableControls();
        }

        private void EnableControls()
        {
            button1.Enabled = RemoteControlClient.Connected;
            button3.Enabled = RemoteControlClient.Connected;
            button4.Enabled = RemoteControlClient.Connected;
            button5.Enabled = RemoteControlClient.Connected;
            button6.Enabled = RemoteControlClient.Connected;
            button7.Enabled = RemoteControlClient.Connected;
            textBox1.Enabled = RemoteControlClient.Connected;
            textBox2.Enabled = RemoteControlClient.Connected;
            textBox3.Enabled = RemoteControlClient.Connected;
            textBox4.Enabled = RemoteControlClient.Connected;
            comboBox1.Enabled = RemoteControlClient.Connected;
            comboBox2.Enabled = RemoteControlClient.Connected;
            comboBox3.Enabled = RemoteControlClient.Connected;
            comboBox3.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RemoteControlClient.SendButtonClick("button1");
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            RemoteControlClient.SendTextBoxText("textBox1", textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            RemoteControlClient.SendLabelSetText("label1", textBox2.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            RemoteControlClient.ComboBoxRequestElements("comboBox1");
            WaitAndStock(comboBox1, "comboBox1");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            RemoteControlClient.ComboBoxRequestElements("comboBox2");
            WaitAndStock(comboBox2, "comboBox2");
        }

        private void WaitAndStock(ComboBox cb, string name)
        {
            while (RemoteControlClient.GetItemsForCombo(name).Count == 0)
            {
                Thread.Sleep(100);
            }

            List<(int, string)> sorted = RemoteControlClient.GetItemsForCombo(name).OrderBy(l => l.Item1)
                .ToList();
            cb.Items.Clear();
            sorted.ForEach(s => cb.Items.Add(s.Item2));
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoteControlClient.ComboBoxSetIndex("comboBox1", comboBox1.SelectedIndex);
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            RemoteControlClient.ComboBoxSetIndex("comboBox2", comboBox2.SelectedIndex);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox3.Text)) RemoteControlClient.TabControlSelectTab("tabControl1", textBox3.Text);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (IsComboSetup) RemoteControlClient.RadioButtonCheck($"radioButton{comboBox3.SelectedIndex + 1}");
            else IsComboSetup = true;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox4.Text)) return;
            RemoteControlClient.TreeViewSelectNodeByText("treeView1", textBox4.Text);
        }
    }
}