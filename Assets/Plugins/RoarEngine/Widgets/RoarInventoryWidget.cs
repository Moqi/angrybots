using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarInventoryWidget : RoarUIWidget
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	
	protected Roar.Components.IInventory inventory;
	protected bool isFetching=false;
	
	public string labelFormat = "DefaultHeavyContentText";
	public string descriptionFormat = "DefaultLightContentText";
	public string typeFormat = "DefaultHeavyContentLeftText";
	public string consumeButtonFormat = "DefaultButton";
	public float consumeButtonWidth = 100;
	
	public bool showDescription = true;
	public bool showType = true;
	public bool allowSorting = true;
	
	public int maxLabelWidth = 58;
	public int maxDescriptionFormatWidth = 310;
	public int maxTypeWidth = 40;
	public int rowHeight = 32;
	public int divideHeight = 50;
	public int margin = 5;

	protected override void OnEnable ()
	{
		inventory = roar.Inventory;
		if (IsLoggedIn)
		{
			
			Fetch();
			
		}
		else
		{
			enabled = false;
		}
		
		
	}
	
	void Reset()
	{
		displayName = "Inventory";
		bounds = new Rect(0,0,512,386);
	}

	protected override void DrawGUI(int windowId)
	{
		if (inventory == null || !IsLoggedIn) return;
		
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching inventory data...", "StatusNormal");
			return;
		}
		
		if( ! inventory.HasDataFromServer )
		{
			GUI.Label (new Rect(0,0,ContentWidth,ContentHeight), "Inventory data not loaded!", "StatusNormal");
			return;
		}
		
		IList<Roar.DomainObjects.InventoryItem> items = inventory.List();

		//TODO: Fixup the hardcoded dimensions here!
		Rect rect = new Rect(margin,0,ContentWidth, 32);
		GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
		
		Vector2 lastLabelSize;
		lastLabelSize = GUI.skin.FindStyle("DefaultSeparationBarText").CalcSize(new GUIContent( "Label"));
		if(maxLabelWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxLabelWidth;
		GUI.Label ( rect, "LABEL", "DefaultSeparationBarText");
		
		rect.x += rect.width + margin;
		
		lastLabelSize =GUI.skin.FindStyle("DefaultSeparationBarText").CalcSize(new GUIContent("Description"));
		if(maxDescriptionFormatWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxDescriptionFormatWidth;
			
		GUI.Label ( rect, "DESCRIPTION", "DefaultSeparationBarText");
		rect.x += rect.width+ margin;
		
		lastLabelSize =GUI.skin.FindStyle("DefaultSeparationBarText").CalcSize(new GUIContent("Type"));
		if(maxTypeWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxTypeWidth;
			
		GUI.Label ( rect, "TYPE", "DefaultSeparationBarText");
		rect.x += rect.width+ margin;
		
		lastLabelSize =GUI.skin.FindStyle(consumeButtonFormat).CalcSize(new GUIContent("Consume"));
		rect.width += lastLabelSize.x;
		rect.width = lastLabelSize.x;
		
		rect.x = margin;
		rect.y += rowHeight;
		
		foreach( Roar.DomainObjects.InventoryItem item in items )
		{
			
			lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( item.label));
			if(maxLabelWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxLabelWidth;
			GUI.Label ( rect, item.label, labelFormat);
			
			rect.x += rect.width + margin;
			
			lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent(item.description));
			if(maxDescriptionFormatWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxDescriptionFormatWidth;
				
			GUI.Label ( rect, item.description, descriptionFormat);
			rect.x += rect.width+ margin;
			
			lastLabelSize =GUI.skin.FindStyle(typeFormat).CalcSize(new GUIContent(item.type));
			if(maxTypeWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxTypeWidth;
				
			GUI.Label ( rect, item.type, typeFormat);
			rect.x += rect.width+ margin;
			rect.width = consumeButtonWidth;

			if(item.consumable && GUI.Button(rect, "Consume", consumeButtonFormat))
			{
				inventory.Use(item.id, OnRoarInventoryConsumeComplete);
			}
			
			rect.x = margin;
			rect.y += rowHeight;
		}
	}
	
	public void Fetch()
	{
		isFetching = true;
		networkActionInProgress = true;
		inventory.Fetch(OnRoarFetchInventoryComplete);
	}
	
	void OnRoarFetchInventoryComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.InventoryItem> > data )
	{
		networkActionInProgress = false;
		isFetching = false;
	}
	
	void OnRoarInventoryConsumeComplete(Roar.CallbackInfo<Roar.WebObjects.Items.UseResponse> data)
	{
		networkActionInProgress = false;
		if(data.code == WebAPI.OK)
		{
			Fetch();
		}
	}
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
