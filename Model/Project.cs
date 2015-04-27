namespace odTimeTracker
{
	using System;

	namespace Model
	{
		public class Project
		{
			public long ProjectId { get; set; }
			public string Name { get; set; }
			public string Description { get; set; }
			public DateTime Created { get; set; }

			public Project()
			{
				Created = DateTime.UtcNow;
			}

			public Project(long projectId, string name, string description)
			{
				ProjectId = projectId;
				Name = name;
				Description = description;
				Created = DateTime.UtcNow;
			}

			public Project(long projectId, string name, string description, DateTime created)
			{
				ProjectId = projectId;
				Name = name;
				Description = description;
				Created = created;
			}
			
			// XXX Add own ToString() method!
		}
	}
}
