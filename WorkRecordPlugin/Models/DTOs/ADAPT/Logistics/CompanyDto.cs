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
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class CompanyDto : BaseDto
	{
		const string Parent = "UserId";

		public CompanyDto() : base(Parent, "Growers", "Vehicles")
		{
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(PropertyName = Parent, Required = Required.Always)]
		public Guid UserGuid { get; set; }

		public List<GrowerDto> Growers { get; set; }

		public List<DeviceElementDto> Vehicles { get; set; }

	}
}
