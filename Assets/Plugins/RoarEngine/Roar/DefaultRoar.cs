using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Logger : Roar.ILogger
{
	public void DebugLog( string k )
	{
		if (Debug.isDebugBuild)
			Debug.Log (k);
	}
}

/**
 * Implementation of the IRoar interface.
 * This is the class you need to drag onto your unity empty to start using the
 * Roar framework. However once that is done you should only use the object
 * through the IRoar interface. That is your unity scripts should look
 * something like this
 *
 *    var roar_:IRoar
 *    function Awake()
 *    {
 *      roar_ = GetComponent(DefaultRoar) as IRoar
 *    }
 *
 * Further documentation about how you can use the Roar object
 * can be found in the IRoar class.
 */

public class DefaultRoar : MonoBehaviour, IRoar, IUnityObject
{
	// These are purely to enable the values to show up in the unity UI.
	public bool debug = true;
	public bool appstoreSandbox = true;
	public string gameKey = string.Empty;
	public enum XMLType { Lightweight, System };
	public GUISkin defaultGUISkin;

	public Roar.IConfig Config { get { return config; } }
	protected Roar.IConfig config;

	public IWebAPI WebAPI { get { return webAPI; } }
	protected IWebAPI webAPI;
	
	public Roar.Components.IFriends Friends { get { return friends; } }
	protected Roar.Components.IFriends friends;

	public Roar.Components.IUser User { get { return user; } }
	protected Roar.Components.IUser user;

	public Roar.Components.IFacebook Facebook { get { return facebook; } }
	protected Roar.Components.IFacebook facebook;

	public Roar.Components.IProperties Properties { get { return properties; } }
	protected Roar.Components.IProperties properties;

	public Roar.Components.ILeaderboards Leaderboards { get { return leaderboards; } }
	protected Roar.Components.ILeaderboards leaderboards;

	//public Roar.Components.IRanking Ranking { get { return Ranking_; } }
	//protected Roar.Components.IRanking Ranking_;

	public Roar.Components.IInventory Inventory { get { return inventory; } }
	protected Roar.Components.IInventory inventory = null;
	
	public Roar.Components.IData Data { get { return data; } }
	protected Roar.Components.IData data;

	public Roar.Components.IShop Shop { get { return shop; } }
	protected Roar.Components.IShop shop;

	public Roar.Components.ITasks Tasks { get { return tasks; } }
	protected Roar.Components.ITasks tasks;

	public Roar.Components.IAchievements Achievements { get { return achievements; } }
	protected Roar.Components.IAchievements achievements;

	public Roar.Components.IGifts Gifts { get { return gifts; } }
	protected Roar.Components.IGifts gifts;

	public Roar.Components.IInAppPurchase Appstore { get{ return appstore;} }
	protected Roar.implementation.Components.InAppPurchase appstore;

	public Roar.Adapters.IUrbanAirship UrbanAirship { get{ return urbanAirship;} }
	protected Roar.implementation.Adapters.UrbanAirship urbanAirship;

	public string AuthToken { get { return config.AuthToken; } }

	private static DefaultRoar instance;
	private static IRoar api;
	private Roar.implementation.IDataStore datastore;
	private Logger logger = new Logger();

	/**
	 * Access to the Roar Engine singleton.
	 */
	public static DefaultRoar Instance
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(DefaultRoar)) as DefaultRoar;
				if (instance == null)
					Debug.LogWarning("Unable to locate the Roar interface.");
			}
			return instance;
		}
	}

	/**
	 * Access to the Roar Engine API singleton.
	 */
	public static IRoar API
	{
		get
		{
			if (api == null)
			{
				DefaultRoar defaultRoar = Instance;
				if (defaultRoar != null)
					api = (IRoar)defaultRoar;
			}
			return api;
		}
	}

	/**
	 * Called by unity when everything is ready to go.
	 * We use this rather than the constructor as its what unity suggests.
	 */
	public void Awake()
	{
		config = new Roar.implementation.Config();

		// Apply public settings
		string key = gameKey.ToLower();
		       //key = key.Replace("_", "");
		Config.Game = key;
		Config.IsDebug = debug;

		RequestSender api = new RequestSender(config,this,logger);
		webAPI = new global::WebAPI(api);
		datastore = new Roar.implementation.DataStore(webAPI, logger);
		friends = new Roar.implementation.Components.Friends( webAPI.friends, datastore, logger);
		user = new Roar.implementation.Components.User(webAPI.user,datastore, logger);
		properties = new Roar.implementation.Components.Properties( datastore );
		leaderboards = new Roar.implementation.Components.Leaderboards(webAPI,datastore, logger);
		inventory = new Roar.implementation.Components.Inventory( webAPI.items, datastore, logger);
		data = new Roar.implementation.Components.Data( webAPI.user, datastore, logger);
		shop = new Roar.implementation.Components.Shop( webAPI.shop, datastore, logger );
		tasks = new Roar.implementation.Components.Tasks( webAPI.tasks, datastore );
		gifts = new Roar.implementation.Components.Gifts(webAPI.mail, datastore, logger);
		facebook = new Roar.implementation.Components.Facebook(webAPI.facebook, datastore, logger);

		if (!Application.isEditor)
		{
			appstore = new Roar.implementation.Components.InAppPurchase( webAPI.appstore, "Roar", logger, appstoreSandbox );
		}

		urbanAirship = new Roar.implementation.Adapters.UrbanAirship(webAPI);

		DontDestroyOnLoad(gameObject);
	}

	public void Start()
	{
		if(urbanAirship!=null) urbanAirship.OnStart();
	}

	public void OnUpdate()
	{
		if(urbanAirship!=null) urbanAirship.OnUpdate();
	}

	string version="1.0.0";

	public string Version( )
	{
		return version;
	}



	public string PlayerId()
	{
		return Properties.GetValue( "id" );
	}

	public bool IsDebug{ get { return Config.IsDebug; } }

	public void DoCoroutine( IEnumerator method )
	{
		this.StartCoroutine(method);
	}

	public Roar.implementation.IDataStore DataStore
	{
		get { return datastore; }
	}

	public Logger Logger
	{
		get { return logger; }
	}

	#region EXTERNAL CALLBACKS
	void OnAppstoreProductData(string productDataXml)
	{
		Appstore.OnProductData(productDataXml);
	}
	void OnAppstoreRequestProductDataInvalidProductId(string invalidProductId)
	{
		Appstore.OnInvalidProductId(invalidProductId);
	}
	void OnAppstoreProductPurchaseComplete(string purchaseXml)
	{
		Appstore.OnPurchaseComplete(purchaseXml);
	}
	void OnAppstoreProductPurchaseCancelled(string productIdentifier)
	{
		Appstore.OnPurchaseCancelled(productIdentifier);
	}
	void OnAppstoreProductPurchaseFailed(string errorXml)
	{
		Appstore.OnPurchaseFailed(errorXml);
	}
	#endregion
}
