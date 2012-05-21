using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/**
 * Implementation of the IRoarIO interface.
 * This is the class you need to drag onto your unity empty to start using the
 * RoarIO framework. However once that is done you should only use the object
 * through the IRoarIO interface. That is your unity scripts should look
 * something like this
 *
 *    var roar_:IRoarIO
 *    function Awake()
 *    {
 *      roar_ = GetComponent(RoarIO) as IRoarIO
 *    }
 *
 * Further documentation about how you can use the RoarIO object
 * can be found in the IRoarIO class.
 */

public class RoarIO : MonoBehaviour, IRoarIO, IRoarInternal
{


  public bool debug = true;
  public string gameKey="";

  public Roar.Components.IConfig Config { get { return Config_; } }
  protected Roar.Components.IConfig Config_;

  public global::IWebAPI WebAPI { get { return WebAPI_; } }
  protected global::IWebAPI WebAPI_;

  public Roar.Components.IUser User { get { return User_; } }
  protected Roar.Components.IUser User_;

  public Roar.Components.IProperties Properties { get { return Properties_; } }
  protected Roar.Components.IProperties Properties_;

  public Roar.Components.IData Data { get { return Data_; } }
  protected Roar.Components.IData Data_;

  public Roar.Components.IInventory Inventory { get { return Inventory_; } }
  protected Roar.Components.IInventory Inventory_ = null;

  public Roar.Components.IShop Shop { get { return Shop_; } }
  protected Roar.Components.IShop Shop_;

  public Roar.Components.IActions Actions { get { return Actions_; } }
  protected Roar.Components.IActions Actions_;

  public Roar.Adapters.IUrbanAirship UrbanAirship { get{ return UrbanAirship_;} }
  protected Roar.implementation.Adapters.UrbanAirship UrbanAirship_;


  public string AuthToken { get { return WebAPI_.roarAuthToken; } }

  /**
   * Called by unity when everything is ready to go.
   * We use this rather than the constructor as its what unity suggests.
   */
  public void Awake()
  {
    Roar.implementation.DataStore data_store = new Roar.implementation.DataStore(this);
    WebAPI_ = new global::WebAPI(this,this);
    User_ = new Roar.implementation.Components.User(this,data_store);
    Config_ = new Roar.implementation.Components.Config(WebAPI_); //TODO: This Component is a bit weird it would be nicer if it behaved like all the others.
    Properties_ = new Roar.implementation.Components.Properties( this, data_store );
    Inventory_ = new Roar.implementation.Components.Inventory( this, data_store );
    Data_ = new Roar.implementation.Components.Data( this, data_store );
    Shop_ = new Roar.implementation.Components.Shop( this, data_store );
    Actions_ = new Roar.implementation.Components.Actions( this, data_store );

    UrbanAirship_ = new Roar.implementation.Adapters.UrbanAirship(WebAPI_);


    DontDestroyOnLoad( this );

    // Apply public settings 
    Config.setVal( "game", gameKey );
  }

  public void Start()
  {
    if(UrbanAirship_!=null) UrbanAirship_.OnStart();
  }

  public void OnUpdate()
  {
    if(UrbanAirship_!=null) UrbanAirship_.OnUpdate();
  }

  string _version="1.0.0";

  public string version( Roar.Callback callback = null )
  {
    if(callback!=null) callback( new Roar.CallbackInfo( _version ) );
    return _version;
  }

  public void login( string username, string password, Roar.Callback callback=null )
  {
    User.doLogin(username,password,callback);
  }

  public void login_facebook_oauth( string oauth_token, Roar.Callback callback=null )
  {
    User.doLoginFacebookOAuth(oauth_token,callback);
  }

  public void logout( Roar.Callback callback=null )
  {
    User.doLogout(callback);
  }

  public void create( string username, string password, Roar.Callback callback=null )
  {
    User.doCreate(username,password,callback);
  }


  public string whoami( Roar.Callback callback=null )
  {
    if (callback!=null) callback( new Roar.CallbackInfo(Properties.getValue( "name" )) );
    return Properties.getValue( "name" );
  }

  public bool isDebug()
  {
    return this.debug;
  }

  public void doCoroutine(IEnumerator methodName)
  {
    this.StartCoroutine(methodName);
  }
}
