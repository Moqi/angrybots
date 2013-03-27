using System.Collections.Generic;
using Roar.WebObjects;

namespace Roar.Components
{

/**
 * \brief An interface for listing and executing user actions.
 *
 * Tasks typically have three main lists: requirements, costs and rewards:
 * - Requirements must be met in order for task to be able to complete
 * - Costs are deducted from a user (and must be met to successfully complete the task)
 * - Rewards describes the rewards that are given to a user for completing the task
 *
 * An action can have requirements, these are found in the requires list.
 * Each requirement will come back with an "ok" tag and some extra information.
 *
 * Requirement types are:
 *  - \c level_requirement A minimum level for the player to attempt the task.
 *   - \c level
 *  - \c item_requirement An item that the player must have to attempt the task
 *   - \c ikey The ikey of the item required
 *   - \c label A human readable description of the requirement. This is needed since
 *          it is not easy/possible to have "2 keen blades of fright" pluralise automatically.
 *   - \c number_required This will often be 1, but need not be
 *   - \c consumed Boolean that determines whether this requirement should be treated as a cost,
 *          rather than just a requirement.
 *  - \c currency_requirement
 *   - \c ikey the required currency type
 *   - \c value how much is needed
 *  - \c attribute_requirement
 *   - \c ikey the required attribute type
 *   - \c value how much is needed
 *  - \c resource_requirement
 *   - \c ikey the required resource type
 *   - \c value how much is needed
 *
 * You can get the list of all actions via the #list function,
 * and execute an action via the #execute function.
 * *
 *
 * @note The #list functions can only be called after the actions have been fetched from the server via a call to #fetch.
 * @note once #fetch has received and processed actions from the server, the #hasDataFromServer
 * property will return true and calls to #list will be functional.
 **/
	public interface ITasks
	{
		/**
	   * Fetch user actions from the server.
	   *
	   * On success:
	   * - invokes callback with parameter *data* containing the actions for the user
	   * - sets #hasDataFromServer to true
	   *
	   * On failure:
	   * - invokes callback with error code and error message
	   *
	   * @param callback the callback function to be passed this function's result.
	   *
	   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	   **/
		void Fetch (Roar.Callback< IDictionary<string,Roar.DomainObjects.Task> > callback);

		/**
	   * Check whether any action data has been obtained from the server.
	   *
	   * @returns true if #fetch has completed execution.
	   */
		bool HasDataFromServer { get; }

		/**
	   * Get a list of all the actions for the authenticated user.
	   *
	   * @returns A list of each action.
	   *
	   * @note This does _not_ make a server call. It requires the actions to
	   *       have already been fetched via a call to #fetch. If this function
	   *       is called prior to the successful completion of a #fetch call,
	   *       it will return an empty array.
	   **/
		IList<Roar.DomainObjects.Task> List ();

		/**
	   * Initiates an action on the server, which evaluates the requirements and conditions for the action.
	   *
	   * On success:
	   * - invokes callback with data param containing result of executed action
	   * - if the action is complete, fires the RoarManager#roarServerTaskCompleteEvent, passing it an IXmlNode
	   *  with the following structure:
	   *
	   *       <task_complete>
	   *         <ikey>task_ikey</ikey>
	   *         <label>Task label</label>
	   *         <description>Task description</description>
	   *         <location></location>
	   *         <tags>comma,separated,tags</tags>
	   *         <costs>
	   *           <stat_change ikey="energy" value="10"/>
	   *         </costs>
	   *         <modifiers>
	   *           <stat_change ikey="xp" value="20"/>
	   *         </modifiers>
	   *         <mastery level="3" progress="100"/>
	   *       </task_complete>
	   *
	   * On failure:
	   * - invokes callback with error code and error message
	   *
	   * @param ikey the key of the action to execute.
	   * @param callback the callback function to be passed this function's result.
	   *
	   * @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
	   **/
		void Execute (string ikey, Roar.Callback<WebObjects.Tasks.StartResponse> callback);
	}

}
