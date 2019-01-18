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
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Utils;

namespace WorkRecordPlugin.Mappers
{
	public class DeviceElementMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public DeviceElementMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		/// <summary>
		/// First try to find the mapped DeviceElement. If not found, then try to map it.
		/// </summary>
		/// <param name="deviceElement"></param>
		/// <param name="alreadyMappedDeviceElementDtos"></param>
		/// <returns></returns>
		public DeviceElementDto FindOrMapInSummaryDto(DeviceElement deviceElement, SummaryDto summaryDto)
		{
			// Find the dto based on referenceId
			DeviceElementDto deviceElementDto = GetAllDeviceElementDtos(summaryDto.DeviceElements).Where(de => de.ReferenceId == deviceElement.Id.ReferenceId).FirstOrDefault();
			if (deviceElementDto != null)
			{
				return deviceElementDto;
			}

			// Map DeviceElement
			deviceElementDto = Map(deviceElement);

			// if it is a parentDevice, add to SummaryDto. Else find parentDevice and add it to his childCollection
			if (deviceElementDto.IsParent)
			{
				summaryDto.DeviceElements.Add(deviceElementDto);
			}
			else
			{
				var parentDeviceElementDto = GetAllDeviceElementDtos(summaryDto.DeviceElements).Where(de => de.ReferenceId == deviceElement.ParentDeviceId).FirstOrDefault();
				if (parentDeviceElementDto == null)
				{
					// ToDo: when parentDeviceElementDto cannot be found or is not yet mapped
					throw new NullReferenceException();
				}
				deviceElementDto.ParentDeviceElementGuid = parentDeviceElementDto.Guid;
				parentDeviceElementDto.ChilderenDeviceElements.Add(deviceElementDto);
			}

			return deviceElementDto;
		}

		private DeviceElementDto Map(DeviceElement deviceElement)
		{
			// Map
			DeviceElementDto deviceElementDto = mapper.Map<DeviceElement, DeviceElementDto>(deviceElement);
			deviceElementDto.Guid = UniqueIdMapper.GetUniqueId(deviceElement.Id);

			// Manufacturer
			var manufacturer = DataModel.Catalog.Manufacturers.FirstOrDefault(b => b.Id.ReferenceId == deviceElement.ManufacturerId);
			if (manufacturer != null)
			{
				deviceElementDto.Brand = manufacturer.Description;
			}
			
			// Brand
			var brand = DataModel.Catalog.Brands.FirstOrDefault(b => b.Id.ReferenceId == deviceElement.BrandId);
			if (brand != null)
			{
				deviceElementDto.Brand = brand.Description;
			}

			// ToDo: Voyager2 Plugin is not ADAPT 2.0 so DeviceSeries is not mapped!
			//// Series
			//var series = DataModel.Catalog.DeviceSeries.FirstOrDefault(b => b.Id.ReferenceId == deviceElement.BrandId);
			//if (series != null)
			//{
			//	deviceElementDto.Series = series.Description;
			//}

			// ToDo: Do we need to map all ChildrenDeviceElements even if they are not referenced in a deviceElementUse in any of the mapped OperationDatas? Currently not.
			//// Find all ChildrenDeviceElements, map them and add them to this DeviceElement
			//var children = DataModel.Catalog.DeviceElements.Where(de => de.ParentDeviceId == deviceElement.Id.ReferenceId);
			//if (children.Any())
			//{
			//	foreach (var childDeviceElement in children)
			//	{
			//		var childDeviceElementDto = Map(childDeviceElement);
			//		if (childDeviceElementDto != null)
			//		{
			//			deviceElementDto.ChilderenDeviceElements.Add(childDeviceElementDto);
			//		}
			//	}
			//}

			// Parent or not?
			if (deviceElement.ParentDeviceId == 0 && deviceElementDto.ParentDeviceElementGuid == null)
			{
				deviceElementDto.IsParent = true;
			}
			else
			{
				deviceElementDto.ParentReferenceId = deviceElement.ParentDeviceId;
			}
			return deviceElementDto;
		}

		private static List<DeviceElementDto> GetAllDeviceElementDtos(List<DeviceElementDto> deviceElements)
		{
			return ListUtils.GetAllDeviceElementDtos(deviceElements);
		}
	}
}