using System.Collections.Generic;

namespace Roar.Components
{
	/**
   * \brief IGifts is an interface for listing all of the items belonging to a user that can be gifted to other users.
   *
   * You can get the list of all giftable items via the #list function or
   * a single giftable item via the #getGift function.
   *
   * A gift is represented in a Hashtable using the following keys:
   *
   * \code
   *
   *  - id: "3467"
   *  - type: "gift"
   *  - label: "a label"
   *  - requirements : list of requirements to send this give
   *  - costs : list of costs for this gift
   *  - on_accept : list of actions to execute on accept
   *  - on_give : list of actions to execute on give
   *  - tags
   *
   * \endcode
   *
   * @todo: are all mailable items gifts, or only those of type:'gift'?
   **/
	public interface IGifts
	{

		/**
	     * Fetch gifts information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the gifts for the user
	     * - fires the RoarManager#giftsReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void Fetch (Roar.Callback< IDictionary<string,DomainObjects.MailPackage> > callback);
		
		/**
	     * Fetch gifts information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the gifts for the user
	     * - fires the RoarManager#giftsReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void FetchSendable (Roar.Callback< IDictionary<string,DomainObjects.Mailable> > callback);

		/**
	     * Check whether any user gifts data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the gift objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each gift.
	     *
	     * @note This does _not_ make a server call. It requires the gifts to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		IList<DomainObjects.MailPackage> List ();


		/**
	     * Returns the gift object for a given key.
	     *
	     * @param id the key that uniquely identifies a gift.
	     *
	     * @returns the gift Hashtable associated with the *id*
	     *          or null if the gift does not exist in the data store.
	     **/
		DomainObjects.MailPackage get (string id);
		
		void AcceptGift (string gift_id, Roar.Callback<WebObjects.Mail.AcceptResponse> callback);
		void SendGift (string recipient_id, string mailable_id, string message, Roar.Callback<WebObjects.Mail.SendResponse> callback);
		void ListAcceptableGifts (Roar.Callback<WebObjects.Mail.WhatCanIAcceptResponse> callback);
		void ListSendableGifts (Roar.Callback<WebObjects.Mail.WhatCanISendResponse> callback);
	}
}
