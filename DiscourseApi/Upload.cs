using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiscourseApi {
	public class Upload : ApiEntryBase {
		public int id;
		public string url;
		public string original_filename;
		public int filesize;
		public int width;
		public int height;
		public int thumbnail_width;
		public int thumbnail_height;
		public string extension;
		public string short_url;
		public JToken retain_hours;
		public string human_filesize;

		static public async Task<Upload> Create(Api api, int user_id, string filename, string type = "composer", bool synchronous = true) {
			return await api.PostFormAsync<Upload>("uploads", null, new {
				type,
				user_id,
				synchronous,
				files = new string[] { filename }
			}, "files[]");
		}
	}
}
