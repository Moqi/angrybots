using System;

namespace Roar.Components
{
	/**
	 * \brief Methods for creating, authenticating and logging out a User.
	 *
	 * @todo The naming of the members of this class seem a little odd.
	 **/
	public interface IGoogle
	{
		/**
		 * Login a player using Google.
		 *
		 * On success:
		 * - invokes callback with empty data parameter, success code and success message
		 * - fires a RoarManager#loggedInEvent
		 *
		 * On failure:
		 * - invokes callback with empty data parameter, error code and error message
		 * - fires a RoarManager#logInFailedEvent containing a failure message
		 *
		 * @param code.
		 * @param google_client_id.
		 * @param cb the callback function to be passed the result.
		 **/
		void Login (string code, string google_client_id, Roar.Callback<WebObjects.Google.LoginUserResponse> cb);
	}
}

