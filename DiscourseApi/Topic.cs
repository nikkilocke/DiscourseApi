using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscourseApi {
	public class TopicUser : ApiEntryBase {
		public int id;
		public string username;
		public string name;
		public string avatar_template;
	}

	public class TopicList : ApiEntryBase {
		public bool can_create_topic;
		public JToken draft;
		public string draft_key;
		public int draft_sequence;
		public int per_page;
		public List<Topic> topics;
	}

	public class TopicListReturn : ApiList<Topic> {
		public List<TopicUser> users;
		public JToken[] primary_groups;
		public TopicList topic_list;
		override public int RetrievedCount { get { return perPage * page; } }
		override public string NextPageUrl {
			get {
				if (!HasMoreData)
					return null;
				return Api.AddGetParams(MetaData.Uri, new {
					page = page + 1
				});
			}
			set { }
		}
		/// <summary>
		/// There is data on the server we haven't fetched yet
		/// </summary>
		override public bool HasMoreData {
			get { return List.Count == perPage; }
		}

		public override ApiList<Topic> Convert(JObject j) {
			TopicListReturn result = j.ConvertToObject<TopicListReturn>();
			if (Api.QueryParams(result.MetaData.Uri).TryGetValue("page", out string p))
				result.page = int.Parse(p);
			result.perPage = result.topic_list.per_page;
			result.List = result.topic_list.topics;
			return result;
		}
	}

	public class LinkCount : ApiEntryBase {
		public string url;
		[JsonProperty(PropertyName = "internal")]
		public bool Internal;
		public bool reflection;
		public int clicks;
	}

	public class PostStream : ApiEntryBase {
		public List<Post> posts;
		public int[] stream;
	}

	public class SuggestedTopic : ApiEntryBase {
		public int id;
		public string title;
		public string fancy_title;
		public string slug;
		public int posts_count;
		public int reply_count;
		public int highest_post_number;
		public string image_url;
		public DateTime created_at;
		public DateTime last_posted_at;
		public bool bumped;
		public DateTime bumped_at;
		public bool unseen;
		public bool pinned;
		public bool unpinned;
		public string excerpt;
		public bool visible;
		public bool closed;
		public bool archived;
		public bool bookmarked;
		public bool liked;
		public string archetype;
		public int like_count;
		public int views;
		public int category_id;
		public JToken featured_link;
		public List<JToken> posters;
	}

	public class Participants : ApiEntryBase {
		public int id;
		public string username;
		public string name;
		public string avatar_template;
		public int post_count;
		public JToken primary_group_name;
		public JToken primary_group_flair_url;
		public JToken primary_group_flair_color;
		public JToken primary_group_flair_bg_color;
	}

	public class Created_by : ApiEntryBase {
		public int id;
		public string username;
		public string name;
		public string avatar_template;
	}

	public class Last_poster : ApiEntryBase {
		public int id;
		public string username;
		public string name;
		public string avatar_template;
	}

	public class Details : ApiEntryBase {
		public int notification_level;
		public bool can_move_posts;
		public bool can_edit;
		public bool can_delete;
		public bool can_remove_allowed_users;
		public bool can_create_post;
		public bool can_reply_as_new_topic;
		public bool can_flag_topic;
		public bool can_convert_topic;
		public bool can_review_topic;
		public int can_remove_self_id;
		public List<Participants> participants;
		public Created_by created_by;
		public Last_poster last_poster;
	}

	public class TopicCommon : ApiEntryBase {
		public int id;
		public string title;
		public string fancy_title;
		public int posts_count;
		public DateTime created_at;
		public int views;
		public int reply_count;
		public int like_count;
		public DateTime last_posted_at;
		public bool visible;
		public bool closed;
		public bool archived;
		public bool has_summary;
		public string archetype;
		public string slug;
		public int category_id;
		public JToken featured_link;
		public bool pinned_globally;
		public bool unpinned;
		public bool pinned;
		public int highest_post_number;
		public bool bookmarked;
	}

	public class FullTopic : Topic {
		public PostStream post_stream;
		public JToken[] timeline_lookup;
		public List<SuggestedTopic> suggested_topics;
		public int word_count;
		public DateTime deleted_at;
		public int user_id;
		public DateTime pinned_at;
		public DateTime pinned_until;
		public JToken draft;
		public string draft_key;
		public int draft_sequence;
		public int current_post_number;
		public JToken deleted_by;
		public bool has_deleted;
		public List<ActionsSummary> actions_summary;
		public int chunk_size;
		public JToken topic_timer;
		public JToken private_topic_timer;
		public int message_bus_last_id;
		public int participant_count;
		public Details details;
	}

	public class Topic : TopicCommon {
		public JToken image_url;
		public bool bumped;
		public DateTime bumped_at;
		public bool unseen;
		public string excerpt;
		public bool liked;
		public string last_poster_username;
		public List<JToken> posters;

		static public async Task<TopicListReturn> ListAll(Api api, int categoryId, bool includeSubCategories = false, int page = 0) {
			JObject j = new JObject();
			j["page"] = page;
			if (!includeSubCategories)
				j["no_subcategories"] = "true";
			JObject data = await api.GetAsync(Api.Combine("c", categoryId), j);
			return (TopicListReturn)new TopicListReturn().Convert(data);
		}

		static public async Task<Post> Create(Api api, int categoryId, string title, string message, 
				string api_username = null, DateTime? created_at = null) {
			return await api.PostAsync<Post>("posts", null, new {
				title,
				category = categoryId,
				raw = message,
				api_username,
				created_at
			});
		}

		static public async Task<FullTopic> Get(Api api, int topicId) {
			return await api.GetAsync<FullTopic>(Api.Combine("t", topicId));
		}

		static public async Task<JObject> Update(Api api, int topicId, string title, int? categoryId = null) {
			return await api.PutAsync(Api.Combine("t", "-", topicId), null, new {
				title,
				category_id = categoryId
			});
		}

		static public async Task<JObject> ChangePostOwners(Api api, int topicId, string username, params int [] post_ids) {
			return await api.PostAsync(Api.Combine("t", topicId, "change-owner"), null, new {
				username,
				post_ids
			});
		}

		static public async Task Delete(Api api, int topicId) {
			await api.DeleteAsync(Api.Combine("t", topicId));
		}

	}


}
