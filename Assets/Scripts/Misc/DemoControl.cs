/*
Copyright (c) 2012, Run With Robots
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the roar.io library nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY RUN WITH ROBOTS ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MICHAEL ANDERSON BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

/*
This source file is a modified version of the original AngryBots tech demo
source file AngryBots/Assets/Scripts/Misc/DemoControl.cs, downloaded via
http://u3d.as/content/unity-technologies/angry-bots/2aL
*/
using UnityEngine;
using System.Collections;


public class DemoControl : MonoBehaviour
{
	/*
	public Texture2D pauseIcon, menuBackground, resumeButton, restartButton, fullscreenButton, muteButton, quitButton;
	
	private const float cornerTextureSize = 48.0f;
	private const float menuWidth = 200.0f, menuHeight = 241.0f, menuHeaderHeight = 26.0f, buttonWidth = 175.0f, buttonHeight = 30.0f;
	
	private bool fullScreenAvailable = false, quitEnabled = true, directKeyQuit = true;
	*/
	
	public static void Restart ()
	{
		DemoControl instance = (DemoControl)FindObjectOfType (typeof (DemoControl));
		if (instance != null)
		{
			Destroy (instance.gameObject);
		}
		Time.timeScale = 1.0f;
		Application.LoadLevel (0);
	}
	/*
	public bool AudioEnabled
	{
		get
		{
			return PlayerPrefs.GetInt ("Play audio", 1) != 0;
		}
		set
		{
			PlayerPrefs.SetInt ("Play audio", value ? 1 : 0);
			UpdateAudio ();
		}
	}
	
	
	void Start ()
	{
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
	
	
	void UpdateAudio ()
	{
		AudioListener.volume = AudioEnabled ? 1.0f : 0.0f;
	}
	
	
	public void FlipFullscreen ()
	{
		Screen.fullScreen = !Screen.fullScreen;
	}
	
	
	public void FlipMute ()
	{
		AudioEnabled = !AudioEnabled;
	}
	
	
	public void FlipPause ()
	{
		Time.timeScale = Time.timeScale == 0.0f ? 1.0f : 0.0f;
	}
	
	
	void Update ()
	{
		if (directKeyQuit)
		{
			if (Input.GetKeyDown (KeyCode.Escape))
			{
				Application.Quit ();
			}
			else if (Input.GetKeyDown (KeyCode.Return) || Input.GetKeyDown (KeyCode.Menu))
			{
				Time.timeScale = 0.0f;
			}
		}
	}
	
	
	void OnGUI ()
	{
		Rect rightRect = new Rect (Screen.width - cornerTextureSize, 0.0f, cornerTextureSize, cornerTextureSize);
		
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
			break;
		}
		
		if (Time.timeScale != 0.0f)
		{
			return;
		}
		
		Rect menuRect = new Rect (
			(Screen.width - menuWidth) * 0.5f,
			(Screen.height - menuHeight) * 0.5f,
			menuWidth,
			menuHeight
		);
		
		GUI.DrawTexture (menuRect, menuBackground);
		
		GUILayout.BeginArea (menuRect);
			GUILayout.Space (menuHeaderHeight);
		
			GUILayout.FlexibleSpace ();
			
			if (MenuButton (resumeButton))
			{
				Time.timeScale = 1.0f;
			}
			
			if (fullScreenAvailable)
			{
				GUILayout.FlexibleSpace ();
				if (MenuButton (fullscreenButton))
				{
					FlipFullscreen ();
				}
			}
			
			#if !UNITY_FLASH
				GUILayout.FlexibleSpace ();
			
				if (MenuButton (muteButton))
				{
					FlipMute ();
				}
			#endif
			
			GUILayout.FlexibleSpace ();
			
			if (MenuButton (restartButton))
			{
				Restart ();
			}
			
			if (quitEnabled)
			{
				GUILayout.FlexibleSpace ();
				if (MenuButton (quitButton))
				{
					Application.Quit ();
				}
			}
			GUILayout.FlexibleSpace ();
		GUILayout.EndArea ();
	}
	
	
	bool MenuButton (Texture2D icon)
	{
		bool wasPressed = false;
		
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
		
			Rect rect = GUILayoutUtility.GetRect (buttonWidth, buttonHeight, GUILayout.Width (buttonWidth), GUILayout.Height (buttonHeight));
		
			switch (Event.current.type)
			{
				case EventType.MouseUp:
					if (rect.Contains (Event.current.mousePosition))
					{
						wasPressed = true;
					}
				break;
				case EventType.Repaint:
					GUI.DrawTexture (rect, icon);
				break;
			}
		
			GUILayout.FlexibleSpace ();
		GUILayout.EndHorizontal ();
		
		return wasPressed;
	}
	*/
}
