using System.Collections;
using UnityEngine;

public class RequestSender : IRequestSender
{
  protected Roar.IConfig config_;
  
  protected string gameKey { get { return config_.game; } }
  protected string roarAuthToken { get { return config_.auth_token; } set { config_.auth_token = value; } }
  protected string roar_api_url { get { return config_.roar_api_url; } }
  protected bool isDebug { get { return config_.isDebug; } }

  protected IUnityObject unity_object_;
  protected Roar.ILogger logger_;

  public RequestSender( Roar.IConfig config, IUnityObject unity_object, Roar.ILogger logger )
  {
    config_=config;
    unity_object_ = unity_object;
    logger_ = logger;
  }
  
  
  public void make_call( string apicall, Hashtable args, RequestCallback cb, Hashtable opt )
  {
    unity_object_.doCoroutine( sendCore( apicall, args, cb, opt ) );
  }

  protected IEnumerator sendCore( string apicall, Hashtable args, RequestCallback cb, Hashtable opt)
  {
    if ( gameKey == "")
    {
      logger_.DebugLog("[roar] -- No game key set!--");
      yield break;
    }

    logger_.DebugLog("[roar] -- Calling: "+apicall);

    // Encode POST parameters
    WWWForm post = new WWWForm();
	if(args!=null)
	{
      foreach (DictionaryEntry param in args)
      {
        post.AddField( param.Key as string, param.Value as string );
      }
	}
    // Add the auth_token to the POST
    post.AddField( "auth_token", roarAuthToken );

    // Fire call sending event
    RoarIOManager.OnRoarNetworkStart();
		
	Debug.Log ( "roar_api_url = " + roar_api_url );
		
	Debug.Log ( "Requesting : " + roar_api_url+gameKey+"/"+apicall+"/" );

    var xhr = new WWW( roar_api_url+gameKey+"/"+apicall+"/", post);
    yield return xhr;

    onServerResponse( xhr.text, apicall, cb, opt );
  }

  protected void onServerResponse( string raw, string apicall, RequestCallback cb, Hashtable opt )
  {
    var uc = apicall.Split("/"[0]);
    var controller = uc[0];
    var action = uc[1];

    string call_id = "0"; // TODO: Hook this up to History

    // Fire call complete event
    RoarIOManager.OnRoarNetworkEnd(call_id.ToString() );

    // -- Parse the Roar response
    // Unexpected server response 
    if (raw[0] != '<')
    {
      // Error: fire the error callback
      IXMLNode errorXml = IXMLNodeFactory.instance.Create("error", raw);
      if (cb!=null) cb( errorXml, IWebAPI.FATAL_ERROR, "Invalid server response", call_id, opt );
      return;
    }

    IXMLNode rootNode = IXMLNodeFactory.instance.Create( raw );

    int callback_code;
    string callback_msg="";

    // XMLNode extends Boo.Lang.Hash
    // XMLNodeList is Array - must point to node sibling level (not parent)
    IXMLNode actionNode = rootNode.GetNode( "roar>0>"+controller+">0>"+action+">0" );
    // Hash XML keeping _name and _text values by default

    // Pre-process <server> block if any and attach any processed data
    IXMLNode serverNode = rootNode.GetNode( "roar>0>server>0" );
    RoarIOManager.NotifyOfServerChanges( serverNode );

    // Status on Server returned an error. Action did not succeed.
    string status = actionNode.GetAttribute( "status" );
    if (status == "error")
    {
      callback_code = IWebAPI.UNKNOWN_ERR;
      callback_msg = actionNode.GetFirstChild("error").Text;
      string server_error = actionNode.GetFirstChild("error").GetAttribute("type");
      if ( server_error == "0" )
      {
        if (callback_msg=="Must be logged in") { callback_code = IWebAPI.UNAUTHORIZED; }
        if (callback_msg=="Invalid auth_token") { callback_code = IWebAPI.UNAUTHORIZED; }
        if (callback_msg=="Must specify auth_token") { callback_code = IWebAPI.BAD_INPUTS; }
        if (callback_msg=="Must specify name and hash") { callback_code = IWebAPI.BAD_INPUTS; }
        if (callback_msg=="Invalid name or password") { callback_code = IWebAPI.DISALLOWED; }
        if (callback_msg=="Player already exists") { callback_code = IWebAPI.DISALLOWED; }
      }

      // Error: fire the callback
      // NOTE: The Unity version ASSUMES callback = errorCallback
      if (cb!=null) cb( rootNode, callback_code, callback_msg, call_id, opt );
    }

    // No error - pre-process the result
    else
    {
      IXMLNode auth_token = actionNode.GetFirstChild("auth_token");
      if (auth_token!=null) roarAuthToken = auth_token.Text;

      callback_code = IWebAPI.OK;
      if (cb!=null) cb( rootNode, callback_code, callback_msg, call_id, opt );
    }

    RoarIOManager.OnCallComplete( new RoarIOManager.CallInfo(rootNode, callback_code, callback_msg, call_id.ToString() ) );
  }
	

}