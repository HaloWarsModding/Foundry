using System;
using System.Windows.Forms;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.IO.Compression;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Windows.Forms.Timer;
using File = System.IO.File;
using YAXLib;
using System.ComponentModel;
using IniParser.Model;
using System.Runtime.Loader;
using Foundry.Views;
using Foundry.HW1.Triggerscript;
using System.Text;
using System.Xml.Serialization;
using System.Xml;
using Foundry.HW1;

namespace Foundry
{
    public partial class FoundryInstance : Form
	{
		//////////////////////////////////////////////////////////////////////////////////////
		#region  foundry instance
		public FoundryInstance()
		{
			Load += new EventHandler(OnLoad);
			FormClosed += new FormClosedEventHandler(OnClose);

			InitializeComponent();

			CurrentPage = null;
			OpenPages = new List<BaseView>();

			ThreadPool.SetMinThreads(16, 16);

			if (!Directory.Exists(AppdataDir)) Directory.CreateDirectory(AppdataDir);

			versionReadout.Text = "v" + System.Diagnostics.FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion.ToString();
			dockpanel.Theme = new VS2015LightTheme();

			memoryMonitorTicker = new Timer();
			memoryMonitorTicker.Tick += (object o, EventArgs e) =>
			{
				memoryReadout.Text = (Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024)).ToString() + "mb";
			};
			memoryMonitorTicker.Interval = 1000;
			memoryMonitorTicker.Start();

			OpenPages = new List<BaseView>();

		}

		//callbacks
		public Workspace workspace;
		private void OnLoad(object o, EventArgs e)
		{
			workspace = new Workspace();
            Browser = new Browser(this);
			Browser.Form.Text = "Browser";
			Browser.Show(this, DockState.DockLeft);


			Editor view = new Editor(this);
//#if DEBUG
			string file = "D:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\Extract\\data\\triggerscripts\\skirmishai_raw_.triggerscript";
			string fileout = "D:\\SteamLibrary\\steamapps\\common\\HaloWarsDE\\Extract\\data\\triggerscripts\\out.triggerscript";
            var options = new YAXLib.Options.SerializerOptions()
			{
				ExceptionBehavior = YAXLib.Enums.YAXExceptionTypes.Ignore,
				ExceptionHandlingPolicies = YAXLib.Enums.YAXExceptionHandlingPolicies.DoNotThrow,
				MaxRecursion = int.MaxValue
			};
			Triggerscript xml = new YAXSerializer<Triggerscript>(options).DeserializeFromFile(file);

			EditorHelpers.Validate(xml);
			using (TextWriter writer = new StreamWriter(fileout))
			{
				new YAXSerializer<Triggerscript>(options).Serialize(xml, writer);
			}
			view.TriggerscriptFile = xml;
//#endif
			view.Show(MainDockPanel, DockState.Document);

			InitConfig();
			InitToolstrip();
			//InitModules();

#if DEBUG
			workspace.Open("D:\\repos\\Foundry\\_resources\\workspace\\");
            Browser.RootItems.Add(WorkspaceBrowser.DataItem(workspace));
            Browser.RootItems.Add(WorkspaceBrowser.ArtItem(workspace));
            Browser.UpdateView();
#endif
        }
        private void OnClose(object o, EventArgs e)
		{
			SaveConfig();
			//if (IsWorkspaceOpen())
			//{
			//	CloseWorkspace();
			//}
			//Controls.Clear();
		}
		private Timer memoryMonitorTicker;
		private void ToolStrip_File_ImportAssetClicked(object sender, EventArgs e)
		{
		}
		private void ToolStrip_File_OpenAssetClicked(object sender, EventArgs e)
		{
		}
        private void Footer_DiscordImageClicked(object sender, EventArgs e)
		{
#if !DEBUG //I dont want to click it :P
			Process.Start("https://discord.gg/kfrCNUTaSc");
#endif
		}

        public DockPanel MainDockPanel { get { return dockpanel; } }

        private OperatorRegistrantToolstrip Operators_MainForm;
        public Operator Operator_File { get; private set; }
        public Operator Operator_Tools { get; private set; }
		public void RefreshToolstrip()
        {
			menuStrip.Items.Clear();
			foreach (ToolStripMenuItem item in Operators_MainForm.GetRootMenuItems())
			{
				menuStrip.Items.Add(item);
			}
		}
        private void InitToolstrip()
		{
			Operators_MainForm = new OperatorRegistrantToolstrip();

			Operator_File = new Operator("File");
            Operators_MainForm.AddOperator(Operator_File);

			//Operator opNewWorkspace = new Operator("New Workspace");
			//opNewWorkspace.OperatorActivated += (sender, e) =>
			//{
			//	CreateWorkspaceWizard cww = new CreateWorkspaceWizard();
			//	if (cww.ShowDialog() == DialogResult.OK)
			//	{
			//		CreateWorkspace(cww.WorkspaceLocation, cww.WorkspaceName, cww.WorkspaceUnpackDefault);
			//	}
			//};
			//opNewWorkspace.Parent = Operator_File;

   //         Operator opOpenWorkspace = new Operator("Open Workspace");
			//opOpenWorkspace.OperatorActivated += (sender, e) =>
			//{
			//	OpenFileDialog ofd = new OpenFileDialog();
			//	ofd.Filter = string.Format("Foundry Project (*{0})|*{0}", ProjectFileExt);

			//	if (ofd.ShowDialog() == DialogResult.OK)
			//	{
			//		OpenWorkspace(ofd.FileName);
			//	}
			//};
   //         opOpenWorkspace.Parent = Operator_File;

   //         Operator opImportArchive = new Operator("Import Archive");
			//opImportArchive.OperatorActivated += (sender, e) =>
   //         {
   //             OpenFileDialog ofd = new OpenFileDialog();
   //             ofd.Filter = "Ensemble Resource Archive (*.era)|*.era";
   //             ofd.Multiselect = true;
   //             ofd.InitialDirectory = string.Format("{0}\\",
   //                         Path.GetDirectoryName(OpenedConfig.GetParamData(Config.Param.GameExe).Value)
   //                         );

   //             if (ofd.ShowDialog() == DialogResult.OK)
   //             {
   //                 UnpackErasAsync(ofd.FileNames, GetNamedWorkspaceDir(NamedWorkspaceDirNames.WorkspaceFolder).FullPath);
   //             }
   //         };
   //         opImportArchive.Parent = Operator_File;


			//Operator opSave = new Operator("Save");
			//opSave.OperatorActivated += (sender, e) =>
			//{
			//	SaveCurrentPage();
			//};
			//opSave.Parent = Operator_File;


   //         Operator_Tools = new Operator("Tools");
   //         Operators_MainForm.AddOperator(Operator_Tools);

			//Operator opSettings = new Operator("Settings");
   //         opSettings.OperatorActivated += (sender, e) =>
   //         {
   //             ConfigPrompt prompt = new ConfigPrompt(OpenedConfig, true);
   //             if (prompt.ShowDialog() == DialogResult.OK)
   //             {
   //                 OpenedConfig = prompt.LocalConfig;
   //             }
   //         };
   //         opSettings.Parent = Operator_Tools;

			RefreshToolstrip();
		}

		public static string AppdataDir
		{
			get
			{
				string ret = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Foundry\\";
				if (!Directory.Exists(ret))
				{
					Directory.CreateDirectory(ret);
				}
                return ret;
            }
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region config
		public string ConfigFile { get { return AppdataDir + "config.cfg"; } }
		public class Config
		{
			public Config()
			{
				datas = new Dictionary<Param, ParamData>()
				{
					{ Param.GameExe, new ParamData("Game executable path", ParamType.File, null, "xgameFinal.exe") }
				};
			}

			public enum Param
			{
				GameExe
			}
			public enum ParamType
			{
				Directory,
				File,
				String,
				Number
			}
			public class ParamData
            {
                public ParamData(string displayName, ParamType type, string defaultValue = null, object validValue = null)
                {
                    DisplayName = displayName;
                    Value = defaultValue;
					Type = type;
					ValidValue = validValue;
                }
                public string Value { get; set; }
                public string DisplayName { get; private set; }
				public ParamType Type { get; private set; }
                public object ValidValue { get; private set; }
            }
			public bool AllParamsValid()
			{
				foreach (Param p in Enum.GetValues<Param>())
				{
					if (!ParamValid(p)) return false;
				}
				return true;
			}
			public bool ParamValid(Param param)
			{
				if (datas.ContainsKey(param))
				{
					ParamData data = datas[param];

					switch (data.Type)
					{
						case ParamType.Directory:
							if (Directory.Exists(data.Value)) return true;
							return false;

						case ParamType.File:
							if (File.Exists(data.Value))
							{
								if (data.ValidValue != null)
								{
									string validFile = (string)data.ValidValue;
									if (Path.GetFileName(data.Value) == validFile) return true;
									else return false;
								}
							}
							return false;
					}
				}
				return false;
			}
            public ParamData GetParamData(Param param)
			{
				return datas[param];
			}


			private Dictionary<Param, ParamData> datas;
		}
		public Config OpenedConfig { get; private set; }
		private void InitConfig()
		{
			OpenedConfig = new Config();

			if (!File.Exists(ConfigFile)) File.Create(ConfigFile).Close();

			string[] cfg = File.ReadAllLines(ConfigFile);

			foreach (string line in cfg)
			{
				string[] keyval = line.Split('=');
				if (keyval.Length != 2) continue;

				string key = keyval[0].Trim();
				string val = keyval[1].Trim();

				Config.Param p;
				bool valid = Enum.TryParse(key, out p);
				if (valid)
				{
					OpenedConfig.GetParamData(p).Value = val;
				}
			}

			//Ensure valid config data by constantly opening the prompt until everything is set.
			if (!OpenedConfig.AllParamsValid())
			{
				ConfigPrompt prompt = new ConfigPrompt(OpenedConfig, false);
				if (prompt.ShowDialog() == DialogResult.OK)
				{
					OpenedConfig = prompt.LocalConfig;
					SaveConfig();
				}
			}
		}
		private void SaveConfig()
		{
			if (!File.Exists(ConfigFile))
			{
				File.Create(ConfigFile);
			}

			string data = "";

			foreach (Config.Param p in Enum.GetValues<Config.Param>())
			{
				string val = OpenedConfig.GetParamData(p).Value;
                if (val == null) continue;

				data += string.Format("{0} = {1}\n", p.ToString(), val);
			}

			File.WriteAllText(ConfigFile, data);
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region  log
		public enum LogEntryType
		{
			Info,
			Warning,
			Error,
			DebugInfo,
			DebugError,
		}
		/// <summary>
		/// Writes text to the window status bar and console.
		/// </summary>
		/// <param name="type">The type of message to send. Affects prefix.</param>
		/// <param name="mainMessage">The main text to be written.</param>
		/// <param name="displayAsStatus">If true, mainMessage will be written to the footer status bar, until another message overwrites it.</param>
		/// <param name="secondaryMessage">If supplied, this will be written to the console, but not displayed on the status bar. Useful for things like exception info.</param>
		public void AppendLog(LogEntryType type, string mainMessage, bool displayAsStatus, string secondaryMessage = null)
		{
#if !DEBUG
			if (type == LogEntryType.DebugInfo || type == LogEntryType.DebugError) return;
#endif
			string prefix;
			switch (type)
			{
				case LogEntryType.Info:
				case LogEntryType.DebugInfo:
					prefix = "[Info] ";
					break;
				case LogEntryType.Warning:
					prefix = "[Warning] ";
					break;
				case LogEntryType.Error:
				case LogEntryType.DebugError:
					prefix = "[Error] ";
					break;
				default:
					prefix = "";
					break;
			}
			string print = prefix + mainMessage;

			if (displayAsStatus)
				logStatus.Text = print;

			Console.WriteLine(print);
			if (secondaryMessage != null)
			{
				Console.WriteLine(secondaryMessage);
			}

			//just for debug error:
			if (type == LogEntryType.DebugError)
			{
				MessageBox.Show(mainMessage, "Hey!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}

        }
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region modules
		private List<BaseModule> Modules { get; set; }
		private void InitModules()
		{
			if (Modules != null) return;

			Modules = new List<BaseModule>();

			string modulesDir = Directory.GetCurrentDirectory() + "\\modules\\";

			if (!Directory.Exists(modulesDir))
				Directory.CreateDirectory(modulesDir);

            AssemblyLoadContext context = new AssemblyLoadContext(null);
            foreach (string file in Directory.EnumerateFiles(modulesDir))
			{
				if (Path.GetFileName(file).StartsWith("Foundry") && file.EndsWith(".dll"))
				{
					AssemblyDependencyResolver resolver = new AssemblyDependencyResolver(file);
					string assembly = resolver.ResolveAssemblyToPath(new AssemblyName(Path.GetFileNameWithoutExtension(file)));
					context.LoadFromAssemblyPath(assembly);
                }
			}

			Type baseModuleType = typeof(BaseModule);
			var assemblies =
				AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.Where(p => baseModuleType.IsAssignableFrom(p) && p.IsClass).ToArray();

			foreach (Type t in assemblies)
			{
				if (t == baseModuleType) continue; //dont make an instance of the base, silly. can this even happen?

				BaseModule module = (BaseModule)Activator.CreateInstance(t);

				module.Init(this);
				Modules.Add(module);
			}

			foreach(BaseModule module in Modules)
			{
				module.PostInit();
			}
		}

		public bool GetModuleByType<T>(out T module) where T : BaseModule
		{
			var valid = Modules.Where(e => e.GetType() == typeof(T));

            if (valid.Any())
			{
				module = (T)valid.First();
				return true;
			}

			module = null;
			return false;
		}
		#endregion


        //////////////////////////////////////////////////////////////////////////////////////
        #region background task
        public void StartBlockingBackgroundTask(Action<object> action, object argument, string displayText)
        {
			using (BackgroundWorker worker = new BackgroundWorker())
			{
				BlockingProgressBar progressBar = new BlockingProgressBar();

                worker.DoWork += (sender, e) =>
				{
					action(argument);
				};
				worker.RunWorkerCompleted += (sender, e) =>
				{
					progressBar.Close();
                };

				worker.RunWorkerAsync();
                progressBar.TaskDisplayText = displayText;
                progressBar.ShowDialog();
			}
        }
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region workspace
		


		public void UnpackErasAsync(string[] eras, string outdir)
		{
			Tuple<string[], string> args = new Tuple<string[], string>(eras, outdir);

			StartBlockingBackgroundTask(
			(o) =>
			{
				Tuple<string[], string> taskargs = (Tuple<string[], string>)o;


                List <Task> tasks = new List<Task>();
				foreach (string era in taskargs.Item1)
				{
					Task t = Task.Run(() =>
					{
						if (!Directory.Exists(taskargs.Item2))
						{
							return;
						}

						Util.ERA.ExpandERA(era, outdir);
					});
					tasks.Add(t);
				}

				Task.WaitAll(tasks.ToArray());
			},
            args,
			"Unpacking archives...");
		}
		#endregion


		//////////////////////////////////////////////////////////////////////////////////////
		#region editors
		public Browser Browser { get; private set; }

		private BaseView CurrentPage { get; set; }
		private List<BaseView> OpenPages { get; set; }

		public void RegisterPage(BaseView p)
		{
			if (OpenPages.Contains(p)) return;

			p.ViewGotFocus += (sender, e) =>
			{
				CurrentPage = p;
			};
			p.ViewLostFocus += (sender, e) =>
			{
				CurrentPage = null;
			};

			List<BaseView> pages = OpenPages.ToList();
			pages.Add(p);
			OpenPages = pages;
		}
		public void UnregisterPage(BaseView p)
		{
			if (!OpenPages.Contains(p)) return;

			List<BaseView> pages = OpenPages.ToList();
			pages.Remove(p);
			OpenPages = pages;
		}
		public void SaveCurrentPage()
        {
			if(CurrentPage != null)
            {
				CurrentPage.Save();
            }
        }
		public void SaveCurrentPageAs()
        {

        }
		public void SaveAllPages()
        {
			foreach(BaseView p in OpenPages)
            {
				p.Save();
            }
        }
		#endregion
	}
}
