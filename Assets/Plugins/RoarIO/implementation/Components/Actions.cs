using System.Collections;
using Roar.Components;

namespace Roar.implementation.Components
{

public class Actions : IActions
{
  protected IRoarInternal roar_internal_;
  protected DataStore data_store_;

  public Actions( IRoarInternal roar_internal, DataStore data_store )
  {
		roar_internal_ = roar_internal;
		data_store_ = data_store;
  }

  public bool hasDataFromServer { get { return data_store_.Actions_.hasDataFromServer; } }
  public void fetch(Roar.Callback callback) { data_store_.Actions_.fetch(callback); }

  public ArrayList list() { return list(null); }
  public ArrayList list(Roar.Callback callback) 
  {
    if (callback!=null) callback( new Roar.CallbackInfo( data_store_.Actions_.list() ) );
    return data_store_.Actions_.list();
  }

  public void execute( string ikey, Roar.Callback callback )
  {

	Hashtable args = new Hashtable();
	args["task_ikey"]=ikey;
	
	Hashtable callback_info = new Hashtable();
	callback_info["cb"] = callback;
	callback_info["key"] = ikey;

    roar_internal_.WebAPI.tasks.start( args, onActionsDo, callback_info );
  }
  protected void onActionsDo( IXMLNode d, int code, string msg, string callid, Hashtable opt )
  {
    Roar.Callback cb = (opt["cb"]!=null) ? opt["cb"] as Roar.Callback : null;;

    if (code != 200)
    {
      if (cb!=null) cb(new Roar.CallbackInfo(null,code,msg));
      return;
    }

    // Event complete info (task_complete) is sent in a <server> chunk
    // (backend quirk related to potentially asynchronous tasks)
    // In this case its ALWAYS a synchronous call, so we KNOW the data will
    // be available - data is formatted in WebAPI Class.
    //var eventData = d["server"] as Hashtable;
	IXMLNode eventData = d.GetFirstChild("server");

    RoarIOManager.OnEventDone(eventData);

    if (cb!=null) cb( new Roar.CallbackInfo(eventData, code, msg) );
  }
}

}