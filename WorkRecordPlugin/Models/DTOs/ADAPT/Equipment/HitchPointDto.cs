using Newtonsoft.Json;
using System;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class HitchPointDto : BaseDto
	{
		const string Parent = "ConnectorId";

		public HitchPointDto() : base(Parent)
		{

		}

		[JsonProperty(PropertyName = EntityId)]
		public Guid Guid { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = Parent)]
		public Guid ConnectorGuid { get; set; }

		public string HitchTypeEnum { get; set; }

		public ReferencePointDto ReferencePoint { get; set; }


	}
}