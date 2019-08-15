using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscourseApi {
	public class UserBadges : ApiEntryBase {
		public int id;
		public DateTime granted_at;
		public int count;
		public int badge_id;
		public int user_id;
		public int granted_by_id;
	}

	public class Badges : ApiEntryBase {
		public int id;
		public string name;
		public string description;
		public int grant_count;
		public bool allow_title;
		public bool multiple_grant;
		public string icon;
		public JToken image;
		public bool listable;
		public bool enabled;
		public int badge_grouping_id;
		public bool system;
		public string slug;
		public bool manually_grantable;
		public int badge_type_id;
	}

	public class BadgeTypes : ApiEntryBase {
		public int id;
		public string name;
		public int sort_order;
	}

	public class Users : ApiEntryBase {
		public int id;
		public string username;
		public JToken name;
		public string avatar_template;
		public bool moderator;
		public bool admin;
	}

	public class UserAuthToken : ApiEntryBase {
		public int id;
		public string client_ip;
		public string location;
		public string browser;
		public string device;
		public string os;
		public string icon;
		public DateTime created_at;
		public DateTime seen_at;
		public bool is_active;
	}

	public class UserAuthTokenLog : ApiEntryBase {
		public int id;
		public string client_ip;
		public string location;
		public string browser;
		public string device;
		public string os;
		public string icon;
		public DateTime created_at;
		public string action;
	}

	public class Groups : ApiEntryBase {
		public int id;
		public bool automatic;
		public string name;
		public string display_name;
		public int user_count;
		public int mentionable_level;
		public int messageable_level;
		public int visibility_level;
		public JToken automatic_membership_email_domains;
		public bool automatic_membership_retroactive;
		public bool primary_group;
		public JToken title;
		public JToken grant_trust_level;
		public JToken incoming_email;
		public bool has_messages;
		public JToken flair_url;
		public JToken flair_bg_color;
		public JToken flair_color;
		public JToken bio_raw;
		public JToken bio_cooked;
		public JToken bio_excerpt;
		public bool public_admission;
		public bool public_exit;
		public bool allow_membership_requests;
		public JToken full_name;
		public int default_notification_level;
		public JToken membership_request_template;
	}

	public class GroupUsers : ApiEntryBase {
		public int group_id;
		public int user_id;
		public int notification_level;
		public bool owner;
	}

	public class UserOptions : ApiEntryBase {
		public int user_id;
		public bool mailing_list_mode;
		public int mailing_list_mode_frequency;
		public bool email_digests;
		public int email_level;
		public int email_messages_level;
		public bool external_links_in_new_tab;
		public bool dynamic_favicon;
		public bool enable_quoting;
		public bool enable_defer;
		public int digest_after_minutes;
		public bool automatically_unpin_topics;
		public int auto_track_topics_after_msecs;
		public int notification_level_when_replying;
		public int new_topic_duration_minutes;
		public int email_previous_replies;
		public bool email_in_reply_to;
		public int like_notification_frequency;
		public bool include_tl0_in_digests;
		public string[] theme_ids;
		public int theme_key_seq;
		public bool allow_private_messages;
		public string homepage_id;
		public bool hide_profile_and_presence;
		public string text_size;
		public int text_size_seq;
		public string title_count_mode;
	}

	public class UserCreateData : ApiEntryBase {
		public string name;
		public string email;
		public string password;
		public string username;
		public bool? active;
		public bool? approved;
	}

	public class UserCreateResult : ApiEntryBase {
		public bool success;
		public bool active;
		public string message;
		public int user_id;
	}

	public class UserReturn : ApiEntry {
		public List<UserBadges> user_badges;
		public List<Badges> badges;
		public List<BadgeTypes> badge_types;
		public List<Users> users;
		public User user;
	}

	public class UserCommon : ApiEntryBase {
		public int id;
		public string username;
		public string name;
		public string avatar_template;
		public string email;
		public DateTime? last_seen_at;
		public DateTime created_at;
		public int trust_level;
		public bool moderator;
		public bool admin;
		public JToken title;
		public int time_read;
		public bool staged;
		public int post_count;

	}

	public class UserListEntry : UserCommon {
		public string[] secondary_emails;
		public bool active;
		public DateTime? last_emailed_at;
		public double? last_seen_age;
		public double? last_emailed_age;
		public double created_at_age;
		public string username_lower;
		public string manual_locked_trust_level;
		public int flag_level;
		public bool suspended;
		public int days_visited;
		public int posts_read_count;
		public int topics_entered;
	}

	public class UserList : ApiList<UserListEntry> {
		public override ApiList<UserListEntry> Convert(JObject j) {
			UserList list = j.ConvertToObject<UserList>();
			if (Api.QueryParams(list.MetaData.Uri).TryGetValue("page", out string p))
				list.page = int.Parse(p);
			return list;
		}

	}

	public class User : UserCommon {
		public DateTime? last_posted_at;
		public bool can_edit;
		public bool can_edit_username;
		public bool can_edit_email;
		public bool can_edit_name;
		public bool ignored;
		public bool muted;
		public bool can_ignore_user;
		public bool can_mute_user;
		public bool can_send_private_messages;
		public bool can_send_private_message_to_user;
		public int uploaded_avatar_id;
		public int badge_count;
		public bool has_title_badges;
		public JToken custom_fields;
		public int pending_count;
		public int profile_view_count;
		public int recent_time_read;
		public JToken primary_group_name;
		public JToken primary_group_flair_url;
		public JToken primary_group_flair_bg_color;
		public JToken primary_group_flair_color;
		public bool second_factor_enabled;
		public bool second_factor_backup_enabled;
		public JToken[] associated_accounts;
		public bool can_be_deleted;
		public bool can_delete_all_posts;
		public JToken locale;
		public JToken[] muted_category_ids;
		public JToken[] watched_tags;
		public JToken[] watching_first_post_tags;
		public JToken[] tracked_tags;
		public JToken[] muted_tags;
		public JToken[] tracked_category_ids;
		public JToken[] watched_category_ids;
		public JToken[] watched_first_post_category_ids;
		public JToken system_avatar_upload_id;
		public string system_avatar_template;
		public int gravatar_avatar_upload_id;
		public string gravatar_avatar_template;
		public JToken[] muted_usernames;
		public JToken[] ignored_usernames;
		public int mailing_list_posts_per_day;
		public bool can_change_bio;
		public JToken user_api_keys;
		public List<UserAuthToken> user_auth_tokens;
		public List<UserAuthTokenLog> user_auth_token_logs;
		public JToken invited_by;
		public List<Groups> groups;
		public List<GroupUsers> group_users;
		public int[] featured_user_badge_ids;
		public UserOptions user_option;

		public static async Task<UserReturn> GetByName(Api api, string name) {
			return await api.GetAsync<UserReturn>(Api.Combine("users", name));
		}

		public static async Task<UserReturn> GetBySSOId(Api api, string id) {
			return await api.GetAsync<UserReturn>(Api.Combine("u", "by-external", id));
		}

		public static async Task<UserList> GetByEmail(Api api, string email) {
			JObject j = await api.GetAsync(Api.Combine("admin", "users", "list", "all"), new {
				email,
				page = 1
			});
			return (UserList)new UserList().Convert(j);
		}

		public static async Task<UserCreateResult> Create(Api api, UserCreateData data) {
			return await api.PostAsync<UserCreateResult>("users", null, data);
		}

		public static async Task<UserList> ListAll(Api api, string flag, string order = "created", bool ascending = true) {
			JObject j = await api.GetAsync(Api.Combine("admin", "users", "list", flag), new {
				order,
				ascending,
				page = 1
			});
			return (UserList)new UserList().Convert(j);
		}

	}



}
