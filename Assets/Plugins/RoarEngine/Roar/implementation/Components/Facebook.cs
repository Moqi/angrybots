using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class Facebook : IFacebook
	{
		protected IDataStore dataStore;
		IWebAPI.IFacebookActions facebook;
		ILogger logger;

		string oAuthToken = null;
		bool isLoggedinViaFacebook;

		public FacebookLoginOptions facebookLoginOptions = FacebookLoginOptions.Normal;

		public enum FacebookLoginOptions
		{
			Normal, //Signed request first, then graph redirect attempt.
			SignedRequestOnly, //only allow signed requests. Useful for canvas only apps.
			ExternalOauthOnly, //Will not attempt to login itself but will use a supplied oauth token.
			InternalNonjavascriptLoginNOT_IMPLEMENTED, //Login by itself for standalone/ios builds and such.
		}

		enum PostLogionAction
		{
			Nothing,
			LoginRoar,
			CreateRoar,
			BindRoar,
		}
		PostLogionAction postLoginAction;
		string requestedName;
		Roar.Callback<WebObjects.Facebook.BindOauthResponse> requestedBindCB;
		Roar.Callback<WebObjects.Facebook.CreateOauthResponse> createoAuthCB;
		Roar.Callback<WebObjects.Facebook.LoginOauthResponse> loginCB;

		public Facebook (IWebAPI.IFacebookActions facebookActions, IDataStore dataStore, ILogger logger)
		{
			this.facebook = facebookActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		protected class FacebookCreateCallback : CBBase<WebObjects.Facebook.CreateOauthResponse>
		{
			protected Facebook facebook;

			public FacebookCreateCallback (Roar.Callback<WebObjects.Facebook.CreateOauthResponse> in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void HandleError (Roar.RequestResult nn)
			{
				RoarManager.OnCreateUserFailed (nn.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.CreateOauthResponse> info)
			{
				facebook.isLoggedinViaFacebook = true;
				RoarManager.OnCreatedUser ();
				RoarManager.OnLoggedIn ();
			}
		}

		protected class FacebookBindCallback : CBBase<WebObjects.Facebook.BindOauthResponse>
		{
			protected Facebook facebook;

			public FacebookBindCallback (Roar.Callback<WebObjects.Facebook.BindOauthResponse> in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void HandleError (Roar.RequestResult nn)
			{
				if (Debug.isDebugBuild)
					Debug.Log("Facebook binding failed "+nn.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.BindOauthResponse> info)
			{
				if (Debug.isDebugBuild)
					Debug.Log("binding succeeded");
			}
		}

		// ---- Access Methods ----
		// ------------------------

		public void FacebookGraphRedirect(string facebookApplicationID, string state, string redirectURL)
		{
			if (Debug.isDebugBuild)
				Debug.Log("redirecting because of a blank code");
			Application.OpenURL("https://graph.facebook.com/oauth/authorize?client_id="+facebookApplicationID+"&redirect_uri="+redirectURL+"&state="+state);
		}

		public void FetchOAuthToken(string codeParameter, Roar.Callback<WebObjects.Facebook.FetchOauthTokenResponse> cb)
		{
			DoFetchFacebookOAuthToken(codeParameter, cb);
		}

		public void AttemptFacebookLoginChain()
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				if(facebookLoginOptions != FacebookLoginOptions.ExternalOauthOnly &&
				facebookLoginOptions != FacebookLoginOptions.InternalNonjavascriptLoginNOT_IMPLEMENTED)
				{
					RequestFacebookSignedRequest();
				}
				else
				if(facebookLoginOptions != FacebookLoginOptions.ExternalOauthOnly &&
					facebookLoginOptions != FacebookLoginOptions.InternalNonjavascriptLoginNOT_IMPLEMENTED &&
					facebookLoginOptions != FacebookLoginOptions.SignedRequestOnly)
				{
					RequestFacebookGetCode();
				}
			}
		}

		public void DoWebplayerLogin(Roar.Callback<WebObjects.Facebook.LoginOauthResponse> callback)
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				loginCB = callback;
				postLoginAction = PostLogionAction.LoginRoar;
				AttemptFacebookLoginChain();
			}
			else
			{
				DoLoginFacebookOAuth(callback);
			}
		}

		public void SignedRequestFailed()
		{
			if(facebookLoginOptions != FacebookLoginOptions.SignedRequestOnly)
			{
				RequestFacebookGetCode();
			}
			else
			{
				if (Debug.isDebugBuild)
					Debug.Log ("Signed request login failed and I have no other login options to fall back on.");
			}
		}

		public void DoLoginFacebookOAuth (Roar.Callback<WebObjects.Facebook.LoginOauthResponse> cb)
		{
			if (oAuthToken == "") {
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook login");
				return;
			}

			Roar.WebObjects.Facebook.LoginOauthArguments args = new Roar.WebObjects.Facebook.LoginOauthArguments ();
			args.oauth_token = oAuthToken;
			facebook.login_oauth (args, new LoginOAuthCallback(cb, this));
		}

		public void DoLoginFacebookSignedReq (string signedReq, Roar.Callback<WebObjects.Facebook.LoginSignedResponse> cb)
		{
			if (signedReq == "") {
				logger.DebugLog ("[roar] -- Must specify signedReq for facebook login");
				return;
			}
			
			Roar.WebObjects.Facebook.LoginSignedArguments args = new Roar.WebObjects.Facebook.LoginSignedArguments ();
			args.signed_request = signedReq;
			facebook.login_signed (args, new LoginSignedCallback (cb, this));
		}

		public void DoFetchFacebookOAuthToken (string code, Roar.Callback<WebObjects.Facebook.FetchOauthTokenResponse> cb)
		{
			Roar.WebObjects.Facebook.FetchOauthTokenArguments args = new Roar.WebObjects.Facebook.FetchOauthTokenArguments();
			args.code = code;
			facebook.fetch_oauth_token(args, new FetchFacebookOAuthTokenCallback (cb, this));
		}

		class FetchFacebookOAuthTokenCallback : CBBase<WebObjects.Facebook.FetchOauthTokenResponse>
		{
			protected Facebook facebook;

			public FetchFacebookOAuthTokenCallback(Roar.Callback<Roar.WebObjects.Facebook.FetchOauthTokenResponse> in_cb, Facebook in_facebook)
				: base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void HandleError(Roar.RequestResult nn)
			{
				if(Debug.isDebugBuild)
					Debug.Log("OAuth Fetch Failed " + nn.msg);
			}

			public override void HandleSuccess(CallbackInfo<WebObjects.Facebook.FetchOauthTokenResponse> info)
			{
				if(Debug.isDebugBuild)
					Debug.Log("oauth successful "+info.data.ToString()+info.msg);

				string oauthToken = info.data.oauth_token ;
				facebook.SetOAuthToken(oauthToken);
				facebook.DoPostLoginAction();
			}
		}

		class LoginOAuthCallback : CBBase<WebObjects.Facebook.LoginOauthResponse>
		{
			protected Facebook facebook;

			public LoginOAuthCallback (Roar.Callback<WebObjects.Facebook.LoginOauthResponse> in_cb, Facebook in_facebook) : base( in_cb )
			{
				facebook = in_facebook;
			}

			public override void HandleError ( Roar.RequestResult  info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.LoginOauthResponse> info)
			{
				facebook.isLoggedinViaFacebook = true;
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
			}
		}
		
		class LoginSignedCallback : CBBase<WebObjects.Facebook.LoginSignedResponse>
		{
			protected Facebook facebook;

			public LoginSignedCallback (Roar.Callback<WebObjects.Facebook.LoginSignedResponse> in_cb, Facebook in_facebook) : base( in_cb )
			{
				facebook = in_facebook;
			}

			public override void HandleError (Roar.RequestResult nn)
			{
				RoarManager.OnLogInFailed (nn.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.LoginSignedResponse> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
			}
		}

		public void DoWebplayerBind(Roar.Callback<WebObjects.Facebook.BindOauthResponse> cb)
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				requestedBindCB = cb;
				postLoginAction = PostLogionAction.BindRoar;
				AttemptFacebookLoginChain();
			}
			else
			{
				Roar.WebObjects.Facebook.BindOauthArguments args = new Roar.WebObjects.Facebook.BindOauthArguments();
				args.oauth_token = oAuthToken;
				facebook.bind_oauth(args, new FacebookBindCallback (cb, this));
			}
		}

		public void DoWebplayerCreate (string name, Roar.Callback<WebObjects.Facebook.CreateOauthResponse> cb)
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				createoAuthCB = cb;
				postLoginAction = PostLogionAction.CreateRoar;
				requestedName = name;
				AttemptFacebookLoginChain();
			}
			else
			{
				if (name == "" || oAuthToken == "" || oAuthToken == null) {
					logger.DebugLog ("[roar] -- Must specify username and signed req for creation");
					return;
				}

				Roar.WebObjects.Facebook.CreateOauthArguments args = new Roar.WebObjects.Facebook.CreateOauthArguments();
				args.name = name;
				args.oauth_token = oAuthToken;
				facebook.create_oauth(args, new FacebookCreateCallback (cb, this));
			}
		}

		public void DoPostLoginAction ( )
		{
			switch(postLoginAction)
			{
			case PostLogionAction.CreateRoar:
				DoWebplayerCreate(requestedName, createoAuthCB);
				break;

			case PostLogionAction.LoginRoar:
				DoWebplayerLogin(loginCB);
				break;

			case PostLogionAction.BindRoar:
				DoWebplayerBind(requestedBindCB);
				break;

			case PostLogionAction.Nothing:
				break;

			}
		}

		// ---- Original Methods ----
		// ------------------------

		public void LoginOAuth (string oauth_token, Roar.Callback<WebObjects.Facebook.LoginOauthResponse> cb)
		{
			if (oauth_token == "") {
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook login");
				return;
			}

			Roar.WebObjects.Facebook.LoginOauthArguments args = new Roar.WebObjects.Facebook.LoginOauthArguments();
			args.oauth_token = oauth_token;
			facebook.login_oauth (args, new LoginOAuthCallback (cb, this));
		}

		public void BindUserOAuth (string oauth_token, Roar.Callback<WebObjects.Facebook.BindOauthResponse> cb)
		{
			if (oauth_token == "")
			{
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook binding");
				return;
			}
			Roar.WebObjects.Facebook.BindOauthArguments args = new Roar.WebObjects.Facebook.BindOauthArguments();
			args.oauth_token = oauth_token;
			facebook.bind_oauth (args, new BindOAuthCallback (cb));
		}
		
		class BindOAuthCallback : CBBase<WebObjects.Facebook.BindOauthResponse>
		{
			public BindOAuthCallback (Roar.Callback<WebObjects.Facebook.BindOauthResponse> in_cb) : base(in_cb)
			{
			}
			
			public override void HandleError (Roar.RequestResult info)
			{
				RoarManager.OnFacebookBindUserOAuthFailed ();
			}
			
			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.BindOauthResponse> info)
			{
				RoarManager.OnFacebookBindUserOAuth ();
			}
		}
		
		public void BindUserSigned (string signed_request, Roar.Callback<WebObjects.Facebook.BindSignedResponse> cb)
		{
			if (signed_request == "")
			{
				logger.DebugLog ("[roar] -- Must specify signed request for facebook binding");
				return;
			}
			Roar.WebObjects.Facebook.BindSignedArguments args = new Roar.WebObjects.Facebook.BindSignedArguments();
			args.signed_request = signed_request;
			facebook.bind_signed (args, new BindSignedCallback (cb));
		}
		
		class BindSignedCallback : CBBase<WebObjects.Facebook.BindSignedResponse>
		{
			public BindSignedCallback (Roar.Callback<WebObjects.Facebook.BindSignedResponse> in_cb) : base(in_cb)
			{
			}
			
			public override void HandleError (Roar.RequestResult info)
			{
				RoarManager.OnFacebookBindUserSignedFailed ();
			}
			
			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.BindSignedResponse> info)
			{
				RoarManager.OnFacebookBindUserSigned ();
			}
		}


		public void CreateOAuth (string name, string oAuthToken, Roar.Callback<WebObjects.Facebook.CreateOauthResponse> cb)
		{
			if (name == "" || oAuthToken == "") {
				logger.DebugLog ("[roar] -- Must specify username and oauthToken for creation");
				return;
			}
			
			Roar.WebObjects.Facebook.CreateOauthArguments args = new Roar.WebObjects.Facebook.CreateOauthArguments();
			args.name = name;
			args.oauth_token = oAuthToken;

			facebook.create_oauth (args, new CreateOAuthCallback (cb));
		}


		protected class CreateOAuthCallback : CBBase<WebObjects.Facebook.CreateOauthResponse>
		{

			public CreateOAuthCallback (Roar.Callback<WebObjects.Facebook.CreateOauthResponse> in_cb) : base(in_cb)
			{
			}

			public override void HandleError ( Roar.RequestResult info)
			{
				RoarManager.OnCreateUserFailed (info.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.CreateOauthResponse> info)
			{
				RoarManager.OnCreatedUser ();
				RoarManager.OnLoggedIn ();
			}
		}

		public void ShopList (Roar.Callback<WebObjects.Facebook.ShopListResponse> cb)
		{
			Roar.WebObjects.Facebook.ShopListArguments args = new Roar.WebObjects.Facebook.ShopListArguments();
			facebook.shop_list (args, new ShopListCallback (cb));
		}

		protected class ShopListCallback : CBBase<WebObjects.Facebook.ShopListResponse>
		{
			public ShopListCallback (Roar.Callback<WebObjects.Facebook.ShopListResponse> in_cb) : base(in_cb)
			{
			}

			public override void HandleError (Roar.RequestResult info)
			{
				RoarManager.OnFacebookShopListFailed (info.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.ShopListResponse> info)
			{
				RoarManager.OnFacebookShopList (info.data.shop_list);
			}
		}

		public void SetOAuthToken(string oauth_token)
		{
			if(oauth_token != "") { oAuthToken = oauth_token; }
		}
		
		public bool IsLoggedIn()
		{
			if(oAuthToken == null || oAuthToken == "")
				return false;
			else
				return true;
			
		}

		public bool IsLoggedInViaFacebook()
		{
			return isLoggedinViaFacebook;
		}

		//Facebook Shop stuff
		public void FetchShopData(Roar.Callback<IDictionary<string,DomainObjects.FacebookShopEntry>> callback)
		{
			dataStore.facebookShop.Fetch(callback);
		}

		public bool HasDataFromServer { get { return dataStore.facebookShop.HasDataFromServer; } }

		public IList<DomainObjects.FacebookShopEntry> List ()
		{
			return dataStore.facebookShop.List();
		}

		/**
		* Function that is called to tell the hosting iframe to passback the signed request string if available.
		* Must be called before using the signed request string.
		*
		*
		**/

		public void RequestFacebookSignedRequest()
		{
			if (Debug.isDebugBuild)
				Debug.Log("Requesting signed request");
			Application.ExternalCall("sendSignedRequest");
		}

		/**
		* Function that is called to ask the hosting browser to pass the get parameter code give by facebook
		* If no get parameter code is available will return blank and a graph authorization url will have to be requested.
		*
		*
		**/
		public void RequestFacebookGetCode()
		{
			if (Debug.isDebugBuild)
				Debug.Log("R	equesting get code");
			Application.ExternalCall("returnCodeIfAvailable");
		}

		/**
		* Request a state parameter in case of a facebook redirect
		*
		**/
		public void RequestFacebookStatePara()
		{
			Application.ExternalCall("returnSateIfAvailable");
		}

	}

}
