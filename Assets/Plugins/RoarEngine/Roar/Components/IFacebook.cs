using System.Collections;
using System.Collections.Generic;

namespace Roar.Components
{
	/**
	 * Methods for creating, authenticating and logging out a User.
	 *
	 * - Webplayer Login chain:
	 * Call DoWebplayerLogin to attempt to login.
	 * This will send a sendSignedRequest message to the hosting frame and that frame will return the oauth token
	 * CatchFacebookRequest in RoarLoginWidget will catch the oauth token. If oauth is empty, SignedRequestFailed is called.
	 * SignedRequestFailed will send a javascript message to the hosting frame 'returnCodeIfAvailable'.
	 * This will either return a code via CatchCodeGetPara in RoarLoginWidget or return an empty string.
	 * If it returns an empty string you force a redirect via FacebookGraphRedirect and the next time around the code will be available.
	 * You may pass a state parameter that stores your current state and must pass a redirect URL.
	 * Once you have the code parameter call FetchOAuthToken to retrieve your oauth token from roars API.
	 * Remember to set your facebook application ID and application secret through the roar editor.
	 *
	 **/
	public interface IFacebook
	{
		/**
		 * Login a player using Facebook OAuth.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#logInFailedEvent containing a failure message
		 *
		 * @param oauth_token the OAuth token.
		 * @param cb the callback function to be passed the result of doLogin.
		 **/
		void LoginOAuth(string oauth_token, Roar.Callback<WebObjects.Facebook.LoginOauthResponse> cb);
		
		/**
		 * Binds OAuth
		 *
		 * On success:
		 * - fires a RoarManager#facebookBindUserOAuthEvent
		 *
		 * On failure:
		 * - fires a RoarManager#facebookBindUserSignedFailedEvent
		 *
		 * @param cb the callback function to be passed the result
		 */
		void BindUserOAuth (string oauth_token, Roar.Callback<WebObjects.Facebook.BindOauthResponse> cb);
		
		/**
		 * Binds Signed Request
		 *
		 * On success:
		 * - fires a RoarManager#facebookBindUserSignedEvent
		 *
		 * On failure:
		 * - fires a RoarManager#facebookBindUserSignedFailedEvent
		 *
		 * @aparam cb the callback function to be passed the result
		 */
		void BindUserSigned (string signed_request, Roar.Callback<WebObjects.Facebook.BindSignedResponse> cb);

		/**
		 * Creates a new user with the given username and facebook authToken, and logs
		 * that player in. This will only work if you have a signed request verifying the current user.
		 * You will automatically get this from Facebook as a POST parameter if you are running an iframe app within Facebook.
		 *
		 * On success:
		 * - fires a RoarManager#createdUserEvent
		 * - automatically calls doLogin()
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#createUserFailedEvent containing a failure message
		 *
		 * @param name the players username
		 * @param the players facebook signed auth
		 * @param cb the callback function to be passed the result of doCreate.
		 */
		void CreateOAuth(string name, string oAuthToken, Roar.Callback<WebObjects.Facebook.CreateOauthResponse> cb);
		
		/**
		 * List shop
		 *
		 * On success:
		 * - fires a RoarManager#facebookShopListEvent
		 *
		 * On failuire:
		 * - fires a RoarManager#facebookShopListFailedEvent
		 *
		 * @param cb the callback function to be passed the result of the ShopList.
		 */
		void ShopList(Roar.Callback<WebObjects.Facebook.ShopListResponse> cb);
		/**
		 * Starts the login chain for logging in using a webplayer (oauth or signed request method).
		 *
		 * On success:
		 * - fires a RoarManager#onCreatedUser event
		 * - fires a RoarManager#onLoggedInUser event
		 *
		 * On failuire:
		 * - fires a RoarManager#login
		 *
		 * @param cb the callback function to be passed the result of the login.
		 */
		void DoWebplayerLogin(Roar.Callback<WebObjects.Facebook.LoginOauthResponse> callback);

		/**
		 *  Starts the login chain for creating a new user using a webplayer
		 *
		 * On success:
		 * - fires a RoarManager#onCreatedUser
		 * - fires a RoarManager#onLoggedInUser event
		 *
		 * On failuire:
		 * - fires a RoarManager#facebookShopListEvent
		 *
		 * @param cb the callback function to be passed the result of the create.
		 */
		void DoWebplayerCreate(string name, Roar.Callback<WebObjects.Facebook.CreateOauthResponse> callback);

		/**
		 * Binds the logged in facebook account to the logged in roar account
		 *
		 * On success:
		 * - fires a RoarManager#facebookBindUseroAuthEvent
		 *
		 * On failuire:
		 * - fires a RoarManager#facebookBindUserOAuthFailedEvent
		 *
		 * @param cb the callback function to be passed the result of the bind.
		 */
		void DoWebplayerBind(Roar.Callback<WebObjects.Facebook.BindOauthResponse> callback);

		/**
		 * Causes the hosting frame to redirect the facebook graph url passing the
		 *
		 * On success:
		 * - The hosting webpage redirects to the facebook graph api
		 *
		 * On failuire:
		 * -
		 *
		 * @param facebookApplicationID  The Id obtained from facebook for the targetted application.
		 * @param redirectURL	The URL that the page needs to redirect to after authenticating with facebook.
		 */
		void FacebookGraphRedirect(string facebookApplicationID, string state, string redirectURL);

		/**
		 * Fetches the oauth token from the roar server passing along the code obtained from the graph url redirect.
		 *
		 * On success:
		 * - callback is called with the appropriate response
		 *
		 * On failuire:
		 * - callback is called with the appropriate response
		 *
		 * @param codeParameter	the code parameter obtained from facebook.
		 * @param cb the callback function to be passed the result of the ShopList.
		 */
		void FetchOAuthToken(string codeParameter, Roar.Callback<WebObjects.Facebook.FetchOauthTokenResponse> callback);

		/**
		 * Sets the oauth string directly if obtained from an external source.
		 *
		 * @param oauth token.
		 */
		void SetOAuthToken(string oauth_token);

		/**
		 * Continues the facebook chain after facebook login happens
		 *
		 */
		void DoPostLoginAction();

		/**
		 * Intimates that the signed request method has failed and to switch to the oauth login method.
		 * @param This string is used to pass the state between redirects. Use it to protect from cross site request forgery.
		 */
		void SignedRequestFailed();

		/**
		 * Returns true or false if facebook is logged in or not.
		 *
		 */
		bool IsLoggedIn();

		/**
		 * Returns true or false if the user is logged in to roar via facebook.
		 *
		 */
		bool IsLoggedInViaFacebook();

		/**
		 * Request facebook state parameter to retireve application state in case of a redirect.
		 *
		 * - Sends a message to the hosting javascript frame trying to call the function 'returnSateIfAvailable'
		 *
		 */
		void RequestFacebookStatePara();


		/**
		* Fetch shop information from the server.
		*
		* On success:
		* - invokes callback with parameter *Array<ShopEntry> data* containing the data for the shop.
		* - sets #hasDataFromServer to true
		*
		* On failure:
		* - invokes callback with error code and error message
		*
		* @param callback the callback function to be passed this function's result.
		*
		* @returns nothing - use a callback and/or subscribe to RoarManager events for results of non-blocking calls.
		*/
		void FetchShopData (Roar.Callback<IDictionary<string,DomainObjects.FacebookShopEntry>> callback);

		/**
		* Get a list of all the available items in the shop.
		*
		* @returns A list of Hashtables for each shop item.
		*
		* @note This does _not_ make a server call. It requires the shop data to
		*have already been fetched via a call to #fetch. If this function
		*is called prior to the successful completion of a #fetch call,
		*it will return an empty array.
		*/
		IList<DomainObjects.FacebookShopEntry> List ();

		/**
		* Check whether facebook shop information has been obtained from the server.
		*
		* @returns true if #fetch has completed execution.
		*/
		bool HasDataFromServer { get; }

	}
}
