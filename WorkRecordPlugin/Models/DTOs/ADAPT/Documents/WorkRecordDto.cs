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
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class WorkRecordDto : BaseDto
	{
		const string Parent = "Documents";

		public WorkRecordDto() : base(Parent)
		{
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		public string Description { get; set; }

		public SummaryDto Summary { get; set; }
		public LoggedDataDto LoggedData { get; set; }

	}
}
