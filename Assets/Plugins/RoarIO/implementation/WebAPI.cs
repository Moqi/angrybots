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
using System.Collections.Generic;
using UnityEngine;


public class WebAPI : IWebAPI
{
  protected IRoarIO roar_;
  protected IRequestSender requestSender_;

  public WebAPI(IRoarIO roar, IRequestSender requestSender)
  {
     roar_ = roar;
     requestSender_ = requestSender;
		
    admin_ = new AdminActions(requestSender);
    friends_ = new FriendsActions(requestSender);
    info_ = new InfoActions(requestSender);
    items_ = new ItemsActions(requestSender);
    leaderboards_ = new LeaderboardsActions(requestSender);
    mail_ = new MailActions(requestSender);
    shop_ = new ShopActions(requestSender);
    scripts_ = new ScriptsActions(requestSender);
    tasks_ = new TasksActions(requestSender);
    user_ = new UserActions(requestSender);
    urbanairship_ = new UrbanairshipActions(requestSender);

  }

  public override string roarAuthToken { get { return roarAuthToken_; } }
  protected string roarAuthToken_ = "";

  // Set these values to your Roar game values
  public string roar_api_url = "http://api.roar.io/";

  // `gameKey` is exposed and set in RoarIO as a public UnityEditor variable
  public override string gameKey { get { return gameKey_; } set { gameKey_ = value; } }
  protected string gameKey_ = "";

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
    protected IRequestSender api;
    public APIBridge( IRequestSender caller ) { api = caller; }
  }


  public class AdminActions : APIBridge, IAdminActions
  {
    public AdminActions( IRequestSender caller ) : base(caller) {}

    public void _set( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("admin/set", obj, cb, opt );
    }

    public void delete_player( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("admin/delete_player", obj, cb, opt );
    }

  }

  public class FriendsActions : APIBridge, IFriendsActions
  {
    public FriendsActions( IRequestSender caller ) : base(caller) {}

    public void accept( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/accept", obj, cb, opt );
    }

    public void decline( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/decline", obj, cb, opt );
    }

    public void invite( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/invite", obj, cb, opt );
    }

    public void invite_info( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/info", obj, cb, opt );
    }

    public void list( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/list", obj, cb, opt );
    }

    public void remove( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("friends/remove", obj, cb, opt );
    }

  }

  public class InfoActions : APIBridge, IInfoActions
  {
    public InfoActions( IRequestSender caller ) : base(caller) {}

    public void ping( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("info/ping", null, cb, opt );
    }

    public void user( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("info/user", obj, cb, opt );
    }

    public void poll( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("info/poll", null, cb, opt );
    }

  }

  public class ItemsActions : APIBridge, IItemsActions
  {
    public ItemsActions( IRequestSender caller ) : base(caller) {}

    public void equip( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/equip", obj, cb, opt );
    }

    public void list( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/list", null, cb, opt );
    }

    public void sell( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/sell", obj, cb, opt );
    }

    public void _set( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/set", obj, cb, opt );
    }

    public void unequip( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/unequip", obj, cb, opt );
    }

    public void use( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/use", obj, cb, opt );
    }

    public void view( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("items/view", obj, cb, opt );
    }

  }

  public class LeaderboardsActions : APIBridge, ILeaderboardsActions
  {
    public LeaderboardsActions( IRequestSender caller ) : base(caller) {}

    public void list( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("leaderboards/list", obj, cb, opt );
    }

    public void view( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("leaderboards/view", obj, cb, opt );
    }

  }

  public class MailActions : APIBridge, IMailActions
  {
    public MailActions( IRequestSender caller ) : base(caller) {}

    public void accept( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("mail/accept", obj, cb, opt );
    }

    public void send( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("mail/send", obj, cb, opt );
    }

    public void what_can_i_accept( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("mail/what_can_i_accept", obj, cb, opt );
    }

    public void what_can_i_send( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("mail/what_can_i_send", obj, cb, opt );
    }

  }

  public class ShopActions : APIBridge, IShopActions
  {
    public ShopActions( IRequestSender caller ) : base(caller) {}

    public void list( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("shop/list", null, cb, opt );
    }

    public void buy( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("shop/buy", obj, cb, opt );
    }

  }

  public class ScriptsActions : APIBridge, IScriptsActions
  {
    public ScriptsActions( IRequestSender caller ) : base(caller) {}

    public void run( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("scripts/run", obj, cb, opt );
    }

  }

  public class TasksActions : APIBridge, ITasksActions
  {
    public TasksActions( IRequestSender caller ) : base(caller) {}

    public void list( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("tasks/list", null, cb, opt );
    }

    public void start( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("tasks/start", obj, cb, opt );
    }

  }

  public class UserActions : APIBridge, IUserActions
  {
    public UserActions( IRequestSender caller ) : base(caller) {}

    public void achievements( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/achievements", null, cb, opt );
    }

    public void create( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/create", obj, cb, opt );
    }

    public void login( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/login", obj, cb, opt );
    }

    public void login_facebook_oauth( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("facebook/login_oauth", obj, cb, opt );
    }

    public void logout( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/logout", null, cb, opt );
    }

    public void netdrive_save( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/netdrive_set", obj, cb, opt );
    }

    public void netdrive_fetch( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/netdrive_get", obj, cb, opt );
    }

    public void _set( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/set", obj, cb, opt );
    }

    public void view( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("user/view", null, cb, opt );
    }

  }

  public class UrbanairshipActions : APIBridge, IUrbanairshipActions
  {
    public UrbanairshipActions( IRequestSender caller ) : base(caller) {}

    public void ios_register( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("urbanairship/ios_register", obj, cb, opt );
    }

    public void push( Hashtable obj, RequestCallback cb, Hashtable opt)
    {
      api.make_call("urbanairship/push", obj, cb, opt );
    }

  }


}

