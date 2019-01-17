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

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class FarmDto : BaseDto
	{
		const string Parent = "GrowerId";

		public FarmDto() : base(Parent, "Fields")
		{
			Fields = new List<FieldDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid GrowerGuid { get; set; }

		public List<FieldDto> Fields { get; set; }
	}
}
