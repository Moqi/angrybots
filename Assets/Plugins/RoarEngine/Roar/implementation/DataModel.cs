using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DC = Roar.DataConversion;
using Roar.DomainObjects;

public interface IDataModel<CT,DT> where DT:class
{
	void Clear();
	void Clear( bool silent );
	bool HasDataFromServer { get; set; }
	
	bool Fetch (Roar.Callback< IDictionary<string,CT> > cb);
	bool Fetch (Roar.Callback< IDictionary<string,CT> > cb, Hashtable p);
	bool Fetch (Roar.Callback< IDictionary<string,CT> > cb, Hashtable p, bool persist);
	
	IList<CT> List ();
	CT Get (string key);
	
	DataModel<CT,DT> Set ( Dictionary<string,CT> data);
	DataModel<CT,DT> Set ( Dictionary<string,CT> data, bool silent);

	
	void Unset (string key);
	void Unset (string key, bool silent);

	
	void AddOrUpdate(string key, CT value);
}


public class DataModel<CT,DT> : IDataModel<CT,DT> where DT:class
{
	public Dictionary<string,CT> attributes = new Dictionary<string,CT> ();
	private Dictionary<string,CT> previousAttributes = new Dictionary<string,CT>();
	private bool hasChanged = false;
	private bool isServerCalling = false;
	public bool HasDataFromServer { get; set; }
	public string name;
	public IDomGetter<DT> getter;
	public IDomToCache<DT,CT> converter;
	
	
	protected Roar.ILogger logger;
	
	public DataModel (string name, IDomGetter<DT> getter, IDomToCache<DT,CT> converter, Roar.ILogger logger)
	{
		this.name = name;
		this.logger = logger;
		this.getter = getter;
		this.converter = converter;
	}

	// Return code for calls attempting to access/modify Model data
	// if none is present
	private void OnNoData ()
	{
		OnNoData (null);
	}

	private void OnNoData (string key)
	{
		string msg = "No data intialised for Model: " + name;
		if (key != null)
			msg += " (Invalid access for \"" + key + "\")";

		logger.DebugLog ("[roar] -- " + msg);
	}

	// Removes all attributes from the model
	public void Clear () { Clear(false); }

	// Removes all attributes from the model
	public void Clear (bool silent)
	{
		attributes = new Dictionary<string,CT> ();

		// Set internal changed flag
		this.hasChanged = true;

		if (!silent) {
			RoarManager.OnComponentChange (name);
		}
	}


	// Internal call to retrieve model data from server and pass back
	// to `callback`. `params` is optional obj to pass to RoarAPI call.
	// `persistModel` optional can prevent Model data clearing.
	public bool Fetch (Roar.Callback< IDictionary<string,CT> > cb)
	{
		return Fetch (cb, null, false);
	}

	public bool Fetch (Roar.Callback< IDictionary<string,CT> > cb, Hashtable p)
	{
		return Fetch (cb, p, false);
	}

	//TODO: Should this take Roar.Callback<CT> instead?a
	public bool Fetch (Roar.Callback< IDictionary<string,CT> > cb, Hashtable p, bool persist)
	{
		// Bail out if call for this Model is already underway
		if (this.isServerCalling)
			return false;

		// Reset the internal register
		if (!persist)
			attributes = new Dictionary<string,CT> ();

		getter.get( new OnFetch( cb, this ) );

		this.isServerCalling = true;
		return true;
	}

	private class OnFetch : ZWebAPI.Callback<DT>
	{
		protected DataModel<CT,DT> model;
		protected Roar.Callback< IDictionary<string,CT> > cb;

		public OnFetch (Roar.Callback< IDictionary<string,CT> > in_cb, DataModel<CT,DT> in_model)
		{
			model = in_model;
			cb = in_cb;
		}

		public void OnError ( Roar.RequestResult info)
		{
			// Reset this function call
			model.isServerCalling = false;
			if(cb!=null) cb( new Roar.CallbackInfo< IDictionary<string,CT> >(null, info.code, info.msg ) );
		}

		public void OnSuccess(Roar.CallbackInfo<DT> info)
		{
			model.isServerCalling = false;
			model.logger.DebugLog ("onFetch got given: " + info.data.ToString() );
			model.ProcessData (info.data, cb);
		}
	}

	// Preps the data from server and places it within the Model
	// Not clear whether this should ammend or replace the data
	// At the moment it ammends!
	private void ProcessData ( DT d, Roar.Callback< IDictionary<string,CT> > cb)
	{
		Dictionary<string,CT> o = converter.convert(d);

		// Flag server cache called
		// Must do before `set()` to flag before change events are fired
		HasDataFromServer = true;

		// Update the Model
		this.Set (o);
		if(cb!=null) cb( new Roar.CallbackInfo< IDictionary<string,CT> >( o, WebAPI.OK, null ) );

		logger.DebugLog ("Setting the model in " + name + " to : " + Roar.Json.ObjectToJSON (o));
		logger.DebugLog ("[roar] -- Data Loaded: " + name);

		// Broadcast data ready event
		RoarManager.OnComponentReady (this.name);
	}


	// Shallow clone object
	public static Dictionary<string,CT> Clone (Dictionary<string,CT> obj)
	{
		return (obj == null) ? null : new Dictionary<string, CT>( obj );
	}


	// Have to prefix 'set' as '_set' due to Unity function name restrictions
	public DataModel<CT,DT> Set (Dictionary<string,CT> data)
	{
		return Set (data, false);
	}

	//TODO: This should probably be called "Add" as it seems to 
	// update/add to the model rather than replace all the entries.
	
	public DataModel<CT,DT> Set ( Dictionary<string,CT> data, bool silent)
	{
		// Setup temporary copy of attributes to be assigned
		// to the previousAttributes register if a change occurs
		var prev = Clone (this.attributes);

		foreach (KeyValuePair<string,CT> prop in data) {
			this.attributes [prop.Key] = prop.Value;

			// Set internal changed flag
			this.hasChanged = true;

			// Broadcasts an attribute specific change event of the form:
			// **change:attribute_name**
			if (!silent) {
				RoarManager.OnComponentChange (this.name);
			}
		}

		// Broadcasts a `change` event if the model changed
		if (HasChanged && !silent) {
			this.previousAttributes = prev;
			this.Change ();
		}

		return this;
	}


	// Removes an attribute from the data model
	// and fires a change event unless `silent` is passed as an option
	public void Unset (string key)
	{
		Unset (key, false);
	}

	public void Unset (string key, bool silent)
	{
		// Setup temporary copy of attributes to be assigned
		// to the previousAttributes register if a change occurs
		var prev = Clone (this.attributes);

		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return;
		}

		if (this.attributes [key] != null) {
			// Remove the specific element
			this.attributes.Remove (key);

			this.hasChanged = true;
			// Broadcasts an attribute specific change event of the form:
			// **change:attribute_name**
			if (!silent) {
				RoarManager.OnComponentChange (this.name);
			}
		}

		// Broadcasts a `change` event if the model changed
		if (HasChanged && !silent) {
			this.previousAttributes = prev;
			this.Change ();
		}
	}


	public CT RawGet(string key)
	{
		Debug.Log("========= TRYING TO GET KEY [" + key + "]");
		if(!HasDataFromServer) { return default(CT); }
		if (attributes.ContainsKey(key)) {
			return this.attributes[key];
		}
		return default(CT);
	}

	// Returns the value of a given data key (usually an object)
	// Using '_get' due to Unity restrictions on function names
	public CT Get (string key)
	{
		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return default(CT);
		}

		CT retval = default(CT);
		if( ! this.attributes.TryGetValue(key, out retval) )
		{
			logger.DebugLog ("[roar] -- No property found: " + key);
		}
		return retval;
	}


	// Returns an array of all the elements in this.attributes
	public IList<CT> List ()
	{
		var l = new List<CT>();

		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData();
			return l;
		}

		foreach (KeyValuePair<string,CT> prop in this.attributes) {
			l.Add (prop.Value);
		}
		return l;
	}

	// Returns the object of an attribute key from the PREVIOUS register
	public CT Previous (string key)
	{
		// Check that server data is present
		if (!HasDataFromServer) {
			this.OnNoData (key);
			return default(CT);
		}

		return this.previousAttributes[key];
	}

	// Flag to indicate whether the model has changed since last "change" event
	public bool HasChanged
	{
		get
		{
			return hasChanged;
		}
	}

	// Manually fires a "change" event on this model
	public void Change ()
	{
		RoarManager.OnComponentChange (this.name);
		this.hasChanged = false;
	}
	
	public void AddOrUpdate( string key, CT value )
	{
		attributes[key]=value;
		Change();
	}
}