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
using System.Data;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class OperationDataDto : BaseDto
	{
		const string Parent = "FieldSummaryDtoId";

		public OperationDataDto() : base(Parent)
		{
			EquipmentConfigurations = new List<EquipmentConfigurationDto>();
			WorkingDatas = new Dictionary<int, List<WorkingDataDto>>();
			SpatialRecords = new Dictionary<int, DataTable>();
		}

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldSummaryGuid { get; set; }

		public string Description { get; set; }
		public string OperationType { get; set; }
		public string Product { get; set; }

		public Dictionary<int, DataTable> SpatialRecords { get; set; }

		//public List<DeviceElementUseDto> DeviceElementUses { get; set; }

		//[JsonIgnore]
		public List<EquipmentConfigurationDto> EquipmentConfigurations { get; set; }

		public Dictionary<int, List<WorkingDataDto>> WorkingDatas { get; set; }

	}
}