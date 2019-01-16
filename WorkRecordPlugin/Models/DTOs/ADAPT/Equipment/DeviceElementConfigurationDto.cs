using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public abstract class DeviceElementConfigurationDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public DeviceElementConfigurationDto() : base(Parent , "Offsets")
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }

		public List<NumericRepresentationDto> Offsets { get; set; }

	}
}