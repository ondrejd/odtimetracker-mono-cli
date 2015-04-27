namespace odTimeTracker
{
	using System;
	using NUnit.Framework;

	namespace Model
	{
		[TestFixture()]
		public class ProjectTest
		{
			private Project BlankProject;
			private Project SampleProject;

			[SetUp]
			protected void SetUp()
			{
				BlankProject = new Project();
				SampleProject = new Project(1, "Sample project", 
					"Description of the sample project", 
					DateTime.Parse("2015-01-01 01:00:00.000"));
			}

			[Test]
			public void BlankProjectTest()
			{
				Assert.AreEqual(0, BlankProject.ProjectId);
				Assert.IsNull(BlankProject.Name);
				Assert.IsNull(BlankProject.Description);
				DateTime TestDateTime = DateTime.UtcNow;
				Assert.AreEqual(TestDateTime.Year, BlankProject.Created.Year);
				Assert.AreEqual(TestDateTime.Month, BlankProject.Created.Month);
				Assert.AreEqual(TestDateTime.Day, BlankProject.Created.Day);
				Assert.AreEqual(TestDateTime.Hour, BlankProject.Created.Hour);
			}

			[Test]
			public void SampleProjectTest()
			{
				Assert.AreEqual(1, SampleProject.ProjectId);
				Assert.AreSame("Sample project", SampleProject.Name);
				Assert.AreSame("Description of the sample project", SampleProject.Description);
				Assert.AreEqual(DateTime.Parse("2015-01-01 01:00:00.000"), SampleProject.Created);
			}
		}
	}
}
