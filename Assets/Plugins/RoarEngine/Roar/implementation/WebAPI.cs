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
using System.Collections.Generic;
using UnityEngine;

public class WebAPI : IWebAPI
{
	protected IRequestSender requestSender_;

	public WebAPI (IRequestSender requestSender)
	{
		requestSender_ = requestSender;

		admin_ = new AdminActions (requestSender);
		appstore_ = new AppstoreActions (requestSender);
		chrome_web_store_ = new ChromeWebStoreActions (requestSender);
		facebook_ = new FacebookActions (requestSender);
		friends_ = new FriendsActions (requestSender);
		google_ = new GoogleActions (requestSender);
		info_ = new InfoActions (requestSender);
		items_ = new ItemsActions (requestSender);
		leaderboards_ = new LeaderboardsActions (requestSender);
		mail_ = new MailActions (requestSender);
		shop_ = new ShopActions (requestSender);
		scripts_ = new ScriptsActions (requestSender);
		tasks_ = new TasksActions (requestSender);
		user_ = new UserActions (requestSender);
		urbanairship_ = new UrbanairshipActions (requestSender);
	}

	public override IAdminActions admin { get { return admin_; } }

	public AdminActions admin_;

	public override IAppstoreActions appstore { get { return appstore_; } }

	public AppstoreActions appstore_;

	public override IChromeWebStoreActions chrome_web_store { get { return chrome_web_store_; } }

	public ChromeWebStoreActions chrome_web_store_;

	public override IFacebookActions facebook { get { return facebook_; } }

	public FacebookActions facebook_;

	public override IFriendsActions friends { get { return friends_; } }

	public FriendsActions friends_;

	public override IGoogleActions google { get { return google_; } }

	public GoogleActions google_;

	public override IInfoActions info { get { return info_; } }

	public InfoActions info_;

	public override IItemsActions items { get { return items_; } }

	public ItemsActions items_;

	public override ILeaderboardsActions leaderboards { get { return leaderboards_; } }

	public LeaderboardsActions leaderboards_;

	public override IMailActions mail { get { return mail_; } }

	public MailActions mail_;

	public override IShopActions shop { get { return shop_; } }

	public ShopActions shop_;

	public override IScriptsActions scripts { get { return scripts_; } }

	public ScriptsActions scripts_;

	public override ITasksActions tasks { get { return tasks_; } }

	public TasksActions tasks_;

	public override IUserActions user { get { return user_; } }

	public UserActions user_;

	public override IUrbanairshipActions urbanairship { get { return urbanairship_; } }

	public UrbanairshipActions urbanairship_;



	public class APIBridge
	{
		protected IRequestSender api;

		public APIBridge (IRequestSender caller)
		{
			api = caller;
		}
	}

	public class AdminActions : APIBridge, IAdminActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.CreatePlayerResponse> create_player_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.DeletePlayerResponse> delete_player_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.IncrementStatResponse> increment_stat_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.LoginUserResponse> login_user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.SetResponse> set_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.SetCustomResponse> set_custom_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Admin.ViewPlayerResponse> view_player_response_parser;

		public AdminActions (IRequestSender caller) : base(caller)
		{
			create_player_response_parser = new Roar.DataConversion.Responses.Admin.CreatePlayer();
			delete_player_response_parser = new Roar.DataConversion.Responses.Admin.DeletePlayer();
			increment_stat_response_parser = new Roar.DataConversion.Responses.Admin.IncrementStat();
			login_user_response_parser = new Roar.DataConversion.Responses.Admin.LoginUser();
			set_response_parser = new Roar.DataConversion.Responses.Admin.Set();
			set_custom_response_parser = new Roar.DataConversion.Responses.Admin.SetCustom();
			view_player_response_parser = new Roar.DataConversion.Responses.Admin.ViewPlayer();

		}

		public void create_player( Roar.WebObjects.Admin.CreatePlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.CreatePlayerResponse> cb)
		{
			api.MakeCall ("admin/create_player", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.CreatePlayerResponse>(cb, create_player_response_parser), false);
		}

		public void delete_player( Roar.WebObjects.Admin.DeletePlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.DeletePlayerResponse> cb)
		{
			api.MakeCall ("admin/delete_player", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.DeletePlayerResponse>(cb, delete_player_response_parser), false);
		}

		public void increment_stat( Roar.WebObjects.Admin.IncrementStatArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.IncrementStatResponse> cb)
		{
			api.MakeCall ("admin/increment_stat", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.IncrementStatResponse>(cb, increment_stat_response_parser), false);
		}

		public void login_user( Roar.WebObjects.Admin.LoginUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.LoginUserResponse> cb)
		{
			api.MakeCall ("admin/login_user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.LoginUserResponse>(cb, login_user_response_parser), false);
		}

		public void _set( Roar.WebObjects.Admin.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.SetResponse> cb)
		{
			api.MakeCall ("admin/set", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.SetResponse>(cb, set_response_parser), false);
		}

		public void set_custom( Roar.WebObjects.Admin.SetCustomArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.SetCustomResponse> cb)
		{
			api.MakeCall ("admin/set_custom", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.SetCustomResponse>(cb, set_custom_response_parser), false);
		}

		public void view_player( Roar.WebObjects.Admin.ViewPlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.ViewPlayerResponse> cb)
		{
			api.MakeCall ("admin/view_player", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Admin.ViewPlayerResponse>(cb, view_player_response_parser), false);
		}

	}

	public class AppstoreActions : APIBridge, IAppstoreActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Appstore.BuyResponse> buy_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Appstore.ShopListResponse> shop_list_response_parser;

		public AppstoreActions (IRequestSender caller) : base(caller)
		{
			buy_response_parser = new Roar.DataConversion.Responses.Appstore.Buy();
			shop_list_response_parser = new Roar.DataConversion.Responses.Appstore.ShopList();

		}

		public void buy( Roar.WebObjects.Appstore.BuyArguments args, ZWebAPI.Callback<Roar.WebObjects.Appstore.BuyResponse> cb)
		{
			api.MakeCall ("appstore/buy", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Appstore.BuyResponse>(cb, buy_response_parser), true);
		}

		public void shop_list( Roar.WebObjects.Appstore.ShopListArguments args, ZWebAPI.Callback<Roar.WebObjects.Appstore.ShopListResponse> cb)
		{
			api.MakeCall ("appstore/shop_list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Appstore.ShopListResponse>(cb, shop_list_response_parser), true);
		}

	}

	public class ChromeWebStoreActions : APIBridge, IChromeWebStoreActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.ChromeWebStore.ListResponse> list_response_parser;

		public ChromeWebStoreActions (IRequestSender caller) : base(caller)
		{
			list_response_parser = new Roar.DataConversion.Responses.ChromeWebStore.List();

		}

		public void list( Roar.WebObjects.ChromeWebStore.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.ChromeWebStore.ListResponse> cb)
		{
			api.MakeCall ("chrome_web_store/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.ChromeWebStore.ListResponse>(cb, list_response_parser), true);
		}

	}

	public class FacebookActions : APIBridge, IFacebookActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.BindOauthResponse> bind_oauth_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.BindSignedResponse> bind_signed_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.CreateOauthResponse> create_oauth_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.CreateSignedResponse> create_signed_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.FetchOauthTokenResponse> fetch_oauth_token_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.FriendsResponse> friends_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.LoginOauthResponse> login_oauth_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.LoginSignedResponse> login_signed_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Facebook.ShopListResponse> shop_list_response_parser;

		public FacebookActions (IRequestSender caller) : base(caller)
		{
			bind_oauth_response_parser = new Roar.DataConversion.Responses.Facebook.BindOauth();
			bind_signed_response_parser = new Roar.DataConversion.Responses.Facebook.BindSigned();
			create_oauth_response_parser = new Roar.DataConversion.Responses.Facebook.CreateOauth();
			create_signed_response_parser = new Roar.DataConversion.Responses.Facebook.CreateSigned();
			fetch_oauth_token_response_parser = new Roar.DataConversion.Responses.Facebook.FetchOauthToken();
			friends_response_parser = new Roar.DataConversion.Responses.Facebook.Friends();
			login_oauth_response_parser = new Roar.DataConversion.Responses.Facebook.LoginOauth();
			login_signed_response_parser = new Roar.DataConversion.Responses.Facebook.LoginSigned();
			shop_list_response_parser = new Roar.DataConversion.Responses.Facebook.ShopList();

		}

		public void bind_oauth( Roar.WebObjects.Facebook.BindOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.BindOauthResponse> cb)
		{
			api.MakeCall ("facebook/bind_oauth", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.BindOauthResponse>(cb, bind_oauth_response_parser), true);
		}

		public void bind_signed( Roar.WebObjects.Facebook.BindSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.BindSignedResponse> cb)
		{
			api.MakeCall ("facebook/bind_signed", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.BindSignedResponse>(cb, bind_signed_response_parser), true);
		}

		public void create_oauth( Roar.WebObjects.Facebook.CreateOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.CreateOauthResponse> cb)
		{
			api.MakeCall ("facebook/create_oauth", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.CreateOauthResponse>(cb, create_oauth_response_parser), false);
		}

		public void create_signed( Roar.WebObjects.Facebook.CreateSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.CreateSignedResponse> cb)
		{
			api.MakeCall ("facebook/create_signed", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.CreateSignedResponse>(cb, create_signed_response_parser), false);
		}

		public void fetch_oauth_token( Roar.WebObjects.Facebook.FetchOauthTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.FetchOauthTokenResponse> cb)
		{
			api.MakeCall ("facebook/fetch_oauth_token", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.FetchOauthTokenResponse>(cb, fetch_oauth_token_response_parser), false);
		}

		public void friends( Roar.WebObjects.Facebook.FriendsArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.FriendsResponse> cb)
		{
			api.MakeCall ("facebook/friends", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.FriendsResponse>(cb, friends_response_parser), true);
		}

		public void login_oauth( Roar.WebObjects.Facebook.LoginOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.LoginOauthResponse> cb)
		{
			api.MakeCall ("facebook/login_oauth", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.LoginOauthResponse>(cb, login_oauth_response_parser), false);
		}

		public void login_signed( Roar.WebObjects.Facebook.LoginSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.LoginSignedResponse> cb)
		{
			api.MakeCall ("facebook/login_signed", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.LoginSignedResponse>(cb, login_signed_response_parser), false);
		}

		public void shop_list( Roar.WebObjects.Facebook.ShopListArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.ShopListResponse> cb)
		{
			api.MakeCall ("facebook/shop_list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Facebook.ShopListResponse>(cb, shop_list_response_parser), true);
		}

	}

	public class FriendsActions : APIBridge, IFriendsActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.AcceptResponse> accept_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.DeclineResponse> decline_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.InviteResponse> invite_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.InviteInfoResponse> invite_info_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.ListResponse> list_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.RemoveResponse> remove_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Friends.ListInvitesResponse> list_invites_response_parser;

		public FriendsActions (IRequestSender caller) : base(caller)
		{
			accept_response_parser = new Roar.DataConversion.Responses.Friends.Accept();
			decline_response_parser = new Roar.DataConversion.Responses.Friends.Decline();
			invite_response_parser = new Roar.DataConversion.Responses.Friends.Invite();
			invite_info_response_parser = new Roar.DataConversion.Responses.Friends.InviteInfo();
			list_response_parser = new Roar.DataConversion.Responses.Friends.List();
			remove_response_parser = new Roar.DataConversion.Responses.Friends.Remove();
			list_invites_response_parser = new Roar.DataConversion.Responses.Friends.ListInvites();

		}

		public void accept( Roar.WebObjects.Friends.AcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.AcceptResponse> cb)
		{
			api.MakeCall ("friends/accept", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.AcceptResponse>(cb, accept_response_parser), true);
		}

		public void decline( Roar.WebObjects.Friends.DeclineArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.DeclineResponse> cb)
		{
			api.MakeCall ("friends/decline", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.DeclineResponse>(cb, decline_response_parser), true);
		}

		public void invite( Roar.WebObjects.Friends.InviteArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.InviteResponse> cb)
		{
			api.MakeCall ("friends/invite", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.InviteResponse>(cb, invite_response_parser), true);
		}

		public void invite_info( Roar.WebObjects.Friends.InviteInfoArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.InviteInfoResponse> cb)
		{
			api.MakeCall ("friends/info", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.InviteInfoResponse>(cb, invite_info_response_parser), true);
		}

		public void list( Roar.WebObjects.Friends.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.ListResponse> cb)
		{
			api.MakeCall ("friends/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.ListResponse>(cb, list_response_parser), true);
		}

		public void remove( Roar.WebObjects.Friends.RemoveArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.RemoveResponse> cb)
		{
			api.MakeCall ("friends/remove", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.RemoveResponse>(cb, remove_response_parser), true);
		}

		public void list_invites( Roar.WebObjects.Friends.ListInvitesArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.ListInvitesResponse> cb)
		{
			api.MakeCall ("friends/list_invites", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Friends.ListInvitesResponse>(cb, list_invites_response_parser), true);
		}

	}

	public class GoogleActions : APIBridge, IGoogleActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.BindUserResponse> bind_user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.BindUserTokenResponse> bind_user_token_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.CreateUserResponse> create_user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.CreateUserTokenResponse> create_user_token_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.FriendsResponse> friends_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.LoginOrCreateUserResponse> login_or_create_user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.LoginUserResponse> login_user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.LoginUserTokenResponse> login_user_token_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Google.TokenResponse> token_response_parser;

		public GoogleActions (IRequestSender caller) : base(caller)
		{
			bind_user_response_parser = new Roar.DataConversion.Responses.Google.BindUser();
			bind_user_token_response_parser = new Roar.DataConversion.Responses.Google.BindUserToken();
			create_user_response_parser = new Roar.DataConversion.Responses.Google.CreateUser();
			create_user_token_response_parser = new Roar.DataConversion.Responses.Google.CreateUserToken();
			friends_response_parser = new Roar.DataConversion.Responses.Google.Friends();
			login_or_create_user_response_parser = new Roar.DataConversion.Responses.Google.LoginOrCreateUser();
			login_user_response_parser = new Roar.DataConversion.Responses.Google.LoginUser();
			login_user_token_response_parser = new Roar.DataConversion.Responses.Google.LoginUserToken();
			token_response_parser = new Roar.DataConversion.Responses.Google.Token();

		}

		public void bind_user( Roar.WebObjects.Google.BindUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.BindUserResponse> cb)
		{
			api.MakeCall ("google/bind_user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.BindUserResponse>(cb, bind_user_response_parser), true);
		}

		public void bind_user_token( Roar.WebObjects.Google.BindUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.BindUserTokenResponse> cb)
		{
			api.MakeCall ("google/bind_user_token", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.BindUserTokenResponse>(cb, bind_user_token_response_parser), true);
		}

		public void create_user( Roar.WebObjects.Google.CreateUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.CreateUserResponse> cb)
		{
			api.MakeCall ("google/create_user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.CreateUserResponse>(cb, create_user_response_parser), false);
		}

		public void create_user_token( Roar.WebObjects.Google.CreateUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.CreateUserTokenResponse> cb)
		{
			api.MakeCall ("google/create_user_token", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.CreateUserTokenResponse>(cb, create_user_token_response_parser), false);
		}

		public void friends( Roar.WebObjects.Google.FriendsArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.FriendsResponse> cb)
		{
			api.MakeCall ("google/friends", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.FriendsResponse>(cb, friends_response_parser), true);
		}

		public void login_or_create_user( Roar.WebObjects.Google.LoginOrCreateUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginOrCreateUserResponse> cb)
		{
			api.MakeCall ("google/login_or_create_user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.LoginOrCreateUserResponse>(cb, login_or_create_user_response_parser), false);
		}

		public void login_user( Roar.WebObjects.Google.LoginUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginUserResponse> cb)
		{
			api.MakeCall ("google/login_user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.LoginUserResponse>(cb, login_user_response_parser), false);
		}

		public void login_user_token( Roar.WebObjects.Google.LoginUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginUserTokenResponse> cb)
		{
			api.MakeCall ("google/login_user_token", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.LoginUserTokenResponse>(cb, login_user_token_response_parser), false);
		}

		public void token( Roar.WebObjects.Google.TokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.TokenResponse> cb)
		{
			api.MakeCall ("google/token", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Google.TokenResponse>(cb, token_response_parser), true);
		}

	}

	public class InfoActions : APIBridge, IInfoActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Info.GetBulkPlayerInfoResponse> get_bulk_player_info_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Info.PingResponse> ping_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Info.UserResponse> user_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Info.PollResponse> poll_response_parser;

		public InfoActions (IRequestSender caller) : base(caller)
		{
			get_bulk_player_info_response_parser = new Roar.DataConversion.Responses.Info.GetBulkPlayerInfo();
			ping_response_parser = new Roar.DataConversion.Responses.Info.Ping();
			user_response_parser = new Roar.DataConversion.Responses.Info.User();
			poll_response_parser = new Roar.DataConversion.Responses.Info.Poll();

		}

		public void get_bulk_player_info( Roar.WebObjects.Info.GetBulkPlayerInfoArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.GetBulkPlayerInfoResponse> cb)
		{
			api.MakeCall ("info/get_bulk_player_info", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Info.GetBulkPlayerInfoResponse>(cb, get_bulk_player_info_response_parser), false);
		}

		public void ping( Roar.WebObjects.Info.PingArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.PingResponse> cb)
		{
			api.MakeCall ("info/ping", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Info.PingResponse>(cb, ping_response_parser), false);
		}

		public void user( Roar.WebObjects.Info.UserArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.UserResponse> cb)
		{
			api.MakeCall ("info/user", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Info.UserResponse>(cb, user_response_parser), false);
		}

		public void poll( Roar.WebObjects.Info.PollArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.PollResponse> cb)
		{
			api.MakeCall ("info/poll", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Info.PollResponse>(cb, poll_response_parser), true);
		}

	}

	public class ItemsActions : APIBridge, IItemsActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.EquipResponse> equip_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.ListResponse> list_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.SellResponse> sell_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.SetResponse> set_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.UnequipResponse> unequip_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.UseResponse> use_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.ViewResponse> view_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Items.ViewAllResponse> view_all_response_parser;

		public ItemsActions (IRequestSender caller) : base(caller)
		{
			equip_response_parser = new Roar.DataConversion.Responses.Items.Equip();
			list_response_parser = new Roar.DataConversion.Responses.Items.List();
			sell_response_parser = new Roar.DataConversion.Responses.Items.Sell();
			set_response_parser = new Roar.DataConversion.Responses.Items.Set();
			unequip_response_parser = new Roar.DataConversion.Responses.Items.Unequip();
			use_response_parser = new Roar.DataConversion.Responses.Items.Use();
			view_response_parser = new Roar.DataConversion.Responses.Items.View();
			view_all_response_parser = new Roar.DataConversion.Responses.Items.ViewAll();

		}

		public void equip( Roar.WebObjects.Items.EquipArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.EquipResponse> cb)
		{
			api.MakeCall ("items/equip", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.EquipResponse>(cb, equip_response_parser), true);
		}

		public void list( Roar.WebObjects.Items.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ListResponse> cb)
		{
			api.MakeCall ("items/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.ListResponse>(cb, list_response_parser), true);
		}

		public void sell( Roar.WebObjects.Items.SellArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.SellResponse> cb)
		{
			api.MakeCall ("items/sell", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.SellResponse>(cb, sell_response_parser), true);
		}

		public void _set( Roar.WebObjects.Items.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.SetResponse> cb)
		{
			api.MakeCall ("items/set", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.SetResponse>(cb, set_response_parser), true);
		}

		public void unequip( Roar.WebObjects.Items.UnequipArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.UnequipResponse> cb)
		{
			api.MakeCall ("items/unequip", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.UnequipResponse>(cb, unequip_response_parser), true);
		}

		public void use( Roar.WebObjects.Items.UseArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.UseResponse> cb)
		{
			api.MakeCall ("items/use", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.UseResponse>(cb, use_response_parser), true);
		}

		public void view( Roar.WebObjects.Items.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ViewResponse> cb)
		{
			api.MakeCall ("items/view", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.ViewResponse>(cb, view_response_parser), false);
		}

		public void view_all( Roar.WebObjects.Items.ViewAllArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ViewAllResponse> cb)
		{
			api.MakeCall ("items/view_all", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Items.ViewAllResponse>(cb, view_all_response_parser), false);
		}

	}

	public class LeaderboardsActions : APIBridge, ILeaderboardsActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Leaderboards.ListResponse> list_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Leaderboards.ViewResponse> view_response_parser;

		public LeaderboardsActions (IRequestSender caller) : base(caller)
		{
			list_response_parser = new Roar.DataConversion.Responses.Leaderboards.List();
			view_response_parser = new Roar.DataConversion.Responses.Leaderboards.View();

		}

		public void list( Roar.WebObjects.Leaderboards.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ListResponse> cb)
		{
			api.MakeCall ("leaderboards/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Leaderboards.ListResponse>(cb, list_response_parser), false);
		}

		public void view( Roar.WebObjects.Leaderboards.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ViewResponse> cb)
		{
			api.MakeCall ("leaderboards/view", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Leaderboards.ViewResponse>(cb, view_response_parser), false);
		}

	}

	public class MailActions : APIBridge, IMailActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Mail.AcceptResponse> accept_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Mail.SendResponse> send_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Mail.WhatCanIAcceptResponse> what_can_i_accept_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Mail.WhatCanISendResponse> what_can_i_send_response_parser;

		public MailActions (IRequestSender caller) : base(caller)
		{
			accept_response_parser = new Roar.DataConversion.Responses.Mail.Accept();
			send_response_parser = new Roar.DataConversion.Responses.Mail.Send();
			what_can_i_accept_response_parser = new Roar.DataConversion.Responses.Mail.WhatCanIAccept();
			what_can_i_send_response_parser = new Roar.DataConversion.Responses.Mail.WhatCanISend();

		}

		public void accept( Roar.WebObjects.Mail.AcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.AcceptResponse> cb)
		{
			api.MakeCall ("mail/accept", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Mail.AcceptResponse>(cb, accept_response_parser), true);
		}

		public void send( Roar.WebObjects.Mail.SendArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.SendResponse> cb)
		{
			api.MakeCall ("mail/send", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Mail.SendResponse>(cb, send_response_parser), true);
		}

		public void what_can_i_accept( Roar.WebObjects.Mail.WhatCanIAcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.WhatCanIAcceptResponse> cb)
		{
			api.MakeCall ("mail/what_can_i_accept", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Mail.WhatCanIAcceptResponse>(cb, what_can_i_accept_response_parser), true);
		}

		public void what_can_i_send( Roar.WebObjects.Mail.WhatCanISendArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.WhatCanISendResponse> cb)
		{
			api.MakeCall ("mail/what_can_i_send", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Mail.WhatCanISendResponse>(cb, what_can_i_send_response_parser), true);
		}

	}

	public class ShopActions : APIBridge, IShopActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Shop.ListResponse> list_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Shop.BuyResponse> buy_response_parser;

		public ShopActions (IRequestSender caller) : base(caller)
		{
			list_response_parser = new Roar.DataConversion.Responses.Shop.List();
			buy_response_parser = new Roar.DataConversion.Responses.Shop.Buy();

		}

		public void list( Roar.WebObjects.Shop.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Shop.ListResponse> cb)
		{
			api.MakeCall ("shop/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Shop.ListResponse>(cb, list_response_parser), true);
		}

		public void buy( Roar.WebObjects.Shop.BuyArguments args, ZWebAPI.Callback<Roar.WebObjects.Shop.BuyResponse> cb)
		{
			api.MakeCall ("shop/buy", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Shop.BuyResponse>(cb, buy_response_parser), true);
		}

	}

	public class ScriptsActions : APIBridge, IScriptsActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Scripts.RunResponse> run_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Scripts.RunAdminResponse> run_admin_response_parser;

		public ScriptsActions (IRequestSender caller) : base(caller)
		{
			run_response_parser = new Roar.DataConversion.Responses.Scripts.Run();
			run_admin_response_parser = new Roar.DataConversion.Responses.Scripts.RunAdmin();

		}

		public void run( Roar.WebObjects.Scripts.RunArguments args, ZWebAPI.Callback<Roar.WebObjects.Scripts.RunResponse> cb)
		{
			api.MakeCall ("scripts/run", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Scripts.RunResponse>(cb, run_response_parser), true);
		}

		public void run_admin( Roar.WebObjects.Scripts.RunAdminArguments args, ZWebAPI.Callback<Roar.WebObjects.Scripts.RunAdminResponse> cb)
		{
			api.MakeCall ("scripts/run_admin", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Scripts.RunAdminResponse>(cb, run_admin_response_parser), false);
		}

	}

	public class TasksActions : APIBridge, ITasksActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Tasks.ListResponse> list_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Tasks.StartResponse> start_response_parser;

		public TasksActions (IRequestSender caller) : base(caller)
		{
			list_response_parser = new Roar.DataConversion.Responses.Tasks.List();
			start_response_parser = new Roar.DataConversion.Responses.Tasks.Start();

		}

		public void list( Roar.WebObjects.Tasks.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Tasks.ListResponse> cb)
		{
			api.MakeCall ("tasks/list", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Tasks.ListResponse>(cb, list_response_parser), true);
		}

		public void start( Roar.WebObjects.Tasks.StartArguments args, ZWebAPI.Callback<Roar.WebObjects.Tasks.StartResponse> cb)
		{
			api.MakeCall ("tasks/start", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Tasks.StartResponse>(cb, start_response_parser), true);
		}

	}

	public class UserActions : APIBridge, IUserActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.AchievementsResponse> achievements_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.ChangeNameResponse> change_name_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.ChangePasswordResponse> change_password_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.CreateResponse> create_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.LoginResponse> login_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.LoginFacebookOauthResponse> login_facebook_oauth_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.LogoutResponse> logout_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.NetdriveSaveResponse> netdrive_save_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.NetdriveFetchResponse> netdrive_fetch_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.SetResponse> set_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.ViewResponse> view_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.PrivateSetResponse> private_set_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.User.PrivateGetResponse> private_get_response_parser;

		public UserActions (IRequestSender caller) : base(caller)
		{
			achievements_response_parser = new Roar.DataConversion.Responses.User.Achievements();
			change_name_response_parser = new Roar.DataConversion.Responses.User.ChangeName();
			change_password_response_parser = new Roar.DataConversion.Responses.User.ChangePassword();
			create_response_parser = new Roar.DataConversion.Responses.User.Create();
			login_response_parser = new Roar.DataConversion.Responses.User.Login();
			login_facebook_oauth_response_parser = new Roar.DataConversion.Responses.User.LoginFacebookOauth();
			logout_response_parser = new Roar.DataConversion.Responses.User.Logout();
			netdrive_save_response_parser = new Roar.DataConversion.Responses.User.NetdriveSave();
			netdrive_fetch_response_parser = new Roar.DataConversion.Responses.User.NetdriveFetch();
			set_response_parser = new Roar.DataConversion.Responses.User.Set();
			view_response_parser = new Roar.DataConversion.Responses.User.View();
			private_set_response_parser = new Roar.DataConversion.Responses.User.PrivateSet();
			private_get_response_parser = new Roar.DataConversion.Responses.User.PrivateGet();

		}

		public void achievements( Roar.WebObjects.User.AchievementsArguments args, ZWebAPI.Callback<Roar.WebObjects.User.AchievementsResponse> cb)
		{
			api.MakeCall ("user/achievements", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.AchievementsResponse>(cb, achievements_response_parser), true);
		}

		public void change_name( Roar.WebObjects.User.ChangeNameArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ChangeNameResponse> cb)
		{
			api.MakeCall ("user/change_name", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.ChangeNameResponse>(cb, change_name_response_parser), true);
		}

		public void change_password( Roar.WebObjects.User.ChangePasswordArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ChangePasswordResponse> cb)
		{
			api.MakeCall ("user/change_password", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.ChangePasswordResponse>(cb, change_password_response_parser), true);
		}

		public void create( Roar.WebObjects.User.CreateArguments args, ZWebAPI.Callback<Roar.WebObjects.User.CreateResponse> cb)
		{
			api.MakeCall ("user/create", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.CreateResponse>(cb, create_response_parser), false);
		}

		public void login( Roar.WebObjects.User.LoginArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LoginResponse> cb)
		{
			api.MakeCall ("user/login", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.LoginResponse>(cb, login_response_parser), false);
		}

		public void login_facebook_oauth( Roar.WebObjects.User.LoginFacebookOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LoginFacebookOauthResponse> cb)
		{
			api.MakeCall ("facebook/login_oauth", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.LoginFacebookOauthResponse>(cb, login_facebook_oauth_response_parser), false);
		}

		public void logout( Roar.WebObjects.User.LogoutArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LogoutResponse> cb)
		{
			api.MakeCall ("user/logout", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.LogoutResponse>(cb, logout_response_parser), true);
		}

		public void netdrive_save( Roar.WebObjects.User.NetdriveSaveArguments args, ZWebAPI.Callback<Roar.WebObjects.User.NetdriveSaveResponse> cb)
		{
			api.MakeCall ("user/netdrive_set", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.NetdriveSaveResponse>(cb, netdrive_save_response_parser), true);
		}

		public void netdrive_fetch( Roar.WebObjects.User.NetdriveFetchArguments args, ZWebAPI.Callback<Roar.WebObjects.User.NetdriveFetchResponse> cb)
		{
			api.MakeCall ("user/netdrive_get", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.NetdriveFetchResponse>(cb, netdrive_fetch_response_parser), true);
		}

		public void _set( Roar.WebObjects.User.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.SetResponse> cb)
		{
			api.MakeCall ("user/set", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.SetResponse>(cb, set_response_parser), true);
		}

		public void view( Roar.WebObjects.User.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ViewResponse> cb)
		{
			api.MakeCall ("user/view", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.ViewResponse>(cb, view_response_parser), true);
		}

		public void private_set( Roar.WebObjects.User.PrivateSetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.PrivateSetResponse> cb)
		{
			api.MakeCall ("user/private_set", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.PrivateSetResponse>(cb, private_set_response_parser), true);
		}

		public void private_get( Roar.WebObjects.User.PrivateGetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.PrivateGetResponse> cb)
		{
			api.MakeCall ("user/private_get", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.User.PrivateGetResponse>(cb, private_get_response_parser), true);
		}

	}

	public class UrbanairshipActions : APIBridge, IUrbanairshipActions
	{
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Urbanairship.IosRegisterResponse> ios_register_response_parser;
		public Roar.DataConversion.IXmlToObject<Roar.WebObjects.Urbanairship.PushResponse> push_response_parser;

		public UrbanairshipActions (IRequestSender caller) : base(caller)
		{
			ios_register_response_parser = new Roar.DataConversion.Responses.Urbanairship.IosRegister();
			push_response_parser = new Roar.DataConversion.Responses.Urbanairship.Push();

		}

		public void ios_register( Roar.WebObjects.Urbanairship.IosRegisterArguments args, ZWebAPI.Callback<Roar.WebObjects.Urbanairship.IosRegisterResponse> cb)
		{
			api.MakeCall ("urbanairship/ios_register", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Urbanairship.IosRegisterResponse>(cb, ios_register_response_parser), true);
		}

		public void push( Roar.WebObjects.Urbanairship.PushArguments args, ZWebAPI.Callback<Roar.WebObjects.Urbanairship.PushResponse> cb)
		{
			api.MakeCall ("urbanairship/push", args.ToHashtable(), new CallbackBridge<Roar.WebObjects.Urbanairship.PushResponse>(cb, push_response_parser), true);
		}

	}


}

