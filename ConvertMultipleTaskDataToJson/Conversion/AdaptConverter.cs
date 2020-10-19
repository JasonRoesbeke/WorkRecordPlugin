using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.PluginManager;
using ConvertMultipleTaskDataToJson.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertMultipleTaskDataToJson.Conversion
{
	class AdaptConverter
	{
		public bool IsInitialised { get; set; }
		public ConsoleParameters ConsoleParameters { get; set; }
		public PluginFactory PluginFactory { get; set; }
		public Settings ImportSettings { get; set; }
		public Settings ExportSettings { get; set; }
		public List<string> Errors { get; set; }

		public AdaptConverter(string[] args)
		{
			IsInitialised = false;
			Initialise(args);
			Errors = new List<string>();
		}

		public AdaptConverter(ConsoleParameters consoleParameters, Settings importSettings = null, Settings exportSettings = null)
		{
			Errors = new List<string>();
			if (!CheckMinimumNeededProperties(consoleParameters))
			{
				IsInitialised = false;
				return;
			}

			ConsoleParameters = consoleParameters;
			PluginFactory = new PluginFactory(ConsoleParameters.PluginsFolderPath);
			ImportSettings = importSettings;
			ExportSettings = exportSettings;
			IsInitialised = true;
		}

		private bool CheckMinimumNeededProperties(ConsoleParameters consoleParameters)
		{
			var tempErrors = new List<string>();
			if (consoleParameters == null)
			{
				Errors.Add("ConsoleParameters is null");
				return false;
			}
			if (consoleParameters.PluginsFolderPath == "" || consoleParameters.PluginsFolderPath == null)
			{
				tempErrors.Add("PluginsFolderPath must not be null");
			}
			if (consoleParameters.ImportDataPath == "" || consoleParameters.ImportDataPath == null)
			{
				tempErrors.Add("ImportDataPath must not be null");
			}
			if (consoleParameters.ImportPluginName == "" || consoleParameters.ImportPluginName == null)
			{
				tempErrors.Add("ImportPluginName must not be null");
			}
			if (consoleParameters.ExportDataPath == "" || consoleParameters.ExportDataPath == null)
			{
				tempErrors.Add("ExportDataPath must not be null");
			}
			if (consoleParameters.ExportPluginName == "" || consoleParameters.ExportPluginName == null)
			{
				tempErrors.Add("ExportPluginName must not be null");
			}
			if (tempErrors.Count > 0)
			{
				Errors.AddRange(tempErrors);
				return false;
			}
			return true;
		}

		private void Initialise(string[] args)
		{
			// Console Parameters
			ConsoleParameters = Parameters.ParseArguments(args);
			if (ConsoleParameters == null)
			{
				return;
			}
			Console.WriteLine("Console Parameters loaded");

			// PluginFactory
			PluginFactory = new PluginFactory(ConsoleParameters.PluginsFolderPath);
			if (PluginFactory == null)
			{
				return;
			}
			Console.WriteLine($"{PluginFactory.AvailablePlugins.Count} Plugins loaded");

			// Import Properties
			ImportSettings = GetSettings(ConsoleParameters.ImportPluginPropertiesFile);
			if (ImportSettings == null)
			{
				return;
			}
			Console.WriteLine($"{ImportSettings.Properties.Count} Import Properties loaded");

			// Export Properties
			ExportSettings = GetSettings(ConsoleParameters.ExportPluginPropertiesFile);
			if (ExportSettings == null)
			{
				return;
			}
			Console.WriteLine($"{ExportSettings.Properties.Count} Export Properties loaded");

			// Finalize Initialisation
			IsInitialised = true;
		}

		public static bool Convert(IPlugin importPlugin, IPlugin exportPlugin, string importDataPath, string exportDataPath)
		{
			List<ApplicationDataModel> adms = new List<ApplicationDataModel>();

			// Import
			Console.WriteLine("Starting ADAPT import");
			DateTime startTime = DateTime.Now;
			// Check if Plugin supports the data
			if (importPlugin.IsDataCardSupported(importDataPath))
			{
				adms.AddRange(importPlugin.Import(importDataPath));
			}
			else
			{
				Console.WriteLine($"ImportPlugin cannot read the data, stopping!");
				return false;
			}
			TimeSpan conversionTime = DateTime.Now.Subtract(startTime);
			Console.WriteLine($"Completed ADAPT import in {conversionTime}, imported {adms.Count} ApplicationDataModels");

			// Export
			Console.WriteLine("Starting ADAPT export");
			startTime = DateTime.Now;
			for (int i = 0; i < adms.Count; i++)
			{
				// Export for each ApplicationDataModel created
				exportPlugin.Export(adms[i], exportDataPath);
				Console.WriteLine($"Exported adm {i + 1} of the total {adms.Count} adms");
			}
			conversionTime = DateTime.Now.Subtract(startTime);
			Console.WriteLine($"Completed ADAPT export in {conversionTime}, exported {adms.Count} ApplicationDataModel(s)");
			return true;
		}

		public bool Convert()
		{
			if (!IsInitialised)
			{
				Console.WriteLine("Converter not initialised");
				return false;
			}

			// Import
			List<ApplicationDataModel> adms = ImportData();
			if (adms == null)
			{
				return false;
			}

			// Export
			for (int i = 0; i < adms.Count; i++)
			{
				// Export for each ApplicationDataModel created
				bool success = ExportData(adms[i], ConsoleParameters);
				if (success)
				{
					Console.WriteLine($"Successfully exported adm {i + 1} of the total {adms.Count} adms");
				}
				else
				{
					Console.WriteLine($"Unsuccessfully exported adm {i + 1} of the total {adms.Count} adms");
				}
			}
			adms = null;
			return true;
		}

		// ToDo: in seperated class?
		private bool ExportData(ApplicationDataModel applicationDataModel, ConsoleParameters consoleParameters)
		{
			if (PluginFactory.AvailablePlugins.Count == 0)
			{
				Console.WriteLine("PluginFactory: no plugins available");
				return false;
			}
			// To Do (check if possible/needed): Task.Run(() => {});
			Console.WriteLine("Starting ADAPT export");
			DateTime startTime = DateTime.Now;

			// Get Plugin
			// ToDo: [Check] version of plugin
			var exportPlugin = PluginFactory.GetPlugin(consoleParameters.ExportPluginName);
			if (exportPlugin == null)
			{
				Console.WriteLine($"Could not find ExportPlugin {consoleParameters.ImportPluginName}");
				return false;
			}

			// Initialise Plugin
			if (ExportSettings != null)
			{
				if (!string.IsNullOrEmpty(ExportSettings.InitialiseString))
				{
					exportPlugin.Initialize(ExportSettings.InitialiseString);
				}
			}

			// Check if Plugin supports the data
			if (ExportSettings != null)
			{
				if (ExportSettings.GetProperties() != null)
				{
					exportPlugin.Export(applicationDataModel, consoleParameters.ExportDataPath, ExportSettings.GetProperties());
				}
			}
			else
			{
				exportPlugin.Export(applicationDataModel, consoleParameters.ExportDataPath);
			}
			TimeSpan conversionTime = DateTime.Now.Subtract(startTime);
			Console.WriteLine($"Completed ADAPT export in {conversionTime}, exported 1 ApplicationDataModels");
			return true;
		}

		private List<ApplicationDataModel> ImportData()
		{
			if (!IsInitialised)
			{
				Console.WriteLine("Converter not initialised");
				return null;
			}

			return ImportData(ConsoleParameters.ImportPluginName, ConsoleParameters.ImportDataPath);
		}

		private List<ApplicationDataModel> ImportData(string importPluginName, string importDataPath)
		{
			if (PluginFactory.AvailablePlugins.Count == 0)
			{
				Console.WriteLine("PluginFactory: no plugins available");
				return null;
			}

			// Get ImportPlugin
			// ToDo: [Check] version of plugin
			// ToDo: Read importDataPath with all available plugins
			var importPlugin = PluginFactory.GetPlugin(importPluginName);
			if (importPlugin == null)
			{
				Console.WriteLine($"Could not find ImportPlugin {importPluginName}");
				return null;
			}
			// Initialise Plugin
			if (ImportSettings != null)
			{
				if (!string.IsNullOrEmpty(ImportSettings.InitialiseString))
				{
					importPlugin.Initialize(ImportSettings.InitialiseString);
				}
			}

			// [Check] if data path is correct
			if (!Directory.Exists(importDataPath))
			{
				Console.WriteLine($"Incorrect importDataPath {importDataPath}");
				return null;
			}

			List<ApplicationDataModel> adms = new List<ApplicationDataModel>();

			// To Do (check if possible/needed): Task.Run(() => {});
			Console.WriteLine("Starting ADAPT import");
			DateTime startTime = DateTime.Now;

			// Check if Plugin supports the data
			if (importPlugin.IsDataCardSupported(importDataPath))
			{
				if (ImportSettings != null)
				{
					if (ImportSettings.GetProperties() != null)
					{
						adms.AddRange(importPlugin.Import(importDataPath, ImportSettings.GetProperties()));
					}
				}
				else
				{
					adms.AddRange(importPlugin.Import(importDataPath));
				}
				TimeSpan conversionTime = DateTime.Now.Subtract(startTime);
				Console.WriteLine($"Completed ADAPT import in {conversionTime}, imported {adms.Count} ApplicationDataModels");
				return adms;
			}
			else
			{
				Console.WriteLine($"ImportPlugin cannot read the data, not imported!");
				return null;
			}
		}

		private static Settings GetSettings(string propertiesFile)
		{
			var fileString = File.ReadAllText(propertiesFile);
			return JsonConvert.DeserializeObject<Settings>(fileString);
		}
	}
}
