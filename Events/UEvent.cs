namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    public class UEventType
    {
        public const string NONE = "NONE";
        public const string CLICK = "CLICK";
    }

    /// <summary>
    /// 
    /// </summary>
    public class UEvent
    {
        /// <summary>
        /// Object started event
        /// </summary>
        public object sender;

        /// <summary>
        /// Current sender of event
        /// </summary>
        public object owner;

        /// <summary>
        /// Value from sender
        /// </summary>
        public object value;

        /// <summary>
        /// 
        /// </summary>
        public string eventType = UEventType.NONE;
    }
}
