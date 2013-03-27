using System;
using System.Collections;

/*! \mainpage %Roar Unity SDK Reference
 *
 * \section intro_sec Introduction
 *
 * \subsection what_is_roar What is Roar?
 *
 * %Roar is a configurable game mechanics platform.<br/>
 * You can access the creation tools from the Admin dashboard.<br/>
 *
 * \subsection what_is_roar_unity What is the Roar Unity SDK used for?
 *
 * The Unity SDK provides a wrapper around the %Roar Web API, making it easier to interact with a %Roar game server from your Unity game.
 * Should you desire to do so, you can roll your own code that uses the Web API directly.
 * However if your game is built with Unity, we recommend the use of the %Roar Unity SDK and welcome any open source contributions to it.
 *
 * \section sdk_basics Roar Unity SDK Basics
 *
 * The two key objects within the %Roar Unity SDK are the interface IRoar (found in IRoar.cs and implemented in DefaultRoar.cs) and the class that provides event management – RoarManager (found in RoarManager.cs).
 *
 * \subsection iroar_interface The IRoar Interface
 *
 * The IRoar interface is what game developers use to interact with their %Roar game on the server.
 * IRoar is composed of a number of component interfaces each representing a distinct area of game functionality.
 * The following table lists each component, clicking on a component name will show more detailed class level documentation.
 *
 *
 <table>
   <tr>
    <th>Component</th>
    <th>Provides Methods For</th>
   </tr>
   <tr>
    <td>Roar.Components.IUser</td>
    <td>Creating, authenticating and logging out a player.</td>
   </tr>
   <tr>
    <td>Roar.Components.IProperties</td>
    <td>Retrieving the properties of a player, properties can include user attributes, resources and currencies for a given user. Properties are setup in the Admin dashboard.</td>
   </tr>
   <tr>
    <td>Roar.Components.IInventory</td>
    <td>Viewing and manipulating a user's inventory. Inventory items are obtained through purchase, as gifts or completing Actions.</td>
   </tr>
   <tr>
    <td>Roar.Components.IShop</td>
    <td>Viewing and buying shop items.</td>
   </tr>
   <tr>
    <td>Roar.Components.IActions</td>
    <td>Listing and executing actions. An action can be 'completed' if requirements/costs can be met, often resulting in player rewards such as levelling up or receiving a bonus item.</td>
   </tr>
   <tr>
    <td>Roar.Components.IAchievements</td>
    <td>Viewing a player's achievements, achievements are attained by completing certain actions.</td>
   </tr>
   <tr>
    <td>Roar.Components.IGifts</td>
    <td>Viewing items a player can gift to other players</td>
   </tr>
   <tr>
    <td>Roar.Components.IInAppPurchase</td>
    <td>Viewing and buying appstore items - Unity iOS builds only</td>
   </tr>
 </table>
 *
 * \subsection roar_manager RoarManager
 *
 * The Unity %Roar SDK handles responses from the server by using a two pronged approach. Some %Roar functions take a callback function as the last argument, this callback function is executed when the %Roar server response has been received and processed by the Unity SDK. However, some events occur on the server and must be subscribed to via event handling functions. Where possible both approaches are made available to the developer e.g. if an authentication attempt fails, it could be handled by:
 *
 * subscribing a login failed function to the logInFailedEvent
 *
 * <code>
 * RoarManager.logInFailedEvent += onLoginFailed;
 * function onLoginFailed(msg) {
 *  ...
 * }
 * roar.login(username, password);
 * </code>
 *
 * or by passing a login failed callback function to the authenticate function
 *
 * <code>
 * roar.login(username, password, function(cb_info:Roar.CallbackInfo){
 *   // cb_info.code
 *   // cb_info.message
 * });
 * </code>
 *
 * Notice how the function is defined within the login function's argument list; this is an anonymous function. Callbacks can (but don't have to) be anonymous functions.
 *
 * The RoarManager class documentation contains a list of all the events that can be subscribed to.
 *
 *
 * \section troubleshooting Troubleshooting
 *
 *
 * Most %Roar API calls require an <strong>auth_token</strong> (authentication token). The auth token is automatically retrieved when a player is authenticated.
 *
 * If you are experiencing problems with %Roar, first check that an authentication token has been assigned as part of player authentication. You can do this by logging the IRoar interface's AuthToken string.
 *
 * If authentication isn't your problem, here's a suggested list of steps you can take to get your problem resolved:
 *
 * <ol>
 *   <li>Read the documentation. Available for you is the:<br/>
 *     <ul>
 *       <li>doxygen generated class documentation<br/>
 *         http://roarengine.github.com/unityapi/
 *       </li>
 *       <li>
 *         project readme on github<br/>
 *         https://github.com/roarengine/sdk-unity/blob/master/README.md
 *       </li>
 *       <li>
 *         %Roar knowledge base<br/>
 *         http://support.roarengine.com/kb
 *       </li>
         <br/>
 *     </ul>
 *   </li>
 *   <li>
 *     Search the support forums for your problem<br/>
 *     http://support.roarengine.com/discussions/support<br/>
 *     If you can't find an existing solution, create a ticket detailing what your problem is, providing whatever details you think will help the %Roar support team and the community solve your problem.
 *   </li>
 * </ol>
 *
 * \section unity_sdk_vs_web_api Does the Unity SDK support all the features available to the Web API?
 *
 * There is a Unity to Web API bridge that enables developers to make any call to the Web API, accessible via the IRoar.WebAPI interface; however due to the nature of the roar server responses, it's important that the right events are fired when a WebAPI response is received.
 *
 * The sdk has several levels of abstraction, some features of the web api are exposed at a high level via 'Components' such as the player's inventory or player's properties, whilst other features are exposed at a low level such as interaction with friends. A short code level comparison of the two follows:
 *
 * To get the contents of the player's inventory:
 * <code>
 * roar.Inventory.list()
 * </code>
 *
 * To get the list of player's friends:
 * <code>
 * roar.WebAPI.IFriendsActions.list(Roar.WebObjects.Friends.ListArguments args, IRequestCallback<Roar.WebObject.Friends.ListResposne> cb)
 * </code>
 *
 * where the first argument is the options to send to the roar server and the second argument is the callback function to be passed the xml response from the server.
 *
 * It is important in using the low level api to ensure that the correct events are passed to the RoarManager class, one can browse through the component classes in the sdk's Roar.implementation.Components namespace to see examples of this requirement.
 *
 * It should be noted that due to the api's alpha status, the low level api is less stable and more likely to evolve than the high level components interface i.e. you are welcome to use the low level api to leverage features that are not yet implemented as components, however you do so at your own risk, with the knowledge that the low level api may change in the near future.
 *
 */
namespace Roar
{
	public class CallbackInfo<T>
	{
		public T data;
		public int code;
		public string msg;

		public CallbackInfo (T in_data, int in_code, string in_msg)
		{
			data = in_data;
			code = in_code;
			msg = in_msg;
		}

		public CallbackInfo (T in_data, int in_code ) : this(in_data, in_code, "") {}
		public CallbackInfo (T in_data) : this( in_data, IWebAPI.OK, "") {}
	};

	public delegate void Callback<T> (CallbackInfo<T> h);
}

/**
 * The public facing container for Roar functionality.
 *
 * You get a real instance of this interface by binding the Roar script to an
 * object in your game.
 *
 * This class provides several utility functions for common tasks, and  several
 * lower-level components for more detailed operations.
 */
public interface IRoar
{
	/**
   * Get a configuration object that lets you configure how various
   * aspects of roar.io behave.
   */
	Roar.IConfig Config { get; }

	/**
   * Low level access to the entire roar api.
   *
   * @note The callbacks used by the #IWebAPI are slightly different from the Callbacks used by
   * the other functions in #IRoar .
   */
	IWebAPI WebAPI { get; }
	
	/**
	 * Core user functions.
	 */
	Roar.Components.IUser User { get; }

	/**
   * Get access to the players properties and stats.
   */
	Roar.Components.IProperties Properties { get; }

	/**
   * Get access to the players inventory functions.
   */
	Roar.Components.IInventory Inventory { get; }

	/**
   * Get access to the shop functions.
   */
	Roar.Components.IShop Shop { get; }

	/**
   * Get access to the tasks/actions functions.
   */
	Roar.Components.ITasks Tasks { get; }

	/**
   * Get access to the achievements functions.
   */
	Roar.Components.IAchievements Achievements { get; }

	/**
   * Get access to the gifts functions.
   */
	Roar.Components.IGifts Gifts { get; }

	Roar.Components.IInAppPurchase Appstore { get; }

	/**
   * Methods for notifications.
   */
	Roar.Adapters.IUrbanAirship UrbanAirship { get; }

	/**
   * The roar authentication token.
   *
   * You usually would not need this, unless you are making some direct
   * calls to the roar servers, but can be usefull for debugging.
   */
	string AuthToken { get; }

	/**
   * Returns a string representing the current version of the roar.io API
   * that you are using.
   *
   * The callback, if provided, is called with the arguments
   *
   *     {"data":"version string", "code":IWebAPI.OK, "msg":null}
   */
	string Version ();

}


