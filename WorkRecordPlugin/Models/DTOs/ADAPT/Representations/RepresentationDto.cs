using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public abstract class RepresentationDto : BaseDto
	{
		public RepresentationDto() : base(null, "Description")
		{

		}
		public string CodeSource { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }

		[JsonIgnore]
		public string LongDescription { get; set; }
	}
}
