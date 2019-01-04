using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class FarmDto : BaseDto
	{
		const string Parent = "GrowerId";

		public FarmDto() : base(Parent, "Fields")
		{
			Fields = new List<FieldDto>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid GrowerGuid { get; set; }

		public List<FieldDto> Fields { get; set; }
	}
}
