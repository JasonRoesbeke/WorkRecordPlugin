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

namespace WorkRecordPlugin
{
	public class InfoFile
	{
		public InfoFile(string name, string version, string adaptVersion, string description, PluginProperties exportProperties, DateTime dateOfConversion)
		{
			NamePlugin = name;
			VersionPlugin = version;
			AdaptVersion = adaptVersion;
			DescriptionOfCatalog = description;
			ExportProperties = exportProperties;
			DateOfConversion = dateOfConversion;
		}

		[JsonProperty(Order = -4)]
		public string AdaptVersion { get; set; }
		[JsonProperty(Order = -3)]
		public string NamePlugin { get; set; }

		[JsonProperty(Order = -2)]
		public string VersionPlugin { get; set; }

		[JsonProperty(Order = -1)]
		public string DescriptionOfCatalog { get; set; }

		[JsonProperty(Order = 0)]
		public DateTime DateOfConversion { get; set; }

		[JsonProperty(Order = 2)]
		public PluginProperties ExportProperties { get; set; }
	}
}