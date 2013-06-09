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
  protected IDataStore data_store_;
  protected ILogger logger_;
		
  // Universal Data Store - getData + setData
  private Hashtable Data_ = new Hashtable();
  
  public Data( IWebAPI.IUserActions user_actions, IDataStore data_store, ILogger logger )
  {
		user_actions_ = user_actions;
		data_store_ = data_store;
		logger_ = logger;
  }
  // ---- Data Methods ----
  // ----------------------
  // UNITY Note: Data is never coerced from a string to an Object(Hash)
  // which is left as an exercise for the reader
  public void load( string key, Roar.Callback<string> callback )
  {
    // If data is already present in the client cache, return that
    if (Data_[key] != null) 
    {
      var ret = Data_[key] as string;
      if (callback!=null) callback( new Roar.CallbackInfo<string>(ret, IWebAPI.OK, ret) );
    }
    else
	{
		WebObjects.User.NetdriveFetchArguments args = new Roar.WebObjects.User.NetdriveFetchArguments();
		args.ikey =  key;

		user_actions_.netdrive_fetch( args, new OnGetData( callback, this, key ) );
	}
  }
  class OnGetData : CBBase<WebObjects.User.NetdriveFetchResponse>
  {
    protected Data data;
    protected string key;
    Roar.Callback<string> cbx;
  
    public OnGetData( Roar.Callback<string> in_cb, Data in_data, string in_key) : base(null)
    {
      data = in_data;
      key = in_key;
      cbx = in_cb;
    }
  
  public override void HandleSuccess( CallbackInfo<WebObjects.User.NetdriveFetchResponse> info )
  {
    //TODO: Move this into the ParseXML function in NetdriveFetchResponse
    /*
    string value = "";
    string str = null;

    IXMLNode nd = info.data.GetNode("roar>0>user>0>netdrive_get>0>netdrive_field>0>data>0");
    if(nd!=null)
    {
      str = nd.Text;
    }
    if (str!=null) value = str;

    data.Data_[key] = value;
    info.msg = value;

    if ( value==null || value == "") 
    { 
      data.logger_.DebugLog("[roar] -- No data for key: "+key);
      info.code = IWebAPI.UNKNOWN_ERR;
      info.msg = "No data for key: "+key;
      cbx( new CallbackInfo<string>( null, IWebAPI.UNKNOWN_ERR, "no data for key: "+key ) );
    }
    */
    
    data.Data_[key] = info.data;

    
    cbx( new CallbackInfo<string>( info.data.data, IWebAPI.OK, null ) );
    RoarManager.OnDataLoaded( key, info.data.data);
  }
  }


  // UNITY Note: Data is forced to a string to save us having to
  // manually 'stringify' anything.
  public void save( string key, string val, Roar.Callback<WebObjects.User.NetdriveSaveResponse> callback)
  {
    Data_[ key ] = val;

	WebObjects.User.NetdriveSaveArguments args = new Roar.WebObjects.User.NetdriveSaveArguments();
	args.ikey=key;
	args.data=val;
		
    user_actions_.netdrive_save( args, new OnSetData(callback, this, key, val) );
  }
  
  class OnSetData : CBBase<WebObjects.User.NetdriveSaveResponse>
  {
    protected Data data;
    protected string key;
    protected string value;

    public OnSetData( Roar.Callback<WebObjects.User.NetdriveSaveResponse> in_cb, Data in_data, string in_key, string in_value) : base(in_cb)
    {
      data = in_data;
      key = in_key;
      value = in_value;
    }

    public override void HandleSuccess( CallbackInfo<WebObjects.User.NetdriveSaveResponse> info )
    {
      RoarManager.OnDataSaved(key, value);
    }
  }

}

}
