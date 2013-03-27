using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;

namespace Roar.Components
{

	/**
   * \brief ILeaderboards is an interface for listing all of a leaderboards.
   **/
	public interface ILeaderboards
	{

		/**
	     * Fetch leaderboard information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the leaderboard list
	     * - fires the RoarManager#leaderboardsReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void FetchBoardList( Roar.Callback< ILeaderboards > callback );
		
		/**
		 * Fetch info for a given board
		 **/
		void FetchBoard( string board_id, int page, Roar.Callback< ILeaderboards > callback );
		

		/**
	     * Check whether any leaderboard data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasBoardList { get; }

		/**
	     * Get a list of all the leaderboard objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each leaderboard.
	     *
	     * @note This does _not_ make a server call. It requires the leaderboards to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		IList<DomainObjects.LeaderboardInfo> BoardList ();
		
		IList<Roar.DomainObjects.LeaderboardEntry> GetLeaderboard( string board_id, int page );

	}
}
