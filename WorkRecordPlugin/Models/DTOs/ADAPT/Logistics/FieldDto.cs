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
using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class FieldDto : BaseDto
	{
		const string Parent = "FarmId";

		public FieldDto() : base(Parent, "FieldBoundaries")
		{
			FieldBoundaries = new List<Feature>();
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid FarmGuid { get; set; }

		public List<Feature> FieldBoundaries { get; set; }
	}
}
