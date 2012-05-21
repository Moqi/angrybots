using UnityEngine;
using System.Collections;


/**
 * This class is a fake! you should replace it with a real version that does whatyou need.
 * It's modeled closely on the prime31 Social Networking plugins interface.
 */

public class FacebookBinding : MonoBehaviour
{
	static bool logged_in = false;
	
	public static void init( string applicationId )
	{
		Debug.Log("FacebookBinding.init called with "+applicationId);
	}
	
	public static string getAccessToken()
	{
		if( ! logged_in )
		{
			Debug.LogError("FacebookBinding.getAccessToken used when not logged in");
			return "invalid";
		}
		
		Debug.Log ("FacebookBinding.getAccessToken called" );
		return "abefbedb123123b123abda_facebook_";
	}
	
	public static void login()
	{
		logged_in = true;
		Debug.Log ("FacebookBinding.login called");
		FacebookManager.OnLogin();
	}
	
	public static void logout()
	{
		logged_in = false;
		Debug.Log ("FacebookBinding.logout called");
		FacebookManager.OnLogout();
	}

}

