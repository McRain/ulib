using UnityEngine;

/// <summary>
/// 
/// </summary>
public class ChatMessage
    {
        /// <summary>
        /// From user to server contains last recived message Id
        /// From server to user contains message Id
        /// </summary>
        public int id;
        /// <summary>
        /// Body of message
        /// </summary>
        public string msg = string.Empty;
        /// <summary>
        /// From user to server contains target user id (0 : to all)
        /// from server to user contains sender id
        /// </summary>
        public int userId;
        /// <summary>
        /// 
        /// </summary>
        public string messageType = "message";

        /// <summary>
        /// 
        /// </summary>
        [System.NonSerialized]
        public string senderName = string.Empty;
        
        /// <summary>
        /// 
        /// </summary>
        [System.NonSerialized]
        public Color color = Color.white;

        /// <summary>
        /// 
        /// </summary>
        [System.NonSerialized]
        public string style = "Label";
    }

