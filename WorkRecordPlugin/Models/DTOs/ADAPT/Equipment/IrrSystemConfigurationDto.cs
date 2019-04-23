using WorkRecordPlugin.Models.DTOs.ADAPT.Representations;

namespace WorkRecordPlugin.Models.DTOs.ADAPT.Equipment
{
	public class IrrSystemConfigurationDto : DeviceElementConfigurationDto
	{
		public IrrSystemConfigurationDto()
		{
			// ToDo: SectionConfigurationIds = new List<int>();
			// ToDo: EndgunConfigurationIds = new List<int>();
		}
		// ToDo: public Point SystemPosition { get; set; }

		public int? FieldId { get; set; }

		// ToDo: public List<int> SectionConfigurationIds { get; set; }

		// ToDo: public MultiPolygon SpatialFootprint { get; set; }

		// ToDo: public LineString GuidancePath { get; set; }

		public string CornerArmType { get; set; }

		public string FlowDataPedigree { get; set; }

		public string PositionDataPedigree { get; set; }

		public string TimeDataPedigree { get; set; }

		public NumericRepresentationValueDto SystemLength { get; set; }

		// ToDo: public List<int> EndgunConfigurationIds { get; set; }

		public NumericRepresentationValueDto NominalPressure { get; set; }

		public string PressureLocation { get; set; }

		public NumericRepresentationValueDto NominalEfficiency { get; set; }

		public NumericRepresentationValueDto NominalFlow { get; set; }

		public NumericRepresentationValueDto NominalFullCircleTime { get; set; }

		public NumericRepresentationValueDto BearingOffset { get; set; }
	}
}
