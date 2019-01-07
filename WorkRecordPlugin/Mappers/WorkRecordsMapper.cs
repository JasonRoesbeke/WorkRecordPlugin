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
using System.Collections.Generic;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;

namespace WorkRecordPlugin.Mappers
{
	public class WorkRecordsMapper
	{
		public WorkRecordsMapper()
		{
		}

		public List<WorkRecordDto> MapWorkRecords(ApplicationDataModel dataModel)
		{
			// ToDo: check if better null-checking is needed?
			if (dataModel == null)
			{
				return null;
			}
			if (dataModel.Documents == null)
			{
				return null;
			}

			FieldWorkRecordMapper recordMapper = new FieldWorkRecordMapper(dataModel);
			List<WorkRecordDto> mappedRecords = new List<WorkRecordDto>();

			foreach (WorkRecord workRecord in dataModel.Documents.WorkRecords)
			{
				var fieldWorkRecordDto = recordMapper.Map(workRecord);
				if (fieldWorkRecordDto != null)
				{
					fieldWorkRecordDto.Guid = UniqueIdMapper.GetUniqueId(workRecord.Id);
					fieldWorkRecordDto.Description = workRecord.Description;
					mappedRecords.Add(fieldWorkRecordDto);
				}
			}
			return mappedRecords;

		}
	}
}