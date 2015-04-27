namespace odTimeTracker
{
	using System;
	using System.Collections.Generic;
	using odTimeTracker.Model;

	namespace Storage
	{
		public interface IStorage
		{
			bool Initialize();

			Activity[] GetRunningActivity();

			Activity InsertActivity(Activity activity);
			Project InsertProject(Project project);

			//Activity UpdateActivity(Activity activity);
			//Project UpdateProject(Project project);

			//void RemoveActivity(Activity activity);
			//void RemoveProject(Project project);

			//Activity[] SelectActivityById();

			/// <summary>Selects latest five activities (just temporary).</summary>
			/// <returns>Latest five activities.</returns>
			List<Activity> SelectActivities();

			/// <summary>Selects project by the name.</summary>
			/// <returns>Project with given name.</returns>
			/// <param name="name">Name of the project.</param>
			Project[] SelectProjectByName(string name);

			/// <summary>Selects all projects.</summary>
			/// <returns>All projects.</returns>
			List<Project> SelectProjects();

			/// <summary>Stops the activity (sets <c>Stopped</c> and updates database record).</summary>
			/// <returns>Updated activity.</returns>
			/// <param name="activity">Activity to stop.</param>
			Activity StopActivity(Activity activity);
		}
	}
}
