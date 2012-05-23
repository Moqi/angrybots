using System.Collections;
using Roar.Components;

namespace Roar.implementation.Components
{

public class Properties : IProperties
{
  protected DataStore data_store_;
  protected IRoarInternal roar_internal_;
	
  public Properties( IRoarInternal roar_internal, DataStore data_store )
  {
	roar_internal_ = roar_internal;
	data_store_ = data_store;
	RoarIOManager.roarServerUpdateEvent += this.OnUpdate;
  }

  public void fetch( Roar.Callback callback){ data_store_.Properties_.fetch(callback); }
  public bool hasDataFromServer { get { return data_store_.Properties_.hasDataFromServer; } }

  public ArrayList list() { return list(null); }
  public ArrayList list( Roar.Callback callback) 
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Properties_.list() ) );
    return data_store_.Properties_.list();
  }

  // Returns the *object* associated with attribute `key`
  public object getProperty( string key ) { return getProperty(key,null); }
  public object getProperty( string key, Roar.Callback callback )
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Properties_._get(key) ) );
    return data_store_.Properties_._get(key);
  }

  // Returns the *value* of attribute `key`
  public string getValue( string ikey ) { return getValue(ikey,null); }
  public string getValue( string ikey, Roar.Callback callback )
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Properties_.getValue(ikey) ) );
    return data_store_.Properties_.getValue(ikey);
    }

	

  protected void OnUpdate(IXMLNode update)
  {
    //Since you can get change events from login calls, when the Properties object is not yet setup we need to be careful here:
    if( ! hasDataFromServer ) return;

    //var d = event['data'] as Hashtable;

    //TODO: This should probably be using RoarIO.Properties._set or something like that but their use is not clear to me
    var v = getProperty(update.GetAttribute("ikey")) as Hashtable;
    if(v!=null)
    {
      v["value"] = update.GetAttribute("value");
    }
 }	

}
}