using System;
using System.Collections.Generic;
using System.Linq;
using AgGateway.ADAPT.ApplicationDataModel.ADM;
using AgGateway.ADAPT.ApplicationDataModel.Equipment;
using AutoMapper;
using WorkRecordPlugin.Models.DTOs.ADAPT.AutoMapperProfiles;
using WorkRecordPlugin.Models.DTOs.ADAPT.Documents;
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

		/// <summary>
		/// First try to find the mapped DeviceElement. If not found, then try to map it.
		/// </summary>
		/// <param name="deviceElement"></param>
		/// <param name="alreadyMappedDeviceElementDtos"></param>
		/// <returns></returns>
		public DeviceElementDto FindOrMap(DeviceElement deviceElement, List<DeviceElementDto> alreadyMappedDeviceElementDtos)
		{
			// Find the dto based on referenceId
			DeviceElementDto deviceElementDto = GetAllDeviceElementDtos(alreadyMappedDeviceElementDtos).Where(de => de.ReferenceId == deviceElement.Id.ReferenceId).FirstOrDefault();
			if (deviceElementDto != null)
			{
				return deviceElementDto;
			}

			// Map
			return Map(deviceElement);
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
			return deviceElementDto;
		}

		public static List<DeviceElementDto> GetAllDeviceElementDtos(List<DeviceElementDto> deviceElements)
		{
			List<DeviceElementDto> allDeviceElementDtos = new List<DeviceElementDto>();

			foreach (var deviceElement in deviceElements)
			{
				allDeviceElementDtos.Add(deviceElement);
				var children = GetAllDeviceElementDtos(deviceElement.ChilderenDeviceElements);
				if (children.Any())
				{
					allDeviceElementDtos.AddRange(children);
				}
			}

			return allDeviceElementDtos;
		}
	}
}