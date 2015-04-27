namespace odTimeTracker
{
	using System;
	using NUnit.Framework;

	namespace Model
	{
		[TestFixture()]
		public class ActivityTest
		{
			private Activity BlankActivity;
			private Activity FinishedActivity;
			private Activity RunningActivity;

			[SetUp]
			protected void SetUp()
			{
				BlankActivity = new Activity();
				FinishedActivity = new Activity(1, 1, 
					"Sample activity", 
					"Description of the sample activity.", 
					"tag1,tag2", 
					DateTime.Parse("2015-01-01 01:00:00.000"), 
					DateTime.Parse("2015-01-01 02:00:00.000"));
				RunningActivity = new Activity(2, 1, 
					"Sample running activity", 
					"Some description...", null, 
					DateTime.Parse("2015-01-01 02:15:00.000"));
			}

			[Test]
			public void BlankActivityTest()
			{
				Assert.AreEqual(0, BlankActivity.ActivityId);
				Assert.AreEqual(0, BlankActivity.ProjectId);
				Assert.IsNull(BlankActivity.Name);
				Assert.IsNull(BlankActivity.Description);
				Assert.IsNull(BlankActivity.Tags);
				DateTime TestDateTime = DateTime.UtcNow;
				Assert.AreEqual(TestDateTime.Year, BlankActivity.Created.Year);
				Assert.AreEqual(TestDateTime.Month, BlankActivity.Created.Month);
				Assert.AreEqual(TestDateTime.Day, BlankActivity.Created.Day);
				Assert.AreEqual(TestDateTime.Hour, BlankActivity.Created.Hour);
				Assert.AreEqual(DateTime.Parse("0001-01-01 00:00:00.000"), BlankActivity.Stopped);
			}

			[Test]
			public void FinishedActivityTest()
			{
				Assert.AreEqual(1, FinishedActivity.ActivityId);
				Assert.AreEqual(1, FinishedActivity.ProjectId);
				Assert.AreEqual("Sample activity", FinishedActivity.Name);
				Assert.AreEqual("Description of the sample activity.", FinishedActivity.Description);
				Assert.AreEqual("tag1,tag2", FinishedActivity.Tags);
				Assert.AreEqual(DateTime.Parse("2015-01-01 01:00:00.000"), FinishedActivity.Created);
				Assert.AreEqual(DateTime.Parse("2015-01-01 02:00:00.000"), FinishedActivity.Stopped);
				Assert.AreEqual(3600, FinishedActivity.GetDuration().TotalSeconds);
			}

			[Test]
			public void RunningActivityTest()
			{
				Assert.AreEqual(2, RunningActivity.ActivityId);
				Assert.AreEqual(1, RunningActivity.ProjectId);
				Assert.AreEqual("Sample running activity", RunningActivity.Name);
				Assert.AreEqual("Some description...", RunningActivity.Description);
				Assert.IsNull(RunningActivity.Tags);
				Assert.AreEqual(DateTime.Parse("2015-01-01 02:15:00.000"), RunningActivity.Created);
				Assert.AreEqual(DateTime.Parse("0001-01-01 00:00:00.000"), BlankActivity.Stopped);
			}
		}
	}
}
