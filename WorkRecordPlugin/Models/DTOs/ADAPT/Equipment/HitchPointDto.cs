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

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class HitchPointDto : BaseDto
	{
		const string Parent = "ConnectorId";

		public HitchPointDto() : base(Parent)
		{

		}

		//[JsonProperty(PropertyName = EntityId, Order = -2)]
		//public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonIgnore]
		[JsonProperty(PropertyName = Parent)]
		public Guid ConnectorGuid { get; set; }

		public string HitchTypeEnum { get; set; }

		public ReferencePointDto ReferencePoint { get; set; }


	}
}