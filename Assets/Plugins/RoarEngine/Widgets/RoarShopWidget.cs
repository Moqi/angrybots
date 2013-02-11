using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar;
using Roar.DomainObjects;

public class RoarShopWidget : RoarUIWidget
{
	public delegate void RoarShopWidgetBuyHandler(Roar.DomainObjects.ShopEntry shop_entry);
	public static event RoarShopWidgetBuyHandler OnItemBuyRequest;
	
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	
	public string shopItemLabelStyle = "ShopItemLabel";
	public string shopItemDescriptionStyle = "ShopItemDescription";
	public string shopItemCostStyle = "ShopItemCost";
	public string shopItemBuyButtonStyle = "ShopItemBuyButton";
	public Rect shopItemBounds = new Rect(0, 0, 450, 90);
	public Rect buyButtonBounds= new Rect(200, 25, 100, 40);
	public float shopItemSpacing = 12;
	
	private bool isFetching;
	private bool isBuying = false;
	private float whenLastFetched;
	private Roar.Components.IShop shop;
	private IList<ShopEntry> shopEntries;
	
	private IList<string> errorMessages = new List<string>();
	
	protected override void OnEnable ()
	{
		if (IsLoggedIn)
		{
			base.OnEnable();
			shop = DefaultRoar.Instance.Shop;
			if (shop != null)
			{
				if (whenToFetch == WhenToFetch.OnEnable 
				|| (whenToFetch == WhenToFetch.Once && !shop.HasDataFromServer)
				|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
				)
				{
					Fetch();
				}
			}
			else if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Shop data is null; unable to render shop widget");
			}
		}
		else
		{
			enabled = false;
		}
	}

	public void Fetch()
	{
		isFetching = true;
		shop.Fetch(OnRoarFetchShopComplete);
	}
	
	void CalculateScrollBounds()
	{
		ScrollViewContentWidth = shopItemBounds.width;
		ScrollViewContentHeight = Mathf.Max(contentBounds.height, (shopEntries.Count + errorMessages.Count) * (shopItemBounds.height + shopItemSpacing));
	}
	
	void OnRoarFetchShopComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.ShopEntry> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		shopEntries = shop.List();
		errorMessages = new List<string>();
		
		if( info.code!=IWebAPI.OK)
		{
			Debug.Log ( string.Format("Error loading shop:{0}:{1}", info.code, info.msg) );
			FlashError( info.msg ); //This updates the scroll bounds.
			return;
		}

		CalculateScrollBounds();
	}
	
	protected override void DrawGUI(int windowId)
	{
		if (shop == null || !IsLoggedIn) return;
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching shop data...", "StatusNormal");
		}
		else
		{
			//TODO: Dont use same rect for errors and items
			Rect itemRect = shopItemBounds;

			foreach( string e in errorMessages )
			{
				GUI.Label ( itemRect, e );
				itemRect.y += itemRect.height + shopItemSpacing;
			}
			
			foreach (ShopEntry item in shopEntries)
			{
				GUI.Label(itemRect, item.label, shopItemLabelStyle);
				GUI.Label(itemRect, item.description, shopItemDescriptionStyle);
				//GUI.Label(itemRect, string.Format("{0} {1}", item.costs[0].amount.ToString(), RoarTypesCache.UserStatByKey(item.costs[0].key).Title), shopItemCostStyle);
				
				//Only render if theres exactly one cost and its a stat cost.
				if (item.costs.Count == 1)
				{
					Roar.DomainObjects.Costs.Stat stat_cost = item.costs[0] as Roar.DomainObjects.Costs.Stat;
					if( stat_cost != null )
					{
						//TODO: This is not rendering in the right place.
						GUI.Label (itemRect, string.Format ("Costs {0} {1}", stat_cost.value, stat_cost.ikey), shopItemCostStyle ) ;
					}
				}
				
				GUI.BeginGroup(itemRect);
				
				//For now only check the costs
				bool can_buy = true;
				foreach( Roar.DomainObjects.Cost cost in item.costs)
				{
					// If its a stat cost we should check against the current value of the players stat rather than trusting the 
					// cached value from the shop.
					Roar.DomainObjects.Costs.Stat stat_cost = cost as Roar.DomainObjects.Costs.Stat;
					if( stat_cost != null )
					{
						string v = roar.Properties.GetValue( stat_cost.ikey );
						int vv;
						//If we can't get the info we need, we'll need to rely on the value from the shop.
						if( v!=null && System.Int32.TryParse(v, out vv ) )
						{
							if( vv < stat_cost.value ) { can_buy = false; break; }
							continue;
						}
					}
					if( ! cost.ok ) { can_buy = false; break; }
				}

				GUI.enabled = can_buy && !isBuying;
				
				if (GUI.Button(buyButtonBounds, "Buy", shopItemBuyButtonStyle))
				{
					if (Debug.isDebugBuild)
					{
						Debug.Log(string.Format("buy request: {0}", item.ikey));
					}
					if (OnItemBuyRequest != null)
					{
						OnItemBuyRequest(item);
					}
					isBuying = true;
					shop.Buy( item.ikey, OnBuyComplete );
				}
				GUI.enabled = true;
				GUI.EndGroup();
				
				itemRect.y += itemRect.height + shopItemSpacing;
			}
		}
	}
	
	protected void OnBuyComplete( CallbackInfo<Roar.WebObjects.Shop.BuyResponse> response )
	{
		isBuying = false;
		if( response.code!=WebAPI.OK )
		{
			FlashError( response.msg );
		}
	}
	
	protected void FlashError( string mesg )
	{
		errorMessages.Add( mesg );
		CalculateScrollBounds();
	}
}
