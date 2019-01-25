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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Mappers
{
	class SummaryMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private readonly ExportProperties ExportProperties;

		public SummaryMapper(ApplicationDataModel dataModel, ExportProperties exportProperties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
			ExportProperties = exportProperties;
		}


		public SummaryDto Map(WorkRecord workRecord)
		{
			// Grower/Farm/Field/Fieldboundary
			SummaryDto fieldSummaryDto = SetGFFFB(workRecord);

			if (fieldSummaryDto == null)
			{
				return null;
			}

			// EquipmentConfigurations

			// OperationSummaries
			var operationSummaryDtos = MapOperationSummaries(workRecord);
			if (operationSummaryDtos.Any())
			{
				fieldSummaryDto.OperationSummaries.AddRange(operationSummaryDtos);
			}

			// SummaryDatas
			//var summaryDataDtos = MapSummaryDatas(workRecord);
			//if (summaryDataDtos.Any())
			//{
			//	fieldSummaryDto.SummaryData.AddRange(summaryDataDtos);
			//}

			// Do not map DeviceElements here, only map the DeviceElements referenced in a DeviceElementUse in an OperationData to make sure the file does not contain unnecessary data.
			//List<DeviceElementDto> DeviceElements = MapDeviceElements(DataModel.Catalog);
			//if (DeviceElements.Any())
			//{
			//	fieldSummaryDto.DeviceElements.AddRange(DeviceElements);
			//}

			// TimeScopes
			var startTime = workRecord.TimeScopes.Where(ts => ts.DateContext == DateContextEnum.ActualStart).FirstOrDefault();
			if (startTime != null)
			{
				fieldSummaryDto.EventDate = startTime.TimeStamp1;
			}
			var endTime = workRecord.TimeScopes.Where(ts => ts.DateContext == DateContextEnum.ActualEnd).FirstOrDefault();
			if (endTime != null)
			{
				fieldSummaryDto.EventEndDate = endTime.TimeStamp1;
			}

			return fieldSummaryDto;
		}

		private IEnumerable<OperationSummaryDto> MapSummaryDatas(WorkRecord workRecord)
		{
			List<OperationSummaryDto> summaryDatas = new List<OperationSummaryDto>();
			SummaryDataMapper summaryDataMapper = new SummaryDataMapper(DataModel);
			IEnumerable<Summary> summaries = DataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
			//summaryDatas.AddRange(summaryDataMapper.Map(summaries));
			return summaryDatas;
		}

		private IEnumerable<OperationSummaryDto> MapOperationSummaries(WorkRecord workRecord)
		{
			List<OperationSummaryDto> operationSummaries = new List<OperationSummaryDto>();
			OperationSummaryMapper operationSummaryMapper = new OperationSummaryMapper(DataModel);
			IEnumerable<Summary> summaries = DataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
			foreach (var summary in summaries)
			{
				if (summary.OperationSummaries == null)
				{
					continue;
				}
				// OperationData
				operationSummaries.AddRange(operationSummaryMapper.Map(summary));
			}
			return operationSummaries;
		}

		private SummaryDto SetGFFFB(WorkRecord workRecord)
		{
			int? growerId = workRecord.GrowerId;
			List<int> farmIds = workRecord.FarmIds ?? new List<int>();
			List<int> fieldIds = workRecord.FieldIds ?? new List<int>();

			// [AgGateway] Needed for ISOXML plugin (v2.0.0, ADAPT 1.2.0)
			// Check also in summaries for receferences to GFF
			var summaries = DataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
			foreach (var summary in summaries)
			{
				if (summary.GrowerId != null)
				{
					if (growerId != (int)summary.GrowerId)
					{
						if (growerId != null)
						{
							// Should normally not happen that there are references to different growers in 1 workRecord
							throw new ArgumentException();
						}
						growerId = (int)summary.GrowerId;
					}
				}
				if (summary.FarmId != null)
				{
					if (!farmIds.Contains((int)summary.FarmId))
					{
						farmIds.Add((int)summary.FarmId);
					}
				}
				if (summary.FieldId != null)
				{
					if (!fieldIds.Contains((int)summary.FieldId))
					{
						fieldIds.Add((int)summary.FieldId);
					}
				}
			}

			Grower grower = DataModel.Catalog.Growers.Find(g => g.Id.ReferenceId == growerId);
			var farms = DataModel.Catalog.Farms.Where(f => farmIds.Contains(f.Id.ReferenceId));
			var fields = DataModel.Catalog.Fields.Where(f => fieldIds.Contains(f.Id.ReferenceId));

			SummaryDto fieldSummaryDto = new SummaryDto();

			if (!fields.Any() || !farms.Any() || grower == null)
			{
				// NEED MINIMUM 1 REFERENCE TO A FIELD?
				return fieldSummaryDto;
			}

			// Grower
			GrowerDto growerDto = mapper.Map<GrowerDto>(grower);
			growerDto.Guid = UniqueIdMapper.GetUniqueId(grower.Id);
			fieldSummaryDto.Grower = growerDto;

			// Farms
			foreach (var farm in farms)
			{
				// Farm
				FarmDto farmDto = mapper.Map<FarmDto>(farm);
				farmDto.Guid = UniqueIdMapper.GetUniqueId(farm.Id);
				growerDto.Farms.Add(farmDto);

				// Fields
				var farmFields = fields.Where(f => f.FarmId == farm.Id.ReferenceId);
				foreach (var field in farmFields)
				{
					// Field
					FieldDto fieldDto = mapper.Map<FieldDto>(field);
					fieldDto.Guid = UniqueIdMapper.GetUniqueId(field.Id);
					farmDto.Fields.Add(fieldDto);

					// Fieldboundary
					IEnumerable<FieldBoundary> fieldBoundaries = DataModel.Catalog.FieldBoundaries.Where(f => f.FieldId == field.Id.ReferenceId);
					FieldBoundaryMapper fieldBoundaryMapper = new FieldBoundaryMapper(DataModel, ExportProperties);
					fieldDto.FieldBoundaries = fieldBoundaryMapper.Map(fieldBoundaries, fieldDto);
				}
			}

			return fieldSummaryDto;
		}
	}
}