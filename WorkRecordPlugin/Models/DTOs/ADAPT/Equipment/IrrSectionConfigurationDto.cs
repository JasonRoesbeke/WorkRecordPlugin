using System;
using System.Collections.Generic;
using System.Text;
using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class IrrSectionConfigurationDto : DeviceElementConfigurationDto
	{
		public string SectionType { get; set; }

		public NumericRepresentationValueDto InnerDistance { get; set; }

		public NumericRepresentationValueDto OuterDistance { get; set; }

		// ToDo: public MultiPolygon SpatialFootprint { get; set; }

		public NumericRepresentationValueDto NominalEfficiency { get; set; }
	}
}
