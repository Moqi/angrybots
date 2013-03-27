using System;
using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using UnityEngine;
using System.Runtime.InteropServices;
using Roar.DomainObjects;

namespace Roar.implementation.Components
{
	public class InAppPurchase : IInAppPurchase
	{
		protected ILogger logger;
		protected IDataStore dataStore;
		protected IWebAPI.IAppstoreActions actions;
		protected bool isSandbox;
		protected bool hasDataFromAppstore;
		protected bool isServerCalling;
		protected IDictionary<string, AppstoreShopEntry> productsMap;
		protected IList<AppstoreShopEntry> productsList;
		
		//TODO: Ugly that we need two of these for now.
		protected Roar.Callback<string> purchaseCallback;
		protected Roar.Callback<Roar.WebObjects.Appstore.BuyResponse> purchaseCallbackX;

		public InAppPurchase (IWebAPI.IAppstoreActions actions, string nativeCallbackGameObject, ILogger logger, bool isSandbox)
		{
			this.logger = logger;
			this.actions = actions;
			this.isSandbox = isSandbox;
			hasDataFromAppstore = false;
			isServerCalling = false;
			productsMap = new Dictionary<string, AppstoreShopEntry> ();
			productsList = new List<AppstoreShopEntry> ();
    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitInit(nativeCallbackGameObject);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitInit({0}) from Unity Editor", nativeCallbackGameObject));
    		#endif
		}

		/**
	     * Fetch has two stages:
	     * 1. Retrieve the appstore product ids from roar.
	     * 2. Use the product ids to retrieve the product details from the appstore.
	     **/
		public void Fetch (Roar.Callback<WebObjects.Appstore.ShopListResponse> callback)
		{
			if (isServerCalling) {
				return;
			}
			isServerCalling = false;
			productsMap.Clear ();
			productsList.Clear ();
			actions.shop_list ( new Roar.WebObjects.Appstore.ShopListArguments(), new AppstoreListCallback (callback, this));
		}

		class AppstoreListCallback : ZWebAPI.Callback<WebObjects.Appstore.ShopListResponse>
		{
			InAppPurchase appstore;
			Roar.Callback<WebObjects.Appstore.ShopListResponse> cb_;

			public AppstoreListCallback (Roar.Callback<WebObjects.Appstore.ShopListResponse> in_cb, InAppPurchase in_appstore)
			{
				appstore = in_appstore;
				cb_ = in_cb;
			}

			public void OnSuccess (Roar.CallbackInfo<WebObjects.Appstore.ShopListResponse> info)
			{
				appstore.isServerCalling = false;
				appstore.logger.DebugLog (string.Format ("onAppstoreList.onSuccess() called with: {0}", info.data.ToString()));
				string combinedProductIdentifiers = string.Join (",", (string[])info.data.shop_list.ConvertAll<string>(e => e.product_identifier).ToArray());
#if UNITY_IOS && !UNITY_EDITOR
				_StoreKitRequestProductData(combinedProductIdentifiers);
#else
				appstore.logger.DebugLog (string.Format ("Can't call _StoreKitRequestProductData({0}) from Unity Editor", combinedProductIdentifiers));
#endif
				if (cb_ != null)
				{
					cb_(info);
				}
			}
			
			public void OnError( RequestResult info )
			{
				appstore.isServerCalling = false;
				if (cb_ != null)
				{
					cb_( new CallbackInfo<WebObjects.Appstore.ShopListResponse>( null, info.code, info.msg) );
				}

			}
		}

		public IList<AppstoreShopEntry> List ()
		{
			return  productsList;
		}

		public AppstoreShopEntry GetShopItem (string productIdentifier)
		{
			return productsMap [productIdentifier];
		}



		public bool HasDataFromServer { get { return hasDataFromAppstore; } }

		protected void ValidateReceipt (string receiptId, Roar.Callback<WebObjects.Appstore.BuyResponse> callback)
		{


			
			WebObjects.Appstore.BuyArguments args = new Roar.WebObjects.Appstore.BuyArguments();
			args.receipt = receiptId;
			args.sandbox = isSandbox;

			actions.buy (args, new OnReceiptValidation (this));
		}

		class OnReceiptValidation : ZWebAPI.Callback<WebObjects.Appstore.BuyResponse>
		{
			InAppPurchase appstore;

			public OnReceiptValidation (InAppPurchase in_appstore)
			{
				appstore = in_appstore;
			}

			public void OnSuccess (CallbackInfo<WebObjects.Appstore.BuyResponse> info)
			{
				appstore.logger.DebugLog (string.Format ("onReceiptValidation() called with: {0}", info.data.ToString()));
			}
			
			public void OnError (RequestResult info)
			{
				appstore.logger.DebugLog (string.Format ("onReceiptValidation() called with: {0}", info.data.DebugAsString ()));
			}
		}


		//TODO: Ugly that we need two of these
		public void Purchase (string productId, Roar.Callback<string> cb, Roar.Callback<Roar.WebObjects.Appstore.BuyResponse> cbx)
		{
			purchaseCallback = cb;
			purchaseCallbackX = cbx;
    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitPurchase(productId);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
   			#endif
		}

		public void Purchase (string productId, int quantity, Roar.Callback<string> cb, Roar.Callback<Roar.WebObjects.Appstore.BuyResponse> cbx)
		{
			purchaseCallback = cb;
			purchaseCallbackX = cbx;

    		#if UNITY_IOS && !UNITY_EDITOR
      		_StoreKitPurchaseQuantity(productId, quantity);
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchase({0}) from Unity Editor", productId));
    		#endif
		}

		public bool PurchasesEnabled ()
		{
    		#if UNITY_IOS && !UNITY_EDITOR
      		return _StoreKitPurchasesEnabled();
    		#else
			logger.DebugLog (string.Format ("Can't call _StoreKitPurchasesEnabled() from Unity Editor"));
			return false;
    		#endif
		}
		
		public Roar.DataConversion.IXCRMParser ixcrm_parser = new Roar.DataConversion.XCRMParser();

		// Store Kit wrapper to roar Unity client communication methods

		public void OnProductData (string productDataXml)
		{

			hasDataFromAppstore = true;
			productsList.Clear ();
			productsMap.Clear ();
			logger.DebugLog (string.Format ("OnProductData() called with: {0}", productDataXml));
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			doc.LoadXml(productDataXml);

			System.Xml.XmlNode appstoreNode = doc.FirstChild;

			if ( ! appstoreNode.HasChildNodes )
			{
				logger.DebugLog ("No products passed to OnProductData()");
				return;
			}
			
			foreach ( System.Xml.XmlNode shopItemXml in appstoreNode )
			{
				if( shopItemXml.NodeType != System.Xml.XmlNodeType.Element ) continue;
				AppstoreShopEntry product = AppstoreShopEntry.CreateFromXml( shopItemXml, ixcrm_parser );
				productsList.Add (product);
				productsMap.Add (product.product_identifier, product);
			}
		}

		public void OnInvalidProductId (string invalidProductId)
		{
			logger.DebugLog (string.Format ("OnInvalidProductId() called with: {0}", invalidProductId));
		}

		public void OnPurchaseComplete (string purchaseXml)
		{
			logger.DebugLog (string.Format ("OnPurchaseComplete() called with: {0}", purchaseXml));
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			doc.LoadXml(purchaseXml);

			System.Xml.XmlElement root = doc.FirstChild as System.Xml.XmlElement;
			System.Xml.XmlElement purchaseNode = root.SelectSingleNode("./shop_item_purchase_success") as System.Xml.XmlElement;
			string transactionIdentifier = purchaseNode.GetAttribute ("transaction_identifier");
			ValidateReceipt (transactionIdentifier, purchaseCallbackX);
		}

		public void OnPurchaseCancelled (string productIdentifier)
		{
			logger.DebugLog (string.Format ("OnPurchaseCancelled() called with: {0}", productIdentifier));
			if (purchaseCallback != null) {
				purchaseCallback (new CallbackInfo<string> (productIdentifier, IWebAPI.DISALLOWED, "Purchase cancelled by user"));
			}
		}

		public void OnPurchaseFailed (string errorXml)
		{
			logger.DebugLog (string.Format ("OnPurchaseFailed() called with: {0}", errorXml));
			if (purchaseCallback != null) {
				purchaseCallback (new CallbackInfo<string> (errorXml, IWebAPI.UNKNOWN_ERR, "Purchase failed"));
			}
		}

		// roar Unity client to Store Kit wrapper communication methods

    #region DllImports
    #if UNITY_IOS
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitInit(string unityGameObject);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern bool _StoreKitPurchasesEnabled();
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitRequestProductData(string productIdentifiers);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitPurchase(string productIdentifier);
        [DllImport ("__Internal", CharSet = CharSet.Auto)]
        static extern void _StoreKitPurchaseQuantity(string productIdentifier, int quantity);
    #endif
    #endregion
	}
}
