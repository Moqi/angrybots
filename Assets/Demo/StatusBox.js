#pragma strict


static function render(msg) {
	// Make an area at the center of the screen  
	GUILayout.BeginArea(RenderUtils.getCentredRect(200, 50), GUI.skin.GetStyle("Box"));	
	GUILayout.BeginVertical();
	GUILayout.FlexibleSpace();
	// make a label indicating the status.
	var labelStyle = GUI.skin.GetStyle("Label");
	labelStyle.alignment = TextAnchor.MiddleCenter; 
	GUILayout.Label(msg as String, labelStyle);

	GUILayout.FlexibleSpace();
	GUILayout.EndVertical();
	// End the area we started above.
	GUILayout.EndArea();
}