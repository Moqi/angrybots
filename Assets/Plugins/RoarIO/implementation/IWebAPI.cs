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
using System.Collections;

public abstract class IWebAPI
{
	public delegate void Callback( IXMLNode d, int code, string msg, string id, Hashtable opt);

	public abstract string gameKey { get; set; }
	public abstract string roarAuthToken { get; }

	//TODO: This should be removed
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
		void _set( Hashtable obj, RequestCallback cb, Hashtable opt);
		void delete_player( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IFriendsActions
	{
		void accept( Hashtable obj, RequestCallback cb, Hashtable opt);
		void decline( Hashtable obj, RequestCallback cb, Hashtable opt);
		void invite( Hashtable obj, RequestCallback cb, Hashtable opt);
		void invite_info( Hashtable obj, RequestCallback cb, Hashtable opt);
		void list( Hashtable obj, RequestCallback cb, Hashtable opt);
		void remove( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IInfoActions
	{
		void ping( Hashtable obj, RequestCallback cb, Hashtable opt);
		void user( Hashtable obj, RequestCallback cb, Hashtable opt);
		void poll( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IItemsActions
	{
		void equip( Hashtable obj, RequestCallback cb, Hashtable opt);
		void list( Hashtable obj, RequestCallback cb, Hashtable opt);
		void sell( Hashtable obj, RequestCallback cb, Hashtable opt);
		void _set( Hashtable obj, RequestCallback cb, Hashtable opt);
		void unequip( Hashtable obj, RequestCallback cb, Hashtable opt);
		void use( Hashtable obj, RequestCallback cb, Hashtable opt);
		void view( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface ILeaderboardsActions
	{
		void list( Hashtable obj, RequestCallback cb, Hashtable opt);
		void view( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IMailActions
	{
		void accept( Hashtable obj, RequestCallback cb, Hashtable opt);
		void send( Hashtable obj, RequestCallback cb, Hashtable opt);
		void what_can_i_accept( Hashtable obj, RequestCallback cb, Hashtable opt);
		void what_can_i_send( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IShopActions
	{
		void list( Hashtable obj, RequestCallback cb, Hashtable opt);
		void buy( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IScriptsActions
	{
		void run( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface ITasksActions
	{
		void list( Hashtable obj, RequestCallback cb, Hashtable opt);
		void start( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IUserActions
	{
		void achievements( Hashtable obj, RequestCallback cb, Hashtable opt);
		void create( Hashtable obj, RequestCallback cb, Hashtable opt);
		void login( Hashtable obj, RequestCallback cb, Hashtable opt);
		void login_facebook_oauth( Hashtable obj, RequestCallback cb, Hashtable opt);
		void logout( Hashtable obj, RequestCallback cb, Hashtable opt);
		void netdrive_save( Hashtable obj, RequestCallback cb, Hashtable opt);
		void netdrive_fetch( Hashtable obj, RequestCallback cb, Hashtable opt);
		void _set( Hashtable obj, RequestCallback cb, Hashtable opt);
		void view( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

	public interface IUrbanairshipActions
	{
		void ios_register( Hashtable obj, RequestCallback cb, Hashtable opt);
		void push( Hashtable obj, RequestCallback cb, Hashtable opt);
	}

}

