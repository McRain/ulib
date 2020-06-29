using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ULIB
{
    /// <summary>
    /// 
    /// </summary>
    public class UBaseMessageGui : MonoBehaviour
    {
        /// <summary>
        /// 
        /// </summary>
        public bool actived;
        /*public float showSpeed = 2.7f;
        public float showTime = 2.0f;
    
        public float waitMouseOver = 0.5f;*/

        /// <summary>
        /// 
        /// </summary>
        public int startMessageGuiId = 1000;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msgGui"></param>
        public delegate IEnumerator MessageGuiEvent(UBaseMessageBox msgGui);

        /*public Dictionary<object, Rect> messageRects = new Dictionary<object, Rect>();
        public Dictionary<object, MessageGuiEvent> showers = new Dictionary<object, MessageGuiEvent>();
        public Dictionary<object, MessageGuiEvent> showeds = new Dictionary<object, MessageGuiEvent>();
        public Dictionary<object, MessageGuiEvent> hiders = new Dictionary<object, MessageGuiEvent>();*/

        protected bool hideFlag;
        private static int _msgId = 1000;

        private readonly List<UBaseMessageBox> _msgs = new List<UBaseMessageBox>();

        //private readonly Dictionary<object, Type> _types = new Dictionary<object, Type>();

        /// <summary>
        /// 
        /// </summary>
        public readonly Dictionary<object, UMessageWorker> workers = new Dictionary<object, UMessageWorker>();

        /// <summary>
        /// 
        /// </summary>
        public string rectField = "";

        void Start()
        {
            _msgId = startMessageGuiId;
            MessageManager.OnMessageStart += OnMessage;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="worker"></param>
        public void AddWorker(object messageType, UMessageWorker worker)
        {
            workers.Add(messageType, worker);
        }

        /*public void AddWorkers(object messageType,MessageGuiEvent shower,MessageGuiEvent showed, MessageGuiEvent hider)
        {
            if(shower!=null && !showers.ContainsKey(messageType))
                showers.Add(messageType,shower);
            if (showed != null && !showeds.ContainsKey(messageType))
                showeds.Add(messageType, showed);
            if (hiders != null && !hiders.ContainsKey(messageType))
                hiders.Add(messageType, hider);
        }

        public void AddWorkers(object messageType, MessageGuiEvent shower, MessageGuiEvent showed, MessageGuiEvent hider,Rect startRect)
        {
            AddWorkers(messageType,shower,showed,hider);
            if (startRect != null && !messageRects.ContainsKey(messageType))
                messageRects.Add(messageType, startRect);
        }*/

        void OnMessage(UMessage inmsg)
        {
            //Debug.Log("base.OnMessage type " + inmsg.messageType);

            //var ubox = (UBaseMessageBox)gameObject.AddComponent(_types[inmsg.messageType]);
            /*var dlist = inmsg.onEvent.GetInvocationList();
            var contains = false;
            foreach (var del in dlist)
            {
                if(del.Target.GetType() == typeof(UBaseMessageGui))
                    contains = true;
            }
            if (!contains)
                inmsg.onEvent += OnClick;*/
            var ubox = (UBaseMessageBox)gameObject.AddComponent(workers[inmsg.messageType].component);
            ubox.msgId = _msgId++;
            ubox.message = inmsg;
            //ubox.rect = messageRects[ubox.msg.messageType];
            ubox.rect = workers[ubox.message.messageType].rect;
            ubox.content = new GUIContent( ubox.message.text);
            //ubox.uBaseMessageGui = this;
            ubox.onEvent += OnClick;
            _msgs.Add(ubox);
            StartCoroutine(workers[ubox.message.messageType].shower(ubox));
        }

        void OnClick(UEvent uEvent)
        {
            var box = (UBaseMessageBox) uEvent.owner;
            StartCoroutine(workers[box.message.messageType].hider(box));
        }

        void OnGUI()
        {
            foreach (var uMessageBox in _msgs)
                if (uMessageBox.visible)
                    uMessageBox.rect = GUI.Window(uMessageBox.msgId, uMessageBox.rect, uMessageBox.windowFunction, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uMessageBox"></param>
        public void Showed(UBaseMessageBox uMessageBox)
        {
            StartCoroutine(IShowed(uMessageBox));
        }

        IEnumerator IShowed(UBaseMessageBox uMessageBox)
        {
            yield return new WaitForSeconds(1);
            if (uMessageBox.message.Buttons.Length > 0 &&
                uMessageBox.rect.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                StartCoroutine(workers[uMessageBox.message.messageType].showed(uMessageBox));
            else
                StartCoroutine(workers[uMessageBox.message.messageType].hider(uMessageBox));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uMessageBox"></param>
        public void Hide(UBaseMessageBox uMessageBox)
        {
            StartCoroutine(workers[uMessageBox.message.messageType].hider(uMessageBox));
        }
    }
}
