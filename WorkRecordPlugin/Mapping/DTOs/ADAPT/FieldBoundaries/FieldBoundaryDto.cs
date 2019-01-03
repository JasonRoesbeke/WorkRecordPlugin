using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.FieldBoundaries
{
	public class FieldBoundaryDto : BaseDto
	{
		const string Parent = "FieldId";

		public FieldBoundaryDto() : base(Parent, "IsPassible", "Acres")
		{
		}

		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid FieldGuid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string SpatialDataUrl { get; set; }

		public bool IsPassible { get; set; }

		public float Acres { get; set; }

		public bool Active { get; set; }
	}
}
