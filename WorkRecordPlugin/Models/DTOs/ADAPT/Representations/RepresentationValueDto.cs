using System;
using System.Collections.Generic;
using System.Text;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public abstract class RepresentationValueDto : BaseDto
	{
		public int? Code { get; set; }

		public string Designator { get; set; }

		public int Color { get; set; }
	}
}
