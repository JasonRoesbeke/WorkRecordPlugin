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
using System.Linq;
using System.Text;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	class WorkRecordMapper
	{
		private readonly ApplicationDataModel DataModel;
		private readonly ExportProperties ExportProperties;

		public WorkRecordMapper(ApplicationDataModel dataModel, ExportProperties exportProperties)
		{
			DataModel = dataModel;
			ExportProperties = exportProperties;
		}

		public List<WorkRecordDto> MapWorkRecords()
		{
			// ToDo: check if better null-checking is needed?
			if (DataModel == null)
			{
				return null;
			}
			if (DataModel.Documents == null)
			{
				return null;
			}

			List<WorkRecordDto> mappedRecords = new List<WorkRecordDto>();

			foreach (WorkRecord workRecord in DataModel.Documents.WorkRecords)
			{
				var fieldWorkRecordDto = Map(workRecord);
				if (fieldWorkRecordDto != null)
				{
					mappedRecords.Add(fieldWorkRecordDto);
				}
			}
			return mappedRecords;
		}

		public WorkRecordDto Map(WorkRecord workRecord)
		{
			if (ExportProperties.Anonymized)
			{
				// Randomize the Anonymization values for each workRecord
				Random rnd = new Random();
				// ToDo: [IoF2020-WP6] Is a distance between 30 & 80 km enough to be anonymized?
				ExportProperties.RandomDistance = rnd.Next(30000, 80000);
				ExportProperties.RandomBearing = rnd.Next(0, 360);
			}

			WorkRecordDto fieldWorkRecordDto = new WorkRecordDto();

			fieldWorkRecordDto.Guid = UniqueIdMapper.GetUniqueId(workRecord.Id);
			fieldWorkRecordDto.Description = workRecord.Description;

			SummaryMapper fieldSummaryMapper = new SummaryMapper(DataModel, ExportProperties);
			var summaryDto = fieldSummaryMapper.Map(workRecord);
			if (summaryDto == null)
			{
				// Do not map further because summary is required
				return null;
			}

			fieldWorkRecordDto.Summary = summaryDto;

			LoggedDataMapper fieldOperationDataMapper = new LoggedDataMapper(DataModel, ExportProperties);
			fieldWorkRecordDto.LoggedData = fieldOperationDataMapper.Map(workRecord, summaryDto);

			return fieldWorkRecordDto;
		}
	}
}
