#pragma strict

import Roar.Components;
import Photon;

private var roar:IRoar;

public var LaserScope        : GameObject = null;
public var LaserScopeBlue    : GameObject = null;
public var LaserScopeRed     : GameObject = null;
public var LaserScopeMagenta : GameObject = null;
public var LaserScopeGreen   : GameObject = null;
public var LaserScopeCyan    : GameObject = null;
public var LaserScopeYellow  : GameObject = null;
public var LaserScopeWhite   : GameObject = null;
public var CurrentScope : int = 0;

function findLasers () {
	LaserScope        = gameObject.Find("WeaponSlot");
	LaserScopeBlue    = gameObject.Find("WeaponSlotBlue");
	LaserScopeRed     = gameObject.Find("WeaponSlotRed");
	LaserScopeMagenta = gameObject.Find("WeaponSlotMagenta");
	LaserScopeGreen   = gameObject.Find("WeaponSlotGreen");
	LaserScopeCyan    = gameObject.Find("WeaponSlotCyan");
	LaserScopeYellow  = gameObject.Find("WeaponSlotYellow");
	LaserScopeWhite   = gameObject.Find("WeaponSlotWhite");
}

function Awake () {
	roar = gameObject.Find("Roar").GetComponent(DefaultRoar) as IRoar;
	if (roar == null) Debug.Log("roar is null");
	else Debug.Log("roar found");
	Debug.Log ("sonda");
	findLasers();
}

function Update () {
	if (CurrentScope < 1 || LaserScope != null) return;
	findLasers();
	changeLaserScopeColour(CurrentScope);
}

RoarManager.loggedInEvent += onLogin;

function onLogin () {
	Debug.Log("Just logged in.");
}

RoarManager.propertiesReadyEvent += onPropertiesReady;

function onPropertiesReady () {
	Debug.Log("PROPERTIES LOADED");
	Debug.Log("NAME = [" + roar.Properties.GetProperty("name").value + "]");
	Debug.Log("PHOTON PLAYER NAME = [" + PhotonNetwork.playerName + "]");
	PhotonNetwork.playerName = roar.Properties.GetProperty("name").value;
	PhotonNetwork.playerRoarID = roar.Properties.GetProperty("id").value;
	//PhotonNetwork.SendPlayerName();
	Debug.Log("ID = [" + roar.Properties.GetProperty("id").value + "]");
	var profile_viewer:GameObject = GameObject.Find("ProfileViewerWidget");
	if (profile_viewer != null) {
		Debug.Log("Profile Viewer Widget FOUND!");
		profile_viewer.SendMessage("ViewProfile", roar.Properties.GetProperty("id").value, SendMessageOptions.RequireReceiver);
		Debug.Log("MESSAGE SENT");
	} else Debug.Log("Profile Viewer Widget NOT FOUND!");
	roar.Inventory.Fetch(null);
}

function fetch_callback (info : Roar.CallbackInfo.<System.Collections.Generic.IDictionary.<String, Roar.DomainObjects.PlayerAttribute> >) {
	Debug.Log("PROPERTIES FETCHED");
	Debug.Log("NAME = [" + roar.Properties.GetProperty("name").value + "]");
	Debug.Log("PHOTON PLAYER NAME = [" + PhotonNetwork.playerName + "]");
	Debug.Log("ID = [" + roar.Properties.GetProperty("id").value + "]");
	PhotonNetwork.playerName = "sonda";
}

RoarManager.inventoryReadyEvent += onInventoryReady;
RoarManager.inventoryChangeEvent += onInventoryReady;
RoarManager.goodBoughtEvent += onPurchase;

function onPurchase () {
	Debug.Log("PURCHASED SOMETHING");
	roar.Inventory.Fetch(null);
}

function onInventoryReady () {
	Debug.Log("INVENTORY READY");
	var items = roar.Inventory.List();
	Time.timeScale = 1.0;
	var scope_id = 1;
	for (item in items) {
		Debug.Log("		ITEM: [" + item.description + "] [" + item.label + "]");
		switch (item.label) {
		case "Super Speed": Time.timeScale += 0.5; break;
		case "Laser Sight": scope_id += 1; break;
		default: break;
		}
	}
	changeLaserScopeColour(scope_id);
}

function onInventoryChanged () {
	Debug.Log("INVENTORY CHANGED");
}

function changeLaserScopeColour (scope_id : int) {
	CurrentScope = scope_id;
	if (LaserScope == null) findLasers();
	Debug.Log ("SCOPE COLOUR [" + scope_id + "]");
	if (LaserScope != null) LaserScope.active = false;
	if (LaserScopeBlue != null) LaserScopeBlue.active = false;
	if (LaserScopeRed != null) LaserScopeRed.active = false;
	if (LaserScopeMagenta != null) LaserScopeMagenta.active = false;
	if (LaserScopeGreen != null) LaserScopeGreen.active = false;
	if (LaserScopeCyan != null) LaserScopeCyan.active = false;
	if (LaserScopeYellow != null) LaserScopeYellow.active = false;
	if (LaserScopeWhite != null) LaserScopeWhite.active = false;
	if (scope_id > 8) scope_id = 8;
	switch (scope_id) {
	case 2:
		if (LaserScopeBlue != null) {
			LaserScopeBlue.active = true;
			LaserScopeBlue.GetComponent(LineRenderer).enabled = true;
			LaserScopeBlue.GetComponent(AudioSource).enabled = true;
			LaserScopeBlue.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 3:
		if (LaserScopeRed != null) {
			LaserScopeRed.active = true;
			LaserScopeRed.GetComponent(LineRenderer).enabled = true;
			LaserScopeRed.GetComponent(AudioSource).enabled = true;
			LaserScopeRed.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 4:
		if (LaserScopeMagenta != null) {
			LaserScopeMagenta.active = true;
			LaserScopeMagenta.GetComponent(LineRenderer).enabled = true;
			LaserScopeMagenta.GetComponent(AudioSource).enabled = true;
			LaserScopeMagenta.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 5:
		if (LaserScopeGreen != null) {
			LaserScopeGreen.active = true;
			LaserScopeGreen.GetComponent(LineRenderer).enabled = true;
			LaserScopeGreen.GetComponent(AudioSource).enabled = true;
			LaserScopeGreen.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 6:
		if (LaserScopeCyan != null) {
			LaserScopeCyan.active = true;
			LaserScopeCyan.GetComponent(LineRenderer).enabled = true;
			LaserScopeCyan.GetComponent(AudioSource).enabled = true;
			LaserScopeCyan.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 7:
		if (LaserScopeYellow != null) {
			LaserScopeYellow.active = true;
			LaserScopeYellow.GetComponent(LineRenderer).enabled = true;
			LaserScopeYellow.GetComponent(AudioSource).enabled = true;
			LaserScopeYellow.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	case 8:
		if (LaserScopeWhite != null) {
			LaserScopeWhite.active = true;
			LaserScopeWhite.GetComponent(LineRenderer).enabled = true;
			LaserScopeWhite.GetComponent(AudioSource).enabled = true;
			LaserScopeWhite.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	default:
		if (LaserScope != null) {
			LaserScope.active = true;
			//LaserScope.GetComponent(LineRenderer).enabled = true;
			//LaserScope.GetComponent(AudioSource).enabled = true;
			//LaserScope.GetComponent(TriggerOnMouseOrJoystick).enabled = true;
		}
		break;
	}
}
