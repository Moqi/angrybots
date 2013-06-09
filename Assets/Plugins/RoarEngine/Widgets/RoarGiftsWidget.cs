using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;

public class RoarGiftsWidget : RoarUIWidget
{
	public enum WhenToFetch { OnEnable, Once, Occassionally, Manual };
	public WhenToFetch whenToFetch = WhenToFetch.Occassionally;
	public float howOftenToFetch = 60;
	private bool isFetching;
	private float whenLastFetched;
	private Roar.Components.IGifts gifts;
	public float entrySpacing;
	public Rect  entryBounds = new Rect(0, 0, 350, 25);

	public string playerIdFormatString ="{0}";
	public string playerIdFormat = "DefaultLabel";
	public string nameFormatString = "{0}";
	public string messageFormat = "DefaultLabel";
	public string levelFormatString = "{0}";
	public string typeFormat = "DefaultLabel";
	public string buttonFormat = "DefaultButton";

	public float topSpacing = 5;
	public float sectionHeight = 45;
	public float fromColumnWidth = 100;
	public float messageColumnWidth = 200;
	public float typeColumnWidth = 80;
	public float interColumnSeprators = 10;
	public float divideHeight = 25;
	public float buttonWidth = 90;
	public float buttonHeight = 40;
	public float footerSpacing = 90;
	Vector2 internalScrollSize = Vector2.zero;
	public float messageBoxWidth = 150;
	public float messageBoxHeight = 60;
	public float playerIdBoxWidth = 150;
	public float textBoxHeight = 30;
	public float labelWidth = 130;
	public float labelHeight = 30;
	public float verticalSeparators = 5;
	public float selectButtonWidth = 20;
	string statusString;

	Mailable selectedMailable = null;
	string playerIdToSendTo = "";
	string messageToSend= "";

	enum Section
	{
		Normal,
		ReadingMail,
		SendingMail,
	}
	Section section = Section.Normal;
	MailPackage mailBeingRead;

	IDictionary<string, Roar.DomainObjects.MailPackage> mailDict = null;
	IDictionary<string, Roar.DomainObjects.Mailable> mailableDict = null;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		gifts = DefaultRoar.Instance.Gifts;
		if (gifts != null)
		{
			if (whenToFetch == WhenToFetch.OnEnable 
			|| (whenToFetch == WhenToFetch.Once && !gifts.HasDataFromServer)
			|| (whenToFetch == WhenToFetch.Occassionally && (whenLastFetched == 0 || Time.realtimeSinceStartup - whenLastFetched >= howOftenToFetch))
			)
			{
				Fetch();
				FetchMailable();
			}
		}
		else if (Debug.isDebugBuild)
		{
			Debug.LogWarning("Friends data is null; unable to render friends list widget");
		}
	}

	public void Fetch()
	{
		networkActionInProgress = true;
		isFetching = true;
		gifts.Fetch(OnRoarFetchGiftsComplete);
		gifts.FetchSendable(OnRoarFetchMailableGiftsComplete);
	}
	
	public void FetchMailable()
	{
		networkActionInProgress = true;
		gifts.FetchSendable(OnRoarFetchMailableGiftsComplete);
	}
	
	void OnRoarFetchGiftsComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.MailPackage> > info)
	{
		whenLastFetched = Time.realtimeSinceStartup;
		networkActionInProgress = false;
		isFetching = false;
		mailDict = info.data;
		Debug.Log("acceptable gifts count "+mailDict.Count);
	}

	void OnRoarFetchMailableGiftsComplete(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Mailable> > info)
	{
		networkActionInProgress = false;
		mailableDict = info.data;
		Debug.Log("sendable gifts count "+mailableDict.Count);
	}

	protected override void DrawGUI(int windowId)
	{
		if (!IsLoggedIn) return;
		if (isFetching)
		{
			GUI.Label(new Rect(this.bounds.width/2f - 256,this.bounds.height/2f - 32,512,64), "Fetching Gifts...", "StatusNormal");
			return;
		}

		if( gifts==null)
		{
			GUI.Label(new Rect(this.bounds.width/2f - 256,this.bounds.height/2f - 32,512,64), "Error loading Gifts...", "StatusError");
			return;
		}

		float heightSoFar = 0;
		Rect currentRect = new Rect(interColumnSeprators, heightSoFar, fromColumnWidth, divideHeight);

		if(section == Section.Normal)
		{
			GUI.Box(new Rect(0, 0, contentBounds.width, divideHeight), new GUIContent(""), "DefaultSeparationBar");
			GUI.Label ( currentRect, "FROM", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + fromColumnWidth;

			GUI.Label ( currentRect, "MESSAGE", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + messageColumnWidth;

			GUI.Label ( currentRect, "TYPE", "DefaultSeparationBarText");
			currentRect.x += interColumnSeprators + typeColumnWidth;
			currentRect.x = interColumnSeprators;
			currentRect.y += divideHeight;
			currentRect.height = sectionHeight;

			internalScrollSize = GUI.BeginScrollView(new Rect(currentRect.x, currentRect.y, contentBounds.width, contentBounds.height - currentRect.y - verticalSeparators - labelHeight*2 -footerSpacing), internalScrollSize, 
				new Rect(currentRect.x, currentRect.y, contentBounds.width - interColumnSeprators*2, sectionHeight*mailDict.Count));
			foreach (KeyValuePair<string,Roar.DomainObjects.MailPackage> f in mailDict)
			{
				currentRect.width = fromColumnWidth;
				GUI.Label(currentRect,  f.Value.sender_name, playerIdFormat );
				currentRect.x += interColumnSeprators + fromColumnWidth;
				currentRect.width = messageColumnWidth;
				GUI.Label(currentRect, f.Value.message, messageFormat );
				currentRect.x += interColumnSeprators + messageColumnWidth;
				currentRect.width = typeColumnWidth;
				GUI.Label(currentRect, f.Value.type, typeFormat );
				currentRect.x += interColumnSeprators + typeColumnWidth;

				currentRect.width = buttonWidth;
				currentRect.height = buttonHeight;
				if(GUI.Button(currentRect, "Open", buttonFormat))
				{
					statusString = "";
					mailBeingRead = f.Value;
					section = Section.ReadingMail;
					drawSubheading = true;
					subheaderName = f.Value.sender_name+"'s Message";
				}
				currentRect.x = interColumnSeprators;
				
				currentRect.y += sectionHeight;
			}
			GUI.EndScrollView();

			currentRect.y += verticalSeparators;
			currentRect.width = contentBounds.width - interColumnSeprators*2;
			currentRect.height = labelHeight *2;

			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");
			
			GUI.Box(new Rect(0, contentBounds.height - footerSpacing, contentBounds.width, footerSpacing), new GUIContent(""), "DefaultFooterStyle");
			currentRect.x = interColumnSeprators;
			currentRect.y = contentBounds.height - footerSpacing/2 - buttonHeight/2;
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;

			if(GUI.Button(currentRect, "Send Gift", "DefaultButton"))
			{
				statusString = "";
				FetchMailable();
				section = Section.SendingMail;
				drawSubheading = true;
				subheaderName = "Send Gift";
			}
		}

		if(section == Section.ReadingMail)
		{
			currentRect.width = contentBounds.width;
			currentRect.height = divideHeight;
			currentRect.x = 0;
			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");
			currentRect.y += divideHeight;
			
			currentRect.y += divideHeight;
			currentRect.x = interColumnSeprators;
			currentRect.width = 3*labelWidth + 2*interColumnSeprators;
			currentRect.height = labelHeight;

			GUI.Label(currentRect, "From: " + mailBeingRead.sender_name +"("+mailBeingRead.sender_id+")");
			currentRect.width = contentBounds.width - interColumnSeprators;
			currentRect.y += labelHeight;
			GUI.Label(currentRect, "Message: ", "DefaultLabel");
			currentRect.x += labelWidth+interColumnSeprators;
			currentRect.width = contentBounds.width - interColumnSeprators;
			GUI.Label(currentRect, mailBeingRead.message, "DefaultLabel");

			currentRect.y += labelHeight;
			currentRect.x = interColumnSeprators;

			CRMVisitor<string> visitorObject = new RoarTasksWidget.CRMToString();
			string modifierString = "Modifiers: \n";
			foreach(Modifier m in mailBeingRead.modifiers)
				modifierString += visitorObject.visit_modifier(m)+"\n";

			if(mailBeingRead.modifiers.Count == 0)
				modifierString += "None";
			
			Vector2 size = GUI.skin.FindStyle("DefaultLabel").CalcSize(new GUIContent(modifierString));

			currentRect.width = size.x;
			currentRect.height = size.y;
			GUI.Label(currentRect, modifierString, "DefaultLabel");

			currentRect.y += currentRect.height+verticalSeparators;
			currentRect.x = interColumnSeprators;

			string itemsString = "Items: \n";
			
			foreach(InventoryItem m in mailBeingRead.items)
				itemsString += m.label;

			Vector2 itemSize = GUI.skin.FindStyle("DefaultLabel").CalcSize(new GUIContent(itemsString));
			currentRect.width = itemSize.x;
			currentRect.height = itemSize.y;
			GUI.Label(currentRect, itemsString, "DefaultLabel");
			
			currentRect.y += currentRect.height+verticalSeparators;
			currentRect.x = interColumnSeprators;
			
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;
			if(GUI.Button(currentRect, "Accept", "DefaultButton"))
			{
				gifts.AcceptGift(mailBeingRead.id, onRoarGiftsGiftAccepted);
				networkActionInProgress = true;
			}

			currentRect.x += buttonWidth + interColumnSeprators;
			if(GUI.Button(currentRect, "Back", "DefaultButton"))
			{
				statusString = "";
				drawSubheading = false;
				section = Section.Normal;
			}
			currentRect.x += interColumnSeprators + typeColumnWidth;
		}

		if(section == Section.SendingMail)
		{
			currentRect.y += verticalSeparators;
			currentRect.width = contentBounds.width;
			currentRect.height = divideHeight;
			currentRect.x = 0;
			GUI.Label(currentRect, statusString, "DefaultSmallStatusText");
			currentRect.y += divideHeight;
			
			currentRect.x = interColumnSeprators;
			currentRect.width = labelWidth;
			currentRect.height = labelHeight;
			GUI.Label(currentRect,"To: (PlayerID)");
			currentRect.x += labelWidth + interColumnSeprators;
			currentRect.width = contentBounds.width- currentRect.x  - interColumnSeprators - selectButtonWidth;
			currentRect.height = textBoxHeight;
			playerIdToSendTo = GUI.TextField(currentRect, playerIdToSendTo, "DefaultTextArea");
			currentRect.x += contentBounds.width- currentRect.x  - interColumnSeprators - selectButtonWidth;
			currentRect.width = selectButtonWidth;
			if(GUI.Button(currentRect, "?", "DefaultButton"))
			{
				Debug.Log("going");
				System.Action<string> act = (pid) => {
					playerIdToSendTo = pid;
				};
				GameObject.Find("/PlayerSelectionWidget").SendMessage("setCallbackAndEnable", act);
			}
			currentRect.y += labelHeight+verticalSeparators;
			currentRect.x = interColumnSeprators;
			currentRect.width = labelWidth;
			currentRect.height = labelHeight;
			GUI.Label(currentRect, "Message: ", "DefaultLabel");
			currentRect.x += labelWidth + interColumnSeprators;
			currentRect.width = contentBounds.width -currentRect.x  - interColumnSeprators;
			currentRect.height = messageBoxHeight;
			messageToSend = GUI.TextArea(currentRect, messageToSend, "DefaultBigMessageBox");
			
			currentRect.y += messageBoxHeight+verticalSeparators;
			currentRect.x = interColumnSeprators;
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;
			
			if(GUI.Button(currentRect, "Send", "DefaultButton"))
			{
				if(selectedMailable == null)
				{
					statusString = "Please select an item to send with your gift";
				}
				else
				{
					networkActionInProgress = true;
					gifts.SendGift(playerIdToSendTo, selectedMailable.id, messageToSend, onRoarGiftsGiftSent);
				}
			}
			
			currentRect.x += buttonWidth + interColumnSeprators;
			if(GUI.Button(currentRect, "Back", "DefaultButton"))
			{
				statusString = "";
				drawSubheading = false;
				section = Section.Normal;
			}
			currentRect.x = interColumnSeprators;
			currentRect.y+= buttonHeight+verticalSeparators;
			internalScrollSize = GUI.BeginScrollView(new Rect(currentRect.x, currentRect.y, contentBounds.width, contentBounds.height - currentRect.y), internalScrollSize, 
				new Rect(currentRect.x, currentRect.y, contentBounds.width - interColumnSeprators*2, sectionHeight*mailableDict.Count));
			
			foreach (KeyValuePair<string,Roar.DomainObjects.Mailable> f in mailableDict)
			{
				currentRect.width = fromColumnWidth;
				GUI.Label(currentRect,  f.Value.label, playerIdFormat );
				currentRect.x += interColumnSeprators + fromColumnWidth;
				currentRect.width = messageColumnWidth;
				GUI.Label(currentRect, f.Value.type, messageFormat );
				currentRect.x += interColumnSeprators + messageColumnWidth;
				
				if(GUI.Toggle(currentRect, (selectedMailable == f.Value)?true:false,"Select"))
				{
					selectedMailable = f.Value;
				}
				currentRect.x = interColumnSeprators;
				
				currentRect.y += sectionHeight;
			}
			GUI.EndScrollView();
			currentRect.x += interColumnSeprators + typeColumnWidth;
		}
	}

	void onRoarGiftsGiftAccepted(Roar.CallbackInfo<Roar.WebObjects.Mail.AcceptResponse> data)
	{
		networkActionInProgress = false;
		if(data.code == IWebAPI.OK)
		{
			section = Section.Normal;
			statusString = "Gift accepted successfully!";
			Debug.Log("gift accepted "+ data.msg);
		}
		else
		{
			statusString = "Gift acceptance Failed, Reason: "+data.msg;
			Debug.Log("gift accepted "+ data.msg);
		}
	}

	void onRoarGiftsGiftSent(Roar.CallbackInfo<Roar.WebObjects.Mail.SendResponse> data)
	{
		networkActionInProgress = false;
		if(data.code == IWebAPI.OK)
		{
			statusString = "Gift sent succcessfully!";
			Debug.Log("Gift sent "+data.msg);
			selectedMailable = null;
		}
		else
		{
			statusString = "Gift sending failed. Reason: "+data.msg;
			Debug.Log("Gift sent "+data.msg);
		}
	}

	public void SendMailWithPID(string pid)
	{
		enabled = true;
		section = Section.SendingMail;
		statusString = "";
		FetchMailable();
		section = Section.SendingMail;
		drawSubheading = true;
		subheaderName = "Send Gift";
		playerIdToSendTo = pid;
	}
}