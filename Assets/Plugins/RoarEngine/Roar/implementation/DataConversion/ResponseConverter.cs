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
using System.Linq;
using UnityEngine;
using Roar.DomainObjects;


namespace Roar.DataConversion.Responses
{
	namespace Admin
	{
		//Response from admin/create_player
		public class CreatePlayer : IXmlToObject< Roar.WebObjects.Admin.CreatePlayerResponse >
		{
			public Roar.WebObjects.Admin.CreatePlayerResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.CreatePlayerResponse retval = new Roar.WebObjects.Admin.CreatePlayerResponse();
				retval.auth_token = n.SelectSingleNode("./admin/create_user/auth_token").GetInnerTextOrDefault(null);
				retval.player_id = n.SelectSingleNode("./admin/create_user/player_id").GetInnerTextOrDefault(null);
				return retval;
			}
		}
		//Response from admin/delete_player
		public class DeletePlayer : IXmlToObject< Roar.WebObjects.Admin.DeletePlayerResponse >
		{
			public Roar.WebObjects.Admin.DeletePlayerResponse Build( System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.DeletePlayerResponse retval = new Roar.WebObjects.Admin.DeletePlayerResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from admin/increment_stat
		public class IncrementStat : IXmlToObject< Roar.WebObjects.Admin.IncrementStatResponse >
		{
			public Roar.WebObjects.Admin.IncrementStatResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.IncrementStatResponse retval = new Roar.WebObjects.Admin.IncrementStatResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from admin/login_user
		public class LoginUser : IXmlToObject< Roar.WebObjects.Admin.LoginUserResponse >
		{
			public Roar.WebObjects.Admin.LoginUserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.LoginUserResponse retval = new Roar.WebObjects.Admin.LoginUserResponse();
				retval.auth_token = n.SelectSingleNode("./admin/login_user/auth_token").GetInnerTextOrDefault(null);
				retval.player_id = n.SelectSingleNode("./admin/login_user/player_id").GetInnerTextOrDefault(null);
				return retval;
			}
		}
		//Response from admin/set
		public class Set : IXmlToObject< Roar.WebObjects.Admin.SetResponse >
		{
			public Roar.WebObjects.Admin.SetResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.SetResponse retval = new Roar.WebObjects.Admin.SetResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from admin/set_custom
		public class SetCustom : IXmlToObject< Roar.WebObjects.Admin.SetCustomResponse >
		{
			public Roar.WebObjects.Admin.SetCustomResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.SetCustomResponse retval = new Roar.WebObjects.Admin.SetCustomResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from admin/view_player
		public class ViewPlayer : IXmlToObject< Roar.WebObjects.Admin.ViewPlayerResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			public Roar.WebObjects.Admin.ViewPlayerResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Admin.ViewPlayerResponse retval = new Roar.WebObjects.Admin.ViewPlayerResponse();
				retval.player = Player.CreateFromXml(n.SelectSingleNode("./admin/view_player") as System.Xml.XmlElement);
				retval.items = new List<InventoryItem>();
				System.Xml.XmlNodeList item_nodes = n.SelectNodes("./admin/view_player/items/item");
				foreach (System.Xml.XmlElement item_node in item_nodes)
				{
					retval.items.Add(DomainObjects.InventoryItem.CreateFromXml(item_node, ixcrm_parser));
				}
				return retval;
			}
		}

 	}
	namespace Appstore
	{
		//Response from appstore/buy
		public class Buy : IXmlToObject< Roar.WebObjects.Appstore.BuyResponse >
		{
			public Roar.WebObjects.Appstore.BuyResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Appstore.BuyResponse retval = new Roar.WebObjects.Appstore.BuyResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from appstore/shop_list
		public class ShopList : IXmlToObject< Roar.WebObjects.Appstore.ShopListResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Appstore.ShopListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Appstore.ShopListResponse retval = new Roar.WebObjects.Appstore.ShopListResponse();
				retval.shop_list = new List<AppstoreShopEntry>();
				System.Xml.XmlNodeList product_nodes = n.SelectNodes("./appstore/shop_list/shopitem");
				foreach (System.Xml.XmlNode product_node in product_nodes)
				{
					retval.shop_list.Add(AppstoreShopEntry.CreateFromXml(product_node as System.Xml.XmlElement, ixcrm_parser));
				}
				return retval;
			}
		}

 	}
	namespace ChromeWebStore
	{
		//Response from chrome_web_store/list
		public class List : IXmlToObject< Roar.WebObjects.ChromeWebStore.ListResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.ChromeWebStore.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.ChromeWebStore.ListResponse retval = new Roar.WebObjects.ChromeWebStore.ListResponse();
				retval.shop_items = new List<ChromeWebStoreShopEntry>();
				System.Xml.XmlNodeList shop_item_nodes = n.SelectNodes("./chrome_web_store/list/shopitem");
				foreach (System.Xml.XmlNode shop_item_node in shop_item_nodes)
				{
					retval.shop_items.Add(ChromeWebStoreShopEntry.CreateFromXml(shop_item_node as System.Xml.XmlElement, ixcrm_parser));
				}
				return retval;
			}
		}

 	}
	namespace Facebook
	{
		//Response from facebook/bind_oauth
		public class BindOauth : IXmlToObject< Roar.WebObjects.Facebook.BindOauthResponse >
		{
			public Roar.WebObjects.Facebook.BindOauthResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.BindOauthResponse retval = new Roar.WebObjects.Facebook.BindOauthResponse();
				return retval;
			}
		}
		//Response from facebook/bind_signed
		public class BindSigned : IXmlToObject< Roar.WebObjects.Facebook.BindSignedResponse >
		{
			public Roar.WebObjects.Facebook.BindSignedResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.BindSignedResponse retval = new Roar.WebObjects.Facebook.BindSignedResponse();
				return retval;
			}
		}
		//Response from facebook/create_oauth
		public class CreateOauth : IXmlToObject< Roar.WebObjects.Facebook.CreateOauthResponse >
		{
			public Roar.WebObjects.Facebook.CreateOauthResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.CreateOauthResponse retval = new Roar.WebObjects.Facebook.CreateOauthResponse();
				retval.auth_token = n.SelectSingleNode("./facebook/create_oauth/auth_token").InnerText;
				retval.player_id = n.SelectSingleNode("./facebook/create_oauth/player_id").InnerText;
				return retval;
			}
		}
		//Response from facebook/create_signed
		public class CreateSigned : IXmlToObject< Roar.WebObjects.Facebook.CreateSignedResponse >
		{
			public Roar.WebObjects.Facebook.CreateSignedResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.CreateSignedResponse retval = new Roar.WebObjects.Facebook.CreateSignedResponse();
				retval.auth_token = n.SelectSingleNode("./facebook/create_signed/auth_token").InnerText;
				retval.player_id = n.SelectSingleNode("./facebook/create_signed/player_id").InnerText;
				return retval;
			}
		}
		//Response from facebook/fetch_oauth_token
		public class FetchOauthToken : IXmlToObject< Roar.WebObjects.Facebook.FetchOauthTokenResponse >
		{
			public Roar.WebObjects.Facebook.FetchOauthTokenResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.FetchOauthTokenResponse retval = new Roar.WebObjects.Facebook.FetchOauthTokenResponse();
				retval.oauth_token = n.SelectSingleNode("./facebook/fetch_oauth_token/oauth_token").InnerText;
				return retval;
			}
		}
		//Response from facebook/friends
		public class Friends : IXmlToObject< Roar.WebObjects.Facebook.FriendsResponse >
		{
			public Roar.WebObjects.Facebook.FriendsResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.FriendsResponse retval = new Roar.WebObjects.Facebook.FriendsResponse();
				retval.facebook_friends = new List<DomainObjects.FacebookFriendInfo>();
				System.Xml.XmlNodeList friend_nodes = n.SelectNodes("./facebook/friends/friend");
				foreach( System.Xml.XmlElement friend_node in friend_nodes )
				{
					DomainObjects.FacebookFriendInfo f = new DomainObjects.FacebookFriendInfo();
					f.fb_name = friend_node.GetAttributeOrDefault("fb_name",null);
					f.fb_id = friend_node.GetAttributeOrDefault("fb_id",null);
					f.id = friend_node.GetAttributeOrDefault("id",null);
					f.name = friend_node.GetAttributeOrDefault("name",null);
					retval.facebook_friends.Add(f);
				}	
				return retval;
			}
		}
		//Response from facebook/login_oauth
		public class LoginOauth : IXmlToObject< Roar.WebObjects.Facebook.LoginOauthResponse >
		{
			public Roar.WebObjects.Facebook.LoginOauthResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.LoginOauthResponse retval = new Roar.WebObjects.Facebook.LoginOauthResponse();
				retval.auth_token = n.SelectSingleNode("./facebook/login_oauth/auth_token").InnerText;
				retval.player_id = n.SelectSingleNode("./facebook/login_oauth/player_id").InnerText;
				return retval;
			}
		}
		//Response from facebook/login_signed
		public class LoginSigned : IXmlToObject< Roar.WebObjects.Facebook.LoginSignedResponse >
		{
			public Roar.WebObjects.Facebook.LoginSignedResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.LoginSignedResponse retval = new Roar.WebObjects.Facebook.LoginSignedResponse();
				retval.auth_token = n.SelectSingleNode("./facebook/login_signed/auth_token").InnerText;
				retval.player_id = n.SelectSingleNode("./facebook/login_signed/player_id").InnerText;
				return retval;
			}
		}
		//Response from facebook/shop_list
		public class ShopList : IXmlToObject< Roar.WebObjects.Facebook.ShopListResponse >
		{
			public IXCRMParser ixcrm_parser;
			
			public ShopList()
			{
				this.ixcrm_parser = new XCRMParser();
			}
			
			public Roar.WebObjects.Facebook.ShopListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Facebook.ShopListResponse retval = new Roar.WebObjects.Facebook.ShopListResponse();
				retval.shop_list = new List<DomainObjects.FacebookShopEntry>();
				System.Xml.XmlNodeList shop_item_nodes = n.SelectNodes("./facebook/shop_list/shopitem");
				foreach( System.Xml.XmlElement shop_item_node in shop_item_nodes )
				{
					retval.shop_list.Add( DomainObjects.FacebookShopEntry.CreateFromXml( shop_item_node, ixcrm_parser ) );
				}
				return retval;
			}
		}

 	}
	namespace Friends
	{
		//Response from friends/accept
		public class Accept : IXmlToObject< Roar.WebObjects.Friends.AcceptResponse >
		{
			public Roar.WebObjects.Friends.AcceptResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.AcceptResponse retval = new Roar.WebObjects.Friends.AcceptResponse();
				return retval;
			}
		}
		//Response from friends/decline
		public class Decline : IXmlToObject< Roar.WebObjects.Friends.DeclineResponse >
		{
			public Roar.WebObjects.Friends.DeclineResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.DeclineResponse retval = new Roar.WebObjects.Friends.DeclineResponse();
				return retval;
			}
		}
		//Response from friends/invite
		public class Invite : IXmlToObject< Roar.WebObjects.Friends.InviteResponse >
		{
			public Roar.WebObjects.Friends.InviteResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.InviteResponse retval = new Roar.WebObjects.Friends.InviteResponse();
				return retval;
			}
		}
		//Response from friends/invite_info
		public class InviteInfo : IXmlToObject< Roar.WebObjects.Friends.InviteInfoResponse >
		{
			public Roar.WebObjects.Friends.InviteInfoResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.InviteInfoResponse retval = new Roar.WebObjects.Friends.InviteInfoResponse();
				retval.info = new DomainObjects.FriendInviteInfo();
				//TODO: This path suggest that we're ggtting the wrong thing back from roar
				System.Xml.XmlElement info_node = n.SelectSingleNode("./friends/info") as System.Xml.XmlElement;
				System.Xml.XmlElement from_node = info_node.SelectSingleNode("./from") as System.Xml.XmlElement;
				System.Xml.XmlElement message_node = info_node.SelectSingleNode("./message") as System.Xml.XmlElement;
				if (from_node != null)
				{
					retval.info.player_id = from_node.GetAttribute("player_id");
					retval.info.name = from_node.GetAttribute("name");
					if (! System.Int32.TryParse(from_node.GetAttribute("level"), out retval.info.level))
					{
						throw new  Roar.DataConversion.InvalidXMLElementException("Unable to parse level to integer");
					}
				}
					
				if (message_node != null)
				{
					retval.info.message = message_node.GetAttribute("value");
				}
				return retval;
			}
		}
		//Response from friends/list
		public class List : IXmlToObject< Roar.WebObjects.Friends.ListResponse >
		{
			public Roar.WebObjects.Friends.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.ListResponse retval = new Roar.WebObjects.Friends.ListResponse();
				retval.friends = new List<DomainObjects.Friend>();
				
				System.Xml.XmlNodeList friend_nodes = n.SelectNodes("./friends/list/friend");
				foreach( System.Xml.XmlElement friend_node in friend_nodes )
				{
					DomainObjects.Friend a = Roar.DomainObjects.Friend.CreateFromXml(friend_node);
					retval.friends.Add(a);
				}
				return retval;
			}
		}
		//Response from friends/remove
		public class Remove : IXmlToObject< Roar.WebObjects.Friends.RemoveResponse >
		{
			public Roar.WebObjects.Friends.RemoveResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.RemoveResponse retval = new Roar.WebObjects.Friends.RemoveResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from friends/list_invites
		public class ListInvites : IXmlToObject< Roar.WebObjects.Friends.ListInvitesResponse >
		{
			public Roar.WebObjects.Friends.ListInvitesResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Friends.ListInvitesResponse retval = new Roar.WebObjects.Friends.ListInvitesResponse();
				retval.invites = new List<DomainObjects.FriendInvite>();
				System.Xml.XmlNodeList invite_nodes = n.SelectNodes("./friends/list_invites/invite");
				foreach( System.Xml.XmlElement invite_node in invite_nodes )
				{
					DomainObjects.FriendInvite invite = new DomainObjects.FriendInvite();

					invite.invite_id = invite_node.GetAttribute("invite_id");
					invite.player_id = invite_node.GetAttribute("from_player_id");
					invite.player_name = invite_node.GetAttribute("from_player_name");
					invite.message = invite_node.GetAttribute("message");
					
					retval.invites.Add(invite);
				}
				return retval;
			}
		}

 	}
	namespace Google
	{
		//Response from google/bind_user
		public class BindUser : IXmlToObject< Roar.WebObjects.Google.BindUserResponse >
		{
			public Roar.WebObjects.Google.BindUserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.BindUserResponse retval = new Roar.WebObjects.Google.BindUserResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from google/bind_user_token
		public class BindUserToken : IXmlToObject< Roar.WebObjects.Google.BindUserTokenResponse >
		{
			public Roar.WebObjects.Google.BindUserTokenResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.BindUserTokenResponse retval = new Roar.WebObjects.Google.BindUserTokenResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from google/create_user
		public class CreateUser : IXmlToObject< Roar.WebObjects.Google.CreateUserResponse >
		{
			public Roar.WebObjects.Google.CreateUserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.CreateUserResponse retval = new Roar.WebObjects.Google.CreateUserResponse();
				System.Xml.XmlElement create_user_node = n.SelectSingleNode("./google/create_user") as System.Xml.XmlElement;
				if (create_user_node != null)
				{
					System.Xml.XmlNode node = create_user_node.SelectSingleNode("./auth_token");
					if (node != null)
					{
						retval.auth_token = node.InnerText;
					}
					node = create_user_node.SelectSingleNode("./player_id");
					if (node != null)
					{
						retval.player_id = node.InnerText;
					}
				}
				return retval;
			}
		}
		//Response from google/create_user_token
		public class CreateUserToken : IXmlToObject< Roar.WebObjects.Google.CreateUserTokenResponse >
		{
			public Roar.WebObjects.Google.CreateUserTokenResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.CreateUserTokenResponse retval = new Roar.WebObjects.Google.CreateUserTokenResponse();
				System.Xml.XmlNode create_user_token_node = n.SelectSingleNode("./google/create_user");
				if (create_user_token_node != null)
				{
					retval.auth_token = create_user_token_node.SelectSingleNode("./auth_token").GetInnerTextOrDefault(null);
					retval.player_id = create_user_token_node.SelectSingleNode("./player_id").GetInnerTextOrDefault(null);
				}
				return retval;
			}
		}
		//Response from google/friends
		public class Friends : IXmlToObject< Roar.WebObjects.Google.FriendsResponse >
		{
			public Roar.WebObjects.Google.FriendsResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.FriendsResponse retval = new Roar.WebObjects.Google.FriendsResponse();
				retval.friends = new List<GoogleFriend>();
				System.Xml.XmlNodeList friend_nodes = n.SelectNodes("./google/friends/friend");
				foreach (System.Xml.XmlElement friend_node in friend_nodes)
				{
					retval.friends.Add(GoogleFriend.CreateFromXml(friend_node));
				}
				return retval;
			}
		}
		//Response from google/login_or_create_user
		public class LoginOrCreateUser : IXmlToObject< Roar.WebObjects.Google.LoginOrCreateUserResponse >
		{
			public Roar.WebObjects.Google.LoginOrCreateUserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.LoginOrCreateUserResponse retval = new Roar.WebObjects.Google.LoginOrCreateUserResponse();
				System.Xml.XmlNode login_or_create_user_node = n.SelectSingleNode("./google/login_or_create_user");
				if (login_or_create_user_node != null)
				{
					System.Xml.XmlNode node = login_or_create_user_node.SelectSingleNode("./auth_token");
					if (node != null)
					{
						retval.auth_token = node.InnerText;
					}
					node = login_or_create_user_node.SelectSingleNode("./player_id");
					if (node != null)
					{
						retval.player_id = node.InnerText;
					}
					node = login_or_create_user_node.SelectSingleNode("./mode");
					if (node != null)
					{
						retval.mode = node.InnerText;
					}
				}
				return retval;
			}
		}
		//Response from google/login_user
		public class LoginUser : IXmlToObject< Roar.WebObjects.Google.LoginUserResponse >
		{
			public Roar.WebObjects.Google.LoginUserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.LoginUserResponse retval = new Roar.WebObjects.Google.LoginUserResponse();
				System.Xml.XmlNode login_user_node = n.SelectSingleNode("./google/login_user");
				if (login_user_node != null)
				{
					System.Xml.XmlNode node = login_user_node.SelectSingleNode("./auth_token");
					if (node != null)
					{
						retval.auth_token = node.InnerText;
					}
					node = login_user_node.SelectSingleNode("./player_id");
					if (node != null)
					{
						retval.player_id = node.InnerText;
					}
				}
				return retval;
			}
		}
		//Response from google/login_user_token
		public class LoginUserToken : IXmlToObject< Roar.WebObjects.Google.LoginUserTokenResponse >
		{
			public Roar.WebObjects.Google.LoginUserTokenResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.LoginUserTokenResponse retval = new Roar.WebObjects.Google.LoginUserTokenResponse();
				System.Xml.XmlNode login_user_token_node = n.SelectSingleNode("./google/login_user");
				if (login_user_token_node != null)
				{
					System.Xml.XmlNode node = login_user_token_node.SelectSingleNode("./auth_token");
					if (node != null)
					{
						retval.auth_token = node.InnerText;
					}
					node = login_user_token_node.SelectSingleNode("./player_id");
					if (node != null)
					{
						retval.player_id = node.InnerText;
					}
				}
				return retval;
			}
		}
		//Response from google/token
		public class Token : IXmlToObject< Roar.WebObjects.Google.TokenResponse >
		{
			public Roar.WebObjects.Google.TokenResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Google.TokenResponse retval = new Roar.WebObjects.Google.TokenResponse();
				System.Xml.XmlElement token_node = n.SelectSingleNode("./google/token/token") as System.Xml.XmlElement;
				if (token_node != null)
				{
					retval.token = token_node.GetAttribute("value");
				}
				return retval;
			}
		}

 	}
	namespace Info
	{
		//Response from info/get_bulk_player_info
		public class GetBulkPlayerInfo : IXmlToObject< Roar.WebObjects.Info.GetBulkPlayerInfoResponse >
		{
			public Roar.WebObjects.Info.GetBulkPlayerInfoResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Info.GetBulkPlayerInfoResponse retval = new Roar.WebObjects.Info.GetBulkPlayerInfoResponse();
				retval.players = new Dictionary<string, Roar.DomainObjects.BulkPlayerInfo>();
				System.Xml.XmlNodeList player_nodes = n.SelectNodes("./info/get_bulk_player_info/player_info");
				foreach (System.Xml.XmlElement pn in player_nodes)
				{
					Roar.DomainObjects.BulkPlayerInfo player = new Roar.DomainObjects.BulkPlayerInfo();
					System.Xml.XmlNodeList stat_nodes = pn.SelectNodes("./stats/stat");
					foreach (System.Xml.XmlElement stat_node in stat_nodes)
					{
						player.stats.Add(stat_node.GetAttribute("ikey"), stat_node.GetAttribute("value"));
					}
					System.Xml.XmlNodeList property_nodes = pn.SelectNodes("./properties/property");
					foreach (System.Xml.XmlElement property_node in property_nodes)
					{
						player.properties.Add(property_node.GetAttribute("ikey"), property_node.GetAttribute("value"));
					}
					retval.players.Add(pn.GetAttribute("id"), player);
				}
				return retval;
			}
		}
		//Response from info/ping
		public class Ping : IXmlToObject< Roar.WebObjects.Info.PingResponse >
		{
			public Roar.WebObjects.Info.PingResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Info.PingResponse retval = new Roar.WebObjects.Info.PingResponse();
				System.Xml.XmlNode ping_node = n.SelectSingleNode("./info/ping");
				if (ping_node != null)
				{
					retval.text = ping_node.SelectSingleNode("./text").InnerText;
				}
				return retval;
			}
		}
		//Response from info/user
		public class User : IXmlToObject< Roar.WebObjects.Info.UserResponse >
		{
			public Roar.WebObjects.Info.UserResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Info.UserResponse retval = new Roar.WebObjects.Info.UserResponse();
				retval.player = Player.CreateFromXml(n.SelectSingleNode("./info/user") as System.Xml.XmlElement);
				return retval;
			}
		}
		//Response from info/poll
		public class Poll : IXmlToObject< Roar.WebObjects.Info.PollResponse >
		{
			public Roar.WebObjects.Info.PollResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Info.PollResponse retval = new Roar.WebObjects.Info.PollResponse();
				//TODO: Implement me
				return retval;
			}
		}

 	}
	namespace Items
	{
		//Response from items/equip
		public class Equip : IXmlToObject< Roar.WebObjects.Items.EquipResponse >
		{
			public Roar.WebObjects.Items.EquipResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.EquipResponse retval = new Roar.WebObjects.Items.EquipResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from items/list
		public class List : IXmlToObject< Roar.WebObjects.Items.ListResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Items.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.ListResponse retval = new Roar.WebObjects.Items.ListResponse();
				retval.items = new List<InventoryItem>();
				System.Xml.XmlNodeList item_nodes = n.SelectNodes("./items/list/item");
				foreach(System.Xml.XmlElement item_node in item_nodes)
				{
					retval.items.Add(DomainObjects.InventoryItem.CreateFromXml(item_node, ixcrm_parser));
				}
				return retval;
			}
		}
		//Response from items/sell
		public class Sell : IXmlToObject< Roar.WebObjects.Items.SellResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Items.SellResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.SellResponse retval = new Roar.WebObjects.Items.SellResponse();
				System.Xml.XmlNode item_node = n.SelectSingleNode("./items/sell/item");
				if (item_node != null)
				{
					retval.item = DomainObjects.InventoryItem.CreateFromXml(item_node as System.Xml.XmlElement, ixcrm_parser);
				}
				System.Xml.XmlNode effect_node = n.SelectSingleNode("./items/sell/effect");
				if (effect_node != null)
				{
					retval.effect = DomainObjects.ModifierResult.CreateFromXml(effect_node as System.Xml.XmlElement);
				}
				return retval;
			}
		}
		//Response from items/set
		public class Set : IXmlToObject< Roar.WebObjects.Items.SetResponse >
		{
			public Roar.WebObjects.Items.SetResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.SetResponse retval = new Roar.WebObjects.Items.SetResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from items/unequip
		public class Unequip : IXmlToObject< Roar.WebObjects.Items.UnequipResponse >
		{
			public Roar.WebObjects.Items.UnequipResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.UnequipResponse retval = new Roar.WebObjects.Items.UnequipResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from items/use
		public class Use : IXmlToObject< Roar.WebObjects.Items.UseResponse >
		{
			public Roar.WebObjects.Items.UseResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.UseResponse retval = new Roar.WebObjects.Items.UseResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from items/view
		public class View : IXmlToObject< Roar.WebObjects.Items.ViewResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Items.ViewResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.ViewResponse retval = new Roar.WebObjects.Items.ViewResponse();
				retval.items = new List<ItemArchetype>();
				System.Xml.XmlNodeList item_nodes = n.SelectNodes("./items/view/item");
				foreach(System.Xml.XmlElement item_node in item_nodes)
				{
					retval.items.Add(DomainObjects.ItemArchetype.CreateFromXml(item_node, ixcrm_parser));
				}
				return retval;
			}
		}
		//Response from items/view_all
		public class ViewAll : IXmlToObject< Roar.WebObjects.Items.ViewAllResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Items.ViewAllResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Items.ViewAllResponse retval = new Roar.WebObjects.Items.ViewAllResponse();
				retval.items = new List<ItemArchetype>();
				System.Xml.XmlNodeList item_nodes = n.SelectNodes("./items/view_all/item");
				foreach(System.Xml.XmlElement item_node in item_nodes)
				{
					retval.items.Add(DomainObjects.ItemArchetype.CreateFromXml(item_node, ixcrm_parser));
				}
				return retval;
			}
		}

 	}
	namespace Leaderboards
	{
		//Response from leaderboards/list
		public class List : IXmlToObject< Roar.WebObjects.Leaderboards.ListResponse >
		{
			public Roar.WebObjects.Leaderboards.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Leaderboards.ListResponse retval = new Roar.WebObjects.Leaderboards.ListResponse();
				retval.boards = new List<DomainObjects.LeaderboardInfo>();
				
				System.Xml.XmlNodeList board_nodes = n.SelectNodes("./leaderboards/list/board");
				foreach( System.Xml.XmlElement nn in board_nodes )
				{
					retval.boards.Add( Roar.DomainObjects.LeaderboardInfo.CreateFromXml(nn) );
				}
				return retval;
			}
		}
		//Response from leaderboards/view
		public class View : IXmlToObject< Roar.WebObjects.Leaderboards.ViewResponse >
		{
			public Roar.WebObjects.Leaderboards.ViewResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Leaderboards.ViewResponse retval = new Roar.WebObjects.Leaderboards.ViewResponse();
				System.Xml.XmlElement nn = n.SelectSingleNode("./leaderboards/view/ranking") as System.Xml.XmlElement;
				retval.leaderboard_data = BuildLeaderboardData(nn);
				//TODO: Implement me
				return retval;
			}
			
			
			public DomainObjects.LeaderboardData BuildLeaderboardData( System.Xml.XmlElement n )
			{
				DomainObjects.LeaderboardData retval = new DomainObjects.LeaderboardData();

				retval.ikey = n.GetAttribute("ikey");
				retval.id = n.GetAttribute("board_id");
				retval.resource_id = n.GetAttribute("resource_id");
				retval.label = n.GetAttribute("label");
				
				//Work around misspelling of offset.
				retval.offset = n.HasAttribute("offset") ? int.Parse( n.GetAttribute("offset") ) : (
					n.HasAttribute("offest") ? int.Parse( n.GetAttribute("offest") ) : 0 );
					
				retval.num_results = n.HasAttribute("num_results") ? int.Parse ( n.GetAttribute("num_results") ) : 0;
				retval.page = n.HasAttribute("page") ? int.Parse( n.GetAttribute("page") ) : 0;
				retval.low_is_high = n.HasAttribute("low_is_high") && System.Boolean.Parse(n.GetAttribute("low_is_high") );

			retval.entries = new List<LeaderboardEntry>();

			foreach( System.Xml.XmlNode c in n )
			{
				if( c.NodeType != System.Xml.XmlNodeType.Element ) continue;
				if( c.Name=="entry")
				{
					LeaderboardEntry lbe = new LeaderboardEntry();
					foreach ( System.Xml.XmlAttribute kv in c.Attributes)
					{
						if( kv.Name=="player_id" )
						{
							lbe.player_id = kv.Value;
						}
						else if( kv.Name=="rank" )
						{
							lbe.rank = System.Int32.Parse( kv.Value );
						}
						else if( kv.Name=="value" )
						{
							lbe.value = System.Double.Parse( kv.Value );
						}
						else if( kv.Name=="ikey" )
						{
							//Ignored
						}
						else
						{
							throw new UnexpectedXMLElementException("unexpected attribute, \""+kv.Name+"\", on Leaderboard");
						}
					}

					lbe.properties = new List<LeaderboardExtraProperties>();
					foreach( System.Xml.XmlNode cc in c )
					{
						if( cc.NodeType != System.Xml.XmlNodeType.Element ) continue;
						if(cc.Name == "custom" )
						{
							foreach( System.Xml.XmlNode ccc in cc )
							{
								if( ccc.NodeType != System.Xml.XmlNodeType.Element ) continue;
								if( ccc.Name == "property" )
								{
									LeaderboardExtraProperties prop = new LeaderboardExtraProperties();
									System.Xml.XmlElement ccc_e = ccc as System.Xml.XmlElement;
									prop.ikey = ccc_e.GetAttribute("ikey");
									prop.value = ccc_e.GetAttribute("value");
									lbe.properties.Add( prop );
								}
								else
								{
									throw new UnexpectedXMLElementException("unexpected child, \""+c.Name+"\", on Leadeboard Entry custom properties");
								}
							}
						}
						else
						{
							throw new UnexpectedXMLElementException("unexpected child, \""+c.Name+"\", on Leadeboard Entry");
						}
					}

					retval.entries.Add( lbe );
				}
				else
				{
					throw new UnexpectedXMLElementException("unexpected child, \""+c.Name+"\", on Leadeboard");
				}
			}
			return retval;
			}

		}

 	}
	namespace Mail
	{
		//Response from mail/accept
		public class Accept : IXmlToObject< Roar.WebObjects.Mail.AcceptResponse >
		{
			public Roar.WebObjects.Mail.AcceptResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Mail.AcceptResponse retval = new Roar.WebObjects.Mail.AcceptResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from mail/send
		public class Send : IXmlToObject< Roar.WebObjects.Mail.SendResponse >
		{
			public Roar.WebObjects.Mail.SendResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Mail.SendResponse retval = new Roar.WebObjects.Mail.SendResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from mail/what_can_i_accept
		public class WhatCanIAccept : IXmlToObject< Roar.WebObjects.Mail.WhatCanIAcceptResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Mail.WhatCanIAcceptResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Mail.WhatCanIAcceptResponse retval = new Roar.WebObjects.Mail.WhatCanIAcceptResponse();
				retval.packages = new List<MailPackage>();
				System.Xml.XmlNodeList package_nodes = n.SelectNodes("./mail/what_can_i_accept/package");
				foreach (System.Xml.XmlElement package_node in package_nodes)
				{
					retval.packages.Add(MailPackage.CreateFromXml(package_node, ixcrm_parser));
				}
				return retval;
			}
		}
		//Response from mail/what_can_i_send
		public class WhatCanISend : IXmlToObject< Roar.WebObjects.Mail.WhatCanISendResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Mail.WhatCanISendResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Mail.WhatCanISendResponse retval = new Roar.WebObjects.Mail.WhatCanISendResponse();
				retval.mailables = new List<Mailable>();
				System.Xml.XmlNodeList mailable_nodes = n.SelectNodes("./mail/what_can_i_send/mailable");
				foreach (System.Xml.XmlElement mailable_node in mailable_nodes)
				{
					retval.mailables.Add(Mailable.CreateFromXml(mailable_node, ixcrm_parser));
				}
				return retval;
			}
		}

 	}
	namespace Shop
	{
		//Response from shop/list
		public class List : IXmlToObject< Roar.WebObjects.Shop.ListResponse >
		{
			public Roar.DataConversion.IXCRMParser ixcrm_parser = new Roar.DataConversion.XCRMParser();

			public Roar.WebObjects.Shop.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Shop.ListResponse retval = new Roar.WebObjects.Shop.ListResponse();
				retval.shop_entries = new List<Roar.DomainObjects.ShopEntry>();
				System.Xml.XmlNodeList shopitem_nodes = n.SelectNodes("./shop/list/shopitem");
				foreach( System.Xml.XmlElement nn in shopitem_nodes )
				{
					retval.shop_entries.Add( Roar.DomainObjects.ShopEntry.CreateFromXml(nn,ixcrm_parser) );
				}
				return retval;
			}
		}
		//Response from shop/buy
		public class Buy : IXmlToObject< Roar.WebObjects.Shop.BuyResponse >
		{
			public Roar.WebObjects.Shop.BuyResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Shop.BuyResponse retval = new Roar.WebObjects.Shop.BuyResponse();
				System.Xml.XmlNode buy_node = n.SelectSingleNode("./shop/buy");
				retval.buy_response = Roar.DomainObjects.ModifierResult.CreateFromXml(buy_node as System.Xml.XmlElement);
				return retval;
			}
		}

 	}
	namespace Scripts
	{
		//Response from scripts/run
		public class Run : IXmlToObject< Roar.WebObjects.Scripts.RunResponse >
		{
			public Roar.WebObjects.Scripts.RunResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Scripts.RunResponse retval = new Roar.WebObjects.Scripts.RunResponse();
				System.Xml.XmlNode result_node = n.SelectSingleNode("./scripts/run");
				retval.result.resultNode = result_node as System.Xml.XmlElement;
				return retval;
			}
		}
		//Response from scripts/run_admin
		public class RunAdmin : IXmlToObject< Roar.WebObjects.Scripts.RunAdminResponse >
		{
			public Roar.WebObjects.Scripts.RunAdminResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Scripts.RunAdminResponse retval = new Roar.WebObjects.Scripts.RunAdminResponse();
				System.Xml.XmlNode result_node = n.SelectSingleNode("./scripts/run");
				retval.result.resultNode = result_node as System.Xml.XmlElement;
				return retval;
			}
		}
 	}
	namespace Tasks
	{
		//Response from tasks/list
		public class List : IXmlToObject< Roar.WebObjects.Tasks.ListResponse >
		{
			public IXCRMParser ixcrm_parser = new XCRMParser();
			
			public Roar.WebObjects.Tasks.ListResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Tasks.ListResponse retval = new Roar.WebObjects.Tasks.ListResponse();
				retval.tasks = new List<Task>();
				System.Xml.XmlNodeList tasks_nodes = n.SelectNodes("./tasks/list/task");
				foreach( System.Xml.XmlElement task in tasks_nodes )
				{
					retval.tasks.Add(Roar.DomainObjects.Task.CreateFromXml(task, ixcrm_parser));
				}
				return retval;
			}
		}
		//Response from tasks/start
		public class Start : IXmlToObject< Roar.WebObjects.Tasks.StartResponse >
		{
			public Roar.WebObjects.Tasks.StartResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Tasks.StartResponse retval = new Roar.WebObjects.Tasks.StartResponse();
				//TODO: Implement me
				return retval;
			}
		}

 	}
	namespace User
	{
		//Response from user/achievements
		public class Achievements : IXmlToObject< Roar.WebObjects.User.AchievementsResponse >
		{
			public Roar.WebObjects.User.AchievementsResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.AchievementsResponse retval = new Roar.WebObjects.User.AchievementsResponse();
				retval.achievements = new List<Achievement>();
				System.Xml.XmlNodeList achievement_nodes = n.SelectNodes("./user/achievements/achievement");
				foreach (System.Xml.XmlElement achievement_node in achievement_nodes)
				{
					retval.achievements.Add(Achievement.CreateFromXml(achievement_node));
				}
				return retval;
			}
		}
		//Response from user/change_name
		public class ChangeName : IXmlToObject< Roar.WebObjects.User.ChangeNameResponse >
		{
			public Roar.WebObjects.User.ChangeNameResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.ChangeNameResponse retval = new Roar.WebObjects.User.ChangeNameResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/change_password
		public class ChangePassword : IXmlToObject< Roar.WebObjects.User.ChangePasswordResponse >
		{
			public Roar.WebObjects.User.ChangePasswordResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.ChangePasswordResponse retval = new Roar.WebObjects.User.ChangePasswordResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/create
		public class Create : IXmlToObject< Roar.WebObjects.User.CreateResponse >
		{
			public Roar.WebObjects.User.CreateResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.CreateResponse retval = new Roar.WebObjects.User.CreateResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/login
		public class Login : IXmlToObject< Roar.WebObjects.User.LoginResponse >
		{
			public Roar.WebObjects.User.LoginResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.LoginResponse retval = new Roar.WebObjects.User.LoginResponse();
				System.Xml.XmlNode node = n.SelectSingleNode("./user/login/auth_token");
				if (node != null)
				{
					retval.auth_token = node.InnerText;
				}
				node = n.SelectSingleNode("./user/login/player_id");
				if (node != null)
				{
					retval.player_id = node.InnerText;
				}
				return retval;
			}
		}
		//Response from user/login_facebook_oauth
		public class LoginFacebookOauth : IXmlToObject< Roar.WebObjects.User.LoginFacebookOauthResponse >
		{
			public Roar.WebObjects.User.LoginFacebookOauthResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.LoginFacebookOauthResponse retval = new Roar.WebObjects.User.LoginFacebookOauthResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/logout
		public class Logout : IXmlToObject< Roar.WebObjects.User.LogoutResponse >
		{
			public Roar.WebObjects.User.LogoutResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.LogoutResponse retval = new Roar.WebObjects.User.LogoutResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/netdrive_save
		public class NetdriveSave : IXmlToObject< Roar.WebObjects.User.NetdriveSaveResponse >
		{
			public Roar.WebObjects.User.NetdriveSaveResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.NetdriveSaveResponse retval = new Roar.WebObjects.User.NetdriveSaveResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/netdrive_fetch
		public class NetdriveFetch : IXmlToObject< Roar.WebObjects.User.NetdriveFetchResponse >
		{
			public Roar.WebObjects.User.NetdriveFetchResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.NetdriveFetchResponse retval = new Roar.WebObjects.User.NetdriveFetchResponse();
				System.Xml.XmlElement node = n.SelectSingleNode("./user/netdrive_get/netdrive_field") as System.Xml.XmlElement;
				if (node != null)
				{
					retval.ikey = node.GetAttribute("ikey");
					node = node.SelectSingleNode("data") as System.Xml.XmlElement;
					if (node != null)
					{
						retval.data = node.InnerText;
					}
				}
				return retval;
			}
		}
		//Response from user/set
		public class Set : IXmlToObject< Roar.WebObjects.User.SetResponse >
		{
			public Roar.WebObjects.User.SetResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.SetResponse retval = new Roar.WebObjects.User.SetResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/view
		public class View : IXmlToObject< Roar.WebObjects.User.ViewResponse >
		{
			public Roar.WebObjects.User.ViewResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.ViewResponse retval = new Roar.WebObjects.User.ViewResponse();
				retval.attributes = new List<PlayerAttribute>();
				System.Xml.XmlNodeList attribute_nodes = n.SelectNodes("./user/view/attribute");
				foreach( System.Xml.XmlElement attribute_node in attribute_nodes )
				{
					PlayerAttribute a = new PlayerAttribute();
					a.ParseXml(attribute_node);
					retval.attributes.Add(a);
				}
				return retval;
			}
		}
		//Response from user/private_set
		public class PrivateSet : IXmlToObject< Roar.WebObjects.User.PrivateSetResponse >
		{
			public Roar.WebObjects.User.PrivateSetResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.PrivateSetResponse retval = new Roar.WebObjects.User.PrivateSetResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from user/private_get
		public class PrivateGet : IXmlToObject< Roar.WebObjects.User.PrivateGetResponse >
		{
			public Roar.WebObjects.User.PrivateGetResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.User.PrivateGetResponse retval = new Roar.WebObjects.User.PrivateGetResponse();
				System.Xml.XmlElement private_field_node = n.SelectSingleNode("./user/private_get/private_field") as System.Xml.XmlElement;
				if (private_field_node != null)
				{
					retval.ikey = private_field_node.GetAttribute("ikey");
					retval.data = private_field_node.GetAttribute("data");
				}
				return retval;
			}
		}

 	}
	namespace Urbanairship
	{
		//Response from urbanairship/ios_register
		public class IosRegister : IXmlToObject< Roar.WebObjects.Urbanairship.IosRegisterResponse >
		{
			public Roar.WebObjects.Urbanairship.IosRegisterResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Urbanairship.IosRegisterResponse retval = new Roar.WebObjects.Urbanairship.IosRegisterResponse();
				//TODO: Implement me
				return retval;
			}
		}
		//Response from urbanairship/push
		public class Push : IXmlToObject< Roar.WebObjects.Urbanairship.PushResponse >
		{
			public Roar.WebObjects.Urbanairship.PushResponse Build(System.Xml.XmlElement n)
			{
				Roar.WebObjects.Urbanairship.PushResponse retval = new Roar.WebObjects.Urbanairship.PushResponse();
				//TODO: Implement me
				return retval;
			}
		}

 	}

}

