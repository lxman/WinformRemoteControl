using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;
using WatsonTcp;

namespace WinformRemoteControl
{
    public class Client : WatsonTcpClient
    {
        public event EventHandler<string> Notification;
        public event EventHandler<List<(int, string)>> ComboBoxElementListReceived;
        
        private Dictionary<string, Guid> ServerControls;
        private readonly Dictionary<Guid, List<(int, string)>> ComboBoxElementLists = new Dictionary<Guid, List<(int, string)>>();
        
        public Client(string addr, int port) : base(addr, port)
        {
            Initialize();
        }
        
        public Client(string addr, int port, string pfxCertFile, string pfxCertPassword) : base(addr, port, pfxCertFile, pfxCertPassword)
        {
            Initialize();
        }

        public void SendButtonClick(string name)
        {
            Send(FormButtonClickHeader(GetGuidFromName(name)));
        }

        public void SendTextBoxText(string name, string text)
        {
            Send(FormTextBoxSetTextHeader(GetGuidFromName(name), text));
        }

        public void SendLabelSetText(string name, string text)
        {
            Send(FormLabelSetTextHeader(GetGuidFromName(name), text));
        }

        public void ComboBoxRequestElements(string name)
        {
            Send(FormComboBoxRequestElementsHeader(GetGuidFromName(name)));
        }

        public void ComboBoxSetIndex(string name, int index)
        {
            Send(FormComboBoxSetIndexHeader(GetGuidFromName(name), index));
        }

        public List<(int, string)> GetItemsForCombo(string name)
        {
            return ComboBoxElementLists.ContainsKey(GetGuidFromName(name)) ? ComboBoxElementLists[GetGuidFromName(name)] : new List<(int, string)>();
        }

        public void TabControlSelectTab(string name, string text)
        {
            Send(FormTabControlSelectTabHeader(GetGuidFromName(name), text));
        }

        private Guid GetGuidFromName(string name)
        {
            return ServerControls.FirstOrDefault(sc => sc.Key == name).Value;
        }

        private void Initialize()
        {
            ServerConnected += OnServerConnected;
            ServerDisconnected += OnServerDisconnected;
            AuthenticationFailure += OnAuthenticationFailure;
            MessageReceived += OnMessageReceived;
        }

        private void OnMessageReceived(object sender, MessageReceivedFromServerEventArgs e)
        {
            MessageType ct = (MessageType)Enum.Parse(typeof(MessageType), e.Metadata["MessageType"].ToString());
            switch (ct)
            {
                case MessageType.ServerMessageResponse:
                    ServerControls =
                        JsonConvert.DeserializeObject<Dictionary<string, Guid>>(Encoding.UTF8.GetString(e.Data));
                    break;
                case MessageType.Notification:
                    Notification?.Invoke(this, e.Metadata["Name"].ToString());
                    break;
                case MessageType.ControlMessageResponse:
                    if ((ControlCommand) Enum.Parse(typeof(ControlCommand),
                        e.Metadata["ControlResponseType"].ToString()) == ControlCommand.ComboBoxGetElements)
                    {
                        Guid id = Guid.Parse(e.Metadata["Guid"].ToString());
                        if (ComboBoxElementLists.ContainsKey(id)) ComboBoxElementLists.Remove(id);
                        ComboBoxElementLists.Add(id, JsonConvert.DeserializeObject<List<(int, string)>>(Encoding.UTF8.GetString(e.Data)));
                        ComboBoxElementListReceived?.Invoke(this, JsonConvert.DeserializeObject<List<(int, string)>>(Encoding.UTF8.GetString(e.Data)));
                    }
                    break;
            }
        }

        private void OnAuthenticationFailure(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnServerDisconnected(object sender, EventArgs e)
        {
            MessageBox.Show("Server disconnected.");
        }

        private void OnServerConnected(object sender, EventArgs e)
        {
            Send(FormControlListRequestHeader());
        }

        private static Dictionary<object, object> FormControlListRequestHeader()
        {
            return new Dictionary<object, object>
            {
                ["MessageType"] = MessageType.Server,
                ["ServerCommand"] = ServerCommand.GetControls
            };
        }

        private static Dictionary<object, object> FormButtonClickHeader(Guid id)
        {
            return FormBaseControlCommandHeader(ControlCommand.ButtonClick, id);
        }

        private static Dictionary<object, object> FormTextBoxSetTextHeader(Guid id, string text)
        {
            Dictionary<object, object> header = FormBaseControlCommandHeader(ControlCommand.TextBoxSetText, id);
            header.Add("Text", text);
            return header;
        }

        private static Dictionary<object, object> FormLabelSetTextHeader(Guid id, string text)
        {
            Dictionary<object, object> header = FormBaseControlCommandHeader(ControlCommand.LabelSetText, id);
            header.Add("Text", text);
            return header;
        }

        private static Dictionary<object, object> FormTabControlSelectTabHeader(Guid id, string text)
        {
            Dictionary<object, object> header = FormBaseControlCommandHeader(ControlCommand.TabControlSelectTab, id);
            header.Add("Text", text);
            return header;
        }

        private static Dictionary<object, object> FormComboBoxRequestElementsHeader(Guid id)
        {
            return FormBaseControlCommandHeader(ControlCommand.ComboBoxGetElements, id);
        }

        private static Dictionary<object, object> FormComboBoxSetIndexHeader(Guid id, int index)
        {
            Dictionary<object, object> header = FormBaseControlCommandHeader(ControlCommand.ComboBoxSetIndex, id);
            header.Add("Index", index);
            return header;
        }

        private static Dictionary<object, object> FormBaseControlCommandHeader(ControlCommand cmd, Guid id)
        {
            return new Dictionary<object, object>
            {
                ["MessageType"] = MessageType.Control,
                ["ControlCommand"] = cmd,
                ["Guid"] = id
            };
        }
    }
}