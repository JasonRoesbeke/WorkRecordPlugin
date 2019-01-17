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
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;
using WorkRecordPlugin.Models.DTOs.ADAPT.Logistics;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class SummaryDto : BaseDto
	{
		const string Parent = "FieldWorkRecordId";

		public SummaryDto() : base(Parent, "Notes")
		{
			SummaryData = new List<StampedMeteredValuesDto>();
			OperationSummaries = new List<OperationSummaryDto>();
			Users = new List<UserDto>();
			DeviceElements = new List<DeviceElementDto>();
			Notes = new List<string>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(PropertyName = Parent)]
		[JsonIgnore]
		public Guid FieldWorkRecordId { get; set; }

		// Company
		public CompanyDto Company { get; set; }

		// Grower/Farm/Field/FieldBoundary tree
		public GrowerDto Grower { get; set; }

		// ToDo: create UserRoleDto
		[JsonProperty(PropertyName = "Persons")]
		public List<UserDto> Users { get; set; }

		public DateTime? EventDate { get; set; }

		public DateTime? EventEndDate { get; set; }

		public List<string> Notes { get; set; }
		// ToDo: create ProductDto

		public List<DeviceElementDto> DeviceElements { get; set; }

		// Change list to Dictonary to have it paired with a depth key value
		public List<DeviceElementConfigurationDto> DeviceElementConfigurations { get; set; }

		public List<StampedMeteredValuesDto> SummaryData { get; set; }

		public List<OperationSummaryDto> OperationSummaries { get; set; }
	}
}
