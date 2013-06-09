using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar;
using Roar.DomainObjects;

public class RoarFacebookShopWidget : RoarUIWidget
{
	public delegate void RoarShopWidgetBuyHandler(Roar.DomainObjects.ShopEntry shop_entry);

	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;

	public string shopItemLabelStyle = "DefaultHeavyContentText";
	public string shopItemDescriptionStyle = "DefaultLightContentText";
	public string shopItemCostStyle = "DefaultHeavyContentText";
	public string shopItemBuyButtonStyle = "DefaultButton";

	public float buyButtonWidth =100;
	public float buyButtonHeight = 40;
	public float interColumnSeparators =5;
	public float divideHeight = 20;
	public float priceColumnWidth = 40;//Buy button always sticks to the right. description takes up the rest.

	public float topSeparation = 15;

	private bool isFetching;
	private bool isBuying = false;
	private float whenLastFetched;
	private Roar.Components.IFacebook facebook;
	private IList<FacebookShopEntry> shopEntries;

	private IList<string> errorMessages = new List<string>();

	protected override void OnEnable ()
	{
		if (IsLoggedIn)
		{
			base.OnEnable();
			facebook = DefaultRoar.Instance.Facebook;
			if (facebook != null)
			{
				if (whenToFetch == WhenToFetch.OnEnable
				|| (whenToFetch == WhenToFetch.Once && !facebook.IsLoggedInViaFacebook())
				|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
				)
				{
					Fetch();
				}
			}
			else if (Debug.isDebugBuild)
			{
				Debug.LogWarning("Cant load shop data");
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
		facebook.FetchShopData(OnRoarFetchShopComplete);
	}

	void CalculateScrollBounds()
	{
		ScrollViewContentWidth = contentBounds.width;
	}

	void OnRoarFetchShopComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.FacebookShopEntry> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		isFetching = false;
		shopEntries = facebook.List();
		errorMessages = new List<string>();

		if( info.code!=IWebAPI.OK)
		{
			Debug.Log ( string.Format("Error loading shop:{0}:{1}", info.code, info.msg));
			FlashError( info.msg ); //This updates the scroll bounds.
			return;
		}

		CalculateScrollBounds();
	}

	protected override void DrawGUI(int windowId)
	{
		if (facebook == null || !IsLoggedIn) return;
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching shop data...", "StatusNormal");
		}
		GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");

		GUI.Label(new Rect(interColumnSeparators, 0, priceColumnWidth, divideHeight), "ITEM", "DefaultSeparationBarText");

		GUI.Label(new Rect(contentBounds.width - interColumnSeparators*2 - buyButtonWidth - priceColumnWidth, 0, priceColumnWidth, divideHeight), "COST", "DefaultSeparationBarText");

		float heightSoFar = divideHeight;
		if(shopEntries != null)
		foreach (FacebookShopEntry item in shopEntries)
		{
			Vector2 descSize = GUI.skin.FindStyle(shopItemDescriptionStyle).CalcSize(new GUIContent(item.description));
			Vector2 labSize = GUI.skin.FindStyle(shopItemLabelStyle).CalcSize(new GUIContent(item.label));
			float height =  descSize.y+ labSize.y + topSeparation;


			GUI.Box(new Rect(0, heightSoFar, contentBounds.width, height), new GUIContent(""), "DefaultHorizontalSection");
			float ySoFar = heightSoFar + topSeparation;

			GUI.Box(new Rect(interColumnSeparators, ySoFar, labSize.x, labSize.y), item.label, shopItemLabelStyle);
			ySoFar+= labSize.y;

			GUI.Box(new Rect(interColumnSeparators, ySoFar, descSize.x, descSize.y), item.description, shopItemDescriptionStyle);

			//TODO: This is not rendering in the right place.
			GUI.Label (new Rect(contentBounds.width - buyButtonWidth - priceColumnWidth - 2*interColumnSeparators, heightSoFar, priceColumnWidth, height),  item.price, shopItemCostStyle) ;

			//GUI.Label (new Rect(contentBounds.width - buyButtonWidth - priceColumnWidth - 2*interColumnSeparators, heightSoFar + labSize.y, priceColumnWidth, labSize.y),  stat_cost.ikey, shopItemDescriptionStyle) ;
			//For now only check the costs
			bool can_buy = true;


			GUI.enabled = can_buy && !isBuying;

			if (GUI.Button(new Rect(contentBounds.width - interColumnSeparators - buyButtonWidth, heightSoFar + (labSize.y+descSize.y + topSeparation)/2 - buyButtonHeight/2, buyButtonWidth, buyButtonHeight), "Buy", shopItemBuyButtonStyle))
			{
				if (Debug.isDebugBuild)
				{
					Debug.Log(string.Format("buy request: {0}", item.ikey));
				}
				Application.ExternalCall("buySomething", item.ikey);
				isBuying = true;

			}
			GUI.enabled = true;

			heightSoFar += labSize.y+descSize.y + topSeparation;
		}
		if(alwaysShowVerticalScrollBar)
			if(heightSoFar < contentBounds.height)
				ScrollViewContentHeight = contentBounds.height;
			else
				ScrollViewContentHeight = heightSoFar;
		else
			ScrollViewContentHeight = heightSoFar;
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
