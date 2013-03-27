using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoarFacebookBindWidget : RoarUIWidget
{
	public string status = "";
	public string facebookApplicationID = "";
	public string buttonStyle="DefaultButton";

	public float buttonWidth = 300;
	public float buttonHeight = 90;

	protected override void OnEnable ()
	{
		if(roar.Facebook.IsLoggedInViaFacebook())
			enabled = false;
	}

	protected override void DrawGUI(int windowId)
	{
		if(networkActionInProgress)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Working", "StatusNormal");
			ScrollViewContentHeight = contentBounds.height;
		}
		else
		{
			GUI.Label(new Rect(0, 0, contentBounds.width, contentBounds.height/3), status);

			if(roar.Facebook.IsLoggedIn())
			{
				GUI.Label(new Rect(0, contentBounds.height/3, contentBounds.width, contentBounds.height/3), 	"Logged into facebook");
			}
			else
			{
				if(GUI.Button(new Rect(contentBounds.width/2 - buttonWidth/2, contentBounds.height/3, buttonWidth, buttonHeight), "Login Facebook", buttonStyle))
				{
					roar.Facebook.DoWebplayerLogin(OnRoarFacebookLoginComplete);
				}
			}

			if (GUI.Button(new Rect(contentBounds.width/2 - buttonWidth/2,contentBounds.height/3*2, buttonWidth, buttonHeight), "Bind Facebook", buttonStyle))
			{
				status = "Logging in through facebook...";
				networkActionInProgress = true;
				roar.Facebook.DoWebplayerBind(OnRoarFacebookBindComplete);
			}
		}
		GUI.enabled = true;
	}

	void OnRoarFacebookFetchOauthTokenComplete(Roar.CallbackInfo<Roar.WebObjects.Facebook.FetchOauthTokenResponse> info)
	{
		if(info.code == IWebAPI.OK)
		{
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
			networkActionInProgress = false;
			break;
		case 3: // Invalid name or password
		default:
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}

	void OnRoarFacebookBindComplete(Roar.CallbackInfo<Roar.WebObjects.Facebook.BindOauthResponse> info)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("OnRoarBindComplete ({0}): {1}", info.code, info.msg));
			if( info.data!= null)
			{
			}
			else
			{
				Debug.Log("OnRoarBindComplete got null data");
			}
		}

		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			this.enabled = false;
			// fetch the player's properties after successful login
			networkActionInProgress = false;
			break;

		case 3: // Invalid name or password
			status = info.msg;
			break;
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
			// fetch the player's properties after successful login
			networkActionInProgress = false;
			break;

		case 3: // Invalid name or password
		default:
			//isError = true;
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
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
		Debug.Log("got code get para"+paras);
		if(paras.Split(' ')[0] == "")
		{
			//Invoke redirect with authorization.
			//fire event that says we are redirecting to login/authorize.
			roar.Facebook.FacebookGraphRedirect(facebookApplicationID, "", paras.Split(' ')[1]);
			return;
		}

		Debug.Log("got string para");
		Debug.Log("string is "+paras);
		string codeParameter = paras.Split(' ')[0];

		roar.Facebook.FetchOAuthToken(codeParameter, OnRoarFacebookFetchOauthTokenComplete);
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
			roar.Facebook.SignedRequestFailed();
			//fire signed request event failed. go for the graph api method.
		}
		else
		{
			roar.Facebook.SetOAuthToken(oAuth);
			roar.Facebook.DoPostLoginAction();
		}
	}

	#endregion

}
