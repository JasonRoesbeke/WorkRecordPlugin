using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

namespace WorkRecordPlugin.Mappers
{
	public class DeviceElementMapper
	{
		private readonly IMapper mapper;
		private readonly ApplicationDataModel DataModel;

		public DeviceElementMapper(ApplicationDataModel dataModel)
		{
			var config = new MapperConfiguration(cfg => {
				cfg.AddProfile<FieldSummaryProfile>();
			});

			mapper = config.CreateMapper();
			DataModel = dataModel;
		}

		public IEnumerable<DeviceElementDto> FindAndMap()
		{
			List<DeviceElementDto> deviceElementDtos = new List<DeviceElementDto>();

			// Find DeviceElements where ParentId == 0
			var ParentDeviceElements = DataModel.Catalog.DeviceElements.Where(de => de.ParentDeviceId == 0);
			if (!ParentDeviceElements.Any())
			{
				return null;
			}

			foreach (var parentDeviceElement in ParentDeviceElements)
			{
				// Map ParentDevice
				DeviceElementDto parentDeviceElementDto = Map(parentDeviceElement);
				if (parentDeviceElementDto != null)
				{
					parentDeviceElementDto.IsParent = true;
					deviceElementDtos.Add(parentDeviceElementDto);
				}
			}
			return deviceElementDtos;
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

			// Find all ChildrenDeviceElements, map them and add them to this DeviceElement
			var children = DataModel.Catalog.DeviceElements.Where(de => de.ParentDeviceId == deviceElement.Id.ReferenceId);
			if (children.Any())
			{
				foreach (var childDeviceElement in children)
				{
					var childDeviceElementDto = Map(childDeviceElement);
					if (childDeviceElementDto != null)
					{
						deviceElementDto.ChilderenDeviceElements.Add(childDeviceElementDto);
					}
				}
			}
			return deviceElementDto;
		}
	}
}