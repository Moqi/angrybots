using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{
	public class Facebook : IFacebook
	{
		protected DataStore dataStore;
		IWebAPI.IFacebookActions facebook;
		ILogger logger;

        string oAuthToken = null;
		
		enum PostLogionAction
		{
			Nothing,
			LoginRoar,
			CreateRoar,
			BindRoar,
		}
        PostLogionAction postLoginAction;
		string requestedName;
		Roar.Callback requestedCb;

        
		public Facebook (IWebAPI.IFacebookActions facebookActions, DataStore dataStore, ILogger logger)
		{
			this.facebook = facebookActions;
			this.dataStore = dataStore;
			this.logger = logger;

			// -- Event Watchers
			// Flush models on logout
			RoarManager.loggedOutEvent += () => {
				dataStore.Clear (true); };

			// Watch for initial inventory ready event, then watch for any
			// subsequent `change` events
			RoarManager.inventoryReadyEvent += () => CacheFromInventory ();
		}

		protected class FacebookCreateCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public FacebookCreateCallback (Roar.Callback in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnCreateUserFailed (info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
                FacebookManager.OnFacebookPlayerCreated();
				RoarManager.OnCreatedUser ();
				RoarManager.OnLoggedIn ();
				return null;
			}
		}

        protected class FacebookBindCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public FacebookBindCallback (Roar.Callback in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
                Debug.Log("Facebook binding failed "+info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
                FacebookManager.OnFacebookBindSuccessful();
                Debug.Log("binding succeeded");
				return null;
			}
		}


		// ---- Access Methods ----
		// ------------------------


		protected class LoginCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public LoginCallback (Roar.Callback in_cb, Facebook in_facebook) : base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLogInFailed (info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
				return null;
			}
		}

        public void FacebookGraphRedirect(string redirectURL)
        {
            Debug.Log("redirecting because of a blank code");
			Application.OpenURL("https://graph.facebook.com/oauth/authorize?client_id="+DefaultRoar.Instance.facebookApplicationID+"&redirect_uri="+redirectURL);

        }

        public void FetchOAuthToken(string codeParameter)
        {
            DoFetchFacebookOAuthToken(codeParameter, null);

        }
		
		public void AttemptFacebookLoginChain()
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				DefaultRoar.FacebookLoginOptions facebookLoginOptions = DefaultRoar.Instance.facebookLoginOptions;
            
				if(facebookLoginOptions != DefaultRoar.FacebookLoginOptions.ExternalOauthOnly && 
	            facebookLoginOptions != DefaultRoar.FacebookLoginOptions.InternalNonjavascriptLoginNOT_IMPLEMENTED)
		        {
		            RequestFacebookSignedRequest();
		        }
	            else	            
		        if(facebookLoginOptions != DefaultRoar.FacebookLoginOptions.ExternalOauthOnly && 
		            facebookLoginOptions != DefaultRoar.FacebookLoginOptions.InternalNonjavascriptLoginNOT_IMPLEMENTED &&
		            facebookLoginOptions != DefaultRoar.FacebookLoginOptions.SignedRequestOnly)
		        {
		
		            RequestFacebookGetCode();
		        }
				
			}
			
		}
		
		public void DoMainLogin(Roar.Callback callback)
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				requestedCb = callback;
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
            Debug.Log("Signed request failed");
			if(DefaultRoar.Instance.facebookLoginOptions != DefaultRoar.FacebookLoginOptions.SignedRequestOnly)
            {
			    RequestFacebookGetCode();
            }
            else
            {
                FacebookManager.OnLoginFailed("Signed request login failed and no further login options are available");
            }
			
		}

		public void DoLoginFacebookOAuth (Roar.Callback cb)
		{
			if (oAuthToken == "") {
				logger.DebugLog ("[roar] -- Must specify oauth_token for facebook login");
				return;
			}

			Hashtable args = new Hashtable ();
			args ["oauth_token"] = oAuthToken;

			facebook.login_oauth (args, new LoginFacebookOAuthCallback (cb, this));
		}

		public void DoLoginFacebookSignedReq (string signedReq, Roar.Callback cb)
		{
			if (signedReq == "") {
				logger.DebugLog ("[roar] -- Must specify signedReq for facebook login");
				return;
			}

			Hashtable args = new Hashtable ();
			args ["signed_request"] = signedReq;

			facebook.login_signed (args, new LoginCallback (cb, this));
		}

		public void DoFetchFacebookOAuthToken (string code, Roar.Callback cb)
		{
			Hashtable args = new Hashtable ();
			args ["code"] = code;
			facebook.fetch_oauth_token(args, new FetchFacebookOAuthTokenCallback (cb, this));
		}


		class FetchFacebookOAuthTokenCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public FetchFacebookOAuthTokenCallback(Roar.Callback in_cb, Facebook in_facebook)
				: base(in_cb)
			{
				facebook = in_facebook;
			}

			public override void OnFailure(CallbackInfo<IXMLNode> info)
			{
				Debug.Log("OAuth Fetch Failed " + info.msg);
				
			}

			public override object OnSuccess(CallbackInfo<IXMLNode> info)
			{
				Debug.Log("oauth successful "+info.data.Children.ToString()+info.msg);
				Debug.Log("oauth successful "+info.data.GetFirstChild("roar").GetFirstChild("facebook").GetFirstChild("fetch_oauth_token").GetFirstChild("oauth_token").Text );

                string oauthToken = info.data.GetFirstChild("roar").GetFirstChild("facebook").GetFirstChild("fetch_oauth_token").GetFirstChild("oauth_token").Text ;
                facebook.SetOAuthToken(oauthToken);
				facebook.DoPostLoginAction();
				
				return info.data.GetFirstChild("roar").GetFirstChild("facebook").GetFirstChild("fetch_oauth_token").GetFirstChild("oauth_token").Text ;
				
			}
		}


		class LoginFacebookOAuthCallback : SimpleRequestCallback<IXMLNode>
		{
			protected Facebook facebook;

			public LoginFacebookOAuthCallback (Roar.Callback in_cb, Facebook in_facebook) : base( in_cb )
			{
				facebook = in_facebook;
			}

			public override void OnFailure (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLogInFailed (info.msg);
                FacebookManager.OnLoginFailed(info.msg);
			}

			public override object OnSuccess (CallbackInfo<IXMLNode> info)
			{
				RoarManager.OnLoggedIn ();
				// @todo Perform auto loading of game and player data
				return null;
			}
		}


        public void DoBindFacebook(Roar.Callback cb)
        {
			
			if(oAuthToken == null || oAuthToken == "")
			{
				requestedCb = cb;
				postLoginAction = PostLogionAction.BindRoar;
				
				AttemptFacebookLoginChain();
					
			}
			else
			{
			
			
				Hashtable args = new Hashtable ();
				args ["oauth_token"] = oAuthToken;

				facebook.bind_oauth(args, new FacebookBindCallback (cb, this));
			}

        }

		public void DoCreateFacebook (string name,  Roar.Callback cb)
		{
			if(oAuthToken == null || oAuthToken == "")
			{
				requestedCb = cb;
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
	
				Hashtable args = new Hashtable ();
				args ["name"] = name;
				args ["oauth_token"] = oAuthToken;
	
				facebook.create_oauth(args, new FacebookCreateCallback (cb, this));
			}
		}
		
		public void DoPostLoginAction ( )
		{
            switch(postLoginAction)
			{
			case PostLogionAction.CreateRoar:
				DoCreateFacebook(requestedName, requestedCb);
				break;
				
			case PostLogionAction.LoginRoar:
				DoLoginFacebookOAuth(requestedCb);
				
				break;
				
			case PostLogionAction.BindRoar:
				DoBindFacebook(requestedCb);
				break;
				
			case PostLogionAction.Nothing:
				
				break;
				
				
			}
			
		}

		//TODO: not sure this belongs in this class!
		public void CacheFromInventory (Roar.Callback cb=null)
		{
			if (! dataStore.inventory.HasDataFromServer)
				return;

			// Build sanitised ARRAY of ikeys from Inventory.list()
			var l = dataStore.inventory.List ();
			var ikeyList = new ArrayList ();
			for (int i=0; i<l.Count; i++)
				ikeyList.Add ((l [i] as Hashtable) ["ikey"]);

			var toCache = dataStore.cache.ItemsNotInCache (ikeyList) as ArrayList;

			// Build sanitised Hashtable of ikeys from Inventory
			// No need to call server as information is already present
			Hashtable cacheData = new Hashtable ();
			for (int i=0; i<toCache.Count; i++) {
				for (int k=0; k<l.Count; k++) {
					// If the Inventory ikey matches a value in the
					// list of items to cache, add it to our `cacheData` obj
					if ((l [k] as Hashtable) ["ikey"] == toCache [i])
						cacheData [toCache [i]] = l [k];
				}
			}

			// Enable update of cache if it hasn't been initialised yet
			dataStore.cache.HasDataFromServer = true;
			dataStore.cache.Set (cacheData);
		}

        public void SetOAuthToken(string oauth_token)
        {
            Debug.Log("Set oauth token "+oauth_token);
            if(oauth_token != "")
                oAuthToken = oauth_token;
            FacebookManager.OnOAuthTokenReady();
        }


         /**
         * Function that is called to tell the hosting iframe to passback the signed request string if available.
         * Must be called before using the signed request string.
         *
         *
         **/
        public void RequestFacebookSignedRequest()
        {
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
		    Application.ExternalCall("returnCodeIfAvailable");
	    }

	}

}
