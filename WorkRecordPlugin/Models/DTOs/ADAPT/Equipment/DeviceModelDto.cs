using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class DeviceModelDto : BaseDto
	{

		public DeviceModelDto(string Parent) : base(Parent, "Brand", "Manufacturer")
		{
		}

		public DeviceModelDto() : base(null, "Brand", "Manufacturer")
		{
		}

		[JsonProperty(PropertyName = EntityId, Order = -2)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always, Order = -1)]
		public string Description { get; set; }

		//[JsonIgnore] // ADAPT 2.0.1
		//public string Manufacturer { get; set; }

		public string Brand { get; set; }

		[JsonIgnore] // ADAPT 2.0.1
		public string Series { get; set; }
	}
}
