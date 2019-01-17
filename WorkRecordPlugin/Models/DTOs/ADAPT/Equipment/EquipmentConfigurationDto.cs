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
	public class EquipmentConfigurationDto : BaseDto
	{
		const string Parent = "OperationDataId";

		public EquipmentConfigurationDto() : base(Parent)
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid OperationDataGuid { get; set; }

		public ConnectorDto Connector1 { get; set; }

		public ConnectorDto Connector2 { get; set; }


	}
}
