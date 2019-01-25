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
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin
{
	public class WorkRecordExporter
	{
		private InternalJsonSerializer _internalJsonSerializer;

		public WorkRecordExporter(InternalJsonSerializer internalJsonSerializer)
		{
			_internalJsonSerializer = internalJsonSerializer;
		}

		public bool WriteInfoFile(string path, string name, string version, string description, ExportProperties exportProperties)
		{
			return WriteJson(path, new InfoFile(name, version, description, exportProperties), InfoFileConstants.InfoFileName);
		}

		public bool Write(string path, WorkRecordDto workRecordDto)
		{
			if (workRecordDto.Description == null)
			{
				workRecordDto.Description = workRecordDto.Guid.ToString();
			}
			return WriteJson(path, workRecordDto, workRecordDto.Description);
		}

		private bool WriteJson<T>(string path, T objectToSerialize, string fileName) where T : class
		{
			var jsonFormat = Path.GetTempFileName();
			try
			{
				// Add pluginFolder name to path
				path = Path.Combine(path, InfoFileConstants.PluginFolder);

				// Ensure path exists
				Directory.CreateDirectory(path);

				_internalJsonSerializer.Serialize((T)objectToSerialize, jsonFormat);
				var safeFileName = ZipUtils.GetSafeName(fileName);
				// ToDo: add option to zip, using ZipUtil => +-8% of original size
				//ZipUtil.Zip(Path.Combine(path, fileName + ".zip"), jsonFormat);

				var exportFileName = Path.Combine(path, safeFileName + InfoFileConstants.jsonFileExtension);
				// Check if no file is already created with same name
				if (File.Exists(exportFileName))
				{
					exportFileName = AlreadyExits(exportFileName);

					//// Check if is the same file. Is not, then do not overwrite
					//FileInfo alreadyExitsFile = new FileInfo(exportFileName);
					//FileInfo tempFile = new FileInfo(jsonFormat);
					//if (alreadyExitsFile.Length != tempFile.Length)
					//{
					//	exportFileName = AlreadyExits(exportFileName);
					//}

				}

				File.Copy(jsonFormat, exportFileName, true);
			}
			catch (Exception)
			{
				return false;
			}
			finally
			{
				try
				{
					File.Delete(jsonFormat);
				}
				catch
				{
				}
			}
			return true;
		}

		private string AlreadyExits(string fullPath)
		{
			int count = 1;

			string fileNameOnly = Path.GetFileNameWithoutExtension(fullPath);
			string extension = Path.GetExtension(fullPath);
			string path = Path.GetDirectoryName(fullPath);
			string newFullPath = fullPath;

			while (File.Exists(newFullPath))
			{
				string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
				newFullPath = Path.Combine(path, tempFileName + extension);
			}
			return newFullPath;
		}

		
	}
}