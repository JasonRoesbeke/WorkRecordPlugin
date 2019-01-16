using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkRecordPlugin.Models.DTOs.ADAPT.Common;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Representations
{
	public class NumericValueDto : BaseDto
	{
		public NumericValueDto() : base(null, "UnitOfMeasure")
		{
		}

		[JsonProperty(DefaultValueHandling = DefaultValueHandling.Include, NullValueHandling = NullValueHandling.Include)]
		public double Value { get; set; }
		public UnitOfMeasureDto UnitOfMeasure { get; set; }
	}
}