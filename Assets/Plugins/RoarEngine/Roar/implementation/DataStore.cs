using DC = Roar.DataConversion;
using System.Collections;
using System.Collections.Generic;
using Roar.DomainObjects;


public interface IDomToCache<DT,CT>
{
	Dictionary<string,CT> convert(DT d);
}

public class UserViewToProperty : IDomToCache<Roar.WebObjects.User.ViewResponse, Roar.DomainObjects.PlayerAttribute>
{
	public Dictionary<string,Roar.DomainObjects.PlayerAttribute> convert(Roar.WebObjects.User.ViewResponse d)
	{
		Dictionary<string,Roar.DomainObjects.PlayerAttribute> retval = new Dictionary<string, Roar.DomainObjects.PlayerAttribute>();
		foreach( Roar.DomainObjects.PlayerAttribute p in d.attributes)
		{
			retval[p.ikey] = p;
		}
		return retval;
	}
}

public class ItemsListToItem : IDomToCache<Roar.WebObjects.Items.ListResponse,Roar.DomainObjects.InventoryItem>
{
	public Dictionary<string, Roar.DomainObjects.InventoryItem> convert( Roar.WebObjects.Items.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.InventoryItem> retval = new Dictionary<string, Roar.DomainObjects.InventoryItem>();
		foreach( Roar.DomainObjects.InventoryItem item in d.items )
		{
			retval[item.id] = item;
		}
		return retval;
	}
}

public class ShopListToShopEntry : IDomToCache<Roar.WebObjects.Shop.ListResponse,Roar.DomainObjects.ShopEntry>
{
	public Dictionary<string, Roar.DomainObjects.ShopEntry> convert( Roar.WebObjects.Shop.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.ShopEntry> retval = new Dictionary<string, Roar.DomainObjects.ShopEntry>();
		foreach( Roar.DomainObjects.ShopEntry x in d.shop_entries )
		{
			retval[x.ikey] = x;
		}
		return retval;
	}
}

public class FriendsListToFriend : IDomToCache<Roar.WebObjects.Friends.ListResponse,Roar.DomainObjects.Friend>
{
	public Dictionary<string, Roar.DomainObjects.Friend> convert( Roar.WebObjects.Friends.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.Friend> retval = new Dictionary<string, Roar.DomainObjects.Friend>();
		foreach( Roar.DomainObjects.Friend f in d.friends )
		{
			retval[ f.player_id ] = f;
		}
		return retval;
	}
}

public class ItemsViewToItemPrototype: IDomToCache<Roar.WebObjects.Items.ViewResponse,Roar.DomainObjects.ItemPrototype>
{
	public Dictionary<string, Roar.DomainObjects.ItemPrototype> convert( Roar.WebObjects.Items.ViewResponse d)
	{
		Dictionary<string,Roar.DomainObjects.ItemPrototype> retval = new Dictionary<string, Roar.DomainObjects.ItemPrototype>();
		//TODO: Implement this
		return retval;
	}
}

public class TaskListToTask : IDomToCache<Roar.WebObjects.Tasks.ListResponse, Roar.DomainObjects.Task>
{
	public Dictionary<string, Roar.DomainObjects.Task> convert( Roar.WebObjects.Tasks.ListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.Task> retval = new Dictionary<string, Roar.DomainObjects.Task>();
		foreach( Roar.DomainObjects.Task task in d.tasks )
		{
			retval[task.ikey] = task;
		}
		return retval;
	}
}

public class FacebookShopListToFacebookShop : IDomToCache<Roar.WebObjects.Facebook.ShopListResponse,Roar.DomainObjects.FacebookShopEntry>
{
	public Dictionary<string, Roar.DomainObjects.FacebookShopEntry> convert( Roar.WebObjects.Facebook.ShopListResponse d)
	{
		Dictionary<string,Roar.DomainObjects.FacebookShopEntry> retval = new Dictionary<string, Roar.DomainObjects.FacebookShopEntry>();
		foreach( Roar.DomainObjects.FacebookShopEntry x in d.shop_list )
		{
			retval[x.ikey] = x;
		}
		return retval;
	}
}

public class LeaderboardViewToLeaderboardData : IDomToCache<Roar.WebObjects.Leaderboards.ViewResponse, Roar.DomainObjects.LeaderboardData>
{
	public Dictionary<string, Roar.DomainObjects.LeaderboardData> convert( Roar.WebObjects.Leaderboards.ViewResponse d)
	{
		Dictionary<string,Roar.DomainObjects.LeaderboardData> retval = new Dictionary<string, Roar.DomainObjects.LeaderboardData>();
		return retval;
	}
}

public class MailAcceptResponseToAcceptableMails : IDomToCache<Roar.WebObjects.Mail.WhatCanIAcceptResponse,Roar.DomainObjects.MailPackage>
{
	public Dictionary<string, Roar.DomainObjects.MailPackage> convert( Roar.WebObjects.Mail.WhatCanIAcceptResponse d)
	{
		Dictionary<string,Roar.DomainObjects.MailPackage> retval = new Dictionary<string, Roar.DomainObjects.MailPackage>();
		foreach( Roar.DomainObjects.MailPackage x in d.packages )
		{
			retval[x.id] = x;
		}
		return retval;
	}
}

public class MailSendResponseToMailable : IDomToCache<Roar.WebObjects.Mail.WhatCanISendResponse,Roar.DomainObjects.Mailable>
{
	public Dictionary<string, Roar.DomainObjects.Mailable> convert( Roar.WebObjects.Mail.WhatCanISendResponse d)
	{
		Dictionary<string,Roar.DomainObjects.Mailable> retval = new Dictionary<string, Roar.DomainObjects.Mailable>();
		foreach( Roar.DomainObjects.Mailable x in d.mailables )
		{
			retval[x.id] = x;
		}
		return retval;
	}
}

public class FooToFoo : IDomToCache<Foo,Foo>
{
	public Dictionary<string, Foo> convert( Foo d)
	{
		throw new System.NotImplementedException();
	}
}


public interface IDomGetter<DT>
{
	void get( ZWebAPI.Callback<DT> cb );
	

}

	
	
public class GenericGetter<T,A> : IDomGetter<T> where A:new()
{
	public delegate void GetFn( A args, ZWebAPI.Callback<T> cb );
	
	GetFn getfn;
	
	public GenericGetter( GetFn getfn ) 
	{
		this.getfn = getfn;
	}
	
	
	public void get( ZWebAPI.Callback<T> cb )
	{
		A args = new A();
		getfn(args,cb);
	}
}

public class UserViewGetter : GenericGetter<Roar.WebObjects.User.ViewResponse, Roar.WebObjects.User.ViewArguments>
{
	public UserViewGetter( IWebAPI api ) : base( api.user.view ) {}
}

public class ItemsListGetter : GenericGetter<Roar.WebObjects.Items.ListResponse, Roar.WebObjects.Items.ListArguments>
{
	public ItemsListGetter( IWebAPI api ) :base( api.items.list ) {}
}

public class ShopListGetter : GenericGetter<Roar.WebObjects.Shop.ListResponse, Roar.WebObjects.Shop.ListArguments>
{
	public ShopListGetter( IWebAPI api ) :base( api.shop.list ) {}
}

public class FriendsListGetter : GenericGetter<Roar.WebObjects.Friends.ListResponse, Roar.WebObjects.Friends.ListArguments>
{
	public FriendsListGetter( IWebAPI api ) : base( api.friends.list ) {}
}

public class LeaderboardViewGetter : GenericGetter<Roar.WebObjects.Leaderboards.ViewResponse, Roar.WebObjects.Leaderboards.ViewArguments>
{
	public LeaderboardViewGetter( IWebAPI api ) : base( api.leaderboards.view) {}
}

public class ItemsViewGetter : GenericGetter<Roar.WebObjects.Items.ViewResponse, Roar.WebObjects.Items.ViewArguments>
{
	public ItemsViewGetter( IWebAPI api ) : base( api.items.view ) {}
}

public class TasksListGetter : GenericGetter<Roar.WebObjects.Tasks.ListResponse, Roar.WebObjects.Tasks.ListArguments>
{
	public TasksListGetter( IWebAPI api ) : base( api.tasks.list ) {}
}

public class FacebookShopListGetter : GenericGetter<Roar.WebObjects.Facebook.ShopListResponse, Roar.WebObjects.Facebook.ShopListArguments>
{
	public FacebookShopListGetter( IWebAPI api ) : base( api.facebook.shop_list ) {}
}

public class GiftsAcceptableGetter : GenericGetter<Roar.WebObjects.Mail.WhatCanIAcceptResponse, Roar.WebObjects.Mail.WhatCanIAcceptArguments>
{
	public GiftsAcceptableGetter( IWebAPI api ) : base( api.mail.what_can_i_accept ) {}
}

public class GiftsSendableGetter : GenericGetter<Roar.WebObjects.Mail.WhatCanISendResponse, Roar.WebObjects.Mail.WhatCanISendArguments>
{
	public GiftsSendableGetter( IWebAPI api ) : base( api.mail.what_can_i_send ) {}
}

public class FooGetter : IDomGetter<Foo>
{
	public FooGetter( IWebAPI api ) 
	{
		this.api = api;
	}
	
	protected IWebAPI api;
	public void get( ZWebAPI.Callback<Foo> cb )
	{
		throw new System.NotImplementedException();
	}
}


namespace Roar.implementation
{
	public interface IDataStore
	{
		void Clear (bool x);
		
		IDataModel<DomainObjects.PlayerAttribute,Roar.WebObjects.User.ViewResponse> properties { get; }
		IDataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory { get; }
		IDataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop { get; }
		IDataModel<DomainObjects.Task,WebObjects.Tasks.ListResponse> actions { get; }
		IDataModel<DomainObjects.FacebookShopEntry,WebObjects.Facebook.ShopListResponse> facebookShop {get;}
		IDataModel<DomainObjects.MailPackage,WebObjects.Mail.WhatCanIAcceptResponse> giftsAcceptable { get; }
		IDataModel<DomainObjects.Mailable,WebObjects.Mail.WhatCanISendResponse> giftsSendable { get; }
		IDataModel<Foo,Foo> achievements { get; }
		IDataModel<DomainObjects.LeaderboardData,WebObjects.Leaderboards.ViewResponse> ranking { get; }
		IDataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends { get; }
		IDataModel<Foo,Foo> appStore { get; }
		IItemCache cache { get; }
	}


	public class DataStore : IDataStore
	{
		public DataStore (IWebAPI webapi, ILogger logger)
		{
			//This should convert from the response type to key-value pairs
			
			properties_ = new DataModel<DomainObjects.PlayerAttribute,Roar.WebObjects.User.ViewResponse> ("properties", new UserViewGetter(webapi), new UserViewToProperty(), logger);
			inventory_ = new DataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> ("inventory", new ItemsListGetter(webapi), new ItemsListToItem(), logger);
			shop_ = new DataModel<DomainObjects.ShopEntry,WebObjects.Shop.ListResponse>("shop", new ShopListGetter(webapi), new ShopListToShopEntry(), logger);
			actions_ = new DataModel<DomainObjects.Task,WebObjects.Tasks.ListResponse> ("tasks", new TasksListGetter(webapi), new TaskListToTask(), logger);
			facebookShop_ = new DataModel<DomainObjects.FacebookShopEntry, WebObjects.Facebook.ShopListResponse>("facebook", new FacebookShopListGetter(webapi), new FacebookShopListToFacebookShop(), logger);
			giftsAcceptable_ = new DataModel<DomainObjects.MailPackage, WebObjects.Mail.WhatCanIAcceptResponse>("giftsAcceptable", new GiftsAcceptableGetter(webapi), new MailAcceptResponseToAcceptableMails(), logger);
			giftsSendable_ = new DataModel<DomainObjects.Mailable, WebObjects.Mail.WhatCanISendResponse>("giftsSendable", new GiftsSendableGetter(webapi), new MailSendResponseToMailable(), logger);
			achievements_ = new DataModel<Foo,Foo> ("achievements", new FooGetter(webapi), new FooToFoo(), logger);
			ranking_ = new DataModel<DomainObjects.LeaderboardData,WebObjects.Leaderboards.ViewResponse> ("ranking", new LeaderboardViewGetter(webapi), new LeaderboardViewToLeaderboardData(), logger);
			friends_ = new DataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> ("friends",  new FriendsListGetter(webapi), new FriendsListToFriend(), logger);
			cache_ = new ItemCache ("cache", new ItemsViewGetter(webapi), new ItemsViewToItemPrototype(), logger);
			appStore_ = new DataModel<Foo,Foo> ("appstore", new FooGetter(webapi), new FooToFoo(), logger);
		}

		public void Clear (bool x)
		{
			properties.Clear (x);
			inventory.Clear (x);
			shop.Clear (x);
			actions.Clear (x);
			facebookShop.Clear(x);
			giftsAcceptable.Clear (x);
			achievements.Clear (x);
			ranking.Clear (x);
			friends.Clear (x);
			cache.Clear (x);
			appStore.Clear (x);
		}
		
		

		public IDataModel<DomainObjects.PlayerAttribute,Roar.WebObjects.User.ViewResponse> properties { get { return properties_; } }
		public IDataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory { get { return inventory_; } }
		public IDataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop { get { return shop_; } }
		public IDataModel<DomainObjects.Task,WebObjects.Tasks.ListResponse> actions { get { return actions_; } }
		public IDataModel<DomainObjects.FacebookShopEntry, WebObjects.Facebook.ShopListResponse> facebookShop {get {return facebookShop_;}}
		public IDataModel<DomainObjects.MailPackage,WebObjects.Mail.WhatCanIAcceptResponse> giftsAcceptable { get { return giftsAcceptable_; } }
		public IDataModel<DomainObjects.Mailable,WebObjects.Mail.WhatCanISendResponse> giftsSendable { get { return giftsSendable_; } }
		public IDataModel<Foo,Foo> achievements { get { return achievements_; } }
		public IDataModel<DomainObjects.LeaderboardData,WebObjects.Leaderboards.ViewResponse> ranking { get { return ranking_; } }
		public IDataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends { get { return friends_; } }
		public IDataModel<Foo,Foo> appStore { get { return appStore_; } }
		public IItemCache cache { get { return cache_; } }
		
		public DataModel<DomainObjects.PlayerAttribute,Roar.WebObjects.User.ViewResponse> properties_;
		public DataModel<DomainObjects.InventoryItem,Roar.WebObjects.Items.ListResponse> inventory_;
		public DataModel<DomainObjects.ShopEntry,Roar.WebObjects.Shop.ListResponse> shop_;
		public DataModel<DomainObjects.Task,WebObjects.Tasks.ListResponse> actions_;
		public DataModel<DomainObjects.FacebookShopEntry, WebObjects.Facebook.ShopListResponse> facebookShop_;
		public DataModel<DomainObjects.MailPackage,WebObjects.Mail.WhatCanIAcceptResponse> giftsAcceptable_;
		public DataModel<DomainObjects.Mailable,WebObjects.Mail.WhatCanISendResponse> giftsSendable_;
		public DataModel<Foo,Foo> achievements_;
		public DataModel<DomainObjects.LeaderboardData,WebObjects.Leaderboards.ViewResponse> ranking_;
		public DataModel<DomainObjects.Friend,WebObjects.Friends.ListResponse> friends_;
		public DataModel<Foo,Foo> appStore_;
		public ItemCache cache_;
	}

}
