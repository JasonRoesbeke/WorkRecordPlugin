using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin
{
	public static class InfoFileConstants
	{
		public const string InfoFileName = "InfoFileWorkRecordExport";
		public const string jsonFileExtension = ".json";
		public const string PluginFolderPrefix = "ADAPTWorkRecords";
		public const string FileFormat = "{0}" + jsonFileExtension;

		public static string ConvertToSearchPattern(string filePattern)
		{
			return filePattern.Replace("{0}", "*");
		}
	}
}
