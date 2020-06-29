using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ChatUser
{
    /// <summary>
    /// 
    /// </summary>
    public int id;
    /// <summary>
    /// 
    /// </summary>
    public string name = string.Empty;
    private Texture2D _icon;
    private string _iconUrl = string.Empty;
    //public ChatStatus status = ChatStatus.ONLINE;

    /// <summary>
    /// Not used in current version
    /// </summary>
    public string iconUrl
    {
        get { return _iconUrl; }
        set
        {
            if (_iconUrl != value && value.Length > 0)
            {
                _iconUrl = value;
                /*if (_icon == null)
                    _icon = ResourceManager.Instance.GetAvatar(_iconUrl, OnLoadIcon);*/
            }
        }
    }

    /// <summary>
    /// Not used in current version
    /// </summary>
    public Texture2D icon
    {
        get
        {
            if (_icon == null)
                return _icon; // ResourceManager.Instance.defaultIcon;
            return _icon;
        }
    }

    private void OnLoadIcon(object ico)
    {
        if (ico is Texture2D)
        {
            _icon = (Texture2D) ico;
            //_icon = ResourceManager.Instance.AddAvatar(_iconUrl, (Texture2D)ico);
        }
    }
}

