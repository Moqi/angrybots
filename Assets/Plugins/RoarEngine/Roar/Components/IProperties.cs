using System.Collections;
using System.Collections.Generic;


namespace Roar.Components
{

	/**
   * \brief IProperties is an interface for retrieving the properties of the authenticated user.
   *
   * Properties can include user attributes, resources and currencies for a given user, along with other core user information.
   *
   * You can get the list of all properties via the #list function,
   * a single property via the #getProperty function or if you only want
   * the value of a property - you can use the #getValue function.
   *
   * \code
   * Hashtable userHealth = Properties.getProperty("health");
   *
   * string currentValue = userHealth["value"];
   *        // or
   *        currentValue = Properties.getValue("health");
   *
   * string maxHealth = userHealth["max"];
   *
   * \endcode
   *
   **/
	public interface IProperties
	{

		/**
	     * Fetch player information from the server.
	     *
	     * On success:
	     * - invokes callback with parameter *Hastable data* containing the properties for the user
	     * - fires the RoarManager#propertiesReadyEvent
	     * - sets #hasDataFromServer to true
	     *
	     * On failure:
	     * - invokes callback with error code and error message
	     *
	     * @param callback the callback function to be passed this function's result.
	     *
	     * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	     **/
		void Fetch (Roar.Callback< IDictionary<string,DomainObjects.PlayerAttribute> > callback);

		/**
	     * Check whether any user properties data has been obtained from the server.
	     *
	     * @returns true if #fetch has completed execution.
	     **/
		bool HasDataFromServer { get; }

		/**
	     * Get a list of all the property objects for the authenticated user.
	     *
	     * @returns A list of Hashtables for each user property.
	     *
	     * @note This does _not_ make a server call. It requires the user properties to
	     *       have already been fetched via a call to #fetch. If this function
	     *       is called prior to the successful completion of a #fetch call,
	     *       it will return an empty array.
	     **/
		IList<DomainObjects.PlayerAttribute> List ();

		/**
	     * Returns the property object for a given key.
	     *
	     * @param key the key that uniquely identifies a property.
	     *
	     * @returns the property Hashtable associated with the *key*
	     *          or null if the property does not exist in the data store.
	     **/
		DomainObjects.PlayerAttribute GetProperty (string key);

		/**
	     * Returns the *value* attribute of a property object.
	     *
	     * @param ikey the key that uniquely identifies a property.
	     *
	     * @return the *value* attribute of a property object
	     *         or null if the user property does not exist in the data store.
	     */
		string GetValue (string ikey);
	}
}
