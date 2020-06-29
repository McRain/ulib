using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    public static class ChatMessageType
    {
        /// <summary>
        /// 
        /// </summary>
        public const string Message = "Message";
        /// <summary>
        /// 
        /// </summary>
        public const string Service = "Service";
    }

    /// <summary>
    /// 
    /// </summary>
    public class ChatManager
    {
        private static ChatManager _instance;

        /// <summary>
        /// 
        /// </summary>
        public static GatewayType chatGateway = GatewayType.Www;

        /// <summary>
        /// 
        /// </summary>
        public static string targetClassName = "ChatManager";
        
        /// <summary>
        /// 
        /// </summary>
        public static string entryMethodName = "Entry";
        
        /// <summary>
        /// 
        /// </summary>
        public static string chatMessageName = "Say";

        /// <summary>
        /// Name of method for change room
        /// </summary>
        public static string changeRoomName = "Room";

        /// <summary>
        /// 
        /// </summary>
        public static string quitMessageName = "Quit";

        /// <summary>
        /// 
        /// </summary>
        public static string userListKey = "usrs";
        
        /// <summary>
        /// 
        /// </summary>
        public static string messagesListKey = "msgs";
        
        /// <summary>
        /// 
        /// </summary>
        public static string lastMsgId = "last";

        private static bool _sendedMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="evt"></param>
        public delegate void OnChat(UChatEvent evt);

        /// <summary>
        /// 
        /// </summary>
        public static event OnChat ChatEvent;

        /// <summary>
        /// 
        /// </summary>
        public static bool useTags;

        public static ChatRoom room;

        private static int _queryPeriod = 5;
        private static int _maxMessage = 25;
        private static float _sendCount;
        private static bool _chatEntries;
        private static bool _querySended;
        private static int _lastMessageId;

        private static IRemoteObject _www;
        
        private static ChatMessage _lastMessage = new ChatMessage();


        /// <summary>
        /// 
        /// </summary>
        public static readonly Dictionary<string, Color> Colors = new Dictionary<string, Color>
                                                                 {
                                                                     {ChatMessageType.Message,Color.white},
                                                                     {ChatMessageType.Service,Color.yellow}
                                                                 };
        /// <summary>
        /// 
        /// </summary>
        public static readonly Dictionary<string, string> Styles = new Dictionary<string, string>
                                                                  {
                                                                      {ChatMessageType.Message,"Label"},
                                                                      {ChatMessageType.Service,"Label"}
                                                                  };
        /*private static List<Color> _userColorAllow = new List<Color>
                                                {
            Color.white,Color.green,Color.gray,Color.yellow
        };*/


        private static readonly List<ChatMessage> _messages = new List<ChatMessage>();
        private static readonly Dictionary<int, ChatUser> _users = new Dictionary<int, ChatUser>();

        //public static readonly Dictionary<string,UCommand> Tags = new Dictionary<string, UCommand>(); 

        private static readonly Dictionary<string, ChatMessage> ServiceMessages = new Dictionary<string, ChatMessage>();

        //public AudioSource moderSound;



        //static bool _chatSoundEnable;


        //private static bool _querySended;

        //public delegate void ChatEvent(ChatEventType type);
        //public static event ChatEvent OnRecivedMessages;


        /*private static string _inputText = "";
        private static string _emptyString = "";*/
        //static string lastMessage = "";

        //static bool _chatEnabled;
        //static bool _userRegister;
        //static ChatStatus stat = ChatStatus.ONLINE;
        //static char serviceChar = '#';
        //private static int moderatorLvl = 3;
        //static int adminLvl = 6;
        //static int spamCount = 0;
        //static GameMessage currentMessage = new GameMessage();

        /*private GUISkin _skin;
        private static ChatMessage _lastMessage = new ChatMessage();

        private static Rect _chatWinRect = new Rect(Screen.width - 512, 32, 512, 256);
        private float _chatHeigt = 256;
        private int _chatWin = 1;

        private static Texture2D _winDownIcon;
        private static Texture2D _winUpIcon;*/

        /*void Awake()
        {
            //********************
            if (ServiceMessages.Count == 0)
            {
                ServiceMessages.Add("#REG#", LanguageManager.GetLabel("#REG#"));
                ServiceMessages.Add("#OUT#", LanguageManager.GetLabel("#OUT#"));
                ServiceMessages.Add("#BAN#", LanguageManager.GetLabel("#BAN#"));
                ServiceMessages.Add("#WRN#", LanguageManager.GetLabel("#WRN#"));
                ServiceMessages.Add("#NEW#", LanguageManager.GetLabel("#NEW#"));
                ServiceMessages.Add("#END#", LanguageManager.GetLabel("#END#"));
                ServiceMessages.Add("#MSG#", LanguageManager.GetLabel("#MSG#"));
                ServiceMessages.Add("#LVL#", LanguageManager.GetLabel("#LVL#"));
            }
            //*******************
        }*/

        /// <summary>
        /// 
        /// </summary>
        public static ChatManager Instance
        {
            get { return _instance ?? (_instance = new ChatManager()); }
        }

        /// <summary>
        /// 
        /// </summary>
        public static int RefreshTime
        {
            get { return _queryPeriod; }
            set { _queryPeriod = value; }

        }

        /// <summary>
        /// 
        /// </summary>
        public static bool SendedMessage
        {
            get { return _sendedMessage; }
        }

        #region Lists messages,users and service messages

        /// <summary>
        /// 
        /// </summary>
        public static List<ChatMessage> Messages
        {
            get { return _messages; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void AddMessage(ChatMessage msg)
        {
            _messages.Add(msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageIndex"></param>
        public static void RemoveMessage(int messageIndex)
        {
            _messages.RemoveAt(messageIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void RemoveMessage(ChatMessage message)
        {
            _messages.Remove(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        public static void RemoveMessages(int from, int to)
        {
            _messages.RemoveRange(from, to);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearMessages()
        {
            _messages.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="msg"></param>
        public static void AddServiceMessage(string key, ChatMessage msg)
        {
            msg.color = Colors[msg.messageType];
            if (ServiceMessages.ContainsKey(key))
                ServiceMessages[key] = msg;
            else
                ServiceMessages.Add(key, msg);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceMsgs"></param>
        public static void AddServiceMessages(Dictionary<string, ChatMessage> serviceMsgs)
        {
            foreach (var serviceMsg in serviceMsgs)
                AddServiceMessage(serviceMsg.Key, serviceMsg.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveServiceMessage(string key)
        {
            if (ServiceMessages.ContainsKey(key))
                lock (ServiceMessages)
                {
                    ServiceMessages.Remove(key);
                }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearServiceMessages()
        {
            ServiceMessages.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        public static Dictionary<int, ChatUser> Users
        {
            get { return _users; }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearUsers()
        {
            _users.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgType"></param>
        /// <param name="styleName"></param>
        public static void AddStyle(string msgType, string styleName)
        {
            if (Styles.ContainsKey(msgType))
                Styles[msgType] = styleName;
            else
                Styles.Add(msgType, styleName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgType"></param>
        public static void RemoveStyle(string msgType)
        {
            if (Styles.ContainsKey(msgType))
                Styles.Remove(msgType);

        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearStyle()
        {
            Styles.Clear();
        }



        #endregion

        #region public chat methods

        /// <summary>
        /// 
        /// </summary>
        public static void Entry()
        {
            SendEntry(new Hashtable());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        public static void Entry(Hashtable info)
        {
            SendEntry(info);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="refreshPeriod"></param>
        /// <param name="maxMessage"></param>
        public static void Entry(Hashtable info, int refreshPeriod, int maxMessage)
        {
            _maxMessage = maxMessage;
            _queryPeriod = refreshPeriod;
            SendEntry(info);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Request()
        {
            if (!_querySended)
            {
                //if (_chatEntries)
                //{
                if (_sendCount <= 0)
                {
                    _sendCount = _queryPeriod;
                    var cm = new ChatMessage();
                    /*if(_lastMessage.msg!=string.Empty)
                    {
                        cm.msg = _lastMessage.msg;
                        _lastMessage.msg = string.Empty;
                    }*/
                    /*if (_lastMessage.msg != string.Empty)
                    {
                        cm.msg = _lastMessage.msg;
                        _lastMessage.msg = string.Empty;
                    }*/
                    //Gateway.GetSender().Call(_instance, "Say", cm, OnRecivedChat);
                    SendChat(cm);
                }
                //}
                _sendCount--;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saymsg"></param>
        public static void Say(string saymsg)
        {
            Say(new ChatMessage { id = 0, messageType = ChatMessageType.Message, msg = saymsg, userId = 0 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="messageType"></param>
        public static void Say(string message, string messageType)
        {
            Say(new ChatMessage { id = 0, messageType = messageType, msg = message, userId = 0 });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void Say(ChatMessage msg)
        {
            if (_chatEntries)
            {
                if (!_lastMessage.msg.Equals(msg.msg) && !string.IsNullOrEmpty(msg.msg))
                {
                    _sendCount = _queryPeriod;
                    _lastMessage = msg;
                    SendChat(_lastMessage);
                }
                else
                {
                    if (string.IsNullOrEmpty(msg.msg))
                    {
                        if (ChatEvent != null)
                            ChatEvent(new UChatEvent { owner = Instance, sender = Instance, eventType = UChatEventType.EMPTY, value = msg.msg });
                        /*if( ServiceMessages.ContainsKey("#EMT#"))
                             _messages.Add(ServiceMessages["#EMT#"]);*/
                    }
                    else /*if(ServiceMessages.ContainsKey("#FLD#"))*/
                        if (ChatEvent != null)
                            ChatEvent(new UChatEvent { owner = Instance, sender = Instance, eventType = UChatEventType.FLOOD, value = msg.msg });
                    // _messages.Add(ServiceMessages["#FLD#"]);
                }

            }
            else /*if (ServiceMessages.ContainsKey("#OUT#"))
            _messages.Add(ServiceMessages["#OUT#"]);*/
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { owner = Instance, sender = Instance, eventType = UChatEventType.OUT, value = msg });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public static void ServiceMessage(string text)
        {
            _messages.Add(new ChatMessage { color = Colors.ContainsKey(ChatMessageType.Service) ? Colors[ChatMessageType.Service] : Color.yellow, messageType = ChatMessageType.Service, msg = text, senderName = "Chat", style = Styles.ContainsKey(ChatMessageType.Service) ? Styles[ChatMessageType.Service] : "Label" });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="msgType"></param>
        /// <param name="msgColor"></param>
        /// <param name="sender"></param>
        public static void ServiceMessage(string text, string msgType, Color msgColor, string sender)
        {
            _messages.Add(new ChatMessage { color = msgColor, messageType = msgType, msg = text, senderName = sender, style = Styles.ContainsKey(msgType) ? Styles[msgType] : "Label" });
        }

        /// <summary>
        /// 
        /// </summary>
        public static void Quit()
        {
            _chatEntries = false;
            _querySended = true;
            SendQuit();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string GetUserName(int userId)
        {
            return _users.ContainsKey(userId) ? _users[userId].name : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roomId"></param>
        public static void ChangeRoom(int roomId)
        {
            if (_chatEntries)
            {
                SendChangeRoom(roomId);
            }
        }

        #endregion

        #region GatewayMethods

        private static void SendEntry(Hashtable info)
        {
            _querySended = true;
            if (_www == null)
                _www = Gateway.GetSender(chatGateway);
            _www.Call(targetClassName, entryMethodName, info, RecivedEntry);
        }

        private static void RecivedEntry(object inData)
        {
            _querySended = false;
            if (inData is Hashtable)
            {
                _sendCount = _queryPeriod;
                _chatEntries = true;

                var hash = (Hashtable)inData;
                if (hash.ContainsKey(userListKey))
                    ResetUsers( (Dictionary<int, ChatUser>)hash[userListKey]);
                    
                
                if (hash.ContainsKey(lastMsgId))
                    _lastMessageId = (int)hash[lastMsgId];
                _lastMessage = new ChatMessage();
                /*if (hash.ContainsKey(messagesListKey))
                {
                    var msgs = (List<ChatMessage>)hash[messagesListKey];
                    foreach (var msg in msgs)
                    {
                        msg.senderName = GetUserName(msg.userId);
                        msg.color = Colors[msg.messageType];
                        msg.style = Styles[msg.messageType];
                        _lastMessageId = msg.id;
                        _messages.Add(msg);
                        
                    }
                    var msgsCount = _messages.Count - _maxMessage;
                    for (var i = 0; i < msgsCount; i++)
                        _messages.RemoveAt(0);
                }*/
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.ENTRY, owner = Instance, sender = Instance, value = hash });
                   
            }
            else
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.ERROR, owner = Instance, sender = Instance, value = inData });
        }

        private static void ResetUsers(Dictionary<int, ChatUser> dict )
        {
            _users.Clear();
            foreach (var chatUser in dict)
                _users.Add(chatUser.Key, chatUser.Value);
        }

        private static void SendChat(ChatMessage msg)
        {
            if (_chatEntries)
            {
                _querySended = true;
                msg.id = _lastMessageId;
                if (_www == null)
                    _www = Gateway.GetSender(chatGateway);
                _www.Call(targetClassName, chatMessageName, msg, RecivedChat);
            }
            
        }

        private static void RecivedChat(object inData)
        {
            _querySended = false;
            if (inData is Hashtable)
            {
                var hash = (Hashtable)inData;
                if (hash.ContainsKey(userListKey))
                    if (hash.ContainsKey(userListKey))
                        ResetUsers((Dictionary<int, ChatUser>)hash[userListKey]);
                if (hash.ContainsKey(messagesListKey))
                {
                    var msgs = (List<ChatMessage>)hash[messagesListKey];
                    foreach (var msg in msgs)
                    {
                        if (msg.id > _lastMessageId)
                        {
                            msg.senderName = GetUserName(msg.userId);
                            msg.color = Colors[msg.messageType];
                            msg.style = Styles[msg.messageType];
                            /*if (useTags)
                            {
                                foreach (var uCommand in Tags)
                                    if (msg.msg.Contains(uCommand.Key))
                                    {
                                        uCommand.Value.parameters = new object[] { uCommand.Key, msg };
                                        CommandManager.Execute(uCommand.Value);
                                        /*msg.msg = msg.msg.Replace(uCommand.Key,
                                                                  CommandManager.Execute(uCommand.Value).ToString());*/
                            /*}
                    }*/

                            _lastMessageId = msg.id;
                            _messages.Add(msg);
                        }
                    }
                    var msgsCount = _messages.Count - _maxMessage;
                    for (var i = 0; i < msgsCount; i++)
                        _messages.RemoveAt(0);
                }
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.RECIVED, owner = Instance, sender = Instance, value = hash });
            }
            else
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.ERROR, owner = Instance, sender = Instance, value = inData });
        }

        private static void SendChangeRoom(int newRoomId)
        {
            _querySended = true;
            if (_www == null)
                _www = Gateway.GetSender(chatGateway);
            _www.Call(targetClassName, changeRoomName, newRoomId, RecivedRoom);
        }

        private static void RecivedRoom(object inData)
        {
            _querySended = false;
            if (inData is ChatRoom)
            {
                room = (ChatRoom) inData;
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.ROOM, owner = Instance, sender = Instance, value = room });
            }
            else
                if (ChatEvent != null)
                    ChatEvent(new UChatEvent { eventType = UChatEventType.ERROR, owner = Instance, sender = Instance, value = inData });
        }

        private static void SendQuit()
        {
            if (_www == null)
                _www = Gateway.GetSender(chatGateway);
            _www.Call(targetClassName, quitMessageName, null);
            _querySended = false;
        }

        #endregion


        /*void Start()
        {
            GateParameters.InitGateway(false);

            /*if (Application.isEditor && !GameManager.started)
            {
                return;
            }*/
        /*_instance = this;

            /*foreach (KeyValuePair<string, string> servicePare in LanguageManager.strings)
            {

            }*/
        /*for (var i = 0; i < 256; i++)
            {
                _emptyString += " ";
            }

            /*if (GameManager.Player.gmlvl >= (int)GameManager.Parameters["chatMinLvl"])
            {
                //_queryPeriod = (int)gm.GetParam("chatQueryPeriod");
                //adminLvl = gm.parameters.chatAdminLvl;
                moderatorLvl = (int)GameManager.Parameters["chatModerLvl"];
                maxLine = (int)GameManager.Parameters["chatMaxLine"];
            }*/
        //_skin = ResourceManager.Skin;
        //ResizeChatRect();
        /*Open(new Hashtable { { "uid", 2 } });
        }*/

        /*void ResizeChatRect()
        {
            chatWinRect = new Rect(Screen.width * 0.5f - 112, 32, Screen.width * 0.5f + 112, Screen.width * 0.5f+50);
        }*/

        /*static Vector2 _chatScrollPos;
        static Vector2 _usersScrollPos;

        void OnGUI()
        {

            if (_chatEnabled)
            {
                //GUI.depth = 999;
                GUI.skin = _skin;
                _chatWinRect.height = _chatHeigt;
                _chatWinRect = GUI.Window(98, _chatWinRect, ShowChatWindow,"");
                //GUITools.ShowWindowToolTip(GUI.tooltip);
            }
        }*/

        /*void ShowChatWindow(int winOD)
        {
           // GUITools.ShowWindow(_chatWinRect, LanguageManager.GetLabel("chatGlobalTitle"), Close, _chatWin);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label(LanguageManager.GetLabel("generalChatLabel"));

            _chatScrollPos = GUILayout.BeginScrollView(_chatScrollPos, false, true, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            foreach (var cms in _messages)
                DrawChatLine(cms);
            GUILayout.EndScrollView();
            if (Event.current.type == EventType.keyDown && Event.current.character.ToString() == "\n")
            {
                Say(_inputText);
            }
            GUI.SetNextControlName("Chat input field");
            _inputText = GUILayout.TextArea(_inputText, 249, GUILayout.ExpandWidth(true), GUILayout.Height(35));

            /*GUILayout.BeginHorizontal();
            if (Event.current.type == EventType.keyDown && Event.current.character.ToString() == "\n" && inputText.Length > 0)
            {
                Say(inputText);
            }
            GUI.SetNextControlName("Chat input field");
            inputText = GUILayout.TextArea(inputText, 249, GUILayout.Height(35));
            //GUI.FocusControl("Chat input field");
            if (GUILayout.Button(LanguageManager.GetLabel("sayChat"), GUILayout.ExpandWidth(false)))
            {
                Say(inputText);
            }
            GUILayout.EndHorizontal();*/
        /*GUILayout.BeginHorizontal();
            for (var i = 0; i < UserColorAllow.Count; i++)
            {
                GUI.color = UserColorAllow[i];
                if (GUILayout.Button(LanguageManager.GetLabel("color" + i)))
                {
                    Say(_inputText, i);
                }
                GUI.color = Color.white;

            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.BeginVertical(GUILayout.Width(160));
            GUILayout.Label(LanguageManager.GetLabel("userChatList"));
            _usersScrollPos = GUILayout.BeginScrollView(_usersScrollPos, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            foreach (var us in _users)
            {
                GUILayout.BeginHorizontal();
                /*if (GameManager.Player.gmlvl >= moderatorLvl)
                {
                    if (GUILayout.Button("!", GUILayout.Width(15)))
                    {
                        SendAlert(us.Key);
                    }
                    if (GUILayout.Button("x", GUILayout.Width(15)))
                    {
                        SendBan(us.Key, 60);
                    }
                    /*if (GUILayout.Button("p", GUILayout.Width(15)))
                    {
                        inputText += "#PM#" + us.Value.name + "#";
                    }*/
        //}
        /*GUILayout.Label(new GUIContent("(" + us.Value.lvl + ") " + us.Value.name), "listText");
                /*if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    inputText += us.Value.name;
                }*/
        /*GUILayout.EndHorizontal();
                /*if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    inputText += "#PM#" + us.Value.name + "#";
                }*/

        /*}
            GUILayout.EndScrollView();
            GUILayout.FlexibleSpace();
            _chatSoundEnable = GUILayout.Toggle(_chatSoundEnable, LanguageManager.GetLabel("chatSound"));
            //pmSoundEnable = GUILayout.Toggle(chatSoundEnable, LanguageManager.GetLabel("chatPMSound"));
            //_queryPeriod = GUITools.NumStep(_queryPeriod,LanguageManager.GetLabel("chatPeriod"), 1, 10,12,false);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(LanguageManager.GetLabel("exit"), GUILayout.ExpandWidth(true)))
                Close();
            if (GUILayout.Button(_winUpIcon))
            {
                _chatHeigt = 256;
                _chatWin = 1;
            }
            if (GUILayout.Button(_winDownIcon))
            {
                _chatHeigt = 512;
                _chatWin = 2;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
            /*if (GUI.GetNameOfFocusedControl() != "Chat input field")
                GUI.FocusControl("Chat input field");*/
        //}



        /*public static void DrawChatLine(ChatMessage cm)
        {
            var chatStyle = "chatMessage";
            chatStyle = "label";
            var msg = cm.msg;
            //GUILayout.BeginHorizontal();
            if (msg.Contains("#")) //Service
            {
                chatStyle += "SERVICE";
                //GUILayout.Label("CHAT", "chatSenderName");
                GUILayout.Label(msg, chatStyle);

            }
            else //
            {
                //GUILayout.Label(cm.senderName.Replace("*", GetNameFromList(cm.senderID)) + " : ","chatSenderName", GUILayout.ExpandWidth(true));
                GUILayout.BeginHorizontal(cm.senderName.Replace("*", GetNameFromList(cm.userId)), "label"
                    /*, "chatSenderName"*/
        //);

        //GUI.color = userColorAllow[cm.msgtype];
        /*GUILayout.Label(msg, chatStyle);
        /*if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            inputText += msg.TrimStart();
        }*/
        /*GUI.color = Color.white;
        GUILayout.EndHorizontal();
        //Rect rct = GUILayoutUtility.GetLastRect();
        //rct.x -= cm.senderName.Length;

    }
    //GUILayout.EndHorizontal();
    /*if (gm.player.gmlvl >= moderatorLvl)
    {
        if (Event.current.type == EventType.MouseUp && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            inputText += "#DEL#" + cm.id.ToString();
        }
    }*/
        //}


        /*public static void Open(Hashtable info)
            {
                _chatWinRect = new Rect(Screen.width - 512, 32, 512, 256);
                //Debug.Log("ChatManager:Open");
                _chatEnabled = true;
                _userRegister = false;
                SendReg(info);
            }

            public static void Open(Hashtable info,float refreshPeriod,int maxMessage)
            {
                _queryPeriod = refreshPeriod;
                _maxMessage = maxMessage;
                _chatWinRect = new Rect(Screen.width - 512, 32, 512, 256);
                //Debug.Log("ChatManager:Open");
                _chatEnabled = true;
                _userRegister = false;
                SendReg(info);
            }*/

        /*public static void Close()
        {
            //_instance.CancelInvoke("GetLastMessage");
            _chatEnabled = /*userRegister =*/
        /*false;
_messages = new List<ChatMessage>();
_users = new Dictionary<int, ChatUser>();
Gateway.GetSender().Call(_instance, "UnRegChat", null);
}

static void SendReg(Hashtable info)
{
Gateway.GetSender().Call(targetClassName, entryMethodName, info, OnReg);
}

static void OnReg(object inData)
{
if (inData is Hashtable)
{
//_winDownIcon = ResourceManager.Instance.winIcons[0];
//_winUpIcon = ResourceManager.Instance.winIcons[1];

var hash = (Hashtable)inData;
if (hash["addusers"] != null)
{
_users = (Dictionary<int, ChatUser>)hash["addusers"];
}
_userRegister = true;
_messages = new List<ChatMessage>();
_sendCount = _queryPeriod;
//Instance.InvokeRepeating("GetLastMessage", 1, 1);
_messages.Add(new ChatMessage(){id = 0,msg = "You entry to chat",senderName = "CHAT",userId = 0});
if(ChatEvent!=null)
ChatEvent(new UChatEvent() { eventType = UChatEventType.ENTRY, owner = Instance, sender = Instance, value = null });
}
}

static void SendBan(int uid, int minutesTime)
{
var hd = new Hashtable
{
{"bid",uid},
{"min",minutesTime}
};
Gateway.GetSender().Call(_instance, "BanChat", hd);
}

static void SendAlert(int uuid)
{
var hd = new Hashtable
{
{"bid",uuid}
};
Gateway.GetSender().Call(_instance, "WarnChat", hd);
}

static void Say(string msg)
{
if (msg.Trim().Length > 0)
{
var cm = new ChatMessage { msg = msg.Replace("\n", "").Trim() };
SendMsg(cm);
}
}

static void Say(string msg, int inType)
{
if (msg.Length > 0)
{
var cm = new ChatMessage { msg = msg.Replace("\n", "").Trim() };
//cm.msgtype = inType;
SendMsg(cm);
}
}

static void SendCommand(ChatMessage cmc)
{

}

static int FindUserId(string userFio)
{
var uid = 0;
foreach (var chatUser in _users)
{
if (chatUser.Value.name.Equals(userFio))
{
uid = chatUser.Key;
break;
}
                
}
return uid;
//return (from usr in _users where usr.Value.name.Equals(userFio) select usr.Key).FirstOrDefault();
}

static void SendMsg(ChatMessage cm)
{
if (_querySended)
{
_lastMessage = cm;
}
else
{
_querySended = true;
//Debug.Log("SEND CHAT " + cm.msg);
if (_lastMessage.msg != "")
{
cm.msg = _lastMessage.msg;
_lastMessage.msg = "";
}
Gateway.GetSender().Call(_instance, "SayChat", cm, OnRecivedChat);
//chatScrollPos.y = _messages.Count * 70;
_inputText = ""; //Clear line
_sendCount = _queryPeriod;
//GUI.FocusControl("Chat input field");
}
}

static void OnRecivedChat(object inData)
{
//blockedQuery = true;
if (!(inData is Hashtable)) return;
var hash = (Hashtable)inData;
if (hash["addusers"] != null)
{
_users = (Dictionary<int, ChatUser>)hash["addusers"];
/*foreach (KeyValuePair<int, ChatUser> usr in newUsers)
{
if (!_users.ContainsKey(usr.Key))
_users.Add(usr.Key, usr.Value);
}*/
        //}
        //Debug.Log("OnRecivedChat2");
        /*if (hash["removeusers"] != null)
            {
                Dictionary<int, ChatUser> removedUsers = (Dictionary<int, ChatUser>)hash["removeusers"];
                foreach (KeyValuePair<int, ChatUser> usr in removedUsers)
                {
                    if (_users.ContainsKey(usr.Key))
                        _users.Remove(usr.Key);
                }
            }*/
        //Debug.Log("OnRecivedChat3");

        /*var inMsgs = (List<ChatMessage>)hash["newmsgs"];
        /*if (inMsgs.Count > 0)
        {
            if (_chatSoundEnable)
                _instance.moderSound.Play();
        }*/
        /*foreach (var inmsg in inMsgs)
        {
            inmsg.senderName = GetNameFromList(inmsg.userId);
            if (inmsg.msg.Contains("#"))
            {
                foreach (var serviceMsg in _serviceMessages)
                {
                    if (inmsg.msg.Contains(serviceMsg.Key))
                    {
                        var msgs = inmsg.msg.Split(new[] { '#' });
                        inmsg.msg = " # " + serviceMsg.Value.Replace("%tag", msgs[2]);

                    }
                    /*inmsg.msg = inmsg.msg.Replace("#REG", " # " + LanguageManager.GetReplacedLabel("enterChat", inmsg.senderName));
                        inmsg.msg = inmsg.msg.Replace("#UNREG", " # " + LanguageManager.GetReplacedLabel("exitChat", inmsg.senderName));
                        inmsg.msg = inmsg.msg.Replace("#BAN", " # " + LanguageManager.GetReplacedLabel("banChat", inmsg.senderName));
                        if (inmsg.msg.Contains("#START"))
                        {
						
                        } else if (inmsg.msg.Contains("#END"))
                        {
                            string[] msgs = inmsg.msg.Split(new char[] { '#' });
                            if (msgs.Length > 2)
                            {
                                inmsg.msg = " # " + LanguageManager.GetReplacedLabel("endRallyChat", msgs[2]);
                            }
                        }*/
        /*}
    }
    else
        inmsg.msg = _emptyString.Substring(0, inmsg.senderName.Length * 2 + 5) + " : " + inmsg.msg;
    _messages.Add(inmsg);
    while (_messages.Count > _maxMessage)
        _messages.RemoveAt(0);
    //_chatScrollPos.y = 10000;
}
//if(GUI.GetNameOfFocusedControl()!="ChatInputField")
_querySended = false;
/*if (OnRecivedMessages != null)
        OnRecivedMessages(ChatEventType.UPDATE);*/
        /*}

        static string GetNameFromList(int uid)
        {
            return _users.ContainsKey(uid) ? _users[uid].name : "*";
        }

        public static bool IsEnabled
        {
            get
            {
                return _chatEnabled;
            }
        }

        void GetLastMessage()
        {
            if (!_querySended)
            {
                if (_chatEnabled && _userRegister)
                {
                    if (_sendCount <= 0)
                    {
                        _sendCount = _queryPeriod;
                        var cm = new ChatMessage();
                        if (_lastMessage.msg != "")
                        {
                            cm.msg = _lastMessage.msg;
                            _lastMessage.msg = "";
                        }
                        Gateway.GetSender().Call(_instance, "Say", cm, OnRecivedChat);
                    }
                }
                _sendCount--;
            }
        }*/
    }
}
