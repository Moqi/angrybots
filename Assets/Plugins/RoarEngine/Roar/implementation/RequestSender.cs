using System.Collections;
using UnityEngine;

public class RequestSender : IRequestSender
{
	protected Roar.IConfig config;
	
	protected string GameKey { get { return config.Game; } }
	protected string RoarAuthToken { get { return config.AuthToken; } set { config.AuthToken = value; } }
	protected string RoarAPIUrl { get { return config.RoarAPIUrl; } }
	protected bool IsDebug { get { return config.IsDebug; } }
	
	protected IUnityObject unityObject;
	protected Roar.ILogger logger;

	public RequestSender( Roar.IConfig config, IUnityObject unityObject, Roar.ILogger logger )
	{
		this.config=config;
		this.unityObject = unityObject;
		this.logger = logger;
	}
		
	public void MakeCall( string apicall, Hashtable args, IRequestCallback cb, bool requires_auth_token )
	{
		unityObject.DoCoroutine( SendCore( apicall, args, cb, requires_auth_token ) );
	}

	protected void AddAuthToken( Hashtable args, WWWForm post )
	{
		// Note: we dont add it here if it is in args
		// as it will be added automatically by the addition of the args
		// This means that uses can override the auth_token at the
		// lowest level if they want to.
		if( args!=null && args.ContainsKey("auth_token") ) return;

		if( RoarAuthToken == null || RoarAuthToken == "" )
		{
			//TODO: Ugly that this is a System.Exception, and not some derived class.
			throw new System.Exception("No auth_token provided - you must be logged in to make this call." );
		}

		// Add the auth_token to the POST
		post.AddField( "auth_token", RoarAuthToken );
	}
	
	protected IEnumerator SendCore( string apicall, Hashtable args, IRequestCallback cb, bool requires_auth_token )
	{
		if ( GameKey == "")
		{
			logger.DebugLog("[roar] -- No game key set!--");
			yield break;
		}
	
		logger.DebugLog("[roar] -- Calling: "+apicall);
		
		// Encode POST parameters
		WWWForm post = new WWWForm();
		if(args!=null)
		{
			foreach (DictionaryEntry param in args)
			{
				//Debug.Log(string.Format("{0} => {1}", param.Key, param.Value));
				post.AddField( param.Key as string, param.Value as string );
			}
		}

		if( requires_auth_token )
		{
			AddAuthToken(args,post);
		}
		
		// Fire call sending event
		RoarManager.OnRoarNetworkStart();
		
		//Debug.Log ( "roar_api_url = " + RoarAPIUrl );
		if (Debug.isDebugBuild)
			Debug.Log ( "Requesting : " + RoarAPIUrl+GameKey+"/"+apicall+"/" );
		
		//NOTE: This is a work-around for unity not supporting zero length body for POST requests
		if ( post.data.Length == 0 )
		{
			post.AddField("dummy","x");
		}
		
		var xhr = new WWW( RoarAPIUrl+GameKey+"/"+apicall+"/", post);
		yield return xhr;
		
		OnServerResponse( xhr.text, apicall, cb );
	}
	
	
	protected void OnServerResponse( string raw, string apicall, IRequestCallback cb )
	{
		var uc = apicall.Split("/"[0]);
		var controller = uc[0];
		var action = uc[1];
		
		if (Debug.isDebugBuild)
			Debug.Log(raw);
		
		// Fire call complete event
		RoarManager.OnRoarNetworkEnd("no id");
		
		// -- Parse the Roar response
		// Unexpected server response 
		if ( raw==null || raw.Length==0 || raw[0] != '<')
		{
			// Error: fire the error callback
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
			System.Xml.XmlElement error = doc.CreateElement("error");
			doc.AppendChild(error);
			error.AppendChild(doc.CreateTextNode(raw));
			if (cb!=null)
			{
				cb.OnRequest(
					new Roar.RequestResult(
						RoarExtensions.CreateXmlElement("error",raw),
						IWebAPI.FATAL_ERROR,
						"Invalid server response"
					) );
			}
			return;
		}
	
		System.Xml.XmlElement root = RoarExtensions.CreateXmlElement(raw);
		
		int callback_code;
		string callback_msg="";
		
		System.Xml.XmlElement actionElement = root.SelectSingleNode( "/roar/"+controller+"/"+action ) as System.Xml.XmlElement;
		
		// Pre-process <server> block if any and attach any processed data
		System.Xml.XmlElement serverElement = root.SelectSingleNode( "/roar/server" ) as System.Xml.XmlElement;
		RoarManager.NotifyOfServerChanges( serverElement );
		
		// Status on Server returned an error. Action did not succeed.
		string status = actionElement.GetAttribute( "status" );
		if (status == "error")
		{
			callback_code = IWebAPI.UNKNOWN_ERR;
			callback_msg = actionElement.SelectSingleNode("error").InnerText;
			string server_error = (actionElement.SelectSingleNode("error") as System.Xml.XmlElement).GetAttribute("type");
			if ( server_error == "0" )
			{
				if (callback_msg=="Must be logged in") { callback_code = IWebAPI.UNAUTHORIZED; }
				if (callback_msg=="Invalid auth_token") { callback_code = IWebAPI.UNAUTHORIZED; }
				if (callback_msg=="Must specify auth_token") { callback_code = IWebAPI.BAD_INPUTS; }
				if (callback_msg=="Must specify name and hash") { callback_code = IWebAPI.BAD_INPUTS; }
				if (callback_msg=="Invalid name or password") { callback_code = IWebAPI.DISALLOWED; }
				if (callback_msg=="Player already exists") { callback_code = IWebAPI.DISALLOWED; }

				logger.DebugLog(string.Format("[roar] -- response error: {0} (api call = {1})", callback_msg, apicall));
			}
			
			// Error: fire the callback
			// NOTE: The Unity version ASSUMES callback = errorCallback
			if (cb!=null) cb.OnRequest( new Roar.RequestResult(root, callback_code, callback_msg) );
		}
		
		// No error - pre-process the result
		else
		{
			System.Xml.XmlElement auth_token = actionElement.SelectSingleNode(".//auth_token") as System.Xml.XmlElement;
			if (auth_token!=null && !string.IsNullOrEmpty(auth_token.InnerText)) RoarAuthToken = auth_token.InnerText;
			
			callback_code = IWebAPI.OK;
			if (cb!=null) cb.OnRequest( new Roar.RequestResult( root, callback_code, callback_msg) );
		}
		
		RoarManager.OnCallComplete( new RoarManager.CallInfo( root, callback_code, callback_msg, "no id" ) );
	}
}