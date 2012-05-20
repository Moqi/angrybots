#pragma strict

import System.Collections;
import UnityEngine;
import Roar.Components;
 
/**
 * roar.io user authentication details.
 **/
var game_name = "angrybots";
var user_name = "bobo";
var password  = "bobo";

var pauseIcon : Texture2D;
private var cornerTextureSize = 48.0f;
private var fullScreenAvailable:boolean = false;
private var quitEnabled:boolean = true;
private var directKeyQuit:boolean = true;

//IXMLNodeFactory.instance = new SystemXMLNodeFactory();		
IXMLNodeFactory.instance = new XMLNodeFactory();

/**
 * The various states of the demo application.
 **/
public enum UIState {
  Login,
  Create,
  Authenticating,
  CreatingUser,
  RoarLoading,
  InGame,
  About,
  Shop,
  Inventory,
  Main
};

// the current game state
private var uiState : UIState = UIState.Login;

// the roar.io api interface
private var roar:IRoarIO;
private var inventory:IInventory;
private var shop:IShop;

private var menuDisabled:boolean = false;
private var isAuthenticated = false;
private var confirmText:String;

private var anonToolbarIndex : int = 1;
private var anonToolbarStrings : String[] = ["Main", "Login", "Create", "About"];
private var anonToolbarUIStates : UIState[] = [UIState.Main, UIState.Login, UIState.Create, UIState.About];

private var userToolbarIndex : int = 0;
private var userToolbarStrings : String[] = ["Main", "Inventory", "Shop", "About"];
private var userToolbarUIStates : UIState[] = [UIState.Main, UIState.Inventory, UIState.Shop, UIState.About];

private var inventorySelectIndex = -1;
private var equippedSelectIndex = -1;
private var selectedItemId;
private var inventoryScroll : Vector2;
private var equippedScroll : Vector2;


private var shopSelectIndex = -1;
private var shopScroll : Vector2;
private var shopSelectId;

private var inMenu = true;

function Awake()
{
  roar = gameObject.Find("Roar").GetComponent(RoarIO) as IRoarIO;
}

function Start () {
  	roar.Config.setVal( 'game', game_name);
  	inventory = roar.Inventory;
  	shop = roar.Shop;
  
  	UpdateAudio ();
	
	switch (Application.platform)
	{
		case RuntimePlatform.OSXWebPlayer:
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.NaCl:
			fullScreenAvailable = true;
			quitEnabled = false;
			directKeyQuit = false;
		break;
		case RuntimePlatform.FlashPlayer:
			fullScreenAvailable = false;
			quitEnabled = false;
			directKeyQuit = false;
		break;
		case RuntimePlatform.OSXPlayer:
		case RuntimePlatform.WindowsPlayer:
			fullScreenAvailable = true;
			directKeyQuit = false;
		break;
	}
}

function FlipPause() {
	inMenu = !inMenu;
	if(inMenu) {
	// the menu has been activated, set the ui state to the correct menu screen
		if(isAuthenticated) {
			setUIState(userToolbarUIStates[userToolbarIndex]);
		} else {
			setUIState(anonToolbarUIStates[anonToolbarIndex]);
		}
	}
}

function CanPlay() {
	return isAuthenticated == true;
}

function OnGUI () {

	if(CanPlay()) {
		// render the pause buton
		var rightRect = Rect(Screen.width - cornerTextureSize, 0.0f, cornerTextureSize, cornerTextureSize);
		switch (Event.current.type)
		{
			case EventType.Repaint:
				GUI.DrawTexture (rightRect, pauseIcon);
			break;
			case EventType.MouseDown:
				if (rightRect.Contains (Event.current.mousePosition))
				{
					FlipPause ();
					Event.current.Use ();
				}
			case EventType.MouseUp:
				if (rightRect.Contains (Event.current.mousePosition))
				{ 
					Event.current.Use ();
				}
			break;
		}
	}

	// this if as far as we need to go if the user is not viewing the menu
	if(inMenu) {
		Time.timeScale = 0.0f;
	} else {
		Time.timeScale = 1.0f;
		return;
	}
	// either handle rendering of various loading screen states
	// or render the menu and the current menu screen
	switch(uiState) {
	  	case UIState.Authenticating:
	  		handleAuthenticating();
	  		break;
	  	case UIState.CreatingUser:
	  		handleCreatingUser();
	  		break;	  		
	  	case UIState.RoarLoading:
	  		handleRoarLoading();
	  		break;
		default:
			renderMenu();
			break;
	}
}

function renderMenu() {
	var menuWidth = Screen.width - 50;
	var menuHeight = Screen.height - 50;

	var menuRect = RenderUtils.getCentredRect(menuWidth, menuHeight);
	
	// render the menu container
	if(this.menuDisabled) {
		GUI.enabled = false;
	}
	GUILayout.BeginArea (menuRect, GUI.skin.GetStyle("Box"));
	GUILayout.BeginVertical();
		
		// render the toolbar
		var currentToolbarIndex = 0;
		if(isAuthenticated == false) {
			currentToolbarIndex = anonToolbarIndex;
			anonToolbarIndex = GUILayout.Toolbar(anonToolbarIndex, anonToolbarStrings);
			if(anonToolbarIndex != currentToolbarIndex) {
				setUIState(anonToolbarUIStates[anonToolbarIndex]);
			}
		} else {
			currentToolbarIndex = userToolbarIndex;
			userToolbarIndex = GUILayout.Toolbar(userToolbarIndex, userToolbarStrings);
			if(userToolbarIndex != currentToolbarIndex) {
				setUIState(userToolbarUIStates[userToolbarIndex]);
			}
		}
		
		// now render the current menu page
		switch(uiState) {
		  	case UIState.Login:
		  		renderLogin(false);
		  		break;
		  	case UIState.Create:
		  		renderLogin(true);
		  		break;		  		
		  	case UIState.About:
		  		AboutScreen.render();
		  		break;
		  	case UIState.Main:
		  		renderMain();
		  		break;
		  	case UIState.Shop:
		  		renderShop();
		  		break;
		  	case UIState.Inventory:
		  		renderInventory();
		  		break;
			default:
				break;
		};
	GUILayout.EndVertical();
	GUILayout.EndArea ();
	GUI.enabled = true;
}

function singleColumnBegin() {
	GUILayout.BeginHorizontal();
	GUILayout.FlexibleSpace();
	GUILayout.BeginVertical(GUILayout.MaxWidth(200.0f));
	GUILayout.FlexibleSpace();
}

function singleColumnEnd() {
	GUILayout.FlexibleSpace();
	GUILayout.EndVertical();
	GUILayout.FlexibleSpace();
	GUILayout.EndHorizontal();
}

function renderLogin(create:boolean) {
	singleColumnBegin();
		GUILayout.Box("username");
		user_name = GUILayout.TextField(user_name);
		GUILayout.Space(4.0f);
		GUILayout.Box("password");
		password = GUILayout.TextField(password);
		GUILayout.Space(10.0f);
		if(create) {
			if (GUILayout.Button( "Create User" ))
		    {
		      setUIState(UIState.CreatingUser);
		      roar.create(user_name, password, function(d:Roar.CallbackInfo){});
		    }		
		} else {
			if (GUILayout.Button("Login")) 
		    {
		      setUIState(UIState.Authenticating);
		      roar.login(user_name, password, function(d:Roar.CallbackInfo){});
		    }
	    }
	singleColumnEnd();
}	


function renderMain() {
	singleColumnBegin();
	
	GUILayout.Space(50.0f);
	
	if(GUILayout.Button( "Resume" ))
	{
		FlipPause();
	}
	
	if (fullScreenAvailable)
	{
		GUILayout.FlexibleSpace ();
		if (GUILayout.Button("Fullscreen"))
		{
			FlipFullscreen ();
		}
	}
	
	#if !UNITY_FLASH
		GUILayout.FlexibleSpace ();
	
		if (GUILayout.Button("Mute"))
		{
			FlipMute ();
		}
	#endif
	
	GUILayout.FlexibleSpace ();
	
	if (GUILayout.Button("Restart"))
	{
		Restart ();
	}
	
	GUILayout.FlexibleSpace ();

   	if (GUILayout.Button( "Quit" ))
    {
      Application.Quit ();
    }
    GUILayout.Space(50.0f);
    
    singleColumnEnd();
}

function renderShop() {
	GUILayout.BeginHorizontal();
	
	// inventory left column start
	GUILayout.BeginVertical();
	
		GUILayout.Box("Inventory");

		var shopItems = shop.list() as ArrayList;
		
		shopScroll = GUILayout.BeginScrollView(shopScroll);
		
		var labels:ArrayList = new ArrayList();
		var itemIds:ArrayList = new ArrayList();  
		for(var i=0;i<shopItems.Count;i++) {
			var item:Hashtable = shopItems[i];
			labels.Add(item["label"]);
			itemIds.Add(item["shop_ikey"]);
		}
		if(labels.Count > 0) {
			var shopItemLabels:String[] = labels.ToArray(typeof(String));
			shopSelectIndex = GUILayout.SelectionGrid(shopSelectIndex, shopItemLabels, 3);
			
			if(shopSelectIndex == -1) {
				shopSelectId = null;
			} else {
				shopSelectId = itemIds[shopSelectIndex];
			}
		} else {
			GUILayout.Label("no goods available at this time");
		}
		
		GUILayout.EndScrollView();
	
	// inventory left column end
	GUILayout.EndVertical();
	
	// inventory right column start
	GUILayout.BeginVertical(GUILayout.MinWidth(300));
	
	if(shopSelectId) {
		var selectedItem:Hashtable = shop.getShopItem(shopSelectId);
		if(selectedItem == null) {
			shopSelectId = null;
		} else {
			GUILayout.Box("Item Profile");
			GUILayout.Label(selectedItem["label"] as String);
			GUILayout.Label(selectedItem["ikey"] as String);
			GUILayout.Box("Actions");
	
			var canBuy = true;
			if( canBuy )
	      	{
		        if(GUILayout.Button("Buy"))
		        {
		            shop.buy( item['shop_ikey'] as String, null);
		        }
	      	}
  		}
	} else {
		GUILayout.FlexibleSpace();
	}

	// inventory right column end
	GUILayout.EndVertical();
	
	GUILayout.EndHorizontal();
}

function renderInventory() {
	GUILayout.BeginHorizontal();
	
	// inventory left column start
	GUILayout.BeginVertical();
	
		GUILayout.Box("Inventory");

		var inventoryItems = inventory.list() as ArrayList;
		
		inventoryScroll = GUILayout.BeginScrollView(inventoryScroll);
		
		var labels:ArrayList = new ArrayList();
		var itemIds:ArrayList = new ArrayList();  
		for(var i=0;i<inventoryItems.Count;i++) {
			var item:Hashtable = inventoryItems[i];
			if(!item["equipped"] || item["equipped"] == "false") {
				labels.Add(item["label"]);
				itemIds.Add(item["id"]);
			}
		}
		if(labels.Count > 0) {
			var inventoryLabels:String[] = labels.ToArray(typeof(String));
			var currentSelectIndex = inventorySelectIndex;
			inventorySelectIndex = GUILayout.SelectionGrid(inventorySelectIndex, inventoryLabels, 3);
			if(inventorySelectIndex != -1 && inventorySelectIndex != currentSelectIndex) {
				var itemId = itemIds[inventorySelectIndex];
				onInventorySelect(inventorySelectIndex, itemId);
			}
		} else {
			GUILayout.Label("inventory empty");
		}
		
		GUILayout.EndScrollView();
	
		GUILayout.Box("Equipped");
		equippedScroll = GUILayout.BeginScrollView(equippedScroll);  
		labels.Clear();
		itemIds.Clear();
		for(i=0;i<inventoryItems.Count;i++) {
			item = inventoryItems[i];
			if(item["equipped"] && item["equipped"] != "false") {
				labels.Add(item["label"]);
				itemIds.Add(item["id"]);
			}
		}
		if(labels.Count > 0) {
			var equippedLabels:String[] = labels.ToArray(typeof(String)) as String[];
			currentSelectIndex = equippedSelectIndex;
			equippedSelectIndex = GUILayout.SelectionGrid(equippedSelectIndex, equippedLabels, 3);
			if(equippedSelectIndex != -1 && equippedSelectIndex != currentSelectIndex) {
				itemId = itemIds[equippedSelectIndex];
				onEquippedSelect(equippedSelectIndex, itemId);
			}
		} else {
			GUILayout.Label("no items equipped");
		}
		
		GUILayout.EndScrollView();

	// inventory left column end
	GUILayout.EndVertical();
	
	// inventory right column start
	GUILayout.BeginVertical(GUILayout.MinWidth(300));
	
	if(selectedItemId) {
		var selectedItem:Hashtable = inventory.getGood(selectedItemId);
		if(selectedItem == null) {
			selectedItemId = null;
		} else {
			GUILayout.Box("Item Profile");
			GUILayout.Label(selectedItem["label"] as String);
			GUILayout.Label(selectedItem["id"] as String);
			GUILayout.Label(selectedItem["ikey"] as String);
			GUILayout.Label(selectedItem["equipped"] as String);
			GUILayout.Box("Actions");
	
			if( selectedItem["equipped"] == "true" )
	      	{
		        if(GUILayout.Button("Unequip"))
		        {
		            inventory.deactivate( item['id'] as String, null);
		        }
	      	}
	      	else
	      	{
		        if( GUILayout.Button("Equip") )
		        {
		            inventory.activate( item['id'] as String, null);
		        }
	      	}
	      	if( selectedItem['sellable'] == "true" )
	      	{
	        	if(GUILayout.Button("Sell"))
	        	{
	          		inventory.sell( selectedItem['id'] as String, null);
	      		}
	  		}
  		}
	} else {
		GUILayout.FlexibleSpace();
	}

	// inventory right column end
	GUILayout.EndVertical();
	
	GUILayout.EndHorizontal();
}



function handleAuthenticating() {
	StatusBox.render("Authenticating " + user_name);
}

function handleCreatingUser() {
	StatusBox.render("Creating " + user_name);
}


/**
 * Per frame handling of the RoarLoading state.
 * Simply sit tight and wait for the:
 * - user properties
 * - user inventory items
 * - shop items 
 * to load from the server.
 **/
function handleRoarLoading() {
	if(roar.Properties.hasDataFromServer && 
	   roar.Inventory.hasDataFromServer &&
	   roar.Shop.hasDataFromServer) {
	   setUIState(UIState.InGame);
	   return;
	}
	StatusBox.render("Loading roar.io");
}

RoarIOManager.logInFailedEvent += onLoginFailed;
function onLoginFailed(msg) {
	rConsole("roar.io Login failed! " + msg);
	showConfirm(msg);
	setUIState(UIState.Login);
}

RoarIOManager.loggedInEvent += onLogin;
function onLogin()
{
  rConsole('Logged in with authtoken ' + roar.AuthToken);
  isAuthenticated = true;
  setUIState(UIState.RoarLoading);
  roar.Properties.fetch(null);
  roar.Inventory.fetch(null);
  roar.Shop.fetch(null);
}

function setUIState(s:UIState) {
	if(s == UIState.InGame) {
		inMenu = false;
	} else {
		inMenu = true;
	}
	uiState = s;
	rConsole("entering ui state: " + s);
}

function rConsole(status) {
  Debug.Log(status);
}

function showConfirm(msg) {
	this.menuDisabled = true;
	this.gameObject.GetComponent(ConfirmBox).Show(msg, onConfirm);
}

function onConfirm() {
	this.menuDisabled = false;
}

function onInventorySelect(index, itemId) {
	equippedSelectIndex = -1;
	selectedItemId = itemId;
}

function onEquippedSelect(index, itemId) {
	inventorySelectIndex = -1;
	selectedItemId = itemId;
}

RoarIOManager.goodActivatedEvent += onEquipped;
function onEquipped(goodInfo:RoarIOManager.GoodInfo) {
	gameObject.GetComponent(EquipmentManager).Equip(goodInfo.ikey);
}

RoarIOManager.goodDeactivatedEvent += onUnequipped;
function onUnequipped(goodInfo:RoarIOManager.GoodInfo) {
	gameObject.GetComponent(EquipmentManager).Unequip(goodInfo.ikey);
}


RoarIOManager.goodSoldEvent += onGoodSold;
function onGoodSold(goodInfo:RoarIOManager.GoodInfo) {


}

RoarIOManager.inventoryReadyEvent += onInventoryReady;
function onInventoryReady() {
	// make sure the equipment manager knows what the user is equipped with
	gameObject.GetComponent(EquipmentManager).Reset();
	var inventoryItems = inventory.list() as ArrayList;
	for(var i=0;i<inventoryItems.Count;i++) {
		var item:Hashtable = inventoryItems[i];
		if(item["equipped"] && item["equipped"] != "false") {
			gameObject.GetComponent(EquipmentManager).Equip(item["ikey"]);
		}
	}
}

function GetAudioEnabled()
{
	return PlayerPrefs.GetInt ("Play audio", 1) != 0;
}
	
function SetAudioEnabled(e) 
{
	PlayerPrefs.SetInt ("Play audio", e ? 1 : 0);
	UpdateAudio ();
}

function UpdateAudio ()
{
	AudioListener.volume = GetAudioEnabled() ? 1.0f : 0.0f;
}

function FlipFullscreen ()
{
	Screen.fullScreen = !Screen.fullScreen;
}

function FlipMute ()
{
	SetAudioEnabled(!GetAudioEnabled());
}

function Update ()
{
	if (directKeyQuit)
	{
		if (Input.GetKeyDown (KeyCode.Escape))
		{
			Application.Quit ();
		}
		else if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Menu))
		{
			if(CanPlay()) {
				FlipPause();
			}
		}
	}
}

static function Restart ()
{
	var instance:RoarDemo = FindObjectOfType (typeof (RoarDemo));
	if (instance != null)
	{
		Destroy (instance.gameObject);
	}
	Time.timeScale = 1.0f;
	Application.LoadLevel (0);
}