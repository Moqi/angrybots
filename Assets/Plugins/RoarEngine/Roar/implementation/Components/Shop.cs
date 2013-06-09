using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Shop : IShop
	{
		protected IWebAPI.IShopActions shopActions;
		protected IDataStore dataStore;
		protected ILogger logger;

		public Shop (IWebAPI.IShopActions shopActions, IDataStore dataStore, ILogger logger)
		{
			this.shopActions = shopActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback<IDictionary<string,DomainObjects.ShopEntry> > callback)
		{
			dataStore.shop.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.shop.HasDataFromServer; } }

		public void Buy (string shop_ikey, Roar.Callback<WebObjects.Shop.BuyResponse> callback)
		{
			ShopBuy (shop_ikey, callback);
		}

		public IList<DomainObjects.ShopEntry> List ()
		{
			return dataStore.shop.List();
		}


		// Returns the *object* associated with attribute `key`
		public DomainObjects.ShopEntry GetShopItem (string ikey)
		{
			return dataStore.shop.Get (ikey);
		}

		public void ShopBuy (string shop_ikey, Roar.Callback<WebObjects.Shop.BuyResponse> cb)
		{
			var shop_item = dataStore.shop.Get (shop_ikey);

			// Make the call if the item is in the shop
			if (shop_item == null) {
				logger.DebugLog ("[roar] -- Cannot find to purchase: " + shop_ikey);
				return;
			}
			logger.DebugLog ("trying to buy me a : " + shop_item.ikey + ":" + shop_item.label );

			WebObjects.Shop.BuyArguments args = new Roar.WebObjects.Shop.BuyArguments();
			args.shop_item_ikey = shop_item.ikey;

			shopActions.buy (args, new ShopBuyCallback (cb, this, shop_item.ikey));
		}

		protected class ShopBuyCallback : CBBase<WebObjects.Shop.BuyResponse>
		{
			string ikey;

			public ShopBuyCallback ( Roar.Callback<WebObjects.Shop.BuyResponse> in_cb, Shop in_shop, string in_ikey) : base(in_cb)
			{
				ikey = in_ikey;
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Shop.BuyResponse> info)
			{
				RoarManager.OnGoodBought (
					new RoarManager.PurchaseInfo (
						ikey,
						info.data.buy_response
					) );
			}
		}
	}

}
