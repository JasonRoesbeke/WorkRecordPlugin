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
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ADAPT.DTOs.Documents;
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

		public bool WriteInfoFile(string path, string name, string version, string description, PluginProperties exportProperties)
		{
			var adaptVersion = Assembly.LoadFrom("AgGateway.ADAPT.ApplicationDataModel.dll").GetName().Version.ToString();
			return WriteJson(path, new InfoFile(name, version, adaptVersion, description, exportProperties, DateTime.Now), InfoFileConstants.InfoFileName);
		}

		public bool Write(string path, List<WorkRecordDto> workRecordDtos)
		{
			var succes = 0;
			foreach (var workRecordDto in workRecordDtos)
			{
				succes += Write(path, workRecordDto) ? 0 : 1;
			}
			return succes == 0;
		}
		public bool Write(string path, WorkRecordDto workRecordDto)
		{
			if (workRecordDto == null)
			{
				return false;
			}
			if (workRecordDto.Description == null)
			{
				workRecordDto.Description = workRecordDto.Guid.ToString();
			}
			return WriteJson(path, workRecordDto, workRecordDto.Description); ;
		}

		private bool WriteJson<T>(string path, T objectToSerialize, string fileName) where T : class
		{
			var jsonFormat = Path.GetTempFileName();
			try
			{
				// Ensure path exists
				Directory.CreateDirectory(path);

				_internalJsonSerializer.Serialize(objectToSerialize, jsonFormat);
				var safeFileName = ZipUtils.GetSafeName(fileName);
				// ToDo: add option to zip, using ZipUtil => +-8% of original size
				//ZipUtil.Zip(Path.Combine(path, fileName + ".zip"), jsonFormat);

				var exportFileName = Path.Combine(path, safeFileName + InfoFileConstants.JsonFileExtension);
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
			string path = Path.GetDirectoryName(fullPath) ?? "";
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