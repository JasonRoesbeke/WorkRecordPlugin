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
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

namespace WorkRecordPlugin.Mappers
{
	public class DeviceElementConfigurationMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public DeviceElementConfigurationMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public DeviceElementConfigurationDto Map(DeviceElementConfiguration config)
		{
			var deviceElementConfigurationDto = MapDeviceElement(config);
			if (deviceElementConfigurationDto == null)
			{
				return null;
			}
			deviceElementConfigurationDto.Guid = UniqueIdMapper.GetUniqueId(config.Id);
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
			return null;
		}
	}
}