#pragma strict

import Roar.Components;

// Well, this is a nice-to-have parameter
var player_level = 127;

// Laser scopes with different colours
// ... only one should be active
var laser_scope_main    : GameObject = null;
var laser_scope_blue    : GameObject = null;
var laser_scope_red     : GameObject = null;
var laser_scope_magenta : GameObject = null;
var laser_scope_green   : GameObject = null;
var laser_scope_cyan    : GameObject = null;
var laser_scope_yellow  : GameObject = null;
var laser_scope_white   : GameObject = null;
var laser_scope_black   : GameObject = null;

private var roar : IRoar = null;




function Start ()
{
	// This stops the game (by setting the time to stop passing).
	Time.timeScale = 0.0;
	laser_scope_main    = GameObject.Find("WeaponSlot");
	laser_scope_blue    = GameObject.Find("WeaponSlotBlue");
	laser_scope_red     = GameObject.Find("WeaponSlotRed");
	laser_scope_magenta = GameObject.Find("WeaponSlotMagenta");
	laser_scope_green   = GameObject.Find("WeaponSlotGreen");
	laser_scope_cyan    = GameObject.Find("WeaponSlotCyan");
	laser_scope_yellow  = GameObject.Find("WeaponSlotYellow");
	laser_scope_white   = GameObject.Find("WeaponSlotWhite");
	laser_scope_black   = GameObject.Find("WeaponSlotBlack");
	set_laser_scope_colour(1);
}





function Awake ()
{
	// Obviously, we need the roar object if we want to talk to Roar Engine
	roar = gameObject.Find("Roar").GetComponent(DefaultRoar) as IRoar;
}







function OnLogin ()
{
	// This restarts the playback (by setting the time to pass).
	Time.timeScale = 1.0;
}
// Here we want our OnLogin function to be called when the user successfully logs-in
RoarManager.loggedInEvent += OnLogin;
// ... by the way, if we want to react on login failure, we would use RoarManager.loginFailedEvent







function OnStatChange ()
{
	// First, we need to see if the "level" property is available
	var level = roar.Properties.GetProperty("level");
	// Well, if it isn't than there is nothing we can do about it
	if (level == null) return;
	// But if it is present, then we can use it
	player_level = parseInt(level.value);
	// ... and change the colour of the lase scope (which depend on the player's level
	set_laser_scope_colour(player_level);
}

// Here we want our OnStatChange function to be called whenever stats are changed
RoarManager.propertiesChangeEvent += OnStatChange;
// ... by the way, the stats are changed by executing tasks on the server
//     Look for the "process_kill" function in Health.js file
//     which does this job





// This function changes the colour of the laser scope
function set_laser_scope_colour (colour : int)
{
	// First we check if the colour is in valid range
	if (colour < 1 || colour > 9) return;
	// If it is the we deactivate all scopes
	if (laser_scope_main) laser_scope_main.active = false;
	if (laser_scope_blue) laser_scope_blue.active = false;
	if (laser_scope_red) laser_scope_red.active = false;
	if (laser_scope_magenta) laser_scope_magenta.active = false;
	if (laser_scope_green) laser_scope_green.active = false;
	if (laser_scope_cyan) laser_scope_cyan.active = false;
	if (laser_scope_yellow) laser_scope_yellow.active = false;
	if (laser_scope_white) laser_scope_white.active = false;
	if (laser_scope_black) laser_scope_black.active = false;
	// ... and reactivate the one, which we want
	switch (colour)
	{
	case 1:
		if (laser_scope_main) laser_scope_main.active = true;
		break;
	case 2:
		if (laser_scope_blue) laser_scope_blue.active = true;
		break;
	case 3:
		if (laser_scope_red) laser_scope_red.active = true;
		break;
	case 4:
		if (laser_scope_magenta) laser_scope_magenta.active = true;
		break;
	case 5:
		if (laser_scope_green) laser_scope_green.active = true;
		break;
	case 6:
		if (laser_scope_cyan) laser_scope_cyan.active = true;
		break;
	case 7:
		if (laser_scope_yellow) laser_scope_yellow.active = true;
		break;
	case 8:
		if (laser_scope_white) laser_scope_white.active = true;
		break;
	case 9:
		if (laser_scope_main) laser_scope_main.active = true;
		break;
	default:
		break;
	}
}

