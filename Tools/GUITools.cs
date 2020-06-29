using UnityEngine;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class GUITools
	{
        public static Texture2D ScreenShoot(Camera srcCamera, int width, int height)
        {
            var renderTexture = new RenderTexture(width, height, 0);
            var targetTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
            srcCamera.targetTexture = renderTexture;
            srcCamera.Render();
            RenderTexture.active = renderTexture;
            targetTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            targetTexture.Apply();
            srcCamera.targetTexture = null;
            RenderTexture.active = null;
            srcCamera.ResetAspect();
            return targetTexture;
        }

        public static void LerpTexture(Texture2D alphaTexture,ref Texture2D texture)
        {
            var bgColors = alphaTexture.GetPixels();
            var tarCols = texture.GetPixels();
            for (var i = 0; i < tarCols.Length; i++)
            {
                tarCols[i] = bgColors[i].a > 0.99f ? bgColors[i] : Color.Lerp(tarCols[i], bgColors[i], bgColors[i].a);
            }
            texture.SetPixels(tarCols);
            texture.Apply();
        }

		/*public static float toolTipDelay = 0.35f;
		public static float toolTipX = 0;
		public static float tooltipFadeSpeed = 0.075f;*/


		/*public static void ShowQueryWindowTop(Rect windowRect, string label)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winQuery);//Background
			GUILayout.Space(30);
		}*/

		/*public static Color ColorWin(Color rgb)
		{
			GUILayout.BeginHorizontal();
			GUILayout.BeginVertical();
			rgb.r = GUILayout.VerticalSlider(rgb.r, 1.0f, 0.0f);
			GUILayout.Label(ResourceManager.Instance.colorIcons[0], GUILayout.Width(16), GUILayout.Height(16));
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			rgb.g = GUILayout.VerticalSlider(rgb.g, 1.0f, 0.0f);
			GUILayout.Label(ResourceManager.Instance.colorIcons[1], GUILayout.Width(16), GUILayout.Height(16));
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
			rgb.b = GUILayout.VerticalSlider(rgb.b, 1.0f, 0.0f);
			GUILayout.Label(ResourceManager.Instance.colorIcons[2], GUILayout.Width(16), GUILayout.Height(16));
			GUILayout.EndVertical();
			/*GUILayout.BeginVertical();
			rgb.a = GUILayout.VerticalSlider(rgb.a, 1.0f, 0.0f);
			GUILayout.Label("П");
			GUILayout.EndVertical();*/
			/*GUILayout.EndHorizontal();
			return rgb;
		}*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="minVal"></param>
		/// <param name="maxVal"></param>
		/// <returns></returns>
		public static int NumStep(int val, int minVal, int maxVal)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("-"))
			{
				if (val > minVal)
					val--;
			}
			if (GUILayout.Button("+"))
			{
				if (val < maxVal)
					val++;
			}
			GUILayout.EndHorizontal();
			return val;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="current"></param>
		/// <returns></returns>
		public static int NumStepArr(ref int[] keys, ref int current)
		{
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("-"))
			{
				if (current > 0)
					current--;
			}
			if (GUILayout.Button("+"))
			{
				if (current < keys.Length - 1)
					current++;
			}
			GUILayout.EndHorizontal();
			return keys[current];
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="label"></param>
		/// <param name="minVal"></param>
		/// <param name="maxVal"></param>
		/// <param name="valueWidth"></param>
		/// <returns></returns>
		public static int NumStep(int val, string label, int minVal, int maxVal, int valueWidth)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.ExpandWidth(true));
			if (GUILayout.Button("-"))
			{
				if (val > minVal)
					val--;
			}
			GUILayout.Label(val.ToString(), "", GUILayout.Width(valueWidth));
			if (GUILayout.Button("+"))
			{
				if (val < maxVal)
					val++;
			}
			GUILayout.EndHorizontal();
			return val;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="val"></param>
		/// <param name="label"></param>
		/// <param name="minVal"></param>
		/// <param name="maxVal"></param>
		/// <param name="valueWidth"></param>
		/// <param name="leftLabel"></param>
		/// <returns></returns>
		public static int NumStep(int val, string label, int minVal, int maxVal, int valueWidth, bool leftLabel)
		{
			GUILayout.BeginHorizontal();
			if (leftLabel)
				GUILayout.Label(label, GUILayout.ExpandWidth(true));
			if (GUILayout.Button("-"))
			{
				if (val > minVal)
					val--;
			}
			GUILayout.Label(val.ToString(), "", GUILayout.Width(valueWidth));
			if (GUILayout.Button("+"))
			{
				if (val < maxVal)
					val++;
			}
			if (!leftLabel)
				GUILayout.Label(label, GUILayout.ExpandWidth(true));
			GUILayout.EndHorizontal();
			return val;
		}

		/// <summary>
		/// Returns the texture created from the tiles of original texture. 
		/// The texture size is set to screen size.
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		public static Texture2D RepeatTexture(Texture2D src)
		{
			var cols = src.GetPixels();
			var srcW = src.width;
			var srcH = src.height;
			var targetW = srcW * ((Screen.width + srcW) / srcW);
			var targetH = srcH * ((Screen.height + srcH) / srcH);
			var result = new Texture2D(targetW, targetH, TextureFormat.RGB24, false);
			for (var ix = 0; ix < targetW; ix += srcW)
				for (var iy = 0; iy < targetH; iy += srcH)
					result.SetPixels(ix, iy, srcW, srcH, cols);
			result.Apply();
			return result;
		}

		/*static Rect loadRect = new Rect(Screen.width * 0.5f - 128, Screen.height * 0.5f - 32, 256, 64);
		//static float offset = 0;
		public static void ShowLoad()
		{
			//offset+=0.1f;
			GUI.Box(loadRect, "WAIT...");
		}

		//float offsetT = 0.0f;
		public static void DrawRepeatTexture(Rect position, Texture tex)
		{
			float rX = position.width;
			float rY = position.height;
			float cX = 0;
			float cY = 0;
			GUI.BeginGroup(position);
			for (cX = 0; cX < rX; cX += tex.width)
			{
				//GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
				for (cY = 0; cY < rY; cY += tex.height)
				{
					GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
				}
			}
			GUI.EndGroup();
		}

		public static Texture2D RenderRepeatTexture(Rect position, Texture2D tex)
		{
			float targW = position.width;
			float targH = position.height;
			//border = ResourceManager.Instance.borderTexture.width;
			Texture2D result = new Texture2D(Mathf.FloorToInt(targW), Mathf.FloorToInt(targH));
			Color[] colors = tex.GetPixels(0, 0, tex.width, tex.height);
			float cX = 0;
			float cY = 0;
			//GUI.BeginGroup(position);
			for (cX = 0; cX < targW - tex.width; cX += tex.width)
			{
				////GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
				for (cY = 0; cY < targH - tex.height; cY += tex.height)
				{
					//GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
					result.SetPixels(Mathf.FloorToInt(cX), Mathf.FloorToInt(cY), Mathf.FloorToInt(tex.width), Mathf.FloorToInt(tex.height), colors);
				}
			}
			//Color[] borderCol = ResourceManager.Instance.borderTexture.GetPixels(0, 0, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height);
			/*if (border > 0)
			{
				for (int i = 0; i < result.width-border; i += ResourceManager.Instance.borderTexture.width)
				{
					result.SetPixels(i, result.height-border*2, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//bottom
					result.SetPixels(i, 0, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//top
				}
				for (int m = 0; m < result.height - border*2; m += ResourceManager.Instance.borderTexture.height)
				{
					result.SetPixels(result.width - border * 2, m, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//bottom
					result.SetPixels(0, m, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//top
				}
				//result.SetPixels(0, (int)(rY+border*2), (int)(rX + border * 2), border, borderCol);//bottom
			}*/
			/*result.Apply();
			//GUI.EndGroup();
			return result;
		}

		//static int border = 10;

		//public static Texture2D DrawFullRepeatTexture(Rect position, Texture2D tex)
		//{
		//Texture2D result = new Texture2D((int)position.width,(int) position.height);
		//CopyPixels(ref tex, ref result);
		/*float targW = position.width;
		float targH = position.height;
		//border = ResourceManager.Instance.borderTexture.width;
		Texture2D result = new Texture2D(Mathf.FloorToInt(targW), Mathf.FloorToInt(targH));
		Color[] colors = tex.GetPixels(0, 0, tex.width, tex.height);
		float cX = 0;
		float cY = 0;
		//GUI.BeginGroup(position);
		for (cX = 0; cX < targW; cX += tex.width)
		{
			////GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
			for (cY = 0; cY < targH; cY += tex.height)
			{
				//GUI.DrawTexture(new Rect(cX, cY, tex.width, tex.height), tex);
				result.SetPixels(Mathf.FloorToInt(cX), Mathf.FloorToInt(cY), Mathf.FloorToInt(tex.width), Mathf.FloorToInt(tex.height), colors);
			}
		}
		*/
		//Color[] borderCol = ResourceManager.Instance.borderTexture.GetPixels(0, 0, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height);
		/*if (border > 0)
		{
			for (int i = 0; i < result.width-border; i += ResourceManager.Instance.borderTexture.width)
			{
				result.SetPixels(i, result.height-border*2, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//bottom
				result.SetPixels(i, 0, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//top
			}
			for (int m = 0; m < result.height - border*2; m += ResourceManager.Instance.borderTexture.height)
			{
				result.SetPixels(result.width - border * 2, m, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//bottom
				result.SetPixels(0, m, ResourceManager.Instance.borderTexture.width, ResourceManager.Instance.borderTexture.height, borderCol);//top
			}
			//result.SetPixels(0, (int)(rY+border*2), (int)(rX + border * 2), border, borderCol);//bottom
		}*/
		//	result.Apply();
		//GUI.EndGroup();
		//	return result;
		//}

		/*static void CopyPixels(ref Texture2D srcTexture, ref Texture2D targetTexture)
		{
			for (int trgX = 0; trgX < targetTexture.width; trgX+=srcTexture.width)
			{
				for (int trgY = 0; trgY < targetTexture.height; trgY+=srcTexture.height)
				{
					for (int i = 0; i < srcTexture.width; i++)
					{
						for (int m = 0; m < srcTexture.height; m++)
						{
							Color col = srcTexture.GetPixel(i, m);
							targetTexture.SetPixel(trgX+i, trgY+m, col);
						}
					}
				}
			}
		}*/

		/*public static Color ColorPalette(string label, Color rgb)
		{
			Vector2 mousePos = Event.current.mousePosition;
			GUILayout.BeginHorizontal();
			GUILayout.Label(label, GUILayout.Width(175));
			GUILayout.Box("","PaletteBox");
			Rect rct = GUILayoutUtility.GetLastRect();
			if(rct.Contains(mousePos) && Input.GetMouseButtonDown(0)){
				Texture2D bgColor = Camera.main.GetComponent<MainMenu>().skin.customStyles[0].normal.background;
				rgb = bgColor.GetPixel(Mathf.FloorToInt(mousePos.x - rct.x), Mathf.FloorToInt(mousePos.y-rct.y-20));
			
			}
			GUILayout.EndHorizontal();
			return rgb;
		}*/

		/*public static void LayoutWindowButtons(EventWindowOK onSave,string saveLabel , EventWindowOK onLoad , string loadLabel, int helpID){
			if (GUILayout.Button(saveLabel))
				onSave();
			if (GUILayout.Button(loadLabel))
				onLoad();
			if (GUILayout.Button("?"))
			{

			}			
		}*/

		/*public static void TitledWindow(Rect rect,ref Texture2D txtr)
		{
			if(txtr==null)
				txtr = ResourceManager.WindowTexture;
			//return txtr;
			//return GUI.Window(winID, rect, fun, new GUIContent(titleLabel, WindowManager.TitleTexture), "guiWindow");
		}*/

		//static Dictionary<Rect, float> rects = new Dictionary<Rect, float>();

		/*public static void ShowWindow(Rect windowRect, string label, int textureIndex)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[textureIndex]);//Background
			GUILayout.Space(30);
		}

		public static void ShowWindow(Rect windowRect, string label, ref bool showClose)
		{	//GUITools.ShowWindow(vinilWinRect, LanguageManager.GetLabels("tuning", "vinil", " : "), ref vinilWinShow);
			//GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
			//GUITools.ShowWindowToolTip(GUI.tooltip);
			//WindowManager.TitleTexture 
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[2]);//Background
			if (showClose && ShowCloseButton(windowRect.width - 40))
				showClose = false;
			GUILayout.Space(30);
		}

		public static void ShowWindow(Rect windowRect, string label, ref bool showClose, int textureIndex)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[textureIndex]);//Background
			if (showClose && ShowCloseButton(windowRect.width - 40))
				showClose = false;
			GUILayout.Space(30);
		}

		public static void ShowWindow(ref Rect windowRect, string label, ref bool showClose, ref float maxisize)
		{
			if (windowRect.height >= 30)
			{
				/*if (GUI.Button(new Rect(20, 2, 20, 20), WindowManager.minButtonTexture))
				{
					maxisize = windowRect.height;
					windowRect.height = 23;
				}*/
				/*if (showClose && ShowCloseButton(windowRect.width - 40))
					showClose = false;
				GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			}
			else
			{
				/*if (GUI.Button(new Rect(50, 1, 15, 15), WindowManager.minButtonTexture))
					windowRect.height = maxisize;*/
				/*if (showClose && ShowCloseButton(windowRect.width - 60))
					showClose = false;
				GUI.Label(new Rect(0, 0, windowRect.width, 16), label, "smallCenterLabel");//Label
			}


			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[2]);//Background

			GUILayout.Space(30);
		}

		public delegate void WinClose();

		public static void ShowWindow(Rect windowRect, string label, WinClose winFunc)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[2], ScaleMode.StretchToFill);
			if (ShowCloseButton(windowRect.width - 40))
				winFunc.Invoke();
			GUILayout.Space(30);
		}

		public static void ShowWindow(Rect windowRect, string label, Texture2D icon, WinClose winFunc)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[2]);
			GUI.DrawTexture(new Rect(0, 0, 64, 64), icon);
			if (ShowCloseButton(windowRect.width - 40))
				winFunc.Invoke();
			GUILayout.Space(30);
		}

		public static void ShowTexturedWindow(ref Texture2D txtr, Rect windowRect, string label, WinClose winFunc)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), txtr);
			//GUI.DrawTexture(new Rect(0, 0, 64, 64), icon);
			if (ShowCloseButton(windowRect.width - 40))
				winFunc.Invoke();
			GUILayout.Space(30);
		}


		public static void ShowWindow(ref Rect windowRect, string label, int minSize, int maxSize, WinClose winFunc)
		{
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[2]);
			if (minSize > 0)
				if (GUI.Button(new Rect(windowRect.width - 70, 2, 20, 20), "_"))
				{
					if (windowRect.height == maxSize)
						windowRect.height = minSize;
					else
						windowRect.height = maxSize;
				}
			if (ShowCloseButton(windowRect.width - 40))
				winFunc.Invoke();
			GUILayout.Space(30);
		}

		public static void ShowWindow(Rect windowRect, string label, WinClose winFunc, int textureIndex)
		{	//GUITools.ShowWindow(vinilWinRect, LanguageManager.GetLabels("tuning", "vinil", " : "), ref vinilWinShow);
			//GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true)
			//GUITools.ShowWindowToolTip(GUI.tooltip);
			//WindowManager.TitleTexture 
			GUI.Label(new Rect(0, 0, windowRect.width, 22), label, "smallCenterLabel");//Label
			GUI.DrawTexture(new Rect(0, 23, windowRect.width, windowRect.height - 23), ResourceManager.Instance.winSkins[textureIndex]);//Background
			if (ShowCloseButton(windowRect.width - 32))
				winFunc.Invoke();
			GUILayout.Space(30);
		}

		static bool ShowCloseButton(float xPos)
		{
			return GUI.Button(new Rect(xPos, 0, 22, 22), ResourceManager.Instance.closeButton);
		}

		static string previosTooltip = "";
		static float setTooltipTime = 0.0f;
		static float tipA = 0.0f;

		public static void ShowToolTip(string ttip)
		{
			if (ttip != "")
			{
				if (previosTooltip != ttip)
				{
					previosTooltip = ttip;
					setTooltipTime = Time.time;
					tipA = -toolTipDelay;
				}
				else if ((Time.time - setTooltipTime) > toolTipDelay)
				{
					GUI.depth = 999;
					float fontWidth = ttip.Length * 7;
					//string bgStyle = "tooltipBG";
					float pos = Mathf.Clamp(Input.mousePosition.x - toolTipX, fontWidth * 0.5f, Screen.width - fontWidth * 0.5f);
					float posY = Mathf.Clamp(Screen.height - Input.mousePosition.y - 50, 0, Screen.height - 28);
					//if (Input.mousePosition.x + fontWidth > Screen.width-10)
					//{
					//	bgStyle += "2";
					//	pos = Screen.width - fontWidth-10;
					//}
					if (tipA < 1)
						tipA += tooltipFadeSpeed;
					GUI.Label(new Rect(Input.mousePosition.x - 12, Screen.height - Input.mousePosition.y - 25, 32, 32), ResourceManager.Instance.tt);
					GUI.BeginGroup(new Rect(pos - fontWidth * 0.5f, posY, fontWidth, 28), "", "tooltip");
					GUI.color = new Color(0, 0, 0, tipA);
					//GUI.Box(new Rect(0, 0, fontWidth, 35), "", "tooltip");
					GUI.Label(new Rect(2, 2, fontWidth, 25), ttip, "smallCenterLabel");
					GUI.color = new Color(1, 1, 1, tipA);
					GUI.Label(new Rect(0, 0, fontWidth, 25), ttip, "smallCenterLabel");
					GUI.EndGroup();
				}
			}
		}

		public static void ShowWindowToolTip(string ttip)
		{
			if (ttip != "")
			{
				//Debug.Log(ttip);
				int ttLines = 0;
				if (ttip.LastIndexOf("\n") >= 0)
					ttLines = 25;
				float fontWidth = (ttip.Length - ttLines * 0.75f) * 11 + 11;
				//string bgStyle = "tooltipBG";
				float pos = Event.current.mousePosition.x + toolTipX;
				GUI.color = new Color(0, 0, 0, 0.75f);
				GUI.Box(new Rect(pos - 5, Event.current.mousePosition.y - 31 - ttLines, fontWidth, 25 + ttLines), "", "tooltip");
				GUI.Label(new Rect(pos + 1, Event.current.mousePosition.y - 28 - ttLines, fontWidth, 25), ttip, "smallCenterLabel");
				GUI.color = new Color(1, 1, 1, 1);
				GUI.Label(new Rect(pos - 1, Event.current.mousePosition.y - 30 - ttLines, fontWidth, 25), ttip, "smallCenterLabel");
			}
		}

		public static bool StringToBool(string val)
		{
			if (val.CompareTo("+") > 0 || val.CompareTo("true") > 0 || val.CompareTo("1") > 0)
				return true;
			else
				return false;
		}

		public static string TaskToString(string task, object val)
		{
			if (task.Contains("time"))
				return GameTools.SecoundToMinute((float)val) + ":" + GameTools.SecoundToSecoundMs((float)val);
			else
				return val.ToString();
		}

		public static Texture2D TextToTexture(Rect screeRect)
		{
			Texture2D tex = new Texture2D((int)screeRect.width, (int)screeRect.height, TextureFormat.RGB24, false);
			tex.ReadPixels(screeRect, 0, 0);
			tex.Apply();
			return tex;
		}

		/*public static GameObject createChartElements(int[] vals){
			GameObject chart = new GameObject();
			int i = 0;
			foreach(int val in vals){
				createChartElement(i, val).transform.parent = chart.transform;
			}
			return chart;
		}

		static GameObject createChartElement(int count, int val)
		{
			GameObject result = GameObject.CreatePrimitive(PrimitiveType.Cube);
			result.transform.localPosition = new Vector3(0, count, 0);
			result.transform.localScale = new Vector3(0, 0, val);
			return result;
		}*/
	}
}
