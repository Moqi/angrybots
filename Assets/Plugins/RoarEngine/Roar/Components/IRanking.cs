using System.Collections;
using System.Collections.Generic;

namespace Roar.Components
{

	/**
   * \brief IRanking is an interface for listing leaderboard rankings.
   **/
	public interface IRanking
	{
		int Page { set; }

		/**
	     * Fetch ranking information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the ranking
	     * - fires the RoarManager#rankingReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void Fetch (Roar.Callback< IDictionary<string,DomainObjects.LeaderboardData> > callback);

		/**
	     * Check whether any ranking data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the ranking objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each ranking.
	     *
	     * @note This does _not_ make a server call. It requires the leaderboards to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		IList<DomainObjects.LeaderboardData> List ();

		/**
	     * Returns the ranking object for a given key.
	     *
	     * @param ikey the key that uniquely identifies a ranking.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the ranking does not exist in the data store.
	     **/
		DomainObjects.LeaderboardData GetEntry (string ikey);
	}
}
