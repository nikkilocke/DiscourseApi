using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscourseApi {
	/// <summary>
	/// Permissions for individual groups
	/// </summary>
	public enum PermissionLevel {
		None = 0,
		Create = 1,
		Reply = 2,
		See = 3
	}

	/// <summary>
	/// Set of permissions for an object - group-slug, PermissionLevel
	/// </summary>
	public class Permissions : JObject {
		public Permissions() {
		}

		public Permissions(IEnumerable<GroupPermission> original) {
			foreach (var p in original)
				this[p.group_name] = p.permission_type;
		}

		public Permissions(params GroupPermission [] original) {
			foreach (var p in original)
				this[p.group_name] = p.permission_type;
		}

		public IEnumerable<GroupPermission> List {
			get {
				return Properties().Select(p => new GroupPermission() {
					group_name = p.Name,
					permission_type = (PermissionLevel)(int)p.Value
				});
			}
		}

		public new PermissionLevel this[string group] {
			get {
				return ContainsKey(group) ? (PermissionLevel)(int)base[group] : PermissionLevel.None;
			}
			set {
				if (value == PermissionLevel.None)
					base.Remove(group);
				else
					base[group] = (int)value;
			}
		}
	}

	public class CategoryCommon : ApiEntryBase {
		public const string DefaultColor = "0088CC";
		public const string DefaultTextColor = "FFFFFF";
		/// <summary>
		/// Required
		/// </summary>
		public string name;
		public string color = DefaultColor;
		public string text_color = DefaultTextColor;
		public string slug;
		public int? parent_category_id;
		public int? position;
		public string topic_template;
		public string sort_order;
		public bool? sort_ascending;
		public bool? show_subcategory_list;
		public int? num_featured_topics;
		public string default_view;
		public string subcategory_list_style;
		public string default_top_period;
		public int? minimum_required_tags;
		public bool? navigate_to_first_post_after_read;
		public float? auto_close_hours;
		public bool? auto_close_based_on_last_post;
		public bool? email_in;
		public bool? email_in_allow_strangers;
		public bool? mailinglist_mirror;
		public bool? suppress_from_latest;
		public bool? all_topics_wiki;
		public bool? allow_badges;
		public bool? allow_global_tags;
		public bool? topic_featured_link_allowed;
		public int? search_priority;
		public string reviewable_by_group_name;
	}

	public class UpdateCategoryParams : CategoryCommon {
		public Permissions permissions;
		public JObject uploaded_logo_id;
		public JObject uploaded_background_id;
	}

	public class GroupPermission : ApiEntryBase {
		public GroupPermission() {
		}
		public GroupPermission(string group, PermissionLevel permission) {
			group_name = group;
			permission_type = permission;
		}
		public PermissionLevel permission_type;
		public string group_name;
	}

	public class Category : CategoryCommon {
		public int id;
		public int? topic_count;
		public int? post_count;
		public string description;
		public string description_text;
		public string topic_url;
		public bool? read_restricted;
		public int? permission;
		public int? notification_level;
		public bool? can_edit;
		public bool? has_children;
		public int? topics_day;
		public int? topics_week;
		public int? topics_month;
		public int? topics_year;
		public int? topics_all_time;
		public string description_excerpt;
		public JObject uploaded_logo;
		public JObject uploaded_background;
		public JToken custom_fields;
		public string[] available_groups;
		public List<GroupPermission> group_permissions;
		public bool? can_delete;
		public string cannot_delete_reason;
		public int[] subcategory_ids;

		static public async Task<CategoryListReturn> ListAll(Api api) {
			return await api.GetAsync<CategoryListReturn>("categories");
		}

		static public async Task<Category> Create(Api api, UpdateCategoryParams data) {
			JObject j = await api.PostAsync("categories", null, data);
			return j["category"].ToObject<Category>();
		}

		static public async Task<Category> Get(Api api, int categoryId) {
			JObject j = await api.GetAsync(Api.Combine("c", categoryId, "show"));
			return j["category"].ToObject<Category>();
		}

		static public async Task<Category> Update(Api api, int categoryId, UpdateCategoryParams data) {
			JObject j = await api.PutAsync(Api.Combine("categories", categoryId), null, data);
			return j["category"].ToObject<Category>();
		}

		public async Task Update(Api api) {
			await api.PutAsync(Api.Combine("categories", id), null, this);
		}

		static public async Task Delete(Api api, int categoryId) {
			await api.DeleteAsync(Api.Combine("categories", categoryId));
		}
	}

	public class CategoryList : ApiEntryBase {
		public bool can_create_category;
		public bool can_create_topic;
		public string draft;
		public string draft_key;
		public int draft_sequence;
		public List<Category> categories;
	}

	public class CategoryListReturn : ApiEntry {
		public CategoryList category_list;
	}


}
