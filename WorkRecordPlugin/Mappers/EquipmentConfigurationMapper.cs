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
		private readonly PluginProperties ExportProperties;

		public EquipmentConfigurationMapper(ApplicationDataModel dataModel, PluginProperties exportProperties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
			ExportProperties = exportProperties;
		}

		public EquipmentConfigurationDto Map(EquipmentConfiguration equipmentConfiguration, SummaryDto summaryDto)
		{
			var equipmentConfigurationDto = mapper.Map<EquipmentConfiguration, EquipmentConfigurationDto>(equipmentConfiguration);
			equipmentConfigurationDto.Guid = UniqueIdMapper.GetUniqueId(equipmentConfiguration.Id);
			if (ExportProperties.Anonymized)
			{
				equipmentConfigurationDto.Description = "EquipmentConfiguration " + equipmentConfiguration.Id.ReferenceId;
			}

			// Connector 1
			var connector = DataModel.Catalog.Connectors.FirstOrDefault(c => c.Id.ReferenceId == equipmentConfiguration.Connector1Id);
			equipmentConfigurationDto.Connector1 = MapConnector(equipmentConfiguration.Connector1Id, summaryDto);
			// Connector 2 (if available)
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
			if (connector == null)
			{
				// ToDo: when connector could not be found in Catalog
				throw new NullReferenceException();
			}

			// Map or Find deviceElement and its config
			DeviceElementConfigurationDto deviceElementConfigurationDto = MapOrFind(connector.DeviceElementConfigurationId, summaryDto);
			if (deviceElementConfigurationDto == null)
			{
				// ToDo: when deviceElementConfigurationDto could not be found or mapped
				throw new NullReferenceException();
			}

			var connectorDto = mapper.Map<Connector, ConnectorDto>(connector);
			if (ExportProperties.Anonymized)
			{
				connectorDto.Description = "Connector " + connector.Id.ReferenceId;
			}
			connectorDto.DeviceElementConfigurationGuid = deviceElementConfigurationDto.Guid;
			if (connector.HitchPointId != 0)
			{
				var hitchPoint = DataModel.Catalog.HitchPoints.FirstOrDefault(h => h.Id.ReferenceId == connector.HitchPointId);
				if (hitchPoint == null)
				{
					// ToDo: when hitchPoint could not be found in Catalog
					throw new NullReferenceException();
				}
				connectorDto.HitchPoint = mapper.Map<HitchPoint, HitchPointDto>(hitchPoint);
				//connectorDto.HitchPoint.Guid = UniqueIdMapper.GetUniqueId(hitchPoint.Id);
				if (ExportProperties.Anonymized)
				{
					connectorDto.HitchPoint.Description = "HitchPoint " + hitchPoint.Id.ReferenceId;
				}
			}

			return connectorDto;
		}

		private DeviceElementConfigurationDto MapOrFind(int deviceElementConfigurationId, SummaryDto summaryDto)
		{
			var deviceElementConfig = DataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(dec => dec.Id.ReferenceId == deviceElementConfigurationId);
			if (deviceElementConfig == null)
			{
				// ToDo: when deviceElementConfig could not be found in Catalog
				throw new NullReferenceException();
			}

			DeviceElementConfigurationMapper deviceElementConfigurationMapper = new DeviceElementConfigurationMapper(DataModel, ExportProperties);
			return deviceElementConfigurationMapper.FindOrMapInSummaryDto(deviceElementConfig, summaryDto);
		}
	}
}