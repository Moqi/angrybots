using System.Collections.Generic;
using System.Linq;
using Roar.DataConversion;

namespace Roar
{
	namespace DomainObjects
	{
	
		public abstract class Cost
		{
			public bool ok;
			public string reason;
		};
		
		public abstract class Requirement
		{
			public bool ok;
			public string reason;
		};
		
		public abstract class Modifier
		{
		};
		
		public abstract class ItemStat
		{
			public string ikey;
			public int value;
			public ItemStat (string ikey, int value)
			{
				this.ikey = ikey;
				this.value = value;
			}
		};
		
		
		namespace Costs
		{
			public class Multiple : Cost
			{
				public List<Cost> costs;
			}
			
			public class Stat : Cost
			{
				public string ikey;
				public string type;
				public int value;
			}
			
			public class Item : Cost
			{
				public string ikey;
				public int number_required;
			}
		}
		
		namespace Modifiers
		{
			public class RemoveItems : Modifier
			{
				public string ikey;
				public int count;
			}
			
			public class GrantStat : Modifier
			{
				public string ikey;
				public string type;
				public int value;
			}
			
			
			public class GrantStatRange : Modifier
			{
				public string ikey;
				public string type;
				public int min;
				public int max;
			}
			
			public class GrantXp : Modifier
			{
				public int value;
			}
			
			public class GrantXpRange : Modifier
			{
				public int min;
				public int max;
			}
			
			public class GrantItem : Modifier
			{
				public string ikey;
			}
			
			public class Nothing : Modifier
			{
			}
			
			public class NamedReference : Modifier
			{
			}
			
			public class Script : Modifier
			{
			}
			
			public class Multiple : Modifier
			{
				public List<Modifier> modifier;
			}
			
			public class IfThenElse : Modifier
			{
				public IList<Requirement> if_;
				public IList<Modifier> then_;
				public IList<Modifier> else_;
			}
			
			public class RandomChoice : Modifier
			{
				public struct ChoiceEntry
				{
					public int weight;
					public IList<Modifier> modifiers;
					public IList<Requirement> requirements;
				}
				
				public List<ChoiceEntry> choices;
			}
			
		}
		
		namespace Requirements
		{
			public class Level : Requirement
			{
				public int level;
			}
			
			public class Item : Requirement
			{
				public string ikey;
				public int number_required;
			}
			
			public class Stat : Requirement
			{
				public string ikey;
				public string type;
				public int value;
			}
			
			public class Multiple : Requirement
			{
				public List<Requirement> requirements;
			}
			
			public class True : Requirement
			{
			}
			
			public class False : Requirement
			{
			}
			
			public class Friends : Requirement
			{
				public int required;
			}
		}
		
		namespace ItemStats
		{
			public class RegenStat : ItemStat
			{
				public int every;
				
				public RegenStat (string ikey, int value) : base (ikey, value)
				{
				}
			}
			
			public class RegenStatLimited : ItemStat
			{
				public int repeat;
				public int times_used;
				
				public RegenStatLimited (string ikey, int value) : base (ikey, value)
				{
				}
			}
			
			public class GrantStat : ItemStat
			{
				public GrantStat (string ikey, int value) : base (ikey, value)
				{
				}
			}
			
			public class EquipAttribute : ItemStat
			{
				public EquipAttribute (string ikey, int value) : base (ikey, value)
				{
				}
			}
			
			public class CollectStat : ItemStat
			{
				public int every;
				public int window;
				public int collect_at;
				
				public CollectStat (string ikey, int value) : base (ikey, value)
				{
				}
			}
			
			public class UnknownStat : ItemStat {
				public UnknownStat (string ikey, int value) : base (ikey, value)
				{
				}
			}
		}
		
		
		public class ShopEntry
		{
			public string ikey;
			public string label;
			public string description;
			
			public IList<Cost> costs = new List<Cost>();
			public IList<Modifier> modifiers = new List<Modifier>();
			public IList<Requirement> requirements = new List<Requirement>();
			
			public IList<string> tags = new List<string>();
			
			public static ShopEntry CreateFromXml( System.Xml.XmlElement n, IXCRMParser xcrm_parser )
			{
				ShopEntry retval = new ShopEntry();
				
				retval.ikey = n.GetAttribute("ikey");
				retval.label = n.GetAttribute("label");
				retval.description = n.GetAttribute("description");
				
				
				System.Xml.XmlElement costs_node = n.SelectSingleNode("./costs") as System.Xml.XmlElement;
				if (costs_node != null)
				{
					retval.costs = xcrm_parser.ParseCostList(costs_node);
				}
				// else branches not necessary as costs, modifiers and requirements are INITIALISED by definition
				
				System.Xml.XmlElement modifiers_node = n.SelectSingleNode("./modifiers") as System.Xml.XmlElement;
				if (modifiers_node != null)
				{
					retval.modifiers = xcrm_parser.ParseModifierList(modifiers_node);
				}
				
				System.Xml.XmlElement requirements_node = n.SelectSingleNode("./requirements") as System.Xml.XmlElement;
				if (requirements_node != null)
				{
					retval.requirements = xcrm_parser.ParseRequirementList(requirements_node);
				}
				
				return retval;
			}
		};
		
		public class ModifierResult
		{
			public int add_xp = 0;
			public IDictionary<string, int> stat_change = new Dictionary<string, int>();
			public IDictionary<string, int> removed_items = new Dictionary<string, int>();
			public IDictionary<string, IList<string>> add_item = new Dictionary<string, IList<string>>();
			public IList<string> tags = new List<string>();
			
			protected void AddCostsOrModifiers( System.Xml.XmlNodeList cost_or_modifier_nodes )
			{
				foreach( System.Xml.XmlElement cost_or_modifier_node in cost_or_modifier_nodes)
				{
					string ikey = cost_or_modifier_node.GetAttribute("ikey");
					int value = 0;
					switch (cost_or_modifier_node.GetAttribute("type"))
					{
					case "stat_change":
						System.Int32.TryParse(cost_or_modifier_node.GetAttribute("value"), out value);
						if (stat_change.ContainsKey(ikey))
						{
							stat_change[ikey] += value;
						}
						else
						{
							stat_change.Add(ikey, value);
						}
						break;
					case "removed_items":
						System.Int32.TryParse(cost_or_modifier_node.GetAttribute("count"), out value);
						if (removed_items.ContainsKey(ikey))
						{
							removed_items[ikey] += value;
						}
						else
						{
							removed_items.Add(ikey, value);
						}
						break;
					case "add_xp":
						System.Int32.TryParse(cost_or_modifier_node.GetAttribute("value"), out value);
						add_xp += value;
						break;
					case "add_item":
						if (add_item.ContainsKey(ikey))
						{
							add_item[ikey].Add(cost_or_modifier_node.GetAttribute("item_id"));
						}
						else
						{
							IList<string> list = new List<string>();
							list.Add(cost_or_modifier_node.GetAttribute("item_id"));
							add_item.Add(ikey, list);
						}
						break;
					default:
						System.Console.WriteLine ("TYPE [" + cost_or_modifier_node.GetAttribute("type") + "]");
						break;
					}
				}
			}
			
			public static ModifierResult CreateFromXml (System.Xml.XmlElement n)
			{
				ModifierResult retval = new ModifierResult();
				System.Xml.XmlNodeList cost_nodes = n.SelectNodes("./costs/cost");
				System.Xml.XmlNodeList modifier_nodes = n.SelectNodes("./modifiers/modifier");
				retval.AddCostsOrModifiers(cost_nodes);
				retval.AddCostsOrModifiers(modifier_nodes);
				System.Xml.XmlNodeList tag_nodes = n.SelectNodes("./tags/tag");
				foreach(System.Xml.XmlElement tag_node in tag_nodes)
				{
					retval.tags.Add(tag_node.GetAttribute("value"));
				}
				return retval;
			}
		};


		public class Friend
		{
			public string player_id;
			public string name;
			public int level;
			public static Friend CreateFromXml( System.Xml.XmlElement n )
			{
				Friend f = new Friend();
				f.player_id = n.SelectSingleNode("./player_id").GetInnerTextOrDefault(null);
				f.name = n.SelectSingleNode("./name").GetInnerTextOrDefault(null);
				f.level = System.Convert.ToInt32( n.SelectSingleNode("./level").GetInnerTextOrDefault("0") );
				return f;
			}
		};

		public class FriendInviteInfo
		{
			public string message;
			public string player_id;
			public string name;
			public int level;
		}
		
		public class FriendInvite
		{
			public string invite_id;
			public string player_id;
			public string player_name;
			public string message;
		}

		public class XPAttributes
		{
			public int value;
			public int next_level;
			public int level_start;
		};
		
		public class Player
		{
			public string id;
			public string name;
			public int level;
			public XPAttributes xp = new XPAttributes();
			public Dictionary<string, PlayerAttribute> attributes = new Dictionary<string, PlayerAttribute>();
			
			public static Player CreateFromXml (System.Xml.XmlElement n)
			{
				Player retval = new Player();
				if (n == null)
				{
					return retval;
				}
				System.Xml.XmlNodeList attribute_nodes = n.SelectNodes("./attribute");
				foreach( System.Xml.XmlElement nn in attribute_nodes )
				{
					switch (nn.GetAttribute("ikey"))
					{
					case "id":
						retval.id = nn.GetAttribute("value");
						break;
					case "name":
						retval.name = nn.GetAttribute("value");
						break;
					case "level":
						System.Int32.TryParse(nn.GetAttribute("value"), out retval.level);
						break;
					case "xp":
						System.Int32.TryParse(nn.GetAttribute("value"), out retval.xp.value);
						System.Int32.TryParse(nn.GetAttribute("level_start"), out retval.xp.level_start);
						System.Int32.TryParse(nn.GetAttribute("next_level"), out retval.xp.next_level);
						break;
					default:
						Roar.DomainObjects.PlayerAttribute attr = new Roar.DomainObjects.PlayerAttribute();
						attr.ParseXml(nn);
						retval.attributes.Add(attr.ikey, attr);
						break;
					}
				}
				return retval;
			}
		};
		
		public class BulkPlayerInfo
		{
			public Dictionary<string, string> stats = new Dictionary<string, string>();
			public Dictionary<string, string> properties = new Dictionary<string, string>();
		};
		
		public class GoogleFriend
		{
			public string id;
			public string name;
			public string gplus_id;
			public string gplus_name;
			
			public static GoogleFriend CreateFromXml (System.Xml.XmlElement n)
			{
				GoogleFriend retval = new GoogleFriend();
				retval.id = n.GetAttributeOrDefault("id",null);
				retval.name = n.GetAttributeOrDefault("name",null);
				retval.gplus_id = n.GetAttributeOrDefault("gplus_id",null);
				retval.gplus_name = n.GetAttributeOrDefault("gplus_name",null);
				return retval;
			}
		}

		public class LeaderboardInfo
		{
			public string board_id;
			public string ikey;
			public string resource_id;
			public string label;
			
			public static LeaderboardInfo CreateFromXml( System.Xml.XmlElement n )
			{
				LeaderboardInfo retval = new LeaderboardInfo();
				retval.ikey = n.GetAttribute("ikey");
				retval.board_id = n.GetAttribute("board_id");
				retval.label = n.GetAttribute("label");
				retval.resource_id = n.GetAttribute("resource_id");
				return retval;
			}
		}

		public class LeaderboardExtraProperties
		{
			public string ikey;
			public string value;
		}

		public class LeaderboardEntry
		{
			public int rank;
			public string player_id;
			public double value;
			public IList<LeaderboardExtraProperties> properties;
		}

		//TODO: Problem with this is that we only keep one page for each leadeerboard request... we really should merge them somehow.
		/**
		 * Expected XML is like this:
		 *
		 * <leaderboards>
		 *   <view status="ok">
		 *      <ranking ikey="mojo" offset="0" num_results="100" page="1" low_is_high="false">
		 *         <entry rank="1" player_id="612421456098" value="560">
		 *           <custom>
		 *             <property ikey="player_name" value="Monkey"/>
		 *           </custom>
		 *         </entry>
		 *         <entry rank="2" player_id="195104156933" value="514">
		 *           <custom>
		 *             <property ikey="player_name" value="Dragon"/>
		 *           </custom>
		 *        </entry>
		 *        <entry rank="3" player_id="440312985759" value="490"/>
		 *          <custom>
		 *            <property ikey="player_name" value="Fun and Awesome DUUUUUDE"/>
		 *           </custom>
		 *        </entry>
		 *     </ranking>
		 *   </view>
		 * </leaderboards>
		 */

		public class LeaderboardData
		{
			public string board_id;
			public string id;
			public string ikey;
			public string resource_id;
			public string label;
			
			public int offset;
			public int num_results;
			public int page;
			public bool low_is_high;

			public IList<LeaderboardEntry> entries;

		}
		
		public class InventoryItem
		{
			public string id;
			public string ikey;
			public string label;
			public string description;
			public bool sellable;
			public bool consumable;
			public string type;
			public bool equipped;
			public int count;
			
			public IList<ItemStat> stats = new List<ItemStat>();
			public IList<DomainObjects.Modifier> price = new List<DomainObjects.Modifier>();
			public IList<string> tags = new List<string>();
			public IList<ItemArchetypeProperty> properties = new List<ItemArchetypeProperty>();
			
			public static InventoryItem CreateFromXml ( System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				DomainObjects.InventoryItem retval = new DomainObjects.InventoryItem();
				retval.id = n.GetAttribute("id");
				retval.ikey = n.GetAttribute("ikey");
				retval.type = n.GetAttribute("type");
				retval.label = n.GetAttribute("label");
				retval.description = n.GetAttribute("description");
				
				if ( n.HasAttribute("count") && ! System.Int32.TryParse(n.GetAttribute("count"), out retval.count))
				{
					throw new InvalidXMLElementException("Unable to parse count to integer");
				}
				if (n.HasAttribute("consumable"))
				{
					retval.consumable = n.GetAttribute("consumable").ToLower() == "true";
				}
				if (n.HasAttribute("sellable"))
				{
					retval.sellable = n.GetAttribute("sellable").ToLower() == "true";
				}
				if (n.HasAttribute("equipped"))
				{
					retval.equipped = n.GetAttribute("equipped").ToLower() == "true";
				}
				retval.stats = ixcrm_parser.ParseItemStatList(n.SelectSingleNode("./stats") as System.Xml.XmlElement);
				retval.price = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./price") as System.Xml.XmlElement);
				retval.tags = ixcrm_parser.ParseTagList(n.SelectSingleNode("./tags") as System.Xml.XmlElement);
				System.Xml.XmlNodeList property_nodes = n.SelectNodes("./properties/property");
				foreach(System.Xml.XmlElement property_node in property_nodes)
				{
					Roar.DomainObjects.ItemArchetypeProperty property = new Roar.DomainObjects.ItemArchetypeProperty();
					property.ikey = property_node.GetAttribute("ikey");
					property.value = property_node.GetAttribute("value");
					retval.properties.Add(property);
				}
				return retval;
			}
		}
		
		public class ItemPrototype
		{
			public string ikey;
		}
		
		public class ItemArchetypeProperty
		{
			public string ikey;
			public string value;
		}
		
		public class ItemArchetype
		{
			public string id;
			public string ikey;
			public string type;
			public string label;
			public string description;
			public bool sellable;
			public bool consumable;
			public IList<ItemStat> stats = new List<ItemStat>();
			public IList<DomainObjects.Modifier> price = new List<DomainObjects.Modifier>();
			public IList<string> tags = new List<string>();
			public IList<ItemArchetypeProperty> properties = new List<ItemArchetypeProperty>();
			
			public static ItemArchetype CreateFromXml (System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				DomainObjects.ItemArchetype retval = new DomainObjects.ItemArchetype();
				retval.id = n.GetAttributeOrDefault("id",null);
				retval.ikey = n.GetAttributeOrDefault("ikey",null);
				retval.type = n.GetAttributeOrDefault("type",null);
				retval.label = n.GetAttributeOrDefault("label",null);
				retval.description = n.GetAttributeOrDefault("description",null);
				
				if (n.HasAttribute("consumable"))
				{
					retval.consumable = n.GetAttribute("consumable").ToLower() == "true";
				}
				if (n.HasAttribute("sellable"))
				{
					retval.sellable = n.GetAttribute("sellable").ToLower() == "true";
				}
				retval.stats = ixcrm_parser.ParseItemStatList(n.SelectSingleNode("./stats") as System.Xml.XmlElement);
				retval.price = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./price") as System.Xml.XmlElement);
				retval.tags = ixcrm_parser.ParseTagList(n.SelectSingleNode("./tags") as System.Xml.XmlElement);
				System.Xml.XmlNodeList property_nodes = n.SelectNodes("./properties/property");
				foreach(System.Xml.XmlElement property_node in property_nodes)
				{
					Roar.DomainObjects.ItemArchetypeProperty property = new Roar.DomainObjects.ItemArchetypeProperty();
					property.ikey = property_node.GetAttribute("ikey");
					property.value = property_node.GetAttribute("value");
					retval.properties.Add(property);
				}
				return retval;
			}
		}
		
		public class MailPackage
		{
			public string id;
			public string type;
			public string sender_id;
			public string sender_name;
			public string message;
			public IList<InventoryItem> items = new List<InventoryItem>();
			public IList<string> tags = new List<string>();
			public IList<DomainObjects.Modifier> modifiers = new List<DomainObjects.Modifier>();
			
			public static MailPackage CreateFromXml (System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				MailPackage retval = new MailPackage();
				retval.id = n.GetAttribute("id");
				retval.type = n.GetAttribute("type");
				retval.sender_id = n.GetAttribute("sender_id");
				retval.sender_name = n.GetAttribute("sender_name");
				retval.message =n.GetAttribute("message");
				System.Xml.XmlNodeList item_nodes = n.SelectNodes("./item");
				foreach (System.Xml.XmlElement item_node in item_nodes)
				{
					retval.items.Add(InventoryItem.CreateFromXml(item_node, ixcrm_parser));
				}
				retval.tags = ixcrm_parser.ParseTagList(n);
				retval.modifiers = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./modifiers") as System.Xml.XmlElement);
				return retval;
			}
		}
		
		public class Mailable
		{
			public string id;
			public string type;
			public string label;
			public IList<DomainObjects.Requirement> requirements = new List<DomainObjects.Requirement>();
			public IList<DomainObjects.Cost> costs = new List<DomainObjects.Cost>();
			public IList<DomainObjects.Modifier> on_accept = new List<DomainObjects.Modifier>();
			public IList<DomainObjects.Modifier> on_give = new List<DomainObjects.Modifier>();
			public IList<string> tags = new List<string>();
			
			public static Mailable CreateFromXml (System.Xml.XmlElement n, IXCRMParser ixcrm_parser)
			{
				Mailable retval = new Mailable();
				retval.id = n.GetAttribute("id");
				retval.type = n.GetAttribute("type");
				retval.label = n.GetAttribute("label");
				retval.requirements = ixcrm_parser.ParseRequirementList(n.SelectSingleNode("./requirements") as System.Xml.XmlElement);
				retval.costs = ixcrm_parser.ParseCostList(n.SelectSingleNode("./costs") as System.Xml.XmlElement);
				retval.on_accept = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./on_accept") as System.Xml.XmlElement);
				retval.on_give = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./on_give") as System.Xml.XmlElement);
				retval.tags = ixcrm_parser.ParseTagList(n.SelectSingleNode("./tags") as System.Xml.XmlElement);
				return retval;
			}
		}
		
		
		public class PlayerAttribute
		{
			public string ikey = null;
			public string name = null;
			public string label = null;
			public string value = null;
			public string type = null;
			public string min = null;
			public string max = null;
			public string regen_amount = null;
			public string regen_every = null;
			public string level_start = null;
			public string next_level = null;
			
			public void ParseXml( System.Xml.XmlElement nn )
			{
				ikey = nn.GetAttributeOrDefault("ikey",null);
				name = nn.GetAttribute("name");
				if( ikey == null ) { ikey = name;} 

				label = nn.GetAttribute("label");
				value = nn.GetAttribute("value");
				type = nn.GetAttribute("type");
				min = nn.GetAttribute("min");
				max = nn.GetAttribute("max");
				regen_amount = nn.GetAttribute("regen_amount");
				regen_every = nn.GetAttribute("regen_every");
				level_start = nn.GetAttribute("level_start");
				next_level = nn.GetAttribute("next_level");
			}
		}
		
		public class FacebookFriendInfo
		{
			public string fb_name;
			public string fb_id;
			public string name; //Only non-null if the player is a user in the current game
			public string id; //Only non-null if the player is a user in the current game
		}
		
		public class FacebookShopEntry
		{
			public string ikey;
			public string description;
			public string label;
			public string price;
			public string product_url;
			public string image_url;
			public IList<Modifier> modifiers;
			
			public static FacebookShopEntry CreateFromXml( System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser )
			{
				FacebookShopEntry retval = new FacebookShopEntry();
				retval.ikey = n.GetAttribute("ikey");
				retval.description = n.GetAttribute("description");
				retval.label = n.GetAttribute("label");
				retval.price = n.GetAttribute("price");
				retval.product_url = n.GetAttribute("product_url");
				retval.image_url = n.GetAttribute("image_url");
				
				System.Xml.XmlNodeList modifier_nodes = n.SelectNodes("./modifiers");
				if( modifier_nodes.Count > 0 )
				{
					retval.modifiers = ixcrm_parser.ParseModifierList( modifier_nodes[0] as System.Xml.XmlElement );
				}
				else
				{
					retval.modifiers = new List<Modifier>();
				}
				
				return retval;
			}
		}
		
		public class AppstoreShopEntry
		{
			public string product_identifier;
			public string label;
			public IList<Modifier> modifiers = new List<Modifier>();
			
			public static AppstoreShopEntry CreateFromXml (System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				AppstoreShopEntry retval = new AppstoreShopEntry();
				retval.product_identifier = n.GetAttribute("product_identifier");
				retval.label = n.GetAttribute("label");
				retval.modifiers = ixcrm_parser.ParseModifierList(n.SelectSingleNode("modifiers") as System.Xml.XmlElement);
				return retval;
			}
			
			public static AppstoreShopEntry CreateFromXml (System.Xml.XmlNode n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				AppstoreShopEntry retval = new AppstoreShopEntry();
				retval.product_identifier = n.Attributes["product_identifier"].Value;
				retval.label = n.Attributes["label"].Value;
				retval.modifiers = ixcrm_parser.ParseModifierList( n.SelectSingleNode("modifiers") as System.Xml.XmlElement );
				return retval;
			}
		}
		
		public class ChromeWebStoreShopEntry
		{
			public string ikey;
			public string label;
			public string description;
			public string price;
			public string jwt;
			public IList<Modifier> modifiers = new List<Modifier>();
			public IList<string> tags = new List<string>();
			
			public static ChromeWebStoreShopEntry CreateFromXml (System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				ChromeWebStoreShopEntry retval = new ChromeWebStoreShopEntry();
				retval.ikey = n.GetAttribute("ikey");
				

				retval.label = n.GetAttribute("label");
				retval.description = n.GetAttribute("description");
				retval.price = n.GetAttribute("price");
				retval.jwt = n.GetAttribute("jwt");
				retval.modifiers = ixcrm_parser.ParseModifierList(n.SelectSingleNode("modifiers") as System.Xml.XmlElement);
				retval.tags = ixcrm_parser.ParseTagList(n.SelectSingleNode("tags") as System.Xml.XmlElement);
				return retval;
			}
		}
		
		public class ScriptRunResult
		{
			public System.Xml.XmlElement resultNode;	
			
		}
		
		public class Task
		{
			public string ikey;
			public string label;
			public string description;
			public string location;
			public int mastery_level;
			public int mastery_progress;
			public IList<DomainObjects.Cost> costs = new List<DomainObjects.Cost>();
			public IList<DomainObjects.Modifier> rewards = new List<DomainObjects.Modifier>();
			public IList<DomainObjects.Requirement> requirements = new List<DomainObjects.Requirement>();
			public IList<string> tags = new List<string>();
			
			public static Task CreateFromXml (System.Xml.XmlElement n, Roar.DataConversion.IXCRMParser ixcrm_parser)
			{
				DomainObjects.Task retval = new DomainObjects.Task();
				retval.ikey = n.GetAttribute("ikey");
				retval.label = n.SelectSingleNode("./label").GetInnerTextOrDefault(null);

				retval.description = n.SelectSingleNode("./description").GetInnerTextOrDefault(null);
				retval.location = n.SelectSingleNode("./location").GetInnerTextOrDefault(null);

				if (! System.Int32.TryParse( n.SelectSingleNode("./mastery/@level").GetValueOrDefault("0"), out retval.mastery_level))
				{
					//throw new InvalidXMLElementException("Unable to parse mastery level to integer");
					retval.mastery_level = 0;
				}
				if (! System.Int32.TryParse( n.SelectSingleNode("./mastery/@progress").GetValueOrDefault("0"), out retval.mastery_progress))
				{
					//throw new InvalidXMLElementException("Untable to parse mastery progress to integer");
					retval.mastery_progress = 0;
				}
				
				retval.costs = ixcrm_parser.ParseCostList(n.SelectSingleNode("./costs") as System.Xml.XmlElement);
				retval.rewards = ixcrm_parser.ParseModifierList(n.SelectSingleNode("./rewards") as System.Xml.XmlElement);
				retval.requirements = ixcrm_parser.ParseRequirementList(n.SelectSingleNode("./requires") as System.Xml.XmlElement);
				retval.tags = ixcrm_parser.ParseTagList(n.SelectSingleNode("./tags") as System.Xml.XmlElement);
				return retval;
			}
		}
		
		public class Achievement
		{
			public string ikey;
			public string status;
			public string label;
			public string progress;
			public string description;
			
			public static Achievement CreateFromXml (System.Xml.XmlElement n)
			{
				Achievement retval = new Achievement();
				retval.ikey = n.SelectSingleNode("./ikey").GetInnerTextOrDefault(null);
				retval.status = n.SelectSingleNode("./status").GetInnerTextOrDefault(null);
				retval.label = n.SelectSingleNode("./label").GetInnerTextOrDefault(null);
				retval.progress = n.SelectSingleNode("./progress").GetInnerTextOrDefault(null);
				retval.description = n.SelectSingleNode("./description").GetInnerTextOrDefault(null);
				return retval;
			}
		}
		


	}
}
