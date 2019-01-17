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

		public SummaryMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}


		public SummaryDto Map(WorkRecord workRecord)
		{
			SummaryDto fieldSummaryDto = SetGFFFB(workRecord);

			if (fieldSummaryDto == null)
			{
				return null;
			}

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
				// OperationData
				operationSummaries.AddRange(operationSummaryMapper.Map(summary));
			}
			return operationSummaries;
		}

		private SummaryDto SetGFFFB(WorkRecord workRecord)
		{
			SummaryDto fieldSummaryDto = new SummaryDto();

			if (workRecord.FieldIds.Count == 0)
			{
				return null;
			}

			Grower grower = DataModel.Catalog.Growers.Find(g => g.Id.ReferenceId == workRecord.GrowerId);

			List<Farm> farms = new List<Farm>();
			foreach (var farmId in workRecord.FarmIds)
			{
				Farm farm = DataModel.Catalog.Farms.Find(f => f.Id.ReferenceId == farmId);
				if (farm != null)
				{
					if (farm.GrowerId == workRecord.GrowerId)
					{
						farms.Add(farm);
					}
				}
			}

			List<Field> fields = new List<Field>();
			foreach (var fieldId in workRecord.FieldIds)
			{
				Field field = DataModel.Catalog.Fields.Find(f => f.Id.ReferenceId == fieldId);
				if (field != null)
				{
					if (field.GrowerId == workRecord.GrowerId)
					{
						fields.Add(field);
					}
				}
			}

			if (fields.Count == 0 || farms.Count == 0 || grower == null)
			{
				return null;
			}

			// Grower
			GrowerDto growerDto = mapper.Map<GrowerDto>(grower);
			growerDto.Guid = UniqueIdMapper.GetUniqueId(grower.Id);
			fieldSummaryDto.Grower = growerDto;

			foreach (var farm in farms)
			{
				// Farm
				FarmDto farmDto = mapper.Map<FarmDto>(farm);
				farmDto.Guid = UniqueIdMapper.GetUniqueId(farm.Id);
				growerDto.Farms.Add(farmDto);

				// Field
				var field = fields.Find(f => f.FarmId == farm.Id.ReferenceId);
				FieldDto fieldDto = mapper.Map<FieldDto>(field);
				fieldDto.Guid = UniqueIdMapper.GetUniqueId(field.Id);
				farmDto.Fields.Add(fieldDto);

				// Fieldboundary
				IEnumerable<FieldBoundary> fieldBoundaries = DataModel.Catalog.FieldBoundaries.Where(f => f.FieldId == field.Id.ReferenceId);
				FieldBoundaryMapper fieldBoundaryMapper = new FieldBoundaryMapper(DataModel);
				fieldDto.FieldBoundaries = fieldBoundaryMapper.Map(fieldBoundaries, fieldDto);
			}

			

			return fieldSummaryDto;
		}
	}
}