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
	
	private Dictionary<string, string> externalPlayers = new Dictionary<string, string>();
	
	public float entrySpacing;
	public Rect  entryBounds = new Rect(0, 0, 350, 25);

	public string playerIdFormatString ="{0}";
	public string playerIdFormat = "DefaultHeavyContentLeftText";
	public string nameFormatString = "{0}";
	public string nameFormat = "DefaultHeavyContentLeftText";
	public string levelFormatString = "{0}";
	public string levelFormat = "DefaultHeavyContentLeftText";

	public float topSpacing = 5;
	public float sectionHeight = 45;
	public float playerIDColumnWidth = 200;
	public float messageColumnWidth = 350;
	public float playerNameColumnWidth= 150;
	public float levelColumnWidth = 80;
	public float interColumnSeprators = 10;
	public float divideHeight = 25;
	public float buttonWidth = 140;
	public float buttonHeight = 40;
	public float footerSpacing = 90;
	public float messageBoxWidth = 150;
	public float messageBoxHeight = 60;
	public float playerIdBoxWidth = 150;
	public float textBoxHeight = 30;
	public float labelWidth = 130;
	public float labelHeight = 30;
	public float verticalSeparators = 5;
	public float selectButtonWidth = 20;
	public string statusString="";
	public string buttonFormat = "DefaultButton";

	Roar.DomainObjects.FriendInvite inviteManipulated;

	IDictionary<string, Roar.DomainObjects.Friend> friendsDict = null;
	IList<Roar.DomainObjects.FriendInvite> friendsInviteList = null;

	enum Section
	{
		Normal,
		AcceptInvites,
		SendingInvite,
	}

	Section currentSection;
	string playerIdToSendTo = "";
	string messageToSend= "";
	
	public void insertExternalPlayer (string id, string name) {
		externalPlayers.Add(id, name);
	}

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

	public void FetchAcceptable()
	{
		networkActionInProgress = true;
		friends.ListFriendInvites(OnRoarFetchListInvitesComplete);
	}

	void OnRoarFriendsRemove(Roar.CallbackInfo<Roar.WebObjects.Friends.RemoveResponse> data)
	{
		if(data.code == WebAPI.OK)
		{
			networkActionInProgress = false;
			statusString = "Friend removed successfully!";
			Fetch();
		}
		else
		{
			networkActionInProgress = false;
			statusString = "Friend remove failed. "+data.msg;
		}
	}

	void OnRoarFriendsAccept(Roar.CallbackInfo<Roar.WebObjects.Friends.AcceptResponse> data)
	{
		if(data.code == WebAPI.OK)
		{
			friendsInviteList.Remove(inviteManipulated);
			networkActionInProgress = false;
			statusString = "Invite accepted successfully!";
		}
		else
		{
			networkActionInProgress = false;
			statusString = "Invite accepting failed. "+data.msg;
		}
	}

	void OnRoarFriendsSendInvite(Roar.CallbackInfo<Roar.WebObjects.Friends.InviteResponse> data)
	{
		if(data.code == WebAPI.OK)
		{
			networkActionInProgress = false;
			statusString = "Invite sent successfully!";
			messageToSend = "";
			currentSection = Section.Normal;
		}
		else
		{
			networkActionInProgress = false;
			statusString = "Invite sending failed. "+data.msg;
		}
	}

	void OnRoarFriendsDeclineInvite(Roar.CallbackInfo<Roar.WebObjects.Friends.DeclineResponse> data)
	{
		if(data.code == WebAPI.OK)
		{
			friendsInviteList.Remove(inviteManipulated);
			networkActionInProgress = false;
			statusString = "Invite sent successfully!";
		}
		else
		{
			networkActionInProgress = false;
			statusString = "Invite sending failed. "+data.msg;
		}
	}

	void OnRoarFetchFriendsComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Friend> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		networkActionInProgress = false;
		isFetching = false;
		friendsDict = info.data;
	}

	void OnRoarFetchListInvitesComplete(Roar.CallbackInfo< Roar.WebObjects.Friends.ListInvitesResponse> info)
	{
		if(info.code == WebAPI.OK)
		{
			whenLastFetched = Time.realtimeSinceStartup;
			networkActionInProgress = false;
			isFetching = false;
			friendsInviteList = info.data.invites;
		}
		else
		{
			networkActionInProgress = false;
			isFetching = false;
			statusString = "Fetch failed. "+info.msg;
		}
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
		float heightSoFar = 0;
		Rect currentRect = new Rect(interColumnSeprators, heightSoFar, playerIDColumnWidth, divideHeight);
		
		if(currentSection == Section.Normal)
		{
			GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
			GUI.Label ( currentRect, "PLAYERID", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + playerIDColumnWidth;
			GUI.Label ( currentRect, "NAME", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + messageColumnWidth;
			GUI.Label ( currentRect, "LEVEL", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + levelColumnWidth;
			currentRect.x = interColumnSeprators;
			currentRect.y += divideHeight;
			currentRect.height = sectionHeight;
			ScrollViewContentHeight = Mathf.Max (contentBounds.height, friendsDict.Count * (entryBounds.height + entrySpacing));
			foreach (KeyValuePair<string,Roar.DomainObjects.Friend> f in friendsDict)
			{
				currentRect.width = playerIDColumnWidth;
				GUI.Label(currentRect, string.Format( playerIdFormatString, f.Value.player_id ), playerIdFormat );
				currentRect.x += interColumnSeprators + playerIDColumnWidth;
				currentRect.width = playerNameColumnWidth;
				GUI.Label(currentRect, string.Format( nameFormatString, f.Value.name ), nameFormat );
				currentRect.x += interColumnSeprators + playerNameColumnWidth;
				currentRect.width = levelColumnWidth;
				GUI.Label(currentRect, string.Format( levelFormatString, f.Value.level ), levelFormat );
				currentRect.x += levelColumnWidth + interColumnSeprators;

				if(GUI.Button(currentRect, "Delete", "DefaultButton"))
				{
					networkActionInProgress = true;
					friends.RemoveFriend(f.Value.player_id, DefaultRoar.Instance.PlayerId(), OnRoarFriendsRemove);
				}
				currentRect.x = interColumnSeprators;
				currentRect.y += sectionHeight;
			}
			foreach(string key in externalPlayers.Keys)
			{
				currentRect.width = playerIDColumnWidth;
				GUI.Label(currentRect, string.Format(playerIdFormatString, key), playerIdFormat);
				currentRect.x += interColumnSeprators + playerIDColumnWidth;
				currentRect.width = playerNameColumnWidth;
				GUI.Label(currentRect, string.Format(nameFormatString, externalPlayers[key]), nameFormat);
				currentRect.x += interColumnSeprators + playerNameColumnWidth;
				currentRect.width = levelColumnWidth;
				currentRect.x += levelColumnWidth + interColumnSeprators;
				if(GUI.Button(currentRect, "Invite", "DefaultButton"))
				{
					friends.InviteFriend(key, DefaultRoar.Instance.PlayerId(), OnRoarFriendsSendInvite);
					Debug.Log("INVITATION FROM [" + DefaultRoar.Instance.PlayerId() + "] TO [" + key + "]");
				}
				currentRect.x = interColumnSeprators;
				currentRect.y += sectionHeight;
			}
			
			currentRect.width = contentBounds.width;
			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");
			
			GUI.Box(new Rect(0, contentBounds.height - footerSpacing, contentBounds.width, footerSpacing), new GUIContent(""), "DefaultFooterStyle");
			currentRect.x = interColumnSeprators;
			currentRect.y = contentBounds.height - footerSpacing/2 - buttonHeight/2;
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;

			if(GUI.Button(currentRect, "Invite Friend", "DefaultButton"))
			{
				statusString = "";
				currentSection = Section.SendingInvite;
				drawSubheading = true;
				subheaderName = "Invite Friends";
			}

			currentRect.x += buttonWidth+interColumnSeprators;
			
			if(GUI.Button(currentRect, "Check Invites", "DefaultButton"))
			{
				statusString = "";
				currentSection = Section.AcceptInvites;
				FetchAcceptable();
				drawSubheading = true;
				subheaderName = "Pending Invites";
			}
		}
		if(currentSection == Section.AcceptInvites)
		{
			GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
			GUI.Label ( currentRect, "PLAYERID", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + playerIDColumnWidth;
			GUI.Label ( currentRect, "NAME", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + messageColumnWidth;
			GUI.Label ( currentRect, "LEVEL", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + levelColumnWidth;
			currentRect.x = interColumnSeprators;
			currentRect.y += divideHeight;
			currentRect.height = sectionHeight;
			ScrollViewContentHeight = Mathf.Max (contentBounds.height, friendsDict.Count * (entryBounds.height + entrySpacing));
			if(friendsInviteList != null)
			foreach (Roar.DomainObjects.FriendInvite f in friendsInviteList)
			{
				currentRect.width = playerIDColumnWidth;
				GUI.Label(currentRect, f.player_id, playerIdFormat );
				currentRect.x += interColumnSeprators + playerIDColumnWidth;
				currentRect.width = playerNameColumnWidth;
				GUI.Label(currentRect, f.player_name, nameFormat );
				currentRect.x += interColumnSeprators + playerNameColumnWidth;
				currentRect.width = messageColumnWidth;
				GUI.Label(currentRect, f.message, levelFormat );

				currentRect.x += interColumnSeprators+messageColumnWidth;
				currentRect.width = buttonWidth;
				currentRect.height = buttonHeight;
				
				if(GUI.Button(currentRect, "Accept", buttonFormat))
				{
					inviteManipulated = f;
					networkActionInProgress = true;
					friends.AcceptFriendInvite(f.player_id, f.invite_id, OnRoarFriendsAccept );
				}
				currentRect.x += interColumnSeprators + buttonWidth;
				if(GUI.Button(currentRect, "Decline", buttonFormat))
				{
					inviteManipulated = f;
					friends.DeclineFriendInvite(f.invite_id, OnRoarFriendsDeclineInvite);
				}
				currentRect.y += sectionHeight;
			}

			currentRect.width = contentBounds.width;
			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");

			GUI.Box(new Rect(0, contentBounds.height - footerSpacing, contentBounds.width, footerSpacing), new GUIContent(""), "DefaultFooterStyle");
			currentRect.x = interColumnSeprators;
			currentRect.y = contentBounds.height - footerSpacing/2 - buttonHeight/2;
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;

			if(GUI.Button(currentRect, "Back", "DefaultButton"))
			{
				Fetch();
				statusString = "";
				currentSection = Section.Normal;
				drawSubheading = false;
				subheaderName = "Invite Friends";
			}
		}

		if(currentSection == Section.SendingInvite)
		{
			currentRect.y += verticalSeparators;
			currentRect.width = contentBounds.width;
			currentRect.height = divideHeight;
			currentRect.x = interColumnSeprators;
			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");
			currentRect.y += divideHeight;

			currentRect.x = interColumnSeprators;
			currentRect.width = labelWidth;
			currentRect.height = labelHeight;
			GUI.Label(currentRect,"To: (PlayerID)");
			currentRect.x += labelWidth + interColumnSeprators;
			currentRect.width = contentBounds.width- currentRect.x  - interColumnSeprators - selectButtonWidth;
			currentRect.height = textBoxHeight;
			playerIdToSendTo = GUI.TextField(currentRect, playerIdToSendTo, "DefaultTextArea");
			currentRect.x += contentBounds.width- currentRect.x  - interColumnSeprators - selectButtonWidth;
			currentRect.width = selectButtonWidth;
			if(GUI.Button(currentRect, "?", "DefaultButton"))
			{
				Debug.Log("going");
				System.Action<string> act = (pid) => {
					playerIdToSendTo = pid;
				};
				GameObject.Find("/PlayerSelectionWidget").SendMessage("setCallbackAndEnable", act);
			}
			currentRect.y += labelHeight+verticalSeparators;
			currentRect.x = interColumnSeprators;
			currentRect.width = labelWidth;
			currentRect.height = labelHeight;
			GUI.Label(currentRect, "Message: ", "DefaultLabel");
			currentRect.x += labelWidth + interColumnSeprators;
			currentRect.width = contentBounds.width -currentRect.x  - interColumnSeprators;
			currentRect.height = messageBoxHeight;
			messageToSend = GUI.TextArea(currentRect, messageToSend, "DefaultBigMessageBox");

			currentRect.y += messageBoxHeight+verticalSeparators;
			currentRect.x = interColumnSeprators;
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;

			if(GUI.Button(currentRect, "Send", "DefaultButton"))
			{
				friends.InviteFriend(playerIdToSendTo, DefaultRoar.Instance.PlayerId(), OnRoarFriendsSendInvite);
			}

			currentRect.x += buttonWidth + interColumnSeprators;
			if(GUI.Button(currentRect, "Back", "DefaultButton"))
			{
				statusString = "";
				drawSubheading = false;
				currentSection = Section.Normal;
			}

			currentRect.x = interColumnSeprators;
			currentRect.y+= buttonHeight+verticalSeparators;
		}
	}

	public void SendInviteWithPID(string pid)
	{
		this.enabled = true;
		currentSection = Section.SendingInvite;
		statusString = "";
		currentSection = Section.SendingInvite;
		drawSubheading = true;
		subheaderName = "Invite Friends";
		playerIdToSendTo = pid;
	}
}
