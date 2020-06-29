
using System;
using ULIB;

/// <summary>
/// Class with data for button from message window
/// </summary>
public class UMessageButton
{
    /// <summary>
    /// Text for button label
    /// Default: string.Empty
    /// </summary>
    public string label = string.Empty;

    /// <summary>
    /// The method is called when the button is pressed.
    /// Receives as a parameter to the contents of the "value" field.
    /// </summary>
    public MessageManager.OnButton callback;

    /// <summary>
    /// 
    /// </summary>
    public UCommand command;

    /// <summary>
    /// The command start execute when the button is pressed.
    /// </summary>
    public object value;

    /// <summary>
    /// Parent UMessage
    /// </summary>
    public UMessage container;

    /// <summary>
    /// 
    /// </summary>
    [NonSerialized]
    public UMessageEvent onEvent;

    public override string ToString()
    {
        return base.ToString() + " label: " + label + " value: " + value;
    }

    /*
    /// <summary>
    /// Icon for button
    /// Default: null
    /// </summary>
    public Texture2D icon;
    /// <summary>
    /// Name of style for button.
    /// Default: Button
    /// </summary>
    public string style = "Button";*/
    /*
/// <summary>
/// Function called by click button
/// </summary>
public MessageManager.OnMessageButton func;

/// <summary>
/// Functions called by click button
/// </summary>
public List<UCommand> commands/* = new List<UCommand>()*/
    /*;
/// <summary>
/// Any data, sended to function 'func' on click button.
/// </summary>
public object value;*/

    //internal UMessage container;

    //public event MessageManager.ButtonEvent OnRealise;

    /// <summary>
    /// 
    /// </summary>
    public UMessageButton()
    {
        //container = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonLabel"></param>
    /// <param name="callbackFunction"></param>
    /// <param name="callbackValue"></param>
    public UMessageButton(string buttonLabel, MessageManager.OnButton callbackFunction, object callbackValue)
    {
        label = buttonLabel;
        callback = callbackFunction;
        value = callbackValue;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonLabel"></param>
    /// <param name="ucommand"></param>
    /// <param name="val"></param>
    public UMessageButton(string buttonLabel, UCommand ucommand)
    {
        label = buttonLabel;
        command = ucommand;
    }

    /*
    /// <summary>
    /// 
    /// </summary>
    /// <param name="buttonLabel"></param>
    /// <param name="buttonIcon"></param>
    /// <param name="buttonStyle"></param>
    /// <param name="callback"></param>
    /// <param name="buttonCommands"></param>
    /// <param name="startValue"></param>
    public UMessageButton(string buttonLabel,Texture2D buttonIcon,string buttonStyle,MessageManager.OnMessageButton callback,List<UCommand> buttonCommands,object startValue )
    {
        label = buttonLabel;
        /*icon = buttonIcon;
        style = buttonStyle;
        func = callback;
        commands = buttonCommands;
        value = startValue;*/
    /*
}

/*
/// <summary>
/// 
/// </summary>
/// <param name="buttonLabel"></param>
/// <param name="onClickEvent"></param>
/// <param name="val"></param>
public UMessageButton(string buttonLabel,  object val)
: this(buttonLabel, MessageManager.ButtonIcon,  MessageManager.ButtonCommands, val)
{
			
}

/// <summary>
/// 
/// </summary>
/// <param name="buttonLabel"></param>
/// <param name="onClickEvent"></param>
/// <param name="eventCommands"></param>
/// <param name="val"></param>
public UMessageButton(string buttonLabel,  List<UCommand> eventCommands, object val)
: this(buttonLabel, MessageManager.ButtonIcon, eventCommands,val)
{
			
}

/// <summary>
/// 
/// </summary>
/// <param name="buttonLabel"></param>
/// <param name="buttonIcon"></param>
/// <param name="clickCommands"></param>
/// <param name="clickValue"></param>
public UMessageButton(string buttonLabel, Texture2D buttonIcon,List<UCommand> clickCommands, object clickValue  )
{
label = buttonLabel;
icon = buttonIcon;
//func = clickEvent;
commands = clickCommands;
value = clickValue;
//container = null;
}*/

    /// <summary>
    /// 
    /// </summary>
    [System.Obsolete("Use Click()")]
    public void Realise()
    {
        Click();
    }

    /// <summary>
    /// 
    /// </summary>
    public void Click()
    {
        if (command != null)
            CommandManager.Execute(command);
        if (callback != null)
            callback(value);
        if (onEvent != null)
            onEvent(new UEvent { eventType = UEventType.CLICK, sender = this, owner = this, value = value });
        //object obj = null;
        /*if (commands != null)
            value = CommandManager.Execute(commands);
        if (func != null)
            func(value,this);*/

        /*if (value == null)
            value = obj;*/
        /*if(func!=null)
            func(value,this);*/
        //container.Actived = this;
        //MessageManager.Realise(this);
        /*if (OnRealise != null)
            OnRealise(value, this);*/
    }
}