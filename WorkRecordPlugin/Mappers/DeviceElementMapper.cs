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
		private readonly IMapper _mapper;
		private readonly ApplicationDataModel _dataModel;
		private readonly PluginProperties _properties;

		public DeviceElementMapper(ApplicationDataModel dataModel, PluginProperties properties)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<WorkRecordDtoProfile>();
			});

			_mapper = config.CreateMapper();
			_dataModel = dataModel;
			_properties = properties;
		}

		/// <summary>
		/// First try to find the mapped DeviceElement. If not found, then try to map it.
		/// </summary>
		/// <param name="deviceElement"></param>
		/// <param name="summaryDto"></param>
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

			// if it is a parentDevice, add to SummaryDto, else find parentDevice and add it to his childCollection
			if (deviceElementDto.IsDeviceElementParent)
			{
				summaryDto.DeviceElements.Add(deviceElementDto);
			}
			else
			{
				var parentDeviceElement = _dataModel.Catalog.DeviceElements.Where(de => de.Id.ReferenceId == deviceElement.ParentDeviceId).FirstOrDefault();
				if (parentDeviceElement == null)
				{
					// ToDo: when parentDeviceElement cannot be found in Catalog
					throw new NullReferenceException();
				}
				//var parentDeviceElementDto = GetAllDeviceElementDtos(summaryDto.DeviceElements).Where(de => de.ReferenceId == deviceElement.ParentDeviceId).FirstOrDefault();
				var parentDeviceElementDto = FindOrMapInSummaryDto(parentDeviceElement, summaryDto);
				if (parentDeviceElementDto == null)
				{
					// ToDo: when parentDeviceElementDto cannot be mapped/found
					throw new NullReferenceException();
				}
				deviceElementDto.ParentDeviceElementGuid = parentDeviceElementDto.Guid;
				parentDeviceElementDto.ChildrenDeviceElements.Add(deviceElementDto);
			}

			return deviceElementDto;
		}

		private DeviceElementDto Map(DeviceElement deviceElement)
		{
			// Map
			DeviceElementDto deviceElementDto = _mapper.Map<DeviceElement, DeviceElementDto>(deviceElement);
			deviceElementDto.Guid = UniqueIdMapper.GetUniqueGuid(deviceElement.Id);

			if (_properties.Anonymise)
			{
				deviceElementDto.Description = "deviceElement " + deviceElement.Id.ReferenceId;
			}

			// Map Baseproperties
			MapDetails(deviceElementDto, deviceElement.BrandId, deviceElement.SeriesId);

			// Manufacturer -> move to MapDetails in ADAPT 2.0!
			var manufacturer = _dataModel.Catalog.Manufacturers.FirstOrDefault(b => b.Id.ReferenceId == deviceElement.ManufacturerId);
			if (manufacturer != null)
			{
				deviceElementDto.Manufacturer = manufacturer.Description;
			}

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
			if (deviceElement.ParentDeviceId == 0)
			{
				// Parent and no reference to a DeviceModel
				deviceElementDto.IsDeviceElementParent = true;
			}
			else
			{
				var parentDeviceModel = _dataModel.Catalog.DeviceModels.FirstOrDefault(dm => dm.Id.ReferenceId == deviceElement.ParentDeviceId);
				if (parentDeviceModel != null)
				{
					// ParentDevice is a DeviceModel
					deviceElementDto.IsDeviceElementParent = true;
					deviceElementDto.DeviceModel = _mapper.Map<DeviceModel, DeviceModelDto>(parentDeviceModel);
					if (_properties.Anonymise)
					{
						deviceElementDto.DeviceModel.Description = "deviceElement " + parentDeviceModel.Id.ReferenceId;
					}
					MapDetails(deviceElementDto.DeviceModel, parentDeviceModel.BrandId, parentDeviceModel.SeriesId);
				}
				else
				{
					// Not a parent
					deviceElementDto.ParentReferenceId = deviceElement.ParentDeviceId;
				}
			}
			return deviceElementDto;
		}

		private void MapDetails(DeviceModelDto deviceModelDto, int brandId, int seriesId)
		{
			// Brand
			var brand = _dataModel.Catalog.Brands.FirstOrDefault(b => b.Id.ReferenceId == brandId);
			if (brand != null)
			{
				deviceModelDto.Brand = brand.Description;
			}

			//// Series
			//var series = DataModel.Catalog.DeviceSeries.FirstOrDefault(b => b.Id.ReferenceId == seriesId);
			//if (series != null)
			//{
			//	deviceElementDto.Series = series.Description;
			//}
		}

		private static List<DeviceElementDto> GetAllDeviceElementDtos(List<DeviceElementDto> deviceElements)
		{
			return ListUtils.GetAllDeviceElementDtos(deviceElements);
		}

		public void Map(WorkRecordDto workRecordDto)
		{
			if (workRecordDto.Summary != null)
			{
				if (workRecordDto.Summary.DeviceElements != null)
				{
					foreach (var deviceElementDto in workRecordDto.Summary.DeviceElements)
					{
						MapAndAddToDataModel(deviceElementDto);
					}
				}
			}
		}

		private void MapAndAddToDataModel(DeviceElementDto deviceElementDto, bool isParentDeviceElement = false)
		{
			DeviceElement deviceElement = _mapper.Map<DeviceElementDto, DeviceElement>(deviceElementDto);
			deviceElement.Id.UniqueIds.Add(UniqueIdMapper.GetUniqueId(deviceElementDto.Guid, _properties.InfoFile));
			_dataModel.Catalog.DeviceElements.Add(deviceElement);

			if (deviceElementDto.DeviceModel != null)
			{
				DeviceModel deviceModel = _mapper.Map<DeviceModelDto, DeviceModel>(deviceElementDto.DeviceModel);
				deviceModel.Id.UniqueIds.Add(UniqueIdMapper.GetUniqueId(deviceElementDto.DeviceModel.Guid, _properties.InfoFile));
				_dataModel.Catalog.DeviceModels.Add(deviceModel);

				// Parent is DeviceModel
				deviceElement.ParentDeviceId = deviceModel.Id.ReferenceId;
			}
			else if (isParentDeviceElement)
			{

			}
		}
	}
}