using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class EndgunTableEntryDto
	{
		public NumericRepresentationValueDto Pressure { get; set; }
		public NumericRepresentationValueDto FlowValue { get; set; }
		public NumericRepresentationValueDto ThrowValue { get; set; }
	}
}
