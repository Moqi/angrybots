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

public class Shop : IShop
{
  protected IWebAPI.IShopActions shop_actions_;
  protected DataStore data_store_;
  protected ILogger logger_;

  public Shop( IWebAPI.IShopActions shop_actions, DataStore data_store, ILogger logger )
  {
		shop_actions_ = shop_actions;
		data_store_ = data_store;
		logger_ = logger;

		RoarIOManager.shopReadyEvent += () => cacheFromShop();
  }

  public void fetch(Roar.Callback callback) { data_store_.Shop_.fetch(callback); }
  public bool hasDataFromServer { get { return data_store_.Shop_.hasDataFromServer; } }

  public void buy( string shop_ikey, Roar.Callback callback ) { shopBuy( shop_ikey, callback ); }

  public ArrayList list() { return list(null); }
  public ArrayList list(Roar.Callback callback) 
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Shop_.list() ) );
    return data_store_.Shop_.list();
  }

  // Returns the *object* associated with attribute `key`
  public object getShopItem( string ikey ) { return getShopItem(ikey,null); }
  public object getShopItem( string ikey, Roar.Callback callback )
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Shop_._get(ikey) ) );
    return data_store_.Shop_._get(ikey); 
  }
		
  public void shopBuy( string shop_ikey, Roar.Callback cb )
  {
    var shop_item = data_store_.Shop_._get( shop_ikey);

    // Make the call if the item is in the shop
    if (shop_item==null)
    {
      logger_.DebugLog("[roar] -- Cannot find to purchase: "+ shop_ikey);
      return;
    }
    logger_.DebugLog ("trying to buy me a : "+Roar.Json.ObjectToJSON(shop_item) );
    string ikey = shop_item["ikey"] as string;
		
	Hashtable args = new Hashtable();
	args["shop_item_ikey"] = shop_ikey;
		
	Hashtable callback_info = new Hashtable();
	callback_info["cb"]=cb;
	callback_info["ikey"]=ikey;
		

    shop_actions_.buy( args, onShopBuy, callback_info );
  }		
		
  protected void onShopBuy( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;
    string ikey = opt["ikey"] as string;

    if (code!=200)
    {
      if (cb!=null) cb(new Roar.CallbackInfo(null,code,msg));
      return;
    }
	IXMLNode result = d.GetNode("roar>0>shop>0>buy>0");	

    // Obtain the server id for the purchased item
    // NOTE: Assumes ONLY ONE item per "shopitem" entity
	IXMLNode cost = result.GetNode( "costs>0>cost>0" );
	IXMLNode item = result.GetNode( "modifiers>0>modifier>0" );


    //string id = item["item_id"] as string;
	string id = item.GetAttribute("item_id");	

    // Notify the system
    //RoarIO.Events.fire( "GOOD_BOUGHT", {
    //    "currency_name": cost["ikey"] as string
    //  , "item_price"   : int.Parse(cost["value"] as string)
    //  , "item_id"      : ikey
    //  , "item_qty"     : 1
    //});
	RoarIOManager.OnGoodBought( new RoarIOManager.PurchaseInfo( cost.GetAttribute("ikey"),  int.Parse(cost.GetAttribute("value")), ikey, 1) );
		
	Hashtable data = new Hashtable();
	data["id"]=id;
	data["ikey"]=ikey;
    if (cb!=null) cb( new Roar.CallbackInfo (data, code, msg) );
  }
		
  // Builds a list of items to fetch from Server by comparing
  // what's in the Shop list and what's currently in the cache
  public bool cacheFromShop()
  {
    if (data_store_.Shop_.hasDataFromServer)
    {
      // Build sanitised ARRAY of ikeys from Shop.list()
      var l = data_store_.Shop_.list() as ArrayList;
      var ikeyList = new ArrayList();

      foreach( Hashtable v in l)
      {
         ikeyList.Add( v["ikey"] );
      } 
      
      return data_store_.Cache_.addToCache( ikeyList );
    }
    else return false;
  }
		

		
}
	
}