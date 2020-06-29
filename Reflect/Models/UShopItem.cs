using ULIB;
using UnityEngine;

	/// <summary>
	/// Items for shop.
	/// </summary>
	public class UShopItem:UChildModel,IIcon
	{
		//paretn - may be pack or stock
		//label - Title
		/// <summary>
		/// Unique id items in shop
		/// </summary>
		public int itemId;
		/// <summary>
		/// Price
		/// </summary>
		public float price;

		/// <summary>
		/// Additional price
		/// </summary>
		public float priceGold;

		/// <summary>
		/// 
		/// </summary>
		public int Count;

		private string _iconName = "";

		/// <summary>
		/// Get icon texture. For set use IconName
		/// </summary>
		public Texture2D Icon { get; set; }

		/// <summary>
		/// Change icon name. If ResourceManager contains not icon in cache - texture will be load.
		/// </summary>
		public string IconName
		{
			get { return _iconName; }
			set
			{
				if(!_iconName.Equals(value))
				{
					_iconName = value;
					/*ResourceManager.GetIcon(
						_iconName,
						OnGetIcon);*/
				}
			}
		}

		void OnGetIcon(string key,object iconTexture)
		{
			Icon = (Texture2D) iconTexture;
		}
	}
