using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarLoginWidget : RoarUIWidget
{
	public delegate void RoarLoginWidgetHandler();
	public static event RoarLoginWidgetHandler OnFullyLoggedIn; // logged in & stats fetched
	protected Roar.Components.ITasks tasks;
	protected bool isFetching=false;
	
	public bool enableOnAwake = true;
	public bool fetchPropertiesOnLogin = true;
	public bool saveUsername = true;
	public bool savePassword = false;
	
	private static string KEY_USERNAME = "roar-username";
	private static string KEY_PASSWORD = "roar-password";
	
	private string status = "Supply a username and password to log in or to register a new account.";
	private string username = string.Empty;
	private string password = string.Empty;
	public int loginBoxSpacing = 20;
	public float labelHeight= 25;
	public float statusWidth = 400;
	public float fieldHeight = 50;
	public float fieldWidth = 280;
	public float buttonWidth = 150;
	public float buttonHeight = 40;
	public float footerSpacing = 50;
	public float buttonSpacing = 10;
	public string buttonStyle="DefaultButton";
	public string boxStyle="DefaultTextArea";
	public string loginLabelStyle = "DefaultHeavyContentLeftText";
	public string statusStyle = "DefaultSmallStatusText";
	
	public bool allowFacebookLogin = true;
	public string facebookApplicationID = "";
	bool lastRequestLogin; //true if the last request was a login facebook request, else it was a create facebook request
	enum SecondaryLogin
	{
		None,
		Facebook,
		Google,
	};

	SecondaryLogin secondaryLogin = SecondaryLogin.None;
	protected override void Awake ()
	{
		base.Awake ();
		if(enableOnAwake)
			enabled = true;
	}
	
	void Reset ()
	{
		displayName = "Login Widget";
		bounds.width = 512;
		bounds.height = 450;
	}

	protected override void OnEnable ()
	{
		if (saveUsername)
		{
			username = PlayerPrefs.GetString(KEY_USERNAME, string.Empty);
		}
		if (savePassword)
		{
			password = PlayerPrefs.GetString(KEY_PASSWORD, string.Empty);
		}
		
		
	}
	
		
	protected override void DrawGUI(int windowId)
	{
		float totalSpaceAvailableY = contentBounds.height; //the total space available between header and footer.
		
		totalSpaceAvailableY = totalSpaceAvailableY - 2*fieldHeight - 2* labelHeight - loginBoxSpacing - footerSpacing;
		

		Rect currentRect = new Rect(contentBounds.width/2 - fieldWidth/2, totalSpaceAvailableY/2, fieldWidth, labelHeight);

		if(networkActionInProgress)
			GUI.enabled = false;
		GUI.Label(currentRect, "Username", loginLabelStyle);
		currentRect.y += labelHeight;
		currentRect.width = fieldWidth;
		currentRect.height = fieldHeight;
		username = GUI.TextField(currentRect, username, boxStyle);
		currentRect.y+=fieldHeight;
		currentRect.y += loginBoxSpacing;
		currentRect.width = fieldWidth;
		currentRect.height = labelHeight;
		
		if(secondaryLogin == SecondaryLogin.None)
		{
			GUI.Label(currentRect, "Password", loginLabelStyle);
			currentRect.y += labelHeight;
			currentRect.width = fieldWidth;
			currentRect.height = fieldHeight;
			password = GUI.PasswordField(currentRect, password, '*', boxStyle);
			currentRect.y += fieldHeight;
			currentRect.y += labelHeight;
		}

		currentRect.x = 0;
		currentRect.width = contentBounds.width;
		GUI.Label(currentRect, status, statusStyle);
		currentRect.y = contentBounds.height - footerSpacing;
		GUI.Box(new Rect(0, contentBounds.height - footerSpacing, contentBounds.width, contentBounds.height), new GUIContent(""), "DefaultFooterStyle");
		currentRect.width = buttonWidth;
		currentRect.height = buttonHeight;
		currentRect.x = contentBounds.width - buttonWidth - buttonSpacing;
		currentRect.y = contentBounds.height - buttonHeight/2 - footerSpacing/2;

		if(secondaryLogin == SecondaryLogin.None)
		{
			GUI.enabled = username.Length > 0 && password.Length > 0 && !networkActionInProgress;
			if ((GUI.Button(currentRect, "Log In", buttonStyle) || ( Event.current.keyCode == KeyCode.Return)) && !networkActionInProgress)
			{

				status = "Logging in...";
				networkActionInProgress = true;
				if (saveUsername)
				{
					PlayerPrefs.SetString(KEY_USERNAME, username);
				}
				if (savePassword)
				{
					PlayerPrefs.SetString(KEY_PASSWORD, password);
				}
				if (Debug.isDebugBuild)
					Debug.Log(string.Format("[Debug] Logging in as [{0}] with password [{1}].", username, password));

				roar.User.Login(username, password, OnRoarLoginComplete);
			}
			currentRect.x -= buttonWidth + buttonSpacing;

			if (GUI.Button(currentRect, "Create", buttonStyle) && !networkActionInProgress)
			{
				status = "Creating new player account...";
				networkActionInProgress = true;
				roar.User.Create(username, password, OnRoarAccountCreateComplete);
			}
			GUI.enabled = true;

			if(allowFacebookLogin)
			{
				currentRect.x -= buttonWidth + buttonSpacing;

				if (GUI.Button(currentRect, "Facebook", buttonStyle))
				{
					drawSubheading = true;
					subheaderName = "Facebook";
					status = "Click 'Login' to log in through facebook or supply a username and click 'Create' to create account through facebook";
					secondaryLogin = SecondaryLogin.Facebook;
				}
			}
			currentRect.y+= buttonSpacing + buttonHeight;
		}

		if(secondaryLogin == SecondaryLogin.Facebook)
		{

			if ((GUI.Button(currentRect, "Log In", buttonStyle) || ( Event.current.keyCode == KeyCode.Return)) && !networkActionInProgress)
			{
				lastRequestLogin = true;
				status = "Logging in through facebook...";
				networkActionInProgress = true;
				roar.Facebook.DoWebplayerLogin(OnRoarFacebookLoginComplete);
			}
			currentRect.x -= buttonWidth + buttonSpacing;

			GUI.enabled = username.Length > 0 && !networkActionInProgress;

			if (GUI.Button(currentRect, "Create", buttonStyle) && !networkActionInProgress)
			{
				lastRequestLogin = false;
				status = "Logging in to Facebook...";
				networkActionInProgress = true;
				roar.Facebook.DoWebplayerCreate(username, OnRoarFacebookCreateComplete);
			}

			currentRect.x -= buttonWidth + buttonSpacing;

			if (GUI.Button(currentRect, "Back", buttonStyle))
			{
				status = "Supply a username and password to log in or to register a new account";
				drawSubheading = false;
				secondaryLogin = SecondaryLogin.None;
			}

			currentRect.y+= buttonSpacing + buttonHeight;
			GUI.enabled = true;
		}
		
		
		GUI.enabled = true;
	}
	
	public void Fetch()
	{
		isFetching = true;
		tasks.Fetch(OnRoarFetchTasksComplete);
	}
	
	void OnRoarFetchTasksComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Task> > data )
	{
		isFetching = false;
	}
	
	void OnRoarFacebookFetchOauthTokenComplete(Roar.CallbackInfo<Roar.WebObjects.Facebook.FetchOauthTokenResponse> info)
	{
		if(info.code == IWebAPI.OK)
		{
			if (Debug.isDebugBuild)
				Debug.Log("oauth fetched successfully");
			networkActionInProgress = false;
		}
		else
		{
			networkActionInProgress = false;
			status = "Error, "+info.msg;
		}
	}

	void OnRoarFacebookLoginComplete(Roar.CallbackInfo<Roar.WebObjects.Facebook.LoginOauthResponse> info)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("OnRoarLoginComplete ({0}): {1}", info.code, info.msg));
			if( info.data!= null)
			{
			Debug.Log( string.Format("OnRoarLoginComplete got auth_token {0}", info.data.auth_token ) );
			Debug.Log( string.Format("OnRoarLoginComplete got player_id {0}",  info.data.player_id ) );
			}
			else
			{
				Debug.Log("OnRoarLogingComplete got null data");
			}
		}
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			// fetch the player's properties after successful login
			this.enabled = false;
			if (fetchPropertiesOnLogin)
			{
				DefaultRoar.Instance.Properties.Fetch(OnRoarPropertiesFetched);
			}
			else
			{
				networkActionInProgress = false;
			}

			break;
		case 3: // Invalid name or password
		default:
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}

	void OnRoarFacebookCreateComplete(Roar.CallbackInfo<Roar.WebObjects.Facebook.CreateOauthResponse> info)
	{
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			this.enabled = false;
			// fetch the player's properties after successful login
			if (fetchPropertiesOnLogin)
			{
				DefaultRoar.Instance.Properties.Fetch(OnRoarPropertiesFetched);
			}
			else
			{
				networkActionInProgress = false;
			}
			break;
		case 3: // Invalid name or password
		default:
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}
	void OnRoarLoginComplete(Roar.CallbackInfo<Roar.WebObjects.User.LoginResponse> info)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("OnRoarLoginComplete ({0}): {1}", info.code, info.msg));
			if( info.data!= null)
			{
			Debug.Log( string.Format("OnRoarLoginComplete got auth_token {0}", info.data.auth_token ) );
			Debug.Log( string.Format("OnRoarLoginComplete got player_id {0}",  info.data.player_id ) );
			}
			else
			{
				Debug.Log("OnRoarLogingComplete got null data");
			}
		}
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			this.enabled = false;
			// fetch the player's properties after successful login
			if (fetchPropertiesOnLogin)
			{
				DefaultRoar.Instance.Properties.Fetch(OnRoarPropertiesFetched);
			}
			else
			{
				
				networkActionInProgress = false;
			}
			
			break;
		case 3: // Invalid name or password
		default:
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}

	void OnRoarAccountCreateComplete(Roar.CallbackInfo<Roar.WebObjects.User.CreateResponse> info)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("OnRoarAccountCreateComplete ({0}): {1}", info.code, info.msg));
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			status = "Account successfully created. You can now log in.";
			break;
		case 3:
		default:
			status = info.msg;
			break;
		}
		networkActionInProgress = false;
	}
	
	void OnRoarPropertiesFetched(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.PlayerAttribute> > info)
	{
		networkActionInProgress = false;
		if (OnFullyLoggedIn != null) OnFullyLoggedIn();
	}

	#region JAVASCRIPT CALLBACK
	/**
	* Function that is called from javascript and is handed the facebook code parameter. Call graph.authorize with this.
	*
	*
	* @param code is the get parameter picked up from facebook 'GET'. Can be null.
	**/
	void CatchCodeGetPara(string paras)
	{
		if(paras.Split(' ')[0] == "")
		{
			//Invoke redirect with authorization.
			//fire event that says we are redirecting to login/authorize.
			if(lastRequestLogin)
				roar.Facebook.FacebookGraphRedirect(facebookApplicationID, "LoginFacebookOnLoad", paras.Split(' ')[1]);
			else
				roar.Facebook.FacebookGraphRedirect(facebookApplicationID, "CreateFacebookOnLoad", paras.Split(' ')[1]);

			return;
		}

		string codeParameter = paras.Split(' ')[0];

		roar.Facebook.FetchOAuthToken(codeParameter, OnRoarFacebookFetchOauthTokenComplete);
	}

	/**
	* Function that is called from javascript and is handed the facebook state parameter
	*
	*
	* @param code is the get parameter picked up from facebook 'GET'. Can be null.
	**/
	void CatchStatePara(string paras)
	{
		if (Debug.isDebugBuild)
			Debug.Log("caught state para"+paras);

		if(paras == "")
		{
			return;
		}
		else
		{
			if(paras == "LoginFacebookOnLoad")
			{
				status = "Logging in through facebook...";
				networkActionInProgress = true;
				roar.Facebook.DoWebplayerLogin(OnRoarFacebookLoginComplete);
			}

			if(paras == "CreateFacebookOnLoad")
			{
				if(username != null || username != "")
				{
					status = "Logging in to Facebook...";
					networkActionInProgress = true;
					roar.Facebook.DoWebplayerCreate(username, OnRoarFacebookCreateComplete);
				}
				else
					status = "Please specify a valid username when creating an account";
			}
		}
	}

	/**
	 * Function that is called from javascript and is passed the signedRequest string
	 *
	 *
	 * @param signedRequest is the actual signed request picked up from facebook 'POST'
	 **/
	void CatchFacebookRequest(string oAuth)
	{
		if (oAuth == "")
		{
			//fire signed request event failed. go for the graph api method.
			roar.Facebook.SignedRequestFailed();
		}
		else
		{
			roar.Facebook.SetOAuthToken(oAuth);
			roar.Facebook.DoPostLoginAction();
		}
	}
	#endregion
}
