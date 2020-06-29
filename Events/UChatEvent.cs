namespace ULIB
{

    /// <summary>
    /// 
    /// </summary>
    public class UChatEvent : UEvent
    {

    }

    /// <summary>
    /// Used in ChatManager for UChat event
    /// </summary>
    public class UChatEventType
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ENTRY = "ENTRY";
        /// <summary>
        /// 
        /// </summary>
        public const string RECIVED = "RECIVED";
        /// <summary>
        /// 
        /// </summary>
        public const string ERROR = "ERROR";
        /// <summary>
        /// 
        /// </summary>
        public const string EMPTY = "EMPTY";
        /// <summary>
        /// 
        /// </summary>
        public const string FLOOD = "FLOOD";

        public const string ROOM = "ROOM";
        /// <summary>
        /// 
        /// </summary>
        public const string OUT = "OUT";
    }
}
