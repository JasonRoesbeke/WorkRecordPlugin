using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Documents
{
	public class StampedMeteredValuesDto
	{
		public List<NumericRepresentationValueDto> Values { get; set; }

		public DateTime TimeStamp { get; set; }
	}
}
