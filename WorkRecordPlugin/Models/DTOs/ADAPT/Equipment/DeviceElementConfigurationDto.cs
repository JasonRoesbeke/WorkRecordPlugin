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
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public abstract class DeviceElementConfigurationDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public DeviceElementConfigurationDto() : base(Parent , "Offsets")
		{
			Offsets = new List<NumericRepresentationValueDto>();
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always, Order = -1)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }

		[JsonIgnore]
		public int DeviceElementReferenceId { get; set; }

		public List<NumericRepresentationValueDto> Offsets { get; set; }
	}
}