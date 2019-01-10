using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceElementUseDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public DeviceElementUseDto() : base(Parent, "TotalDistanceTravelled", "TotalElapsedTime")
		{
			WorkingDatas = new List<WorkingDataDto>();
		}

		[JsonProperty(Required = Required.Always)]
		public int Depth { get; set; }

		[JsonProperty(Required = Required.Always)]
		public int Order { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public Guid DeviceElementConfigurationId { get; set; }

		public NumericRepresentationValueDto TotalDistanceTravelled { get; set; }

		public NumericRepresentationValueDto TotalElapsedTime { get; set; }

		[JsonProperty(Required = Required.Always)]
		public List<WorkingDataDto> WorkingDatas { get; set; }
	}
}