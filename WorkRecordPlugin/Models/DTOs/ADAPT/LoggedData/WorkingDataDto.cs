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
using AgGateway.ADAPT.ApplicationDataModel.LoggedData;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public abstract class WorkingDataDto : BaseDto
	{
		const string Parent = "DeviceElementConfigurationId";

		public WorkingDataDto() : base(Parent)
		{
		}

		//[JsonProperty(PropertyName = Parent)]
		//public Guid DeviceElementUseGuid { get; set; }

		[JsonIgnore]
		public int ReferenceId { get; set; }

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		//[JsonProperty(PropertyName = Parent)]
		//public Guid DeviceElementGuid { get; set; }

		//[JsonProperty(Required = Required.Always)]
		[JsonProperty(PropertyName = Parent, Order = -1)]
		public Guid DeviceElementConfigurationId { get; set; }

		[JsonProperty(Required = Required.Always, Order = 1)]
		public RepresentationDto Representation { get; set; }
	}
}
