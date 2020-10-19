using ConvertMultipleTaskDataToJson.Conversion;
using ConvertMultipleTaskDataToJson.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertMultipleTaskDataToJson
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!CheckImportDataPathAndExportDataPath(args, out string error, out string importDataPath, out string exportDataPath))
			{
				Console.WriteLine(error);
				WaitForUserInputThenExit();
			}
			if (importDataPath == "" || exportDataPath == "")
			{
				Console.WriteLine("importDataPath == '' || exportDataPath == ''");
				WaitForUserInputThenExit();
			}

			var rootFolders = GetListOfTaskdataFolders(importDataPath);
			if (rootFolders == null)
			{
				Console.WriteLine("rootFolders == null");
				WaitForUserInputThenExit();
			}

			int count = 0;
			int totalFolders = rootFolders.Count();
			AdaptConverter converter;
			foreach (var folder in rootFolders)
			{
				count++;
				Console.WriteLine($"Starting ADAPT conversion for folder {Path.GetFileName(folder)} ({count}/{totalFolders})");
				// Only ImportDataPath and ExportDataPath needed
				var consoleParameters = new ConsoleParameters();
				var root = Directory.GetCurrentDirectory();
				consoleParameters.PluginsFolderPath = Path.Combine(root, "Resources\\adapt2.0.4plugins");
				consoleParameters.ImportDataPath = folder;
				consoleParameters.ImportPluginName = "ISOv4Plugin";
				consoleParameters.ExportPluginName = "WorkRecordPlugin";
				consoleParameters.ExportDataPath = exportDataPath;
				converter = new AdaptConverter(consoleParameters);

				try
				{
					if (converter.Convert())
					{
						Console.WriteLine("Successfull ADAPT conversion");
						continue;
					}
				}
				catch (Exception e)
				{
					Console.WriteLine("Exception: " + e.Message + " InnerException: " + e.InnerException?.Message);
				}


				Console.WriteLine($"ADAPT conversion unsuccesfull for folder {Path.GetFileName(folder)}");
			}
			Console.WriteLine($"ADAPT conversion succesfull for folders in {importDataPath}");
			WaitForUserInputThenExit();
		}

		private static void WaitForUserInputThenExit()
		{
			Console.ReadLine();
			Environment.Exit(-1);
		}

		private static bool CheckImportDataPathAndExportDataPath(string[] args, out string error, out string importDataPath, out string exportDataPath)
		{
			error = "";
			importDataPath = "";
			exportDataPath = "";
			if (args != null)
			{
				if (args.Count() == 2)
				{
					importDataPath = args[0];
					if (!Directory.Exists(importDataPath))
					{
						error += "import path does not exist, ";
					}
					exportDataPath = args[1];
					if (!Directory.Exists(exportDataPath))
					{
						error += "export path does not exist, ";
					}

					if (error == "")
					{
						return true;
					}
				}
			}
			else
			{
				error += "import (1) and export(2) datapath needed, ";
			}
			error += "stopping conversion..";
			return false;
		}

		private static List<string> GetListOfTaskdataFolders(string dataPath)
		{
			var taskdataFolders = new List<string>();
			taskdataFolders.Add(dataPath);
			return taskdataFolders;
		}
	}
}
