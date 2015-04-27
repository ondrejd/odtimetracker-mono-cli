namespace odTimeTracker
{
	using System;
	using System.IO;
	using Mono.Data.Sqlite;
	using odTimeTracker.Model;

	namespace Storage
	{
		public class SqliteStorage : odTimeTracker.Storage.IStorage
		{
			/// <summary>Connection to our SQLite database.</summary>
			private SqliteConnection Connection;

			public static string DocumentsDirPath {	
				get { 
					return Environment.GetFolderPath(Environment.SpecialFolder.Personal); 
				}
			}

			public static string DatabaseFilePath { 
				get { 
					return Path.Combine(DocumentsDirPath, ".odtimetracker.sqlite"); 
				}
			}

			// ===================================================================================

			public SqliteStorage()
			{
			}

			~SqliteStorage ()
			{
				// Ensure that database connection is properly closed
				if (Connection != null)
				{
					try 
					{
						if (Connection.State == System.Data.ConnectionState.Open)
						{
							Connection.Close();
						}
					} 
					catch (SqliteException ex)
					{ 
						//PrintLine ("Closing connection failed.", ConsoleColor.Red);
						Console.WriteLine("Error: {0}", ex.ToString());
					} 
					finally
					{
						Connection.Dispose();
					}
				}
			}

			// ===================================================================================

			/// <summary>Executes the non query SQL.</summary>
			/// <param name="SqlString">SQL string.</param>
			private void ExecuteNonQuerySql(string SqlString)
			{
				SqliteCommand Cmd = null;

				try
				{
					Cmd = Connection.CreateCommand();
					Cmd.CommandText = SqlString;
					Cmd.CommandType = System.Data.CommandType.Text;
					Cmd.ExecuteNonQuery();
				} 
				catch (SqliteException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				} 
				finally
				{
					Cmd.Dispose();
				}
			}

			/// <summary>Creates the database schema.</summary>
			private void CreateDatabaseSchema()
			{
				// Enable foreign keys
				ExecuteNonQuerySql("PRAGMA foreign_keys = ON; ");
				// Tables
				ExecuteNonQuerySql(
					"CREATE TABLE Projects (" +
					"ProjectId INTEGER PRIMARY KEY, " +
					"Name TEXT, " +
					"Description TEXT, " +
					"Created TEXT NOT NULL " + 
					")"
				);
				ExecuteNonQuerySql(
					"CREATE TABLE Activities (" +
					"ActivityId INTEGER PRIMARY KEY, " +
					"ProjectId INTEGER NOT NULL, " +
					"Name TEXT, " +
					"Description TEXT, " +
					"Tags TEXT, " +
					"Created TEXT NOT NULL, " +
					"Stopped TEXT, " +
					"FOREIGN KEY(ProjectId) REFERENCES Projects(ProjectId) " + 
					")"
				);

				// Default project
				Project defaultProject = new Project();
				defaultProject.Name = "Default project";
				defaultProject.Description = "Default blank project.";
				defaultProject.Created = DateTime.UtcNow;
				InsertProject(defaultProject);
			}

			// ===================================================================================

			/// <summary>Initialize storage.</summary>
			public bool Initialize()
			{
				string DbFile = DatabaseFilePath;
				bool exists = File.Exists(DbFile);

				if (!exists)
				{
					SqliteConnection.CreateFile(DbFile);
				}

				Connection = new SqliteConnection("Data Source=" + DbFile);
				Connection.Open();

				if (!exists)
				{
					try
					{
						CreateDatabaseSchema();
					} 
					catch (SqliteException ex)
					{
						Console.WriteLine("Error: {0}", ex.ToString());
						return false;
					}
				}

				return true;
			}

			/// <summary>Retrieves currently running activity</summary>
			/// <returns>Currently running activity; otherwise returns <c>Null</c>.</returns>
			public Activity[] GetRunningActivity()
			{
				Activity[] Ret = new Activity[1];

				string sql = "SELECT * " + 
					"FROM Activities WHERE Stopped = '0001-01-01 00:00:00' LIMIT 1 ";

				using (SqliteCommand cmd = new SqliteCommand(sql, Connection))
				{
					using (SqliteDataReader rdr = cmd.ExecuteReader())
					{
						while (rdr.Read()) 
						{
							Activity a = new Activity();
							a.ActivityId = (long) rdr["ActivityId"];
							a.ProjectId = (long) rdr["ProjectId"];
							a.Name = (string) rdr["Name"];
							a.Description = (string) rdr["Description"];
							a.Created = DateTime.Parse((string) rdr["Created"]);
							a.Stopped = DateTime.Parse((string) rdr["Stopped"]);
							Ret[0] = a;
						}
					}         
				}

				return Ret;
			}

			/// <summary>Inserts new activity into the database.</summary>
			/// <returns>Activity with properly set <c>ActivityID</c>.</returns>
			/// <param name="activity">New activity.</param>
			public Activity InsertActivity(Activity activity)
			{
				SqliteCommand InsertCmd = null;

				try
				{
					InsertCmd = Connection.CreateCommand();
					InsertCmd.CommandType = System.Data.CommandType.Text;
					InsertCmd.CommandText = "INSERT INTO Activities " + 
						"(ProjectId, Name, Description, Tags, Created, Stopped) " + 
						"VALUES " + 
						"(@ProjectId, @Name, @Description, @Tags, @Created, @Stopped) ";
					InsertCmd.Prepare();
					InsertCmd.Parameters.AddWithValue("@ProjectId", activity.ProjectId);
					InsertCmd.Parameters.AddWithValue("@Name", activity.Name);
					InsertCmd.Parameters.AddWithValue("@Description", activity.Description);
					InsertCmd.Parameters.AddWithValue("@Tags", activity.Tags);
					InsertCmd.Parameters.AddWithValue("@Created", activity.Created);
					InsertCmd.Parameters.AddWithValue("@Stopped", activity.Stopped);
					InsertCmd.ExecuteNonQuery();

					InsertCmd.CommandText = "select last_insert_rowid()";
					long activityId = (long) InsertCmd.ExecuteScalar();

					activity.ActivityId = activityId;
				} 
				catch (SqliteException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				} 
				finally
				{
					if (InsertCmd != null)
					{
						InsertCmd.Dispose();
					}
				}

				return activity;
			}

			/// <summary>Inserts new project into the database.</summary>
			/// <returns>Project with properly set <c>ProjectID</c>.</returns>
			/// <param name="project">New project.</param>
			public Project InsertProject(Project project)
			{
				SqliteCommand InsertCmd = null;

				try
				{
					InsertCmd = Connection.CreateCommand();
					InsertCmd.CommandType = System.Data.CommandType.Text;
					InsertCmd.CommandText = "INSERT INTO Projects (Name, Description, Created) " + 
						"VALUES (@Name, @Description, @Created) ";
					InsertCmd.Prepare();
					InsertCmd.Parameters.AddWithValue("@Name", project.Name);
					InsertCmd.Parameters.AddWithValue("@Description", project.Description);
					InsertCmd.Parameters.AddWithValue("@Created", project.Created);
					InsertCmd.ExecuteNonQuery();

					InsertCmd.CommandText = "select last_insert_rowid()";
					long projectId = (long) InsertCmd.ExecuteScalar();

					project.ProjectId = projectId;
				} 
				catch (SqliteException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				} 
				finally
				{
					if (InsertCmd != null)
					{
						InsertCmd.Dispose();
					}
				}

				return project;
			}

			/// <summary>Selects project by the name.</summary>
			/// <returns>Project with given name.</returns>
			/// <param name="name">Name of the project.</param>
			public Project[] SelectProjectByName(string name)
			{
				Project[] Ret = new Project[1];

				string sql = "SELECT * FROM Projects WHERE Name = '" + name + "' LIMIT 1; ";

				using (SqliteCommand cmd = new SqliteCommand(sql, Connection))
				{
					using (SqliteDataReader rdr = cmd.ExecuteReader())
					{
						while (rdr.Read()) 
						{
							Project p = new Project();
							p.ProjectId = (long) rdr["ProjectId"];
							p.Name = (string) rdr["Name"];
							p.Description = rdr["Description"].ToString();
							p.Created = DateTime.Parse((string) rdr["Created"]);
							Ret[0] = p;
						}
					}         
				}

				return Ret;
			}

			/// <summary>Stops the activity (sets <c>Stopped</c> and updates database record).</summary>
			/// <returns>Updated activity.</returns>
			/// <param name="activity">Activity to stop.</param>
			public Activity StopActivity(Activity activity)
			{
				DateTime StoppedTime = DateTime.UtcNow;
				SqliteCommand InsertCmd = null;

				try
				{
					InsertCmd = Connection.CreateCommand();
					InsertCmd.CommandType = System.Data.CommandType.Text;
					InsertCmd.CommandText = "UPDATE Activities SET Stopped = @Stopped " + 
						"WHERE ActivityId = @ActivityId LIMIT 1 ";
					InsertCmd.Prepare();
					InsertCmd.Parameters.AddWithValue("@Stopped", StoppedTime);
					InsertCmd.Parameters.AddWithValue("@ActivityId", activity.ActivityId);
					InsertCmd.ExecuteNonQuery();
				} 
				catch (SqliteException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
				} 
				finally
				{
					if (InsertCmd != null)
					{
						InsertCmd.Dispose();
					}
				}

				activity.Stopped = StoppedTime;

				return activity;
			}
		}
	}
}
