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
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceModelDto : BaseDto
	{

		public DeviceModelDto(string Parent) : base(Parent, "Brand", "Manufacturer")
		{
		}

		public DeviceModelDto() : base(null, "Brand", "Manufacturer")
		{
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always, Order = -1)]
		public string Description { get; set; }

		//[JsonIgnore] // ADAPT 2.0.1
		//public string Manufacturer { get; set; }

		public string Brand { get; set; }

		[JsonIgnore] // ADAPT 2.0.1
		public string Series { get; set; }
	}
}
