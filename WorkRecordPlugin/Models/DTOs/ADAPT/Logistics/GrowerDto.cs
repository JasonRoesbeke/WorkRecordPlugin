using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class GrowerDto : BaseDto
	{
		const string Parent = "CompanyId";

		public GrowerDto() : base(Parent, "Farms")
		{
			Farms = new List<FarmDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Name { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid CompanyGuid { get; set; }

		public List<FarmDto> Farms { get; set; }
	}
}
