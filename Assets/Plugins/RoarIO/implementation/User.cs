/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System.Collections;
using UnityEngine;

using Roar.Components;

namespace Roar.implementation.Components
{

public class User : IUser
{
  protected DataStore DataStore_;
  IWebAPI.IUserActions user_actions_;
  ILogger logger_;

  public User( IWebAPI.IUserActions user_actions, DataStore data_store, ILogger logger )
  {
    user_actions_ = user_actions;
    DataStore_ = data_store;
    logger_ = logger;

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
      logger_.DebugLog("[roar] -- Must specify username and password for login");
      return;
    }

    Hashtable args = new Hashtable();
    args["name"]=name;
    args["hash"]=hash;
    Hashtable callback_info = new Hashtable();
    callback_info["cb"]=cb;
    callback_info["name"]=name;
    callback_info["hash"]=hash;
    user_actions_.login( args, onDoLogin, callback_info );
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
      logger_.DebugLog("[roar] -- Must specify oauth_token for facebook login");
      return;
    }

    Hashtable args = new Hashtable();
    args["oauth_token"] = oauth_token;

    Hashtable callback_info = new Hashtable();
    callback_info["cb"] = cb;
    callback_info["oauth_token"]=oauth_token;

    user_actions_.login_facebook_oauth( args, onDoLoginFacebookOAuth, callback_info );
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
    user_actions_.logout( null, onDoLogout, callback_info);
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
    if (name == "" || hash == "")
    {
      logger_.DebugLog("[roar] -- Must specify username and password for login");
      return;
    }
    Hashtable args = new Hashtable();
    args["name"] = name;
    args["hash"] = hash;

    Hashtable callback_info = new Hashtable();
    callback_info["cb"] = cb;
    callback_info["name"] = name;
    callback_info["hash"] = hash;

    user_actions_.create(args, onDoCreate, callback_info);
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

    var toCache = DataStore_.Cache_.itemsNotInCache( ikeyList ) as ArrayList;

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
