using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceElementDto : BaseDto
	{
		const string Parent = "CompanyId";

		public DeviceElementDto() : base(Parent, "Brand", "Series")
		{
			DeviceElementUses = new List<DeviceElementUseDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid CompanyGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Type { get; set; }

		public string Brand { get; set; }

		public string Series { get; set; }

		public Guid ParentDeviceElementGuid { get; set; }

		public List<DeviceElementUseDto> DeviceElementUses { get; set; }

		public DeviceElementConfigurationDto DeviceElementConfiguration { get; set; }
	}
}
