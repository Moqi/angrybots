using System.Collections;
using Roar.Components;
using UnityEngine;

namespace Roar.implementation.Components
{

public class Data : IData
{
  protected IRoarInternal roar_internal_;
  protected DataStore data_store_;
		
  // Universal Data Store - getData + setData
  private Hashtable Data_ = new Hashtable();
		
  public Data( IRoarInternal roar_internal, DataStore data_store )
  {
		roar_internal_ = roar_internal;
		data_store_ = data_store;			
  }
  // ---- Data Methods ----
  // ----------------------
  // UNITY Note: Data is never coerced from a string to an Object(Hash)
  // which is left as an exercise for the reader
  public void load( string key, Roar.Callback callback )
  {
    // If data is already present in the client cache, return that
    if (Data_[key] != null) 
    {
      var ret = Data_[key];
      if (callback!=null) callback( new Roar.CallbackInfo(ret, IWebAPI.OK, null) );
    }
    else
	{
		Hashtable args = new Hashtable();
		args["ikey"] = key;
			
		Hashtable callback_info = new Hashtable();
		callback_info["cb"] = callback;
		callback_info["key"] = key;

		roar_internal_.WebAPI.user.netdrive_fetch( args, onGetData, callback_info );
	}
  }
  protected void onGetData( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if(cb!=null) cb(new Roar.CallbackInfo(null,code,msg));
      return;
    }

    string data = "";
    string str = null;
    //if (d["netdrive_field"] != null)

      // Glorious Unity object slicing. Define every step.
      //var nd = (((d["netdrive_field"] as ArrayList)[0] as Hashtable)["data"] as ArrayList);
	IXMLNode nd = d.GetNode("netdrive_field>0>data>0");
	if(nd!=null)
	{
	  str = nd.Text;
    }
    if (str!=null) data = str;

    Data_[opt["key"]] = data;

    if (data == "") 
    { 
      if (roar_internal_.isDebug()) Debug.Log("[roar] -- No data for key: "+opt["key"]);
      if (cb!=null) cb( new Roar.CallbackInfo(data, IWebAPI.UNKNOWN_ERR, "No data for key: "+opt["key"]) );
      return;
    }

	RoarIOManager.OnDataLoaded(opt["key"] as string, data);
    if (cb!=null) cb( new Roar.CallbackInfo (data, code, msg) );
  }


  // UNITY Note: Data is forced to a string to save us having to
  // manually 'stringify' anything.
  public void save( string key, string val, Roar.Callback callback)
  {
    Data_[ key ] = val;
    Hashtable cbOptions = new Hashtable();
	cbOptions["cb"]=callback;
	cbOptions["key"]=key;
	cbOptions["data"]=val;

	Hashtable args = new Hashtable();
	args["ikey"]=key;
	args["data"]=val;
		
    roar_internal_.WebAPI.user.netdrive_save( args, onSetData, cbOptions );
  }
  protected void onSetData( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;

    if (code != 200)
    {
      if(cb!=null) cb(new Roar.CallbackInfo(null,code,msg));
      return;
    }

	RoarIOManager.OnDataSaved(opt["key"] as string, opt["data"] as string);
	Hashtable data = new Hashtable();
	data["key"] = opt["key"];
	data["data"] = opt["data"];
		
    if (cb!=null) cb( new Roar.CallbackInfo (data, code, msg) );
  }	
	
}
	
}