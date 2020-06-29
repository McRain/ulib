using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// Class for menu used
	/// </summary>
	public sealed class MenuManager
	{
		private static MenuManager _instance;

		private static Dictionary<int, UMenu> _menu = new Dictionary<int, UMenu>();
		static int _menuIndex;

		#region Events

		/// <summary>
		/// Delegate for MenuManager events
		/// </summary>
		/// <param name="id"></param>
		public delegate void MenuEvent(int id);
		/// <summary>
		/// Event on change list of menu
		/// </summary>
		public static event MenuEvent OnChangeMenu;
		/// <summary>
		/// Event on change current menu index (MenuManager.MenuIndex)
		/// </summary>
		public static event MenuEvent OnChangeIndex;
		/// <summary>
		/// Event on run MenuPoint 
		/// </summary>
		public static event MenuEvent OnPoint;

		#endregion

		#region Instances

		/// <summary>
		/// Return instance of MenuManager
		/// </summary>
		public static MenuManager Instance
		{
			get { return _instance ?? (_instance = new MenuManager()); }
		}

		/// <summary>
		/// Return instance of MenuManager
		/// </summary>
		public static MenuManager GetInstance()
		{
			return _instance ?? (_instance = new MenuManager());
		}

		#endregion

		#region Get/Set
		/// <summary>
		/// Get/Set full list of menu point
		/// </summary>
		public static Dictionary<int, UMenu> Menu
		{
			get { return _menu; }
			set
			{
				_menu = value;
				if (OnChangeMenu != null)
					OnChangeMenu(_menu.Count);
			}
		}

		/// <summary>
		/// Current menu index
		/// </summary>
		public static int MenuIndex
		{
			get
			{
				return _menuIndex;
			}
			set
			{
				if (_menuIndex != value)
				{
					_menuIndex = value;
					if (OnChangeIndex != null)
						OnChangeIndex(_menuIndex);
				}
			}
		}

		/*public static void Back()
		{
			_menuIndex = PreviosIndexes.Last();
			PreviosIndexes.RemoveAt(PreviosIndexes.Count-1);
		}

		public static void To(int newIndex)
		{
			if (_menuIndex == newIndex)
				return;
			_menuIndex = newIndex;
			PreviosIndexes.Add(_menuIndex);
			if (OnMenuEvent != null)
				OnMenuEvent();
		}*/

		#endregion

		#region Points

		/// <summary>
		/// Add one menu point to list
		/// If the list contains the point with some ID - it will be overwritten
		/// </summary>
		/// <param name="pointIndex"></param>
		/// <param name="menu"></param>
		public static void AddPoint(int pointIndex, UMenu menu)
		{
			if (!_menu.ContainsKey(pointIndex))
				_menu.Add(pointIndex, menu);
			else
				_menu[pointIndex] = menu;
			if (OnChangeMenu != null)
				OnChangeMenu(_menuIndex);
		}

		/// <summary>
		/// Set or add range of menu to menuList
		/// </summary>
		/// <param name="newPoints">The added range</param>
		/// <param name="replace">If true - old pointslist will be overwritten. If false - points will be add to pointlist </param>
		public static void AddPoints(Dictionary<int, UMenu> newPoints, bool replace)
		{
			if (replace)
				_menu = newPoints;
			else
				foreach (var menuPoint in newPoints)
					_menu.Add(menuPoint.Key, menuPoint.Value);
			if (OnChangeMenu != null)
				OnChangeMenu(_menuIndex);
		}

		/// <summary>
		/// Remove one menu point from list
		/// </summary>
		/// <param name="pointId"></param>
		public static void RemovePoint(int pointId)
		{
			if (_menu.ContainsKey(pointId))
			{
				lock (_menu)
					_menu.Remove(pointId);
				if (OnChangeMenu != null)
					OnChangeMenu(_menuIndex);
			}
		}

		#endregion

		#region Menu

		/// <summary>
		/// Return menu filtered by current meny index
		/// </summary>
		public static Dictionary<int, UMenu> GetCurrentMenu()
		{
		    var dictionary = new Dictionary<int, UMenu>();
		    foreach (var menuKey in _menu)
		        if (menuKey.Value.parent == _menuIndex) dictionary.Add(menuKey.Key, menuKey.Value);
		    return dictionary;// _menu.Where(pair => pair.Value.parent == _menuIndex).ToDictionary(pair => pair.Key, pair => pair.Value);
		}

	    /// <summary>
		/// Return menu filtered by parentId
		/// </summary>
		public static Dictionary<int, UMenu> GetMenu(int parentId)
	    {
	        var dictionary = new Dictionary<int, UMenu>();
	        foreach (var menuKey in _menu)
	        {
	            if (menuKey.Value.parent == parentId) dictionary.Add(menuKey.Key, menuKey.Value);
	        }
	        return dictionary;// _menu.Where(pair => pair.Value.parent == _menuIndex).ToDictionary(pair => pair.Key, pair => pair.Value);
	    }

	    #endregion

		#region Methods

		/// <summary>
		/// Execute list of commands in MenuPoint and return results 
		/// First try execute UMenu.commands if count> 0
		/// ELSE try execute UMenu.commandIds if count > 0
		/// </summary>
		/// <param name="mp"></param>
        public static object[] Run(UMenu mp)
		{
		    //Debug.Log("RUN");
		    if (OnPoint != null)
		        OnPoint(mp.id);
		    if (mp.commands.Count > 0)
		        return CommandManager.Execute(mp.commands);
		    if (mp.commandIds.Count > 0)
		        return CommandManager.Execute(mp.commandIds);
		    return null;
		}

	    #endregion
	}
}