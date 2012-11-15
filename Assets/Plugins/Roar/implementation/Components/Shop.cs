using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{
	public class Shop : IShop
	{
		protected IWebAPI.IShopActions shopActions;
		protected DataStore dataStore;
		protected ILogger logger;

		public Shop (IWebAPI.IShopActions shopActions, DataStore dataStore, ILogger logger)
		{
			this.shopActions = shopActions;
			this.dataStore = dataStore;
			this.logger = logger;

			RoarManager.shopReadyEvent += () => CacheFromShop ();
		}

		public void Fetch (Roar.Callback callback)
		{
			dataStore.shop.Fetch (callback);
		}

		public bool HasDataFromServer { get { return dataStore.shop.HasDataFromServer; } }

		public void Buy (string shop_ikey, Roar.Callback callback)
		{
			ShopBuy (shop_ikey, callback);
		}

		public IList<DomainObjects.ShopEntry> List ()
		{
			return dataStore.shop.List();
		}

		public IList<DomainObjects.ShopEntry> List (Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<IList<DomainObjects.ShopEntry> > (dataStore.shop.List ()));
			return dataStore.shop.List();
		}

		// Returns the *object* associated with attribute `key`
		public DomainObjects.ShopEntry GetShopItem (string ikey)
		{
			return GetShopItem(ikey, null);
		}

		public DomainObjects.ShopEntry GetShopItem (string ikey, Roar.Callback callback)
		{
			if (callback != null)
				callback (new Roar.CallbackInfo<DomainObjects.ShopEntry> (dataStore.shop.Get (ikey)));
			return dataStore.shop.Get (ikey);
		}

		public void ShopBuy (string shop_ikey, Roar.Callback cb)
		{
			var shop_item = dataStore.shop.Get (shop_ikey);

			// Make the call if the item is in the shop
			if (shop_item == null) {
				logger.DebugLog ("[roar] -- Cannot find to purchase: " + shop_ikey);
				return;
			}
			logger.DebugLog ("trying to buy me a : " + shop_item.as_json() );

			Hashtable args = new Hashtable ();
			args ["shop_item_ikey"] = shop_item.ikey;

			shopActions.buy (args, new ShopBuyCallback (cb, this, shop_item.ikey));
		}

		protected class ShopBuyCallback : SimpleRequestCallback<IXMLNode>
		{
			//Shop shop;
			string ikey;

			public ShopBuyCallback (Roar.Callback in_cb, Shop in_shop, string in_ikey) : base(in_cb)
			{
				//shop = in_shop;
				ikey = in_ikey;
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				IXMLNode result = info.data.GetNode ("roar>0>shop>0>buy>0");

				// Obtain the server id for the purchased item
				// NOTE: Assumes ONLY ONE item per "shopitem" entity

				IXMLNode cost = result.GetNode ("costs>0>cost>0");
				IXMLNode item = result.GetNode ("modifiers>0>modifier>0");


				string id = item.GetAttribute ("item_id");

				RoarManager.OnGoodBought (
					new RoarManager.PurchaseInfo (
						cost.GetAttribute ("ikey"), //currency_name
						int.Parse (cost.GetAttribute ("value")), // item_price
						ikey, // iitem_id
						1                                      //item_qty
				));

				Hashtable data = new Hashtable ();
				data ["id"] = id;
				data ["ikey"] = ikey;

				return data;
			}
		}

		// Builds a list of items to fetch from Server by comparing
		// what's in the Shop list and what's currently in the cache
		public bool CacheFromShop ()
		{
			if (dataStore.shop.HasDataFromServer) {
				// Build sanitised ARRAY of ikeys from Shop.list()
				IList<DomainObjects.ShopEntry> l = dataStore.shop.List ();
				var ikeyList = new ArrayList ();

				foreach (DomainObjects.ShopEntry v in l) {
					// TODO: This is a new-style fudge that should be undone.
					ikeyList.Add (v.ikey);
				}

				return dataStore.cache.AddToCache (ikeyList);
			} else
				return false;
		}



	}

}
