using Newtonsoft.Json;
using System;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public abstract class DeviceElementConfigurationDto : BaseDto
	{
		const string Parent = "DeviceElementId";

		public DeviceElementConfigurationDto() : base(Parent)
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid DeviceElementGuid { get; set; }
	}
}