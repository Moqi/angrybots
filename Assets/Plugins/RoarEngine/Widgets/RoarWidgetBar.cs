using UnityEngine;
using System.Collections.Generic;
using Roar;

public class RoarWidgetBar : RoarUIWidget
{
	private List<Transform> buttons;
	public int interButtonSpacing = 0;
	public int numRows = 2;


	protected override void DrawGUI(int windowId)
	{
		Rect buttonRect = new Rect(0, 0, 0, contentBounds.height/numRows);

		buttonRect.width = contentBounds.width/Mathf.Ceil(buttons.Count / (float)numRows) - interButtonSpacing * (buttons.Count - 1)/numRows;

		foreach (Transform button in buttons)
		{
			if (GUI.Button (buttonRect, button.name, "DefaultButton"))
			{
				((MonoBehaviour) button.GetComponent(typeof(MonoBehaviour))).enabled = !((MonoBehaviour) button.GetComponent(typeof(MonoBehaviour))).enabled;
			}
			buttonRect.x += buttonRect.width + interButtonSpacing;
			if(buttonRect.x >= contentBounds.width)
			{
				buttonRect.x = 0;
				buttonRect.y += contentBounds.height/numRows;
			}
		}
	}

	void Reset()
	{
		bounds.width = 800;
		bounds.height = 100;
		apearance.headerHeight = 30;
		apearance.closeButtonSize = 15;
		autoEnableOnLogIn = true;
		displayName = "Widget Bar";
	}

	protected override void Awake()
	{
		base.Awake();
		buttons = new List<Transform>();
		foreach (Transform child in transform.parent)
		{
			if (child.transform != this.transform)
			{
				buttons.Add(child);
			}
		}
	}
}