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
			Offsets = new List<NumericRepresentationDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonIgnore]
		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }

		public List<NumericRepresentationDto> Offsets { get; set; }

		[JsonIgnore]
		public int ReferenceId { get; set; }

	}
}