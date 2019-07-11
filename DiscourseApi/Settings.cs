using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace DiscourseApi {
	public interface ISettings {
		/// <summary>
		/// The Uri of the server (e.g. https://localhost:3000/)
		/// </summary>
		Uri ServerUri { get; }

		/// <summary>
		/// Application Name is required by the api
		/// </summary>
		string ApplicationName { get; }

		/// <summary>
		/// Api Key from the admin panel.
		/// </summary>
		string ApiKey { get; }

		/// <summary>
		/// Api Username for authentication
		/// </summary>
		string ApiUsername { get; }

		/// <summary>
		/// Set to greater than zero to log all requests going to Basecamp. 
		/// Larger numbers give more verbose logging.
		/// </summary>
		int LogRequest { get; }

		/// <summary>
		/// Set greater than zero to log all replies coming from Basecamp. 
		/// Larger numbers give more verbose logging.
		/// </summary>
		int LogResult { get; }

		/// <summary>
		/// After BaseCampApi has update tokens, save the infomation.
		/// </summary>
		void Save();

	}

	public class Settings : ISettings {
		/// <summary>
		/// The Uri of the server (e.g. https://localhost:8065/)
		/// </summary>
		public Uri ServerUri { get; set; }
		/// <summary>
		/// Application Name is required by the api
		/// </summary>
		public string ApplicationName { get; set; }
		/// <summary>
		/// Api Key from the admin panel.
		/// </summary>
		public string ApiKey { get; set; }

		/// <summary>
		/// Api Username for authentication
		/// </summary>
		public string ApiUsername { get; set; }

		/// <summary>
		/// Set to greater than zero to log all requests going to Basecamp. 
		/// Larger numbers give more verbose logging.
		/// </summary>
		public int LogRequest { get; set; }

		/// <summary>
		/// Set greater than zero to log all replies coming from Basecamp. 
		/// Larger numbers give more verbose logging.
		/// </summary>
		public int LogResult { get; set; }

		[JsonIgnore]
		public string Filename;

		/// <summary>
		/// Load a Settings object from LocalApplicationData/BaseCampApi/Settings.json
		/// </summary>
		public static Settings Load() {
			string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DiscourseApi");
			Directory.CreateDirectory(dataPath);
			string filename = Path.Combine(dataPath, "Settings.json");
			Settings settings = new Settings();
			settings.Load(filename);
			return settings;
		}

		/// <summary>
		/// Load a Settings object from the supplied json file
		/// </summary>
		public virtual void Load(string filename) {
			if (File.Exists(filename))
				using (StreamReader s = new StreamReader(filename))
					JsonConvert.PopulateObject(s.ReadToEnd(), this);
			this.Filename = filename;
		}

		/// <summary>
		/// Save updated settings back where they came from
		/// </summary>
		public virtual void Save() {
			Directory.CreateDirectory(Path.GetDirectoryName(Filename));
			using (StreamWriter w = new StreamWriter(Filename))
				w.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented));
		}

		/// <summary>
		/// Check the Settings for missing data.
		/// If you derive from this class you can override this method to add additional checks.
		/// </summary>
		/// <returns>List of error strings - empty if no missing data</returns>
		public virtual List<string> Validate() {
			List<string> errors = new List<string>();
			if (ServerUri == null) {
				errors.Add("ServerUri missing");
			}
			if (string.IsNullOrEmpty(ApplicationName)) {
				errors.Add("ApplicationName missing");
			}
			if (ApiKey == null) {
				errors.Add("ApiKey missing");
			}
			if (string.IsNullOrEmpty(ApiUsername)) {
				errors.Add("ApiUserName missing");
			}
			return errors;
		}
	}
}
