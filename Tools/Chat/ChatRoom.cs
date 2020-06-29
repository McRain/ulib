using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ChatRoom
{
    /// <summary>
    /// 
    /// </summary>
    public int Id;
    /// <summary>
    /// 
    /// </summary>
    public string RoomName = string.Empty;

    /// <summary>
    /// 
    /// </summary>
    public int MaxLines = 19;

    /// <summary>
    /// 
    /// </summary>
    public readonly Dictionary<int,ChatMessage> Messages = new Dictionary<int, ChatMessage>();

    /// <summary>
    /// 
    /// </summary>
    public readonly Dictionary<int,ChatUser> Users = new Dictionary<int, ChatUser>();

    private readonly Dictionary<int,ChatMessage> _messagesUnknowUsers = new Dictionary<int, ChatMessage>(); 

    #region Constructor and Instance

    /// <summary>
    /// 
    /// </summary>
    public ChatRoom()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="roomName"></param>
    public ChatRoom(string roomName)
    {
        RoomName = roomName;
    }

    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newUser"></param>
    public void AddUser(ChatUser newUser)
    {
        if (!Users.ContainsKey(newUser.id))
            Users.Add(newUser.id, newUser);
        else
            Users[newUser.id] = newUser;
        if (!_messagesUnknowUsers.ContainsKey(newUser.id)) return;
        Messages.Add(_messagesUnknowUsers[newUser.id].id, _messagesUnknowUsers[newUser.id]);
        _messagesUnknowUsers.Remove(newUser.id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="uid"></param>
    public void RemoveUser(int uid)
    {
        if (Users.ContainsKey(uid))
            Users.Remove(uid);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newMessage"></param>
    public void AddMessage(ChatMessage newMessage)
    {
        if (Users.ContainsKey(newMessage.userId))
            Messages.Add(newMessage.id, newMessage);
        else
            _messagesUnknowUsers.Add(newMessage.userId, newMessage);
        if (Messages.Count <= MaxLines) return;
        var removeCount = Messages.Count - MaxLines;
        //Messages.RemoveRange(0, removeCount);
    }

    public void RemoveMessage()
    {
        
    }
}

