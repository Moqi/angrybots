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

namespace Roar.implementation.Components
{

public class Actions : IActions
{
  protected DataStore data_store_;
  protected IWebAPI.ITasksActions task_actions_;

  public Actions( IWebAPI.ITasksActions task_actions, DataStore data_store )
  {
		task_actions_ = task_actions;
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

    task_actions_.start( args, onActionsDo, callback_info );
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