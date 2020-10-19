namespace WorkRecordPlugin
{
	public static class InfoFileConstants
	{
		public const string InfoFileName = "InfoFileWorkRecordExport";
		public const string JsonFileExtension = ".json";
		public const string PluginFolderPrefix = "ADAPTWorkRecords";
		public const string FileFormat = "{0}" + JsonFileExtension;

		public static string ConvertToSearchPattern(string filePattern)
		{
			return filePattern.Replace("{0}", "*");
		}
	}
}
