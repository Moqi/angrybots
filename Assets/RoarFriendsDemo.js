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
//	roar.Properties.Fetch(fetch_callback);
}

RoarManager.propertiesReadyEvent += onPropertiesReady;

function onPropertiesReady () {
	Debug.Log("PROPERTIES LOADED");
	Debug.Log("NAME = [" + roar.Properties.GetProperty("name").value + "]");
	Debug.Log("PHOTON PLAYER NAME = [" + PhotonNetwork.playerName + "]");
	PhotonNetwork.playerName = roar.Properties.GetProperty("name").value;
	PhotonNetwork.SendPlayerName();
	Debug.Log("ID = [" + roar.Properties.GetProperty("id").value + "]");
}

function fetch_callback (info : Roar.CallbackInfo.<System.Collections.Generic.IDictionary.<String, Roar.DomainObjects.PlayerAttribute> >) {
	Debug.Log("PROPERTIES FETCHED");
	Debug.Log("NAME = [" + roar.Properties.GetProperty("name").value + "]");
	Debug.Log("PHOTON PLAYER NAME = [" + PhotonNetwork.playerName + "]");
	Debug.Log("ID = [" + roar.Properties.GetProperty("id").value + "]");
	PhotonNetwork.playerName = "sonda";
}
