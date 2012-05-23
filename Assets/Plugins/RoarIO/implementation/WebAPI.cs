using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebAPI : IWebAPI
{
  protected IRoarIO roar_;
  protected IRoarInternal roarInternal_;
	
  public WebAPI(IRoarIO roar, IRoarInternal roar_internal)
  {
     roar_ = roar;
     roarInternal_ = roar_internal;
		
    admin_ = new AdminActions(this, roarInternal_);
    friends_ = new FriendsActions(this, roarInternal_);
    info_ = new InfoActions(this, roarInternal_);
    items_ = new ItemsActions(this, roarInternal_);
    leaderboards_ = new LeaderboardsActions(this, roarInternal_);
    mail_ = new MailActions(this, roarInternal_);
    shop_ = new ShopActions(this, roarInternal_);
    scripts_ = new ScriptsActions(this, roarInternal_);
    tasks_ = new TasksActions(this, roarInternal_);
    user_ = new UserActions(this, roarInternal_);
    urbanairship_ = new UrbanairshipActions(this, roarInternal_);

  }

  public override string roarAuthToken { get { return roarAuthToken_; } }
  protected string roarAuthToken_ = "";

  // Set these values to your Roar game values
  public string roar_api_url = "http://api.roar.io/";

  // `gameKey` is exposed and set in RoarIO as a public UnityEditor variable
  public override string gameKey { get { return gameKey_; } set { gameKey_ = value; } }
  protected string gameKey_ = "";

  public override IEnumerator sendCore( string apicall, Hashtable args, Callback cb, Hashtable opt)
  {
    if (gameKey == "")
    {
      if (roarInternal_.isDebug()) Debug.Log("[roar] -- No game key set!--");
      yield break;
	  //return false;
    }

    if (roarInternal_.isDebug()) Debug.Log("[roar] -- Calling: "+apicall);

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

    var xhr = new WWW( roar_api_url+gameKey+"/"+apicall+"/", post);
    yield return xhr;

    // if (roar_.isDebug()) Debug.Log(xhr.text);

    onServerResponse( xhr.text, apicall, cb, opt );
  }

  protected void onServerResponse( string raw, string apicall, Callback cb, Hashtable opt )
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
      if (cb!=null) cb( errorXml, FATAL_ERROR, "Invalid server response", call_id, opt );
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
    notifyOfServerChanges( serverNode );

    // Status on Server returned an error. Action did not succeed.
    string status = actionNode.GetAttribute( "status" );
    if (status == "error")
    {
      callback_code = UNKNOWN_ERR;
      callback_msg = actionNode.GetFirstChild("error").Text;
      string server_error = actionNode.GetFirstChild("error").GetAttribute("type");
      if ( server_error == "0" )
      {
        if (callback_msg=="Must be logged in") { callback_code = UNAUTHORIZED; }
        if (callback_msg=="Invalid auth_token") { callback_code = UNAUTHORIZED; }
        if (callback_msg=="Must specify auth_token") { callback_code = BAD_INPUTS; }
        if (callback_msg=="Must specify name and hash") { callback_code = BAD_INPUTS; }
        if (callback_msg=="Invalid name or password") { callback_code = DISALLOWED; }
        if (callback_msg=="Player already exists") { callback_code = DISALLOWED; }
      }

      // Error: fire the callback
      // NOTE: The Unity version ASSUMES callback = errorCallback
      if (cb!=null) cb( rootNode, callback_code, callback_msg, call_id, opt );
    }

    // No error - pre-process the result
    else
    {
      IXMLNode auth_token = actionNode.GetFirstChild("auth_token");
      if (auth_token!=null) roarAuthToken_ = auth_token.Text;

      callback_code = OK;
      if (cb!=null) cb( rootNode, callback_code, callback_msg, call_id, opt );
    }

    RoarIOManager.OnCallComplete( new RoarIOManager.CallInfo(rootNode, callback_code, callback_msg, call_id.ToString() ) );
  }

  // Checks for a <server> node and broadcasts events
  // for any matching chunks
  private void notifyOfServerChanges( IXMLNode server )
  {
    if (server==null) return;

    // Dispatch the entire <server> object
    RoarIOManager.OnRoarServerAll(server);
    foreach ( IXMLNode chunkData in server.Children)
    {
        if(chunkData==null) throw new System.Exception("Null XML Node found!");

        // Reprocess the <task_complete> server chunk
        switch( chunkData.Name )
        {
          case "update":
            RoarIOManager.OnRoarServerUpdate(chunkData);
            break;
		  case "item_add":
            RoarIOManager.OnRoarServerItemAdd(chunkData);
            break;
          case "item_use":
            RoarIOManager.OnRoarServerItemUse(chunkData);
            break;
          case "item_lose":
            RoarIOManager.OnRoarServerItemLose(chunkData);
            break;
          case "inventory_changed":
            RoarIOManager.OnRoarServerInventoryChanged(chunkData);
            break;
          default:
            Debug.Log("Server event "+chunkData.Name+" not yet implemented");
            break;
        }
    }
  }



  // Take a Hashtable that has been processed by ToHashDeep and remove _ attributes
  // TODO: This should probably be static
  public override Hashtable StripHashOfUnderscoreAttribs( Hashtable parse )
  {
    Hashtable thisLevel = new Hashtable();
    // Step through the object at this level
    foreach (DictionaryEntry param in parse)
    {    
      string key = (param.Key as string);

      // XMLNodeLists must be parsed and added to this level recursively
      if ( param.Value is ArrayList)
      {
        ArrayList ar = param.Value as ArrayList;

        for (int i=0; i< ar.Count; i++)
        {
          if ( thisLevel[key]==null ) thisLevel[key] = new ArrayList();
          // Only process XMLNodes (as Hashes of course)
          if ( ar[i] is Hashtable) 
            (thisLevel[key] as ArrayList).Add( StripHashOfUnderscoreAttribs( ar[i] as Hashtable) );
        }
      }
      // We're dealing with strings
      // But strip out anything starting with '_' (XML Parser crap)
      else if ( (key)[0] != '_' ) 
      {
        // Convert string booleans to native booleans
        if (param.Value as string == "true") thisLevel[ key ] = true;
        else if (param.Value as string == "false") thisLevel[ key ] = false;
        else thisLevel[ key ] = param.Value;
      }
    }
    return thisLevel;
  }

  // Converts a Hashtable object with nexted XMLNodeLists (ie. output from
  // the XML Parser) to a pure, neat Hashtable
  static Hashtable ToHashDeep( Hashtable parse ) { return ToHashDeep(parse, false ); }
  static Hashtable ToHashDeep( Hashtable parse, bool preserve )
  {
	if( parse == null) throw new System.ArgumentNullException("parse");
	var thisLevel = new Hashtable();
    // Step through the object at this level
    foreach (DictionaryEntry param in parse)
    {    
      string key = (param.Key as string);

      // XMLNodeLists must be parsed and added to this level recursively
      if ( param.Value is ArrayList)
      {
        ArrayList ar = param.Value as ArrayList;
		if ( thisLevel[key]==null ) thisLevel[key] = new ArrayList();

        for (int i=0; i< ar.Count; i++)
        {
          // Only process XMLNodes (as Hashes of course)
          if ( ar[i] is XMLNode) 
            (thisLevel[key] as ArrayList).Add( ToHashDeep( ar[i] as Hashtable, preserve ) );
        }
      }
      // We're dealing with strings
      // But strip out anything starting with '_' (XML Parser crap)
      else if ( preserve || (key!="") || (key)[0] != '_' ) 
      {
        // Convert string booleans to native booleans
        if (param.Value as string == "true") thisLevel[ key ] = true;
        else if (param.Value as string == "false") thisLevel[ key ] = false;
        else thisLevel[ key ] = param.Value;
      }
    }
    return thisLevel;
  }

  public override IAdminActions admin { get { return admin_; } }
  public AdminActions admin_;

  public override IFriendsActions friends { get { return friends_; } }
  public FriendsActions friends_;

  public override IInfoActions info { get { return info_; } }
  public InfoActions info_;

  public override IItemsActions items { get { return items_; } }
  public ItemsActions items_;

  public override ILeaderboardsActions leaderboards { get { return leaderboards_; } }
  public LeaderboardsActions leaderboards_;

  public override IMailActions mail { get { return mail_; } }
  public MailActions mail_;

  public override IShopActions shop { get { return shop_; } }
  public ShopActions shop_;

  public override IScriptsActions scripts { get { return scripts_; } }
  public ScriptsActions scripts_;

  public override ITasksActions tasks { get { return tasks_; } }
  public TasksActions tasks_;

  public override IUserActions user { get { return user_; } }
  public UserActions user_;

  public override IUrbanairshipActions urbanairship { get { return urbanairship_; } }
  public UrbanairshipActions urbanairship_;



  public class APIBridge
  {
    protected IWebAPI api;
    protected IRoarInternal roar_internal_;
    public APIBridge( IWebAPI caller, IRoarInternal roar_internal ) { api = caller; roar_internal_ = roar_internal; }
  }


  public class AdminActions : APIBridge, IAdminActions
  {
    public AdminActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void _set( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("admin/set", obj, cb, opt ));
    }

    public void delete_player( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("admin/delete_player", obj, cb, opt ));
    }

  }

  public class FriendsActions : APIBridge, IFriendsActions
  {
    public FriendsActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void accept( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/accept", obj, cb, opt ));
    }

    public void decline( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/decline", obj, cb, opt ));
    }

    public void invite( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/invite", obj, cb, opt ));
    }

    public void invite_info( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/info", obj, cb, opt ));
    }

    public void list( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/list", obj, cb, opt ));
    }

    public void remove( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("friends/remove", obj, cb, opt ));
    }

  }

  public class InfoActions : APIBridge, IInfoActions
  {
    public InfoActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void ping( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("info/ping", null, cb, opt ));
    }

    public void user( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("info/user", obj, cb, opt ));
    }

    public void poll( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("info/poll", null, cb, opt ));
    }

  }

  public class ItemsActions : APIBridge, IItemsActions
  {
    public ItemsActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void equip( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/equip", obj, cb, opt ));
    }

    public void list( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/list", null, cb, opt ));
    }

    public void sell( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/sell", obj, cb, opt ));
    }

    public void _set( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/set", obj, cb, opt ));
    }

    public void unequip( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/unequip", obj, cb, opt ));
    }

    public void use( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/use", obj, cb, opt ));
    }

    public void view( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("items/view", obj, cb, opt ));
    }

  }

  public class LeaderboardsActions : APIBridge, ILeaderboardsActions
  {
    public LeaderboardsActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void list( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("leaderboards/list", obj, cb, opt ));
    }

    public void view( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("leaderboards/view", obj, cb, opt ));
    }

  }

  public class MailActions : APIBridge, IMailActions
  {
    public MailActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void accept( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("mail/accept", obj, cb, opt ));
    }

    public void send( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("mail/send", obj, cb, opt ));
    }

    public void what_can_i_accept( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("mail/what_can_i_accept", obj, cb, opt ));
    }

    public void what_can_i_send( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("mail/what_can_i_send", obj, cb, opt ));
    }

  }

  public class ShopActions : APIBridge, IShopActions
  {
    public ShopActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void list( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("shop/list", null, cb, opt ));
    }

    public void buy( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("shop/buy", obj, cb, opt ));
    }

  }

  public class ScriptsActions : APIBridge, IScriptsActions
  {
    public ScriptsActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void run( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("scripts/run", obj, cb, opt ));
    }

  }

  public class TasksActions : APIBridge, ITasksActions
  {
    public TasksActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void list( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("tasks/list", null, cb, opt ));
    }

    public void start( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("tasks/start", obj, cb, opt ));
    }

  }

  public class UserActions : APIBridge, IUserActions
  {
    public UserActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void achievements( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/achievements", null, cb, opt ));
    }

    public void create( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/create", obj, cb, opt ));
    }

    public void login( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/login", obj, cb, opt ));
    }

    public void login_facebook_oauth( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("facebook/login_oauth", obj, cb, opt ));
    }

    public void logout( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/logout", null, cb, opt ));
    }

    public void netdrive_save( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/netdrive_set", obj, cb, opt ));
    }

    public void netdrive_fetch( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/netdrive_get", obj, cb, opt ));
    }

    public void _set( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/set", obj, cb, opt ));
    }

    public void view( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("user/view", null, cb, opt ));
    }

  }

  public class UrbanairshipActions : APIBridge, IUrbanairshipActions
  {
    public UrbanairshipActions( IWebAPI caller, IRoarInternal roar_internal ) : base(caller,roar_internal) {}

    public void ios_register( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("urbanairship/ios_register", obj, cb, opt ));
    }

    public void push( Hashtable obj, Callback cb, Hashtable opt)
    {
      roar_internal_.doCoroutine( api.sendCore("urbanairship/push", obj, cb, opt ));
    }

  }


}

