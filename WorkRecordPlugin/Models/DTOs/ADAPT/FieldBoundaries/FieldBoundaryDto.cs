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

namespace WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries
{
	public class FieldBoundaryDto : BaseDto
	{
		const string Parent = "FieldId";

		public FieldBoundaryDto() : base(Parent, "IsPassible", "Acres")
		{
		}

		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid FieldGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string SpatialData { get; set; }

		public bool IsPassible { get; set; }

		public float Acres { get; set; }

		public bool Active { get; set; }
	}
}
