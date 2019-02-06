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

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class LoggedDataDto : BaseDto
	{
		const string Parent = "FieldWorkRecordId";
		public LoggedDataDto() : base(Parent)
		{
			OperationDatas = new List<OperationDataDto>();
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldWorkRecordGuid { get; set; }

		public List<OperationDataDto> OperationDatas { get; set; }

		public string Description { get; set; }
	}
}
