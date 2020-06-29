using UnityEngine;

namespace ULIB
{
    /// <summary>
    /// Not used in current version
    /// </summary>
    class AdvertisingTools:MonoBehaviour
    {
        private string text = "Advertising";
        void Start()
        {
            
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0,50,300,50));
            GUILayout.Label(text);
            GUILayout.EndArea();
        }
    }
}
