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
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class EquipmentConfigurationMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public EquipmentConfigurationMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public EquipmentConfigurationDto Map(EquipmentConfiguration equipmentConfiguration, SummaryDto summaryDto)
		{
			var equipmentConfigurationDto = mapper.Map<EquipmentConfiguration, EquipmentConfigurationDto>(equipmentConfiguration);
			equipmentConfigurationDto.Guid = UniqueIdMapper.GetUniqueId(equipmentConfiguration.Id);

			// Find the DeviceElementConfigurations
			var deviceElements = ListUtils.GetAllDeviceElementDtos(summaryDto.DeviceElements);
			var deviceElementConfigs = deviceElements.SelectMany(de => de.DeviceElementConfigurations);
			// Connector 1
			var connector = DataModel.Catalog.Connectors.FirstOrDefault(c => c.Id.ReferenceId == equipmentConfiguration.Connector1Id);
			equipmentConfigurationDto.Connector1 = MapConnector(equipmentConfiguration.Connector1Id, summaryDto);
			// Connector 2
			if (equipmentConfiguration.Connector2Id != null)
			{
				connector = DataModel.Catalog.Connectors.FirstOrDefault(c => c.Id.ReferenceId == equipmentConfiguration.Connector2Id);
				equipmentConfigurationDto.Connector2 = MapConnector((int)equipmentConfiguration.Connector2Id, summaryDto);
			}

			return equipmentConfigurationDto;
		}

		private ConnectorDto MapConnector(int connectorId, SummaryDto summaryDto)
		{
			var connector = DataModel.Catalog.Connectors.FirstOrDefault(c => c.Id.ReferenceId == connectorId);
			var deviceElementDtos = ListUtils.GetAllDeviceElementDtos(summaryDto.DeviceElements);
			var deviceElementDto = deviceElementDtos.FirstOrDefault(de => de.DeviceElementConfigurations.FirstOrDefault(dec => dec.ReferenceId == connector.DeviceElementConfigurationId) != null);

			// Map deviceElement and its config
			DeviceElementConfigurationDto deviceElementConfigurationDto = null;
			if (deviceElementDto == null)
			{
				deviceElementConfigurationDto = Map(connector.DeviceElementConfigurationId, out int deviceElementId);
				if (deviceElementConfigurationDto == null)
				{
					// ToDo: when deviceElementConfigurationDto could not be found or mapped
					throw new NullReferenceException();
				}

				DeviceElement deviceElement = DataModel.Catalog.DeviceElements.FirstOrDefault(de => de.Id.ReferenceId == deviceElementId);
				DeviceElementMapper deviceElementMapper = new DeviceElementMapper(DataModel);
				deviceElementDto = deviceElementMapper.FindOrMapInSummaryDto(deviceElement, summaryDto);
				if (deviceElementDto == null)
				{
					// ToDo: when deviceElementDto could not be found or mapped
					throw new NullReferenceException();
				}
				deviceElementDto.DeviceElementConfigurations.Add(deviceElementConfigurationDto);
			}
			else
			{
				deviceElementConfigurationDto = deviceElementDto.DeviceElementConfigurations.FirstOrDefault(dec => dec.ReferenceId == connector.DeviceElementConfigurationId);
				if (deviceElementConfigurationDto == null)
				{
					deviceElementConfigurationDto = Map(connector.DeviceElementConfigurationId, out int deviceElementId);
					if (deviceElementConfigurationDto == null)
					{
						// ToDo: when deviceElementConfigurationDto could not be found or mapped
						throw new NullReferenceException();
					}
				}
			}
			
			var connectorDto = mapper.Map<Connector, ConnectorDto>(connector);
			connectorDto.DeviceElementConfigurationGuid = deviceElementDto.DeviceElementConfigurations.FirstOrDefault(dec => dec.ReferenceId == connector.DeviceElementConfigurationId).Guid;
			if (connector.HitchPointId != 0)
			{
				var hitchPoint = DataModel.Catalog.HitchPoints.FirstOrDefault(h => h.Id.ReferenceId == connector.HitchPointId);
				if (hitchPoint == null)
				{
					// ToDo: when hitchPoint could not be found in Catalog
					throw new NullReferenceException();
				}
				connectorDto.HitchPoint = mapper.Map<HitchPoint, HitchPointDto>(hitchPoint);
				connectorDto.HitchPoint.Guid = UniqueIdMapper.GetUniqueId(hitchPoint.Id);
			}

			return connectorDto;
		}

		private DeviceElementConfigurationDto Map(int deviceElementConfigurationId, out int deviceElementId)
		{
			var deviceElementConfig = DataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(dec => dec.Id.ReferenceId == deviceElementConfigurationId);
			if (deviceElementConfig == null)
			{
				// ToDo: when deviceElementConfig could not be found in Catalog
				throw new NullReferenceException();
			}
			deviceElementId = deviceElementConfig.DeviceElementId;

			DeviceElementConfigurationMapper deviceElementConfigurationMapper = new DeviceElementConfigurationMapper(DataModel);
			return deviceElementConfigurationMapper.Map(deviceElementConfig);
		}
	}
}