#pragma strict

import UnityEngine;

private var confirmMessage:String;
private var callback : Function;

function Start () {

}

function Update () {

}

function OnGUI() {
	if(confirmMessage) {
		
		GUI.depth = -10;
		// Make an area at the center of the screen  
		//var tempColor = GUI.backgroundColor; 
		//GUI.backgroundColor = Color.black;
		GUILayout.BeginArea(RenderUtils.getCentredRect(200, 80), GUI.skin.GetStyle("Box"));	
		
		//GUI.backgroundColor = tempColor;
		GUI.color = Color.red;
		
		// make a label indicating the status.
		var labelStyle = GUI.skin.GetStyle("Label");
		labelStyle.alignment = TextAnchor.MiddleCenter; 
		GUILayout.Label(confirmMessage as String, labelStyle);
	
	    if(GUILayout.Button("OK")) {
			confirmMessage = null;
			callback();
		}
		
		// End the area we started above.
		GUILayout.EndArea();
	}
}

function Show(msg, cb) {
	confirmMessage = msg;
	callback = cb;
}
