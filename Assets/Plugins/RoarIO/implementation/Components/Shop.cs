using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{

public class Shop : IShop
{
  protected IRoarInternal roar_internal_;
  protected DataStore data_store_;

  public Shop( IRoarInternal roar_internal, DataStore data_store )
  {
		roar_internal_ = roar_internal;
		data_store_ = data_store;
			
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
      Debug.Log("[roar] -- Cannot find to purchase: "+ shop_ikey);
      return;
    }
    Debug.Log ("trying to buy me a : "+Roar.Json.ObjectToJSON(shop_item) );
    string ikey = shop_item["ikey"] as string;
		
	Hashtable args = new Hashtable();
	args["shop_item_ikey"] = shop_ikey;
		
	Hashtable callback_info = new Hashtable();
	callback_info["cb"]=cb;
	callback_info["ikey"]=ikey;
		

    roar_internal_.WebAPI.shop.buy( args, onShopBuy, callback_info );
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


    // Only add to inventory if it has previously been intialised
    if (data_store_.Inventory_.hasDataFromServer)
    {
      var keysToAdd = new ArrayList();
      keysToAdd.Add(ikey);

      if (!data_store_.Cache_.has( ikey )) 
      {
        data_store_.addToCache( keysToAdd, h => addToInventory( ikey, id ) );
      }
      else addToInventory( ikey, id );
    }

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
      
      return data_store_.addToCache( ikeyList );
    }
    else return false;
  }
		

		
}
	
}