using System.Collections;
using System.Collections.Generic;

namespace ULIB
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class ShopManager
	{
		private static ShopManager _instance;

	    private static IRemoteObject _wwwObject;

		/// <summary>
		/// 
		/// </summary>
		public static List<UShopItem> Items = new List<UShopItem>();

		/*/// <summary>
		/// 
		/// </summary>
		public static List<UShopItem> items;*/
		/// <summary>
		/// 
		/// </summary>
		public static bool SendedRequest;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="num"></param>
		/// <param name="data"></param>
		/// <param name="parameters"></param>
		public delegate void ShopEvent(int num, object data,object parameters);
		/// <summary>
		/// 
		/// </summary>
		public static event ShopEvent OnRecivedItems;
		/// <summary>
		/// 
		/// </summary>
		public static event ShopEvent OnRecivedBuy;

		/*/// <summary>
		/// 
		/// </summary>
		public static event ShopEvent OnRecivedSell;*/

		#region Keys
		/// <summary>
		/// 
		/// </summary>
		public static string KeyItem = "key";
		/// <summary>
		/// 
		/// </summary>
		public static string KeyCount = "count";
		/// <summary>
		/// 
		/// </summary>
		public static string KeyList = "list";
		/// <summary>
		/// 
		/// </summary>
		public static string KeySum = "sum";
		/// <summary>
		/// 
		/// </summary>
		public static string KeyParameters = "param";

	    /// <summary>
	    /// 
	    /// </summary>
	    public static string ServerClass = "ShopManager";

		/// <summary>
		/// 
		/// </summary>
		public static string GetListMethod = "GetItemList";
		/// <summary>
		/// 
		/// </summary>
		public static string BuyMethod = "RequestBuy";
		/// <summary>
		/// 
		/// </summary>
		public static string SellMethod = "RequestSell";
        
		/// <summary>
		/// 
		/// </summary>
		public static GatewayType shopGateway = GatewayType.Www;


		#endregion

		#region Instances

		/// <summary>
		/// Return instance of ShopManager
		/// </summary>
		public static ShopManager Instance
		{
			get { return _instance ?? (_instance = new ShopManager()); }
		}

		/// <summary>
		/// Return instance of ShopManager
		/// </summary>
		public static ShopManager GetInstance()
		{
			return _instance ?? (_instance = new ShopManager());
		}

		#endregion

		#region Lists
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parameters"></param>
		public static void GetItemList(object parameters)
		{
            if (Items.Count == 0)
            {
                if (_wwwObject == null)
                    _wwwObject = Gateway.GetSender(shopGateway);
                _wwwObject.Call(ServerClass,
                                GetListMethod,
                                new Hashtable
                                    {
                                        {KeyParameters, parameters}
                                    },
                                OnRecivedItemlist);
            }

            else
            {
                OnRecivedItemlist(new Hashtable
				                  		{
											{KeyList,Items},
											{KeyParameters,parameters}
				                  		}
                                    );
            }
		}

		static void OnRecivedItemlist(object inData)
		{
			if(inData is Hashtable)
			{
				var hash = (Hashtable) inData;
				var list = (List<UShopItem>) hash[KeyList];
				Items = new List<UShopItem>(list);
				if (OnRecivedItems != null)
					OnRecivedItems(Items.Count,null, hash[KeyParameters]);
			}else if(Gateway.Debug)
			{
			    if (inData != null)
			        ULog.Log("ShopManager.OnRecivedItemlist : not Hashtable recived: Type of " + inData.GetType() +
			                 " : as string " + inData);
			    else
			        ULog.Log("ShopManager.OnRecivedItemlist : not Hashtable recived: NULL");

			    //ULog.Log("ShopManager.OnRecivedItemlist : not Hashtable recived"+(inData!=null?inData.ToString():" - NULL"));
			}
		}
		#endregion

		#region Buy

		/// <summary>
		/// 
		/// </summary>
		/// <param name="shopItemIndex"></param>
		/// <param name="itemsCount"></param>
		/// <param name="userMoney"></param>
		/// <param name="userMoneyGold"></param>
		/// <returns></returns>
		public static bool CanBuy(int shopItemIndex,int itemsCount,int userMoney,int userMoneyGold)
		{
			var canBuy = false;
			var activedItem = Items[shopItemIndex];
			if (activedItem.price * itemsCount <= userMoney &&
				activedItem.priceGold * itemsCount <= userMoneyGold)
				canBuy = true;
			return canBuy;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="shopItemId"></param>
		/// <param name="itemsCount"></param>
		public static void RequestBuyItem(int shopItemId, int itemsCount)
		{
			RequestBuyItem(shopItemId, itemsCount, null);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="shopItemId"></param>
		/// <param name="itemsCount"></param>
		/// <param name="parameters"></param>
		public static bool RequestBuyItem(int shopItemId, int itemsCount,object parameters)
		{
			if (!SendedRequest)
			{
				SendedRequest = true;
				if (_wwwObject == null)
                    _wwwObject = Gateway.GetSender(shopGateway);
                _wwwObject.Call(ServerClass, BuyMethod, new Hashtable
			           													{
			           														{ KeyItem, shopItemId }, 
																			{ KeyCount, itemsCount },
																			{KeyParameters,parameters}
			           													}, OnRequestedBuy);
				return true;
			}
			return false;
		}

		static void OnRequestedBuy(object inData)
		{
			SendedRequest = false;
			if (inData is Hashtable)
			{
				var hash = (Hashtable)inData;
				var buyCount = (int)hash[KeyCount];
				var shopItemId = (int)hash[KeyItem];
				if (OnRecivedBuy != null)
					OnRecivedBuy(shopItemId,buyCount, hash[KeyParameters]);

			}else if(Gateway.Debug)
			{
				ULog.Log("ShopManager.OnRequestedBuy "+inData);
			}
		}

		#endregion

		#region Sell

		/*/// <summary>
		/// 
		/// </summary>
		/// <param name="shopItemId"></param>
		/// <param name="itemsCount"></param>
		public static void RequestSellItem(int shopItemId, int itemsCount)
		{
			RequestSellItem(shopItemId, itemsCount, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="shopItemId"></param>
		/// <param name="itemsCount"></param>
		/// <param name="parameters"></param>
		public static void RequestSellItem(int shopItemId, int itemsCount, object parameters)
		{
			SendedRequest = true;
			var hash = new Hashtable
			           	{
			           		{ KeyItem, shopItemId }, 
							{ KeyCount, itemsCount }, 
							{ KeyParameters, parameters }
			           	};
			Gateway.GetSender(shopGateway).Call(_instance, SellMethod, hash, OnRequestedSell);
		}

		static void OnRequestedSell(object inData)
		{
			SendedRequest = false;
			if (inData is Hashtable)
			{
				var hash = (Hashtable)inData;
				var sellCount = Int32.Parse(hash[KeyCount].ToString());
				var shopItemId = 0;
				if(sellCount>0)
				{
					shopItemId = Int32.Parse(hash[KeyItem].ToString());
				}
				
				if (OnRecivedSell != null)
					OnRecivedSell(sellCount, shopItemId, hash[KeyParameters]);
			}
		}*/
		#endregion
	}
}
