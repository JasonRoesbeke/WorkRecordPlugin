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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceElementDto : DeviceModelDto
	{
		const string Parent = "CompanyId";

		// ToDo: invstigate add jsonResolver.IgnoreProperty(typeof(Person), "Title");
		public DeviceElementDto() : base(Parent)
		{
			ChilderenDeviceElements = new List<DeviceElementDto>();
			//DeviceElementConfigurations = new List<DeviceElementConfigurationDto>();
		}

		[JsonProperty(PropertyName = Parent)]
		public Guid CompanyGuid { get; set; }

		[JsonIgnore]
		public Guid? ParentDeviceElementGuid { get; set; }

		public List<DeviceElementDto> ChilderenDeviceElements { get; set; }

		//public List<DeviceElementConfigurationDto> DeviceElementConfigurations { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Type { get; set; }

		public string Manufacturer { get; set; }

		[JsonIgnore]
		public int ParentReferenceId { get; set; }

		[JsonIgnore]
		public bool IsDeviceElementParent { get; set; }

		public DeviceModelDto DeviceModel { get; set; }
	}
}
