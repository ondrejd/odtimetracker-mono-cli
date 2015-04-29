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

			/// <summary>
			/// Initializes a new instance of the <see cref="odTimeTracker.Model.Project"/> class 
			/// as a blank project.
			/// </summary>
			public Project()
			{
				Created = DateTime.UtcNow;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="odTimeTracker.Model.Project"/> class 
			/// with basic data.
			/// </summary>
			/// <param name="projectId">Project identifier.</param>
			/// <param name="name">Name of the project.</param>
			/// <param name="description">Description of the project.</param>
			public Project(long projectId, string name, string description)
			{
				ProjectId = projectId;
				Name = name;
				Description = description;
				Created = DateTime.UtcNow;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="odTimeTracker.Model.Project"/> class 
			/// with all data.
			/// </summary>
			/// <param name="projectId">Project identifier.</param>
			/// <param name="name">Name of the project.</param>
			/// <param name="description">Description of the project.</param>
			/// <param name="created">Datetime of project's creation.</param>
			public Project(long projectId, string name, string description, DateTime created)
			{
				ProjectId = projectId;
				Name = name;
				Description = description;
				Created = created;
			}
		}
	}
}
