using System.Collections;
using System.Collections.Generic;
using Roar.Components;
using Roar.DomainObjects;

namespace Roar.implementation.Components
{

	public class Friends : IFriends
	{
		protected IDataStore dataStore;
		IWebAPI.IFriendsActions friendsActions;
		protected ILogger logger;

		public Friends (IWebAPI.IFriendsActions friendsActions, IDataStore dataStore, ILogger logger)
		{
			this.friendsActions = friendsActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		public void Fetch (Roar.Callback< IDictionary<string,DomainObjects.Friend> > callback)
		{
			dataStore.friends.Fetch(callback);
		}

		public bool HasDataFromServer { get { return dataStore.friends.HasDataFromServer; } }

		public IList<Friend> List ()
		{
			return dataStore.friends.List();
		}


		public Friend GetFriend (string ikey)
		{
			return dataStore.friends.Get(ikey);
		}

		public void AcceptFriendInvite (string friends_id, string invite_id, Roar.Callback<WebObjects.Friends.AcceptResponse> cb)
		{
			if (friends_id == "" || invite_id == "")
			{
				logger.DebugLog ("[roar] -- Must specify friends_id and invite_id for accepting a friend invite.");
				return;
			}

			WebObjects.Friends.AcceptArguments args = new WebObjects.Friends.AcceptArguments();
			args.friends_id = friends_id;
			args.invite_id = invite_id;
			friendsActions.accept (args, new AcceptFriendsCallback (cb));
		}

		protected class AcceptFriendsCallback : CBBase<WebObjects.Friends.AcceptResponse>
		{
			public AcceptFriendsCallback (Roar.Callback<WebObjects.Friends.AcceptResponse> in_cb) : base (in_cb) {}
		}

		public void DeclineFriendInvite (string invite_id, Roar.Callback<WebObjects.Friends.DeclineResponse> cb)
		{
			if (invite_id == "")
			{
				logger.DebugLog ("[roar] -- Must specify invite_id for declining a friend invite.");
				return;
			}

			WebObjects.Friends.DeclineArguments args = new WebObjects.Friends.DeclineArguments();
			args.invite_id = invite_id;
			friendsActions.decline (args, new DeclineFriendsCallback (cb));
		}

		protected class DeclineFriendsCallback : CBBase<WebObjects.Friends.DeclineResponse>
		{
			public DeclineFriendsCallback (Roar.Callback<WebObjects.Friends.DeclineResponse> in_cb) : base (in_cb) {}
		}

		public void InviteFriend (string friend_id, string player_id, Roar.Callback<WebObjects.Friends.InviteResponse> cb)
		{
			if (friend_id == "")
			{
				logger.DebugLog("[roar] -- Must specify friend_id for inviting a friend.");
				return;
			}
			if (player_id == "")
			{
				logger.DebugLog("[roar] -- Must specify player_id for inviting a friend.");
			}

			WebObjects.Friends.InviteArguments args = new WebObjects.Friends.InviteArguments();
			args.friend_id = friend_id;
			args.player_id = player_id;
			friendsActions.invite(args, new InviteFriendCallback(cb));
		}

		protected class InviteFriendCallback : CBBase<WebObjects.Friends.InviteResponse>
		{
			public InviteFriendCallback (Roar.Callback<WebObjects.Friends.InviteResponse> in_cb) : base (in_cb) {}
		}

		public void RemoveFriend (string friend_id, string player_id, Roar.Callback<WebObjects.Friends.RemoveResponse> cb)
		{
			if (friend_id == "")
			{
				logger.DebugLog("[roar] -- Must specify friend_id for removing a friend.");
			}
			if (player_id == "")
			{
				logger.DebugLog("[roar] -- Must specify player_id for removing a friend.");
			}

			WebObjects.Friends.RemoveArguments args = new WebObjects.Friends.RemoveArguments();
			args.friend_id = friend_id;
			args.player_id = player_id;
			friendsActions.remove(args, new RemoveFriendCallback(cb));
		}

		protected class RemoveFriendCallback : CBBase<WebObjects.Friends.RemoveResponse>
		{
			public RemoveFriendCallback (Roar.Callback<WebObjects.Friends.RemoveResponse> in_cb) : base (in_cb) {}
		}

		public void ListFriendInvites (Roar.Callback<WebObjects.Friends.ListInvitesResponse> cb)
		{
			friendsActions.list_invites( new WebObjects.Friends.ListInvitesArguments(), new CBBase<WebObjects.Friends.ListInvitesResponse>(cb) );
		}

		public void FriendInviteInfo (string invite_id, Roar.Callback<WebObjects.Friends.InviteInfoResponse> cb)
		{
			if (invite_id == "")
			{
				logger.DebugLog("[roar] -- Must specify invite_id for fetch invite info.");
			}

			WebObjects.Friends.InviteInfoArguments args = new WebObjects.Friends.InviteInfoArguments();
			args.invite_id = invite_id;
			friendsActions.invite_info(args, new CBBase<WebObjects.Friends.InviteInfoResponse>(cb));
		}

	}
}
