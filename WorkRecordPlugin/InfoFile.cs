/*******************************************************************************
  * Copyright (C) 2019 AgGateway and ADAPT Contributors
  * Copyright (C) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace WorkRecordPlugin
{
	public class InfoFile
	{
		public InfoFile(string name, string version, string description, ExportProperties exportProperties)
		{
			NamePlugin = name;
			VersionPlugin = version;
			ADAPTVersion = Assembly.LoadFrom("AgGateway.ADAPT.ApplicationDataModel.dll").GetName().Version.ToString();
			DescriptionOfCatalog = description;
			ExportProperties = exportProperties;
			DateConversion = DateTime.Now;
		}

		[JsonProperty(Order = -4)]
		public string ADAPTVersion { get; }
		[JsonProperty(Order = -3)]
		public string NamePlugin { get; }

		[JsonProperty(Order = -2)]
		public string VersionPlugin { get; }

		[JsonProperty(Order = -1)]
		public string DescriptionOfCatalog { get; }

		[JsonProperty(Order = 0)]
		public string DateOfConversion
		{
			get
			{
				return DateConversion.ToString();
			}
		}

		[JsonIgnore]
		public DateTime DateConversion { get; }

		[JsonProperty(Order = 2)]
		public ExportProperties ExportProperties { get; }

	}

	public static class InfoFileConstants
	{
		public const string InfoFileName = "InfoFileWorkRecordExport";
		public const string jsonFileExtension = ".json";
		public const string PluginFolder = "ADAPTWorkRecords";
        public const string FileFormat = "{0}" + jsonFileExtension;
	}
}