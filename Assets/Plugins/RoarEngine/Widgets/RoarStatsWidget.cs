using UnityEngine;
using System.Collections;
using Roar;

public class RoarStatsWidget : RoarUIWidget
{
	public bool autoCalculateContentBounds = true;
	public string defaultValueFormat = "N0";
	public Stat[] statsToDisplay;
	
	private Roar.Components.IProperties properties;
	
	[System.Serializable]
	public class Stat
	{
		public enum StatValueType { Unspecified = 0, String = 1, Number = 2, Boolean = 3 };
		
		public bool enabled;
		public string key;
		public Rect bounds;
		public string title;
		public string titleStyle;
		public string valueFormat;
		public string valueStyle;
		public StatValueType valueType;
		
		private Roar.DomainObjects.PlayerAttribute userStat;
		private string value;
		
		public Stat()
		{
			this.bounds = new Rect(0, 0, 150, 100);	
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
	
	protected override void Awake()
	{
		base.Awake();

		if (useScrollView && autoCalculateContentBounds && statsToDisplay.Length > 0)
		{
			Rect[] rects = new Rect[statsToDisplay.Length];
			for (int i=0; i<statsToDisplay.Length; i++)
			{
				rects[i] = statsToDisplay[i].bounds;
			}
			Rect boundsUnion = RoarUIUtil.UnionRect(rects);
			ScrollViewContentWidth = boundsUnion.width + Mathf.Abs(boundsUnion.x);
			ScrollViewContentHeight = boundsUnion.height + Mathf.Abs(boundsUnion.y);
		}
		
		foreach (Stat stat in statsToDisplay)
		{
			if (stat.valueFormat == null || stat.valueFormat.Length == 0)
			{
				stat.valueFormat = defaultValueFormat;
			}
		}

		RoarManager.propertiesChangeEvent += LoadPropertyData;
	}
	
	protected void LoadPropertyData()
	{
		if (properties != null)
		{
			foreach (Stat stat in statsToDisplay)
			{
				if (stat != null && stat.key != null && stat.key.Length > 0)
				{
					Roar.DomainObjects.PlayerAttribute userStat = properties.GetProperty(stat.key);
					stat.UserStat = userStat;
					
					if (stat.valueFormat.Length > 0)
						stat.Value = string.Format("{0:"+stat.valueFormat+"}", userStat != null ? userStat.value : "null");
						//stat.Value = string.Format("{0:"+stat.valueFormat+"}", properties.GetValue(stat.key));
					else
						stat.Value = userStat != null ? userStat.value : null;
						//stat.Value = properties.GetValue(stat.key);
					
					if (stat.title == null || stat.title.Length == 0)
					{
						stat.title = userStat != null ? userStat.label : "UNKNOWN";
						/*
						object statProperty = properties.GetProperty(stat.key);
						if (statProperty is Hashtable)
						{
							Hashtable property = (Hashtable)statProperty;
							if (property.ContainsKey("label"))
							{
								stat.title = (string)property["label"];
							}
						}
						*/
					}
					else
					{
						//TODO: Why was this here?
						//userStat.Title = stat.title;
					}
				}
			}
		}
	}
	
	protected override void OnEnable ()
	{
		base.OnEnable ();
		properties = DefaultRoar.Instance.Properties;
		if (Debug.isDebugBuild && properties == null)
		{
			Debug.LogWarning("Properties is null; unable to render stats widget");
		}

		LoadPropertyData();
	}
	
	protected override void DrawGUI(int windowId)
	{
		if (properties == null) return;
		
		foreach (Stat stat in statsToDisplay)
		{
			if (stat != null && stat.enabled)
			{
				GUI.Label(stat.bounds, stat.title, stat.titleStyle);
				GUI.Label(stat.bounds, stat.Value, stat.valueStyle);
			}
		}
	}
}
