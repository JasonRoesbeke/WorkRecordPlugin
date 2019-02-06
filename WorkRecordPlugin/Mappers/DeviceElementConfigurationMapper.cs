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

namespace WorkRecordPlugin.Mappers
{
	public class DeviceElementConfigurationMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;
		private readonly ExportProperties ExportProperties;

		public DeviceElementConfigurationMapper(ApplicationDataModel dataModel, ExportProperties exportProperties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
			ExportProperties = exportProperties;
		}

		public DeviceElementConfigurationDto FindOrMapInSummaryDto(DeviceElementConfiguration deviceElementConfig, SummaryDto summaryDto)
		{
			// Find the dto based on referenceId
			DeviceElementConfigurationDto deviceElementConfigurationDto = summaryDto.DeviceElementConfigurations.Where(de => de.ReferenceId == deviceElementConfig.Id.ReferenceId).FirstOrDefault();
			if (deviceElementConfigurationDto != null)
			{
				return deviceElementConfigurationDto;
			}

			// Map DeviceElementConfig
			deviceElementConfigurationDto = Map(deviceElementConfig);
			if (deviceElementConfigurationDto == null)
			{
				// ToDo: when deviceElementConfigurationDto cannot be mapped
				throw new NullReferenceException();
			}

			// Add reference to DeviceElementDto
			DeviceElement deviceElement = DataModel.Catalog.DeviceElements.FirstOrDefault(de => de.Id.ReferenceId == deviceElementConfig.DeviceElementId);
			if (deviceElement == null)
			{
				// ToDo: when deviceElement could not be found in Catalog
				throw new NullReferenceException();
			}
			DeviceElementMapper deviceElementMapper = new DeviceElementMapper(DataModel, ExportProperties);
			DeviceElementDto deviceElementDto = deviceElementMapper.FindOrMapInSummaryDto(deviceElement, summaryDto);
			deviceElementConfigurationDto.DeviceElementGuid = deviceElementDto.Guid;

			// Add to SummaryDto
			summaryDto.DeviceElementConfigurations.Add(deviceElementConfigurationDto);

			return deviceElementConfigurationDto;
		}

		private DeviceElementConfigurationDto Map(DeviceElementConfiguration config)
		{
			var deviceElementConfigurationDto = MapDeviceElement(config);
			if (deviceElementConfigurationDto == null)
			{
				return null;
			}
			deviceElementConfigurationDto.Guid = UniqueIdMapper.GetUniqueId(config.Id);

			if(ExportProperties.Anonymized)
			{
				deviceElementConfigurationDto.Description = "DeviceElementConfiguration " + config.Id.ReferenceId;
			}

			return deviceElementConfigurationDto;
		}

		private DeviceElementConfigurationDto MapDeviceElement(DeviceElementConfiguration config)
		{
			if (config is ImplementConfiguration)
			{
				return mapper.Map<ImplementConfiguration, ImplementConfigurationDto>((ImplementConfiguration)config);
			}
			else if (config is SectionConfiguration)
			{
				return mapper.Map<SectionConfiguration, SectionConfigurationDto>((SectionConfiguration)config);
			}
			else if (config is MachineConfiguration)
			{
				return mapper.Map<MachineConfiguration, MachineConfigurationDto>((MachineConfiguration)config);
			}
			// ToDo: ADAPT 2.0
			//else if (config is EndgunConfiguration)
			//{
			//	return mapper.Map<EndgunConfiguration, EndgunConfigurationDto>((EndgunConfiguration)config);
			//}
			//else if	(config is IrrSectionConfiguration)
			//{
			//	return mapper.Map<IrrSectionConfiguration, IrrSectionConfigurationDto>((IrrSectionConfiguration)config);
			//}
			//else if (config is IrrSystemConfiguration)
			//{
			//	return mapper.Map<IrrSystemConfiguration, IrrSystemConfigurationDto>((IrrSystemConfiguration)config);
			//}
			return null;
		}
	}
}