using System.Collections;

public abstract class IWebAPI
{
	public delegate void Callback( IXMLNode d, int code, string msg, string id, Hashtable opt);

	public abstract string gameKey { get; set; }
	public abstract string roarAuthToken { get; }

	public abstract IEnumerator sendCore( string apicall, Hashtable args, Callback cb, Hashtable opt);
	public abstract Hashtable StripHashOfUnderscoreAttribs( Hashtable parse );

	public abstract IAdminActions admin { get; }
	public abstract IFriendsActions friends { get; }
	public abstract IInfoActions info { get; }
	public abstract IItemsActions items { get; }
	public abstract ILeaderboardsActions leaderboards { get; }
	public abstract IMailActions mail { get; }
	public abstract IShopActions shop { get; }
	public abstract IScriptsActions scripts { get; }
	public abstract ITasksActions tasks { get; }
	public abstract IUserActions user { get; }
	public abstract IUrbanairshipActions urbanairship { get; }


	public const int UNKNOWN_ERR  = 0;    // Default unspecified error (parse manually)
	public const int UNAUTHORIZED = 1;    // Auth token is no longer valid. Relogin.
	public const int BAD_INPUTS   = 2;    // Incorrect parameters passed to Roar
	public const int DISALLOWED   = 3;    // Action was not allowed (but otherwise successful)
	public const int FATAL_ERROR  = 4;    // Server died somehow (sad/bad/etc)
	public const int AWESOME      = 11;   // Turn it up.
	public const int OK           = 200;  // Everything ok - proceed


	public interface IAdminActions
	{
		void _set( Hashtable obj, Callback cb, Hashtable opt);
		void delete_player( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IFriendsActions
	{
		void accept( Hashtable obj, Callback cb, Hashtable opt);
		void decline( Hashtable obj, Callback cb, Hashtable opt);
		void invite( Hashtable obj, Callback cb, Hashtable opt);
		void invite_info( Hashtable obj, Callback cb, Hashtable opt);
		void list( Hashtable obj, Callback cb, Hashtable opt);
		void remove( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IInfoActions
	{
		void ping( Hashtable obj, Callback cb, Hashtable opt);
		void user( Hashtable obj, Callback cb, Hashtable opt);
		void poll( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IItemsActions
	{
		void equip( Hashtable obj, Callback cb, Hashtable opt);
		void list( Hashtable obj, Callback cb, Hashtable opt);
		void sell( Hashtable obj, Callback cb, Hashtable opt);
		void _set( Hashtable obj, Callback cb, Hashtable opt);
		void unequip( Hashtable obj, Callback cb, Hashtable opt);
		void use( Hashtable obj, Callback cb, Hashtable opt);
		void view( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface ILeaderboardsActions
	{
		void list( Hashtable obj, Callback cb, Hashtable opt);
		void view( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IMailActions
	{
		void accept( Hashtable obj, Callback cb, Hashtable opt);
		void send( Hashtable obj, Callback cb, Hashtable opt);
		void what_can_i_accept( Hashtable obj, Callback cb, Hashtable opt);
		void what_can_i_send( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IShopActions
	{
		void list( Hashtable obj, Callback cb, Hashtable opt);
		void buy( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IScriptsActions
	{
		void run( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface ITasksActions
	{
		void list( Hashtable obj, Callback cb, Hashtable opt);
		void start( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IUserActions
	{
		void achievements( Hashtable obj, Callback cb, Hashtable opt);
		void create( Hashtable obj, Callback cb, Hashtable opt);
		void login( Hashtable obj, Callback cb, Hashtable opt);
		void login_facebook_oauth( Hashtable obj, Callback cb, Hashtable opt);
		void logout( Hashtable obj, Callback cb, Hashtable opt);
		void netdrive_save( Hashtable obj, Callback cb, Hashtable opt);
		void netdrive_fetch( Hashtable obj, Callback cb, Hashtable opt);
		void _set( Hashtable obj, Callback cb, Hashtable opt);
		void view( Hashtable obj, Callback cb, Hashtable opt);
	}

	public interface IUrbanairshipActions
	{
		void ios_register( Hashtable obj, Callback cb, Hashtable opt);
		void push( Hashtable obj, Callback cb, Hashtable opt);
	}

}

