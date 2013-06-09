using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarPlayerSelectionWidget : RoarUIWidget
{
	public float buttonWidth = 100;
	public float buttonHeight = 60;
	public float interSeparation = 5;
	public float leftSeparation = 30;
	public float sectionHeight = 40;
	public bool displayPlayersFromFriends = true;
	public bool displayPlayersFromFriendsInvites = true;
	public bool displayPlayersFromLeaderboards = true;
	public bool displayExternalPlayers = true;
	
	public System.Action<string> onIDSelected;
	
	private Dictionary<string, string> externalPlayers = new Dictionary<string, string>();
	
	public void setCallbackAndEnable(System.Action<string> s)
	{
		onIDSelected = s;
		enabled = true;
		Debug.Log("Enabled and Going");
	}
	
	protected override void Awake()
	{
		PlayerDictionary.Init();
		base.Awake();
		externalPlayers.Add("123", "sonda");
	}
	
	protected override void OnEnable ()
	{
		PlayerDictionary.UpdateFriendsDictionary();
	}

	protected override void DrawGUI(int windowId)
	{
		Rect currentRect = new Rect(leftSeparation, interSeparation, contentBounds.width - 2*leftSeparation, sectionHeight);
		
		if(displayPlayersFromFriends)
		{
			foreach(string key in PlayerDictionary.playerIDFriendDict.Keys)
			{
				currentRect.y += sectionHeight + interSeparation;
				if(GUI.Button(currentRect, PlayerDictionary.playerIDFriendDict[key].name, "DefaultButton"))
				{
					onIDSelected(key);
					enabled = false;
				}
			}
		}
		
		if(displayPlayersFromFriendsInvites)
		{
			foreach(string key in PlayerDictionary.playerIDFriendInviteDict.Keys)
			{
				currentRect.y += sectionHeight + interSeparation;
				if(GUI.Button(currentRect, PlayerDictionary.playerIDFriendInviteDict[key], "DefaultButton"))
				{
					onIDSelected(key);
					enabled = false;
				}
			}
		}
		
		if(displayPlayersFromLeaderboards)
		{
			foreach(string key in PlayerDictionary.playerIDleaderboardDict.Keys)
			{
				currentRect.y += sectionHeight + interSeparation;
				if(GUI.Button(currentRect, PlayerDictionary.playerIDleaderboardDict[key][0].value, "DefaultButton"))
				{
					onIDSelected(key);
					enabled = false;
				}
			}
		}
		if(displayExternalPlayers)
		{
			foreach(string key in externalPlayers.Keys)
			{
				currentRect.y += sectionHeight + interSeparation;
				if(GUI.Button(currentRect, externalPlayers[key], "DefaultButton"))
				{
					onIDSelected(key);
					enabled = false;
				}
			}
		}
		
		if(alwaysShowVerticalScrollBar)
			if(currentRect.y < contentBounds.height)
				ScrollViewContentHeight = contentBounds.height;
			else
				ScrollViewContentHeight = currentRect.y;
		else
			ScrollViewContentHeight = currentRect.y;
		
	}

}
