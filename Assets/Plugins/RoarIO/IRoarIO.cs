using System;
using System.Collections;

namespace Roar
{
  public class CallbackInfo
  {
    public CallbackInfo( object dataIn, int codeIn=IWebAPI.OK, string msgIn=null)
    {
      data = dataIn;
      code = codeIn;
      msg = msgIn;
    }

    public int code;
    public string msg;
    public object data;
  };

  /**
   * Many roar.io functions take a callback function.  Often this callback is
   * optional, but if you wish to use one it is always a #Roar.Callback type.
   * You might not need one if you choose to catch the results of the call using
   * the events in #RoarIOManager.
   *
   * The Hashtable returned will usually contain three parameters:
   *
   *   + code : an int corresponding to the values in #IWebAPI
   *   + msg : a string message, often empty on success, but containing more
   *     details in the case of an error
   *   + data : an object with the results of the call.
   *
   * The only place you might need to provide a function of a different signature
   * is when using the events specified in #RoarIOManager. These events accept a
   * function that corresponds to the data available to the event. See the
   * individual events for details.
   */
  public delegate void Callback( CallbackInfo h );

}

/**
 * The public facing container for Roar functionality.
 *
 * You get a real instance of this interface by binding the RoarIO script to an
 * object in your game.
 *
 * This class provides several utility functions for common tasks, and  several
 * lower-level components for more detailed operations.
 */
public interface IRoarIO
{
  /**
   * Get a configuration object that lets you configure how various
   * aspects of roar.io behave.
   *
   * @todo This is mostly unused currently!
   */
  Roar.Components.IConfig Config { get; }

  /**
   * Get access to the players properties and stats.
   */
  Roar.Components.IProperties Properties { get; }

  /**
   * @todo This does nothing and might be internal and should be shifted to RoarIO?
   */
  Roar.Components.IData Data { get; }

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
  Roar.Components.IActions Actions { get; }

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
  string version( Roar.Callback callback = null );

  /**
   * Login a player.
   *
   * Requests an authentication token from the server for the player,
   * which is used to validate subsequent requests.
   *
   * On success:
   * - invokes callback with empty data parameter, success code and success message
   * - fires a RoarIOManager#loggedInEvent
   *
   * On failure:
   * - invokes callback with empty data parameter, error code and error message
   * - fires a RoarIOManager#logInFailedEvent containing a failure message
   *
   * @param name the players username
   * @param hash the players password
   * @param cb the callback function to be passed the result of doLogin.
   **/
  void login( string username, string password, Roar.Callback callback=null );

  /**
   * Login a player using Facebook OAuth.
   *
   * On success:
   * - invokes callback with empty data parameter, success code and success message
   * - fires a RoarIOManager#loggedInEvent
   *
   * On failure:
   * - invokes callback with empty data parameter, error code and error message
   * - fires a RoarIOManager#logInFailedEvent containing a failure message
   *
   * @param oauth_token the OAuth token.
   * @param cb the callback function to be passed the result of doLogin.
   **/
  void login_facebook_oauth( string oauth_token, Roar.Callback callback=null );

  /**
   * Logs out a user.
   * Clears the authentication token for a user. Must re-login to authenticate.
   *
   * On success:
   * - fires a RoarIOManager#loggedOutEvent
   *
   * On failure:
   * - invokes callback with empty data parameter, error code and error message
   *
   * @param the callback function to be passed the result of doLoginFacebookOAuth.
   **/
  void logout( Roar.Callback callback=null );

  /**
   * Creates a new user with the given username and password, and logs
   * that player in.
   *
   * On success:
   * - fires a RoarIOManager#createdUserEvent
   * - automatically calls doLogin()
   *
   * On failure:
   * - invokes callback with empty data parameter, error code and error message
   * - fires a RoarIOManager#createUserFailedEvent containing a failure message
   *
   * @param name the players username
   * @param hash the players password
   * @param cb the callback function to be passed the result of doCreate.
   **/
  void create( string username, string password, Roar.Callback callback=null );

  /**
   * @todo Document me!
   */
  string whoami( Roar.Callback callback=null );
}


