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
	public abstract IAdminActions admin { get; }
	public abstract IAppstoreActions appstore { get; }
	public abstract IChromeWebStoreActions chrome_web_store { get; }
	public abstract IFacebookActions facebook { get; }
	public abstract IFriendsActions friends { get; }
	public abstract IGoogleActions google { get; }
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
		void create_player( Roar.WebObjects.Admin.CreatePlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.CreatePlayerResponse> cb);
		void delete_player( Roar.WebObjects.Admin.DeletePlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.DeletePlayerResponse> cb);
		void increment_stat( Roar.WebObjects.Admin.IncrementStatArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.IncrementStatResponse> cb);
		void login_user( Roar.WebObjects.Admin.LoginUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.LoginUserResponse> cb);
		void _set( Roar.WebObjects.Admin.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.SetResponse> cb);
		void set_custom( Roar.WebObjects.Admin.SetCustomArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.SetCustomResponse> cb);
		void view_player( Roar.WebObjects.Admin.ViewPlayerArguments args, ZWebAPI.Callback<Roar.WebObjects.Admin.ViewPlayerResponse> cb);
	}

	public interface IAppstoreActions
	{
		void buy( Roar.WebObjects.Appstore.BuyArguments args, ZWebAPI.Callback<Roar.WebObjects.Appstore.BuyResponse> cb);
		void shop_list( Roar.WebObjects.Appstore.ShopListArguments args, ZWebAPI.Callback<Roar.WebObjects.Appstore.ShopListResponse> cb);
	}

	public interface IChromeWebStoreActions
	{
		void list( Roar.WebObjects.ChromeWebStore.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.ChromeWebStore.ListResponse> cb);
	}

	public interface IFacebookActions
	{
		void bind_oauth( Roar.WebObjects.Facebook.BindOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.BindOauthResponse> cb);
		void bind_signed( Roar.WebObjects.Facebook.BindSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.BindSignedResponse> cb);
		void create_oauth( Roar.WebObjects.Facebook.CreateOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.CreateOauthResponse> cb);
		void create_signed( Roar.WebObjects.Facebook.CreateSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.CreateSignedResponse> cb);
		void fetch_oauth_token( Roar.WebObjects.Facebook.FetchOauthTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.FetchOauthTokenResponse> cb);
		void friends( Roar.WebObjects.Facebook.FriendsArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.FriendsResponse> cb);
		void login_oauth( Roar.WebObjects.Facebook.LoginOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.LoginOauthResponse> cb);
		void login_signed( Roar.WebObjects.Facebook.LoginSignedArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.LoginSignedResponse> cb);
		void shop_list( Roar.WebObjects.Facebook.ShopListArguments args, ZWebAPI.Callback<Roar.WebObjects.Facebook.ShopListResponse> cb);
	}

	public interface IFriendsActions
	{
		void accept( Roar.WebObjects.Friends.AcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.AcceptResponse> cb);
		void decline( Roar.WebObjects.Friends.DeclineArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.DeclineResponse> cb);
		void invite( Roar.WebObjects.Friends.InviteArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.InviteResponse> cb);
		void invite_info( Roar.WebObjects.Friends.InviteInfoArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.InviteInfoResponse> cb);
		void list( Roar.WebObjects.Friends.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.ListResponse> cb);
		void remove( Roar.WebObjects.Friends.RemoveArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.RemoveResponse> cb);
		void list_invites( Roar.WebObjects.Friends.ListInvitesArguments args, ZWebAPI.Callback<Roar.WebObjects.Friends.ListInvitesResponse> cb);
	}

	public interface IGoogleActions
	{
		void bind_user( Roar.WebObjects.Google.BindUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.BindUserResponse> cb);
		void bind_user_token( Roar.WebObjects.Google.BindUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.BindUserTokenResponse> cb);
		void create_user( Roar.WebObjects.Google.CreateUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.CreateUserResponse> cb);
		void create_user_token( Roar.WebObjects.Google.CreateUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.CreateUserTokenResponse> cb);
		void friends( Roar.WebObjects.Google.FriendsArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.FriendsResponse> cb);
		void login_or_create_user( Roar.WebObjects.Google.LoginOrCreateUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginOrCreateUserResponse> cb);
		void login_user( Roar.WebObjects.Google.LoginUserArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginUserResponse> cb);
		void login_user_token( Roar.WebObjects.Google.LoginUserTokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.LoginUserTokenResponse> cb);
		void token( Roar.WebObjects.Google.TokenArguments args, ZWebAPI.Callback<Roar.WebObjects.Google.TokenResponse> cb);
	}

	public interface IInfoActions
	{
		void get_bulk_player_info( Roar.WebObjects.Info.GetBulkPlayerInfoArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.GetBulkPlayerInfoResponse> cb);
		void ping( Roar.WebObjects.Info.PingArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.PingResponse> cb);
		void user( Roar.WebObjects.Info.UserArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.UserResponse> cb);
		void poll( Roar.WebObjects.Info.PollArguments args, ZWebAPI.Callback<Roar.WebObjects.Info.PollResponse> cb);
	}

	public interface IItemsActions
	{
		void equip( Roar.WebObjects.Items.EquipArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.EquipResponse> cb);
		void list( Roar.WebObjects.Items.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ListResponse> cb);
		void sell( Roar.WebObjects.Items.SellArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.SellResponse> cb);
		void _set( Roar.WebObjects.Items.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.SetResponse> cb);
		void unequip( Roar.WebObjects.Items.UnequipArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.UnequipResponse> cb);
		void use( Roar.WebObjects.Items.UseArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.UseResponse> cb);
		void view( Roar.WebObjects.Items.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ViewResponse> cb);
		void view_all( Roar.WebObjects.Items.ViewAllArguments args, ZWebAPI.Callback<Roar.WebObjects.Items.ViewAllResponse> cb);
	}

	public interface ILeaderboardsActions
	{
		void list( Roar.WebObjects.Leaderboards.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ListResponse> cb);
		void view( Roar.WebObjects.Leaderboards.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.Leaderboards.ViewResponse> cb);
	}

	public interface IMailActions
	{
		void accept( Roar.WebObjects.Mail.AcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.AcceptResponse> cb);
		void send( Roar.WebObjects.Mail.SendArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.SendResponse> cb);
		void what_can_i_accept( Roar.WebObjects.Mail.WhatCanIAcceptArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.WhatCanIAcceptResponse> cb);
		void what_can_i_send( Roar.WebObjects.Mail.WhatCanISendArguments args, ZWebAPI.Callback<Roar.WebObjects.Mail.WhatCanISendResponse> cb);
	}

	public interface IShopActions
	{
		void list( Roar.WebObjects.Shop.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Shop.ListResponse> cb);
		void buy( Roar.WebObjects.Shop.BuyArguments args, ZWebAPI.Callback<Roar.WebObjects.Shop.BuyResponse> cb);
	}

	public interface IScriptsActions
	{
		void run( Roar.WebObjects.Scripts.RunArguments args, ZWebAPI.Callback<Roar.WebObjects.Scripts.RunResponse> cb);
		void run_admin( Roar.WebObjects.Scripts.RunAdminArguments args, ZWebAPI.Callback<Roar.WebObjects.Scripts.RunAdminResponse> cb);
	}

	public interface ITasksActions
	{
		void list( Roar.WebObjects.Tasks.ListArguments args, ZWebAPI.Callback<Roar.WebObjects.Tasks.ListResponse> cb);
		void start( Roar.WebObjects.Tasks.StartArguments args, ZWebAPI.Callback<Roar.WebObjects.Tasks.StartResponse> cb);
	}

	public interface IUserActions
	{
		void achievements( Roar.WebObjects.User.AchievementsArguments args, ZWebAPI.Callback<Roar.WebObjects.User.AchievementsResponse> cb);
		void change_name( Roar.WebObjects.User.ChangeNameArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ChangeNameResponse> cb);
		void change_password( Roar.WebObjects.User.ChangePasswordArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ChangePasswordResponse> cb);
		void create( Roar.WebObjects.User.CreateArguments args, ZWebAPI.Callback<Roar.WebObjects.User.CreateResponse> cb);
		void login( Roar.WebObjects.User.LoginArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LoginResponse> cb);
		void login_facebook_oauth( Roar.WebObjects.User.LoginFacebookOauthArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LoginFacebookOauthResponse> cb);
		void logout( Roar.WebObjects.User.LogoutArguments args, ZWebAPI.Callback<Roar.WebObjects.User.LogoutResponse> cb);
		void netdrive_save( Roar.WebObjects.User.NetdriveSaveArguments args, ZWebAPI.Callback<Roar.WebObjects.User.NetdriveSaveResponse> cb);
		void netdrive_fetch( Roar.WebObjects.User.NetdriveFetchArguments args, ZWebAPI.Callback<Roar.WebObjects.User.NetdriveFetchResponse> cb);
		void _set( Roar.WebObjects.User.SetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.SetResponse> cb);
		void view( Roar.WebObjects.User.ViewArguments args, ZWebAPI.Callback<Roar.WebObjects.User.ViewResponse> cb);
		void private_set( Roar.WebObjects.User.PrivateSetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.PrivateSetResponse> cb);
		void private_get( Roar.WebObjects.User.PrivateGetArguments args, ZWebAPI.Callback<Roar.WebObjects.User.PrivateGetResponse> cb);
	}

	public interface IUrbanairshipActions
	{
		void ios_register( Roar.WebObjects.Urbanairship.IosRegisterArguments args, ZWebAPI.Callback<Roar.WebObjects.Urbanairship.IosRegisterResponse> cb);
		void push( Roar.WebObjects.Urbanairship.PushArguments args, ZWebAPI.Callback<Roar.WebObjects.Urbanairship.PushResponse> cb);
	}

}

