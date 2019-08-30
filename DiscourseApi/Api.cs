using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Linq;
using System.IO;

namespace DiscourseApi {
	public class Api : IDisposable {
		HttpClient _client;

		public Api(ISettings settings) {
			Settings = settings;
			_client = new HttpClient(new HttpClientHandler() { AllowAutoRedirect = false });
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
			_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
			_client.DefaultRequestHeaders.Add("User-Agent", Settings.ApplicationName);
		}

		public void Dispose() {
			if (_client != null) {
				_client.Dispose();
				_client = null;
			}
		}

		/// <summary>
		/// The Settings object to use for this Api instance.
		/// </summary>
		public ISettings Settings;

		/// <summary>
		/// Log messages will be passed to this handler
		/// </summary>
		public delegate void LogHandler(string message);

		/// <summary>
		/// Event receives all log messages (to, for example, save them to file or display them to the user)
		/// </summary>
		public event LogHandler LogMessage;

		/// <summary>
		/// Event receives all log messages (to, for example, save them to file or display them to the user)
		/// </summary>
		public event LogHandler ErrorMessage;

		/// <summary>
		/// Post to the Api, returning an object
		/// </summary>
		/// <typeparam name="T">The object type expected</typeparam>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		/// <param name="postParameters">Any post parameters to pass (in an object or JObject)</param>
		public async Task<T> PostAsync<T>(string application, object getParameters = null, object postParameters = null) where T : new() {
			JObject j = await PostAsync(application, getParameters, postParameters);
			if (typeof(ApiList).IsAssignableFrom(typeof(T))) {
				JObject r = (getParameters == null ? (object)new ListRequest() : getParameters).ToJObject();
				r["PostParameters"] = postParameters.ToJObject();
				j["Request"] = r;
			}
			return convertTo<T>(j);
		}

		/// <summary>
		/// Post to the Api, returning a JObject
		/// </summary>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		/// <param name="postParameters">Any post parameters to pass (in an object or JObject)</param>
		public async Task<JObject> PostAsync(string application, object getParameters = null, object postParameters = null) {
			string uri = makeUri(application);
			uri = AddGetParams(uri, getParameters);
			return await SendMessageAsync(HttpMethod.Post, uri, postParameters);
		}

		/// <summary>
		/// Get from  the Api, returning an object
		/// </summary>
		/// <typeparam name="T">The object type expected</typeparam>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		public async Task<T> GetAsync<T>(string application, object getParameters = null) where T : new() {
			JObject j = await GetAsync(application, getParameters);
			if (typeof(ApiList).IsAssignableFrom(typeof(T)))
				j["Request"] = (getParameters == null ? (object)new ListRequest() : getParameters).ToJObject();
			return convertTo<T>(j);
		}

		/// <summary>
		/// Get from  the Api, returning a Jobject
		/// </summary>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		public async Task<JObject> GetAsync(string application, object getParameters = null) {
			string uri = makeUri(application);
			uri = AddGetParams(uri, getParameters);
			return await SendMessageAsync(HttpMethod.Get, uri);
		}

		/// <summary>
		/// Put to  the Api, returning an object
		/// </summary>
		/// <typeparam name="T">The object type expected</typeparam>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		/// <param name="postParameters">Any post parameters to pass (in an object or JObject)</param>
		public async Task<T> PutAsync<T>(string application, object getParameters = null, object postParameters = null) where T : new() {
			JObject j = await PutAsync(application, getParameters, postParameters);
			return convertTo<T>(j);
		}

		/// <summary>
		/// Put to  the Api, returning a JObject
		/// </summary>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		/// <param name="postParameters">Any post parameters to pass (in an object or JObject)</param>
		public async Task<JObject> PutAsync(string application, object getParameters = null, object postParameters = null) {
			string uri = makeUri(application);
			uri = AddGetParams(uri, getParameters);
			return await SendMessageAsync(HttpMethod.Put, uri, postParameters);
		}

		/// <summary>
		/// Delete to  the Api, returning an object
		/// </summary>
		/// <typeparam name="T">The object type expected</typeparam>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		public async Task<T> DeleteAsync<T>(string application, object getParameters = null) where T : new() {
			JObject j = await DeleteAsync(application, getParameters);
			return convertTo<T>(j);
		}

		/// <summary>
		/// Delete to  the Api, returning a JObject
		/// </summary>
		/// <typeparam name="T">The object type expected</typeparam>
		/// <param name="application">The part of the url after the company</param>
		/// <param name="getParameters">Any get parameters to pass (in an object or JObject)</param>
		public async Task<JObject> DeleteAsync(string application, object getParameters = null) {
			string uri = makeUri(application);
			uri = AddGetParams(uri, getParameters);
			return await SendMessageAsync(HttpMethod.Delete, uri);
		}

		/// <summary>
		/// API post using multipart/form-data.
		/// </summary>
		/// <param name="application">The full Uri you want to call (including any get parameters)</param>
		/// <param name="getParameters">Get parameters (or null if none)</param>
		/// <param name="postParameters">Post parameters as an  object or JObject
		/// </param>
		/// <returns>The result as a T Object.</returns>
		public async Task<T> PostFormAsync<T>(string application, object getParameters, object postParameters, params string[] fileParameterNames) where T : new() {
			JObject j = await PostFormAsync(application, getParameters, postParameters, fileParameterNames);
			return convertTo<T>(j);
		}

		/// <summary>
		/// API post using multipart/form-data.
		/// </summary>
		/// <param name="application">The full Uri you want to call (including any get parameters)</param>
		/// <param name="getParameters">Get parameters (or null if none)</param>
		/// <param name="postParameters">Post parameters as an  object or JObject
		/// </param>
		/// <returns>The result as a JObject, with MetaData filled in.</returns>
		public async Task<JObject> PostFormAsync(string application, object getParameters, object postParameters, params string[] fileParameterNames) {
			string uri = AddGetParams(makeUri(application), getParameters);
			using (DisposableCollection objectsToDispose = new DisposableCollection()) {
				MultipartFormDataContent content = objectsToDispose.Add(new MultipartFormDataContent());
				foreach (var o in postParameters.ToCollection()) {
					if (Array.IndexOf(fileParameterNames, o.Key) >= 0) {
						string filename = o.Value;
						FileStream fs = objectsToDispose.Add(new FileStream(filename, FileMode.Open));
						HttpContent v = objectsToDispose.Add(new StreamContent(fs));
						content.Add(v, o.Key, Path.GetFileName(filename));
					} else {
						HttpContent v = objectsToDispose.Add(new StringContent(o.Value));
						content.Add(v, o.Key);
					}
				}
				return await SendMessageAsync(HttpMethod.Post, uri, content);
			}

		}

		/// <summary>
		/// Log a message to trace and, if present, to the LogMessage event handlers
		/// </summary>
		public void Log(string message) {
			message = "Discourse log:" + message;
			System.Diagnostics.Trace.WriteLine(message);
			LogMessage?.Invoke(message);
		}

		/// <summary>
		/// Log a message to trace and, if present, to the ErrorMessage event handlers
		/// </summary>
		public void Error(string message) {
			message = "Discourse error:" + message;
			System.Diagnostics.Trace.WriteLine(message);
			ErrorMessage?.Invoke(message);
		}

		/// <summary>
		/// Combine a list of arguments into a string, with "/" between them (escaping if required)
		/// </summary>
		public static string Combine(params object[] args) {
			return string.Join("/", args.Select(a => Uri.EscapeUriString(a.ToString())));
		}

		static readonly char[] argSplit = new char[] { '=' };

		/// <summary>
		/// Split the query parameters from a uri into a dictionary
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static Dictionary<string, string> QueryParams(string uri) {
			Uri u = new Uri(uri);
			Dictionary<string, string> query = new Dictionary<string, string>();
			foreach (string arg in u.Query.Split('&', '?')) {
				if (string.IsNullOrEmpty(arg)) continue;
				string[] parts = arg.Split(argSplit, 2);
				query[Uri.UnescapeDataString(parts[0])] = parts.Length < 2 ? null : Uri.UnescapeDataString(parts[1]);
			}
			return query;
		}

		/// <summary>
		/// Add or Replace Get Parameters to a uri
		/// </summary>
		/// <param name="parameters">Object whose properties are the arguments - e.g. new {
		/// 		type = "web_server",
		/// 		client_id = Settings.ClientId,
		/// 		redirect_uri = Settings.RedirectUri
		/// 	}</param>
		/// <returns>uri?arg1=value1&amp;arg2=value2...</returns>
		public static string AddGetParams(string uri, object parameters = null) {
			if (parameters != null) {
				Dictionary<string, string> query = QueryParams(uri);
				JObject j = parameters.ToJObject();
				foreach (var v in j) {
					if (v.Value.IsNullOrEmpty())
						query.Remove(v.Key);
					else
						query[v.Key] = v.Value.ToString();
				}
				uri = uri.Split('?')[0] + "?" + string.Join("&", query.Keys.Select(k => Uri.EscapeUriString(k) + "=" + Uri.EscapeUriString(query[k])));
			}
			return uri;
		}

		/// <summary>
		/// General API message sending.
		/// </summary>
		/// <param name="method">Get/Post/etc.</param>
		/// <param name="uri">The full Uri you want to call (including any get parameters)</param>
		/// <param name="postParameters">Post parameters as an :-
		/// object (converted to Json, MIME type application/json)
		/// JObject (converted to Json, MIME type application/json)
		/// string (sent as is, MIME type text/plain)
		/// FileStream (sent as stream, with Attachment file name, Content-Length, and MIME type according to file extension)
		/// </param>
		/// <returns>The result as a JObject, with MetaData filled in.</returns>
		public async Task<JObject> SendMessageAsync(HttpMethod method, string uri, object postParameters = null) {
			using (HttpResponseMessage result = await SendMessageAsyncAndGetResponse(method, uri, postParameters)) {
				return await parseJObjectFromResponse(uri, result);
			}
		}

		DateTime lastRequest = DateTime.MinValue;

		/// <summary>
		/// Send a message and get the result.
		/// Deal with rate limiting return values and redirects.
		/// </summary>
		/// <param name="method">Get/Post/etc.</param>
		/// <param name="uri">The full Uri you want to call (including any get parameters)</param>
		/// <param name="postParameters">Post parameters as an object or JObject</param>
		public async Task<HttpResponseMessage> SendMessageAsyncAndGetResponse(HttpMethod method, string uri, object postParameters = null) {
			for (; ; ) {
				if (Settings.DelayBetweenApiCalls > 0) {
					double elapsedSinceLastRequest = (DateTime.Now - lastRequest).TotalMilliseconds;
					if (elapsedSinceLastRequest < Settings.DelayBetweenApiCalls)
						await Task.Delay((int)(Settings.DelayBetweenApiCalls - elapsedSinceLastRequest));
				}
				lastRequest = DateTime.Now;
				string content = null;
				using (DisposableCollection disposeMe = new DisposableCollection()) {
					var message = disposeMe.Add(new HttpRequestMessage(method, uri));
					if (!string.IsNullOrEmpty(Settings.ApiKey))
						message.Headers.Add("Api-Key", Settings.ApiKey);
					if (!string.IsNullOrEmpty(Settings.ApiUsername))
						message.Headers.Add("Api-Username", Settings.ApiUsername);
					message.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
					message.Headers.Add("User-Agent", Settings.ApplicationName);
					if (postParameters != null) {
						if (postParameters is FileStream f) {
							content = Path.GetFileName(f.Name);
							f.Position = 0;
							message.Content = disposeMe.Add(new StreamContent(f));
							string contentType = MimeMapping.MimeUtility.GetMimeMapping(content);
							message.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
							message.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment") {
								FileName = content
							};
							message.Content.Headers.ContentLength = f.Length;
							content = "File: " + content;
						} else if (postParameters is HttpContent) {
							message.Content = (HttpContent)postParameters;
						} else {
							content = postParameters.ToJson();
							message.Content = disposeMe.Add(new FormUrlEncodedContent(postParameters.ToCollection()));
						}
					}
					HttpResponseMessage result;
					int backoff = 1000;
					int delay;
					if (Settings.LogRequest > 0)
						Log($"Sent -> {(Settings.LogRequest > 1 ? message.ToString() : message.RequestUri.ToString())}:{content}");
					result = await _client.SendAsync(message);
					if (Settings.LogResult > 1)
						Log($"Received -> {result}");
					if (!result.IsSuccessStatusCode) {
						Error($"Message -> {message}:{content}");
						Error($"Response -> {result}");
					}
					switch (result.StatusCode) {
						case HttpStatusCode.Found:      // Redirect
							uri = result.Headers.Location.AbsoluteUri;
							delay = 1;
							break;
						case (HttpStatusCode)429:       // TooManyRequests
							IEnumerable<string> values;
							delay = 5000;
							if (result.Headers.TryGetValues("Retry-After", out values)) {
								try {
									int d = 1000 * int.Parse(values.FirstOrDefault());
									if (d > delay)
										delay = d;
								} catch {
								}
							}
							break;
						case HttpStatusCode.BadGateway:
						case HttpStatusCode.ServiceUnavailable:
						case HttpStatusCode.GatewayTimeout:
							backoff *= 2;
							delay = backoff;
							if (delay > 16000)
								return result;
							break;
						default:
							return result;
					}
					result.Dispose();
					await Task.Delay(delay);
				}
			}
		}

		/// <summary>
		/// Build a JObject from a response
		/// </summary>
		/// <param name="uri">To store in the MetaData</param>
		async Task<JObject> parseJObjectFromResponse(string uri, HttpResponseMessage result) {
			JObject j = null;
			string data = await result.Content.ReadAsStringAsync();
			if (data.StartsWith("{")) {
				j = JObject.Parse(data);
			} else if (data.StartsWith("[")) {
				j = new JObject {
					["List"] = JArray.Parse(data)
				};
			} else {
				j = new JObject();
				if (!string.IsNullOrEmpty(data))
					j["content"] = data;
			}
			JObject metadata = new JObject();
			metadata["Uri"] = uri;
			IEnumerable<string> values;
			if (result.Headers.TryGetValues("Last-Modified", out values)) metadata["Modified"] = values.FirstOrDefault();
			j["MetaData"] = metadata;
			if (Settings.LogResult > 0 || !result.IsSuccessStatusCode)
				Log("Received Data -> " + j);
			if (!result.IsSuccessStatusCode)
				throw new ApiException(result.ReasonPhrase, j);
			return j;
		}

		/// <summary>
		/// Convert a JObject to an Object.
		/// If it is an ApiEntry, and error is not empty, throw an exception.
		/// </summary>
		/// <typeparam name="T">Object to convert to</typeparam>
		static T convertTo<T>(JObject j) where T : new() {
			T t = j.ConvertToObject<T>();
			if (t is ApiEntry e && e.Error)
				throw new ApiException(e.MetaData.Error.message, j);
			return t;
		}

		static readonly Regex _http = new Regex("^https?://");

		/// <summary>
		/// Make the standard Uri (put BaseUri and CompanyId on the front)
		/// </summary>
		/// <param name="application">The remainder of the Uri</param>
		protected string makeUri(string application) {
			return _http.IsMatch(application) ? application : Settings.ServerUri + application;
		}

	}

	/// <summary>
	/// Exception to hold more information when an API call fails
	/// </summary>
	public class ApiException : ApplicationException {
		static string getMessage(string message, JObject result) {
			if(result != null) {
				JToken errors = result["errors"];
				if(errors != null && errors.Type == JTokenType.Array)
					message += ":" + string.Join(";", ((JArray)errors).Select(j => j.ToString()));
			}
			return message;
		}
		public ApiException(string message, JObject result) : base(getMessage(message, result)) {
			Result = result;
		}
		public JObject Result { get; private set; }
		public override string ToString() {
			return base.ToString() + "\r\nResult = " + Result;
		}
	}
}
