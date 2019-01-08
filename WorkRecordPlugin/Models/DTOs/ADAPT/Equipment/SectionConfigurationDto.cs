using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	class SectionConfigurationDto
	{
		public NumericRepresentationValueDto SectionWidth { get; set; }

		public NumericRepresentationValueDto LateralOffset { get; set; }

		public NumericRepresentationValueDto InlineOffset { get; set; }
	}
}
