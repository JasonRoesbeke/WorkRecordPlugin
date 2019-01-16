using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.LoggedData
{
	public class WorkingDataDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public WorkingDataDto() : base(Parent)
		{
		}

		//[JsonProperty(PropertyName = Parent)]
		//public Guid DeviceElementUseGuid { get; set; }

		[JsonIgnore]
		public int ReferenceId { get; set; }

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		//[JsonProperty(PropertyName = Parent)]
		//public Guid DeviceElementGuid { get; set; }

		//[JsonProperty(Required = Required.Always)]
		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementConfigurationId { get; set; }

		[JsonProperty(Required = Required.Always)]
		public NumericRepresentationDto Representation { get; set; }
	}
}
