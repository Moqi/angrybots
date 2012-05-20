#pragma strict

static function getCentredRect(width:int, height:int) {
	return Rect((Screen.width - width) * 0.5f, 
	            (Screen.height - height) * 0.5f, 
	             width, 
	             height);
}