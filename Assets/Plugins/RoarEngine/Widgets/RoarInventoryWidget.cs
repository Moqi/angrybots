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
	
	public string labelFormat = "InventoryLabel";
	public string descriptionFormat = "InventoryDescription";
	public string typeFormat = "InventoryType";
	public string consumeButtonFormat = "InventoryConsumeButton";
	
	public bool showDescription = true;
	public bool showType = true;
	public bool allowSorting = true;
	
	public int maxLabelWidth = 150;
	public int maxDescriptionFormatWidth = 350;
	public int maxTypeWidth = 100;
	public int rowHeight = 32;
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
		Rect rect = new Rect(0,0,ContentWidth, 32);
		GUI.Label ( rect, string.Format("Contains {0} items", items.Count) );
		rect.y += rowHeight;
		
		Vector2 lastLabelSize;
		lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( "Label"));
		if(maxLabelWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxLabelWidth;
		GUI.Label ( rect, "Label", labelFormat);
		
		rect.x += rect.width + 5;
		
		lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent("Description"));
		if(maxDescriptionFormatWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxDescriptionFormatWidth;
			
		GUI.Label ( rect, "Description", descriptionFormat);
		rect.x += rect.width+ 5;
		
		lastLabelSize =GUI.skin.FindStyle(typeFormat).CalcSize(new GUIContent("Type"));
		if(maxTypeWidth == 0)
			rect.width = lastLabelSize.x;
		else
			rect.width = maxTypeWidth;
			
		GUI.Label ( rect, "Type", typeFormat);
		rect.x += rect.width+ 5;
		
		lastLabelSize =GUI.skin.FindStyle(consumeButtonFormat).CalcSize(new GUIContent("Consume"));
		rect.width = lastLabelSize.x;
		
		rect.x = 0;
		rect.y += rowHeight;
		
		foreach( Roar.DomainObjects.InventoryItem item in items )
		{
			
			lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( item.label));
			if(maxLabelWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxLabelWidth;
			GUI.Label ( rect, item.label, labelFormat);
			
			rect.x += rect.width + 5;
			
			lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent(item.description));
			if(maxDescriptionFormatWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxDescriptionFormatWidth;
				
			GUI.Label ( rect, item.description, descriptionFormat);
			rect.x += rect.width+ 5;
			
			lastLabelSize =GUI.skin.FindStyle(typeFormat).CalcSize(new GUIContent(item.type));
			if(maxTypeWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxTypeWidth;
				
			GUI.Label ( rect, item.type, typeFormat);
			rect.x += rect.width+ 5;
			
			lastLabelSize =GUI.skin.FindStyle(consumeButtonFormat).CalcSize(new GUIContent("Consume"));
			rect.width = lastLabelSize.x;
			
			if(item.consumable)
			{
				if(GUI.Button(rect, "Consume", consumeButtonFormat))
				{
					
				}
				
			}
			rect.x = 0;
			rect.y += rowHeight;
		}
	}
	
	public void Fetch()
	{
		isFetching = true;
		inventory.Fetch(OnRoarFetchInventoryComplete);
	}
	
	void OnRoarFetchInventoryComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.InventoryItem> > data )
	{
		isFetching = false;
	}
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
