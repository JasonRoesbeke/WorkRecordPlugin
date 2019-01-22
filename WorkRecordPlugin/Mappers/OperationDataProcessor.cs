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
using System.Data;
using System.Globalization;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Common;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AgGateway.ADAPT.ApplicationDataModel.Representations;
using AgGateway.ADAPT.ApplicationDataModel.Shapes;
using AgGateway.ADAPT.Representation.RepresentationSystem;
using AgGateway.ADAPT.Representation.RepresentationSystem.ExtensionMethods;
using AgGateway.ADAPT.Representation.UnitSystem;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataProcessor
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private DataTable _dataTable;
		private Dictionary<int, DataTable> _dataTablesPerDepth;

		public OperationDataProcessor(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public void ProcessOperationData(OperationData operationData, SummaryDto summaryDto, OperationDataDto operationDataDto, int maximumDepth = -1)
		{
			// ToDo: [AgGateway] change to public Func<int,IEnumerable<SpatialRecord>> GetSpatialRecords { get; set; } where int is depth
			var spatialRecords = operationData.GetSpatialRecords();

			if (spatialRecords.Any())
			{
				// Requested depth of mapping, default is the maximum
				if (maximumDepth <= -1 || maximumDepth > operationData.MaxDepth)
				{
					maximumDepth = operationData.MaxDepth;
				}
				
				// WorkingData per value of depth
				var metersPerDepth = GetMetersPerDepth(operationData, maximumDepth, summaryDto);
				operationDataDto.WorkingDatas = metersPerDepth.ToDictionary(d => d.Key, d => d.Value.Select(kvp => kvp.Value).ToList());

				SpatialRecordMapper spatialRecordMapper = new SpatialRecordMapper(DataModel);
				operationDataDto.SpatialRecords = spatialRecordMapper.Map(spatialRecords, metersPerDepth, maximumDepth, summaryDto);
			}
		}

		private Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>> GetMetersPerDepth(OperationData operationData, int depth, SummaryDto summaryDto)
		{
			Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>> workingDataWithDepth = new Dictionary<int, List<KeyValuePair<WorkingData, WorkingDataDto>>>();

			for (int i = 0; i <= depth; i++)
			{
				IEnumerable<DeviceElementUse> deviceElementUses = operationData.GetDeviceElementUses(i);

				// Create the list of meters to fill the dataSet.Column
				var allMeters = new List<KeyValuePair<WorkingData, WorkingDataDto>>();

				// DeviceElements & DeviceElementConfigurations
				foreach (var deviceElementUse in deviceElementUses)
				{
					// ToDo: Check when you use IEnumerable instead of List, provides this then less or more performance when serializing to JSON?
					foreach (var workingData in deviceElementUse.GetWorkingDatas())
					{
						if (workingData.Representation == null)
						{
							continue;
						}
						var deviceElementConfigurationDto = MapDeviceElementConfiguration(deviceElementUse, summaryDto);
						if (deviceElementConfigurationDto == null)
						{
							// ToDo: handle deviceElementConfigurationDto == null
							throw new NullReferenceException();
						}

						var workingDataDto = mapper.Map<WorkingData, WorkingDataDto>(workingData);
						workingDataDto.Guid = UniqueIdMapper.GetUniqueId(workingData.Id);
						workingDataDto.DeviceElementConfigurationGuid = deviceElementConfigurationDto.Guid;
						allMeters.Add(new KeyValuePair<WorkingData, WorkingDataDto>(workingData, workingDataDto));
					}
				}
				workingDataWithDepth.Add(i, allMeters);
			}
			return workingDataWithDepth;
		}

		public DeviceElementConfigurationDto MapDeviceElementConfiguration(DeviceElementUse deviceElementUse, SummaryDto summaryDto)
		{
			DeviceElementConfiguration config = DataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(c => c.Id.ReferenceId == deviceElementUse.DeviceConfigurationId);
			if (config == null)
			{
				// ToDo: when DeviceElementConfigurations could not be found in Catalog
				throw new NullReferenceException();
			}

			// Check if deviceElementConfiguration is already mapped and added to the summaryDto. If not, map it and and add reference of deviceElement by either finding or mapping deviceElement itself
			DeviceElementConfigurationMapper deviceElementConfigurationMapper = new DeviceElementConfigurationMapper(DataModel);
			DeviceElementConfigurationDto deviceElementConfigurationDto = deviceElementConfigurationMapper.FindOrMapInSummaryDto(config, summaryDto);

			return deviceElementConfigurationDto;
		}
	}
}