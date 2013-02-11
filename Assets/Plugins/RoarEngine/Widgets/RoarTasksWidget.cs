using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//TODO: Should this be called RoarActionsWidget?

public class RoarTasksWidget : RoarUIWidget
{
	
	protected Roar.Components.ITasks tasks;
	protected bool isFetching=false;
	
	protected override void OnEnable ()
	{
		if (!IsLoggedIn)
		{
			Debug.Log ("RoarTasksWidget enabled before login - unable to fetch data");
			return;
		}
		
		tasks = DefaultRoar.Instance.Tasks;		
		Fetch();
	}
	
		
	protected override void DrawGUI(int windowId)
	{
		if (tasks == null || !IsLoggedIn) return;
		
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching tasks data...", "StatusNormal");
			return;
		}
		
		if( ! tasks.HasDataFromServer )
		{
			GUI.Label (new Rect(0,0,ContentWidth,ContentHeight), "Inventory data not loaded!", "StatusNormal");
			return;
		}
		

		IList<Roar.DomainObjects.Task> items = tasks.List();

		//TODO: Fixup the hardcoded dimensions here!
		Rect rect = new Rect(0,0,ContentWidth, 32);
		GUI.Label ( rect, string.Format("Contains {0} items", items.Count) );
		rect.y += 32;

		foreach( Roar.DomainObjects.Task item in items )
		{
			GUI.Label ( rect, item.label + " : " + item.description);
			rect.y += 32;
		}
	}
	
	public void Fetch()
	{
		isFetching = true;
		tasks.Fetch(OnRoarFetchTasksComplete);
	}
	
	void OnRoarFetchTasksComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Task> > data )
	{
		isFetching = false;
	}

}
