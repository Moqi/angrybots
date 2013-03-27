using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;

public static class PlayerDictionary {

	public static Dictionary<string, IList<LeaderboardExtraProperties>>  playerIDleaderboardDict;
	public static Dictionary<string, Roar.DomainObjects.Friend> playerIDFriendDict;
	public static Dictionary<string, string> playerIDFriendInviteDict;
	
	public static void Init()
	{
		playerIDleaderboardDict = new Dictionary<string, IList<LeaderboardExtraProperties>>();
		playerIDFriendDict = new Dictionary<string, Friend>();
		playerIDFriendInviteDict = new Dictionary<string, string>();
		
		Roar.implementation.Components.Leaderboards.OnLeaderboardFetchCompleted += LoadChangedLeaderboard;
		RoarManager.roarServerFriendRequestEvent+= delegate(Roar.Events.FriendRequestEvent friendReq) {
			if(playerIDFriendInviteDict.ContainsKey(friendReq.from_player_id))
			{
				playerIDFriendInviteDict.Add(friendReq.from_player_id, friendReq.name);
			}
		};
	}
	
	static void LoadChangedLeaderboard(Roar.WebObjects.Leaderboards.ViewResponse response)
	{
		
		foreach(LeaderboardEntry e in response.leaderboard_data.entries)
		{
			if(!playerIDleaderboardDict.ContainsKey(e.player_id))
			{
				playerIDleaderboardDict.Add(e.player_id, e.properties);
			}
		}
	}
	
	public static void UpdateFriendsDictionary()
	{
		IList<Roar.DomainObjects.Friend> friendList = DefaultRoar.Instance.Friends.List(); //lists friends and friend invites.
		
		playerIDFriendDict.Clear();
		
		foreach(Friend f in friendList)
		{
			playerIDFriendDict.Add(f.player_id, f);
		}
	}
}
