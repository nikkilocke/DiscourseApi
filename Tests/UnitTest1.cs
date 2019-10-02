using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using DiscourseApi;
using System.IO;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;

namespace Tests {
	public class Settings : DiscourseApi.Settings {
		public bool LoginTests = false;
		public bool ModifyTests = true;
		public bool DestructiveTests = false;
		public string TestUser = "nikki";
		public string TestUser2 = "nikkilocke";
		public int TestCategory = 6;
		public int TestTopic = 8;
		public int TestPost = 11;
		public string TestGroup = "Test";
		public override List<string> Validate() {
			List<string> errors = base.Validate();
#if false
			if (string.IsNullOrEmpty(TestUser))
				errors.Add("TestUser missing");
			if (TestCategory <= 0)
				errors.Add("TestCategory missing");
			if (TestTopic <= 0)
				errors.Add("TestTopic missing");
#endif
			return errors;
		}
	}

	public class TestBase {
		static Settings _settings;
		static Api _api;

		public static Api Api {
			get {
				if (_api == null) {
					_api = new Api(Settings);
				}
				return _api;
			}
		}

		public static Settings Settings {
			get {
				if (_settings == null) {
					string dataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "DiscourseApi");
					Directory.CreateDirectory(dataPath);
					string filename = Path.Combine(dataPath, "TestSettings.json");
					_settings = new Settings();
					_settings.Load(filename);
					List<string> errors = _settings.Validate();
					if (errors.Count > 0)
						throw new ApplicationException(string.Join("\r\n", errors));
				}
				return _settings;
			}
		}


		public static T RunTest<T>(Task<T> task) {
			T t = task.Result;
			Console.WriteLine(t);
			return t;
		}

		public static void RunTest(Task task) {
			task.Wait();
		}

		public static void ShowList<T,L>(Task<L> task) where T:new() where  L : ApiList<T> {
			ApiList<T> result = RunTest(task);
			foreach (T o in result.All(Api))
				Console.WriteLine(o);
		}

		public static void ShowList<T>(Task<List<T>> task) {
			List<T> result = RunTest(task);
			foreach (T o in result)
				Console.WriteLine(o);
		}

	}
	[TestClass]
	public class UserTests : TestBase {
		[TestMethod]
		public void List() {
			foreach (var u in User.ListAll(Api, "active").Result.All(Api))
				Console.WriteLine(u);
		}
		[TestMethod]
		public void GetByName() {
			RunTest(User.GetByName(Api, Settings.TestUser));
		}
	}
	[TestClass]
	public class CategoryTests : TestBase {
		[TestMethod]
		public void List() {
			RunTest(Category.ListAll(Api));
		}
		[TestMethod]
		public void Get() {
			RunTest(Category.Get(Api, Settings.TestCategory));
		}
		[TestMethod]
		public void Create() {
			if (!Settings.ModifyTests)
				return;
			var c = RunTest(Category.Create(Api, new UpdateCategoryParams() {
				name = "Test Sub Category",
				slug = "test-sub",
				parent_category_id = Settings.TestCategory,
				permissions = new Permissions(
					new GroupPermission("Test", PermissionLevel.Create),
					new GroupPermission("trust_level_0", PermissionLevel.See)
					)
			}));
			RunTest(Category.Get(Api, c.id));
			RunTest(Category.Delete(Api, c.id));
		}
	}
	[TestClass]
	public class TopicTests : TestBase {
		[TestMethod]
		public void List() {
			RunTest(Topic.ListAll(Api, Settings.TestCategory));
		}
		[TestMethod]
		public void Get() {
			RunTest(Topic.Get(Api, Settings.TestTopic));
		}
	}
	[TestClass]
	public class PostTests : TestBase {
		[TestMethod]
		public void GetAll() {
			RunTest(Post.GetAll(Api, Settings.TestTopic));
		}
		[TestMethod]
		public void Get() {
			RunTest(Post.Get(Api, Settings.TestPost));
		}
		[TestMethod]
		public void ChangeOwner() {
			Post p = Post.Get(Api, Settings.TestPost).Result;
			string user = p.username == Settings.TestUser ? Settings.TestUser2 : Settings.TestUser;
			RunTest(Topic.ChangePostOwners(Api, p.topic_id, user, p.id));
		}
	}
	[TestClass]
	public class GroupTests : TestBase {
		[TestMethod]
		public void List() {
			ShowList<Group, GroupList>(Group.ListAll(Api));
		}
		[TestMethod]
		public void Get() {
			RunTest(Group.Get(Api, Settings.TestGroup));
		}
	}
}
