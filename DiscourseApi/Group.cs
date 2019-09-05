using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscourseApi {
	public class GroupList : ApiList<Group> {
		public override ApiList<Group> Convert(JObject j) {
			GroupList g = j.ConvertToObject<GroupList>();
			g.TotalCount = (int)j["total_rows_groups"];
			g.List = j["groups"].ToObject<List<Group>>();
			return g;
		}

		public override string NextPageUrl { get; set; }
	}

	public class GroupMember : ApiEntryBase {
		public int id;
		public string username;
		public string avatar_template;
	}

	public class GroupMemberList : ApiList<GroupMember> {
		public List<GroupMember> owners;
		public override ApiList<GroupMember> Convert(JObject j) {
			GroupMemberList result = j.ConvertToObject<GroupMemberList>();
			result.List = result.AdditionalData["members"].ConvertToObject<List<GroupMember>>();
			result.AdditionalData.Remove("members");
			JObject meta = (JObject)result.AdditionalData["meta"];
			result.TotalCount = meta["total"].Value<int>();
			result.Request.limit = meta["limit"].Value<int>();
			result.Request.offset = meta["offset"].Value<int>();
			return result;
		}
	}

	public class UpdateGroupParams : ApiEntryBase {
		public bool? automatic;
		public string name;
		public string display_name;
		public int? mentionable_level;
		public int? messageable_level;
		public int? visibility_level;
		public string automatic_membership_email_domains;
		public bool? automatic_membership_retroactive;
		public bool? primary_group;
		public string title;
		public string grant_trust_level;
		public JToken incoming_email;
		public string flair_url;
		public JToken flair_bg_color;
		public JToken flair_color;
		public string bio_raw;
		public string bio_cooked;
		public string bio_excerpt;
		public bool? public_admission;
		public bool? public_exit;
		public bool? allow_membership_requests;
		public string full_name;
		public int? default_notification_level;
		public JToken membership_request_template;
		public string usernames;
		public string owner_usernames;
		public int? members_visibility_level;
		public bool? can_see_members;
		public bool? publish_read_state;
	}
	public class Group : ApiEntryBase {
		public int id;
		public bool automatic;
		public string name;
		public string display_name;
		public int user_count;
		public int mentionable_level;
		public int messageable_level;
		public int visibility_level;
		public string automatic_membership_email_domains;
		public bool automatic_membership_retroactive;
		public bool primary_group;
		public string title;
		public string grant_trust_level;
		public JToken incoming_email;
		public bool has_messages;
		public string flair_url;
		public JToken flair_bg_color;
		public JToken flair_color;
		public string bio_raw;
		public string bio_cooked;
		public string bio_excerpt;
		public bool public_admission;
		public bool public_exit;
		public bool allow_membership_requests;
		public string full_name;
		public int default_notification_level;
		public JToken membership_request_template;
		public bool is_group_user;
		public bool is_group_owner;
		public bool is_group_owner_display;
		public bool mentionable;
		public bool messageable;
		public int? members_visibility_level;
		public bool? can_see_members;
		public bool? publish_read_state;

		static public async Task<GroupList> ListAll(Api api) {
			JObject data = await api.GetAsync("groups");
			return (GroupList)new GroupList().Convert(data);
		}

		static public async Task<Group> Get(Api api, string name) {
			JObject data = await api.GetAsync(Api.Combine("groups", name));
			return data["group"].ToObject<Group>();
		}

		static public async Task<Group> Create(Api api, string name, string title) {
			UpdateGroupParams data = new UpdateGroupParams() {
				name = name,
				title = title
			};
			return await Create(api, data);
		}

		static public async Task<Group> Create(Api api, UpdateGroupParams data) {
			if (string.IsNullOrEmpty(data.owner_usernames))
				data.owner_usernames = api.Settings.ApiUsername;
			JObject j = new JObject();
			j["group"] = data.ToJObject();
			j = await api.PostAsync(Api.Combine("admin", "groups"), null, j);
			return j["basic_group"].ToObject<Group>();
		}

		static public async Task<JObject> AddUsers(Api api, int groupId, params string[] names) {
			return await api.PutAsync(Api.Combine("groups", groupId, "members"), null, new {
				usernames = string.Join(",", names)
				});
		}

		public async Task<GroupMemberList> GetUsers(Api api, ListRequest request = null) {
			return (GroupMemberList)new GroupMemberList().Convert(await api.GetAsync(Api.Combine("groups", name, "members")));
		}

		public async Task Update(Api api) {
			JObject j = new JObject();
			j["group"] = this.ToJObject();
			await api.PutAsync(Api.Combine("groups", id), null, j);
		}

	}
}
