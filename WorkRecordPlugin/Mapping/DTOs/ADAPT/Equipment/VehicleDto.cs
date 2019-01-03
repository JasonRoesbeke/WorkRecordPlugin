using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class VehicleDto : BaseDto
	{
		const string Parent = "CompanyId";

		public VehicleDto() : base(Parent, "Type", "Brand", "Model" )
		{
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid CompanyGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Type { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Brand { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Model { get; set; }
	}
}
