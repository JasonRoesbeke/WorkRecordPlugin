﻿/*******************************************************************************
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
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceElementUseDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public DeviceElementUseDto() : base(Parent, "TotalDistanceTravelled", "TotalElapsedTime")
		{
		}

		[JsonProperty(Required = Required.Always)]
		public int Depth { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int Order { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public Guid DeviceElementConfigurationId { get; set; }

		public NumericRepresentationValueDto TotalDistanceTravelled { get; set; }

		public NumericRepresentationValueDto TotalElapsedTime { get; set; }

	}
}