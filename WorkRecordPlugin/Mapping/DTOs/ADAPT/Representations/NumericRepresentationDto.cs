using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public class NumericRepresentationDto : BaseDto
	{
		public NumericRepresentationDto() : base(null, "Description")
		{

		}
		public string CodeSource { get; set; }
		public string Code { get; set; }
		public string Description { get; set; }
	}
}