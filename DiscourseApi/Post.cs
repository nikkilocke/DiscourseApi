using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiscourseApi {
	public class ActionsSummary : ApiEntryBase {
		public int id;
		public int count;
		public bool hidden;
		public bool can_act;
	}

	public class Post : ApiEntryBase {
		public int id;
		public string name;
		public string username;
		public string avatar_template;
		public DateTime created_at;
		public string cooked;
		public int post_number;
		public int post_type;
		public DateTime updated_at;
		public int reply_count;
		public JToken reply_to_post_number;
		public int quote_count;
		public int incoming_link_count;
		public int reads;
		public double score;
		public bool yours;
		public int topic_id;
		public string topic_slug;
		public string display_username;
		public JToken primary_group_name;
		public JToken primary_group_flair_url;
		public JToken primary_group_flair_bg_color;
		public JToken primary_group_flair_color;
		public int version;
		public bool can_edit;
		public bool can_delete;
		public JToken can_recover;
		public bool can_wiki;
		public List<LinkCount> link_counts;
		public JToken user_title;
		public string raw;
		public List<ActionsSummary> actions_summary;
		public bool moderator;
		public bool admin;
		public bool staff;
		public int user_id;
		public bool hidden;
		public int trust_level;
		public JToken deleted_at;
		public bool user_deleted;
		public JToken edit_reason;
		public bool can_view_edit_history;
		public bool wiki;
		public JToken reviewable_id;
		public int reviewable_score_count;
		public int reviewable_score_pending_count;

		public static async Task<PostStream> GetAll(Api api, int topicId) {
			JObject j = await api.GetAsync(Api.Combine("t", topicId, "posts"));
			return j["post_stream"].ToObject<PostStream>();
		}

		public static async Task<Post> Get(Api api, int postId) {
			return await api.GetAsync<Post>(Api.Combine("posts", postId));
		}

		public static async Task<Post> Create(Api api, int topicId, string message,
				string api_username = null, DateTime? created_at = null) {
			return await api.PostAsync<Post>("posts", null, new {
				topic_id = topicId,
				raw = message,
				api_username,
				created_at
			});
		}

		public static async Task<Post> Update(Api api, int postId, string message) {
			JObject data = new JObject();
			data["post[raw]"] = message;
			return await api.PutAsync<Post>(Api.Combine("posts", postId), null, data);
		}
    }
}