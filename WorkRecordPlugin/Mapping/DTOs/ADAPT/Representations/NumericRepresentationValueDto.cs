using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public class NumericRepresentationValueDto : BaseDto
	{
		public NumericRepresentationDto Representation { get; set; }
		public NumericValueDto Value { get; set; }
	}
}
