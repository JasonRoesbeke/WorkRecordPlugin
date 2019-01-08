/*******************************************************************************
  * Copyright (c) 2019 CNH Industrial N.V.
  * All rights reserved. This program and the accompanying materials
  * are made available under the terms of the Eclipse Public License v1.0
  * which accompanies this distribution, and is available at
  * http://www.eclipse.org/legal/epl-v20.html
  *
  * Contributors:
  *    Jason Roesbeke - Initial version.
  *******************************************************************************/
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

		public bool Write(string path, WorkRecordDto workRecordDto)
		{
			var jsonFormat = Path.GetTempFileName();
			try
			{
				// Ensure path exists
				Directory.CreateDirectory(path);

				_internalJsonSerializer.Serialize(workRecordDto, jsonFormat);
				var fileName = ZipUtils.GetSafeName(workRecordDto.Description);
				// ToDo: add option to zip, using ZipUtil => +-8% of original size
				//ZipUtil.Zip(Path.Combine(path, fileName + ".zip"), jsonFormat);
				
				var exportFileName = Path.Combine(path, fileName + ".json");
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

		
	}
}