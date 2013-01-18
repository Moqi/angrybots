using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class Facebook : IFacebook
	{
		protected IDataStore dataStore;
		IWebAPI.IFacebookActions facebook;
		ILogger logger;

		public Facebook (IWebAPI.IFacebookActions facebookActions, IDataStore dataStore, ILogger logger)
		{
			this.facebook = facebookActions;
			this.dataStore = dataStore;
			this.logger = logger;
		}

		// ---- Access Methods ----
		// ------------------------

		public void LoginOAuth (string oauth_token, Roar.Callback<WebObjects.Facebook.LoginOauthResponse> cb)
		{
			if (oauth_token == "") {
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook login");
				return;
			}
			
			Roar.WebObjects.Facebook.LoginOauthArguments args = new Roar.WebObjects.Facebook.LoginOauthArguments();
			args.oauth_token = oauth_token;

			facebook.login_oauth (args, new LoginOAuthCallback (cb));
		}

		class LoginOAuthCallback : CBBase<WebObjects.Facebook.LoginOauthResponse>
		{
			public LoginOAuthCallback (Roar.Callback<WebObjects.Facebook.LoginOauthResponse> in_cb) : base( in_cb )
			{
			}

			public override void HandleError ( Roar.RequestResult  info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}

			public override void HandleSuccess (CallbackInfo<WebObjects.Facebook.LoginOauthResponse> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
			}
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

	}

}
