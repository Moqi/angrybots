using UnityEngine;
using System.Collections;
using Roar;

public class RoarProfileViewerWidget: RoarUIWidget
{
	public bool autoCalculateContentBounds = true;
	public string defaultValueFormat = "N0";
	public Prop[] statsToDisplay;

	IWebAPI.IInfoActions infoActions;
	string playerIDUnderCheck=null;
	Roar.DomainObjects.Player playerInfo = null;
	public float propertyHeight = 40;
	public float interColumnSeparation = 10;
	public float labelWidth = 100;
	public float propertyWidth = 300;
	public float verticalSeparation = 10;
	public string labelStyle = "DefaultHeavyContentText";
	public float buttonWidth = 70;

	[System.Serializable]
	public class Prop
	{
		public enum StatValueType { Unspecified = 0, String = 1, Number = 2, Boolean = 3 };
		
		public bool enabled;
		public string key;
		public string title;
		public string titleStyle;
		public string valueFormat;
		public string valueStyle;
		public StatValueType valueType;

		private Roar.DomainObjects.PlayerAttribute userStat;
		private string value;

		public Prop()
		{
		}

		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public Roar.DomainObjects.PlayerAttribute UserStat
		{
			get { return userStat; }
			set { userStat = value; }
		}
	}

	void Reset()
	{
		depth = -1;
		useScrollView = false;	
		autoEnableOnLogIn = true;
		contentBounds = new Rect(0, 0, 256, 76);
	}

	public void Fetch()
	{
		networkActionInProgress = true;
		Roar.WebObjects.Info.UserArguments args = new Roar.WebObjects.Info.UserArguments();
		args.id = playerIDUnderCheck;
		DefaultRoar.Instance.WebAPI.info.user(args, new InfoUserCallback(OnRoarUserInfoComplete));
	}

	protected class InfoUserCallback : CBBase<Roar.WebObjects.Info.UserResponse>
	{
		public InfoUserCallback (Roar.Callback<Roar.WebObjects.Info.UserResponse> in_cb) : base (in_cb) {}
	}

	public void OnRoarUserInfoComplete (Roar.CallbackInfo<Roar.WebObjects.Info.UserResponse> info)
	{
		if(info.code == WebAPI.OK)
		{
			networkActionInProgress = false;
			playerInfo = info.data.player;
			LoadPropertyData();
		}
	}
	
	public void DisplayPlayerInfo(string pid)
	{
		playerIDUnderCheck = pid;
		enabled = true;
		Fetch();
	}
	
	protected override void Awake()
	{
		base.Awake();

		if (useScrollView && autoCalculateContentBounds && statsToDisplay.Length > 0)
		{
			Rect[] rects = new Rect[statsToDisplay.Length];
			Rect boundsUnion = RoarUIUtil.UnionRect(rects);
			ScrollViewContentWidth = boundsUnion.width + Mathf.Abs(boundsUnion.x);
			ScrollViewContentHeight = boundsUnion.height + Mathf.Abs(boundsUnion.y);
		}

		foreach (Prop stat in statsToDisplay)
		{
			if (stat.valueFormat == null || stat.valueFormat.Length == 0)
			{
				stat.valueFormat = defaultValueFormat;
			}
		}
	}
	
	protected void LoadPropertyData()
	{
		if (playerInfo != null)
		{
			foreach (Prop stat in statsToDisplay)
			{
				if (stat != null && stat.key != null && stat.key.Length > 0 && playerInfo.attributes.ContainsKey(stat.key))
				{
					Roar.DomainObjects.PlayerAttribute userStat = playerInfo.attributes[stat.key];
					stat.UserStat = userStat;
					if (stat.valueFormat.Length > 0)
						stat.Value = string.Format("{0:"+stat.valueFormat+"}", userStat != null ? userStat.value : "null");
					else
						stat.Value = userStat != null ? userStat.value : null;
					
					if (stat.title == null || stat.title.Length == 0)
					{
						stat.title = userStat != null ? userStat.label : "UNKNOWN";
					}
				}
			}
		}
	}

	protected override void OnEnable ()
	{
		base.OnEnable ();
		
		if(playerIDUnderCheck == null)
			enabled = false;
		
		if(playerIDUnderCheck != null)
			Fetch();
			
	}

	protected override void DrawGUI(int windowId)
	{
		if(playerInfo != null)
		{
			Rect rect = new Rect(interColumnSeparation, verticalSeparation, labelWidth, propertyHeight);
			GUI.Label(rect, "Name", labelStyle);
			rect.x += labelWidth + interColumnSeparation;
			rect.width = propertyWidth;
			GUI.Label(rect, playerInfo.name, labelStyle);

			rect.x = interColumnSeparation;
			rect.y += verticalSeparation + propertyHeight;
			rect.width = labelWidth;

			GUI.Label(rect, "Level", labelStyle);
			rect.x += labelWidth + interColumnSeparation;
			rect.width = propertyWidth;
			GUI.Label(rect, playerInfo.level.ToString(), labelStyle);

			rect.x = interColumnSeparation;
			rect.y += verticalSeparation + propertyHeight;
			rect.width = labelWidth;
			GUI.Label(rect, "PlayerID", labelStyle);
			rect.x += labelWidth + interColumnSeparation;
			rect.width = propertyWidth;
			GUI.Label(rect, playerInfo.id, labelStyle);

			rect.x = interColumnSeparation;
			rect.y += verticalSeparation + propertyHeight;
			rect.width = labelWidth;

			foreach (Prop stat in statsToDisplay)
			{
				if (stat != null && stat.enabled)
				{
					rect.width = labelWidth;
					GUI.Label(rect, stat.title, labelStyle);
					rect.x += labelWidth +interColumnSeparation;
					rect.width = propertyWidth;
					GUI.Label(rect, stat.Value, labelStyle);
					rect.x = interColumnSeparation;
					rect.y += propertyHeight + verticalSeparation;
				}
			}
			rect.width = buttonWidth;

			if(GUI.Button(rect, "Mail", "DefaultButton"))
			{
				GameObject.Find("Roar/Gifts").SendMessage("SendMailWithPID", playerIDUnderCheck, SendMessageOptions.RequireReceiver);
			}

			rect.x += rect.width + interColumnSeparation;
			if(GUI.Button(rect, "Add Friend", "DefaultButton"))
			{
				GameObject.Find("Roar/Friends").SendMessage("SendInviteWithPID", playerIDUnderCheck, SendMessageOptions.RequireReceiver);
			}

			if(alwaysShowVerticalScrollBar)
				if(rect.y < contentBounds.height)
					ScrollViewContentHeight = contentBounds.height;
				else
					ScrollViewContentHeight = rect.y;
			else
				ScrollViewContentHeight = rect.y;
		}
	}

	public void ViewProfile(string pid)
	{
		playerIDUnderCheck = pid;
		enabled = true;
		Fetch();
	}
}
