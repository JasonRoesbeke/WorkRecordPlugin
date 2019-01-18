using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

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
