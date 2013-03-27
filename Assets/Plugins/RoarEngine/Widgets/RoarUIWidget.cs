using UnityEngine;
using System.Collections;

public abstract class RoarUIWidget : RoarWidgetBase
{	
	public bool autoEnableOnLogIn = false;
	public bool autoDisableOnLogout = true;
	protected DefaultRoar roar;
	private bool isLoggedIn;

	protected override void Awake()
	{
		base.Awake();

		roar = DefaultRoar.Instance;

		// Load a skin if specified.
		if (skin == null) skin = roar.defaultGUISkin;

		// listen for log-in event
		RoarLoginWidget.OnFullyLoggedIn -= OnRoarLogin;
		RoarLoginWidget.OnFullyLoggedIn += OnRoarLogin;
		RoarManager.loggedOutEvent -= OnRoarLogout;
		RoarManager.loggedOutEvent += OnRoarLogout;
		
		enabled = false;
	}
	
	void Reset()
	{
		bounds = new Rect(0,0,512,386);
	}
	
	void OnRoarLogin()
	{
		isLoggedIn = true;
		if (autoEnableOnLogIn)
		{
			enabled = true;
		}
	}

	void OnRoarLogout()
	{
		isLoggedIn = false;
		if (autoDisableOnLogout)
		{
			enabled = false;
		}
	}
	
	protected bool IsLoggedIn
	{
		get { return isLoggedIn; }
	}
	
}
