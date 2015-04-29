namespace odTimeTracker
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;
	using odTimeTracker.Model;
	using odTimeTracker.Storage;

	namespace Storage
	{
		[TestFixture]
		public class SqliteStorageTest
		{
			[Test]
			public void InitializeTest()
			{
				SqliteStorage Storage = new SqliteStorage("Data Source=:memory:", true);
				bool Status = Storage.Initialize();

				Assert.IsTrue(Status);
			}

			[Test]
			public void InsertTest()
			{
				SqliteStorage Storage = new SqliteStorage("Data Source=:memory:", true);
				Storage.Initialize();

				List<Project> lp = Storage.SelectProjects();
				Assert.AreEqual(1, lp.Count);

				// Insert old activity without own project
				Activity a1 = new Activity();
				a1.Name = "Test activity";
				a1.Created = DateTime.Parse("2015-01-01 01:00:00");
				a1.Stopped = DateTime.Parse("2015-01-01 01:30:00");

				Assert.AreEqual(0, a1.ActivityId);

				a1 = Storage.InsertActivity(a1);

				Assert.AreEqual(1, a1.ActivityId);

				//Assert.AreNotEqual(a1.ActivityId, au1.ActivityId);
				Assert.AreEqual(1, a1.ProjectId);
				Assert.AreEqual("Test activity", a1.Name);
				Assert.AreEqual("", a1.Description);
				Assert.AreEqual("", a1.Tags);
				Assert.AreEqual(DateTime.Parse("2015-01-01 01:00:00"), a1.Created);
				Assert.AreEqual(DateTime.Parse("2015-01-01 01:30:00"), a1.Stopped);
				Assert.IsFalse(a1.IsRunning());

				// Insert running activity with project (should be created).
				Activity a2 = new Activity("Second test activity@Test project", Storage);

				Assert.AreEqual(0, a2.ActivityId);

				a2 = Storage.InsertActivity(a2);

				Assert.AreEqual(2, a2.ActivityId);
				Assert.IsTrue(a2.IsRunning());
			}
		}
	}
}
