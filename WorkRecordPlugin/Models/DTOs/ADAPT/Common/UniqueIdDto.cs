using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Common
{
	public class UniqueIdDto
	{
		[JsonProperty(Required = Required.Always)]
		public string Id { get; set; }

		public string IdType { get; set; }

		[JsonProperty(Required = Required.Always)]
		public string Source { get; set; }

		public string SourceType { get; set; }
	}
}
