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
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Mappers
{
	class FieldSummaryMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public FieldSummaryMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<FieldSummaryProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}


		public FieldSummaryDto Map(WorkRecord workRecord)
		{
			FieldSummaryDto fieldSummaryDto = SetGFFFB(workRecord);

			if (fieldSummaryDto == null)
			{
				return null;
			}


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

		private FieldSummaryDto SetGFFFB(WorkRecord workRecord)
		{
			FieldSummaryDto fieldSummaryDto = new FieldSummaryDto();

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

			IEnumerable<Summary> summaries = DataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
			SummaryDataMapper summaryDataMapper = new SummaryDataMapper(DataModel);
			OperationSummaryMapper operationSummaryMapper = new OperationSummaryMapper(DataModel);
			foreach (var summary in summaries)
			{
				// StampedMeteredValues
				//var stampedMeteredValues = summaryDataMapper.Map(summary);
				//if (stampedMeteredValues != null)
				//{
				//	fieldSummaryDto.SummaryData.Add(stampedMeteredValues);
				//}

				// OperationData
				var operationSummaries = operationSummaryMapper.Map(summary);
				if (operationSummaries != null)
				{
					fieldSummaryDto.OperationSummaries.AddRange(operationSummaries);
				}
			}

			return fieldSummaryDto;
		}
	}
}