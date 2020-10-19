using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace WorkRecordPlugin.Utils
{
	public class InfoFileReader
	{
		private readonly Version _assemblyVersion;

		public InfoFileReader(Version assemblyVersion)
		{
			_assemblyVersion = assemblyVersion;
		}

		public InfoFile ReadVersionInfoModel(string path)
		{
			var fileName = Path.Combine(path, string.Format(InfoFileConstants.FileFormat, InfoFileConstants.InfoFileName));
			if (!File.Exists(fileName))
			{
				return null;
			}

			var fileString = File.ReadAllText(fileName);

			var model = JsonConvert.DeserializeObject<InfoFile>(fileString);
			return model;
		}

		public bool ValidateMinimalFolderStructure(string path)
		{
			// [Check] if folder exists
			if (!Directory.Exists(path))
			{
				return false;
			}

			// [Check] if folder name starts with "InfoFileConstants.PluginFolder"
			var folderName = Path.GetFileName(path);
			if (!folderName.StartsWith(InfoFileConstants.PluginFolderPrefix))
			{
				return false;
			}

			// [Check] if folder contains any json files
			if (!Directory.GetFiles(path, string.Format(InfoFileConstants.FileFormat, "*"), SearchOption.AllDirectories).Any())
			{
				return false;
			}

			// Find InfoFileName
			var infoFile = ReadVersionInfoModel(path);
			if (infoFile == null)
			{
				// ToDo: through subDirectories
				return false;
			}

			// [Check] if version is equal to current pluginVersion
			// ToDo: only check Major version number!
			if (infoFile.VersionPlugin != _assemblyVersion.ToString())
			{
				return false;
			}

			return true;
		}
	}
}
