using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;

namespace Roar.Components
{

	/**
   * \brief IFriends is an interface for listing all of a friends.
   **/
	public interface IFriends
	{

		/**
	     * Fetch friend information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the friend list
	     * - fires the RoarManager#friendsReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void Fetch (Roar.Callback< IDictionary<string,DomainObjects.Friend> > callback);

		/**
	     * Check whether any friend list data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the friend objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each friend.
	     *
	     * @note This does _not_ make a server call. It requires the friends list to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		IList<DomainObjects.Friend> List ();

		/**
	     * Returns the friend object for a given key.
	     *
	     * @param ikey the key that uniquely identifies a friend.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the leaderboard does not exist in the data store.
	     **/
		DomainObjects.Friend GetFriend (string ikey);

		void AcceptFriendInvite (string friend_id, string invite_id, Roar.Callback<WebObjects.Friends.AcceptResponse> callback);
		void DeclineFriendInvite (string invite_id, Roar.Callback<WebObjects.Friends.DeclineResponse> callback);
		void InviteFriend (string friend_id, string player_id, Roar.Callback<WebObjects.Friends.InviteResponse> callback);
		void RemoveFriend (string friend_id, string player_id, Roar.Callback<WebObjects.Friends.RemoveResponse> callback);
		void ListFriendInvites (Roar.Callback<WebObjects.Friends.ListInvitesResponse> callback);
		void FriendInviteInfo (string invite_id, Roar.Callback<WebObjects.Friends.InviteInfoResponse> callback);
	}
}
