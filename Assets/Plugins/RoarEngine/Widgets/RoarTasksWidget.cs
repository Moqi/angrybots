using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;

public class RoarTasksWidget : RoarUIWidget
{
	
	protected Roar.Components.ITasks tasks;
	protected bool isFetching=false;
	public string labelFormat = "DefaultHeavyContentLeftText";
	public string descriptionFormat = "DefaultLightContentText";
	public string locationFormat = "DefaultHeavyContentText";
	public string detailFormat = "DefaultLightContentText";
	
	public bool showDescription = true;
	public bool showType = true;
	public bool allowSorting = true;
	
	public int maxLabelWidth = 250;
	public int maxDescriptionFormatWidth = 350;
	public int maxLocationWidth = 100;
	public int rowHeight = 45;
	public int buttonWidth = 130;
	public int buttonHeight = 30;
	public int divideHeight = 30;
	public int margin = 5;
	
	bool hasSelectedTask = false;
	Roar.DomainObjects.Task selectedTask = null;
	
	void Reset()
	{
		bounds.y = 100;
		bounds.width = 880;
		bounds.height = 500;
		displayName = "Tasks";
	}

	public class CRMToString : CRMVisitor<string>
	{
		public override string OnCostNull ()
		{
			return "No Costs!";
		}
		public override string OnCostStat (Roar.DomainObjects.Costs.Stat stat)
		{
			return " Stat : " + stat.ikey + " : " + stat.value;
		}
		
		public override string OnCostItem (Roar.DomainObjects.Costs.Item item)
		{
			return " Item: "+item.ikey+" Num:"+item.number_required;
		}
		
		public override string OnCostMultiple( Roar.DomainObjects.Costs.Multiple multi )
		{
			string subcosts = "";
			foreach( Cost c in multi.costs )
			{
				subcosts += this.visit_cost(c) + "\n";
			}
			return "Multiple" + subcosts;
		}
		
		public override string OnModifierNull ()
		{
			return "No modifiers";
		}
	
		public override string OnModifierGrantItem (Roar.DomainObjects.Modifiers.GrantItem mod)
		{
			return "Grants Item: "+mod.ikey;
		}
		
		public override string OnModifierGrantStat (Roar.DomainObjects.Modifiers.GrantStat mod)
		{
			return "Grants Stat: "+ mod.ikey+" Type: "+mod.type +" Amount: "+mod.value;
		}
		
		public override string OnModifierGrantStatRange (Roar.DomainObjects.Modifiers.GrantStatRange mod)
		{
			return "Grant Stat Range: " + mod.ikey+" Min-Max: "+mod.min+"-"+mod.max+" Type: "+ mod.type;
		}
		
		public override string OnModifierGrantXp (Roar.DomainObjects.Modifiers.GrantXp mod)
		{
			return "Grants Xp: "+mod.value;
		}
		
		public override string OnModifierGrantXpRange (Roar.DomainObjects.Modifiers.GrantXpRange mod)
		{
			return "Grants Xp Range: Min-Max"+ mod.min+"-"+mod.max;
		}
		
		public override string OnModifierIfThenElse (  Roar.DomainObjects.Modifiers.IfThenElse if_then_else )
		{
			string ifString = "", thenString="", elseString="";
			
			foreach( Requirement r in if_then_else.if_ )
			{
				ifString += visit_requirement(r);
			}
			
			foreach( Modifier m in if_then_else.then_ )
			{
				thenString += visit_modifier(m);
			}
			
			foreach( Modifier m in if_then_else.else_ )
			{
				elseString += visit_modifier(m);
			}
			
			
			return "if("+ ifString+") \n then (" + thenString+ ")\nelse "+elseString;
		}

		public override string OnModifierMultiple (Roar.DomainObjects.Modifiers.Multiple mod)
		{
			string subMods = "";
			foreach( Modifier m in mod.modifier )
			{
				subMods += this.visit_modifier(m) + "\n";
			}
			return "Multiple" + subMods; 
		}
		
		public override string OnModifierNamedReference (Roar.DomainObjects.Modifiers.NamedReference mod)
		{
			return "Named Ref: ??";
		}
		
		public override string OnModifierNothing (Roar.DomainObjects.Modifiers.Nothing mod)
		{
			return "Nothing";
		}
		
		public override string OnModifierRandomChoice (Roar.DomainObjects.Modifiers.RandomChoice mod)
		{
			
			string subs = "";
			foreach( Roar.DomainObjects.Modifiers.RandomChoice.ChoiceEntry ch in mod.choices )
			{
				foreach(Modifier m in ch.modifiers)
					subs += this.visit_modifier(m)+"\n";
				if(ch.requirements != null)
				{
					foreach(Requirement r in ch.requirements)
						subs += this.visit_requirement(r)+"\n";
				}
				subs += "Weight: "+ ch.weight+"\n";
			}
			return "Random Choice: " + subs;
			
		}
		
		public override string OnModifierRemoveItems (Roar.DomainObjects.Modifiers.RemoveItems mod)
		{
			return "Removes Items: "+ mod.ikey + " Count: "+mod.count;
		}
		
		public override string OnModifierScript (Roar.DomainObjects.Modifiers.Script mod)
		{
			return "Script: ??";
		}
		
		public override string OnRequirementNull ()
		{
			return "No Requirements!";
		}
		public override string OnRequirementFalse (Roar.DomainObjects.Requirements.False req)
		{
			return "False: ??";
		}
		
		public override string OnRequirementFriends (Roar.DomainObjects.Requirements.Friends req)
		{
			return "Friends Required: "+req.required;
		}
		
		public override string OnRequirementItem (Roar.DomainObjects.Requirements.Item req)
		{
			return "Item Required: "+req.ikey + " Count: "+req.number_required;
		}
		
		public override string OnRequirementLevel (Roar.DomainObjects.Requirements.Level req)
		{
			return "Lvl required: "+req.level;
		}

		public override string OnRequirementMultiple (Roar.DomainObjects.Requirements.Multiple req)
		{
			string subReq = "";
			foreach( Requirement r in req.requirements )
			{
				subReq += this.visit_requirement(r) + "\n";
			}
			return "Multiple: " + subReq; 
		}
		
		public override string OnRequirementStat (Roar.DomainObjects.Requirements.Stat req)
		{
			return "Stats reqd: "+req.ikey + " Count: "+req.value;
		}
		
		public override string OnRequirementTrue (Roar.DomainObjects.Requirements.True req)
		{
			return "True??";
		}
	}
	
	
	protected override void OnEnable ()
	{
		hasSelectedTask = false;
		drawSubheading = false;
		if (!IsLoggedIn)
		{
			Debug.Log ("RoarTasksWidget enabled before login - unable to fetch data");
			enabled = false;
			return;
		}
		
		tasks = DefaultRoar.Instance.Tasks;		
		Fetch();
	}
	
		
	protected override void DrawGUI(int windowId)
	{
		if (tasks == null || !IsLoggedIn) return;
		
		if (isFetching)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Fetching tasks data...", "StatusNormal");
			return;
		}
		
		if( ! tasks.HasDataFromServer )
		{
			GUI.Label (new Rect(0,0,ContentWidth,ContentHeight), "Task data not loaded!", "StatusNormal");
			return;
		}
		
		if(!hasSelectedTask)
		{
	
			IList<Roar.DomainObjects.Task> items = tasks.List();
			
			Rect rect = new Rect(margin,0,ContentWidth, divideHeight);
			GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
			
			Vector2 lastLabelSize;
			lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( "Label"));
			if(maxLabelWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxLabelWidth;
			GUI.Label ( rect, "LABEL", "DefaultSeparationBarText");
			
			rect.x += rect.width + margin;
			
			lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent("Description"));
			if(maxDescriptionFormatWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxDescriptionFormatWidth;
				
			GUI.Label ( rect, "DESCRIPTION", "DefaultSeparationBarText");
			rect.x += rect.width+ margin;
			
			rect.height = rowHeight;
			rect.y += divideHeight;
			rect.x += rect.width+ margin;
			rect.y += lastLabelSize.y;
			foreach( Roar.DomainObjects.Task item in items )
			{
				GUI.Box(new Rect(0, rect.y, contentBounds.width, rowHeight), new GUIContent(""), "DefaultHorizontalSection");
				
				rect.x =margin;
				
				lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( item.label));
				if(maxLabelWidth == 0)
					rect.width = lastLabelSize.x;
				else
					rect.width = maxLabelWidth;
				GUI.Label ( rect, item.label, labelFormat);
				
				rect.x += rect.width + margin;
				
				lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent(item.description));
				if(maxDescriptionFormatWidth == 0)
					rect.width = lastLabelSize.x;
				else
					rect.width = maxDescriptionFormatWidth;
					
				GUI.Label ( rect, item.description, descriptionFormat);
				rect.x += rect.width+ margin;
				
				lastLabelSize =GUI.skin.FindStyle(locationFormat).CalcSize(new GUIContent(item.location));
				if(maxLocationWidth == 0)
					rect.width = lastLabelSize.x;
				else
					rect.width = maxLocationWidth;
					
				GUI.Label ( rect, item.location, locationFormat);
				rect.x += rect.width+ margin;
				rect.height = buttonHeight;
				rect.width = buttonWidth;
				float oldY = rect.y;
				rect.y+= rowHeight/2 - buttonHeight/2;
				
			
				if(GUI.Button(rect, "View", "DefaultButtonCenter"))
				{
					selectedTask = item;
					hasSelectedTask = true;
					drawSubheading = true;
					subheaderName = selectedTask.ikey;
					
				}
				
				rect.y = oldY;
				
				rect.x = margin;
				rect.y += rowHeight;
			}
		}
		else
		{
			Rect rect = new Rect(margin,0,ContentWidth, 32);
			Vector2 lastLabelSize;
			
			lastLabelSize = GUI.skin.FindStyle(labelFormat).CalcSize(new GUIContent( selectedTask.label));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			GUI.Label ( rect, selectedTask.label, labelFormat);
			
			rect.y += lastLabelSize.y;
			
			lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent(selectedTask.description));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
				
			rect.y += lastLabelSize.y;
			
			lastLabelSize =GUI.skin.FindStyle(locationFormat).CalcSize(new GUIContent("Location: "+selectedTask.location));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			
			GUI.Label ( rect, selectedTask.location, locationFormat);
			rect.y += lastLabelSize.y;
			
			lastLabelSize =GUI.skin.FindStyle(detailFormat).CalcSize(new GUIContent("Mastery lvl: "+selectedTask.mastery_level));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			if(maxLocationWidth == 0)
				rect.width = lastLabelSize.x;
			else
				rect.width = maxLocationWidth;
			rect.x = margin;
				
			GUI.Label ( rect, "Mastery lvl: ", labelFormat);
			rect.x += 200;
			GUI.Label ( rect, selectedTask.mastery_level.ToString(), descriptionFormat);
			rect.x = margin;
			rect.y += lastLabelSize.y;
			
			CRMVisitor<string> visitorObject = new CRMToString();
			
			string costsString = "";
			foreach(Roar.DomainObjects.Cost cost in selectedTask.costs)
			{
				costsString += visitorObject.visit_cost(cost)+"\n";
			}
			
			lastLabelSize =GUI.skin.FindStyle(descriptionFormat).CalcSize(new GUIContent("Costs: "+costsString));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			rect.width = lastLabelSize.x;
			
			GUI.Label ( rect, "Costs: ", labelFormat);
			
			rect.x = 200;
			GUI.Label ( rect, costsString, descriptionFormat);
			rect.y += lastLabelSize.y;
			rect.x = margin;
			string rewardsString = "";
			foreach(Roar.DomainObjects.Modifier modifier in selectedTask.rewards)
			{
				rewardsString += visitorObject.visit_modifier(modifier)+"\n";
			}
			
			
			lastLabelSize =GUI.skin.FindStyle(detailFormat).CalcSize(new GUIContent("Rewards: "+rewardsString));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			rect.x = margin;
			GUI.Label (rect, "Rewards: ", labelFormat);
			rect.x += 200;
			GUI.Label (rect, rewardsString, descriptionFormat);
			rect.y += lastLabelSize.y;
			rect.x = margin;
			
			string requirementsString = "";
			foreach(Roar.DomainObjects.Requirement requirement in selectedTask.requirements)
			{
				requirementsString += visitorObject.visit_requirement(requirement)+"\n";
			}
			
			
			lastLabelSize =GUI.skin.FindStyle(detailFormat).CalcSize(new GUIContent("Requirements: "+requirementsString));
			rect.height = lastLabelSize.y;
			rect.width = lastLabelSize.x;
			
			rect.width = lastLabelSize.x;

			GUI.Label (rect, "Requirements: ", labelFormat);

			rect.x += 200;
			GUI.Label (rect, requirementsString, descriptionFormat);
			rect.x = margin;
			rect.y += lastLabelSize.y;
		
			rect.width = buttonWidth;

			if(GUI.Button(rect, "Back"))
			{
				selectedTask = null;
				hasSelectedTask = false;
			}

			rect.x += rect.width+rect.width/2;

			if(GUI.Button(rect, "Execute"))
			{
				tasks.Execute(selectedTask.ikey, null);
				selectedTask = null;
				hasSelectedTask = false;
				
			}
		
		}
	}
	
	public void Fetch()
	{
		networkActionInProgress = true;
		isFetching = true;
		tasks.Fetch(OnRoarFetchTasksComplete);
	}
	
	void OnRoarFetchTasksComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Task> > data )
	{
		networkActionInProgress = false;
		isFetching = false;
	}

}
