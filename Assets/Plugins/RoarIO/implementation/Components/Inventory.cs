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
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{

public class Inventory : IInventory
{
  protected DataStore data_store_;
  protected IWebAPI.IItemsActions item_actions_;
  protected ILogger logger_;

  public Inventory( IWebAPI.IItemsActions item_actions, DataStore data_store, ILogger logger )
  {
	item_actions_ = item_actions;
	data_store_ = data_store;
	logger_ = logger;
	RoarIOManager.roarServerItemAddEvent += this.OnServerItemAdd;
  }

  public bool hasDataFromServer { get { return  data_store_.Inventory_.hasDataFromServer; } }
  public void fetch( Roar.Callback callback) { data_store_.Inventory_.fetch(callback); }

  public ArrayList list() { return list(null); }
  public ArrayList list(Roar.Callback callback) 
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Inventory_.list() ) );
    return data_store_.Inventory_.list();
  }

  public void activate( string id, Roar.Callback callback )
  {
    var item = data_store_.Inventory_._get( id );
    if (item==null)
    {
      logger_.DebugLog("[roar] -- Failed: no record with id: "+id);
      return;
    }
		
    Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;
    item_actions_.equip( args, onActivate, cbOptions );
  }
		
  protected void onActivate( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
	    if (cb!=null) cb( new Roar.CallbackInfo(null, code, msg)  );
      return;
    }

    var item = data_store_.Inventory_._get( opt["id"] as string );
    item["equipped"]=true;
    Hashtable returnObj = new Hashtable();
	returnObj["id"]=opt["id"];
    returnObj["ikey"]=item["ikey"];
    returnObj["label"]=item["label"];

    RoarIOManager.OnGoodActivated( new RoarIOManager.GoodInfo( opt["id"] as string, item["ikey"] as string, item["label"] as string ) );
    if (cb!=null) cb(new Roar.CallbackInfo(returnObj, code, msg) );
  }
		
  public void deactivate( string id, Roar.Callback callback )
  {
    var item = data_store_.Inventory_._get( id as string );
    if (item==null)
    {
      logger_.DebugLog("[roar] -- Failed: no record with id: "+id);
      return;
    }

	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;
		
    item_actions_.unequip( args, onDeactivate, cbOptions );
  }
  protected void onDeactivate( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
       if (cb!=null) cb( new Roar.CallbackInfo(null, code, msg) );
       return;
    }

    var item = data_store_.Inventory_._get( opt["id"] as string );
    item["equipped"]=false;
    Hashtable returnObj = new Hashtable();
	returnObj["id"]=opt["id"];
    returnObj["ikey"]=item["ikey"];
    returnObj["label"]=item["label"];

    RoarIOManager.OnGoodDeactivated( new RoarIOManager.GoodInfo(opt["id"] as string, item["ikey"] as string, item["label"] as string ) );
    if (cb!=null) cb(new Roar.CallbackInfo(returnObj, code, msg));
  }
		
  // `has( key, num )` boolean checks whether user has object `key`
  // and optionally checks for a `num` number of `keys` *(default 1)*
  public bool has( string ikey ) { return has(ikey, 1, null); }
  public bool has( string ikey, int num, Roar.Callback callback )
  { 
    if (callback!=null) callback( new Roar.CallbackInfo(data_store_.Inventory_.has( ikey, num )) );
    return data_store_.Inventory_.has( ikey, num );
  }

  // `quantity( key )` returns the number of `key` objects held by user
  public int quantity( string ikey ) { return quantity(ikey, null); }
  public int quantity( string ikey, Roar.Callback callback )
  { 
    if (callback!=null) callback( new Roar.CallbackInfo(data_store_.Inventory_.quantity( ikey )) );
    return data_store_.Inventory_.quantity( ikey );
  }

  // `sell(id)` performs a sell on the item `id` specified
  public void sell( string id, Roar.Callback callback )
  {

    var item = data_store_.Inventory_._get( id as string );
    if (item==null)
    {
      logger_.DebugLog("[roar] -- Failed: no record with id: "+id);
      return;
    }

    // Ensure item is sellable first
    if ( (bool)item["sellable"] != true)
    {
      var error = item["ikey"]+": Good is not sellable";
      logger_.DebugLog("[roar] -- " + error );
      if (callback!=null) callback( new Roar.CallbackInfo(null, IWebAPI.DISALLOWED,error) );
      return;
    }
		
	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;

    item_actions_.sell( args, onSell, cbOptions );
  }
  protected void onSell( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if (cb!=null) cb( new Roar.CallbackInfo(null,code,msg) );
      return;
    }

    var item = data_store_.Inventory_._get( opt["id"] as string );
    Hashtable returnObj = new Hashtable();
	returnObj["id"]=opt["id"];
    returnObj["ikey"]=item["ikey"];
    returnObj["label"]=item["label"];

    data_store_.Inventory_.unset( opt["id"] as string );
    
    RoarIOManager.OnGoodSold( new RoarIOManager.GoodInfo(opt["id"] as string, item["ikey"] as string, item["label"] as string ) );
    if (cb!=null) cb( new Roar.CallbackInfo(returnObj, code, msg ) );
  }

  // `use(id)` consumes/uses the item `id`
  public void use( string id, Roar.Callback callback )
  {

    var item = data_store_.Inventory_._get( id as string );

    if (item==null)
    {
      logger_.DebugLog("[roar] -- Failed: no record with id: "+id);
      return;
    }
		
    // GH#152: Ensure item is consumable first
	logger_.DebugLog ( Roar.Json.ObjectToJSON(item) );
		
    if ( (bool)item["consumable"] != true)
    {
      var error = item["ikey"]+": Good is not consumable";
      logger_.DebugLog( "[roar] -- "+error );
      if (callback!=null) callback( new Roar.CallbackInfo(null,IWebAPI.DISALLOWED,error) );
      return;
    }

	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
    Hashtable args = new Hashtable();
	args["item_id"] = id;

    item_actions_.use( args, onUse, cbOptions );
  }
  protected void onUse( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if (cb!=null) cb( new Roar.CallbackInfo(null,code,msg) );
      return;
    }

    var item = data_store_.Inventory_._get( opt["id"] as string );
    Hashtable returnObj = new Hashtable();
	returnObj["id"]=opt["id"];
    returnObj["ikey"]=item["ikey"];
    returnObj["label"]=item["label"];

    data_store_.Inventory_.unset( opt["id"] as string );

    RoarIOManager.OnGoodUsed( new RoarIOManager.GoodInfo(opt["id"] as string, item["ikey"] as string, item["label"] as string ) );
    if (cb!=null) cb( new Roar.CallbackInfo (returnObj,code, msg) );
  }

  // `remove(id)` for now is simply an *alias* to sell
  public void remove( string id, Roar.Callback callback ) { sell(id, callback); }

  // Returns raw data object for inventory
  public Hashtable getGood( string id ) { return getGood(id, null); }
  public Hashtable getGood( string id, Roar.Callback callback )
  {
    if (callback!=null) callback( new Roar.CallbackInfo(data_store_.Inventory_._get( id )) );
    return data_store_.Inventory_._get( id );
  }

  protected void OnServerItemAdd(IXMLNode d) {
	// Only add to inventory if it has previously been intialised
    if (hasDataFromServer)
    {
      var keysToAdd = new ArrayList();
      var id = d.GetAttribute("item_id");
      var ikey = d.GetAttribute("item_ikey");
				
      keysToAdd.Add(ikey);

      if (!data_store_.Cache_.has( ikey )) 
      {
        data_store_.Cache_.addToCache( keysToAdd, h => addToInventory( ikey, id ) );
      }
      else addToInventory( ikey, id );
    }
  }
		
  protected void addToInventory( string ikey, string id )
  {
    // Prepare the item to manually add to Inventory
    Hashtable item = new Hashtable();
    item[id] = DataModel._clone( data_store_.Cache_._get( ikey ) );

    // Also set the internal reference id (used by templates)
    var idspec = item[id] as Hashtable;
    idspec["id"] = id;

    // Manually add to inventory
    data_store_.Inventory_._set( item );
  }
		
}

}

