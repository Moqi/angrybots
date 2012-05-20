#pragma strict

static function render() {

	var dialogWidth = 300.0f;
	var dialogHeight = 120.0f;
	
	var dialogRect = Rect((Screen.width - dialogWidth) * 0.5f, 
	                     (Screen.height - dialogHeight) * 0.5f, 
	                     dialogWidth, 
	                     dialogHeight);


	GUILayout.BeginArea (dialogRect, GUI.skin.GetStyle("Box"));
	
		var intro  = "This is a modified version of the official Unity AngryBots tech demo.";
		    intro += "\nIt demonstrates some of the features of the roar.io game monetization api.";
	
		GUILayout.Label(intro);
		
		GUILayout.Space(10.0f);

	    
    GUILayout.EndArea ();
}