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
using System.Collections.Generic;
using System.Linq;
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
