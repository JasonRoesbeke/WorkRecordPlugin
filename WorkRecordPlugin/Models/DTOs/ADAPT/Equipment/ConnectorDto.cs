using Newtonsoft.Json;
using System;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class ConnectorDto : BaseDto
	{
		const string Parent = "EquipmentConfigurationId";

		public ConnectorDto() : base(Parent, "EquipmentConfigurationId", "DeviceElementConfiguration")
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid EquipmentConfigurationId { get; set; }

		public HitchPointDto HitchPoint { get; set; }

		public Guid DeviceElementConfiguration { get; set; }

	}
}