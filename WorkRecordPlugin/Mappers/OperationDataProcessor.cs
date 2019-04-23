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
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class OperationDataProcessor
	{
		private readonly IMapper _mapper;
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _exportProperties;
		private readonly SpatialRecordUtils _spatialRecordUtil;

		public OperationDataProcessor(ApplicationDataModel dataModel, PluginProperties exportProperties, SpatialRecordUtils spatialRecordUtil)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
			_dataModel = dataModel;
			_exportProperties = exportProperties;
			_spatialRecordUtil = spatialRecordUtil;
		}

		public void ProcessOperationData(OperationData operationData, SummaryDto summaryDto, OperationDataDto operationDataDto)
		{
			// ToDo: [AgGateway] change to public Func<int,IEnumerable<SpatialRecord>> GetSpatialRecords { get; set; } where int is depth
			var spatialRecords = operationData.GetSpatialRecords().ToList();

			int maximumDepth = -1;
			if (spatialRecords.Any())
			{
				// Requested depth of mapping
				if (_exportProperties.MaximumMappingDepth != null)
				{
					if (_exportProperties.MaximumMappingDepth >= -1 || _exportProperties.MaximumMappingDepth <= operationData.MaxDepth)
					{
						maximumDepth = (int)_exportProperties.MaximumMappingDepth;
					}
				}
				else
				{
					// default is the maximum
					maximumDepth = operationData.MaxDepth;
				}

				// WorkingData per value of depth
				var metersPerDepth = GetMetersPerDepth(operationData, maximumDepth, summaryDto);
				operationDataDto.WorkingDatas = metersPerDepth.ToDictionary(d => d.Key, d => d.Value.Select(kvp => kvp.Value).ToList());

				SpatialRecordMapper spatialRecordMapper = new SpatialRecordMapper(_exportProperties, _spatialRecordUtil);
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

				// Add TimeStamp, Latitude, Longitude & Elevation
				// ToDo: should this be added to each depth?
				allMeters.AddRange(_spatialRecordUtil.GetKvps());

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

						var workingDataDto = _mapper.Map<WorkingData, WorkingDataDto>(workingData);
						workingDataDto.Guid = UniqueIdMapper.GetUniqueGuid(workingData.Id);
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
			DeviceElementConfiguration config = _dataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(c => c.Id.ReferenceId == deviceElementUse.DeviceConfigurationId);
			if (config == null)
			{
				// ToDo: when DeviceElementConfigurations could not be found in Catalog
				throw new NullReferenceException();
			}

			// Check if deviceElementConfiguration is already mapped and added to the summaryDto. If not, map it and and add reference of deviceElement by either finding or mapping deviceElement itself
			DeviceElementConfigurationMapper deviceElementConfigurationMapper = new DeviceElementConfigurationMapper(_dataModel, _exportProperties);
			DeviceElementConfigurationDto deviceElementConfigurationDto = deviceElementConfigurationMapper.FindOrMapInSummaryDto(config, summaryDto);

			return deviceElementConfigurationDto;
		}
	}
}