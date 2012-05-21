using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DC = Roar.implementation.DataConversion;

public class DataModel
{
  public string _name;
  public Hashtable attributes = new Hashtable();

  private Hashtable _previousAttributes = new Hashtable();
  private bool _hasChanged = false;
  private string _serverDataAPI;
  private string _node;

  private bool _isServerCalling = false;
  public bool hasDataFromServer = false;

  protected IRoarInternal roar_internal_;
  protected DC.IXmlToHashtable xmlParser_;

  public DataModel( string name, string url, string node, ArrayList conditions, IRoarInternal roar_internal, DC.IXmlToHashtable xmlParser )
  {
    _name = name;
    _serverDataAPI = url;
    _node = node;
    roar_internal_ = roar_internal;
    xmlParser_ = xmlParser;
  }

  // Return code for calls attempting to access/modify Model data
  // if none is present
  private void onNoData() { onNoData( null ); }
  private void onNoData( string key )
  {
    string msg = "No data intialised for Model: " + _name;
    if (key!=null) msg  += " (Invalid access for \""+key+"\")";

    if (roar_internal_.isDebug()) Debug.Log( "[roar] -- "+msg );
  }

  // Removes all attributes from the model
  public void clear( bool silent = false )
  { 
    attributes = new Hashtable();
    
    // Set internal changed flag
    this._hasChanged = true;

    if ( !silent ) { RoarIOManager.OnComponentChange( _name ); }
  }


  // Internal call to retrieve model data from server and pass back
  // to `callback`. `params` is optional obj to pass to RoarAPI call.
  // `persistModel` optional can prevent Model data clearing.
  public bool fetch( Roar.Callback cb ) { return fetch( cb, null, false ); }
  public bool fetch( Roar.Callback cb, Hashtable p ) { return fetch( cb, p, false ); }
  public bool fetch( Roar.Callback cb, Hashtable p, bool persist ) 
  {
    // Bail out if call for this Model is already underway
    if (this._isServerCalling) return false;

    // Reset the internal register
    if (!persist) attributes = new Hashtable();

    // Using direct call (serverDataAPI url) rather than API mapping
    // - Unity doesn't easily support functions as strings: func['sub']['mo']()
    Hashtable args = new Hashtable();
    args["cb"] = cb;
    roar_internal_.doCoroutine( roar_internal_.WebAPI.sendCore( _serverDataAPI, p, onFetch, args) );

    this._isServerCalling = true;
    return true;
  }

  private void onFetch( IXMLNode d, int code, string msg, string id, Hashtable opt)
  {
    // Retrieve the final callback from the options payload
    Roar.Callback _cb = opt["cb"] as Roar.Callback;

    // Reset this function call
    this._isServerCalling = false;
    
    if (code != 200)
    {
      if (_cb!=null) _cb( new Roar.CallbackInfo(null,code,msg) );
      return;
    }

    Debug.Log ("onFetch got given: "+d.DebugAsString() );

    // First process the data for Model use
    string[] t = _serverDataAPI.Split('/');
    if( t.Length != 2 ) throw new System.ArgumentException("Invalid url format - must be abc/def");
    string path = "roar>0>"+t[0]+">0>"+t[1]+">0>"+_node;
    List<IXMLNode> nn = d.GetNodeList(path);
    if(nn==null)
    {
      Debug.Log ( string.Format("Unable to get node\nFor path = {0}\nXML = {1}", path, d.DebugAsString()) );
    }
    else
    {
      this._processData( nn );
    }

    // Note: temporarily disabling 'second' notice (first is in _processData)
    // // Debug notice if used
    // if (RoarIO.instance.debug) Debug.Log( '---Model:'+this._name+'\n' + RoarIO.HashToString( this.attributes ) );

    // Run the user callback
    if (_cb!=null) _cb( new Roar.CallbackInfo(this.attributes,code,msg) );
  }

  // Preps the data from server and places it within the Model
  private void _processData( List<IXMLNode> d )
  {
    Hashtable _o = new Hashtable();

    if (d==null) Debug.Log("[roar] -- No data to process!");
    else
    {
      for (var i=0; i<d.Count; i++)
      {
        string key = xmlParser_.GetKey(d[i]);
        if(key==null)
        {
          Debug.Log( string.Format ("no key found for {0}", d[i].DebugAsString() ) );
          continue;
        }
        Hashtable hh = xmlParser_.BuildHashtable(d[i]);
        if( _o.ContainsKey(key) )
        {
          Debug.Log ("Duplicate key found");
        }
        else
        {
          _o[key] = hh;
        }
      }
    }

    // Flag server cache called
    // Must do before `set()` to flag before change events are fired
    this.hasDataFromServer = true;

    // Update the Model
    this._set( _o );

    Debug.Log ("Setting the model in "+_name+" to : "+Roar.Json.ObjectToJSON(_o) );

    // Box.Debug.log('-Server Data-', this._name, this.attributes)
    if (roar_internal_.isDebug()) Debug.Log("[roar] -- Data Loaded: " + _name);

    // Broadcast data ready event
    RoarIOManager.OnComponentReady(this._name);
  }


  // Shallow clone object
  public static Hashtable _clone( Hashtable obj )
  {
    if (obj==null) return null;

    Hashtable copy = new Hashtable();
    foreach (DictionaryEntry prop in obj)
    {
      copy[ prop.Key ] = prop.Value;
    }

    return copy;
  }


  // Have to prefix 'set' as '_set' due to Unity function name restrictions
  public DataModel _set( Hashtable data ) { return _set(data,false); }
  public DataModel _set( Hashtable data, bool silent )
  {
    // Setup temporary copy of attributes to be assigned
    // to the previousAttributes register if a change occurs
    var prev = _clone( this.attributes );

    foreach (DictionaryEntry prop in data)
    {
      this.attributes[ prop.Key ] = prop.Value;

      // Set internal changed flag
      this._hasChanged = true;

      // Broadcasts an attribute specific change event of the form:
      // **change:attribute_name**
      if (!silent) { RoarIOManager.OnComponentChange(this._name); }
    }

    // Broadcasts a `change` event if the model changed
    if (hasChanged() && !silent) 
    { 
      this._previousAttributes = prev;
      this.change();
    }

    return this;
  }


  // Removes an attribute from the data model
  // and fires a change event unless `silent` is passed as an option
  public void unset( string key ) { unset(key,false); }
  public void unset( string key, bool silent )
  {
    // Setup temporary copy of attributes to be assigned
    // to the previousAttributes register if a change occurs
    var prev = _clone( this.attributes );

    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return; }

    if (this.attributes[key]!=null)
    {
      // Remove the specific element
      this.attributes.Remove( key );

      this._hasChanged = true;
      // Broadcasts an attribute specific change event of the form:
      // **change:attribute_name**
      if (!silent) { RoarIOManager.OnComponentChange(this._name); }
    }

    // Broadcasts a `change` event if the model changed
    if (hasChanged() && !silent) 
    {
      this._previousAttributes = prev;
      this.change();
    }
  }

  // Returns the value of a given data key (usually an object)
  // Using '_get' due to Unity restrictions on function names
  public Hashtable _get( string key )
  {
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return null; }

    if (this.attributes[key]!=null) { return this.attributes[key] as Hashtable; }
    Debug.Log("[roar] -- No property found: "+key);
    return null;
  }

  // Returns the embedded value within an object attribute
  public string getValue( string key )
  {
    var o = this._get(key);
    if (o!=null) return o["value"] as string;
    else return null;
  }

  // Returns an array of all the elements in this.attributes
  public ArrayList list()
  {
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData(); return null; }

    var l = new ArrayList();
    foreach (DictionaryEntry prop in this.attributes)
    {
      l.Add( prop.Value );
    }
    return l;
  }

  // Returns the object of an attribute key from the PREVIOUS register
  public Hashtable previous( string key )
  {
    // Check that server data is present
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return null; }

    if (this._previousAttributes[key]!=null) return this._previousAttributes[key] as Hashtable;
    else return null;
  }

  // Checks whether element `key` is present in the
  // list of ikeys in the Model. Optional `number` to search, default 1
  // Returns true if player has equal or greater number, false if not, and
  // null for an invalid query.
  public bool has( string key ) { return has( key, 1); }
  public bool has( string key, int number )
  {
    // Fire warning *only* if no data intitialised, but continue
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return false; }

    int count = 0;
    foreach (DictionaryEntry i in this.attributes)
    {
      // Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
      if ( (i.Value as Hashtable)["ikey"] as string == key) count++;
      else if ( (i.Value as Hashtable)["id"] as string == key) count++;
      else if ( (i.Value as Hashtable)["shop_ikey"] as string == key) count++;
    }

    if (count >= number) return true;
    else { return false; }
  }

  // Similar to Model.has(), but returns the number of elements in the
  // Model of id or ikey `key`.
  public int quantity( string key )
  {
    // Fire warning *only* if no data initialised, but continue
    if ( !this.hasDataFromServer ) { this.onNoData( key ); return 0; }

    int count = 0;
    foreach (DictionaryEntry i in this.attributes)
    {
      // Search `ikey`, `id` and `shop_ikey` keys and increment counter if found
      if ( (i.Value as Hashtable)["ikey"] as string == key) count++;
      else if ( (i.Value as Hashtable)["id"] as string == key) count++;
      else if ( (i.Value as Hashtable)["shop_ikey"] as string == key) count++;
    }

    return count;
  }

  // Flag to indicate whether the model has changed since last "change" event
  public bool hasChanged()
  {
    return this._hasChanged;
  }

  // Manually fires a "change" event on this model
  public void change()
  {
    RoarIOManager.OnComponentChange(this._name);
    this._hasChanged = false;
  }
}
