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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace WorkRecordPlugin.Utils
{
	public class InfoFileReader
	{
		private readonly Version AssemblyVersion;

		public InfoFileReader(Version assemblyVersion)
		{
			AssemblyVersion = assemblyVersion;
		}

		public InfoFile ReadVersionInfoModel(string filename)
		{
			if (!File.Exists(filename))
				return null;

			var fileString = File.ReadAllText(filename);

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
			var fileName = Path.Combine(path, string.Format(InfoFileConstants.FileFormat, InfoFileConstants.InfoFileName));
			var infoFile = ReadVersionInfoModel(fileName);
			if (infoFile == null)
			{
				// ToDo: through subDirectories
				return false;
			}

			// [Check] if version is equal to current pluginVersion
			// ToDo: only check Major version number!
			if (infoFile.VersionPlugin != AssemblyVersion.ToString())
			{
				return false;
			}

			return true;
		}
	}
}
