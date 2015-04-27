// TODO Chybejici konfiguracni soubor nesmi nicemu branit, proste se bude fungovat bez nej s defaultni lokaci pro Sqlite databazi.
// TODO Pokud je v konfiguracnim souboru uvedena cesta k Sqlite databazi, tak ji musime pouzit.
// TODO V konfiguraci take muze byt nastaveni pro zapnuti/vypnuti barevneho vystupu bez nutnosti pouzit prepinace.

namespace odTimeTracker
{
	using System;
	using System.IO;
	using Mono.Data.Sqlite;
	using odTimeTracker.Model;
	using odTimeTracker.Storage;

	class MainClass
	{
		// Flags
		private static bool ColoredOutput = false;

		// Command and its value
		private static string Command;
		private static string CommandValue;

		public static string HomeDirPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile); } }

		public static string ConfigFilePath { get { return Path.Combine(HomeDirPath, ".odtimetracker.conf"); } }

		/// <summary>Instance of used storage.</summary>
		private static SqliteStorage storage;
		public static SqliteStorage Storage {
			get {
				if (storage == null)
				{
					storage = new odTimeTracker.Storage.SqliteStorage();
					storage.Initialize();
				}

				return storage;
			}
		}

		/// <summary>
		/// Asks the yes no question.
		/// </summary>
		/// <returns><c>true</c>, if user entered "y", <c>false</c> otherwise.</returns>
		/// <param name="question">Question to ask.</param>
		private static bool AskYesNoQuestion(string question)
		{
			PrintLine(question + " (y/n)");
			string res = Console.ReadKey().KeyChar.ToString();

			// Move console cursor back left
			Console.SetCursorPosition(Console.CursorLeft - 1, Console.CursorTop);

			return (res == "y");
		}

		/// <summary>
		/// Checks if configuration file exists.
		/// </summary>
		/// <returns><c>true</c>, if config file exists, <c>false</c> otherwise.</returns>
		public static bool CheckIfConfigFileExists()
		{
			return File.Exists(ConfigFilePath);
		}

		/// <summary>
		/// Prints the line to the console.
		/// </summary>
		/// <param name="msg">Message self</param>
		/// <param name="col">Color of the output</param>
		/// <param name="nl">If set to <c>true</c> nl.</param>
		private static void PrintLine(string msg,
		                              ConsoleColor col = ConsoleColor.Gray,
		                              bool nl = false)
		{
			if (ColoredOutput == true)
			{
				Console.ForegroundColor = col;
			}

			Console.WriteLine(msg);

			if (ColoredOutput == true)
			{
				Console.ResetColor();
			}

			if (nl == true)
			{
				Console.WriteLine();
			}
		}

		/// <summary>
		/// Prints the wrong arguments message.
		/// </summary>
		private static void PrintWrongArgumentsMessage()
		{
			PrintLine("Wrong arguments passed - try `help` argument.", ConsoleColor.Red, true);
		}

		/// <summary>
		/// Prints activity running time.
		/// </summary>
		/// <param name="duration">Duration.</param>
		private static void PrintRunningTime(TimeSpan duration)
		{
			string Output = "Running time:";

			int Hours = duration.Hours;
			if (Hours > 0)
			{
				Output += " " + Hours + " h";
			}

			int Mins = duration.Minutes;
			if (Mins > 0)
			{
				Output += " " + Mins + " m";
			}

			int Secs = duration.Seconds;
			if (Secs > 0)
			{
				Output += " " + Secs + " s";
			}

			PrintLine(Output, ConsoleColor.Green, true);
		}

		/// <summary>
		/// Processes the arguments.
		/// </summary>
		/// <param name="args">Arguments passed to the program.</param>
		private static void ProcessArguments(string[] args)
		{
			foreach (string arg in args)
			{
				// Flag: [-c|--colors]
				if (arg == "-c" || arg == "--colors")
				{
					ColoredOutput = true;
				}
				// Commands: [info|install|help|start|stop]
				else if (arg == "info" || arg == "install" || arg == "help" || arg == "start" || arg == "stop")
				{
					if (Command == "" || Command == null)
					{
						Command = arg;
					}
				}
				// Get <topic> or <activity>
				else if (Command == "help" || Command == "start")
				{
					CommandValue = arg;
				}
			}
		}

		//
		// ==================================================================================
		//

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main(string[] args)
		{
			// No arguments - print error message
			if (args.Length == 0 || args.Length > 4)
			{
				PrintWrongArgumentsMessage();
				return;
			}

			// Process arguments
			ProcessArguments(args);

			// Perform the action self
			switch (Command)
			{
				case "info":
					CmdInfo();
					break;
				case "install":
					CmdInstall();
					break;
				case "help":
					string topic = (CommandValue != "" || CommandValue != null) ? CommandValue : "default";
					CmdHelp(topic);
					break;
				case "start":
					CmdStart(CommandValue);
					break;
				case "stop":
					CmdStop();
					break;
				default:
					PrintWrongArgumentsMessage();
					break;
			}
		}

		//
		// ==================================================================================
		//

		/// <summary>
		/// Command `info` - prints information if any activity is running (and for how long time).
		/// </summary>
		private static void CmdInfo()
		{
			// Check if user's configuration already exists
			if (CheckIfConfigFileExists() != true)
			{
				PrintLine("Configuration was not found. You should run `install` command.", ConsoleColor.Red, true);
				return;
			}

			var fActivity = Storage.GetRunningActivity();
			if (fActivity[0] == null)
			{
				PrintLine("No activity is running.", ConsoleColor.Magenta, true);
				return;
			}

			Activity runningActivity = fActivity[0];

			PrintLine("There is running activity: " + runningActivity.Name, ConsoleColor.Green);
			PrintRunningTime(runningActivity.GetDuration());
		}

		/// <summary>
		/// Command `install` - creates configuration file.
		/// </summary>
		private static void CmdInstall()
		{
			// Note: In fact we don't need `.odtimetracker.conf` file right now but in future 
			// we want to store there values for default user and also enable more databases 
			// types (MySql, PostgreSql etc. - ODBC probably)...

			if (CheckIfConfigFileExists() == true)
			{
				if (!AskYesNoQuestion("Configuration file already exists! Do you want to continue?"))
				{
					return;
				}
			} 
			else
			{
				if (!AskYesNoQuestion("Default configuration files will be created in your home folder. Do you want to continue?"))
				{
					return;
				}
			}

			// Create `~/.odtimetracker.conf` file!
			StreamWriter stream;
			stream = File.CreateText(ConfigFilePath);
			stream.WriteLine("db_type=sqlite");
			stream.WriteLine("db_path=" + SqliteStorage.DatabaseFilePath);
			stream.Close();

			PrintLine("Configuration file successfully created. Now you can start using this application.", ConsoleColor.Green, true);
		}

		/// <summary>
		/// Command `help` - prints application's help.
		/// </summary>
		/// <param name="topic">Help topic.</param>
		private static void CmdHelp(string topic)
		{
			// TODO Finish `help` command (fill all topics)!

			string executable = System.Reflection.Assembly.GetEntryAssembly().Location;
			string appName = Path.GetFileNameWithoutExtension(executable);

			PrintLine("Example Usage", ConsoleColor.White);
			PrintLine(" " + appName + " start \"New activity@Project name;tag1,tag2,tag3#Activity description.\"");
			PrintLine(" " + appName + " stop");
			PrintLine(" " + appName + " help start", ConsoleColor.Gray, true);
			PrintLine("Commands", ConsoleColor.White);
			PrintLine(" info             Info about current application status");
			PrintLine(" install          Installation and configuration wizard");
			PrintLine(" help [<topic>]   Display help (general or on given topic)");
			PrintLine(" start <activity> Start new activity");
			PrintLine(" stop             Stop currently running activity", ConsoleColor.Gray, true);
			PrintLine("Switches", ConsoleColor.White);
			PrintLine("  --colors|-c     Turn on colored output", ConsoleColor.Gray, true);
		}

		/// <summary>
		/// Command `start` - starts new activity.
		/// </summary>
		/// <param name="activityString">Activity description.</param> 
		private static void CmdStart(string activityString)
		{
			// Check if user's configuration already exists
			if (CheckIfConfigFileExists() != true)
			{
				PrintLine("Configuration was not found. You should run `install` command.", ConsoleColor.Red, true);
				return;
			}

			// Check if there is running activity
			var fActivity = Storage.GetRunningActivity();
			if (fActivity[0] != null)
			{
				PrintLine("Can not create new activity - other activity is still running.", ConsoleColor.Red, true);
				return;
			}

			// Create new activity
			Activity newActivity = new Activity(activityString, Storage);

			// Save it into the database
			newActivity = Storage.InsertActivity(newActivity);

			// Print message and that is all
			PrintLine("New activity was successfully started with ID " + 
				newActivity.ActivityId.ToString() + ".", 
				ConsoleColor.Green, true);
		}

		/// <summary>
		/// Command `stop` - stops currently running activity.
		/// </summary>
		private static void CmdStop()
		{
			if (CheckIfConfigFileExists() != true)
			{
				PrintLine("Configuration was not found. You should run `install` command.", ConsoleColor.Red, true);
				return;
			}

			var fActivity = Storage.GetRunningActivity();
			if (fActivity[0] == null)
			{
				PrintLine("Can not stop activity - no activity is running.", ConsoleColor.Red, true);
				return;
			}

			Activity runningActivity = Storage.StopActivity(fActivity[0]);

			PrintLine("Activity '" + runningActivity.Name + "' was successfully stopped!", ConsoleColor.Green);
			PrintRunningTime(runningActivity.GetDuration());
		}
	}
}
