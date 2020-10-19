using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Documents;
using AgGateway.ADAPT.ApplicationDataModel.FieldBoundaries;
using AgGateway.ADAPT.ApplicationDataModel.Logistics;
using AutoMapper;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Documents;
using ADAPT.DTOs.Logistics;

namespace WorkRecordPlugin.Mappers
{
	class SummaryMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly IMapper _mapper;
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _properties;

		public SummaryMapper(ApplicationDataModel dataModel, PluginProperties properties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
			_dataModel = dataModel;
			_properties = properties;
		}


		public SummaryDto Map(WorkRecord workRecord)
		{
			// Grower/Farm/Field/Fieldboundary
			SummaryDto fieldSummaryDto = SetGrowerFarmFieldFieldBoundary(workRecord);

			if (fieldSummaryDto == null)
			{
				return null;
			}

			// EquipmentConfigurations

			// OperationSummaries
			var operationSummaryDtos = MapOperationSummaries(workRecord).ToList();
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
			SummaryDataMapper summaryDataMapper = new SummaryDataMapper(_dataModel);
			List<Summary> summaries = _dataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId).ToList();
			foreach (var summary in summaries)
			{
				var summaryDto = summaryDataMapper.Map(summary);
				summaryDatas.Add(summaryDto);
			}
			return summaryDatas;
		}

		private IEnumerable<OperationSummaryDto> MapOperationSummaries(WorkRecord workRecord)
		{
			List<OperationSummaryDto> operationSummaries = new List<OperationSummaryDto>();
			OperationSummaryMapper operationSummaryMapper = new OperationSummaryMapper(_dataModel);
			IEnumerable<Summary> summaries = _dataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
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

		private SummaryDto SetGrowerFarmFieldFieldBoundary(WorkRecord workRecord)
		{
			int? growerId = workRecord.GrowerId;
			List<int> farmIds = workRecord.FarmIds ?? new List<int>();
			List<int> fieldIds = workRecord.FieldIds ?? new List<int>();

			// [AgGateway] Needed for ISOXML plugin (v2.0.0, ADAPT 1.2.0)
			// Check also in summaries for references to GFF
			var summaries = _dataModel.Documents.Summaries.Where(s => s.WorkRecordId == workRecord.Id.ReferenceId);
			foreach (var summary in summaries)
			{
				if (summary.GrowerId != null)
				{
					if (growerId != (int)summary.GrowerId)
					{
						if (growerId != null)
						{
							// ToDo: handle different growers in 1 workRecord
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

			Grower grower = _dataModel.Catalog.Growers.Find(g => g.Id.ReferenceId == growerId);
			var farms = _dataModel.Catalog.Farms.Where(f => farmIds.Contains(f.Id.ReferenceId)).ToList();
			var fields = _dataModel.Catalog.Fields.Where(f => fieldIds.Contains(f.Id.ReferenceId)).ToList();

			SummaryDto fieldSummaryDto = new SummaryDto();
			fieldSummaryDto.Guid = UniqueIdMapper.GetUniqueGuid(summaries.FirstOrDefault().Id, UniqueIdSourceCNH);

			if (!fields.Any() || !farms.Any() || grower == null)
			{
				// NEED MINIMUM 1 REFERENCE TO A FIELD?
				return fieldSummaryDto;
			}

			// Grower
			GrowerDto growerDto = _mapper.Map<GrowerDto>(grower);
			growerDto.Guid = UniqueIdMapper.GetUniqueGuid(grower.Id, UniqueIdSourceCNH);
			if (_properties.Anonymise)
			{
				growerDto.Name = "Grower " + grower.Id.ReferenceId;
			}
			fieldSummaryDto.Grower = growerDto;


			// Farms
			foreach (var farm in farms)
			{
				// Farm
				FarmDto farmDto = _mapper.Map<FarmDto>(farm);
				farmDto.Guid = UniqueIdMapper.GetUniqueGuid(farm.Id, UniqueIdSourceCNH);
				if (_properties.Anonymise)
				{
					farmDto.Description = "Farm " + farm.Id.ReferenceId;
				}
				growerDto.Farms.Add(farmDto);

				// Fields
				var farmFields = fields.Where(f => f.FarmId == farm.Id.ReferenceId);
				foreach (var field in farmFields)
				{
					// Field
					FieldDto fieldDto = _mapper.Map<FieldDto>(field);
					fieldDto.Guid = UniqueIdMapper.GetUniqueGuid(field.Id, UniqueIdSourceCNH);
					if (_properties.Anonymise)
					{
						fieldDto.Description = "Field " + field.Id.ReferenceId;
					}
					farmDto.Fields.Add(fieldDto);

					// Fieldboundary
					IEnumerable<FieldBoundary> fieldBoundaries = _dataModel.Catalog.FieldBoundaries.Where(f => f.FieldId == field.Id.ReferenceId);
					FieldBoundaryMapper fieldBoundaryMapper = new FieldBoundaryMapper(_properties);
					fieldDto.FieldBoundaries = fieldBoundaryMapper.Map(fieldBoundaries, fieldDto);
				}
			}

			return fieldSummaryDto;
		}


		public void Map(WorkRecordDto workRecordDto)
		{
			//if (workRecordDto.Summary != null)
			//{
			//	if (workRecordDto.Summary.Grower != null)
			//	{
			//		// Grower
			//		GrowerMapper growerMapper = new GrowerMapper(DataModel, Properties);
			//		GrowerDto growerDto = workRecordDto.Summary.Grower;
			//		Grower grower = growerMapper.ImportOrFind(growerDto);
			//		if (grower == null)
			//		{

			//		}
			//		grower = mapper.Map<GrowerDto, Grower>(growerDto);
			//		grower.Id.UniqueIds.Add(UniqueIdMapper.GetUniqueId(growerDto.Guid, Properties.InfoFile));
			//		DataModel.Catalog.Growers.Add(grower);

			//		// Farms
			//		foreach (var farmDto in workRecordDto.Summary.Grower.Farms)
			//		{
			//			Farm farm = mapper.Map<FarmDto, Farm>(farmDto);
			//			farm.Id.UniqueIds.Add(UniqueIdMapper.GetUniqueId(farmDto.Guid, Properties.InfoFile));
			//			farm.GrowerId = grower.Id.ReferenceId;
			//			DataModel.Catalog.Farms.Add(farm);
			//			// Field
			//			foreach (var fieldDto in farmDto.Fields)
			//			{
			//				Field field = mapper.Map<FieldDto, Field>(fieldDto);
			//				field.Id.UniqueIds.Add(UniqueIdMapper.GetUniqueId(fieldDto.Guid, Properties.InfoFile));
			//				field.FarmId = field.Id.ReferenceId;
			//				DataModel.Catalog.Fields.Add(field);

			//				// Fieldboundary
			//				FieldBoundaryMapper fieldBoundaryMapper = new FieldBoundaryMapper(DataModel, Properties);
			//				foreach (var fieldBoundaryDto in fieldDto.FieldBoundaries)
			//				{
			//					FieldBoundary fieldBoundary = fieldBoundaryMapper.Map(fieldBoundaryDto);
			//					fieldBoundary.FieldId = field.Id.ReferenceId;
			//					DataModel.Catalog.FieldBoundaries.Add(fieldBoundary);
			//				}
			//			}
			//		}
			//	}
			//}

			//Persons

			//Products

			//Connector

			//DeviceElements & DeviceModels
			DeviceElementMapper deviceElementMapper = new DeviceElementMapper(_dataModel, _properties);
			//deviceElementMapper.Map(workRecordDto);

			//DeviceElementConfigurations
		}
	}
}