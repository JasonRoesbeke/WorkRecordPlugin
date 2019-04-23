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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.ReferenceLayers;
using Newtonsoft.Json;
using WorkRecordPlugin.Mappers;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin
{
	public class WorkRecordImporter
	{
		private readonly PluginProperties _importProperties;

		public WorkRecordImporter(PluginProperties importProperties)
		{
			_importProperties = importProperties;
		}

		public static List<WorkRecordDto> ReadFolder(string folder)
		{
			List<WorkRecordDto> workRecordDtos = new List<WorkRecordDto>();

			// All other json files besides the InfoFile are seen as workRecords
			var workRecords = Directory.EnumerateFiles(folder);
			foreach (var workRecordFile in workRecords)
			{
				using (StreamReader file = File.OpenText(workRecordFile))
				{
					JsonSerializer serializer = new JsonSerializer();
					serializer.TypeNameHandling = TypeNameHandling.Auto;
					try
					{
						WorkRecordDto workRecordDto = (WorkRecordDto)serializer.Deserialize(file, typeof(WorkRecordDto));
						if (workRecordDto != null)
						{
							workRecordDtos.Add(workRecordDto);
						}
					}
					catch (Exception)
					{
						// ToDo: handle JsonReaderExceptions!
					}
				}
			}
			return workRecordDtos;
		}

		public List<ApplicationDataModel> Import(List<WorkRecordDto> workRecordDtos)
		{
			ApplicationDataModel dataModel = InitAdaptDataModel();

			WorkRecordMapper workRecordMapper = new WorkRecordMapper(dataModel, _importProperties);

			foreach (var workRecordDto in workRecordDtos)
			{
				if (workRecordDto.Guid != Guid.Empty)
				{
					workRecordMapper.Map(workRecordDto);
				}
			}

			return new List<ApplicationDataModel>() { dataModel };
		}

		protected ApplicationDataModel InitAdaptDataModel()
		{
			return new ApplicationDataModel
			{
				Catalog = new Catalog(),
				Documents = new Documents(),
				ReferenceLayers = new List<ReferenceLayer>()
			};
		}
	}
}
