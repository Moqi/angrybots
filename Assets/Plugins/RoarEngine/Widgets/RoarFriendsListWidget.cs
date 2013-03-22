using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarFriendsListWidget : RoarUIWidget
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	
	private bool isFetching;
	private float whenLastFetched;
	private Roar.Components.IFriends friends;
	
	public float entrySpacing;
	public Rect  entryBounds = new Rect(0, 0, 350, 25);
	
	public string playerIdFormatString ="{0}";
	public string playerIdFormat = "FriendsId";
	public string nameFormatString = "{0}";
	public string nameFormat = "FriendsName";
	public string levelFormatString = "{0}";
	public string levelFormat = "FriendsLevel";
	
	IDictionary<string, Roar.DomainObjects.Friend> friendsDict = null;
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		friends = DefaultRoar.Instance.Friends;
		if (friends != null)
		{
			if (whenToFetch == WhenToFetch.OnEnable 
			|| (whenToFetch == WhenToFetch.Once && !friends.HasDataFromServer)
			|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
			)
			{
				Fetch();
			}
		}
		else if (Debug.isDebugBuild)
		{
			Debug.LogWarning("Friends data is null; unable to render friends list widget");
		}
	}
	
	public void Fetch()
	{
		networkActionInProgress = true;

		isFetching = true;
		friends.Fetch(OnRoarFetchFriendsComplete);
	}
	
	void OnRoarFetchFriendsComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Friend> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		networkActionInProgress = false;
		isFetching = false;
		friendsDict = info.data;
	}
	
	protected override void DrawGUI(int windowId)
	{
		if (!IsLoggedIn) return; // attempt draw nothing if not logged in
		if (isFetching)
		{
			GUI.Label(new Rect(this.bounds.width/2f - 256,this.bounds.height/2f - 32,512,64), "Fetching friends...", "StatusNormal");
			return;
		}
		
		if( friends==null)
		{
			GUI.Label(new Rect(this.bounds.width/2f - 256,this.bounds.height/2f - 32,512,64), "Error loading friends...", "StatusError");
			return;
		}
		
		ScrollViewContentHeight = Mathf.Max (contentBounds.height, friendsDict.Count * (entryBounds.height + entrySpacing));
		Rect entry = entryBounds;
		foreach (KeyValuePair<string,Roar.DomainObjects.Friend> f in friendsDict)
		{
			GUI.Label(entry, string.Format( playerIdFormatString, f.Value.player_id ), playerIdFormat );
			GUI.Label(entry, string.Format( nameFormatString, f.Value.name ), nameFormat );
			GUI.Label(entry, string.Format( levelFormatString, f.Value.level ), levelFormat );
			entry.y += entry.height + entrySpacing;
		}
	}
	
}
