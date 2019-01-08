using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Equipment;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class CompanyDto : BaseDto
	{
		const string Parent = "UserId";

		public CompanyDto() : base(Parent, "Growers", "Vehicles")
		{
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(PropertyName = Parent, Required = Required.Always)]
		public Guid UserGuid { get; set; }

		public List<GrowerDto> Growers { get; set; }

		public List<DeviceElementDto> Vehicles { get; set; }

	}
}
