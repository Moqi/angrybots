using System;
using System.Collections;
using System.Collections.Generic;


namespace Roar.WebObjects
{

	//Namespace for typesafe arguments and responses to Roars admin/foo calls.
	namespace Admin
	{

		// Arguments to admin/create_player
		public class CreatePlayerArguments
		{
			public string admin_token;
			public string name;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["name"] = name;
				return retval;
			}
		}
		
		// Response from admin/create_player
		public class CreatePlayerResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to admin/delete_player
		public class DeletePlayerArguments
		{
			public string admin_token;
			public string player_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["player_id"] = player_id;
				return retval;
			}
		}
		
		// Response from admin/delete_player
		public class DeletePlayerResponse
		{

		}

		// Arguments to admin/increment_stat
		public class IncrementStatArguments
		{
			public string admin_token;
			public string id;
			public string stat;
			public string amount;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["id"] = id;
				retval["stat"] = stat;
				retval["amount"] = amount;
				return retval;
			}
		}
		
		// Response from admin/increment_stat
		public class IncrementStatResponse
		{

		}

		// Arguments to admin/login_user
		public class LoginUserArguments
		{
			public string admin_token;
			public string name; // name is mutually exclusive with id 
			public string id; // id is mutually exclusive with name 

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				if( name!=null )
				{
				retval["name"] = name;
				}
				if( id!=null )
				{
				retval["id"] = id;
				}
				return retval;
			}
		}
		
		// Response from admin/login_user
		public class LoginUserResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to admin/set
		public class SetArguments
		{
			public string admin_token;
			public string id;
			public string stat;
			public string amount;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["id"] = id;
				retval["stat"] = stat;
				retval["amount"] = amount;
				return retval;
			}
		}
		
		// Response from admin/set
		public class SetResponse
		{

		}

		// Arguments to admin/set_custom
		public class SetCustomArguments
		{
			public string admin_token;
			public string id;
			public string property_ikey;
			public string value;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["id"] = id;
				retval["property_ikey"] = property_ikey;
				retval["value"] = value;
				return retval;
			}
		}
		
		// Response from admin/set_custom
		public class SetCustomResponse
		{

		}

		// Arguments to admin/view_player
		public class ViewPlayerArguments
		{
			public string admin_token;
			public string id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["id"] = id;
				return retval;
			}
		}
		
		// Response from admin/view_player
		public class ViewPlayerResponse
		{
			public Roar.DomainObjects.Player player;
			public List<DomainObjects.InventoryItem> items;

		}

	}

	//Namespace for typesafe arguments and responses to Roars appstore/foo calls.
	namespace Appstore
	{

		// Arguments to appstore/buy
		public class BuyArguments
		{
			public string receipt;
			public bool sandbox;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["receipt"] = receipt;
				retval["sandbox"] = Convert.ToString(sandbox);
				return retval;
			}
		}
		
		// Response from appstore/buy
		public class BuyResponse
		{

		}

		// Arguments to appstore/shop_list
		public class ShopListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from appstore/shop_list
		public class ShopListResponse
		{
			public List<DomainObjects.AppstoreShopEntry> shop_list;

		}

	}

	//Namespace for typesafe arguments and responses to Roars chrome_web_store/foo calls.
	namespace ChromeWebStore
	{

		// Arguments to chrome_web_store/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from chrome_web_store/list
		public class ListResponse
		{
			public List<DomainObjects.ChromeWebStoreShopEntry> shop_items;

		}

	}

	//Namespace for typesafe arguments and responses to Roars facebook/foo calls.
	namespace Facebook
	{

		// Arguments to facebook/bind_oauth
		public class BindOauthArguments
		{
			public string oauth_token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["oauth_token"] = oauth_token;
				return retval;
			}
		}
		
		// Response from facebook/bind_oauth
		public class BindOauthResponse
		{

		}

		// Arguments to facebook/bind_signed
		public class BindSignedArguments
		{
			public string signed_request;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["signed_request"] = signed_request;
				return retval;
			}
		}
		
		// Response from facebook/bind_signed
		public class BindSignedResponse
		{

		}

		// Arguments to facebook/create_oauth
		public class CreateOauthArguments
		{
			public string oauth_token;
			public string name;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["oauth_token"] = oauth_token;
				retval["name"] = name;
				return retval;
			}
		}
		
		// Response from facebook/create_oauth
		public class CreateOauthResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to facebook/create_signed
		public class CreateSignedArguments
		{
			public string signed_request;
			public string name;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["signed_request"] = signed_request;
				retval["name"] = name;
				return retval;
			}
		}
		
		// Response from facebook/create_signed
		public class CreateSignedResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to facebook/fetch_oauth_token
		public class FetchOauthTokenArguments
		{
			public string code;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["code"] = code;
				return retval;
			}
		}
		
		// Response from facebook/fetch_oauth_token
		public class FetchOauthTokenResponse
		{
			public string oauth_token;

		}

		// Arguments to facebook/friends
		public class FriendsArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from facebook/friends
		public class FriendsResponse
		{
			public List<DomainObjects.FacebookFriendInfo> facebook_friends;

		}

		// Arguments to facebook/login_oauth
		public class LoginOauthArguments
		{
			public string oauth_token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["oauth_token"] = oauth_token;
				return retval;
			}
		}
		
		// Response from facebook/login_oauth
		public class LoginOauthResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to facebook/login_signed
		public class LoginSignedArguments
		{
			public string signed_request;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["signed_request"] = signed_request;
				return retval;
			}
		}
		
		// Response from facebook/login_signed
		public class LoginSignedResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to facebook/shop_list
		public class ShopListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from facebook/shop_list
		public class ShopListResponse
		{
			public List<DomainObjects.FacebookShopEntry> shop_list;

		}

	}

	//Namespace for typesafe arguments and responses to Roars friends/foo calls.
	namespace Friends
	{

		// Arguments to friends/accept
		public class AcceptArguments
		{
			public string friends_id;
			public string invite_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["friends_id"] = friends_id;
				retval["invite_id"] = invite_id;
				return retval;
			}
		}
		
		// Response from friends/accept
		public class AcceptResponse
		{

		}

		// Arguments to friends/decline
		public class DeclineArguments
		{
			public string invite_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["invite_id"] = invite_id;
				return retval;
			}
		}
		
		// Response from friends/decline
		public class DeclineResponse
		{

		}

		// Arguments to friends/invite
		public class InviteArguments
		{
			public string friend_id;
			public string player_id; // TODO: This argument may not be needed 

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["friend_id"] = friend_id;
				retval["player_id"] = player_id;
				return retval;
			}
		}
		
		// Response from friends/invite
		public class InviteResponse
		{

		}

		// Arguments to friends/invite_info
		public class InviteInfoArguments
		{
			public string invite_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["invite_id"] = invite_id;
				return retval;
			}
		}
		
		// Response from friends/invite_info
		public class InviteInfoResponse
		{
			public DomainObjects.FriendInviteInfo info;

		}

		// Arguments to friends/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from friends/list
		public class ListResponse
		{
			public List<DomainObjects.Friend> friends;

		}

		// Arguments to friends/remove
		public class RemoveArguments
		{
			public string friend_id;
			public string player_id; // TODO: This argument may not be needed 

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["friend_id"] = friend_id;
				retval["player_id"] = player_id;
				return retval;
			}
		}
		
		// Response from friends/remove
		public class RemoveResponse
		{

		}

		// Arguments to friends/list_invites
		public class ListInvitesArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from friends/list_invites
		public class ListInvitesResponse
		{
			public List<DomainObjects.FriendInvite> invites;

		}

	}

	//Namespace for typesafe arguments and responses to Roars google/foo calls.
	namespace Google
	{

		// Arguments to google/bind_user
		public class BindUserArguments
		{
			public string code;
			public string google_client_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["code"] = code;
				retval["google_client_id"] = google_client_id;
				return retval;
			}
		}
		
		// Response from google/bind_user
		public class BindUserResponse
		{

		}

		// Arguments to google/bind_user_token
		public class BindUserTokenArguments
		{
			public string token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["token"] = token;
				return retval;
			}
		}
		
		// Response from google/bind_user_token
		public class BindUserTokenResponse
		{

		}

		// Arguments to google/create_user
		public class CreateUserArguments
		{
			public string code;
			public string name;
			public string google_client_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["code"] = code;
				retval["name"] = name;
				retval["google_client_id"] = google_client_id;
				return retval;
			}
		}
		
		// Response from google/create_user
		public class CreateUserResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to google/create_user_token
		public class CreateUserTokenArguments
		{
			public string token;
			public string name;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["token"] = token;
				retval["name"] = name;
				return retval;
			}
		}
		
		// Response from google/create_user_token
		public class CreateUserTokenResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to google/friends
		public class FriendsArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from google/friends
		public class FriendsResponse
		{
			public List<DomainObjects.GoogleFriend> friends;

		}

		// Arguments to google/login_or_create_user
		public class LoginOrCreateUserArguments
		{
			public string code;
			public string google_client_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["code"] = code;
				retval["google_client_id"] = google_client_id;
				return retval;
			}
		}
		
		// Response from google/login_or_create_user
		public class LoginOrCreateUserResponse
		{
			public string auth_token;
			public string player_id;
			public string mode;

		}

		// Arguments to google/login_user
		public class LoginUserArguments
		{
			public string code;
			public string google_client_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["code"] = code;
				retval["google_client_id"] = google_client_id;
				return retval;
			}
		}
		
		// Response from google/login_user
		public class LoginUserResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to google/login_user_token
		public class LoginUserTokenArguments
		{
			public string token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["token"] = token;
				return retval;
			}
		}
		
		// Response from google/login_user_token
		public class LoginUserTokenResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to google/token
		public class TokenArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from google/token
		public class TokenResponse
		{
			public string token;

		}

	}

	//Namespace for typesafe arguments and responses to Roars info/foo calls.
	namespace Info
	{

		// Arguments to info/get_bulk_player_info
		public class GetBulkPlayerInfoArguments
		{
			public string admin_token;
			public string player_ids; // This type is not dev-friendly 
			public string stats; // This type is not dev-friendly 
			public string properties; // This type is not dev-friendly 

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["player_ids"] = player_ids;
				retval["stats"] = stats;
				retval["properties"] = properties;
				return retval;
			}
		}
		
		// Response from info/get_bulk_player_info
		public class GetBulkPlayerInfoResponse
		{
			public Dictionary<string, Roar.DomainObjects.BulkPlayerInfo> players;

		}

		// Arguments to info/ping
		public class PingArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from info/ping
		public class PingResponse
		{
			public string text;

		}

		// Arguments to info/user
		public class UserArguments
		{
			public string id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["id"] = id;
				return retval;
			}
		}
		
		// Response from info/user
		public class UserResponse
		{
			public Roar.DomainObjects.Player player;

		}

		// Arguments to info/poll
		public class PollArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from info/poll
		public class PollResponse
		{

		}

	}

	//Namespace for typesafe arguments and responses to Roars items/foo calls.
	namespace Items
	{

		// Arguments to items/equip
		public class EquipArguments
		{
			public string item_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["item_id"] = item_id;
				return retval;
			}
		}
		
		// Response from items/equip
		public class EquipResponse
		{

		}

		// Arguments to items/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from items/list
		public class ListResponse
		{
			public List<DomainObjects.InventoryItem> items;

		}

		// Arguments to items/sell
		public class SellArguments
		{
			public string item_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["item_id"] = item_id;
				return retval;
			}
		}
		
		// Response from items/sell
		public class SellResponse
		{
			public DomainObjects.InventoryItem item;
			public DomainObjects.ModifierResult effect;

		}

		// Arguments to items/set
		public class SetArguments
		{
			public string item_id;
			public string property_ikey;
			public string value;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["item_id"] = item_id;
				retval["property_ikey"] = property_ikey;
				retval["value"] = value;
				return retval;
			}
		}
		
		// Response from items/set
		public class SetResponse
		{

		}

		// Arguments to items/unequip
		public class UnequipArguments
		{
			public string item_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["item_id"] = item_id;
				return retval;
			}
		}
		
		// Response from items/unequip
		public class UnequipResponse
		{

		}

		// Arguments to items/use
		public class UseArguments
		{
			public string item_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["item_id"] = item_id;
				return retval;
			}
		}
		
		// Response from items/use
		public class UseResponse
		{

		}

		// Arguments to items/view
		public class ViewArguments
		{
			public string item_key; // item_key is mutually exclusive with item_keys 
			public string item_keys; // item_keys is mutually exclusive with item_key 

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				if( item_key!=null )
				{
				retval["item_key"] = item_key;
				}
				if( item_keys!=null )
				{
				retval["item_keys"] = item_keys;
				}
				return retval;
			}
		}
		
		// Response from items/view
		public class ViewResponse
		{
			public List<DomainObjects.ItemArchetype> items;

		}

		// Arguments to items/view_all
		public class ViewAllArguments
		{
			public string tags;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["tags"] = tags;
				return retval;
			}
		}
		
		// Response from items/view_all
		public class ViewAllResponse
		{
			public List<DomainObjects.ItemArchetype> items;

		}

	}

	//Namespace for typesafe arguments and responses to Roars leaderboards/foo calls.
	namespace Leaderboards
	{

		// Arguments to leaderboards/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from leaderboards/list
		public class ListResponse
		{
			public List<DomainObjects.LeaderboardInfo> boards;

		}

		// Arguments to leaderboards/view
		public class ViewArguments
		{
			public string board_id;
			public int? num_results;
			public int? offset;
			public int? page;
			public string player_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["board_id"] = board_id;
				if( num_results!=null )
				{
				retval["num_results"] = Convert.ToString(num_results.Value);
				}
				if( offset!=null )
				{
				retval["offset"] = Convert.ToString(offset.Value);
				}
				if( page!=null )
				{
				retval["page"] = Convert.ToString(page.Value);
				}
				if( player_id!=null )
				{
				retval["player_id"] = player_id;
				}
				return retval;
			}
		}
		
		// Response from leaderboards/view
		public class ViewResponse
		{
			public DomainObjects.LeaderboardData leaderboard_data;

		}

	}

	//Namespace for typesafe arguments and responses to Roars mail/foo calls.
	namespace Mail
	{

		// Arguments to mail/accept
		public class AcceptArguments
		{
			public string mail_id;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["mail_id"] = mail_id;
				return retval;
			}
		}
		
		// Response from mail/accept
		public class AcceptResponse
		{

		}

		// Arguments to mail/send
		public class SendArguments
		{
			public string recipient_id;
			public string mailable_id;
			public string message;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["recipient_id"] = recipient_id;
				retval["mailable_id"] = mailable_id;
				retval["message"] = message;
				return retval;
			}
		}
		
		// Response from mail/send
		public class SendResponse
		{

		}

		// Arguments to mail/what_can_i_accept
		public class WhatCanIAcceptArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from mail/what_can_i_accept
		public class WhatCanIAcceptResponse
		{
			public List<DomainObjects.MailPackage> packages;

		}

		// Arguments to mail/what_can_i_send
		public class WhatCanISendArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from mail/what_can_i_send
		public class WhatCanISendResponse
		{
			public List<DomainObjects.Mailable> mailables;

		}

	}

	//Namespace for typesafe arguments and responses to Roars shop/foo calls.
	namespace Shop
	{

		// Arguments to shop/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from shop/list
		public class ListResponse
		{
			public List<DomainObjects.ShopEntry> shop_entries;

		}

		// Arguments to shop/buy
		public class BuyArguments
		{
			public string shop_item_ikey;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["shop_item_ikey"] = shop_item_ikey;
				return retval;
			}
		}
		
		// Response from shop/buy
		public class BuyResponse
		{
			public DomainObjects.ModifierResult buy_response;

		}

	}

	//Namespace for typesafe arguments and responses to Roars scripts/foo calls.
	namespace Scripts
	{

		// Arguments to scripts/run
		public class RunArguments
		{
			public string script;
			public string args;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["script"] = script;
				retval["args"] = args;
				return retval;
			}
		}
		
		// Response from scripts/run
		public class RunResponse
		{
			public DomainObjects.ScriptRunResult result;

		}

		// Arguments to scripts/run_admin
		public class RunAdminArguments
		{
			public string admin_token;
			public string script;
			public string args;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["admin_token"] = admin_token;
				retval["script"] = script;
				retval["args"] = args;
				return retval;
			}
		}
		
		// Response from scripts/run_admin
		public class RunAdminResponse
		{
			public DomainObjects.ScriptRunResult result;

		}

	}

	//Namespace for typesafe arguments and responses to Roars tasks/foo calls.
	namespace Tasks
	{

		// Arguments to tasks/list
		public class ListArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from tasks/list
		public class ListResponse
		{
			public List<DomainObjects.Task> tasks;

		}

		// Arguments to tasks/start
		public class StartArguments
		{
			public string task_ikey;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["task_ikey"] = task_ikey;
				return retval;
			}
		}
		
		// Response from tasks/start
		public class StartResponse
		{

		}

	}

	//Namespace for typesafe arguments and responses to Roars user/foo calls.
	namespace User
	{

		// Arguments to user/achievements
		public class AchievementsArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from user/achievements
		public class AchievementsResponse
		{
			public List<DomainObjects.Achievement> achievements;

		}

		// Arguments to user/change_name
		public class ChangeNameArguments
		{
			public string name;
			public string password;
			public string new_name;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["name"] = name;
				retval["password"] = password;
				retval["new_name"] = new_name;
				return retval;
			}
		}
		
		// Response from user/change_name
		public class ChangeNameResponse
		{

		}

		// Arguments to user/change_password
		public class ChangePasswordArguments
		{
			public string name;
			public string old_password;
			public string new_password;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["name"] = name;
				retval["old_password"] = old_password;
				retval["new_password"] = new_password;
				return retval;
			}
		}
		
		// Response from user/change_password
		public class ChangePasswordResponse
		{

		}

		// Arguments to user/create
		public class CreateArguments
		{
			public string name;
			public string hash;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["name"] = name;
				retval["hash"] = hash;
				return retval;
			}
		}
		
		// Response from user/create
		public class CreateResponse
		{

		}

		// Arguments to user/login
		public class LoginArguments
		{
			public string name;
			public string hash;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["name"] = name;
				retval["hash"] = hash;
				return retval;
			}
		}
		
		// Response from user/login
		public class LoginResponse
		{
			public string auth_token;
			public string player_id;

		}

		// Arguments to user/login_facebook_oauth
		public class LoginFacebookOauthArguments
		{
			public string oauth_token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["oauth_token"] = oauth_token;
				return retval;
			}
		}
		
		// Response from user/login_facebook_oauth
		public class LoginFacebookOauthResponse
		{

		}

		// Arguments to user/logout
		public class LogoutArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from user/logout
		public class LogoutResponse
		{

		}

		// Arguments to user/netdrive_save
		public class NetdriveSaveArguments
		{
			public string ikey;
			public string data;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["ikey"] = ikey;
				retval["data"] = data;
				return retval;
			}
		}
		
		// Response from user/netdrive_save
		public class NetdriveSaveResponse
		{

		}

		// Arguments to user/netdrive_fetch
		public class NetdriveFetchArguments
		{
			public string ikey;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["ikey"] = ikey;
				return retval;
			}
		}
		
		// Response from user/netdrive_fetch
		public class NetdriveFetchResponse
		{
			public string ikey;
			public string data;

		}

		// Arguments to user/set
		public class SetArguments
		{
			public string ikey;
			public string value;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["ikey"] = ikey;
				retval["value"] = value;
				return retval;
			}
		}
		
		// Response from user/set
		public class SetResponse
		{

		}

		// Arguments to user/view
		public class ViewArguments
		{

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				return retval;
			}
		}
		
		// Response from user/view
		public class ViewResponse
		{
			public List<DomainObjects.PlayerAttribute> attributes;

		}

		// Arguments to user/private_set
		public class PrivateSetArguments
		{
			public string ikey;
			public string data;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["ikey"] = ikey;
				retval["data"] = data;
				return retval;
			}
		}
		
		// Response from user/private_set
		public class PrivateSetResponse
		{

		}

		// Arguments to user/private_get
		public class PrivateGetArguments
		{
			public string ikey;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["ikey"] = ikey;
				return retval;
			}
		}
		
		// Response from user/private_get
		public class PrivateGetResponse
		{
			public string ikey;
			public string data;

		}

	}

	//Namespace for typesafe arguments and responses to Roars urbanairship/foo calls.
	namespace Urbanairship
	{

		// Arguments to urbanairship/ios_register
		public class IosRegisterArguments
		{
			public string device_token;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["device_token"] = device_token;
				return retval;
			}
		}
		
		// Response from urbanairship/ios_register
		public class IosRegisterResponse
		{

		}

		// Arguments to urbanairship/push
		public class PushArguments
		{
			public string roar_id;
			public string message;

			public Hashtable ToHashtable()
			{
				Hashtable retval = new Hashtable();
				retval["roar_id"] = roar_id;
				retval["message"] = message;
				return retval;
			}
		}
		
		// Response from urbanairship/push
		public class PushResponse
		{

		}

	}

	
}

