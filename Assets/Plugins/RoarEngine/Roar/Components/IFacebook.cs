using System.Collections;

namespace Roar.Components
{
	/**
	 * \brief Methods for creating, authenticating and logging out a User.
	 *
	 * @todo The naming of the members of this class seem a little odd.
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
	}
}
