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
	
	public string shopItemLabelStyle = "DefaultHeavyContentLeftText";
	public string shopItemDescriptionStyle = "LightTextRightAligned";
	public string shopItemCostStyle = "DefaultHeavyContentLeftText";
	public string shopItemBuyButtonStyle = "DefaultButton";

	public float buyButtonWidth =100;
	public float buyButtonHeight = 40;
	public float interColumnSeparators =20;
	public float divideHeight = 35;
	public float priceColumnWidth = 40;//Buy button always sticks to the right. description takes up the rest.

	public float topSeparation = 15;
	
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

	void Reset()
	{
		bounds = new Rect(0,100,512,386);
		displayName = "Shop";
	}

	public void Fetch()
	{
		networkActionInProgress = true;
		isFetching = true;
		shop.Fetch(OnRoarFetchShopComplete);
	}
	
	void CalculateScrollBounds()
	{
	}
	
	void OnRoarFetchShopComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.ShopEntry> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		networkActionInProgress = false;
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

		GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");

		GUI.Label(new Rect(interColumnSeparators, 0, priceColumnWidth, divideHeight), "ITEM", "DefaultSeparationBarText");

		GUI.Label(new Rect(contentBounds.width - interColumnSeparators*2 - buyButtonWidth - priceColumnWidth, 0, priceColumnWidth, divideHeight), "COST", "DefaultSeparationBarText");


		float heightSoFar = divideHeight;
		if(shopEntries != null)
		foreach (ShopEntry item in shopEntries)
		{
			Vector2 descSize = GUI.skin.FindStyle(shopItemDescriptionStyle).CalcSize(new GUIContent(item.description));
			Vector2 labSize = GUI.skin.FindStyle(shopItemLabelStyle).CalcSize(new GUIContent(item.label));
			float height =  descSize.y+ labSize.y + topSeparation;

			GUI.Box(new Rect(0, heightSoFar, contentBounds.width, height), new GUIContent(""), "DefaultHorizontalSection");
			float ySoFar = heightSoFar + topSeparation;

			GUI.Box(new Rect(interColumnSeparators, ySoFar, labSize.x, labSize.y), item.label, shopItemLabelStyle);
			ySoFar+= labSize.y;

			GUI.Box(new Rect(interColumnSeparators, ySoFar, descSize.x, descSize.y), item.description, shopItemDescriptionStyle);

			if (item.costs.Count == 1)
			{
				Roar.DomainObjects.Costs.Stat stat_cost = item.costs[0] as Roar.DomainObjects.Costs.Stat;
				if( stat_cost != null )
				{
					//TODO: This is not rendering in the right place.
					GUI.Label (new Rect(contentBounds.width - buyButtonWidth - priceColumnWidth - 2*interColumnSeparators, heightSoFar, priceColumnWidth, labSize.y),  stat_cost.value.ToString(), shopItemCostStyle) ;

					GUI.Label (new Rect(contentBounds.width - buyButtonWidth - priceColumnWidth - 2*interColumnSeparators, heightSoFar + labSize.y, priceColumnWidth, labSize.y),  stat_cost.ikey, shopItemDescriptionStyle) ;

				}
			}
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

			if (GUI.Button(new Rect(contentBounds.width - interColumnSeparators - buyButtonWidth, heightSoFar + (labSize.y+descSize.y + topSeparation)/2 - buyButtonHeight/2, buyButtonWidth, buyButtonHeight), "Buy", shopItemBuyButtonStyle))
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
				networkActionInProgress = true;
				shop.Buy( item.ikey, OnBuyComplete );
			}
			GUI.enabled = true;

			heightSoFar += labSize.y+descSize.y + topSeparation;
		}
	}
	
	protected void OnBuyComplete( CallbackInfo<Roar.WebObjects.Shop.BuyResponse> response )
	{
		networkActionInProgress = false;
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
