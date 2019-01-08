using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class WorkingDataDto : BaseDto
	{
		const string Parent = "DeviceElementUseId";

		public WorkingDataDto() : base(Parent)
		{
		}

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementUseGuid { get; set; }

		public NumericRepresentationDto Representation { get; set; }
	}
}
