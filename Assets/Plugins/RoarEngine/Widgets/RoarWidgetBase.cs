using System;
using UnityEngine;
using System.Collections;

[Serializable]
public class WindowInfo
{
	public bool isWindowed = false;
	public bool supportsDragging = false;
	
	protected int windowId = 0;
	protected static int windowIdGenerator = 1;

	// We dont actually generate a windowId until asked for one.
	public int WindowId { get {
		if( isWindowed && windowId == 0 )
		{
			windowIdGenerator++;
			windowId = windowIdGenerator;
		}
			return windowId;
	} }
	
}
public enum AlignmentHorizontal
{
	Left = 0,
	Center = 1,
	Right = 2,
	None = 3
}

public enum AlignmentVertical
{
	Top = 0,
	Center = 1,
	Bottom = 2,
	None = 3
}

[Serializable]
public class Apearance
{
	public Color color = Color.white;
	public string boundingStyle = "WidgetBoundingStyle";
	public string windowBoundingStyle = "WindowedWidgetBoundingStyle";
	public string boundingTitle = string.Empty;
	public Texture boundingImage = null;
	public string headerStyle = "DefaultHeaderStyle";
	public string subheaderStyleLeft = "DefaultSubheaderStyleLeft";
	public string subheaderStyleRight = "DefaultSubheaderStyleRight";//arrow is 13 px
	public string closeButtonStyle = "DefaultCloseButtonStyle";
	public float headerHeight = 50;
	public float closeButtonOffset = 30;
	public float closeButtonSize = 30;
	public int windowBorderWidth = 8;

}

public abstract class RoarWidgetBase : MonoBehaviour
{
	public Rect bounds;
	public Rect contentBounds;
	Rect originalContentBounds;
	
	public GUISkin skin;
	
	public int depth;

	public Apearance apearance = new Apearance();
	public WindowInfo windowInfo = new WindowInfo();

	
	public string displayName = "Widget";
	public bool drawSubheading = false;
	public string subheaderName = "Widget";
	public float subheaderDarkWidth = 100;
	public AlignmentHorizontal horizontalAlignment = AlignmentHorizontal.None;
	public AlignmentVertical verticalAlignment = AlignmentVertical.None;
	public float horizontalOffset;
	public float verticalOffset;
	
	public bool useScrollView = false;
	public bool alwaysShowHorizontalScrollBar = false;
	public bool alwaysShowVerticalScrollBar = false;
	
	
	public string scrollBarStyle = "DefaultScrollbarStyle";
	// Whether the window should be fixed in place each render frame.
	// Easiest if this is set to true unless you have a dragable window
	protected bool RequiresSnap { get { return !windowInfo.supportsDragging; } }
	
	//Whether to render inside a window
	protected bool RequiresWindow { get { return windowInfo.isWindowed; } }

	private GUIContent boundingGUIContent;
	
	// scrolling
	private Vector2 scrollPosition = Vector2.zero;
	private Rect scrollViewRect;
	
	Rect headerRect;
	Rect headerRightRect;
	protected bool networkActionInProgress;
	public MovieTexture spinnerMovieTex;

	protected virtual void SnapBoundsRectIntoPosition()
	{

		// horizontal binding
		if( horizontalAlignment != AlignmentHorizontal.None )
		{
			switch (horizontalAlignment)
			{
			case AlignmentHorizontal.Left:
				bounds.x = 0;
				break;
			case AlignmentHorizontal.Right:
				bounds.x = Screen.width - bounds.width;
				break;
			case AlignmentHorizontal.Center:
				bounds.x = (Screen.width - bounds.width) / 2;
				break;
			}
			bounds.x += horizontalOffset;
		}
		
		if( verticalAlignment != AlignmentVertical.None )
		{
			// vertical binding
			switch (verticalAlignment)
			{
			case AlignmentVertical.Top:
				bounds.y = 0;
				break;
			case AlignmentVertical.Bottom:
				bounds.y = Screen.height - bounds.height;
				break;
			case AlignmentVertical.Center:
				bounds.y = (Screen.height - bounds.height) / 2;
				break;
			}
			bounds.y += verticalOffset;
		}
	}

	protected virtual void Awake()
	{		
		boundingGUIContent = new GUIContent(apearance.boundingTitle, apearance.boundingImage);
		
		scrollViewRect = new Rect();
		
		if (contentBounds.width == 0)
			contentBounds.width = bounds.width;
		if (contentBounds.height == 0)
			contentBounds.height = bounds.height;
		
		scrollViewRect.width = contentBounds.width;
		scrollViewRect.height = contentBounds.height;
		
		SnapBoundsRectIntoPosition();
		
		originalContentBounds = new Rect(contentBounds.x, contentBounds.y, contentBounds.width, contentBounds.height);
	}
	
	protected virtual void OnEnable()
	{
		scrollPosition = Vector2.zero;
	}
	
	protected virtual void OnDisable()
	{}
	
	void Reset()
	{
		bounds = new Rect(0,0,512,386);
	}
	
	void OnGUI()
	{
		contentBounds.y = apearance.headerHeight + apearance.windowBorderWidth;

		if(RequiresWindow)
		{
			contentBounds.x = apearance.windowBorderWidth;
			contentBounds.width = originalContentBounds.width - 2*apearance.windowBorderWidth;
			contentBounds.height = bounds.height - contentBounds.y - apearance.windowBorderWidth;
			headerRect = new Rect(apearance.windowBorderWidth, apearance.windowBorderWidth, contentBounds.width, contentBounds.y);
		}
		else
		{
			contentBounds.height = bounds.height - contentBounds.y- apearance.windowBorderWidth;
			contentBounds.x = apearance.windowBorderWidth;
			contentBounds.width = originalContentBounds.width- 2*apearance.windowBorderWidth;
			headerRect = new Rect(apearance.windowBorderWidth, apearance.windowBorderWidth, contentBounds.width, contentBounds.y);
		}

		if(drawSubheading)
		{
			headerRect = new Rect(apearance.windowBorderWidth,apearance.windowBorderWidth, subheaderDarkWidth,contentBounds.y-apearance.windowBorderWidth);
			headerRightRect = new Rect(apearance.windowBorderWidth + subheaderDarkWidth - 14,apearance.windowBorderWidth, contentBounds.width - apearance.windowBorderWidth - subheaderDarkWidth+17,contentBounds.y-apearance.windowBorderWidth);

		}
		// rendering attributes
		GUI.skin = skin;
		GUI.depth = depth;
		GUI.color = apearance.color;
		
		if ( RequiresSnap )
		{
			SnapBoundsRectIntoPosition();
		}
		
		if( RequiresWindow )
		{
			bounds = GUI.Window(windowInfo.WindowId, bounds, DrawWindow, boundingGUIContent, apearance.windowBoundingStyle);
		}
		else
		{
			GUI.BeginGroup(bounds);
			Rect box = bounds;
			box.x = 0;
			box.y = 0;
			GUI.Box(box, boundingGUIContent, apearance.windowBoundingStyle);

			if(drawSubheading)
			{
				GUI.Box(headerRect, displayName, apearance.subheaderStyleLeft);
				GUI.Box(headerRightRect, subheaderName, apearance.subheaderStyleRight);
			}
			else
			{
				GUI.Box(headerRect, displayName, apearance.headerStyle);
			}
			if(GUI.Button(new Rect(contentBounds.width - apearance.closeButtonOffset, (contentBounds.y+apearance.windowBorderWidth)/2- apearance.closeButtonSize/2, apearance.closeButtonSize, apearance.closeButtonSize),new GUIContent(""), apearance.closeButtonStyle))
			{
				enabled = false;
			}
			StartContentRegion();
			DrawGUI(0);
			EndContentRegion();
			if(networkActionInProgress)
			{
				GUI.enabled = true;
				GUI.Box(new Rect(0, 0, bounds.width, bounds.height), new GUIContent(""), "DarkOverlay");
				if(spinnerMovieTex != null)
				{
					GUI.Box(new Rect(bounds.width/2 - 50, bounds.height/2 - 50, 100, 100), spinnerMovieTex, "DefaultSpinner");
					if(!spinnerMovieTex.isPlaying)
						spinnerMovieTex.Play();
					spinnerMovieTex.loop= true;
				}
			}
			GUI.EndGroup();
		}
	}

	protected void StartContentRegion()
	{
		
		if (useScrollView)
		{
			scrollPosition = GUI.BeginScrollView(contentBounds, scrollPosition, scrollViewRect, alwaysShowHorizontalScrollBar, alwaysShowVerticalScrollBar);
		}
		else
		{
			GUI.BeginGroup(contentBounds);
		}
	}

	protected void EndContentRegion()
	{
		if (useScrollView)
		{
			GUI.EndScrollView();
		}
		else
		{
			GUI.EndGroup();
		}
	}

	protected virtual void DrawWindow(int windowId)
	{
		GUI.Box(new Rect(0, 0, bounds.width, bounds.height), boundingGUIContent, apearance.windowBoundingStyle);

		if(drawSubheading)
		{
			GUI.Box(headerRect, displayName, apearance.subheaderStyleLeft);
			GUI.Box(headerRightRect, subheaderName, apearance.subheaderStyleRight);
		}
		else
		{
			GUI.Box(headerRect, displayName, apearance.headerStyle);
		}

		if(GUI.Button(new Rect(contentBounds.width - apearance.closeButtonOffset, contentBounds.y/2- apearance.closeButtonSize/2, apearance.closeButtonSize, apearance.closeButtonSize),new GUIContent(""), apearance.closeButtonStyle))
		{
			enabled = false;
		}
		StartContentRegion();
		DrawGUI(windowId);
		EndContentRegion();
		if(networkActionInProgress)
		{
			GUI.enabled = true;
			GUI.Box(new Rect(0, 0, bounds.width, bounds.height), new GUIContent(""), "DarkOverlay");

			GUI.Box(new Rect(bounds.width/2 - 50, bounds.height/2 - 50, 100, 100), spinnerMovieTex, "DefaultSpinner");
			if(!spinnerMovieTex.isPlaying)
				spinnerMovieTex.Play();
				spinnerMovieTex.loop= true;
		}
		else
			if( windowInfo.supportsDragging)
			{
				GUI.DragWindow();
			}
	}
	
	protected abstract void DrawGUI(int windowId);
	
	public void ResetScrollPosition()
	{
		scrollPosition = Vector3.zero;
	}
	
	public float ScrollViewContentWidth
	{
		set { scrollViewRect.width = value; }
	}

	public float ScrollViewContentHeight
	{
		set { scrollViewRect.height = value; }
	}
	
	public float ContentWidth
	{
		get
		{
			return contentBounds.width;
		}
	}

	public float ContentHeight
	{
		get
		{
			return contentBounds.height;
		}
	}
	
	public string Title
	{
		get { return apearance.boundingTitle; }
		set
		{
			apearance.boundingTitle = value;
			boundingGUIContent = new GUIContent(apearance.boundingTitle, apearance.boundingImage);
		}
	}

	#region Utility
	public virtual void ResetToDefaultConfiguration()
	{}
	#endregion
}
