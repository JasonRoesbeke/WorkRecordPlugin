using GeoJSON.Net.Feature;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Logistics
{
	public class FieldDto : BaseDto
	{
		const string Parent = "FarmId";

		public FieldDto() : base(Parent, "FieldBoundaries")
		{
			FieldBoundaries = new List<Feature>();
		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid FarmGuid { get; set; }

		public List<Feature> FieldBoundaries { get; set; }
	}
}
