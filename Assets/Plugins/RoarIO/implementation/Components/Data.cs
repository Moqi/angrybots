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

public class Data : IData
{
  protected IWebAPI.IUserActions user_actions_;
  protected DataStore data_store_;
  protected ILogger logger_;
		
  // Universal Data Store - getData + setData
  private Hashtable Data_ = new Hashtable();
  
  public Data( IWebAPI.IUserActions user_actions, DataStore data_store, ILogger logger )
  {
		user_actions_ = user_actions;
		data_store_ = data_store;
		logger_ = logger;
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

		user_actions_.netdrive_fetch( args, onGetData, callback_info );
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
      logger_.DebugLog("[roar] -- No data for key: "+opt["key"]);
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
		
    user_actions_.netdrive_save( args, onSetData, cbOptions );
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