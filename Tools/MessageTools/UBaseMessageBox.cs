using System;
using UnityEngine;

namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    public class UBaseMessageBox : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public Rect rect;
        /// <summary>
        /// 
        /// </summary>
        public bool visible;
        /// <summary>
        /// 
        /// </summary>
        public int msgId = 1001;

        /// <summary>
        /// 
        /// </summary>
        public float startTime;

        /// <summary>
        /// 
        /// </summary>
        public GUIContent content = new GUIContent();

        /// <summary>
        /// 
        /// </summary>
        public GUI.WindowFunction windowFunction;

        /// <summary>
        /// 
        /// </summary>
        public UBaseMessageGui uBaseMessageGui;

        private UMessage _message;

        /// <summary>
        /// 
        /// </summary>
        [NonSerialized]
        public UMessageEvent onEvent;

        /// <summary>
        /// 
        /// </summary>
        public UMessage message
        {
            get { return _message; }
            set
            {
                _message = value;
                if(_message!=null)
                    _message.onEvent += OnMessageEvent;
            }
        }

        void OnMessageEvent(UEvent uEvent)
        {
            if (onEvent != null)
                onEvent(new UEvent { eventType = uEvent.eventType, owner = this, sender = uEvent.sender, value = uEvent.value });
        }

        void Awake()
        {
            windowFunction = WinAlert;
        }

        void WinAlert(int id)
        {
            GUILayout.Label(content);
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            foreach (var uButton in message.Buttons)
                if (GUILayout.Button(uButton.label))
                    uButton.Click();
            GUILayout.EndHorizontal();
            GUI.BringWindowToFront(id);
        }
    }
}
