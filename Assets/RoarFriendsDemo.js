#pragma strict

import Roar.Components;
import Photon;

private var roar:IRoar;

function Awake () {
	roar = gameObject.Find("Roar").GetComponent(DefaultRoar) as IRoar;
	if (roar == null) Debug.Log("roar is null");
	else Debug.Log("roar found");
	Debug.Log ("sonda");
}

RoarManager.loggedInEvent += onLogin;

function onLogin () {
	Debug.Log("Just logged in.");
	//roar.Inventory.List();
//	roar.Properties.Fetch(fetch_callback);
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
	for (item in items) {
		Debug.Log("		ITEM: [" + item.description + "] [" + item.label + "]");
		switch (item.label) {
		case "Super Speed": Time.timeScale += 0.5; break;
		default: break;
		}
	}
}

function onInventoryChanged () {
	Debug.Log("INVENTORY CHANGED");
}