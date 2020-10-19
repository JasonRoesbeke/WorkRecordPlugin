using System;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AutoMapper;
using ADAPT.DTOs.AutoMapperProfiles;
using ADAPT.DTOs.Documents;
using ADAPT.DTOs.Equipment;

namespace WorkRecordPlugin.Mappers
{
	public class EquipmentConfigurationMapper
	{
		private static readonly string UniqueIdSourceCNH = "http://www.cnhindustrial.com";
		private readonly IMapper _mapper;
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _exportProperties;

		public EquipmentConfigurationMapper(ApplicationDataModel dataModel, PluginProperties exportProperties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
			_dataModel = dataModel;
			_exportProperties = exportProperties;
		}

		public EquipmentConfigurationDto Map(EquipmentConfiguration equipmentConfiguration, SummaryDto summaryDto)
		{
			var equipmentConfigurationDto = _mapper.Map<EquipmentConfiguration, EquipmentConfigurationDto>(equipmentConfiguration);
			equipmentConfigurationDto.Guid = UniqueIdMapper.GetUniqueGuid(equipmentConfiguration.Id, UniqueIdSourceCNH);
			if (_exportProperties.Anonymise)
			{
				equipmentConfigurationDto.Description = "EquipmentConfiguration " + equipmentConfiguration.Id.ReferenceId;
			}

			// Connector 1
			equipmentConfigurationDto.Connector1 = MapConnector(equipmentConfiguration.Connector1Id, summaryDto);
			// Connector 2 (if available)
			if (equipmentConfiguration.Connector2Id != null)
			{
				equipmentConfigurationDto.Connector2 = MapConnector((int)equipmentConfiguration.Connector2Id, summaryDto);
			}

			return equipmentConfigurationDto;
		}

		private ConnectorDto MapConnector(int connectorId, SummaryDto summaryDto)
		{
			var connector = _dataModel.Catalog.Connectors.FirstOrDefault(c => c.Id.ReferenceId == connectorId);
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

			var connectorDto = _mapper.Map<Connector, ConnectorDto>(connector);
			if (_exportProperties.Anonymise)
			{
				connectorDto.Description = "Connector " + connector.Id.ReferenceId;
			}
			connectorDto.DeviceElementConfigurationGuid = deviceElementConfigurationDto.Guid;
			if (connector.HitchPointId != 0)
			{
				var hitchPoint = _dataModel.Catalog.HitchPoints.FirstOrDefault(h => h.Id.ReferenceId == connector.HitchPointId);
				if (hitchPoint == null)
				{
					// ToDo: when hitchPoint could not be found in Catalog
					throw new NullReferenceException();
				}
				connectorDto.HitchPoint = _mapper.Map<HitchPoint, HitchPointDto>(hitchPoint);
				//connectorDto.HitchPoint.Guid = UniqueIdMapper.GetUniqueId(hitchPoint.Id);
				if (_exportProperties.Anonymise)
				{
					connectorDto.HitchPoint.Description = "HitchPoint " + hitchPoint.Id.ReferenceId;
				}
			}

			return connectorDto;
		}

		private DeviceElementConfigurationDto MapOrFind(int deviceElementConfigurationId, SummaryDto summaryDto)
		{
			var deviceElementConfig = _dataModel.Catalog.DeviceElementConfigurations.FirstOrDefault(dec => dec.Id.ReferenceId == deviceElementConfigurationId);
			if (deviceElementConfig == null)
			{
				// ToDo: when deviceElementConfig could not be found in Catalog
				throw new NullReferenceException();
			}

			DeviceElementConfigurationMapper deviceElementConfigurationMapper = new DeviceElementConfigurationMapper(_dataModel, _exportProperties);
			return deviceElementConfigurationMapper.FindOrMapInSummaryDto(deviceElementConfig, summaryDto);
		}
	}
}