using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{

public class Inventory : IInventory
{
  protected DataStore data_store_;
  protected IRoarInternal roar_internal_;

  public Inventory( IRoarInternal roar_internal, DataStore data_store )
  {
	roar_internal_ = roar_internal;
	data_store_ = data_store;
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
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- Failed: no record with id: "+id);
      return;
    }
		
    Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;
    roar_internal_.WebAPI.items.equip( args, onActivate, cbOptions );
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
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- Failed: no record with id: "+id);
      return;
    }

	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;
		
    roar_internal_.WebAPI.items.unequip( args, onDeactivate, cbOptions );
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
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- Failed: no record with id: "+id);
      return;
    }

    // Ensure item is sellable first
    if ( item["sellable"] as string !="true")
    {
      var error = item["ikey"]+": Good is not sellable";
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- " + error );
      if (callback!=null) callback( new Roar.CallbackInfo(null, IWebAPI.DISALLOWED,error) );
      return;
    }
		
	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
	Hashtable args = new Hashtable();
	args["item_id"] = id;

    roar_internal_.WebAPI.items.sell( args, onSell, cbOptions );
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
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- Failed: no record with id: "+id);
      return;
    }
		
    // GH#152: Ensure item is consumable first
	Debug.Log ( Roar.Json.ObjectToJSON(item) );
		
    if ( item["consumable"] as string !="true")
    {
      var error = item["ikey"]+": Good is not consumable";
      if (roar_internal_.isDebug()) Debug.Log( "[roar] -- "+error );
      if (callback!=null) callback( new Roar.CallbackInfo(null,IWebAPI.DISALLOWED,error) );
      return;
    }

	Hashtable cbOptions = new Hashtable();
    cbOptions["cb"] = callback;
	cbOptions["id"] = id;
		
    Hashtable args = new Hashtable();
	args["item_id"] = id;

    roar_internal_.WebAPI.items.use( args, onUse, cbOptions );
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
		
}

}

