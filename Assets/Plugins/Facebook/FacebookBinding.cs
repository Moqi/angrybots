using UnityEngine;
using System.Collections;


/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookBinding : MonoBehaviour
{
	public string applicationID;
	static bool isLoggedIn = false;
	static bool isAuthorized = false;
	static string codeParameter = null; //when using oauth this parameter is passed via a get parameter.
    static string oAuthToken = null;

	static IRoar roar;

	public static void Init( string applicationId )
	{
		Debug.Log("FacebookBinding.Init called with "+applicationId);
		//this application id can be derived either from a variable in the unity3d script (entered in the inspector)
		//or can be retrieved from the webapi since there is an option to enter it in the admin panel.

		roar = DefaultRoar.Instance;
	}

	static void onLogin(Roar.CallbackInfo info)
	{
		Debug.Log("facebook binding login");
		Debug.Log(info.msg.ToString());

	}

	

	/**
	 * Creates a user based on the facebook oauth with the requested username.
	 *
	 *
	 * @param Requested name.
	 **/
	public static void CreateFacebookOAuth(string requestedName)
	{
		roar = DefaultRoar.Instance;
		if(oAuthToken != null)
		{

			//roar.CreateFacebookOAuthToken(requestedName, oAuthToken);
		}
	}

	

	
	
    // This function should trigger the whole login sequence.
	public static void Login()
	{
		isLoggedIn = true;
		Debug.Log ("FacebookBinding.login called");
		FacebookManager.OnLogin();
	}

	public static void Logout()
	{
		isLoggedIn = false;
		Debug.Log ("FacebookBinding.logout called");
		FacebookManager.OnLogout();
	}

}

