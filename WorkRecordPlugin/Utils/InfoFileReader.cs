using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public class InfoFileReader
	{
		public InfoFile ReadVersionInfoModel(string filename)
		{
			if (!File.Exists(filename))
				return null;

			var fileString = File.ReadAllText(filename);

			var model = JsonConvert.DeserializeObject<InfoFile>(fileString);
			return model;
		}
	}
}
