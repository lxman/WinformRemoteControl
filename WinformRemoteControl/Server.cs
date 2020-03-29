using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using WatsonTcp;
using WinformRemoteControl.Wrappers;

namespace WinformRemoteControl
{
    public class Server : WatsonTcpServer
    {
        private string IpPort = string.Empty;
        private readonly List<IControlWrapper> Controls = new List<IControlWrapper>();

        public Server(string addr, int port) : base(addr, port)
        {
            Initialize();
        }
        
        public Server(string addr, int port, string pfxCertFile = "", string pfxCertPassword = "") : base(addr, port, pfxCertFile, pfxCertPassword)
        {
            Initialize();
        }

        public void AddControl(Control c)
        {
            List<Control> controls = Controls.Select(ctl => ctl.Control).ToList();
            if (controls.Contains(c)) return;
            switch (c)
            {
                case Button b:
                    Controls.Add(new ButtonWrapper(b));
                    break;
                case Label l:
                    Controls.Add(new LabelWrapper(l));
                    break;
                case TextBox tb:
                    Controls.Add(new TextBoxWrapper(tb));
                    break;
                case ComboBox cb:
                    Controls.Add(new ComboBoxWrapper(cb));
                    break;
                case TabControl tc:
                    Controls.Add(new TabControlWrapper(tc));
                    break;
            }
        }

        public void SendNotification(string notification)
        {
            Send(IpPort, FormNotificationMetadata(notification));
        }

        private void Initialize()
        {
            MaxConnections = 1;
            MessageReceived += OnMessageReceived;
            ClientConnected += OnClientConnected;
            ClientDisconnected += OnClientDisconnected;
            Start();
        }

        private void OnClientDisconnected(object sender, ClientDisconnectedEventArgs e)
        {
            IpPort = string.Empty;
        }

        private void OnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            IpPort = e.IpPort;
        }

        private void OnMessageReceived(object sender, MessageReceivedFromClientEventArgs e)
        {
            MessageType ct = (MessageType)Enum.Parse(typeof(MessageType),
                e.Metadata["MessageType"].ToString());
            switch (ct)
            {
                case MessageType.Server:
                    ServerCommand sc =
                        (ServerCommand) Enum.Parse(typeof(ServerCommand),
                            e.Metadata["ServerCommand"].ToString());
                    ProcessServerCommand(sc);
                    break;
                case MessageType.Control:
                    ControlCommand cc =
                        (ControlCommand) Enum.Parse(typeof(ControlCommand),
                            e.Metadata["ControlCommand"].ToString());
                    ProcessControlCommand(cc, e.Metadata);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void ProcessServerCommand(ServerCommand sc)
        {
            switch (sc)
            {
                case ServerCommand.GetControls:
                    if (!IsClientConnected(IpPort)) return;
                    Dictionary<string, Guid> ctlList = new Dictionary<string, Guid>();
                    Controls.ForEach(c => ctlList.Add(c.Name, c.Identifier));
                    Send(IpPort, FormGetControlsResponseMetadata(), JsonConvert.SerializeObject(ctlList));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ProcessControlCommand(ControlCommand cc, IReadOnlyDictionary<object, object> meta)
        {
            switch (cc)
            {
                case ControlCommand.ButtonClick:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is ButtonWrapper bw)) return;
                    bw.RemoteClick();
                    break;
                case ControlCommand.LabelSetText:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is LabelWrapper lw)) return;
                    lw.SetText(meta["Text"].ToString());
                    break;
                case ControlCommand.TextBoxSetText:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is TextBoxWrapper tbw)) return;
                    tbw.SetText(meta["Text"].ToString());
                    break;
                case ControlCommand.ComboBoxGetElements:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is ComboBoxWrapper cbw)) return;
                    Dictionary<object, object> metadata =
                        FormControlMessageResponseMetadata(Guid.Parse(meta["Guid"].ToString()));
                    List<(int, string)> elements = cbw.GetElements();
                    Send(IpPort, metadata, JsonConvert.SerializeObject(elements));
                    break;
                case ControlCommand.ComboBoxSetIndex:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is ComboBoxWrapper cb)) return;
                    cb.SetSelectedIndex(Convert.ToInt32(meta["Index"]));
                    break;
                case ControlCommand.TabControlSelectTab:
                    if (!(Controls.FirstOrDefault(c => c.Identifier == Guid.Parse(meta["Guid"].ToString())) is
                        TabControlWrapper tc)) return;
                    tc.SelectTab(meta["Text"].ToString());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cc), cc, null);
            }
        }
        
        private static Dictionary<object, object> FormGetControlsResponseMetadata()
        {
            return new Dictionary<object, object>
            {
                ["MessageType"] = MessageType.ServerMessageResponse
            };
        }

        private static Dictionary<object, object> FormNotificationMetadata(string notificationName)
        {
            return new Dictionary<object, object>
            {
                ["MessageType"] = MessageType.Notification,
                ["Name"] = notificationName
            };
        }

        private static Dictionary<object, object> FormControlMessageResponseMetadata(Guid id)
        {
            return new Dictionary<object, object>
            {
                ["MessageType"] = MessageType.ControlMessageResponse,
                ["ControlResponseType"] = ControlCommand.ComboBoxGetElements,
                ["Guid"] = id
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Controls.ForEach(c => c.Dispose());
                Controls.Clear();
            }
            
            base.Dispose(disposing);
        }
    }
}