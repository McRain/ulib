using System;
using ULIB;

/// <summary>
/// Model for MessageSystem
/// </summary>
public class UMessage
{
    /// <summary>
    /// Message text.
    /// </summary>
    public string text = string.Empty;

    /*
    /// <summary>
    /// 
    /// </summary>
    public MessageManager.OnMessage messageEvent;*/

    /// <summary>
    /// Type of message. You can use UMessageType or your own.
    /// </summary>
    public object messageType = UMessageType.Alert;

    /// <summary>
    /// 
    /// </summary>
    [NonSerialized]
    public UMessageEvent onEvent;

    //public event UMessageEvent Realise;

    /*
    /// <summary>
    /// Title for message.
    /// Default: string.Empty
    /// </summary>
    public string title = string.Empty;

    /// <summary>
    /// Icon for message.
    /// Default: null
    /// </summary>
    public Texture2D icon;

    /// <summary>
    /// Data for message body.
    /// Can contain text, icon.
    /// </summary>
    public UMessageData[] data;

    /// <summary>
    /// Type of message.
    /// Default: UMessageType.Alert
    /// </summary>
    public UMessageType messageType = UMessageType.Alert;*/

    /// <summary>
    /// Array of messages buttons. Can be empty.
    /// </summary>
    private UMessageButton[] _buttons;

    /*
    /// <summary>
    /// 
    /// </summary>
    public float showTime;*/

    /// <summary>
    /// 
    /// </summary>
    public UMessage()
    {
        text = string.Empty;
        Buttons = new UMessageButton[0];
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageText"></param>
    public UMessage(string messageText):this(messageText,new UMessageButton[0])
    {
            
    }

    /// <summary>
    /// 
    /// </summary>
    public UMessage(string messageText, UMessageButton[] messageButtons)
    {
        text = messageText;
        _buttons = messageButtons;
        SetButtonContaner();
           
    }

    */
    /*private void SetButtonContaner()
    {

        /*if (_buttons != null)
            foreach (var uMessageButton in _buttons)
            {
                uMessageButton.container = this;
                uMessageButton.RealiseEvent += ButtonRealise;
            }*/

    //}

    /// <summary>
    /// 
    /// </summary>
    public UMessageButton[] Buttons
    {
        get { return _buttons; }
        set
        {
            _buttons = value;
            SetButtonsEvent();
            //SetButtonContaner();
        }
    }

    void SetButtonsEvent()
    {
        if(_buttons!=null)
            foreach (var uMessageButton in _buttons)
                uMessageButton.onEvent += OnChildEvent;
    }

    void OnChildEvent(UEvent uEvent)
    {
        if (onEvent != null)
            onEvent(new UEvent
                        {eventType = uEvent.eventType, owner = this, sender = uEvent.sender, value = uEvent.value});
    }

    /*
    internal void ButtonRealise(object buttonown, object buttonsend, object val)
    {
        if (Realise != null)
            Realise(this, buttonsend, val);
    }*/

    public override string ToString()
    {
        var result = base.ToString() + " text: " + text + " type: " + messageType + " ;";
        foreach (var uMessageButton in _buttons)
            result += " " + uMessageButton + " ; ";
        return result;
    }

    /*
    /// <summary>
    /// Constructor for message.
    /// </summary>
    /// <param name="messageData"></param>
    /// <param name="messageTitle"></param>
    /// <param name="buttons"></param>
    /// <param name="typeOfMessage"></param>
    /// <param name="messageIcon"></param>
    /// <param name="messageStyle"></param>
    public UMessage(UMessageData[] messageData, string messageTitle, IEnumerable<UMessageButton> buttons, UMessageType typeOfMessage,Texture2D messageIcon,string messageStyle)
    {
        data =  messageData;
        title = messageTitle;
        _buttons = new List<UMessageButton>(buttons);
        messageType = typeOfMessage;
        icon = messageIcon;
        style = messageStyle;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textMessage"></param>
    public UMessage(string textMessage)
    {
        data = new UMessageData[1]{new UMessageData(){value = textMessage}};
        text = textMessage;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="textMessage"></param>
    /// <param name="buttons"></param>
    public UMessage(string textMessage, IEnumerable<UMessageButton> buttons)
    {
        text = textMessage;
        if (buttons != null)
            _buttons = new List<UMessageButton>(buttons);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageText"></param>
    /// <param name="titleText"></param>
    public UMessage(string messageText,string titleText)
    {
        text = messageText;
        title = titleText;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="messageText"></param>
    /// <param name="titleText"></param>
    /// /// <param name="buttonList"></param>
    public UMessage(string messageText, string titleText, List<UMessageButton> buttonList)
    {
        text = messageText;
        title = titleText;
        if (buttonList != null)
        {
            _buttons = buttonList;
            /*foreach (var button in _buttons)
                button.container = this;*/
    /*
}
}

/// <summary>
/// 
/// </summary>
/// <param name="messageText"></param>
/// <param name="titleText"></param>
/// /// <param name="buttonList"></param>
public UMessage(string messageText, string titleText, IEnumerable<UMessageButton> buttonList)
{
text = messageText;
title = titleText;
if (buttonList != null)
{
_buttons = new List<UMessageButton>(buttonList);
/*foreach (var button in _buttons)
button.container = this;*/
    /*
}
}*/



    /*
    /// <summary>
    /// 
    /// </summary>
    public UMessageButton[] Buttons
    {
        get { return _buttons; }
    }*/
}

