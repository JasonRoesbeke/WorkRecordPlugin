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
using System.Threading;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	class WorkRecordMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _pluginProperties;

		public WorkRecordMapper(ApplicationDataModel dataModel, PluginProperties properties)
		{
			_dataModel = dataModel;
			_pluginProperties = properties;
		}

		#region Export
		//public List<WorkRecordDto> MapWorkRecords()
		//{
		//	// ToDo: check if better null-checking is needed?
		//	if (_dataModel == null)
		//	{
		//		return null;
		//	}
		//	if (_dataModel.Documents == null)
		//	{
		//		return null;
		//	}

		//	List<WorkRecordDto> mappedRecords = new List<WorkRecordDto>();

		//	foreach (WorkRecord workRecord in _dataModel.Documents.WorkRecords)
		//	{
		//		var fieldWorkRecordDto = Map(workRecord);
		//		if (fieldWorkRecordDto != null)
		//		{
		//			mappedRecords.Add(fieldWorkRecordDto);
		//		}
		//	}
		//	return mappedRecords;
		//}

		private WorkRecordDto Map(WorkRecord workRecord)
		{
			if (workRecord == null)
			{
				return null;
			}
			if (!RequestedToBeMapped(workRecord))
			{
				return null;
			}
			
			WorkRecordDto fieldWorkRecordDto = new WorkRecordDto();

			fieldWorkRecordDto.Guid = UniqueIdMapper.GetUniqueGuid(workRecord.Id, UniqueIdSourceCNH);
			if (_pluginProperties.Anonymise)
			{
				fieldWorkRecordDto.Description = "WorkRecord " + workRecord.Id.ReferenceId;
			}
			else
			{
				fieldWorkRecordDto.Description = workRecord.Description;
			}

			SummaryMapper fieldSummaryMapper = new SummaryMapper(_dataModel, _pluginProperties);
			var summaryDto = fieldSummaryMapper.Map(workRecord);
			if (summaryDto == null)
			{
				// Do not map further because summary is required
				return null;
			}

			fieldWorkRecordDto.Summary = summaryDto;

			LoggedDataMapper fieldOperationDataMapper = new LoggedDataMapper(_dataModel, _pluginProperties);
			fieldWorkRecordDto.LoggedData = fieldOperationDataMapper.Map(workRecord, summaryDto);

			return fieldWorkRecordDto;
		}

		private bool RequestedToBeMapped(WorkRecord workRecord)
		{
			// No WorkRecord- or FieldIds given
			if (!_pluginProperties.WorkRecordsToBeExported.Any() &&
			    !_pluginProperties.FieldIdsWithWorkRecordsToBeExported.Any())
			{
				return true;
			}

			// Only WorkRecordIds given
			if (_pluginProperties.WorkRecordsToBeExported.Contains(workRecord.Id.ReferenceId) &&
			    !_pluginProperties.FieldIdsWithWorkRecordsToBeExported.Any())
			{
				return true;
			}
			
			// Only FieldIds given
			if (!_pluginProperties.WorkRecordsToBeExported.Any() &&
			    _pluginProperties.FieldIdsWithWorkRecordsToBeExported.Intersect(workRecord.FieldIds).Any())
			{
				return true;
			}

			// WorkRecord- and FieldIds given
			if (_pluginProperties.WorkRecordsToBeExported.Contains(workRecord.Id.ReferenceId) &&
			    _pluginProperties.FieldIdsWithWorkRecordsToBeExported.Intersect(workRecord.FieldIds).Any())
			{
				return true;
			}

			return false;
		}

		public List<WorkRecordDto> MapAll(List<int> workRecordIds)
		{
			var workRecordDtos = new List<WorkRecordDto>();
			if (_pluginProperties.Anonymise)
			{
				// Randomize the Anonymization values
				AnonymizeUtils.GenerateRandomValues(_pluginProperties);
			};

			foreach (var workRecordId in workRecordIds)
			{
				var workRecordDto = Map(_dataModel.Documents.WorkRecords.FirstOrDefault(wr => wr.Id.ReferenceId.Equals(workRecordId)));
				if (workRecordDto != null)
				{
					workRecordDtos.Add(workRecordDto);
				}
			}

			return workRecordDtos;
		}

		public WorkRecordDto MapSingle(WorkRecord workRecord)
		{
			if (_pluginProperties.Anonymise)
			{
				// Randomize the Anonymization values
				AnonymizeUtils.GenerateRandomValues(_pluginProperties);
			};

			// [Check] if there are requested "WorkRecordsToBeExported"
			if (_pluginProperties.WorkRecordsToBeExported.Any())
			{
				// [Check] if this workRecord is in the list
				if (_pluginProperties.WorkRecordsToBeExported.Contains(workRecord.Id.ReferenceId))
				{
					// Map
					return Map(workRecord);
				}

				// Do not Map
				return null;
			}

			// Map because there is no list of requested "WorkRecordsToBeExported"
			return Map(workRecord);
		}
		#endregion

		#region Import
		public void Map(WorkRecordDto workRecordDto)
		{
			SummaryMapper summaryMapper = new SummaryMapper(_dataModel, _pluginProperties);
			summaryMapper.Map(workRecordDto);
		}

		#endregion

		
	}
}
