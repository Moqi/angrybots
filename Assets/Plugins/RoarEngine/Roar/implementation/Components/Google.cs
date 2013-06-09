using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class Google : IGoogle
	{
		protected IDataStore dataStore;
		IWebAPI.IGoogleActions google;
		ILogger logger;

		public Google (IWebAPI.IGoogleActions googleActions, IDataStore dataStore, ILogger logger)
		{
			this.google = googleActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		// ---- Access Methods ----
		// ------------------------

		public void Login (string code, string google_client_id, Roar.Callback<WebObjects.Google.LoginUserResponse> cb)
		{
			if (code == "") {
				logger.DebugLog ("[roar] -- Must specify code for google login");
				return;
			}
			
			Roar.WebObjects.Google.LoginUserArguments args = new Roar.WebObjects.Google.LoginUserArguments();
			args.code = code;
			args.google_client_id = google_client_id;

			google.login_user (args, new LoginUserCallback (cb));
		}
		
		class LoginUserCallback : CBBase<WebObjects.Google.LoginUserResponse>
		{
			public LoginUserCallback (Roar.Callback<WebObjects.Google.LoginUserResponse> in_cb) : base(in_cb)
			{
			}
			
			public override void HandleError (Roar.RequestResult info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}
			
			public override void HandleSuccess (CallbackInfo<WebObjects.Google.LoginUserResponse> info)
			{
				RoarManager.OnLoggedIn ();
			}
		}
		
	}

}
