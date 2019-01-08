﻿using Newtonsoft.Json;
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

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		public string Description { get; set; }

		public SummaryDto Summary { get; set; }
		public LoggedDataDto LoggedData { get; set; }

	}
}