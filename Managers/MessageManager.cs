using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public enum UMessageType
	{
		/// <summary>
		/// 
		/// </summary>
		Message = 0,
		/// <summary>
		/// 
		/// </summary>
		Alert = 1,
		/// <summary>
		/// 
		/// </summary>
		Warning = 2,
		/// <summary>
		/// 
		/// </summary>
		Query = 3,
		/// <summary>
		/// 
		/// </summary>
		Error = 4,
        /// <summary>
        /// 
        /// </summary>
        Dialog = 5,
        /// <summary>
        /// 
        /// </summary>
        Splash = 6
	}

	/// <summary>
	/// Controller for ULIB message system.
	/// Represents a tools for show message.
	/// </summary>
	public sealed class MessageManager
	{
		private static MessageManager _instance;

	    /// <summary>
	    /// Delegate for button click 
	    /// </summary>
	    /// <param name="data"></param>
	    public delegate void OnButton(object data);

        /*
	    /// <summary>
	    /// Delegate for message realise
	    /// </summary>
	    /// <param name="data"></param>
	    /// <param name="sender"></param>
	    public delegate void OnMessage(object data, UMessage sender);*/

	    /// <summary>
	    /// 
	    /// </summary>
	    public delegate void MessagesEvent(UMessage message);

        /// <summary>
        /// 
        /// </summary>
	    public static event MessagesEvent OnMessageStart;

        /// <summary>
        /// 
        /// </summary>
	    public static event MessagesEvent OnMessageEnd;

	    /// <summary>
	    /// 
	    /// </summary>
	    public static bool useTranslated;

        /*
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="sender"></param>
	    /// <param name="value"></param>
        public delegate void ButtonEvent(object value, UMessageButton sender);
        
		/// <summary>
		/// 
		/// </summary>
		/// <param name="msg"></param>
		public delegate void MessageEvent(UMessage msg);
        
		/// <summary>
		/// Called after message showed
		/// </summary>
		public static event MessageEvent MessageShow;

		/// <summary>
		/// Called after any button from message is pressed
		/// </summary>
		public static event MessageEvent MessageRealise;


	    public static event MessageEvent MessageShowed;

	    public static event MessageEvent MessageHide;

      
		/// <summary>
		/// Default message show timer.
		/// </summary>
		public static float showTime = 3.0f;*/
        /*
		/// <summary>
		/// All messages.
		/// </summary>
		public static readonly List<UMessage> Messages = new List<UMessage>();*/

        private static readonly Dictionary<int, UMessage> Messages = new Dictionary<int, UMessage>();

        /*
        #region Default Value

	    /// <summary>
	    /// 
	    /// </summary>
	    public static string ButtonLabel = "OK";
        /// <summary>
        /// 
        /// </summary>
        public static Texture2D ButtonIcon = new Texture2D(1,1);
        /// <summary>
        /// 
        /// </summary>
        public static List<UCommand> ButtonCommands = new List<UCommand>();
        
        public static readonly Dictionary<UMessageType,Texture2D> messageIcons = new Dictionary<UMessageType, Texture2D>();

        #endregion
        */  

        #region Instance

        /// <summary>
		/// Return instance of CommandManager
		/// </summary>
		public static MessageManager Instance
		{
			get { return _instance ?? (_instance = new MessageManager()); }
		}

		/// <summary>
		/// Return instance of CommandManager
		/// </summary>
		public static MessageManager GetInstance()
		{
			return _instance ?? (_instance = new MessageManager());
		}

		#endregion
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Remove(UMessage message)
        {
            if(Messages.Contains(message))
                lock (Messages)
                {
                    Messages.Remove(message);
                }
        }*/

        /// <summary>
        /// Store new message to message list. You can show this message by Show(messageNumber).
        /// Replace if message list contains some messageNumber
        /// </summary>
        /// <param name="messageKey"></param>
        /// <param name="msg"></param>
        public static void AddMessage(int messageKey, UMessage msg)
        {
            if (Messages.ContainsKey(messageKey))
                Messages[messageKey] = msg;
            else
                Messages.Add(messageKey, msg);
        }

        /// <summary>
        /// Store new messages to message list.
        /// Replace if message list contains some message by key.
        /// </summary>
        /// <param name="messages"></param>
        public static void AddMessages(Dictionary<int, UMessage> messages)
        {
            foreach (var uMessage in messages)
                AddMessage(uMessage.Key, uMessage.Value);
        }

        /// <summary>
        /// Parse string array to message dictionary.
        /// Format:
        /// for (string[],0,1,2,3,4,5,6,'|')
        /// 0|AlertText1|2|ButtonLabel1|CommandTarget1|CommandMember1|CommandValue1|ButtonLabel2|CommandTarget2|CommandMember2|CommandValue2|
        /// </summary>
        /// <param name="strings"></param>
        /// <param name="messageNumber"></param>
        /// <param name="messageText"></param>
        /// <param name="buttonsCount"></param>
        /// <param name="buttonLabel"></param>
        /// <param name="commandTarget"></param>
        /// <param name="commandMember"></param>
        /// <param name="commandValue"></param>
        /// <param name="splitter"></param>
        /// <returns></returns>
        public static Dictionary<int,UMessage> ParseFromStrings(string[] strings, int messageNumber,int messageText,int buttonsCount,int buttonLabel,int commandTarget,int commandMember,int commandValue,char splitter)
        {
            var result = new Dictionary<int, UMessage>();
            foreach (var s in strings)
            {
                var strMessage = s.Split(splitter);
                var msg = new UMessage() {text = strMessage[messageText]};
                
                var btnCount = System.Int32.Parse(strMessage[buttonsCount]);
                var btns = new UMessageButton[btnCount];
                for (var i = 0; i < btnCount; i++)
                {
                    var strValue = strMessage[commandValue + (i*4)];
                    var button = new UMessageButton
                                     {
                                         label = strMessage[buttonLabel+(i*4)],
                                         command =
                                             new UCommand()
                                                 {
                                                     target = strMessage[commandTarget + (i * 4)],
                                                     member = strMessage[commandMember + (i * 4)],
                                                     parameters = string.IsNullOrEmpty(strValue)? null:new object[]{strValue}
                                                 }
                                     };

                    btns[i] = button;
                }
                msg.Buttons = btns;
                result.Add(System.Int32.Parse(strMessage[messageNumber]), msg);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<int, UMessage> GetMessages()
        {
            return Messages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageKey"></param>
        public static void RemoveMessage(int messageKey)
        {
            if (Messages.ContainsKey(messageKey))
                lock (Messages)
                {
                    Messages.Remove(messageKey);
                }
                
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public static void RemoveMessage(UMessage msg)
        {
            if (Messages.ContainsValue(msg))
                lock (Messages)
                {
                    var ind = -1;
                    foreach (var uMessage in Messages)
                        if (uMessage.Value == msg)
                            ind = uMessage.Key;
                    if(ind>=0)
                        Messages.Remove(ind);
                }
                
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ClearMessages()
        {
            Messages.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void Show(UMessage message)
        {
            //Messages.Add(message);
            if (OnMessageStart != null)
                OnMessageStart(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageKey"></param>
        public static void Show(int messageKey)
        {
            if (Messages.ContainsKey(messageKey))
                Show(Messages[messageKey]);
            else
                ULog.Log("MessageManager: Messages not contains key "+messageKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageText"></param>
        public static void Show(string messageText)
        {
            Show(messageText,new UMessageButton[0],UMessageType.Alert);
            //Show(new UMessage(messageText, buttons));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="button"></param>
        public static void Show(string messageText, UMessageButton button)
        {
            Show(new UMessage {text = messageText,Buttons = new[] { button }});
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageText"></param>
        /// <param name="buttons"></param>
        public static void Show(string messageText, UMessageButton[] buttons)
        {
            Show(new UMessage {text = messageText,Buttons = buttons});
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="messageText"></param>
	    /// <param name="buttons"></param>
	    /// <param name="messageType"></param>
	    public static void Show(string messageText, UMessageButton[] buttons,UMessageType messageType)
        {
            Show(new UMessage {text = messageText,Buttons = buttons, messageType = messageType });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void HideEvent(UMessage message)
        {
            if (OnMessageEnd != null)
                OnMessageEnd(message);
        }



        /*
        /// <summary>
        /// Create and show message with Alert type .
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        public static void Show(string text, UMessageButton[] buttons)
        {
            var msg = new UMessage(text, buttons);
            if (MessageShow != null)
                MessageShow(msg);
        }

        /// <summary>
        /// Create and show message.
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="buttons"></param>
        /// <param name="tp"></param>
        public static void Show(string text, UMessageButton[] buttons, UMessageType tp)
        {
            var msg = new UMessage(text, buttons) { messageType = tp };
            if (MessageShow != null)
                MessageShow(msg);
        }

	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="messageData"></param>
	    /// <param name="messageTitle"></param>
	    /// <param name="buttons"></param>
	    /// <param name="typeOfMessage"></param>
	    /// <param name="messageIcon"></param>
	    /// <param name="messageStyle"></param>
	    public static void Show(UMessageData[] messageData, string messageTitle, IEnumerable<UMessageButton> buttons, UMessageType typeOfMessage,Texture2D messageIcon,string messageStyle)
        {
            var msg = new UMessage(messageData, messageTitle, buttons, typeOfMessage, messageIcon, messageStyle);
            if (MessageShow != null)
                MessageShow(msg);
        }
        */
        /*
	    /// <summary>
	    /// 
	    /// </summary>
	    public static GUISkin skin;
	    private static bool _guiEnable;
	    /// <summary>
	    /// 
	    /// </summary>
	    public static bool GuiEnable
	    {
	        get { return _guiEnable; }
            set
            {
                _guiEnable = value;
                if(_guiEnable)
                {
                    var gui = GameObject.Find("UlibGuiMessenger");
                    if(gui==null)
                    {
                        gui = new GameObject("UlibGuiMessenger");
                       // MessageShow += gui.AddComponent<MessageGui>().OnMessage;
                    }
                }else
                {
                    var gui = GameObject.Find("UlibGuiMessenger");
                    if (gui != null)
                        Object.Destroy(gui);
                }
            }
	    }*/

        /*internal static void Realise(UMessageButton btn)
        {
            foreach (var uMessage in Messages)
                foreach (var button in uMessage.Buttons)
                    if (button == btn)
                        if (MessageRealise != null)
                            MessageRealise(uMessage);
        }*/

		/*internal static void RealiseMessage(UMessage msg)
		{
			if (MessageRealise != null)
				MessageRealise(msg);
		}*/

		//private static readonly Dictionary<UMessageType,Texture2D> Icons = new Dictionary<UMessageType, Texture2D>(); 

		/*private Dictionary<DialogMessageType, Color> _logTypeColor = new Dictionary<DialogMessageType, Color>()
		{
			{DialogMessageType.MESSAGE,Color.white},
			{DialogMessageType.QUERY,Color.blue},
			{DialogMessageType.WARNING,Color.yellow},
			{DialogMessageType.ERROR,Color.red},
		};*/

		/*public static void Gui()
		{
			var acount = Alerts.Count;
			for (int i = 0; i < acount; i++)
			{
				
			}
		}*/

		/*/// <summary>
		/// 
		/// </summary>
		public static void AddAlert(string title, string msgText, List<UMessageEvent> events)
		{
			var msg = new UMessage(msgText, title, new List<UMessageEvent>(events)) { messageType = DialogMessageType.Alert };
			if (Icons.ContainsKey(msg.messageType))
				msg.icon = Icons[msg.messageType];
			Alerts.Add(msg);
		}*/
		
		/*/// <summary>
		/// 
		/// </summary>
		/// <param name="title"></param>
		/// <param name="msgText"></param>
		/// <param name="events"></param>
		public static int AddMessage(string title, string msgText, List<UMessageEvent> events)
		{
			var msg = new UMessage(msgText, title, new List<UMessageEvent>(events)) { messageType = DialogMessageType.Message };
			if (Icons.ContainsKey(msg.messageType))
				msg.icon = Icons[msg.messageType];
			Messages.Add(msg);
			return Messages.IndexOf(msg);
		}*/

        /*
		/// <summary>
		/// Return index of new message. Added new message to Messages and send to event MessageShow;
		/// </summary>
		/// <param name="messageTitle"></param>
		/// <param name="messageText"></param>
		/// <param name="buttonLabel"></param>
		/// <param name="messageIcon"></param>
		[AccessAllow(true)]
		public static void ShowAlerts(string messageTitle, string messageText,string buttonLabel, Texture2D messageIcon)
		{
			/*var msg = new UMessage(messageText, messageTitle, new List<UMessageButton>
			                                                  	{
			                                                  		new UMessageButton()
			                                                  	})
			          	{
			          		messageType = UMessageType.Alert
			          	};
			if (messageIcon != null)
				msg.icon = messageIcon;*/
			//msg.showTime = showTime;
			/*if (Icons.ContainsKey(msg.messageType))
				msg.icon = Icons[msg.messageType];*/
			//Messages.Add(msg);
			/*if (MessageShow != null)
				MessageShow(msg);*/
			//return Messages.IndexOf(msg);
        /*
		}

        /*
        /// <summary>
        /// 
        /// </summary>
        public static void ShowAlert(string messageText)
        {
            if(MessageShow!=null)
                MessageShow(new UMessage(messageText,""));
        }

		/// <summary>
		/// Return index of new message. Added new message to Messages and send to event MessageShow;
		/// </summary>
		/// <param name="messageTitle"></param>
		/// <param name="messageText"></param>
		/// <param name="messageType"></param>
		/// <param name="buttons"></param>
		/// <param name="messageIcon"></param>
		[AccessAllow(true)]
		public static void ShowMessage(string messageTitle,string messageText,UMessageType messageType, List<UMessageButton> buttons,Texture2D messageIcon)
		{
			var msg = new UMessage(messageText, messageTitle, buttons) { messageType = messageType};
			if (messageIcon != null)
				msg.icon = messageIcon;
			//msg.showTime = showTime;
			/*if (Icons.ContainsKey(msg.messageType))
				msg.icon = Icons[msg.messageType];*/
			//Messages.Add(msg);
			/*if (MessageShow != null)
				MessageShow(msg);
			//return Messages.IndexOf(msg);
		}*/

        

	    
	}
}