using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{

public class User : IUser
{
  protected IRoarInternal roar_internal_;
  protected DataStore DataStore_;

  public User( IRoarInternal roar_internal, DataStore data_store )
  {
    roar_internal_ = roar_internal;
    DataStore_ = data_store;

    // -- Event Watchers
    // Flush models on logout
    RoarIOManager.loggedOutEvent += () => { DataStore_.clear(true); };

    // Watch for initial inventory ready event, then watch for any
    // subsequent `change` events
    RoarIOManager.inventoryReadyEvent +=  () => cacheFromInventory();
  }




  // ---- Access Methods ----
  // ------------------------

  public void doLogin( string name, string hash, Roar.Callback cb )
  {
    if (name == "" || hash == "")
    {
      Debug.Log("[roar] -- Must specify username and password for login");
    }
    else
    {
      Hashtable args = new Hashtable();
      args["name"]=name;
      args["hash"]=hash;
      Hashtable callback_info = new Hashtable();
      callback_info["cb"]=cb;
      callback_info["name"]=name;
      callback_info["hash"]=hash;
      roar_internal_.WebAPI.user.login( args, onDoLogin, callback_info );
    }
  }

  protected void onDoLogin( IXMLNode d, int code, string msg, string callid, Hashtable opt)
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if (cb!=null) cb( new Roar.CallbackInfo(null, code, msg) );
      RoarIOManager.OnLogInFailed(msg);
    }
    else
    {
      onLogin( opt, cb, code, msg );
    }
  }

  public void doLoginFacebookOAuth( string oauth_token, Roar.Callback cb )
  {
    if ( oauth_token == "" )
    {
      Debug.Log("[roar] -- Must specify oauth_token for facebook login");
      return;
    }

    Hashtable args = new Hashtable();
    args["oauth_token"] = oauth_token;

    Hashtable callback_info = new Hashtable();
    callback_info["cb"] = cb;
    callback_info["oauth_token"]=oauth_token;

    roar_internal_.WebAPI.user.login_facebook_oauth( args, onDoLoginFacebookOAuth, callback_info );
  }
  protected void onDoLoginFacebookOAuth( IXMLNode d, int code, string msg, string callid, Hashtable opt)
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if (cb!=null) cb( new Roar.CallbackInfo(null, code, msg) );
      RoarIOManager.OnLogInFailed(msg);
    }
    else
    {
      onLogin( opt, cb, code, msg );
    }
  }


  public void doLogout( Roar.Callback cb )
  {
    Hashtable callback_info = new Hashtable();
    callback_info["cb"] = cb;
    roar_internal_.WebAPI.user.logout( null, onDoLogout, callback_info);
  }
  protected void onDoLogout( IXMLNode d, int code, string msg, string callid, Hashtable opt)
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200) { if (cb!=null) cb( new Roar.CallbackInfo(null,code, msg) ); }
    else
    {
      RoarIOManager.OnLoggedOut();
    }
  }


  public void doCreate( string name, string hash, Roar.Callback cb )
  {
    if (name == "" || hash == "") { Debug.Log("[roar] -- Must specify username and password for login"); return; }
    Hashtable args = new Hashtable();
    args["name"] = name;
    args["hash"] = hash;

    Hashtable callback_info = new Hashtable();
    callback_info["cb"] = cb;
    callback_info["name"] = name;
    callback_info["hash"] = hash;

    roar_internal_.WebAPI.user.create(args, onDoCreate, callback_info);
  }
  protected void onDoCreate( IXMLNode d, int code, string msg, string callid, Hashtable opt)
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      RoarIOManager.OnCreateUserFailed(msg);
      if (cb!=null) cb( new Roar.CallbackInfo(null,code, msg) );
    }
    else
    {
      RoarIOManager.OnCreatedUser();
      // Player is auto-logged in on create, so call the onLogin
      onLogin( opt, cb, code, msg );
    }
  }

  protected void onLogin( Hashtable opt, Roar.Callback cb, int code, string msg )
  {
    RoarIOManager.OnLoggedIn();
    Debug.Log ("cb = "+cb.ToString() );

    if (cb!=null) cb( new Roar.CallbackInfo(null, code, msg) );

    // @todo Perform auto loading of game and player data
  }


  public void cacheFromInventory( Roar.Callback cb=null )
  {
    if (! DataStore_.Inventory_.hasDataFromServer) return;

    // Build sanitised ARRAY of ikeys from Inventory.list()
    var l =  DataStore_.Inventory_.list();
    var ikeyList = new ArrayList();
    for (int i=0; i<l.Count; i++) ikeyList.Add( (l[i] as Hashtable)["ikey"] );

    var toCache = DataStore_.itemsNotInCache( ikeyList ) as ArrayList;

    // Build sanitised Hashtable of ikeys from Inventory  
    // No need to call server as information is already present
    Hashtable cacheData = new Hashtable();
    for (int i=0; i<toCache.Count; i++)
    {
      for (int k=0; k<l.Count; k++)
      {
        // If the Inventory ikey matches a value in the
        // list of items to cache, add it to our `cacheData` obj
        if ( (l[k] as Hashtable)["ikey"] == toCache[i])
          cacheData[ toCache[i] ] = l[k];
      }
    }

    // Enable update of cache if it hasn't been initialised yet
    DataStore_.Cache_.hasDataFromServer = true;
    DataStore_.Cache_._set( cacheData );
  }

}

}
