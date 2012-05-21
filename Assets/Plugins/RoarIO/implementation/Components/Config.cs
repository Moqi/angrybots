using System.Collections;

namespace Roar.implementation.Components
{

public class Config : Roar.Components.IConfig
{
	private IWebAPI webapi_;
	private Hashtable props = new Hashtable();
	
	public Config( IWebAPI webapi )
	{
		webapi_ = webapi;
	props["DEBUG"]=true;
    props["game"]="";
    props["apiOnly"]=false;
    props["applifierAppID"]=null;
    props["applifierBarType"]="leaderboard";
    props["autoLogin"]=false;

    // Automatic load data flags
    props["autoLoadPlayerData"]=true;
    props["autoLoadProperties"]=false;
    props["autoLoadInventory"]=false;
    props["autoLoadBadges"]=false;
    props["autoLoadFriends"]=false;
    props["autoLoadShops"]=false;
    props["autoLoadActions"]=false;
    props["autoLoadGifts"]=false;

    props["clariticsAPIKey"]=null;
    props["connectionTimeout"]=5000;
    props["platform"]="web";
    props["pollInterval"]=0;
    props["propertyNotices"]=new ArrayList();
    props["superrewardsAppID"]=null;
    props["uiDimensions"]= new Hashtable();
	(props["uiDimensions"] as Hashtable)["height"]=320;
	(props["uiDimensions"] as Hashtable)["width"]=480 ;
	
	// 3rd Party Adapters
	props["urbanAirshipEnabled"]=false;

	}
	
	public void setVal( string k, string v)
	{
		if( k=="game")
		{
			webapi_.gameKey = v;
		}
		props[k]=v;
	}
		
}
	
}

