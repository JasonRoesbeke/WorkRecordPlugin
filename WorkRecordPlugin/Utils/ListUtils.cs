using System.Collections.Generic;
using System.Linq;
using ADAPT.DTOs.Equipment;

namespace WorkRecordPlugin.Utils
{
	public static class ListUtils
	{

		public static List<DeviceElementDto> GetAllDeviceElementDtos(List<DeviceElementDto> deviceElements)
		{
			List<DeviceElementDto> allDeviceElementDtos = new List<DeviceElementDto>();

			foreach (var deviceElement in deviceElements)
			{
				allDeviceElementDtos.Add(deviceElement);
				var children = GetAllDeviceElementDtos(deviceElement.ChildrenDeviceElements);
				if (children.Any())
				{
					allDeviceElementDtos.AddRange(children);
				}
			}

			return allDeviceElementDtos;
		}
	}
}
